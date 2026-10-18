using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WAHALA.Server.Core;
using WAHALA.Server.Models;

namespace WAHALA.Server.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly ConnectionManager _connectionManager;
        private ClientNode? _selectedClient;

        public ObservableCollection<ClientNode> Clients { get; } = new();
        public ObservableCollection<string> AvailableModules { get; } = new();

        public ClientNode? SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged();
                UpdateModules();
            }
        }

        public DashboardViewModel(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _connectionManager.OnClientConnected += HandleClientConnected;
            _connectionManager.OnDataReceived += HandleData;
        }

        private void HandleClientConnected(string clientId, string handshake)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var node = CommandDispatcher.ParseHandshake(clientId, "unknown", handshake);
                Clients.Add(node);
            });
        }

        private void HandleData(string clientId, byte[] data)
        {
            var node = Clients.FirstOrDefault(c => c.ClientId == clientId);
            if (node != null)
            {
                App.Current.Dispatcher.Invoke(() => node.LastSeen = DateTime.Now.ToString("HH:mm:ss"));
            }
        }

        private void UpdateModules()
        {
            AvailableModules.Clear();
            if (SelectedClient == null) return;
            foreach (var m in CommandDispatcher.GetModulesForPlatform(SelectedClient.Platform))
                AvailableModules.Add(m);
        }

        public async Task SendCommandAsync(string module, string action, string payload)
        {
            if (SelectedClient == null) return;
            string json = CommandDispatcher.BuildCommand(module, action, payload);
            await _connectionManager.SendToClientAsync(SelectedClient.ClientId, json);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}