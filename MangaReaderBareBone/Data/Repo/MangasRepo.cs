using MangaReaderBareBone.Models;
using System.Data.Entity;

namespace MangaReaderBareBone.Data.Repo
{
    public interface MangaRepo
    {
        Task<List<MangaLogsChapterView>> GetUpdatedManga(string status, int numberDays);
        Task<MangaChapters?> GetMangaChaptersByIdAndTitle(int id, string title);
        Task<Manga?> GetMangaByTitle(string title);
    }

    public class MangasRepo : MangaRepo
    {
        private readonly MangaReaderBareBoneContext? _context;

        public MangasRepo() 
        {
        }

        public MangasRepo(MangaReaderBareBoneContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Manga?> GetMangaByTitle(string title)
        {
            return await _context!.Mangas.SingleOrDefaultAsync(e => e.Name == title);
        }

        public async Task<MangaChapters?> GetMangaChaptersByIdAndTitle(int id, string title)
        {
            return await _context!.MangaChapters.SingleOrDefaultAsync(e => e.MangaId == id && e.MangaChapter == title);
        }

        public async Task<List<MangaLogsChapterView>> GetUpdatedManga(string status, int numberDays)
        {
            return await (from mlcv in _context?.MangaLogsChapterView
                  where mlcv.Status == status && mlcv.LogDateTime > DateTime.Today.AddDays(-(double)numberDays)
                  orderby mlcv.LogDateTime descending
                  select mlcv).ToListAsync();
        }
    }
}
