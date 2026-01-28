using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models
{
    public class UserSettings
    {
        [Key]
        public int Id { get; set; }
        public string Pin { get; set; } = "1234"; // Default PIN
        public bool IsDarkMode { get; set; } = false;
        public DateTime? LastBackupAt { get; set; }
    }
}
