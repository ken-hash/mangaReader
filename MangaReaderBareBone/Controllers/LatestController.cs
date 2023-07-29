using MangaReaderBareBone.Data;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;
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
        [ResponseCache(Duration = 60 * 1)]
        public async Task<ActionResult<List<Manga>>> GetLatestChapters(int? numDays = 3, int? numManga = 10)
        {
            if (_context.Mangas == null)
                return NotFound();
            return await GetLatestManga(numDays, numManga, "Added");
        }

        // Get latest number of numManga mangas read and chapters(max 10) in the last past numdays
        [HttpGet("mangaRead")]
        public async Task<ActionResult<List<Manga>>> GetLastReadChapters(int? numDays = 7, int? numManga = 10)
        {
            if (_context.Mangas == null)
                return NotFound();
            return await GetLatestManga(numDays, numManga, "Read");
        }

        private async Task<List<Manga>> GetLatestManga(int? numDays = 7, int? numManga = 10, string? status = "Added", int? maxNumChapters = 10)
        {
            List<MangaLogsChapterView> updatedManga = await (from mlcv in _context.MangaLogsChapterView
                                                             where mlcv.Status == status && mlcv.LogDateTime > DateTime.Today.AddDays(-(double)numDays!)
                                                             orderby mlcv.LogDateTime descending
                                                             select (mlcv)).ToListAsync();
            List<Manga> latestManga = new();
            foreach (MangaLogsChapterView manga in updatedManga)
            {
                if (latestManga.Count() >= numManga)
                    break;
                if (!latestManga.Any(e => e.MangaId == manga?.MangaId))
                {
                    latestManga.Add(new Manga
                    {
                        MangaId = manga.MangaId,
                        Name = manga.Name,
                        Chapters = new List<MangaChapters> {
                        new MangaChapters{
                            MangaChaptersId = manga.MangaChaptersId,
                            MangaChapter = manga.ChapterName,
                            MangaId = manga.MangaId
                            }
                        }
                    });
                }
                else
                {
                    if (latestManga.Where(e => e.MangaId == manga?.MangaId).First().Chapters!.Count() >= maxNumChapters)
                        continue;
                    latestManga.Where(e => e.MangaId == manga?.MangaId).First().Chapters!.Add(
                        new MangaChapters
                        {
                            MangaChaptersId = manga.MangaChaptersId,
                            MangaChapter = manga.ChapterName,
                            MangaId = manga.MangaId
                        });
                }
            }
            return latestManga;
        }

    }
}
