using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WordCloudGenerator;

public class QuadTreeNode<T> where T : LayoutItem
{
    private readonly Stack<T> _contents = new Stack<T>();

    private QuadTreeNode<T>[] _nodes = Array.Empty<QuadTreeNode<T>>();

    public bool IsEmpty
    {
        get
        {
            if (!Bounds.IsEmpty)
            {
                return _nodes.Length == 0;
            }
            return true;
        }
    }

    public Rectangle Bounds { get; }

    public int Count
    {
        get
        {
            int num = 0;
            QuadTreeNode<T>[] nodes = _nodes;
            foreach (QuadTreeNode<T> quadTreeNode in nodes)
            {
                num += quadTreeNode.Count;
            }
            return num + _contents.Count;
        }
    }

    public IEnumerable<T> SubTreeContents
    {
        get
        {
            IEnumerable<T> first = Enumerable.Empty<T>();
            QuadTreeNode<T>[] nodes = _nodes;
            foreach (QuadTreeNode<T> quadTreeNode in nodes)
            {
                first = first.Concat(quadTreeNode.SubTreeContents);
            }
            return first.Concat(_contents);
        }
    }

    public QuadTreeNode(Rectangle bounds)
    {
        Bounds = bounds;
    }

    public bool HasContent(Rectangle queryArea)
    {
        return Query(queryArea).Any();
    }

    public IEnumerable<T> Query(Rectangle queryArea)
    {
        foreach (T content in _contents)
        {
            if (queryArea.IntersectsWith(content.Rectangle))
            {
                yield return content;
            }
        }
        QuadTreeNode<T>[] nodes = _nodes;
        foreach (QuadTreeNode<T> quadTreeNode in nodes)
        {
            if (quadTreeNode.IsEmpty)
            {
                continue;
            }
            if (quadTreeNode.Bounds.Contains(queryArea))
            {
                IEnumerable<T> enumerable = quadTreeNode.Query(queryArea);
                foreach (T item in enumerable)
                {
                    yield return item;
                }
                break;
            }
            if (queryArea.Contains(quadTreeNode.Bounds))
            {
                IEnumerable<T> subTreeContents = quadTreeNode.SubTreeContents;
                foreach (T item2 in subTreeContents)
                {
                    yield return item2;
                }
            }
            else
            {
                if (!quadTreeNode.Bounds.IntersectsWith(queryArea))
                {
                    continue;
                }
                IEnumerable<T> enumerable2 = quadTreeNode.Query(queryArea);
                foreach (T item3 in enumerable2)
                {
                    yield return item3;
                }
            }
        }
    }

    public void Insert(T item)
    {
        if (!Bounds.Contains(item.Rectangle))
        {
            throw new ArgumentOutOfRangeException("item", "Feature is out of the bounds of this quadtree node.");
        }
        if (!_nodes.Any())
        {
            CreateSubNodes();
        }
        QuadTreeNode<T>[] nodes = _nodes;
        foreach (QuadTreeNode<T> quadTreeNode in nodes)
        {
            if (quadTreeNode.Bounds.Contains(item.Rectangle))
            {
                quadTreeNode.Insert(item);
                return;
            }
        }
        _contents.Push(item);
    }

    private void CreateSubNodes()
    {
        if (Bounds.Height * Bounds.Width > 10)
        {
            int num = Bounds.Width / 2;
            int num2 = Bounds.Height / 2;
            _nodes = new QuadTreeNode<T>[4]
            {
                new QuadTreeNode<T>(new Rectangle(Bounds.Location, new Size(num, num2))),
                new QuadTreeNode<T>(new Rectangle(new Point(Bounds.Left, Bounds.Top + num2), new Size(num, num2))),
                new QuadTreeNode<T>(new Rectangle(new Point(Bounds.Left + num, Bounds.Top), new Size(num, num2))),
                new QuadTreeNode<T>(new Rectangle(new Point(Bounds.Left + num, Bounds.Top + num2), new Size(num, num2)))
            };
        }
    }
}