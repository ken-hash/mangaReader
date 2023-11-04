using MangaReaderBareBone.Controllers.Extensions;
using MangaReaderBareBone.Data;
using MangaReaderBareBone.DTO;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NaturalSort.Extension;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

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
        [ResponseCache(Duration = 5)]
        public async Task<ActionResult<List<MangasDTO>>> GetMangaList(int? max)
        {
            var connectionString = "Data Source=192.168.50.11,1433;User ID=KenDev;Password='kiss3825!';Initial Catalog = UbuntuServer;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Persist Security Info=True;MultipleActiveResultSets=true";
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                var storedProcedure = "sp_getAllLatestManga";
                IEnumerable<MangaLogsChapterView> res = await dbConnection.QueryAsync<MangaLogsChapterView>(storedProcedure, commandType: CommandType.StoredProcedure);

                if (!res.Any())
                    return NotFound();
                List<MangasDTO> mangaListDTO = new();
                var mangaListDict = res.GroupBy(m => m.Name!).ToDictionary(k => k.Key, v => v.ToList());
                foreach (var manga in mangaListDict)
                {
                    if (!mangaListDTO.Any(e => e.MangaName == manga.Key))
                    {
                        mangaListDict.TryGetValue(manga.Key, out var mangaLogView);
                        var innerList = mangaLogView!.GroupBy(e => e.Status!).ToDictionary(k => k.Key, v => v.ToList());
                        innerList!.TryGetValue("Added", out var mangaLogAdded);
                        innerList!.TryGetValue("Read", out var mangaLogRead);
                        mangaListDTO.Add(new MangasDTO
                        {
                            MangaName = manga.Key,
                            ChaptersAdded = new List<string?> { mangaLogAdded!.OrderByDescending(e => e.LogDateTime).First().ChapterName },
                            LastChapterRead = mangaLogRead?.OrderByDescending(e => e.LogDateTime).First()?.ChapterName
                        });
                    }
                }
                mangaListDTO = mangaListDTO.OrderBy(e => e.MangaName).ToList();
                return mangaListDTO;
            }
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
        [ResponseCache(Duration = 60 * 2, VaryByQueryKeys = new[] { "id", "name" })]
        public async Task<ActionResult<List<MangasDTO>>> SearchManga(int? id, string? name)
        {
            if (_context.Mangas == null || (!id.HasValue && string.IsNullOrEmpty(name)))
                return NotFound();
            var clean = name!.Trim().ToLower().Replace(" ", "-");
            var mangaList = await (from mlcv in _context.MangaLogsChapterView
                                   where mlcv.MangaId == id || mlcv.Name!.Contains(clean)
                                   select (mlcv)).ToListAsync();
            if (mangaList.Count == 0)
                return NotFound();
            List<MangasDTO> mangaListDTO = new();
            var mangaListDict = mangaList.GroupBy(m => m.Name!).ToDictionary(k => k.Key, v => v.ToList());


            foreach (var manga in mangaListDict)
            {
                if (!mangaListDTO.Any(e => e.MangaName == manga.Key))
                {
                    mangaListDict.TryGetValue(manga.Key, out var mangaLogView);
                    var innerList = mangaLogView!.GroupBy(e => e.Status!).ToDictionary(k => k.Key, v => v.ToList());
                    innerList!.TryGetValue("Added", out var mangaLogAdded);
                    innerList!.TryGetValue("Read", out var mangaLogRead);
                    mangaListDTO.Add(new MangasDTO
                    {
                        MangaName = manga.Key,
                        ChaptersAdded = new List<string?> { mangaLogAdded!.OrderByDescending(e => e.LogDateTime).First().ChapterName },
                        LastChapterRead = mangaLogRead?.OrderByDescending(e => e.LogDateTime).First()?.ChapterName
                    });
                }
            }
            return mangaListDTO.OrderBy(e => e.MangaName).ToList();
        }
    }
}
