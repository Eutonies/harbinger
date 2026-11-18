using System.Text;

namespace Harbinger.UI.Util;

public static class SVGExtensions
{


    public static string ToCoordinateString(this XY xy) => $"{xy.X} {xy.Y}";

    public static string ToCoordinateString(this HarLED led) => led.Coordinate.ToCoordinateString();
    public static string ToCoordinateString(this HarStripLink lnk) => lnk.Coordinate.ToCoordinateString();


    public static string ToLineCommand(this XY xy) => $"L {xy.ToCoordinateString()}";
    public static string ToLineCommand(this HarLED led) => led.Coordinate.ToLineCommand();
    public static string ToLineCommand(this HarStripLink lnk) => lnk.Coordinate.ToLineCommand();

    public static string ToMoveCommand(this XY xy) => $"M {xy.ToCoordinateString()}";
    public static string ToMoveCommand(this HarLED led) => led.Coordinate.ToMoveCommand();
    public static string ToMoveCommand(this HarStripLink lnk) => lnk.Coordinate.ToMoveCommand();



}
