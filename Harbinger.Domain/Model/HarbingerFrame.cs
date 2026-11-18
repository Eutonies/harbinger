using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Model;

public record HarbingerFrame(int MinX, int MinY, int MaxX, int MaxY)
{
    public HarbingerFrame(int width, int height) : this(MinX: 0, MinY: 0, MaxX: width, MaxY: height) { }

    public static HarFram From(int minX, int minY, int maxX, int maxY) => new HarFram(minX, minY, maxX, maxY);
    public static HarFram From(int width, int height) => new HarFram(width, height);

    public readonly XY TopLeft = new XY(MinX, MinY);
    public readonly XY TopRight = new XY(MaxX, MinY);
    public readonly XY BottomLeft = new XY(MinX, MaxY);
    public readonly XY BottomRight = new XY(MaxX, MaxX);

    public readonly int Width = MaxX - MinX;
    public readonly int Height = MaxY - MinY;

    private decimal? _widthDecimal;
    public decimal WidthDecimal => _widthDecimal ??= Width;

    private decimal? _heightDecimal;
    public decimal HeightDecimal => _heightDecimal ??= Height;

    /// <summary>
    /// At least for now, we assume that frame boundaries run parallel to X- and Y- axis respectively
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsWithinFrame(XY point) => IsWithinFrameOnX(point) && IsWithinFrameOnY(point);

    public bool IsWithinFrameOnX(XY point) => point.X >= MinX && point.X <= MaxX;
    public bool IsWithinFrameOnY(XY point) => point.Y >= MinY && point.Y <= MaxY;

    public bool IsSameAs(HarFram oth) => MinX == oth.MinX && MinY == oth.MinY && MaxX == oth.MaxX && MaxY == oth.MaxY;


    /// <summary>
    /// Mapping be: to frame / from frame
    /// </summary>
    /// <param name="framTo"></param>
    /// <param name="framFrom"></param>
    /// <returns></returns>
    public static HarMap operator /(HarFram framTo, HarFram framFrom) => new HarMap(From: framFrom, To: framTo);


}
