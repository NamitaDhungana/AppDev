using JournalApp.Data;
using JournalApp.Interfaces;
using JournalApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JournalApp.Repositories
{
    public class JournalRepository : IJournalRepository
    {
        private readonly AppDbContext _context;

        public JournalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JournalEntry>> GetAllAsync()
        {
            return await _context.JournalEntries.Include(e => e.Tags).OrderByDescending(e => e.Date).ToListAsync();
        }

        public async Task<JournalEntry?> GetByIdAsync(int id)
        {
            return await _context.JournalEntries.Include(e => e.Tags).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<JournalEntry?> GetByDateAsync(DateTime date)
        {
            return await _context.JournalEntries
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(e => e.Date.Date == date.Date);
        }

        public async Task AddAsync(JournalEntry entry)
        {
            await _context.JournalEntries.AddAsync(entry);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JournalEntry entry)
        {
            _context.JournalEntries.Update(entry);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entry = await _context.JournalEntries.FindAsync(id);
            if (entry != null)
            {
                _context.JournalEntries.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<JournalEntry>> SearchAsync(string query)
        {
            return await _context.JournalEntries
                .Include(e => e.Tags)
                .Where(e => e.Title.Contains(query) || e.Content.Contains(query))
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<JournalEntry>> FilterAsync(DateTime? startDate, DateTime? endDate, string? mood, string? tag, string? searchQuery = null)
        {
            var query = _context.JournalEntries.Include(e => e.Tags).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value.Date.AddDays(1).AddSeconds(-1));

            if (!string.IsNullOrEmpty(mood))
                query = query.Where(e => e.PrimaryMood == mood || e.SecondaryMood1 == mood || e.SecondaryMood2 == mood);

            if (!string.IsNullOrEmpty(tag))
                query = query.Where(e => e.Tags.Any(t => t.Name == tag));

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(e => e.Title.Contains(searchQuery) || e.Content.Contains(searchQuery));
            }

            return await query.OrderByDescending(e => e.Date).ToListAsync();
        }

        public async Task<Tag> GetOrCreateTagAsync(string tagName)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());
            if (tag == null)
            {
                tag = new Tag { Name = tagName, Category = "Custom" };
                await _context.Tags.AddAsync(tag);
                await _context.SaveChangesAsync();
            }
            return tag;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<UserSettings> GetUserSettingsAsync()
        {
            var settings = await _context.UserSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new UserSettings { Pin = "1234" };
                _context.UserSettings.Add(settings);
                await _context.SaveChangesAsync();
            }
            return settings;
        }

        public async Task UpdateUserSettingsAsync(UserSettings settings)
        {
            _context.UserSettings.Update(settings);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAllDataAsync()
        {
            _context.JournalEntries.RemoveRange(_context.JournalEntries);
            // Optionally keep tags or clear them too
            await _context.SaveChangesAsync();
        }
    }
}
