using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Model;

public enum HarbingerDirection
{
    Left = 10,
    Right = 20,
    Up = 30,
    Down = 40,
    DiagonalUpLeft = 50,
    DiagonalUpRight = 60,
    DiagonalDownLeft = 70,
    DiagonalDownRight = 80
}

public enum HarbingerLocation
{
    LeftSide = 10,
    RightSide = 20,
    Top = 30,
    Bottom = 40,
    TopLeftCorner = 50,
    TopRightCorner = 60,
    BottomLeftCorner = 70,
    BottomRightCorner = 80
}

public static class HarbingerDirections
{
    public static readonly XY Upwards = new XY(0, -1);
    public static readonly XY Downwards = new XY(0, 1);
    public static readonly XY Leftwards = new XY(-1, 0);
    public static readonly XY Rightwards = new XY(1, 0);

    public static readonly XY LeftUpwards = new XY(-1, -1);
    public static readonly XY LeftDownwards = new XY(-1, 1);
    public static readonly XY RightUpwards = new XY(1, -1);
    public static readonly XY RightDownwards = new XY(1, 1);

}


public static class HarbingerDirectionalExtensions
{
    public static XY StepUp(this XY xy) => xy + HarDirs.Upwards;
    public static XY StepDown(this XY xy) => xy + HarDirs.Downwards;
    public static XY StepLeft(this XY xy) => xy + HarDirs.Leftwards;
    public static XY StepRight(this XY xy) => xy + HarDirs.Rightwards;
    public static XY StepDiagonalLeftUp(this XY xy) => xy + HarDirs.LeftUpwards;
    public static XY StepDiagonalLeftDown(this XY xy) => xy + HarDirs.LeftDownwards;
    public static XY StepDiagonalRightUp(this XY xy) => xy + HarDirs.RightUpwards;
    public static XY StepDiagonalRightDown(this XY xy) => xy + HarDirs.RightDownwards;

    public static HarDir Opposite(this HarDir dir) => dir switch
    {
        HarDir.Down => HarDir.Up,
        HarDir.Up => HarDir.Down,
        HarDir.Left => HarDir.Right,
        HarDir.Right => HarDir.Left,
        HarDir.DiagonalDownLeft => HarDir.DiagonalUpRight,
        HarDir.DiagonalUpRight => HarDir.DiagonalDownLeft,
        HarDir.DiagonalDownRight => HarDir.DiagonalUpLeft,
        HarDir.DiagonalUpLeft => HarDir.DiagonalDownRight,
        _ => throw new ArgumentException($"What direction even is this?: {dir}!")
    };

    public static HarLoc Vertical(this HarLoc loc) => loc switch
    {
        HarLoc.Top | HarLoc.TopLeftCorner | HarLoc.TopRightCorner => HarLoc.Top,
        HarLoc.Bottom | HarLoc.BottomLeftCorner | HarLoc.BottomRightCorner => HarLoc.Bottom,
        _ => throw new ArgumentException($"Talkin' 'bout vertical location for {loc} is pure nonsense!")
    };
    public static HarLoc Horizontal(this HarLoc loc) => loc switch
    {
        HarLoc.LeftSide | HarLoc.TopLeftCorner | HarLoc.BottomLeftCorner => HarLoc.LeftSide,
        HarLoc.RightSide | HarLoc.BottomRightCorner | HarLoc.TopRightCorner => HarLoc.RightSide,
        _ => throw new ArgumentException($"Talkin' 'bout horizontal location for {loc} is pure nonsense!")
    };


    public static HarLoc Opposite(this HarLoc dir) => dir switch
    {
        HarLoc.Top => HarLoc.Bottom,
        HarLoc.Bottom => HarLoc.Top,
        HarLoc.LeftSide => HarLoc.RightSide,
        HarLoc.RightSide => HarLoc.LeftSide,
        HarLoc.TopLeftCorner => HarLoc.BottomRightCorner,
        HarLoc.BottomRightCorner => HarLoc.TopLeftCorner,
        HarLoc.TopRightCorner => HarLoc.BottomLeftCorner,
        HarLoc.BottomLeftCorner => HarLoc.TopRightCorner,
        _ => throw new ArgumentException($"What location even is this?: {dir}!")
    };

    public static HarDir LeadsInto(this HarLoc loc) => loc switch
    {
        HarLoc.Top => HarDir.Up,
        HarLoc.Bottom => HarDir.Down,
        HarLoc.LeftSide => HarDir.Left,
        HarLoc.RightSide => HarDir.Right,
        HarLoc.TopLeftCorner => HarDir.DiagonalUpLeft,
        HarLoc.BottomRightCorner => HarDir.DiagonalDownRight,
        HarLoc.TopRightCorner => HarDir.DiagonalUpRight,
        HarLoc.BottomLeftCorner => HarDir.DiagonalDownLeft,
        _ => throw new ArgumentException($"What location even is this?: {loc}!")
    };

    public static HarDir LeadsAwayFrom(this HarLoc loc) => loc.LeadsInto().Opposite();

}
