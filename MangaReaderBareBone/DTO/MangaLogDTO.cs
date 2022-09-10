namespace MangaReaderBareBone.DTO
{
    /// <summary>
    /// Manga Log Accessible Object
    /// Holds information of Manga Chapter
    /// whether the log is read or added
    /// </summary>
    public class MangaLogDTO
    {
        public string? MangaName { get; set; }
        public string? Status { get; set; }
        public string? ChapterName { get; set; }
    }
}
