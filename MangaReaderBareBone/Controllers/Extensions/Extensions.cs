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
    }
}
