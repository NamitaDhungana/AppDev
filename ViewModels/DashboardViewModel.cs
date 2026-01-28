using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JournalApp.Services;
using JournalApp.Interfaces;
using JournalApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace JournalApp.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly IAnalyticsService _analyticsService;

        [ObservableProperty]
        private ISeries[] _activitySeries = Array.Empty<ISeries>();

        [ObservableProperty]
        private Axis[] _activityXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private ISeries[] _moodSeries = Array.Empty<ISeries>();

        [ObservableProperty]
        private int _currentStreak;

        [ObservableProperty]
        private int _longestStreak;

        [ObservableProperty]
        private int _avgWords;

        [ObservableProperty]
        private Dictionary<string, int> _moodDistribution = new();

        [ObservableProperty]
        private Dictionary<string, int> _mostUsedTags = new();

        [ObservableProperty]
        private DateTime? _exportStartDate;

        [ObservableProperty]
        private DateTime? _exportEndDate;

        [ObservableProperty]
        private Dictionary<DateTime, int> _wordCountTrends = new();

        [ObservableProperty]
        private string _mostFrequentMood = "None";

        [ObservableProperty]
        private string _leastFrequentMood = "None";

        [ObservableProperty]
        private string _quickTitle = string.Empty;

        [ObservableProperty]
        private string _quickContent = string.Empty;


        [ObservableProperty]
        private bool _isBusy;

        public DashboardViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            LoadStatsCommand.Execute(null);
            LoadTodayForQuickEntryCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadStats()
        {
            await Task.Run(async () => {
                var cs = await _analyticsService.GetCurrentStreakAsync();
                var ls = await _analyticsService.GetLongestStreakAsync();
                var aw = await _analyticsService.GetAverageWordCountAsync();
                var md = await _analyticsService.GetMoodDistributionAsync();
                var mt = await _analyticsService.GetMostUsedTagsAsync();
                var wc = await _analyticsService.GetWordCountTrendsAsync();
                var compare = await _analyticsService.GetMoodComparisonAsync();

                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    CurrentStreak = cs;
                    LongestStreak = ls;
                    AvgWords = aw;
                    MoodDistribution = md;
                    MostUsedTags = mt;
                    WordCountTrends = wc;
                    MostFrequentMood = compare.most;
                    LeastFrequentMood = compare.least;

                    // Charts Data
                    if (wc.Count > 0)
                    {
                        var values = wc.Values.ToArray();
                        var dates = wc.Keys.Select(k => k.ToString("MM/dd")).ToArray();

                        ActivitySeries = new ISeries[]
                        {
                            new ColumnSeries<int> { Values = values, Name = "Words", Fill = new SolidColorPaint(SKColors.CornflowerBlue) }
                        };

                        ActivityXAxes = new Axis[]
                        {
                            new Axis { Labels = dates, LabelsRotation = 0 }
                        };
                    }

                    if (md.Count > 0)
                    {
                        MoodSeries = md.Select(kvp => new PieSeries<int>
                        {
                            Values = new int[] { kvp.Value },
                            Name = kvp.Key,
                            InnerRadius = 50
                        }).ToArray();
                    }
                });
            });
        }

        [RelayCommand]
        private async Task ExportPdf()
        {
            var app = (App)System.Windows.Application.Current;
            var repo = app.Services.GetService(typeof(IJournalRepository)) as IJournalRepository;
            var exporter = app.Services.GetService(typeof(IExportService)) as IExportService;
            
            if (repo != null && exporter != null)
            {
                var entries = await repo.FilterAsync(ExportStartDate, ExportEndDate, null, null);
                
                string dateRange = (ExportStartDate.HasValue || ExportEndDate.HasValue) ? "_filtered" : "";
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"MyJournalExport{dateRange}.pdf");
                
                await exporter.ExportToPdfAsync(path, entries);
                System.Windows.MessageBox.Show($"Exported to {path}");
            }
        }
        [RelayCommand]
        private async Task SaveQuickEntry()
        {
            if (string.IsNullOrWhiteSpace(QuickTitle) && string.IsNullOrWhiteSpace(QuickContent))
            {
                MessageBox.Show("Cannot save empty entry.");
                return;
            }

            var app = (App)System.Windows.Application.Current;
            var repo = app.Services.GetService(typeof(IJournalRepository)) as IJournalRepository;
            
            if (repo == null) return;

            IsBusy = true;
            try
            {
                var today = DateTime.Now.Date;
                var existing = await repo.GetByDateAsync(today);

                if (existing != null)
                {
                    existing.Title = QuickTitle;
                    existing.Content = QuickContent;
                    existing.UpdatedAt = DateTime.Now;
                    await repo.UpdateAsync(existing);
                }
                else
                {
                    var entry = new JournalEntry
                    {
                        Title = QuickTitle,
                        Content = QuickContent,
                        Date = today,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        PrimaryMood = "😊 Happy" // Default for quick entry
                    };
                    await repo.AddAsync(entry);
                }

                MessageBox.Show("Today's entry saved from Dashboard!");
                await LoadStats(); // Refresh word counts etc.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        private async Task LoadTodayForQuickEntry()
        {
            var app = (App)System.Windows.Application.Current;
            var repo = app.Services.GetService(typeof(IJournalRepository)) as IJournalRepository;
            if (repo == null) return;

            var existing = await repo.GetByDateAsync(DateTime.Now.Date);
            if (existing != null)
            {
                QuickTitle = existing.Title;
                QuickContent = existing.Content;
            }
        }
    }
}
