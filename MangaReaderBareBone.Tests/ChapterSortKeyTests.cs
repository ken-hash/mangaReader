using System.Linq;
using MangaReaderBareBone.Helpers;

namespace MangaReaderBareBone.Tests;

[TestFixture]
public class ChapterSortKeyTests
{
    [Test]
    public void PureNumbers_AreSortedNumerically()
    {
        var chapters = new[] { "10", "2", "1", "20", "3" };
        var sorted = chapters.OrderBy(x => x, ChapterNaturalComparer.Instance).ToArray();
        Assert.That(sorted, Is.EqualTo(new[] { "1", "2", "3", "10", "20" }));
    }

    [Test]
    public void NullAndEmpty_GoToFront()
    {
        var chapters = new[] { "10", null!, "", "5" };
        var sorted = chapters.OrderBy(x => x, ChapterNaturalComparer.Instance).ToArray();
        Assert.That(sorted, Is.EqualTo(new[] { null, "", "5", "10" }));
    }

    [Test]
    public void Decimals_SortAfterBaseNumber()
    {
        var chapters = new[] { "62.5", "62" };
        var sorted = chapters.OrderBy(x => x, ChapterNaturalComparer.Instance).ToArray();
        Assert.That(sorted, Is.EqualTo(new[] { "62", "62.5" }));
    }

    [Test]
    public void Suffix_SortsAfterBaseButBeforeNextNumber()
    {
        var chapters = new[] { "63", "62-2", "62" };
        var sorted = chapters.OrderBy(x => x, ChapterNaturalComparer.Instance).ToArray();
        Assert.That(sorted, Is.EqualTo(new[] { "62", "62-2", "63" }));
    }

    [Test]
    public void SeasonChapters_SortBySeasonThenNumber()
    {
        var chapters = new[] { "S3-264.1", "S2-180.5", "S2-180" };
        var sorted = chapters.OrderBy(x => x, ChapterNaturalComparer.Instance).ToArray();
        Assert.That(sorted, Is.EqualTo(new[] { "S2-180", "S2-180.5", "S3-264.1" }));
    }

    [Test]
    public void SegmentChapters_SortAfterBaseNumber()
    {
        var chapters = new[] { "55", "54-s1-start", "55-s2-end", "54" };
        var sorted = chapters.OrderBy(x => x, ChapterNaturalComparer.Instance).ToArray();
        Assert.That(sorted, Is.EqualTo(new[] { "54", "54-s1-start", "55", "55-s2-end" }));
    }

    [Test]
    public void TextChapters_SortBeforeNumbers()
    {
        var chapters = new[] { "5", "prologue", "cover" };
        var sorted = chapters.OrderBy(x => x, ChapterNaturalComparer.Instance).ToArray();
        Assert.That(sorted, Is.EqualTo(new[] { "cover", "prologue", "5" }));
    }

    [Test]
    public void PrefixedNumbers_SortAfterPureNumbers()
    {
        var chapters = new[] { "ch5", "5", "ch3" };
        var sorted = chapters.OrderBy(x => x, ChapterNaturalComparer.Instance).ToArray();
        Assert.That(sorted, Is.EqualTo(new[] { "5", "ch3", "ch5" }));
    }

    [Test]
    public void CommissionSortKey_IsConsistent()
    {
        var chapters = new[] { "chapter-1100", "chapter-1050", "S3-264.1", "62.5", "54-s1-end" };
        var sorted = chapters.OrderBy(x => x, ChapterNaturalComparer.Instance).ToArray();
        Assert.That(sorted.Length, Is.EqualTo(5));
        Assert.That(sorted, Is.Unique);
    }

    [Test]
    public void ComputeInsertDateTime_NewChapter_PlacedBetweenNeighbours()
    {
        var existing = new List<(string ChapterName, DateTime DateTime)>
        {
            ("1", new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)),
            ("3", new DateTime(2024, 1, 1, 10, 2, 0, DateTimeKind.Utc)),
        };

        var result = ChapterOrdering.ComputeInsertDateTime(existing, "2");

        Assert.That(result, Is.GreaterThan(new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)));
        Assert.That(result, Is.LessThan(new DateTime(2024, 1, 1, 10, 2, 0, DateTimeKind.Utc)));
    }

    [Test]
    public void ComputeInsertDateTime_TailChapter_UsesUtcNow()
    {
        var existing = new List<(string ChapterName, DateTime DateTime)>
        {
            ("1", new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)),
        };

        var before = DateTime.UtcNow;
        var result = ChapterOrdering.ComputeInsertDateTime(existing, "99");
        var after = DateTime.UtcNow;

        Assert.That(result, Is.InRange(before.AddSeconds(-1), after.AddSeconds(1)));
    }
}
