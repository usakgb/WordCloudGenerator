namespace WordCloudGenerator.Tests;

public class QuadTreeTests
{
    [Fact]
    public void Insert_IncreasesCount()
    {
        var tree = new QuadTree<LayoutItem>(new Rectangle(0, 0, 200, 200));

        tree.Insert(TestHelpers.CreateItem(1, 10, 10, 20, 20));
        tree.Insert(TestHelpers.CreateItem(2, 50, 50, 20, 20));

        Assert.Equal(2, tree.Count);
    }

    [Fact]
    public void Query_ReturnsItemsThatIntersectArea()
    {
        var tree = new QuadTree<LayoutItem>(new Rectangle(0, 0, 200, 200));
        var first = TestHelpers.CreateItem(1, 10, 10, 20, 20);
        var second = TestHelpers.CreateItem(2, 100, 100, 20, 20);
        tree.Insert(first);
        tree.Insert(second);

        var results = tree.Query(new Rectangle(0, 0, 50, 50)).ToList();

        Assert.Single(results);
        Assert.Same(first, results[0]);
    }

    [Fact]
    public void HasContent_ReturnsTrueWhenAreaOverlapsItem()
    {
        var tree = new QuadTree<LayoutItem>(new Rectangle(0, 0, 200, 200));
        tree.Insert(TestHelpers.CreateItem(1, 30, 30, 20, 20));

        Assert.True(tree.HasContent(new Rectangle(35, 35, 10, 10)));
        Assert.False(tree.HasContent(new Rectangle(150, 150, 10, 10)));
    }

    [Fact]
    public void Insert_OutsideBounds_Throws()
    {
        var tree = new QuadTree<LayoutItem>(new Rectangle(0, 0, 100, 100));
        var outside = TestHelpers.CreateItem(1, 90, 90, 20, 20);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Insert(outside));
    }
}