using MangaReaderBareBone.Controllers.Extensions;
using MangaReaderBareBone.Data;
using MangaReaderBareBone.DTO;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("chapterId")]
        public async Task<ActionResult<MangaChapterDTO>> GetMangaChapters(int id)
        {
            if (_context.MangaChapters == null)
            {
                return NotFound();
            }
            MangaChapters? mangaChapter = await _context.MangaChapters.FindAsync(id);

            if (mangaChapter == null)
            {
                return NotFound();
            }
            List<MangaChapters> fullChapterList = _context.MangaChapters.Where(e => e.MangaId == mangaChapter.MangaId).ToList();
            return mangaChapter.toDTO(fullChapterList);
        }
    }
}
