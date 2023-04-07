using MangaReaderBareBone.Controllers.Extensions;
using MangaReaderBareBone.Data;
using MangaReaderBareBone.DTO;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Linq;

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
        public async Task<ActionResult<MangasDTO>> GetManga(int? id, string? name)
        {
            if (_context.Mangas == null || (!id.HasValue && string.IsNullOrEmpty(name)))
            {
                return NotFound();
            }
            Manga? manga = null;
            if (id.HasValue)
            {
                manga = await _context.Mangas.FindAsync(id);
                if (manga == null)
                {
                    return NotFound();
                }
                manga.Chapters = GetMangaChapters(manga!.MangaId)!;

            }
            else if (!string.IsNullOrEmpty(name))
            {
                manga = _context.Mangas.FirstOrDefault(e => e.Name.ToLower() == name.ToLower());
                if (manga == null)
                {
                    return NotFound();
                }
            }
            MangaChapters? lastReadChapter = getLastReadChapter(manga!);
            return manga!.toDTO(lastReadChapter);
        }

        //retrieve mangalist with last added chapter with parameter max number of manga to retrieve
        [HttpGet("mangaList")]
        public ActionResult<List<MangasDTO>> GetMangaList(int? max)
        {
            if (_context.Mangas == null || !max.HasValue || max <= 0)
            {
                return NotFound();
            }
            IQueryable<Manga> mangaList = _context.Mangas.OrderBy(e => e.MangaId).Take(max ?? 1);
                                        
            List<MangasDTO> mangaListDTO = new();
            foreach (var m in mangaList)
            {
                mangaListDTO.Add(new MangasDTO
                {
                    MangaName = m.Name,
                    ChaptersAdded = new List<string?> { getLastAddedChapter(m)?.MangaChapter },
                    LastChapterRead = getLastReadChapter(m)?.MangaChapter
                });
            }
            return mangaListDTO;
        }

        //retrieve manga chapter details using mangaid
        //will list all chapters if no chaptername is provided
        private List<MangaChapters>? GetMangaChapters(int? mangaId, string? chapterName = null, int? maxChapters = 1, bool isReversed = false)
        {
            if (maxChapters <= 0)
            {
                return new List<MangaChapters>();
            }
            if (string.IsNullOrEmpty(chapterName))
            {
                var logsAndChapters = (from mL in _context.MangaLogs
                                       join mC in _context.MangaChapters! on mL.MangaChaptersId equals mC.MangaChaptersId
                                       select new
                                       {
                                           mangaLogs = mL,
                                           mangaChapters = mC
                                       }
                                       ).Where(e => e.mangaLogs.Status == "Added" && e.mangaLogs.MangaId == mangaId).OrderBy(e=>e.mangaLogs.DateTime);

                if (isReversed)
                    logsAndChapters.Reverse();
                return logsAndChapters.Select(e => e.mangaChapters).Take(maxChapters ?? 1).ToList();
            }
            else
            {
                return _context.MangaChapters?.Where(e => e.MangaId == mangaId && e.MangaChapter!.ToLower() == chapterName.ToLower()).OrderBy(e => e.MangaChaptersId).Take(maxChapters ?? 1).ToList();
            }
        }


        private MangaChapters? getLastReadChapter(Manga manga)
        {
            var logsAndChapters = (from mL in _context.MangaLogs
                                   join mC in _context.MangaChapters! on mL.MangaChaptersId equals mC.MangaChaptersId
                                   select new
                                   {
                                       mangaLogs = mL,
                                       mangaChapters = mC
                                   }
                                   ).Where(e => e.mangaLogs.Status == "Read" && e.mangaLogs.MangaId == manga.MangaId).OrderBy(e => e.mangaLogs.DateTime).LastOrDefault(); ;
            return logsAndChapters?.mangaChapters;
        }
        private MangaChapters? getLastAddedChapter(Manga manga)
        {
            var logsAndChapters = (from mL in _context.MangaLogs
                                   join mC in _context.MangaChapters! on mL.MangaChaptersId equals mC.MangaChaptersId
                                   select new
                                   {
                                       mangaLogs = mL,
                                       mangaChapters = mC
                                   }
                                   ).Where(e => e.mangaLogs.Status == "Added" && e.mangaLogs.MangaId == manga.MangaId).OrderBy(e => e.mangaLogs.DateTime).LastOrDefault();
            return logsAndChapters?.mangaChapters;
        }

        //retrieving mangachapters using mangaid and chaptername
        [HttpGet("chapters")]
        public async Task<ActionResult<List<MangaChapterDTO>>> GetChapters(int? mangaId, string? mangaName, string? chapterName)
        {
            if (_context.Mangas == null || (!mangaId.HasValue && string.IsNullOrEmpty(mangaName)))
            {
                return NotFound();
            }
            Manga? manga = null;
            if (mangaId.HasValue)
            {
                manga = await _context.Mangas.FindAsync(mangaId);
            }
            else if (!string.IsNullOrEmpty(mangaName))
            {
                manga = _context.Mangas.FirstOrDefault(e => e.Name!.ToLower() == mangaName.ToLower());
            }
            if (manga == null)
            {
                return NotFound();
            }
            List<MangaChapters>? mangaChapters = string.IsNullOrWhiteSpace(chapterName) ? GetMangaChapters(manga.MangaId, chapterName, 10000) : GetMangaChapters(manga.MangaId, chapterName);

            var logsAndChapters = (from mL in _context.MangaLogs
                                   join mC in _context.MangaChapters! on mL.MangaChaptersId equals mC.MangaChaptersId
                                   select new
                                   {
                                       MangaLog = mL,
                                       MangaChapters = mC
                                   }).Where(e => e.MangaLog.Status == "Added" && e.MangaLog.MangaId == manga.MangaId);

            List<MangaChapters> fullChapterList = logsAndChapters!.ToList().OrderBy(e => e.MangaLog.DateTime).Select(e => e.MangaChapters).ToList();
            if (mangaChapters != null && fullChapterList != null)
            {
                return mangaChapters.toDTOList(fullChapterList);
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

        [HttpGet("Search")]
        public async Task<ActionResult<List<MangasDTO>>> SearchManga(int? id, string? name)
        {
            if (_context.Mangas == null || (!id.HasValue && string.IsNullOrEmpty(name)))
            {
                return NotFound();
            }
            List<MangasDTO> searchResultsDTO = new List<MangasDTO>();
            if (id.HasValue)
            {
                Manga? manga = await _context.Mangas.FindAsync(id);
                if (manga == null)
                {
                    return NotFound();
                }
                manga.Chapters = GetMangaChapters(manga!.MangaId) ?? new List<MangaChapters>();
                MangaChapters? lastRead = getLastReadChapter(manga);
                searchResultsDTO.Add(manga.toDTO(lastRead));
                return searchResultsDTO;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                List<Manga> searchResults = _context.Mangas.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
                if (searchResults == null)
                {
                    return NotFound();
                }
                foreach (Manga manga in searchResults)
                {
                    MangaChapters? lastRead = getLastReadChapter(manga);
                    searchResultsDTO.Add(manga.toDTO(lastRead));
                }
                return searchResultsDTO;
            }
            else
            {
                return NotFound();
            }
        }
    }
}
