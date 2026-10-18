using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WAHALA.Server.Models
{
    public enum PlatformType { Windows, Android, Unknown }

    public class ClientNode : INotifyPropertyChanged
    {
        private string _lastSeen = "just now";
        private bool _isOnline = true;

        public string ClientId { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public PlatformType Platform { get; set; } = PlatformType.Unknown;
        public string OSVersion { get; set; } = string.Empty;
        public bool IsAdminOrRooted { get; set; }

        public string LastSeen
        {
            get => _lastSeen;
            set { _lastSeen = value; OnPropertyChanged(); }
        }

        public bool IsOnline
        {
            get => _isOnline;
            set { _isOnline = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}