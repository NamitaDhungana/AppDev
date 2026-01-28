using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JournalApp.Interfaces;

namespace JournalApp.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IJournalRepository _journalRepository;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public LoginViewModel(IJournalRepository journalRepository)
        {
            _journalRepository = journalRepository;
        }

        [RelayCommand]
        private async Task Login()
        {
            var settings = await _journalRepository.GetUserSettingsAsync();
            
            if (Password == settings.Pin) 
            {
                 ErrorMessage = "";
                 var app = (App)System.Windows.Application.Current;
                 var mainViewModel = app.Services.GetService(typeof(MainViewModel)) as MainViewModel;
                 var entryViewModel = app.Services.GetService(typeof(EntryViewModel)) as EntryViewModel;
                 
                 if (mainViewModel != null && entryViewModel != null)
                 {
                     // Use Task.Run to avoid blocking UI during database check
                     await Task.Run(async () => {
                         await entryViewModel.LoadTodayEntryCommand.ExecuteAsync(null);
                         
                         System.Windows.Application.Current.Dispatcher.Invoke(() => {
                            mainViewModel.NavigateTo(entryViewModel);
                         });
                     });
                 }
            }
            else
            {
                ErrorMessage = "Invalid Password";
            }
        }
    }
}
