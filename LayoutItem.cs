using System.Drawing;

namespace WordCloudGenerator;

public class LayoutItem
{
    public WordEntry Entry { get; }
    public Point Location { get; }
    public Rectangle Measured { get; }
    public double FontSize { get; }

    public LayoutItem(WordEntry entry, Point location, Rectangle measured, double fontSize)
    {
        Entry = entry;
        Location = location;
        Measured = measured;
        FontSize = fontSize;
    }

    public Rectangle Rectangle => new Rectangle(Location, Measured.Size);
}