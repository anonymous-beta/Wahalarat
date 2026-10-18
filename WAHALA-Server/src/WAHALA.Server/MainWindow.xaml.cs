using System.Windows;
using System.Windows.Controls;
using WAHALA.Server.Views;

namespace WAHALA.Server
{
    public partial class MainWindow : Window
    {
        private DashboardView _dashboardView;
        private BuilderView _builderView;
        private SettingsView _settingsView;

        public MainWindow()
        {
            InitializeComponent();
            _dashboardView = new DashboardView();
            _builderView = new BuilderView();
            _settingsView = new SettingsView();
            MainContent.Content = _dashboardView;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _dashboardView;
        }

        private void BtnBuilder_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _builderView;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _settingsView;
        }
    }
}
