using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WAHALA.Server.Core
{
    public class ConnectionManager
    {
        private TcpListener? _listener;
        private readonly ConcurrentDictionary<string, TcpClient> _clients = new();
        private readonly CryptoEngine _crypto;

        public event Action<string, string>? OnClientConnected;
        public event Action<string, byte[]>? OnDataReceived;

        public ConnectionManager(string base64Key)
        {
            _crypto = new CryptoEngine(base64Key);
        }

        public void Start(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Task.Run(AcceptLoopAsync);
        }

        private async Task AcceptLoopAsync()
        {
            while (true)
            {
                try
                {
                    var client = await _listener!.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClientAsync(client));
                }
                catch
                {
                    break;
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[4096];
            var clientId = Guid.NewGuid().ToString();

            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    byte[] packet = new byte[bytesRead];
                    Array.Copy(buffer, packet, bytesRead);
                    string handshake = _crypto.Decrypt(packet);
                    _clients.TryAdd(clientId, client);
                    OnClientConnected?.Invoke(clientId, handshake);

                    while (client.Connected)
                    {
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;
                        byte[] incoming = new byte[bytesRead];
                        Array.Copy(buffer, incoming, bytesRead);
                        OnDataReceived?.Invoke(clientId, incoming);
                    }
                }
            }
            catch { }
            finally
            {
                _clients.TryRemove(clientId, out _);
                client.Close();
            }
        }

        public async Task SendToClientAsync(string clientId, string command)
        {
            if (_clients.TryGetValue(clientId, out var client))
            {
                var encrypted = _crypto.Encrypt(command);
                await client.GetStream().WriteAsync(encrypted, 0, encrypted.Length);
            }
        }
    }
}
