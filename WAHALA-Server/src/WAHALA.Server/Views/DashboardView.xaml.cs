using System.Windows.Controls;
using WAHALA.Server.Core;
using WAHALA.Server.ViewModels;

namespace WAHALA.Server.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView(ConnectionManager connectionManager)
        {
            InitializeComponent();
            var vm = new DashboardViewModel(connectionManager);
            DataContext = vm;
            CommandPanelHost.Content = new CommandPanel { DataContext = vm };
        }
    }
}