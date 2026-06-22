using WordCloudGenerator.Sizer;

namespace WordCloudGenerator.Tests;

public class LogSizerTests
{
    [Fact]
    public void GetFontSize_ReturnsMinFontForLeastFrequentWord()
    {
        var settings = TestHelpers.CreateSettings(minFont: 10, maxFont: 40);
        var entries = new List<WordEntry>
        {
            new("alpha", 1),
            new("beta", 10),
            new("gamma", 100),
        };

        var sizer = new LogSizer(settings, entries);

        Assert.Equal(10, sizer.GetFontSize(1));
    }

    [Fact]
    public void GetFontSize_ReturnsMaxFontForMostFrequentWord()
    {
        var settings = TestHelpers.CreateSettings(minFont: 10, maxFont: 40);
        var entries = new List<WordEntry>
        {
            new("alpha", 1),
            new("beta", 10),
            new("gamma", 100),
        };

        var sizer = new LogSizer(settings, entries);

        Assert.Equal(40, sizer.GetFontSize(100));
    }

    [Fact]
    public void GetFontSize_ReturnsMaxFontWhenAllCountsAreEqual()
    {
        var settings = TestHelpers.CreateSettings(minFont: 12, maxFont: 30);
        var entries = new List<WordEntry>
        {
            new("one", 5),
            new("two", 5),
            new("three", 5),
        };

        var sizer = new LogSizer(settings, entries);

        Assert.Equal(30, sizer.GetFontSize(5));
    }

    [Fact]
    public void GetFontSize_ScalesBetweenMinAndMax()
    {
        var settings = TestHelpers.CreateSettings(minFont: 10, maxFont: 40);
        var entries = new List<WordEntry>
        {
            new("low", 1),
            new("high", 100),
        };

        var sizer = new LogSizer(settings, entries);
        var midSize = sizer.GetFontSize(10);

        Assert.InRange(midSize, 10, 40);
        Assert.True(midSize > 10);
        Assert.True(midSize < 40);
    }
}