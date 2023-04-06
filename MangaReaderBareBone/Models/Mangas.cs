using System.ComponentModel.DataAnnotations;

namespace MangaReaderBareBone.Models
{
    public class Manga
    {
        [Key]
        public int MangaId { get; set; }
        [Required]
        public string? Name { get; set; }

        //TODO add description into database and include pictures for cover and banner to be used in web app
        //public string Description { get; set; }

        public virtual ICollection<MangaChapters>? Chapters { get; set; }
    }
}
