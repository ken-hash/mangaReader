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
                var mangaLogAndChapters = await (from mL in _context.MangaLogs
                                           join mC in _context.MangaChapters on mL.MangaChaptersId equals mC.MangaChaptersId
                                           join m in _context.Mangas on mC.MangaId equals m.MangaId
                                           orderby mL.DateTime 
                                           select new
                                           {
                                               manga = m.Name,
                                               status = mL.Status,
                                               chapter = mC.MangaChapter,
                                               chapterId = mC.MangaChaptersId,
                                               dateTime = mL.DateTime,
                                           }
                                           ).Where(e => e.manga == mangaName && e.status == "Added").ToListAsync();
                var mangaLogAndChaptersRead = await (from mC in _context.MangaChapters
                                           join mL in _context.MangaLogs on mC.MangaChaptersId equals mL.MangaChaptersId into tempLogs
                                           from temp in tempLogs.DefaultIfEmpty()
                                           join m in _context.Mangas on mC.MangaId equals m.MangaId
                                           select new
                                           {
                                               manga = m.Name,
                                               status = temp.Status == "Read" ? "Read" : string.Empty,
                                               chapter = mC.MangaChapter,
                                               chapterId = mC.MangaChaptersId,
                                               dateTime = temp.DateTime,
                                           }
                                           ).Where(e => e.manga == mangaName && e.status == "Read").ToListAsync();
                if (sort?.ToLower() == "desc")
                    mangaLogAndChapters.Reverse();
                List<MangaDetailsDTO> mangaDetails= new ();
                foreach (var m in mangaLogAndChapters)
                {
                    mangaDetails.Add(new MangaDetailsDTO
                    {
                        read = mangaLogAndChaptersRead.Where(e => e.chapterId == m.chapterId).FirstOrDefault()?.status == "Read",
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
            var manga = await _context.Mangas.SingleOrDefaultAsync(m => string.Equals(m.Name, log.MangaName));
            int? mangaId = manga?.MangaId;
            if (mangaId == null)
            {
                return BadRequest("invalid manga");
            }
            var chapter = await _context.MangaChapters!.SingleOrDefaultAsync(e => e.MangaId == mangaId && string.Equals(e.MangaChapter, log.ChapterName));
            int? chapterId = chapter?.MangaChaptersId;
            if (chapterId == null)
            {
                return BadRequest("invalid chapter");
            }
            MangaLog? mangaLog = await _context.MangaLogs.SingleOrDefaultAsync(e => e.MangaChaptersId == chapterId && e.MangaId == mangaId && e.Status == "Read");
            if (mangaLog != null)
            {
                mangaLog.DateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok("Saved");
            }
            MangaLog newLog = new ()
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
