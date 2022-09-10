namespace MangaReaderBareBone.DTO
{
    /// <summary>
    /// Manga Accessible Object
    /// Holds information of Manga Details
    /// of all available manga chapters
    /// and last manga chapter read
    /// </summary>
    public class MangasDTO
    {
        public string? MangaName { get; set; }
        public List<string?>? ChaptersAdded { get; set; }
        public string? LastChapterRead { get; set; }
    }
}
