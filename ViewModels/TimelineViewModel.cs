using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JournalApp.Interfaces;
using JournalApp.Models;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace JournalApp.ViewModels
{
    public partial class TimelineViewModel : ViewModelBase
    {
        private readonly IJournalRepository _journalRepository;

        [ObservableProperty]
        private ObservableCollection<JournalEntry> _entries = new();

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private bool _canLoadMore = true;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private DateTime? _startDate;

        [ObservableProperty]
        private DateTime? _endDate;

        [ObservableProperty]
        private string? _selectedMood;

        public ObservableCollection<string> AllMoods { get; } = new()
        {
            "All", "😊 Happy", "🤩 Excited", "💪 Confident", "🙏 Grateful", "🤩 Productive", "🧘 Calm",
            "😐 Neutral", "🤔 Thoughtful", "🥱 Bored", "😴 Tired", "🧐 Curious",
            "😢 Sad", "😫 Stressed", "😨 Anxious", "😡 Angry", "😔 Lonely", "👎 Bad"
        };

        public TimelineViewModel(IJournalRepository journalRepository)
        {
            _journalRepository = journalRepository;
            _selectedMood = "All";
            LoadEntriesCommand.Execute(null);
        }

        [RelayCommand]
        private async Task Search()
        {
            string? moodFilter = SelectedMood == "All" ? null : SelectedMood;
            
            var results = await _journalRepository.FilterAsync(StartDate, EndDate, moodFilter, null, SearchText);
            
            Entries.Clear();
            foreach (var r in results) Entries.Add(r);

            // When searching/filtering, we disable LoadMore for now as it's not paginated in the repository yet
            CanLoadMore = false; 
        }

        partial void OnSearchTextChanged(string value)
        {
            SearchCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadEntries()
        {
            SearchText = string.Empty;
            StartDate = null;
            EndDate = null;
            SelectedMood = "All";
            
            CurrentPage = 1;
            Entries.Clear();
            await LoadPage();
        }

        partial void OnSelectedMoodChanged(string? value) => SearchCommand.Execute(null);
        partial void OnStartDateChanged(DateTime? value) => SearchCommand.Execute(null);
        partial void OnEndDateChanged(DateTime? value) => SearchCommand.Execute(null);

        [RelayCommand]
        private async Task LoadMore()
        {
            if (!CanLoadMore) return;
            CurrentPage++;
            await LoadPage();
        }

        private async Task LoadPage()
        {
            var all = await _journalRepository.GetAllAsync();
            var page = all.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            
            foreach (var entry in page)
            {
                Entries.Add(entry);
            }

            CanLoadMore = page.Count == PageSize;
        }
    }
}
