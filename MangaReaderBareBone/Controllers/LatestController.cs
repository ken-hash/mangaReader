using MangaReaderBareBone.Data;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MangaReaderBareBone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LatestController : ControllerBase
    {
        private readonly MangaReaderBareBoneContext _context;

        public LatestController(MangaReaderBareBoneContext context)
        {
            _context = context;
        }

        // Get latest number of numManga mangas and chapters(max 10) added in the last past numdays
        [HttpGet("manga")]
        [ResponseCache(Duration = 60 * 5)]
        public async Task<ActionResult<List<Manga>>> GetLatestChapters(int? numDays = 3, int? numManga = 10)
        {
            if (_context.Mangas == null)
            if (_context.Mangas == null)
            {
                return NotFound();
            }
            List<Manga> latestManga = new ();
            var addedMangaInDays = await GetLatestManga(numDays, numManga, "Added");
            var addedMangaInDays = await GetLatestManga(numDays, numManga, "Added");

            foreach (var manga in addedMangaInDays)
            {
                if (latestManga.Count >= numManga)
                {
                    break;
                }
                if (!latestManga.Any(e => e.MangaId == manga?.Mangas!.MangaId))
                if (!latestManga.Any(e => e.MangaId == manga?.Mangas!.MangaId))
                {
                    latestManga.Add(manga?.Mangas!);
                    latestManga.Add(manga?.Mangas!);
                }
                if (latestManga.FirstOrDefault(e => e.MangaId == manga?.Mangas!.MangaId)?.Chapters?.Count >= 10)
                if (latestManga.FirstOrDefault(e => e.MangaId == manga?.Mangas!.MangaId)?.Chapters?.Count >= 10)
                {
                    continue;
                }
            }
            return latestManga;
        }


        // Get latest number of numManga mangas read and chapters(max 10) in the last past numdays
        [HttpGet("mangaRead")]
        [ResponseCache(Duration = 60 * 5)]
        public async Task<ActionResult<List<Manga>>> GetLastReadChapters(int? numDays = 7, int? numManga = 10)
        {
            if (_context.Mangas == null)
            if (_context.Mangas == null)
            {
                return NotFound();
            }
            List<Manga> latestManga = new();
            var addedMangaInDays = await GetLatestManga(numDays, numManga, "Read");
            var addedMangaInDays = await GetLatestManga(numDays, numManga, "Read");

            foreach (var manga in addedMangaInDays)
            {
                if (latestManga.Count >= numManga)
                {
                    break;
                }
                if (!latestManga.Any(e => e.MangaId == manga?.Mangas!.MangaId))
                if (!latestManga.Any(e => e.MangaId == manga?.Mangas!.MangaId))
                {
                    latestManga.Add(manga?.Mangas!);
                    latestManga.Add(manga?.Mangas!);
                }
                if (latestManga.FirstOrDefault(e => e.MangaId == manga?.Mangas!.MangaId)?.Chapters?.Count >= 10)
                if (latestManga.FirstOrDefault(e => e.MangaId == manga?.Mangas!.MangaId)?.Chapters?.Count >= 10)
                {
                    continue;
                }
            }
            return latestManga;
        }

        private async Task<List<TempManga>> GetLatestManga(int? numDays = 7, int? numManga = 10, string? status="Added")
        {
            var latestManga = await (from m in _context.Mangas
                                          join mL in _context.MangaLogs! on m.MangaId equals mL.MangaId
                                          join mC in _context.MangaChapters! on mL.MangaChaptersId equals mC.MangaChaptersId
                                          where mL.Status == status && mL.DateTime > DateTime.Now.AddDays(-(double)numDays!)
                                          orderby mL.DateTime descending
                                          select new TempManga
                                          {
                                              Mangas = m,
                                              MangaLogs = mL,
                                              MangaChapters = mC
                                          }).ToListAsync();
            return latestManga!;
        }
        private class TempManga
        {
            public Manga? Mangas { get; set; }
            public MangaLog? MangaLogs { get; set; }
            public MangaChapters? MangaChapters { get; set; }
        }
    }
}
