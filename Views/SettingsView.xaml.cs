using System.Windows.Controls;
using JournalApp.ViewModels;

namespace JournalApp.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void NewPinBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm)
            {
                vm.NewPin = ((PasswordBox)sender).Password;
            }
        }
    }
}
