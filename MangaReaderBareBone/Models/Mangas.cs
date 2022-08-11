using System.ComponentModel.DataAnnotations;

namespace MangaReaderBareBone.Models
{
    public class Manga
    {
        [Key]
        public int MangaId { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<MangaChapters> Chapters { get; set; }

    }
}
