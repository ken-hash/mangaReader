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
        [ResponseCache(Duration = 60 * 5)]
        public async Task<ActionResult<List<Manga>>> GetLatestChapters(int? numDays = 3, int? numManga = 10)
        {
            if (_context.Mangas == null || _context.MangaLogs == null || _context.MangaChapters == null)
            {
                return NotFound();
            }
            List<Manga> latestManga = new ();
            var addedMangaInDays = (from m in _context.Mangas
                                    join mL in _context.MangaLogs on m.MangaId equals mL.MangaId
                                    join mC in _context.MangaChapters on mL.MangaChaptersId equals mC.MangaChaptersId
                                    select new
                                    {
                                        Mangas = m,
                                        MangaLogs = mL,
                                        MangaChapters = mC
                                    }).Where(e => e.MangaLogs.Status == "Added" && e.MangaLogs.DateTime > DateTime.Now.AddDays(-(double)numDays!)).OrderBy(e => e.MangaLogs.DateTime).Reverse();

            foreach (var manga in addedMangaInDays)
            {
                if (latestManga.Count >= numManga)
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
            }
            return latestManga;
        }


        // Get latest number of numManga mangas read and chapters(max 10) in the last past numdays
        [HttpGet("mangaRead")]
        [ResponseCache(Duration = 60 * 5)]
        public async Task<ActionResult<List<Manga>>> GetLastReadChapters(int? numDays = 7, int? numManga = 10)
        {
            if (_context.Mangas == null || _context.MangaLogs == null || _context.MangaChapters == null)
            {
                return NotFound();
            }
            List<Manga> latestManga = new();
            var addedMangaInDays = (from m in _context.Mangas
                                    join mL in _context.MangaLogs on m.MangaId equals mL.MangaId
                                    join mC in _context.MangaChapters on mL.MangaChaptersId equals mC.MangaChaptersId
                                    select new
                                    {
                                        Mangas = m,
                                        MangaLogs = mL,
                                        MangaChapters = mC
                                    }).Where(e => e.MangaLogs.Status == "Read" && e.MangaLogs.DateTime > DateTime.Now.AddDays(-(double)numDays!)).OrderBy(e => e.MangaLogs.DateTime).Reverse();

            foreach (var manga in addedMangaInDays)
            {
                if (latestManga.Count >= numManga)
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
            }
            return latestManga;
        }
    }
}
