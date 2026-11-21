using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Model;

public record HarbingerCoordinate(int X, int Y)
{

    public override string ToString() => $"({X},{Y})";

    public decimal DiagonalLength(XY oth)
    {
        var diagonalVector = this - oth;
        var horizontalDiffSquared = (diagonalVector.X ^ 2).ToDecimal();
        var verticalDiffSquard = (diagonalVector.Y ^ 2).ToDecimal();
        var diagonalLength = (horizontalDiffSquared + verticalDiffSquard).Sqrt();
        return diagonalLength;
    }

    public decimal DiagonalSideLength(decimal lengthOfDiagonal, decimal knownSide)
    {
        var returnee = (lengthOfDiagonal.Pow2() - knownSide.Pow2()).Sqrt();
        return returnee;
    }

    public XY ShiftDiagonallyWithKnownWidth(decimal lengthOfDiagonal, decimal lengthOfDiagonalWidth, HarDir direction)
    {
        var height = DiagonalSideLength(lengthOfDiagonal: lengthOfDiagonal, knownSide: lengthOfDiagonalWidth);
        var shift = new XY(X: lengthOfDiagonalWidth.ToInt(), Y: height.ToInt()) * direction;
        var returnee = this + shift;
        return returnee;
    }

    public XY ShiftDiagonallyWithKnownHeight(decimal lengthOfDiagonal, decimal lengthOfDiagonalHeight, HarDir direction)
    {
        var width = DiagonalSideLength(lengthOfDiagonal: lengthOfDiagonal, knownSide: lengthOfDiagonalHeight);
        var shift = new XY(X: width.ToInt(), Y: lengthOfDiagonalHeight.ToInt()) * direction;
        var returnee = this + shift;
        return returnee;
    }


    public static XY operator +(XY fst, XY snd) => new XY(fst.X + snd.X, fst.Y + snd.Y);
    public static XY operator -(XY fst, XY snd) => new XY(fst.X - snd.X, fst.Y - snd.Y);
    public static XY operator -(XY xy) => new XY(-xy.X, -xy.Y);

    public static XY operator *(XY xy, int fact) => new XY(xy.X * fact, xy.Y * fact);
    public static XY operator *(XY xy, HarDir dir) => dir.ToUnitVector()
        .Pipe(un => new XY(xy.X * un.X, xy.Y * un.Y));
    public static XY operator /(XY xy, int fact) => new XY(xy.X / fact, xy.Y / fact);

    public static implicit operator XY((int, int) itm) => new XY(itm.Item1, itm.Item2);




}
