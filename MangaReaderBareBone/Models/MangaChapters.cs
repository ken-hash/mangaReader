using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaReaderBareBone.Models
{
    /// <summary>
    /// Manga Chapter Details Table
    /// Holds information about Manga Chapter
    /// </summary>
    public class MangaChapters
    {
        [Key]
        public int MangaChaptersId { get; set; }

        //Chapter Name e.g. Chapter 1, Chapter 1001 Extra
        [Required]
        public string MangaChapter { get; set; }

        //CSV that holds image filenames inside chapter folder
        public string Path { get; set; }

        [ForeignKey("MangaId")]
        [Required]
        public int MangaId { get; set; }
    }
}
