namespace WordCloudGenerator.Tests;

public class WordCloudTests
{
    [Fact]
    public void Generate_ReturnsEmptyForNoEntries()
    {
        var cloud = new WordCloud(TestHelpers.CreateSettings());

        var result = cloud.Generate([]);

        Assert.Empty(result.Placed);
        Assert.Empty(result.Skipped);
    }

    [Fact]
    public void Generate_PlacesWordsInsideCanvas()
    {
        const int width = 500;
        const int height = 400;
        var settings = TestHelpers.CreateSettings(width, height);
        var cloud = new WordCloud(settings);
        var entries = new List<WordEntry>
        {
            new("design", 40),
            new("code", 25),
            new("test", 15),
            new("build", 10),
            new("ship", 5),
        };

        var result = cloud.Generate(entries);

        Assert.NotEmpty(result.Placed);
        Assert.All(result.Placed, item =>
        {
            Assert.True(item.Rectangle.Left >= 0);
            Assert.True(item.Rectangle.Top >= 0);
            Assert.True(item.Rectangle.Right <= width);
            Assert.True(item.Rectangle.Bottom <= height);
        });
    }

    [Fact]
    public void Generate_DoesNotOverlapPlacedWords()
    {
        var cloud = new WordCloud(TestHelpers.CreateSettings(600, 500));
        var entries = new List<WordEntry>
        {
            new("alpha", 80),
            new("beta", 60),
            new("gamma", 45),
            new("delta", 30),
            new("epsilon", 20),
            new("zeta", 12),
            new("eta", 8),
            new("theta", 4),
        };

        var result = cloud.Generate(entries);

        for (var i = 0; i < result.Placed.Count; i++)
        {
            for (var j = i + 1; j < result.Placed.Count; j++)
            {
                Assert.False(
                    TestHelpers.RectanglesOverlap(result.Placed[i].Rectangle, result.Placed[j].Rectangle),
                    $"Expected '{result.Placed[i].Entry.Word}' and '{result.Placed[j].Entry.Word}' not to overlap.");
            }
        }
    }

    [Fact]
    public void Generate_AssignsLargerFontToMoreFrequentWords()
    {
        var settings = TestHelpers.CreateSettings(800, 600, minFont: 10, maxFont: 50);
        var cloud = new WordCloud(settings);
        var entries = new List<WordEntry>
        {
            new("popular", 100),
            new("rare", 1),
        };

        var result = cloud.Generate(entries);
        var popular = result.Placed.Single(i => i.Entry.Word == "popular");
        var rare = result.Placed.Single(i => i.Entry.Word == "rare");

        Assert.True(popular.FontSize > rare.FontSize);
    }

    [Fact]
    public void Generate_UsesMostOfCanvasArea()
    {
        const int width = 500;
        const int height = 400;
        var settings = TestHelpers.CreateSettings(width, height, minFont: 12, maxFont: 42);
        var cloud = new WordCloud(settings);
        var entries = Enumerable.Range(1, 20)
            .Select(i => new WordEntry($"word{i}", 100 - i))
            .ToList();

        var result = cloud.Generate(entries);

        Assert.True(result.Placed.Count >= 10);

        var bounds = Rectangle.FromLTRB(
            result.Placed.Min(item => item.Rectangle.Left),
            result.Placed.Min(item => item.Rectangle.Top),
            result.Placed.Max(item => item.Rectangle.Right),
            result.Placed.Max(item => item.Rectangle.Bottom));

        Assert.True(bounds.Width >= width * 0.6, $"Expected wider spread, got {bounds.Width}px of {width}px.");
        Assert.True(bounds.Height >= height * 0.6, $"Expected taller spread, got {bounds.Height}px of {height}px.");
    }

    [Fact]
    public void Generate_UsesProvidedMeasurer()
    {
        var measured = new List<string>();
        var settings = new WordCloudSettings(300, 300, 10, 30, (text, _) =>
        {
            measured.Add(text);
            return TestHelpers.MeasureText(text, 20);
        });
        var cloud = new WordCloud(settings);

        cloud.Generate([new WordEntry("custom", 10)]);

        Assert.Contains("custom", measured);
    }

    [Fact]
    public void Generate_WithSkipOverflow_PlacesMoreWordsThanStop()
    {
        var settings = TestHelpers.CreateSettings(120, 80, minFont: 14, maxFont: 28);
        var entries = Enumerable.Range(1, 12)
            .Select(i => new WordEntry($"term{i}", 50 - i))
            .ToList();

        settings.OverflowBehavior = OverflowBehavior.Skip;
        var skipResult = new WordCloud(settings).Generate(entries);

        settings.OverflowBehavior = OverflowBehavior.Stop;
        var stopResult = new WordCloud(settings).Generate(entries);

        Assert.True(skipResult.Placed.Count > stopResult.Placed.Count);
        Assert.NotEmpty(skipResult.Skipped);
        Assert.NotEmpty(stopResult.Skipped);
        Assert.Equal(entries.Count, skipResult.Placed.Count + skipResult.Skipped.Count);
        Assert.Equal(entries.Count, stopResult.Placed.Count + stopResult.Skipped.Count);
    }

    [Fact]
    public void Generate_WithStopOverflow_SkipsRemainingWordsAfterFirstFailure()
    {
        var settings = TestHelpers.CreateSettings(80, 60, minFont: 16, maxFont: 30);
        settings.OverflowBehavior = OverflowBehavior.Stop;
        var entries = new List<WordEntry>
        {
            new("one", 100),
            new("two", 90),
            new("three", 80),
            new("four", 70),
            new("five", 60),
            new("six", 50),
        };

        var result = new WordCloud(settings).Generate(entries);

        Assert.NotEmpty(result.Placed);
        Assert.NotEmpty(result.Skipped);
        Assert.DoesNotContain(result.Skipped, entry => result.Placed.Any(item => item.Entry.Word == entry.Word));
    }
}