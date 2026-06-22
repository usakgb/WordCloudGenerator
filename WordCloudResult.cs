using System.Collections.Generic;

namespace WordCloudGenerator;

public sealed class WordCloudResult
{
    public IReadOnlyList<LayoutItem> Placed { get; }
    public IReadOnlyList<WordEntry> Skipped { get; }

    public WordCloudResult(IReadOnlyList<LayoutItem> placed, IReadOnlyList<WordEntry> skipped)
    {
        Placed = placed;
        Skipped = skipped;
    }
}