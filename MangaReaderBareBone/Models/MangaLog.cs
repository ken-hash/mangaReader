using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MangaReaderBareBone.Models
{
    public class MangaLog
    {
        [Key]
        public int MangaLogId { get; set; }
        [Required]
        public string? Status { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        [ForeignKey("MangaId")]
        [Required]
        public int MangaId { get; set; }
        [ForeignKey("MangaChaptersId")]
        public virtual MangaChapters MangaChapters { get; set; }
        [Required]
        public int MangaChaptersId { get; set; }

    }
}
