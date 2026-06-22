using System;
using System.Drawing;

namespace WordCloudGenerator.Layout;

public class SpiralLayout : BaseLayout
{
    public SpiralLayout(WordCloudSettings settings)
        : base(settings)
    {
    }

    public override bool TryFindFreeRectangle(Size size, out Rectangle foundRectangle)
    {
        foundRectangle = Rectangle.Empty;
        var angle = GetPseudoRandomStartAngle(size);
        var maxRadius = Math.Sqrt(Center.X * Center.X + Center.Y * Center.Y);

        const int pointsOnSpiral = 1000;

        for (var i = 0; i < pointsOnSpiral; i++)
        {
            var radius = i / (double)pointsOnSpiral * maxRadius;
            var dx = radius * Math.Sin(angle);
            var dy = radius * Math.Cos(angle);
            foundRectangle = new Rectangle(
                (int)(Center.X + dx - size.Width / 2.0),
                (int)(Center.Y + dy - size.Height / 2.0),
                size.Width,
                size.Height);
            angle += Math.PI / 60.0;

            if (!IsInsideSurface(foundRectangle))
                continue;

            if (!IsTaken(foundRectangle))
                return true;
        }

        return false;
    }

    private static double GetPseudoRandomStartAngle(Size size) => size.Height * size.Width;
}