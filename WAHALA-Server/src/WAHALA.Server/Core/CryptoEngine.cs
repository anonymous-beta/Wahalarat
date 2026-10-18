using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WAHALA.Server.Core
{
    public class CryptoEngine
    {
        private readonly byte[] _key;
        private const int NonceSize = 12;
        private const int TagSize = 16;

        public CryptoEngine(string base64Key)
        {
            _key = Convert.FromBase64String(base64Key);
            if (_key.Length != 32) throw new ArgumentException("Key must be 256 bits (32 bytes).");
        }

        public byte[] Encrypt(string plaintext)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);

            using var cipher = new AesGcm(_key);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[TagSize];

            cipher.Encrypt(nonce, plaintextBytes, ciphertext, tag);

            using var ms = new MemoryStream();
            ms.Write(nonce, 0, nonce.Length);
            ms.Write(tag, 0, tag.Length);
            ms.Write(ciphertext, 0, ciphertext.Length);
            return ms.ToArray();
        }

        public string Decrypt(byte[] packet)
        {
            if (packet.Length < NonceSize + TagSize) throw new CryptographicException("Invalid packet size.");

            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] ciphertext = new byte[packet.Length - NonceSize - TagSize];

            Array.Copy(packet, 0, nonce, 0, NonceSize);
            Array.Copy(packet, NonceSize, tag, 0, TagSize);
            Array.Copy(packet, NonceSize + TagSize, ciphertext, 0, ciphertext.Length);

            using var cipher = new AesGcm(_key);
            byte[] plaintext = new byte[ciphertext.Length];
            cipher.Decrypt(nonce, ciphertext, tag, plaintext);
            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
