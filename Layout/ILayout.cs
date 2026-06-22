using System.Collections.Generic;
using System.Drawing;
using WordCloudGenerator.Sizer;

namespace WordCloudGenerator.Layout;

public interface ILayout
{
    LayoutResult Arrange(WordCloudSettings settings, IEnumerable<WordEntry> entries, IFontSizer sizer);

    IEnumerable<LayoutItem> GetWordsInArea(Rectangle area);
}