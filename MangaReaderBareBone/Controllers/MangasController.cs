﻿using MangaReaderBareBone.Controllers.Extensions;
using MangaReaderBareBone.Data;
using MangaReaderBareBone.DTO;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalSort.Extension;

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
                return NotFound();
            Manga? manga = null;
            if (id.HasValue)
            {
                manga = await _context.Mangas.FindAsync(id);
                if (manga == null)
                    return NotFound();
                manga.Chapters = await GetMangaChapters(manga!.MangaId)!;

            }
            else if (!string.IsNullOrEmpty(name))
            {
                manga = await _context.Mangas.SingleOrDefaultAsync(e => string.Equals(e.Name, name));
                if (manga == null)
                    return NotFound();
            }
            MangaChapters? lastReadChapter = await getLastReadChapter(manga!);
            return manga!.toDTO(lastReadChapter);
        }

        //retrieve mangalist with last added chapter with parameter max number of manga to retrieve
        [HttpGet("mangaList")]
        [ResponseCache(Duration = 60 * 10)]
        public async Task<ActionResult<List<MangasDTO>>> GetMangaList(int? max)
        {
            if (_context.Mangas == null || !max.HasValue || max <= 0)
                return NotFound();
            var queryListAdded = await _context.MangaLogs!
                .Where(mL => mL.Status == "Added")
                .GroupBy(mL => new { mL.MangaId })
                .Select(g => new { g.Key.MangaId, MaxDateTime = g.Max(mL => mL.DateTime) })
                .OrderBy(e => e.MangaId)
                .Distinct()
                .ToListAsync();
            var mangaList = queryListAdded.Select(x => new { x.MangaId, x.MaxDateTime })
                .Join(_context.MangaLogs!,
                    x => new { x.MangaId, x.MaxDateTime },
                    m => new { m.MangaId, MaxDateTime = m.DateTime },
                    (x, m) => new { x.MaxDateTime, x.MangaId, m.MangaChaptersId })
                .Join(_context.Mangas!,
                    x => x.MangaId,
                    m => m.MangaId,
                    (x, m) => new { x.MaxDateTime, x.MangaId, x.MangaChaptersId, m.Name })
                .Join(_context.MangaChapters!,
                    x => x.MangaChaptersId,
                    c => c.MangaChaptersId,
                    (x, c) => new { x.Name, c.MangaChapter, x.MaxDateTime, x.MangaId })
                .ToList().OrderBy(e => e.Name);

            var queryListRead = await _context.MangaLogs!
                .Where(mL => mL.Status == "Read")
                .GroupBy(mL => new { mL.MangaId })
                .Select(g => new { g.Key.MangaId, MaxDateTime = g.Max(mL => mL.DateTime) })
                .OrderBy(e => e.MangaId)
                .Distinct()
                .ToListAsync();
            var mangaListRead = queryListRead.Select(x => new { x.MangaId, x.MaxDateTime })
                .Join(_context.MangaLogs!,
                    x => new { x.MangaId, x.MaxDateTime },
                    m => new { m.MangaId, MaxDateTime = m.DateTime },
                    (x, m) => new { x.MaxDateTime, x.MangaId, m.MangaChaptersId })
                .Join(_context.MangaChapters!,
                    x => x.MangaChaptersId,
                    c => c.MangaChaptersId,
                    (x, c) => new {c.MangaChapter, x.MaxDateTime, x.MangaId })
                .ToList();

            List<MangasDTO> mangaListDTO = new();
            foreach (var m in mangaList)
            {
                mangaListDTO.Add(new MangasDTO
                {
                    MangaName = m.Name,
                    ChaptersAdded = new List<string?> { m.MangaChapter },
                    LastChapterRead = mangaListRead.FirstOrDefault(e=>e.MangaId==m.MangaId)?.MangaChapter
                });
            }
            return mangaListDTO;
        }

        //retrieve manga chapter details using mangaid
        //will list all chapters if no chaptername is provided
        private async Task<List<MangaChapters>?> GetMangaChapters(int? mangaId, string? chapterName = null, int? maxChapters = 1, bool isReversed = false)
        {
            if (maxChapters <= 0)
                return new List<MangaChapters>();
            if (string.IsNullOrEmpty(chapterName))
            {
                var logsAndChapters = await (from mL in _context.MangaLogs
                                             join mC in _context.MangaChapters! on mL.MangaChaptersId equals mC.MangaChaptersId
                                             where mL.Status == "Added" && mL.MangaId == mangaId
                                             orderby mL.DateTime
                                             select new
                                             {
                                                 mangaLogs = mL,
                                                 mangaChapters = mC
                                             }
                                       ).ToListAsync();
                if (isReversed)
                    logsAndChapters.Reverse();
                return logsAndChapters.Select(e => e.mangaChapters).Take(maxChapters ?? 1).ToList();
            }
            else
                return await _context.MangaChapters!.Where(e => e.MangaId == mangaId && string.Equals(e.MangaChapter, chapterName)).ToListAsync();
        }


        private async Task<MangaChapters?> getLastReadChapter(Manga manga)
        {
            var logsAndChapters = await (from mL in _context.MangaLogs
                                         join mC in _context.MangaChapters! on mL.MangaChaptersId equals mC.MangaChaptersId
                                         where mL.Status == "Read" && mL.MangaId == manga.MangaId
                                         orderby mL.DateTime descending
                                         select new
                                         {
                                             mangaLogs = mL,
                                             mangaChapters = mC
                                         }
                                   ).FirstOrDefaultAsync();
            return logsAndChapters?.mangaChapters;
        }

        private async Task<MangaChapters?> getLastReadChapter(int mangaId)
        {
            var logsAndChapters = (from mL in _context.MangaLogs
                                         join mC in _context.MangaChapters! on mL.MangaChaptersId equals mC.MangaChaptersId
                                         where mL.Status == "Read" && mL.MangaId == mangaId
                                         orderby mL.DateTime descending
                                         select new
                                         {
                                             mangaLogs = mL,
                                             mangaChapters = mC
                                         }
                                   ).FirstOrDefault();
            return logsAndChapters?.mangaChapters;
        }

        //retrieving mangachapters using mangaid and chaptername
        [HttpGet("chapters")]
        public async Task<ActionResult<List<MangaChapterDTO>>> GetChapters(int? mangaId, string? mangaName, string? chapterName)
        {
            if (_context.Mangas == null || (!mangaId.HasValue && string.IsNullOrEmpty(mangaName)))
                return NotFound();
            Manga? manga = null;
            if (mangaId.HasValue)
                manga = await _context.Mangas.FindAsync(mangaId);
            else if (!string.IsNullOrEmpty(mangaName))
                manga = await _context.Mangas.SingleOrDefaultAsync(e => string.Equals(e.Name, mangaName));
            if (manga == null)
                return NotFound();
            List<MangaChapters>? mangaChapters = string.IsNullOrWhiteSpace(chapterName) ? await GetMangaChapters(manga.MangaId, chapterName, 10000) : await GetMangaChapters(manga.MangaId, chapterName);

            var logsAndChapters = await (from mL in _context.MangaLogs
                                         join mC in _context.MangaChapters! on mL.MangaChaptersId equals mC.MangaChaptersId
                                         where mL.Status == "Added" && mL.MangaId == manga.MangaId
                                         select new
                                         {
                                             MangaLog = mL,
                                             MangaChapters = mC
                                         }).ToListAsync();

            List<MangaChapters> fullChapterList = logsAndChapters!.ToList().OrderBy(e => e.MangaLog.DateTime).Select(e => e.MangaChapters).ToList();
            if (mangaChapters != null && fullChapterList != null)
                return mangaChapters.toDTOList(fullChapterList);
            else
                return NotFound();
        }

        [HttpGet("Search")]
        public async Task<ActionResult<List<MangasDTO>>> SearchManga(int? id, string? name)
        {
            if (_context.Mangas == null || (!id.HasValue && string.IsNullOrEmpty(name)))
                return NotFound();
            List<MangasDTO> searchResultsDTO = new();
            if (id.HasValue)
            {
                Manga? manga = await _context.Mangas.FindAsync(id);
                if (manga == null)
                    return NotFound();
                manga.Chapters = await GetMangaChapters(manga!.MangaId) ?? new List<MangaChapters>();
                MangaChapters? lastRead = await getLastReadChapter(manga);
                searchResultsDTO.Add(manga.toDTO(lastRead));
                return searchResultsDTO;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                var mangaList = await _context.Mangas
                    .Where(m => m.Name!.ToLower().Contains(name.ToLower()))
                    .Select(e => new { e.Name , e.MangaId})
                    .ToListAsync();
                var comparer = new NaturalSortComparer(StringComparison.OrdinalIgnoreCase);
                foreach (var manga in mangaList)
                {
                    var lastRead = await getLastReadChapter(manga.MangaId);
                    var chapters = _context.MangaChapters!.Where(e => e.MangaId == manga.MangaId).Select(f => f.MangaChapter).ToList();
                    chapters.Sort(comparer);
                    chapters.Reverse();
                    searchResultsDTO.Add(new MangasDTO
                    {
                        MangaName = manga.Name,
                        LastChapterRead = lastRead?.MangaChapter,

                        ChaptersAdded = chapters,
                    });
                }
                return searchResultsDTO;
            }
            else
                return NotFound();
        }
    }
}
