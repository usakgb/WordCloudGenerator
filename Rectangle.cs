namespace WordCloudGenerator;

public readonly struct Rectangle
{
    public static readonly Rectangle Empty = default;

    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }

    public int Left => X;
    public int Top => Y;
    public int Right => X + Width;
    public int Bottom => Y + Height;

    public Point Location => new Point(X, Y);

    public Size Size => new Size(Width, Height);

    public bool IsEmpty => Width <= 0 || Height <= 0;

    public Rectangle(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Rectangle(Point location, Size size)
        : this(location.X, location.Y, size.Width, size.Height)
    {
    }

    public static Rectangle FromLTRB(int left, int top, int right, int bottom) =>
        new Rectangle(left, top, right - left, bottom - top);

    public bool Contains(Rectangle other) =>
        X <= other.X
        && Y <= other.Y
        && Right >= other.Right
        && Bottom >= other.Bottom;

    public bool IntersectsWith(Rectangle other) =>
        Left < other.Right
        && Right > other.Left
        && Top < other.Bottom
        && Bottom > other.Top;
}