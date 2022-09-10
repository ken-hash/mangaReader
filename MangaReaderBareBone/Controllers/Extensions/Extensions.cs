using MangaReaderBareBone.DTO;
using MangaReaderBareBone.Models;

namespace MangaReaderBareBone.Controllers.Extensions
{
    /// <summary>
    /// UNUSED
    /// extension for ienumerable to return async task instead of sync
    /// </summary>
    public static class Extensions
    {
        public static async Task<IEnumerable<T>> Where<T>(this IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            (T x, bool)[] results = await Task.WhenAll(source.Select(async x => (x, await predicate(x))));
            return results.Where(x => x.Item2).Select(x => x.Item1);
        }

        /// <summary>
        /// Extension to transform Manga Object to MangaDTO 
        /// </summary>
        /// <param name="manga"></param>
        /// <param name="lastRead"></param>
        /// <returns></returns>
        public static MangasDTO toDTO(this Manga manga, MangaChapters? lastRead)
        {
            MangasDTO mangaDTO = new MangasDTO
            {
                MangaName = manga.Name,
                ChaptersAdded = manga.Chapters?.toChapterList(),
                LastChapterRead = lastRead?.MangaChapter
            };
            return mangaDTO;
        }

        /// <summary>
        /// Extension to transform MangaChapter Object to MangaChaptersDTO
        /// by retreiving previous/next chapters based on arrangement of
        /// all Added chapters of the selected manga by date
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="fullChapterList"></param>
        /// <returns></returns>
        public static MangaChapterDTO toDTO(this MangaChapters chapter, IEnumerable<MangaChapters> fullChapterList)
        {
            MangaChapterDTO mangaChapterDTO = new MangaChapterDTO
            {
                MangaChapter = chapter.MangaChapter,
                Path = chapter.Path,
            };
            int chapterIndex = fullChapterList.ToList().FindIndex(e => e.MangaChaptersId == chapter.MangaChaptersId);
            if (chapterIndex == fullChapterList.Count() - 1)
            {
                mangaChapterDTO.PreviousChapter = fullChapterList.ToList()[chapterIndex - 1].MangaChapter;
            }
            else if (chapterIndex == 0)
            {
                mangaChapterDTO.NextChapter = fullChapterList.ToList()[chapterIndex + 1].MangaChapter;
            }
            else
            {
                mangaChapterDTO.NextChapter = fullChapterList.ToList()[chapterIndex + 1].MangaChapter;
                mangaChapterDTO.PreviousChapter = fullChapterList.ToList()[chapterIndex - 1].MangaChapter;
            }
            return mangaChapterDTO;
        }

        /// <summary>
        /// Extension to transform MangaChapterList into MangaChaptersDTOList
        /// </summary>
        /// <param name="chapterList"></param>
        /// <param name="fullChapterList"></param>
        /// <returns></returns>
        public static List<MangaChapterDTO> toDTOList(this List<MangaChapters> chapterList, IEnumerable<MangaChapters> fullChapterList)
        {
            List<MangaChapterDTO> mangaChapterDTOList = new List<MangaChapterDTO>();
            foreach (MangaChapters chapter in chapterList)
            {
                mangaChapterDTOList.Add(chapter.toDTO(fullChapterList));
            }
            return mangaChapterDTOList;
        }

        /// <summary>
        /// Extension to strip MangaChaptersList into List of Strings of MangaChapters
        /// </summary>
        /// <param name="chapterList"></param>
        /// <returns></returns>
        public static List<string?> toChapterList(this IEnumerable<MangaChapters> chapterList)
        {
            List<string?> result = new List<string?>();
            foreach (MangaChapters chapter in chapterList)
            {
                result.Add(chapter.MangaChapter);
            }
            return result;
        }
    }
}
