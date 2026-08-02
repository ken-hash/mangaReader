namespace MangaReaderBareBone.Helpers
{
    public static class FuzzySearch
    {
        public static int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            source = source.ToLowerInvariant();
            target = target.ToLowerInvariant();

            int[] v0 = new int[target.Length + 1];
            int[] v1 = new int[target.Length + 1];

            for (int i = 0; i <= target.Length; i++)
            {
                v0[i] = i;
            }

            for (int i = 0; i < source.Length; i++)
            {
                v1[0] = i + 1;

                for (int j = 0; j < target.Length; j++)
                {
                    int cost = (source[i] == target[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(v1[j] + 1, Math.Min(v0[j + 1] + 1, v0[j] + cost));
                }

                for (int j = 0; j <= target.Length; j++)
                {
                    v0[j] = v1[j];
                }
            }

            return v1[target.Length];
        }

        public static double CalculateSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                return source == target ? 1.0 : 0.0;
            }

            int maxLength = Math.Max(source.Length, target.Length);
            if (maxLength == 0) return 1.0;

            int distance = LevenshteinDistance(source, target);
            return 1.0 - ((double)distance / maxLength);
        }

        public static bool IsMatch(string source, string target, double threshold = 0.5)
        {
            if (string.IsNullOrEmpty(target)) return true;
            if (string.IsNullOrEmpty(source)) return false;

            if (source.Contains(target, StringComparison.OrdinalIgnoreCase))
                return true;

            return CalculateSimilarity(source, target) >= threshold;
        }
    }
}
