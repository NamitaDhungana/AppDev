using JournalApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace JournalApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }

        public string DbPath { get; }

        public AppDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "journal_app.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
            // EF9 check for dev changes
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalEntry>()
                .HasIndex(e => e.Date)
                .IsUnique(); // Ensure one entry per day (unique constraints usually ignore time if stored as date, but we handle that in logic too)

            // Seed some default tags
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "Work", Category = "System" },
                new Tag { Id = 2, Name = "Career", Category = "System" },
                new Tag { Id = 3, Name = "Studies", Category = "System" },
                new Tag { Id = 4, Name = "Family", Category = "System" },
                new Tag { Id = 5, Name = "Friends", Category = "System" },
                new Tag { Id = 6, Name = "Relationships", Category = "System" },
                new Tag { Id = 7, Name = "Health", Category = "System" },
                new Tag { Id = 8, Name = "Fitness", Category = "System" },
                new Tag { Id = 9, Name = "Travel", Category = "System" },
                new Tag { Id = 10, Name = "Nature", Category = "System" },
                new Tag { Id = 11, Name = "Reading", Category = "System" },
                new Tag { Id = 12, Name = "Writing", Category = "System" },
                new Tag { Id = 13, Name = "Cooking", Category = "System" },
                new Tag { Id = 14, Name = "Music", Category = "System" },
                new Tag { Id = 15, Name = "Reflection", Category = "System" }
            );

            // Seed initial settings
            modelBuilder.Entity<UserSettings>().HasData(
                new UserSettings { Id = 1, Pin = "1234", IsDarkMode = false }
            );
        }
    }
}
