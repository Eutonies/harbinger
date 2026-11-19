using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Model;

public record HarbingerCoordinate(int X, int Y)
{

    public override string ToString() => $"({X},{Y})";

    public static XY operator +(XY fst, XY snd) => new XY(fst.X + snd.X, fst.Y + snd.Y);
    public static XY operator -(XY fst, XY snd) => new XY(fst.X - snd.X, fst.Y - snd.Y);
    public static XY operator -(XY xy) => new XY(-xy.X, -xy.Y);

    public static XY operator *(XY xy, int fact) => new XY(xy.X * fact, xy.Y * fact);
    public static XY operator /(XY xy, int fact) => new XY(xy.X / fact, xy.Y / fact);

    public static implicit operator XY((int, int) itm) => new XY(itm.Item1, itm.Item2);


}
