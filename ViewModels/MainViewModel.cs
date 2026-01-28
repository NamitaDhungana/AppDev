using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace JournalApp.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentViewModel;

        public MainViewModel(LoginViewModel loginViewModel)
        {
            CurrentViewModel = loginViewModel;
        }

        [RelayCommand]
        public void NavigateToEntry()
        {
            var app = (App)System.Windows.Application.Current;
            if (app.Services.GetService(typeof(EntryViewModel)) is ViewModelBase vm)
                CurrentViewModel = vm;
        }

        [RelayCommand]
        public void NavigateToTimeline()
        {
            var app = (App)System.Windows.Application.Current;
            if (app.Services.GetService(typeof(TimelineViewModel)) is ViewModelBase vm)
                CurrentViewModel = vm;
        }

        [RelayCommand]
        public void NavigateToAnalytics()
        {
            var app = (App)System.Windows.Application.Current;
            if (app.Services.GetService(typeof(DashboardViewModel)) is DashboardViewModel vm)
            {
                CurrentViewModel = vm;
            }
        }

        [RelayCommand]
        public void NavigateToCalendar()
        {
            var app = (App)System.Windows.Application.Current;
            if (app.Services.GetService(typeof(CalendarViewModel)) is CalendarViewModel vm)
            {
                vm.LoadDatesCommand.Execute(null);
                CurrentViewModel = vm;
            }
        }

        [RelayCommand]
        public void NavigateToSettings()
        {
            var app = (App)System.Windows.Application.Current;
            if (app.Services.GetService(typeof(SettingsViewModel)) is ViewModelBase vm)
                CurrentViewModel = vm;
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }
}
