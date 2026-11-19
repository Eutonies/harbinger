using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Model;

public record HarbingerFrame(int MinX, int MinY, int MaxX, int MaxY)
{
    public readonly XY TopLeft = new XY(MinX, MinY);
    public readonly XY TopRight = new XY(MaxX, MinY);
    public readonly XY BottomLeft = new XY(MinX, MaxY);
    public readonly XY BottomRight = new XY(MaxX, MaxX);

    public readonly int Width = MaxX - MinX;
    public readonly int Height = MaxY - MinY;

}
