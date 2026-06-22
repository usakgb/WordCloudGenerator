using System.Collections.Generic;

namespace WordCloudGenerator;

public class QuadTree<T> where T : LayoutItem
{
    private readonly Rectangle _rectangle;

    private readonly QuadTreeNode<T> _root;

    public int Count => _root.Count;

    public QuadTree(Rectangle rectangle)
    {
        _rectangle = rectangle;
        _root = new QuadTreeNode<T>(_rectangle);
    }

    public void Insert(T item)
    {
        _root.Insert(item);
    }

    public IEnumerable<T> Query(Rectangle area)
    {
        return _root.Query(area);
    }

    public bool HasContent(Rectangle area)
    {
        return _root.HasContent(area);
    }
}