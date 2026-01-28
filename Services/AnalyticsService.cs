using JournalApp.Interfaces;
using JournalApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JournalApp.Services
{
    public interface IAnalyticsService
    {
        Task<int> GetCurrentStreakAsync();
        Task<int> GetLongestStreakAsync();
        Task<Dictionary<string, int>> GetMoodDistributionAsync();
        Task<int> GetAverageWordCountAsync();
        Task<Dictionary<DateTime, int>> GetWordCountTrendsAsync(int days = 7);
        Task<(string most, string least)> GetMoodComparisonAsync();
        Task<Dictionary<string, int>> GetMostUsedTagsAsync();
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly IJournalRepository _repo;

        public AnalyticsService(IJournalRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> GetCurrentStreakAsync()
        {
            var entries = (await _repo.GetAllAsync()).OrderByDescending(e => e.Date).ToList();
            if (!entries.Any()) return 0;

            int streak = 0;
            DateTime current = DateTime.Now.Date;

            // If no entry today, check yesterday
            if (entries[0].Date.Date < current)
            {
                current = current.AddDays(-1);
            }

            foreach (var entry in entries)
            {
                if (entry.Date.Date == current)
                {
                    streak++;
                    current = current.AddDays(-1);
                }
                else if (entry.Date.Date < current)
                {
                    break;
                }
            }
            return streak;
        }

        public async Task<int> GetLongestStreakAsync()
        {
            var entries = (await _repo.GetAllAsync()).OrderBy(e => e.Date).Select(e => e.Date.Date).Distinct().ToList();
            if (!entries.Any()) return 0;

            int maxStreak = 0;
            int currentStreak = 1;

            for (int i = 1; i < entries.Count; i++)
            {
                if ((entries[i] - entries[i - 1]).TotalDays == 1)
                {
                    currentStreak++;
                }
                else
                {
                    maxStreak = Math.Max(maxStreak, currentStreak);
                    currentStreak = 1;
                }
            }
            return Math.Max(maxStreak, currentStreak);
        }

        public async Task<Dictionary<string, int>> GetMoodDistributionAsync()
        {
            var entries = await _repo.GetAllAsync();
            return entries.GroupBy(e => e.PrimaryMood)
                          .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<int> GetAverageWordCountAsync()
        {
            var entries = await _repo.GetAllAsync();
            if (!entries.Any()) return 0;

            var totalWords = entries.Sum(e => e.Content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length);
            return totalWords / entries.Count();
        }

        public async Task<Dictionary<DateTime, int>> GetWordCountTrendsAsync(int days = 7)
        {
            var entries = await _repo.GetAllAsync();
            var cutoff = DateTime.Now.Date.AddDays(-days);
            
            return entries.Where(e => e.Date.Date >= cutoff)
                          .GroupBy(e => e.Date.Date)
                          .ToDictionary(g => g.Key, g => g.Sum(e => e.Content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length));
        }

        public async Task<(string most, string least)> GetMoodComparisonAsync()
        {
            var dist = await GetMoodDistributionAsync();
            if (!dist.Any()) return ("None", "None");

            var sorted = dist.OrderByDescending(x => x.Value).ToList();
            return (sorted.First().Key, sorted.Last().Key);
        }

        public async Task<Dictionary<string, int>> GetMostUsedTagsAsync()
        {
            var entries = await _repo.GetAllAsync();
            return entries.SelectMany(e => e.Tags)
                          .GroupBy(t => t.Name)
                          .OrderByDescending(g => g.Count())
                          .Take(10) // Show more tags
                          .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
