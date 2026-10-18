using System.Windows;
using System.Windows.Controls;
using WAHALA.Server.ViewModels;

namespace WAHALA.Server.Views
{
    public partial class CommandPanel : UserControl
    {
        public CommandPanel()
        {
            InitializeComponent();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not DashboardViewModel vm) return;
            if (vm.SelectedClient == null)
            {
                StatusText.Text = "No client selected.";
                return;
            }

            string module = ModuleCombo.SelectedItem as string ?? "";
            string action = ActionBox.Text.Trim();
            string payload = PayloadBox.Text;

            if (string.IsNullOrEmpty(module) || string.IsNullOrEmpty(action))
            {
                StatusText.Text = "Module and Action are required.";
                return;
            }

            try
            {
                await vm.SendCommandAsync(module, action, payload);
                StatusText.Text = $"Sent: {module} / {action}";
            }
            catch (System.Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }
    }
}