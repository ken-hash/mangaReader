using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
