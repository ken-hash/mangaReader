using MangaReaderBareBone.Controllers.Extensions;
using MangaReaderBareBone.Data;
using MangaReaderBareBone.DTO;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            List<MangasDTO> mangaListDTO = new List<MangasDTO>();
            mangaList.ToList().ForEach(manga => manga.Chapters = GetMangaChapters(manga.MangaId, maxChapters: 1, isReversed: true) ?? new List<MangaChapters>());
            List<Manga> noEmpty = mangaList.Where(manga => manga.Chapters.Count > 0).OrderBy(e => e.Name).ToList();
            noEmpty.ForEach(e => mangaListDTO.Add(e.toDTO(getLastReadChapter(e))));
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
                var logsAndChapters = _context.MangaLogs?.Join(_context.MangaChapters!, logs => logs.MangaChaptersId, chapters => chapters.MangaChaptersId, (logs, chapters) => new
                {
                    MangaLog = logs,
                    MangaChapters = chapters
                }).Where(logsAndChapters => logsAndChapters.MangaLog.Status == "Added" && logsAndChapters.MangaLog.MangaId == mangaId);
                var arrangedLogsAndChapters = logsAndChapters!.OrderBy(e => e.MangaLog.DateTime);
                List<MangaChapters>? latestChapterList = null;
                if (isReversed)
                {
                    latestChapterList = arrangedLogsAndChapters.Reverse().Select(e => e.MangaChapters).Take(maxChapters ?? 1).ToList();
                }
                else
                {
                    latestChapterList = arrangedLogsAndChapters.Select(e => e.MangaChapters).Take(maxChapters ?? 1).ToList();
                }

                return latestChapterList;
            }
            else
            {
                return _context.MangaChapters?.Where(e => e.MangaId == mangaId && e.MangaChapter.ToLower() == chapterName.ToLower()).OrderBy(e => e.MangaChaptersId).Take(maxChapters ?? 1).ToList();
            }
        }


        private MangaChapters? getLastReadChapter(Manga manga)
        {
            MangaLog? lastReadLog = _context.MangaLogs?.Where(e => e.MangaId == manga!.MangaId && e.Status == "Read").OrderBy(e => e.DateTime).LastOrDefault();
            MangaChapters? lastReadChapter = null;
            if (lastReadLog != null)
            {
                lastReadChapter = _context.MangaChapters?.FirstOrDefault(e => e.MangaChaptersId == lastReadLog.MangaChaptersId);
            }
            return lastReadChapter;
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
                manga = await _context.Mangas.FirstOrDefaultAsync(e => e.Name.ToLower() == mangaName.ToLower());
            }
            if (manga == null)
            {
                return NotFound();
            }
            List<MangaChapters>? mangaChapters = string.IsNullOrWhiteSpace(chapterName) ? GetMangaChapters(manga.MangaId, chapterName, 10000) : GetMangaChapters(manga.MangaId, chapterName);
            var logsAndChapters = _context.MangaLogs?.Join(_context.MangaChapters!, logs => logs.MangaChaptersId, chapters => chapters.MangaChaptersId, (logs, chapters) => new
            {
                MangaLog = logs,
                MangaChapters = chapters
            }).Where(logsAndChapters => logsAndChapters.MangaLog.Status == "Added" && logsAndChapters.MangaLog.MangaId == manga.MangaId);

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
