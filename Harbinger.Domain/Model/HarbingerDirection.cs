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

}
