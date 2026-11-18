using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Model;

public record HarbingerCoordinate(int X, int Y)
{

    private ulong? _asLong;
    public ulong AsLong => _asLong ??= CalculateAsLong();
    
    private ulong CalculateAsLong()
    {
        ulong xVal = ExtractBytes(X);
        ulong yVal = ExtractBytes(Y) << 32;
        var returnee = yVal | xVal;
        return returnee;
    }
    private static ulong ExtractBytes(int intVal) => (ulong) intVal;

    public override string ToString() => $"({X},{Y})";

    public decimal DistanceTo(XY oth) => DiagonalLength(oth);

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
        try
        {
            var diagPow2 = lengthOfDiagonal.Pow2();
            var sidePow2 = knownSide.Pow2();
            /// In reality, columns actually move over a couple of LED's and the columns are not exactly straight lines, so in order to avoid NaN as result, we cut the model some slack
            var returnee = (knownSide > lengthOfDiagonal ?
                (sidePow2 - diagPow2) : 
                (diagPow2 - sidePow2)).Sqrt();
            return returnee;
        }
        catch(Exception)
        {
            Console.WriteLine($"WOW... What numbers man! diagonal: {lengthOfDiagonal.ToString("N5")}, side: {knownSide.ToString("N5")}");
            throw;
        }
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

    public virtual bool Equals(XY? oth) => oth?
        .Pipe(o => o.X == X && o.Y == Y) ?? false;

    public override int GetHashCode() => AsLong.GetHashCode();

    public IReadOnlyCollection<XY> InAllDirections => Enum.GetValues<HarDir>()
        .Select(dir => this * dir)
        .ToReadOnly();

    public static XY operator +(XY fst, XY snd) => new XY(fst.X + snd.X, fst.Y + snd.Y);
    public static XY operator -(XY fst, XY snd) => new XY(fst.X - snd.X, fst.Y - snd.Y);
    public static XY operator -(XY xy) => new XY(-xy.X, -xy.Y);

    public static XY operator *(XY xy, int fact) => new XY(xy.X * fact, xy.Y * fact);
    public static XY operator *(XY xy, HarDir dir) => dir.ToUnitVector()
        .Pipe(un => new XY(xy.X * un.X, xy.Y * un.Y));
    public static XY operator /(XY xy, int fact) => new XY(xy.X / fact, xy.Y / fact);

    public static implicit operator XY((int, int) itm) => new XY(itm.Item1, itm.Item2);




}
