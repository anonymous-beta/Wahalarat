using System;
using System.Windows;
using WAHALA.Server.Core;
using WAHALA.Server.Views;

namespace WAHALA.Server
{
    public partial class MainWindow : Window
    {
        private DashboardView _dashboardView;
        private BuilderView _builderView;
        private SettingsView _settingsView;
        private ConnectionManager _connectionManager;

        public MainWindow()
        {
            InitializeComponent();
            
            // Generate a session key, start listener
            string base64Key;
            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                base64Key = Convert.ToBase64String(aes.Key);
            }
            
            _connectionManager = new ConnectionManager(base64Key);
            _connectionManager.Start(4444);

            _dashboardView = new DashboardView(_connectionManager);
            _builderView = new BuilderView();
            _settingsView = new SettingsView();
            MainContent.Content = _dashboardView;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e) => MainContent.Content = _dashboardView;
        private void BtnBuilder_Click(object sender, RoutedEventArgs e) => MainContent.Content = _builderView;
        private void BtnSettings_Click(object sender, RoutedEventArgs e) => MainContent.Content = _settingsView;
    }
}