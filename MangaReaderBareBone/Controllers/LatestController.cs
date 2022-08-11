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
        // GET: api/<LatestController>
        [HttpGet("manga")]
        public async Task<ActionResult<List<Manga>>> GetLatestChapters(int? numDays=7, int? numManga=10)
        {
            return NotFound();
            //TODO REFACTOR
            /*
            if (_context.Mangas == null || _context.MangaLogs == null  || _context.MangaChapters == null)
            {
                return NotFound();
            }
            List<Manga> latestManga = new List<Manga>();
            if (numDays.HasValue)
            {
                var addedMangaInDays = _context.MangaLogs?.Where(e => e.Status == "Added" && e.DateTime > DateTime.Now.AddDays(-(Double)numDays));
                if (addedMangaInDays == null)
                {
                    return NotFound();
                }
                foreach (var manga in addedMangaInDays)
                {
                    if (latestManga.Count >= numManga)
                    {
                        break;
                    }
                    var mangaToAdd = await _context.Mangas.FindAsync(manga.MangaId);
                    if (mangaToAdd == null)
                        continue;
                    if (!latestManga.Any(e=>e.MangaId == mangaToAdd.MangaId))
                    {
                        latestManga.Add(mangaToAdd);
                    }
                    var chapterToAdd = await _context.MangaChapters.FindAsync(manga.MangaChaptersId);
                    if (chapterToAdd == null)
                        continue;
                    latestManga.FirstOrDefault(e => e.MangaId == mangaToAdd.MangaId)?.Chapters.Add(chapterToAdd);
                }
            }
            */
        }

    }
}
