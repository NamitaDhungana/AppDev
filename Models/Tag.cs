using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Category { get; set; } = "Custom"; // "System" or "Custom"

        public virtual ICollection<JournalEntry> Entries { get; set; } = new List<JournalEntry>();
    }
}
