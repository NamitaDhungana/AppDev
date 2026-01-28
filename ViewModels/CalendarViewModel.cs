using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JournalApp.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JournalApp.ViewModels
{
    public partial class CalendarViewModel : ViewModelBase
    {
        private readonly IJournalRepository _journalRepository;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        [ObservableProperty]
        private ObservableCollection<DateTime> _entryDates = new();

        public CalendarViewModel(IJournalRepository journalRepository)
        {
            _journalRepository = journalRepository;
            LoadDatesCommand.Execute(null);
        }

        [RelayCommand]
        public async Task LoadDates()
        {
            var entries = await _journalRepository.GetAllAsync();
            EntryDates.Clear();
            var dates = entries.Select(e => e.Date.Date).Distinct().OrderByDescending(d => d);
            foreach (var date in dates)
            {
                EntryDates.Add(date);
            }
        }

        [RelayCommand]
        private void GoToDateEntry()
        {
            // Logic to navigate to EntryView with specific date
            var app = (App)System.Windows.Application.Current;
            var entryVm = app.Services.GetService(typeof(EntryViewModel)) as EntryViewModel;
            var mainVm = app.Services.GetService(typeof(MainViewModel)) as MainViewModel;
            
            if (entryVm != null && mainVm != null)
            {
                entryVm.LoadDateCommand.Execute(SelectedDate);
                mainVm.NavigateTo(entryVm);
            }
        }
    }
}
