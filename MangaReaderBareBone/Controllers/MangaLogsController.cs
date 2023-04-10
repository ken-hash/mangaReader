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
        [ResponseCache(Duration = 60 * 5)]
        public async Task<ActionResult<List<MangaDetailsDTO>>> GetMangaLogByMangaNameAsync(string? mangaName, string? sort = "asc")
        {
            if (_context.MangaLogs == null || string.IsNullOrEmpty(mangaName) || _context.MangaChapters == null || _context.Mangas == null)
            {
                return NotFound();
            }
            Manga? manga = await _context.Mangas.FirstOrDefaultAsync(manga => manga.Name!.ToLower() == mangaName.ToLower());
            if (manga != null)
            {
                var mangaLogAndChapters = (from mL in _context.MangaLogs
                                           join mC in _context.MangaChapters on mL.MangaChaptersId equals mC.MangaChaptersId
                                           join m in _context.Mangas on mC.MangaId equals m.MangaId
                                           select new
                                           {
                                               manga = m.Name,
                                               status = mL.Status,
                                               chapter = mC.MangaChapter,
                                               chapterId = mC.MangaChaptersId,
                                               dateTime = mL.DateTime,
                                           }
                                           ).Where(e => e.manga == mangaName).OrderBy(e => e.dateTime);
                if (sort?.ToLower() == "desc")
                    mangaLogAndChapters.Reverse();
                List<MangaDetailsDTO> mangaDetails= new ();
                foreach (var m in mangaLogAndChapters.Where(e=>e.status=="Added"))
                {
                    mangaDetails.Add(new MangaDetailsDTO
                    {
                        read = mangaLogAndChapters.Any(e=>e.status=="Read" && e.chapter==m.chapter),
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
            int? mangaId = _context.Mangas.FirstOrDefault(e => e.Name.ToLower() == log.MangaName.ToLower())?.MangaId;
            if (mangaId == null)
            {
                return BadRequest("invalid manga");
            }
            int? chapterId = _context.MangaChapters?.FirstOrDefault(e => e.MangaId == mangaId && e.MangaChapter.ToLower() == log.ChapterName.ToLower())?.MangaChaptersId;
            if (chapterId == null)
            {
                return BadRequest("invalid chapter");
            }
            MangaLog? mangaLog = _context.MangaLogs.FirstOrDefault(e => e.MangaChaptersId == chapterId && e.MangaId == mangaId && e.Status == "Read");
            if (mangaLog != null)
            {
                mangaLog.DateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok("Saved");
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
