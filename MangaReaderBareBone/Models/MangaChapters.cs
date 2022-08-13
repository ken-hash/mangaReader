using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaReaderBareBone.Models
{
    public class MangaChapters
    {
        [Key]
        public int MangaChaptersId { get; set; }
        [Required]
        public string MangaChapter { get; set; }

        public string Path { get; set; }

        [ForeignKey("MangaId")]
        [Required]
        public int MangaId { get; set; }
    }
}
