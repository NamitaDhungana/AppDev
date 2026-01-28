using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JournalApp;

using JournalApp.ViewModels;
using System.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NavigationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;

        if (NavigationListBox.SelectedItem == EntryItem) vm.NavigateToEntryCommand.Execute(null);
        else if (NavigationListBox.SelectedItem == TimelineItem) vm.NavigateToTimelineCommand.Execute(null);
        else if (NavigationListBox.SelectedItem == CalendarItem) vm.NavigateToCalendarCommand.Execute(null);
        else if (NavigationListBox.SelectedItem == DashboardItem) vm.NavigateToAnalyticsCommand.Execute(null);
        else if (NavigationListBox.SelectedItem == SettingsItem) vm.NavigateToSettingsCommand.Execute(null);
    }
}