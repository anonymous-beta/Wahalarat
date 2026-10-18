using System.Collections.ObjectModel;
using WAHALA.Server.Models;

namespace WAHALA.Server.ViewModels
{
    public class DashboardViewModel
    {
        public ObservableCollection<ClientNode> Clients { get; } = new();

        public DashboardViewModel()
        {
            // placeholder initial data for demonstration
            Clients.Add(new ClientNode
            {
                ClientId = "demo-001",
                IPAddress = "192.168.1.10",
                Country = "NG",
                Platform = PlatformType.Windows,
                OSVersion = "Windows 11 23H2",
                IsAdminOrRooted = true,
                LastSeen = "just now"
            });
        }
    }
}
