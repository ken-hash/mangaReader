using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangaReaderBareBone.Data;
using MangaReaderBareBone.Models;

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
            var mangaLog = await _context.MangaLogs.FindAsync(id);

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
            var manga = await _context.Mangas.FindAsync(id);
            if (manga != null)
            {
                var mangaLog = _context.MangaLogs.Where(log => manga.MangaId == log.MangaId);
                if (mangaLog == null)
                {
                    return NotFound();
                }
                //coallesce to show empty chapters.. shouldnt happen as it is a foreign key and would be never be empty
                mangaLog.ToList().ForEach(e => e.MangaChapters = _context.MangaChapters.Find(e.MangaChaptersId) ?? new MangaChapters());
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
        public async Task<ActionResult<List<MangaLog>>> GetMangaLogByMangaID(string? mangaName, string?sort="asc")
        {
            if (_context.MangaLogs == null||string.IsNullOrEmpty(mangaName) || _context.MangaChapters == null)
            {
                return NotFound();
            }
            var manga = await _context.Mangas.FirstOrDefaultAsync(manga => manga.Name.ToLower() == mangaName.ToLower());
            if (manga != null)
            {
                var mangaLog = _context.MangaLogs.Where(log => manga.MangaId == log.MangaId);
                if (mangaLog == null)
                {
                    return NotFound();
                }
                //coallesce to show empty chapters.. shouldnt happen as it is a foreign key and would be never be empty
                mangaLog.ToList().ForEach(e => e.MangaChapters = _context.MangaChapters.Find(e.MangaChaptersId)??new MangaChapters());
                if (sort?.ToLower() == "desc")
                {
                    return mangaLog.ToList().OrderBy(e=>e.DateTime).Reverse().ToList();
                }
                return mangaLog.ToList();
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<MangaLog> PostMangaLog(MangaLog log)
        {
            if (!ModelState.IsValid) { 
                return BadRequest(ModelState); 
            }
            if (_context.MangaLogs == null)
                return Problem("Can't connect to database");
            _context.MangaLogs.Add(log);
            _context.SaveChangesAsync();
            return Problem("Disabled Post");
        }
    }
}
