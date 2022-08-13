using MangaReaderBareBone.Data;
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
        public async Task<ActionResult<MangaChapters>> GetMangaChapters(int id)
        {
            if (_context.MangaChapters == null)
            {
                return NotFound();
            }
            MangaChapters? mangaChapters = await _context.MangaChapters.FindAsync(id);

            if (mangaChapters == null)
            {
                return NotFound();
            }

            return mangaChapters;
        }
    }
}
