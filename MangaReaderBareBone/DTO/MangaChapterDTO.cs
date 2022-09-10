namespace MangaReaderBareBone.DTO
{
    /// <summary>
    /// MangaChapter Accessible Object
    /// Holds information about manga chapter's name, images filenames,
    /// Previous and Next Chapter's Name 
    /// </summary>
    public class MangaChapterDTO
    {
        public string? MangaChapter { get; set; }
        public string? Path { get; set; }
        public string? PreviousChapter { get; set; }
        public string? NextChapter { get; set; }
    }
}
