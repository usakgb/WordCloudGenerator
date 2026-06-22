using System.Collections.Generic;
using System.Linq;
using WordCloudGenerator.Layout;
using WordCloudGenerator.Sizer;

namespace WordCloudGenerator;

public class WordCloud
{
    public WordCloudSettings Settings { get; }

    public WordCloud(WordCloudSettings settings)
    {
        Settings = settings;
    }

    public WordCloudResult Generate(List<WordEntry> entries, ILayout? layout = null, IFontSizer? sizer = null)
    {
        sizer ??= new LogSizer(Settings, entries);
        layout ??= new SpiralLayout(Settings);
        var layoutResult = layout.Arrange(Settings, entries, sizer);
        var placed = layout.GetWordsInArea(new Rectangle(0, 0, Settings.Width, Settings.Height)).ToList();
        return new WordCloudResult(placed, layoutResult.Skipped);
    }
}