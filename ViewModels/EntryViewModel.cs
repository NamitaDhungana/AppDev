using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JournalApp.Interfaces;
using JournalApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JournalApp.ViewModels
{
    public partial class EntryViewModel : ViewModelBase
    {
        private readonly IJournalRepository _journalRepository;
        private JournalEntry? _currentEntry;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _content = string.Empty;


        [ObservableProperty]
        private DateTime _date = DateTime.Now;

        [ObservableProperty]
        private string _primaryMood = "Neutral";

        [ObservableProperty]
        private string _secondaryMood1 = string.Empty;

        [ObservableProperty]
        private string _secondaryMood2 = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _tagsInput = string.Empty;

        [ObservableProperty]
        private bool _isExistingEntry;

        public ObservableCollection<string> PositiveMoods { get; } = new()
        {
            "😊 Happy", "🤩 Excited", "💪 Confident", "🙏 Grateful", "🤩 Productive", "🧘 Calm"
        };

        public ObservableCollection<string> NeutralMoods { get; } = new()
        {
            "😐 Neutral", "🤔 Thoughtful", "🥱 Bored", "😴 Tired", "🧐 Curious"
        };

        public ObservableCollection<string> NegativeMoods { get; } = new()
        {
            "😢 Sad", "😫 Stressed", "😨 Anxious", "😡 Angry", "😔 Lonely", "👎 Bad"
        };

        public ObservableCollection<string> QuickTags { get; } = new()
        {
            "Work", "Personal", "Health", "Travel", "Hobby", "Social", "Finance", 
            "Family", "Study", "Food", "Exercise", "Sleep", "Self-Care"
        };

        [RelayCommand]
        private void AddQuickTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(TagsInput))
                TagsInput = tag;
            else
            {
                var existing = TagsInput.Split(',').Select(t => t.Trim()).ToList();
                if (!existing.Contains(tag))
                {
                    existing.Add(tag);
                    TagsInput = string.Join(", ", existing);
                }
            }
        }

        public EntryViewModel(IJournalRepository journalRepository)
        {
            _journalRepository = journalRepository;
        }

        [RelayCommand]
        public async Task LoadDate(DateTime date)
        {
            IsBusy = true;
            try
            {
                Date = date;
                var existingEntry = await _journalRepository.GetByDateAsync(date);

                if (existingEntry != null)
                {
                    _currentEntry = existingEntry;
                    Title = existingEntry.Title;
                    Content = existingEntry.Content;
                    PrimaryMood = existingEntry.PrimaryMood;
                    SecondaryMood1 = existingEntry.SecondaryMood1 ?? string.Empty;
                    SecondaryMood2 = existingEntry.SecondaryMood2 ?? string.Empty;
                    TagsInput = string.Join(", ", existingEntry.Tags.Select(t => t.Name));
                    IsExistingEntry = true;
                }
                else
                {
                    _currentEntry = new JournalEntry { Date = date };
                    Title = string.Empty;
                    Content = string.Empty;
                    PrimaryMood = "Neutral";
                    SecondaryMood1 = string.Empty;
                    SecondaryMood2 = string.Empty;
                    TagsInput = string.Empty;
                    IsExistingEntry = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading: {ex.Message}");
            }
            finally
            {
                 IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadTodayEntry()
        {
             await LoadDate(DateTime.Now.Date);
        }

        [RelayCommand]
        private async Task SaveOrUpdate()
        {
            if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Content))
            {
                MessageBox.Show("Cannot save empty entry.");
                return;
            }

            IsBusy = true;
            try
            {
                if (_currentEntry == null) _currentEntry = new JournalEntry { Date = Date };

                // Enforce one entry per day if it's a new entry (Id == 0)
                if (_currentEntry.Id == 0)
                {
                    var existing = await _journalRepository.GetByDateAsync(Date.Date);
                    if (existing != null)
                    {
                        MessageBox.Show($"An entry for {Date:D} already exists. Please edit the existing entry instead of creating a new one.", "Duplicate Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                _currentEntry.Title = Title;
                _currentEntry.Content = Content;
                _currentEntry.PrimaryMood = PrimaryMood;
                _currentEntry.SecondaryMood1 = string.IsNullOrWhiteSpace(SecondaryMood1) ? null : SecondaryMood1;
                _currentEntry.SecondaryMood2 = string.IsNullOrWhiteSpace(SecondaryMood2) ? null : SecondaryMood2;
                _currentEntry.UpdatedAt = DateTime.Now;

                // Handle Tags
                _currentEntry.Tags.Clear();
                if (!string.IsNullOrWhiteSpace(TagsInput))
                {
                    var tagNames = TagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                            .Select(t => t.Trim())
                                            .Where(t => !string.IsNullOrEmpty(t))
                                            .Distinct();
                    
                    foreach (var tagName in tagNames)
                    {
                        var tag = await _journalRepository.GetOrCreateTagAsync(tagName);
                        _currentEntry.Tags.Add(tag);
                    }
                }

                if (_currentEntry.Id == 0)
                {
                    _currentEntry.CreatedAt = DateTime.Now;
                    await _journalRepository.AddAsync(_currentEntry);
                }
                else
                {
                    await _journalRepository.UpdateAsync(_currentEntry);
                }

                MessageBox.Show("Entry saved successfully!");
            }
            catch (Exception ex)
            {
                 MessageBox.Show($"Error saving entry: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void CreateNew()
        {
            _currentEntry = new JournalEntry { Date = DateTime.Now };
            Title = string.Empty;
            Content = string.Empty;
            PrimaryMood = "😊 Happy";
            SecondaryMood1 = "😐 Neutral";
            SecondaryMood2 = "😔 Lonely";
            TagsInput = string.Empty;
            Date = DateTime.Now;
        }

        [RelayCommand]
        private async Task Delete()
        {
            if (_currentEntry == null || _currentEntry.Id == 0) return;

            var result = MessageBox.Show("Are you sure you want to delete this entry?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await _journalRepository.DeleteAsync(_currentEntry.Id);
                _currentEntry = null;
                // Reset UI
                Title = string.Empty;
                Content = string.Empty;
                PrimaryMood = "Neutral";
                MessageBox.Show("Entry deleted.");
            }
        }
    }
}
