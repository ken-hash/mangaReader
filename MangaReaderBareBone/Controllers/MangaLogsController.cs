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


        [HttpGet("logId")]
        public async Task<ActionResult<MangaLog>> GetMangaLogByID(int id)
        {
            if (_context.MangaLogs == null)
            {
                return NotFound();
            }
            MangaLog? mangaLog = await _context.MangaLogs.FindAsync(id);

            if (mangaLog == null)
            {
                return NotFound();
            }

            return mangaLog;
        }


        //Retrieving all logs for manga using mangaid
        [HttpGet("mangaId")]
        public async Task<ActionResult<List<MangaLog>>> GetMangaLogByMangaID(int id, string? sort = "asc")
        {
            if (_context.MangaLogs == null || _context.MangaChapters == null)
            {
                return NotFound();
            }
            Manga? manga = await _context.Mangas.FindAsync(id);
            if (manga != null)
            {
                IQueryable<MangaLog> mangaLog = _context.MangaLogs.Where(log => manga.MangaId == log.MangaId);
                if (mangaLog == null)
                {
                    return NotFound();
                }
                mangaLog.ToList().ForEach(e => e.MangaChapters = _context.MangaChapters.First(f => f.MangaChaptersId == e.MangaChaptersId));
                if (sort?.ToLower() == "desc")
                {
                    return mangaLog.ToList().OrderBy(e => e.DateTime).Reverse().ToList();
                }
                return mangaLog.ToList();
            }
            return NotFound();
        }


        //Retrieving all logs for manga using manga name with sort as second parameter
        [HttpGet("mangaName")]
        public async Task<ActionResult<List<MangaLog>>> GetMangaLogByMangaID(string? mangaName, string? sort = "asc")
        {
            if (_context.MangaLogs == null || string.IsNullOrEmpty(mangaName) || _context.MangaChapters == null)
            {
                return NotFound();
            }
            Manga? manga = await _context.Mangas.FirstOrDefaultAsync(manga => manga.Name.ToLower() == mangaName.ToLower());
            if (manga != null)
            {
                IQueryable<MangaLog> mangaLog = _context.MangaLogs.Where(log => manga.MangaId == log.MangaId);
                if (mangaLog == null)
                {
                    return NotFound();
                }
                mangaLog.ToList().ForEach(e => e.MangaChapters = _context.MangaChapters.First(f => f.MangaChaptersId == e.MangaChaptersId));
                if (sort?.ToLower() == "desc")
                {
                    return mangaLog.ToList().OrderBy(e => e.DateTime).Reverse().ToList();
                }
                return mangaLog.ToList();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<MangaLogDTO>> PostMangaLogAsync([FromBody] MangaLogDTO log)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.MangaLogs == null)
            {
                return Problem("Can't connect to database");
            }
            if (string.IsNullOrWhiteSpace(log.MangaName) || string.IsNullOrEmpty(log.ChapterName))
            {
                return BadRequest("Invalid Request");
            }
            int? mangaId = _context.Mangas.FirstOrDefault(e=>e.Name.ToLower() == log.MangaName.ToLower())?.MangaId;
            if (mangaId == null)
            {
                return BadRequest("invalid manga");
            }
            int? chapterId = _context.MangaChapters?.FirstOrDefault(e => e.MangaId == mangaId && e.MangaChapter.ToLower() == log.ChapterName.ToLower())?.MangaChaptersId;
            if (chapterId == null)
            {
                return BadRequest("invalid chapter");
            }
            MangaLog newLog = new MangaLog
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
