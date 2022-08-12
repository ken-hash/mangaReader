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
    /// API endpoints on retrieving database details about manga and mangachapters through one route "Manga"
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MangasController : ControllerBase
    {
        private readonly MangaReaderBareBoneContext _context;

        public MangasController(MangaReaderBareBoneContext context)
        {
            _context = context;
        }

        //retrieving manga details in the database using manga id or manga name
        [HttpGet("mangas")]
        public async Task<ActionResult<Manga>> GetManga(int? id, string? name)
        {
            if (_context.Mangas == null || (!id.HasValue && string.IsNullOrEmpty(name)))
            {
                return NotFound();
            }
            if (id.HasValue)
            {
                var manga = await _context.Mangas.FindAsync(id);
                if (manga == null)
                {
                    return NotFound();
                }
                manga.Chapters = GetMangaChapters(manga!.MangaId) ?? new List<MangaChapters>();
                return manga;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                var manga = _context.Mangas.FirstOrDefault(e => e.Name == name);
                if (manga == null)
                {
                    return NotFound();
                }
                manga.Chapters = GetMangaChapters(manga!.MangaId)??new List<MangaChapters>();
                return manga;

            }
            else
            {
                return NotFound();
            }
        }

        //retrieve manga chapter details using mangaid
        //will list all chapters if no chaptername is provided
        private List<MangaChapters>? GetMangaChapters(int? mangaId, string? chapterName=null)
        {
            if (string.IsNullOrEmpty(chapterName))
            {
                return _context.MangaChapters?.Where(e => e.MangaId == mangaId).ToList();
            }
            else
            {
                return _context.MangaChapters?.Where(e => e.MangaId == mangaId && e.MangaChapter == chapterName).ToList();
            }
        }


        //retrieving mangachapters using mangaid and chaptername
        [HttpGet("chapters")]
        public async Task<ActionResult<List<MangaChapters>>> GetChapters(int? mangaId, string? mangaName, string? chapterName)
        {
            if (_context.Mangas == null||(!mangaId.HasValue && string.IsNullOrEmpty(mangaName)))
            {
                return NotFound();
            }

            if (mangaId.HasValue)
            {
                var manga = await _context.Mangas.FindAsync(mangaId);
                if (manga == null)
                {
                    return NotFound();
                }
                var mangaChapters = GetMangaChapters(mangaId, chapterName);
                if (mangaChapters != null)
                {
                    return mangaChapters;
                }
                else
                {
                    return NotFound();
                }
            }
            else if (!string.IsNullOrEmpty(mangaName))
            {
                var manga = await _context.Mangas.FirstOrDefaultAsync(e=>e.Name == mangaName);
                if (manga == null)
                {
                    return NotFound();
                }
                var mangaChapters = GetMangaChapters(manga.MangaId, chapterName);
                if (mangaChapters != null)
                {
                    return mangaChapters;
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }

        //unused for barebone
        [HttpPut("{id}")]
        public IActionResult PutManga(int id, Manga manga)
        {
            return Problem("Disabled Put");
        }

        [HttpPost]
        public ActionResult<Manga> PostManga(Manga manga)
        {
          return Problem("Disabled Post");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteManga(int id)
        {
            return Problem("Disabled Deletion");
        }

    }
}
