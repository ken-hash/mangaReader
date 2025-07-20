namespace MangaReaderBareBone.Models
{
    public class MangaLogsChapterView
    {
        public int MangaId { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public DateTime LogDateTime { get; set; }
        public int MangaChaptersId { get; set; }
        public string? ChapterName { get; set; }

    }
}
