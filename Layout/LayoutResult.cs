using System.Collections.Generic;

namespace WordCloudGenerator.Layout;

public sealed class LayoutResult
{
    public int PlacedCount { get; }
    public IReadOnlyList<WordEntry> Skipped { get; }

    public LayoutResult(int placedCount, IReadOnlyList<WordEntry> skipped)
    {
        PlacedCount = placedCount;
        Skipped = skipped;
    }
}