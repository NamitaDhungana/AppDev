using JournalApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JournalApp.Interfaces
{
    public interface IJournalRepository
    {
        Task<IEnumerable<JournalEntry>> GetAllAsync();
        Task<JournalEntry?> GetByIdAsync(int id);
        Task<JournalEntry?> GetByDateAsync(DateTime date);
        Task AddAsync(JournalEntry entry);
        Task UpdateAsync(JournalEntry entry);
        Task DeleteAsync(int id);
        Task<IEnumerable<JournalEntry>> SearchAsync(string query);
        Task<IEnumerable<JournalEntry>> FilterAsync(DateTime? startDate, DateTime? endDate, string? mood, string? tag, string? searchQuery = null);
        Task<Tag> GetOrCreateTagAsync(string tagName);
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        
        // User Settings
        Task<UserSettings> GetUserSettingsAsync();
        Task UpdateUserSettingsAsync(UserSettings settings);
        Task ClearAllDataAsync();
    }
}
