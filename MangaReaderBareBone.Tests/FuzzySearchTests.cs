using MangaReaderBareBone.Helpers;

namespace MangaReaderBareBone.Tests;

[TestFixture]
public class FuzzySearchTests
{
    [Test]
    public void ExactMatch_HasSimilarityOfOne()
    {
        Assert.That(FuzzySearch.CalculateSimilarity("hello", "hello"), Is.EqualTo(1.0));
    }

    [Test]
    public void CompletelyDifferent_HasLowSimilarity()
    {
        Assert.That(FuzzySearch.CalculateSimilarity("hello", "zzzzz"), Is.LessThan(0.5));
    }

    [Test]
    public void CaseDifference_StillExactMatch()
    {
        Assert.That(FuzzySearch.CalculateSimilarity("Hello", "hello"), Is.EqualTo(1.0));
    }

    [Test]
    public void OneCharDifference_HasHighSimilarity()
    {
        var score = FuzzySearch.CalculateSimilarity("one-piece", "one-pieve");
        Assert.That(score, Is.GreaterThan(0.8));
    }

    [Test]
    public void EmptyStrings_HandleGracefully()
    {
        Assert.That(FuzzySearch.CalculateSimilarity("", ""), Is.EqualTo(1.0));
        Assert.That(FuzzySearch.CalculateSimilarity("hello", ""), Is.EqualTo(0.0));
        Assert.That(FuzzySearch.CalculateSimilarity("", "hello"), Is.EqualTo(0.0));
    }

    [Test]
    public void NullStrings_HandleGracefully()
    {
        Assert.That(FuzzySearch.CalculateSimilarity(null!, null!), Is.EqualTo(1.0));
        Assert.That(FuzzySearch.CalculateSimilarity("hello", null!), Is.EqualTo(0.0));
        Assert.That(FuzzySearch.CalculateSimilarity(null!, "hello"), Is.EqualTo(0.0));
    }

    [Test]
    public void IsMatch_SubstringMatch_ReturnsTrue()
    {
        Assert.That(FuzzySearch.IsMatch("one-piece", "one"), Is.True);
        Assert.That(FuzzySearch.IsMatch("one-piece", "O"), Is.True);
    }

    [Test]
    public void IsMatch_ThresholdBased_ReturnsTrueForCloseMatches()
    {
        Assert.That(FuzzySearch.IsMatch("one-piece", "one-pieve", 0.8), Is.True);
    }

    [Test]
    public void IsMatch_BelowThreshold_ReturnsFalse()
    {
        Assert.That(FuzzySearch.IsMatch("hello", "x", 0.5), Is.False);
    }

    [Test]
    public void LevenshteinDistance_CalculatesCorrectly()
    {
        Assert.That(FuzzySearch.LevenshteinDistance("kitten", "sitting"), Is.EqualTo(3));
        Assert.That(FuzzySearch.LevenshteinDistance("", "abc"), Is.EqualTo(3));
        Assert.That(FuzzySearch.LevenshteinDistance("abc", ""), Is.EqualTo(3));
        Assert.That(FuzzySearch.LevenshteinDistance("abc", "abc"), Is.EqualTo(0));
    }
}
