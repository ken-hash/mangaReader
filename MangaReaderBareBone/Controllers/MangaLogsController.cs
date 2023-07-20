using MangaReaderBareBone.Data;
using MangaReaderBareBone.DTO;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace MangaReaderBareBone.Controllers
{
    /// <summary>
    /// API endpoints on retrieving database details on manga logs
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]

    public class MangaLogsController : ControllerBase
    {
        private readonly MangaReaderBareBoneContext _context;

        public MangaLogsController(MangaReaderBareBoneContext context)
        {
            _context = context;
        }

        //Retrieving all logs for manga using manga name with sort as second parameter
        [HttpGet("mangaName")]
        [ResponseCache(Duration = 60 * 2, VaryByQueryKeys = new[] { "mangaName" })]
        public async Task<ActionResult<List<MangaDetailsDTO>>> GetMangaLogByMangaNameAsync(string? mangaName, string? sort = "asc")
        {
            if (_context.MangaLogs == null || string.IsNullOrEmpty(mangaName) || _context.MangaChapters == null || _context.Mangas == null)
            {
                return NotFound();
            }
            Manga? manga = await _context.Mangas.SingleOrDefaultAsync(m => string.Equals(m.Name, mangaName));
            if (manga != null)
            {
                var mangaLogAndChapters = await (from mlcv in _context.MangaLogsChapterView
                                                 where string.Equals(mlcv.Name, mangaName) && mlcv.Status == "Added"
                                                 orderby mlcv.LogDateTime, mlcv.ChapterName
                                                 select new
                                                 {
                                                     mangaId = mlcv.MangaId,
                                                     manga = mlcv.Name,
                                                     chapter = mlcv.ChapterName,
                                                     chapterId = mlcv.MangaChaptersId,
                                                     dateTime = mlcv.LogDateTime,
                                                 }
                                           ).ToListAsync();
                var mangaLogAndChaptersRead = await (from mlcv in _context.MangaLogsChapterView
                                                     where string.Equals(mlcv.Name, mangaName) && mlcv.Status == "Read"
                                                     orderby mlcv.LogDateTime, mlcv.ChapterName
                                                     select new
                                                     {
                                                         mangaId = mlcv.MangaId,
                                                         manga = mlcv.Name,
                                                         chapter = mlcv.ChapterName,
                                                         chapterId = mlcv.MangaChaptersId,
                                                         dateTime = mlcv.LogDateTime,
                                                     }
                                           ).ToListAsync();
                if (sort?.ToLower() == "desc")
                    mangaLogAndChapters.Reverse();
                List<MangaDetailsDTO> mangaDetails = new();
                foreach (var m in mangaLogAndChapters)
                {
                    mangaDetails.Add(new MangaDetailsDTO
                    {
                        read = mangaLogAndChaptersRead.Any(e => e.chapterId == m.chapterId),
                        mangaChapter = m.chapter,
                        dateTime = m.dateTime
                    });
                }
                return mangaDetails;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<MangaLogDTO>> PostMangaLogAsync([FromBody] MangaLogDTO log)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (_context.MangaLogs == null)
                return Problem("Can't connect to database");
            if (string.IsNullOrWhiteSpace(log.MangaName) || string.IsNullOrEmpty(log.ChapterName))
                return BadRequest("Invalid Request");
            Manga? manga = await _context.Mangas.SingleOrDefaultAsync(m => string.Equals(m.Name, log.MangaName));
            int? mangaId = manga?.MangaId;
            if (mangaId == null)
                return BadRequest("invalid manga");
            MangaChapters? chapter = await _context.MangaChapters!.SingleOrDefaultAsync(e => e.MangaId == mangaId && string.Equals(e.MangaChapter, log.ChapterName));
            int? chapterId = chapter?.MangaChaptersId;
            if (chapterId == null)
                return BadRequest("invalid chapter");
            MangaLog? mangaLog = await _context.MangaLogs.SingleOrDefaultAsync(e => e.MangaChaptersId == chapterId && e.MangaId == mangaId && e.Status == "Read");
            if (mangaLog != null)
            {
                mangaLog.DateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok("Saved");
            }
            MangaLog newLog = new()
            {
                Status = log.Status,
                MangaId = mangaId!,
                MangaChaptersId = chapterId!
            };

            _context.MangaLogs.Add(newLog);
            await _context.SaveChangesAsync();
            return Ok("Success");
        }


    }
}
