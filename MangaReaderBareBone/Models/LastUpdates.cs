using System.ComponentModel.DataAnnotations;

namespace MangaReaderBareBone.Models
{
    public class LastUpdates
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? SiteName { get; set; }
        public string? SiteUrl { get; set; }
        public DateTime LastUpdate { get; set; }
        [Required]
        public string? MachineName { get; set; }
    }
}