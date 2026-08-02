using System.Text.RegularExpressions;

namespace MangaReaderBareBone.Helpers
{
    public sealed class ChapterSortKey : IComparable<ChapterSortKey>
    {
        private const int TierText = 0;
        private const int TierNumeric = 1;
        private const int TierSeasonal = 2;

        private const int SubTypeBase = 0;
        private const int SubTypeDecimal = 1;
        private const int SubTypeSuffix = 2;
        private const int SubTypeSegment = 3;
        private const int SubTypeFallback = 4;

        private static readonly Regex SeasonPattern = new(@"^S(\d+)-(\d+)(?:\.(\d+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SegmentPattern = new(@"^(\d+)-s(\d+)-(.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DecimalPattern = new(@"^(\d+)\.(\d+)$", RegexOptions.Compiled);
        private static readonly Regex SuffixPattern = new(@"^(\d+)-(\d+)$", RegexOptions.Compiled);
        private static readonly Regex FirstNumberPattern = new(@"\d+", RegexOptions.Compiled);

        public int Tier { get; private init; }
        public int PrefixPriority { get; private init; }
        public string Prefix { get; private init; } = string.Empty;
        public int Season { get; private init; }
        public long BaseNumber { get; private init; }
        public int SubType { get; private init; }
        public long SubValue { get; private init; }
        public string Label { get; private init; } = string.Empty;

        private ChapterSortKey() { }

        public static ChapterSortKey Parse(string? chapter)
        {
            chapter = chapter?.Trim() ?? string.Empty;
            if (chapter.Length == 0)
            {
                return new ChapterSortKey { Tier = TierText, PrefixPriority = 1 };
            }

            var m = SeasonPattern.Match(chapter);
            if (m.Success && TryParseNumber(m.Groups[1].Value, out var season) && TryParseNumber(m.Groups[2].Value, out var seasonBase))
            {
                long seasonDecimal = 0;
                var hasDecimal = m.Groups[3].Success && TryParseNumber(m.Groups[3].Value, out seasonDecimal);
                return new ChapterSortKey
                {
                    Tier = TierSeasonal,
                    Season = (int)season,
                    BaseNumber = seasonBase,
                    SubType = hasDecimal ? SubTypeDecimal : SubTypeBase,
                    SubValue = hasDecimal ? seasonDecimal : 0
                };
            }

            m = SegmentPattern.Match(chapter);
            if (m.Success && TryParseNumber(m.Groups[1].Value, out var segmentBase) && TryParseNumber(m.Groups[2].Value, out var segmentNumber))
            {
                return new ChapterSortKey
                {
                    Tier = TierNumeric,
                    BaseNumber = segmentBase,
                    SubType = SubTypeSegment,
                    SubValue = segmentNumber,
                    Label = m.Groups[3].Value
                };
            }

            m = DecimalPattern.Match(chapter);
            if (m.Success && TryParseNumber(m.Groups[1].Value, out var decimalBase) && TryParseNumber(m.Groups[2].Value, out var decimalPart))
            {
                return new ChapterSortKey
                {
                    Tier = TierNumeric,
                    BaseNumber = decimalBase,
                    SubType = SubTypeDecimal,
                    SubValue = decimalPart
                };
            }

            m = SuffixPattern.Match(chapter);
            if (m.Success && TryParseNumber(m.Groups[1].Value, out var suffixBase) && TryParseNumber(m.Groups[2].Value, out var suffixPart))
            {
                return new ChapterSortKey
                {
                    Tier = TierNumeric,
                    BaseNumber = suffixBase,
                    SubType = SubTypeSuffix,
                    SubValue = suffixPart
                };
            }

            if (chapter.All(c => c >= '0' && c <= '9') && TryParseNumber(chapter, out var pureNumber))
            {
                return new ChapterSortKey
                {
                    Tier = TierNumeric,
                    BaseNumber = pureNumber
                };
            }

            m = FirstNumberPattern.Match(chapter);
            if (m.Success && TryParseNumber(m.Value, out var fallbackNumber))
            {
                var prefix = chapter[..m.Index];
                return new ChapterSortKey
                {
                    Tier = TierNumeric,
                    PrefixPriority = prefix.Length == 0 ? 0 : 1,
                    Prefix = prefix,
                    BaseNumber = fallbackNumber,
                    SubType = SubTypeFallback,
                    Label = chapter
                };
            }

            return new ChapterSortKey
            {
                Tier = TierText,
                PrefixPriority = 1,
                Prefix = chapter,
                Label = chapter
            };
        }

        public int CompareTo(ChapterSortKey? other)
        {
            if (other is null) return 1;

            var result = Tier.CompareTo(other.Tier);
            if (result != 0) return result;

            result = PrefixPriority.CompareTo(other.PrefixPriority);
            if (result != 0) return result;

            result = string.CompareOrdinal(Prefix, other.Prefix);
            if (result != 0) return result;

            result = Season.CompareTo(other.Season);
            if (result != 0) return result;

            result = BaseNumber.CompareTo(other.BaseNumber);
            if (result != 0) return result;

            result = SubType.CompareTo(other.SubType);
            if (result != 0) return result;

            result = SubValue.CompareTo(other.SubValue);
            if (result != 0) return result;

            return string.CompareOrdinal(Label, other.Label);
        }

        private static bool TryParseNumber(string text, out long value)
        {
            return long.TryParse(text, out value);
        }
    }

    public sealed class ChapterNaturalComparer : IComparer<string>
    {
        public static readonly ChapterNaturalComparer Instance = new();

        public int Compare(string? x, string? y)
        {
            return ChapterSortKey.Parse(x).CompareTo(ChapterSortKey.Parse(y));
        }
    }

    public static class ChapterOrdering
    {
        private static readonly TimeSpan Epsilon = TimeSpan.FromMilliseconds(100);

        public static DateTime ComputeInsertDateTime(
            IEnumerable<(string ChapterName, DateTime DateTime)> existingChapters,
            string newChapterName)
        {
            var newKey = ChapterSortKey.Parse(newChapterName);

            var ordered = existingChapters
                .Select(e => (Key: ChapterSortKey.Parse(e.ChapterName), e.DateTime))
                .OrderBy(e => e.Key)
                .ToList();

            var successorIndex = ordered.FindIndex(e => e.Key.CompareTo(newKey) > 0);
            if (successorIndex < 0)
            {
                return DateTime.UtcNow;
            }

            var successor = ordered[successorIndex].DateTime;
            if (successorIndex == 0)
            {
                return successor - Epsilon;
            }

            var predecessor = ordered[successorIndex - 1].DateTime;
            if (predecessor >= successor)
            {
                return successor - Epsilon;
            }

            var gap = successor - predecessor;
            if (gap > Epsilon + Epsilon)
            {
                return predecessor + TimeSpan.FromTicks(gap.Ticks / 2);
            }

            var candidate = successor - Epsilon;
            return candidate > predecessor ? candidate : predecessor + TimeSpan.FromTicks(gap.Ticks / 2);
        }
    }
}
