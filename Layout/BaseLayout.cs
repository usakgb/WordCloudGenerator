using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WordCloudGenerator.Sizer;

namespace WordCloudGenerator.Layout;

public abstract class BaseLayout : ILayout
{
    protected QuadTree<LayoutItem> QuadTree { get; }
    protected Point Center { get; }
    protected Rectangle Surface { get; }
    protected WordCloudSettings Settings { get; }

    protected BaseLayout(WordCloudSettings settings)
    {
        var size = new Size(settings.Width, settings.Height);
        Surface = new Rectangle(new Point(0, 0), size);
        QuadTree = new QuadTree<LayoutItem>(Surface);
        Center = new Point(Surface.X + size.Width / 2, Surface.Y + size.Height / 2);
        Settings = settings;
    }

    public abstract bool TryFindFreeRectangle(Size size, out Rectangle foundRectangle);

    public LayoutResult Arrange(WordCloudSettings settings, IEnumerable<WordEntry> entries, IFontSizer sizer)
    {
        var orderedEntries = entries
            .OrderByDescending(entry => sizer.GetFontSize(entry.Count))
            .ThenByDescending(entry => entry.Word.Length)
            .ToList();
        var skipped = new List<WordEntry>();

        for (var index = 0; index < orderedEntries.Count; index++)
        {
            var entry = orderedEntries[index];
            var fontSize = sizer.GetFontSize(entry.Count);
            var measured = settings.Measurer?.Measure(entry.Word, fontSize)
                ?? settings.Measure!(entry.Word, fontSize);

            if (!TryFindFreeRectangle(measured.Size, out var foundRectangle))
            {
                if (settings.OverflowBehavior == OverflowBehavior.Stop)
                {
                    skipped.AddRange(orderedEntries.Skip(index));
                    break;
                }

                skipped.Add(entry);
                continue;
            }

            QuadTree.Insert(new LayoutItem(entry, foundRectangle.Location, measured, fontSize));
        }

        return new LayoutResult(QuadTree.Count, skipped);
    }

    public IEnumerable<LayoutItem> GetWordsInArea(Rectangle area) => QuadTree.Query(area);

    protected bool IsInsideSurface(Rectangle targetRectangle) => IsInside(Surface, targetRectangle);

    protected bool IsTaken(Rectangle targetRectangle) => QuadTree.HasContent(targetRectangle);

    private static bool IsInside(Rectangle outer, Rectangle inner) =>
        inner.X >= outer.X
        && inner.Y >= outer.Y
        && inner.Bottom <= outer.Bottom
        && inner.Right <= outer.Right;
}