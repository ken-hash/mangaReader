using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaReaderBareBone.Models
{
    /// <summary>
    /// Manga Logging Details Table
    /// Holds information about Manga Chapters Added/Read
    /// Neccessary to sort latest added/read chapters
    /// </summary>
    public class MangaLog
    {
        [Key]
        public int MangaLogId { get; set; }
        [Required]
        //Adding a manga chapter would generate a log status of Added
        //Reading a manga chapter would generate a log status of Read
        public string? Status { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        [ForeignKey("MangaId")]
        [Required]
        public int? MangaId { get; set; }
        [ForeignKey("MangaChaptersId")]
        public virtual MangaChapters? MangaChapters { get; set; }
        [Required]
        public int? MangaChaptersId { get; set; }

    }
}
