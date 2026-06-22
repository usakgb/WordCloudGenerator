namespace WordCloudGenerator.Tests;

internal static class TestHelpers
{
    public static WordCloudSettings CreateSettings(
        int width = 400,
        int height = 300,
        int minFont = 10,
        int maxFont = 40)
    {
        return new WordCloudSettings(width, height, minFont, maxFont, MeasureText);
    }

    public static Rectangle MeasureText(string text, double fontSize)
    {
        var width = (int)Math.Ceiling(text.Length * fontSize * 0.6);
        var height = (int)Math.Ceiling(fontSize);
        return new Rectangle(0, 0, Math.Max(width, 1), Math.Max(height, 1));
    }

    public static LayoutItem CreateItem(int id, int x, int y, int width, int height)
    {
        var entry = new WordEntry($"word{id}", id);
        var measured = new Rectangle(0, 0, width, height);
        return new LayoutItem(entry, new Point(x, y), measured, 12);
    }

    public static bool RectanglesOverlap(Rectangle a, Rectangle b)
    {
        return a.IntersectsWith(b)
            && !(a.Right <= b.Left || b.Right <= a.Left || a.Bottom <= b.Top || b.Bottom <= a.Top);
    }
}