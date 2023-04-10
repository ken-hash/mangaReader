using MangaReaderBareBone.Controllers.Extensions;
using MangaReaderBareBone.Data;
using MangaReaderBareBone.DTO;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MangaReaderBareBone.Controllers
{
    /// <summary>
    /// API endpoint to retrieve chapter details about specific mangachapterid
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MangaChaptersController : ControllerBase
    {
        private readonly MangaReaderBareBoneContext _context;

        public MangaChaptersController(MangaReaderBareBoneContext context)
        {
            _context = context;
        }

        [HttpPost("DeleteChapter/{mangaTitle}/{mangaChapterTitle}")]
        public async Task<IActionResult> DeleteMangaChapter(string mangaTitle, string mangaChapterTitle)
        {
            if (_context.MangaChapters == null)
            {
                return NotFound();
            }
            Manga? manga = await _context.Mangas.FirstOrDefaultAsync(e => e.Name == mangaTitle);
            if (manga == null)
            {
                return NotFound();
            }
            MangaChapters? mangaChapter = await _context.MangaChapters.FirstOrDefaultAsync(e => e.MangaId == manga.MangaId && e.MangaChapter == mangaChapterTitle);
            if (mangaChapter == null)
            {
                return NotFound();
            }
            List<MangaLog> mangaLog = _context.MangaLogs!.Where(e => e.MangaChaptersId == mangaChapter.MangaChaptersId).ToList();
            foreach (var log in mangaLog)
            {
                _context.MangaLogs!.Remove(log);
            }
            _context.MangaChapters.Remove(mangaChapter);
            await _context.SaveChangesAsync();

            return Ok($"Deleted {mangaTitle} : Chapter {mangaChapterTitle}");
        }

        [HttpPost("AddChapter")]
        public async Task<IActionResult> AddMangaChapter([FromBody] piMangaChapter piMangaChapter)
        {
            if (!ModelState.IsValid || piMangaChapter == null)
            {
                return BadRequest(ModelState);
            }
            if (_context.MangaChapters == null)
            {
                return NotFound();
            }
            Manga? manga = await _context.Mangas.FirstOrDefaultAsync(e => e.Name == piMangaChapter.Name);
            if (manga == null)
            {
                Manga newManga = new Manga
                {
                    Name = piMangaChapter.Name,
                };
                await _context.Mangas.AddAsync(newManga);
                manga=newManga;
                await _context.SaveChangesAsync();
            }

            MangaChapters? mangaChapter = await _context.MangaChapters.FirstOrDefaultAsync(e => e.MangaId == manga.MangaId && e.MangaChapter == piMangaChapter.MangaChapter);
            if (mangaChapter == null)
            {
                MangaChapters newChapter = new MangaChapters
                {
                    MangaId = manga.MangaId,
                    MangaChapter = piMangaChapter.MangaChapter,
                    Path = piMangaChapter.Path
                };
                await _context.MangaChapters.AddAsync(newChapter);
                await _context.SaveChangesAsync();
                MangaLog newLog = new MangaLog
                {
                    MangaId = manga.MangaId,
                    Status = "Added",
                    DateTime = DateTime.Now,
                    MangaChaptersId = newChapter.MangaChaptersId
                };
                await _context.MangaLogs!.AddAsync(newLog);
                await _context.SaveChangesAsync();
                return Created("", $"Added {piMangaChapter.Name} : Chapter {piMangaChapter.MangaChapter}");

            }
            else if (mangaChapter.Path != piMangaChapter.Path)
            {
                MangaLog mangaLog = await _context.MangaLogs!.FirstAsync(e => e.MangaChaptersId == mangaChapter.MangaChaptersId && e.Status == "Added");
                mangaChapter.Path = piMangaChapter.Path;
                await _context.SaveChangesAsync();
                return Ok($"Modified {piMangaChapter.Name} : Chapter {piMangaChapter.MangaChapter}");
            }
            return BadRequest("Chapter existed.");
        }
    }
}
