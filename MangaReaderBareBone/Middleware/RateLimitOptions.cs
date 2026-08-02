namespace MangaReaderBareBone.Middleware
{
    public class RateLimitOptions
    {
        public const string SectionName = "RateLimiting";

        public int GlobalPermitLimit { get; set; } = 100;
        public int WritePermitLimit { get; set; } = 30;
    }
}
