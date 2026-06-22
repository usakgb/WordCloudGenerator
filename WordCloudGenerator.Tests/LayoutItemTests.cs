using System.Drawing;

namespace WordCloudGenerator.Tests;

public class LayoutItemTests
{
    [Fact]
    public void Rectangle_ComposesLocationAndMeasuredSize()
    {
        var entry = new WordEntry("cloud", 7);
        var item = new LayoutItem(entry, new Point(15, 25), new Rectangle(0, 0, 80, 20), 18);

        Assert.Equal(new Rectangle(15, 25, 80, 20), item.Rectangle);
    }
}