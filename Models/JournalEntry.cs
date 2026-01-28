using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; } // For ensuring one entry per day
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // Markdown or RichText content

        public string PrimaryMood { get; set; } = string.Empty;
        public string? SecondaryMood1 { get; set; }
        public string? SecondaryMood2 { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
