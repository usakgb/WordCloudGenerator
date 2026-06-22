using System;
using System.Drawing;

namespace WordCloudGenerator;

public class WordCloudSettings
{
    public int MinFont { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int MaxFont { get; set; }
    public OverflowBehavior OverflowBehavior { get; set; } = OverflowBehavior.Skip;
    public Func<string, double, Rectangle>? Measure { get; set; }
    public IMeasurer? Measurer { get; set; }

    public WordCloudSettings(int width, int height, int minFont, int maxFont, IMeasurer measurer)
    {
        Width = width;
        Height = height;
        MinFont = minFont;
        MaxFont = maxFont;
        Measurer = measurer;
    }

    public WordCloudSettings(int width, int height, int minFont, int maxFont, Func<string, double, Rectangle> measure)
    {
        Width = width;
        Height = height;
        MinFont = minFont;
        MaxFont = maxFont;
        Measure = measure;
    }
}