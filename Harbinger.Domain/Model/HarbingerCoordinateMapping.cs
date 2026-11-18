using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Model;

public record HarbingerCoordinateMapping(
    HarFram From,
    HarFram To
    )
{
    public XY this[XY inp] => Map(inp);

    public XY Map(XY xy) => new XY(X: MapX(xy), Y: MapY(xy));
    public int MapX(XY xy) => MapX(xy.X);
    public int MapY(XY xy) => MapY(xy.Y);


    public int MapX(int x) => Map(x, minFrom: From.MinX, totalLengthFrom: From.WidthDecimal, minTo: To.MinX, totalLengthTo: To.WidthDecimal);
    public int MapY(int y) => Map(y, minFrom: From.MinY, totalLengthFrom: From.HeightDecimal, minTo: To.MinY, totalLengthTo: To.HeightDecimal);

    private int Map(int coordPart, int minFrom, decimal totalLengthFrom, int minTo, decimal totalLengthTo) => ((coordPart.ToDecimal() - minFrom.ToDecimal()) / totalLengthFrom)
        .Pipe(divRes => divRes * totalLengthTo + minTo.ToDecimal())
        .Pipe(_ => _.ToInt());


}


public static class HarbingerCoordinateMappings
{

    public static XY Map(this XY xy, HarFram definedFor, HarFram mapTo)
    {
        var xRatio = (xy.X.ToDecimal() - definedFor.MinX.ToDecimal()) / definedFor.WidthDecimal;
        var yRatio = (xy.Y.ToDecimal() - definedFor.MinY.ToDecimal()) / definedFor.HeightDecimal;
        var mappedX = xRatio * mapTo.WidthDecimal + mapTo.MinX.ToDecimal();
        var mappedY = yRatio * mapTo.HeightDecimal + mapTo.MinY.ToDecimal();
        var returnee = new XY(mappedX.ToInt(), mappedY.ToInt());
        return returnee;
    }

}
