using MangaReaderBareBone.Data;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<List<Manga>>> GetLatestChapters(int? numDays = 3, int? numManga = 10)
        {
            if (_context.Mangas == null || _context.MangaLogs == null || _context.MangaChapters == null)
            {
                return NotFound();
            }
            List<Manga> latestManga = new List<Manga>();
            var addedMangaInDays = _context.Mangas?.Join(_context.MangaLogs, manga => manga.MangaId, logs => logs.MangaId, (manga, logs) => new
            { Mangas = manga, MangaLogs = logs }
            ).Where(mangaAndLogs => mangaAndLogs.MangaLogs.Status == "Added" && mangaAndLogs.MangaLogs.DateTime > DateTime.Now.AddDays(-(double)numDays));
            if (addedMangaInDays == null)
            {
                return NotFound();
            }
            foreach (var manga in addedMangaInDays.ToList().OrderBy(e => e.MangaLogs.DateTime).Reverse())
            {
                if (latestManga.Count >= numManga && !latestManga.Any(e => e.MangaId == manga.Mangas.MangaId))
                {
                    break;
                }
                if (!latestManga.Any(e => e.MangaId == manga.Mangas.MangaId))
                {
                    latestManga.Add(manga.Mangas);
                }
                if (latestManga.FirstOrDefault(e => e.MangaId == manga.Mangas.MangaId)?.Chapters?.Count >= 10)
                {
                    continue;
                }

                MangaChapters? chapterToAdd = await _context.MangaChapters.FindAsync(manga.MangaLogs.MangaChaptersId);
                if (chapterToAdd == null)
                {
                    continue;
                }

                latestManga.FirstOrDefault(e => e.MangaId == manga.Mangas.MangaId)?.Chapters?.Add(chapterToAdd);

            }
            return latestManga;
        }


        // Get latest number of numManga mangas read and chapters(max 10) in the last past numdays
        [HttpGet("mangaRead")]
        public async Task<ActionResult<List<Manga>>> GetLastReadChapters(int? numDays = 7, int? numManga = 10)
        {
            if (_context.Mangas == null || _context.MangaLogs == null || _context.MangaChapters == null)
            {
                return NotFound();
            }
            List<Manga> latestManga = new List<Manga>();
            var addedMangaInDays = _context.Mangas?.Join(_context.MangaLogs, manga => manga.MangaId, logs => logs.MangaId, (manga, logs) => new
            { Mangas = manga, MangaLogs = logs }
            ).Where(mangaAndLogs => mangaAndLogs.MangaLogs.Status == "Read" && mangaAndLogs.MangaLogs.DateTime > DateTime.Now.AddDays(-(double)numDays));
            if (addedMangaInDays == null)
            {
                return NotFound();
            }
            foreach (var manga in addedMangaInDays.ToList().OrderBy(e => e.MangaLogs.DateTime).Reverse())
            {
                if (latestManga.Count >= numManga && !latestManga.Any(e => e.MangaId == manga.Mangas.MangaId))
                {
                    break;
                }
                if (!latestManga.Any(e => e.MangaId == manga.Mangas.MangaId))
                {
                    latestManga.Add(manga.Mangas);
                }
                if (latestManga.FirstOrDefault(e => e.MangaId == manga.Mangas.MangaId)?.Chapters?.Count >= 10)
                {
                    continue;
                }

                MangaChapters? chapterToAdd = await _context.MangaChapters.FindAsync(manga.MangaLogs.MangaChaptersId);
                if (chapterToAdd == null)
                {
                    continue;
                }

                latestManga.FirstOrDefault(e => e.MangaId == manga.Mangas.MangaId)?.Chapters.Add(chapterToAdd);
            }
            return latestManga;
        }
    }
}
