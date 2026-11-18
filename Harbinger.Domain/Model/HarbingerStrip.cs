using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Model;

public record HarbingerStrip(HarFram Frame, IReadOnlyCollection<HarbingerStripLink> Links)
{
    public static HarStripLinkTurn Turn(int index, XY xy, int turnIndex) => new HarStripLinkTurn(
        LED: new HarbingerLED(xy),
        LEDIndex: index,
        TurnIndex: turnIndex
        );
    public static HarStripLinkCol Col(int index, XY xy, int columnIndex) => new HarStripLinkCol(
        LED: new HarbingerLED(xy),
        LEDIndex: index,
        ColumnIndex: columnIndex
        );

}




public closed record HarbingerStripLink(HarbingerLED LED, int LEDIndex) : IPlaceable
{
    public XY Coordinate => LED.Coordinate;
}

public record HarbingerStripVerticalLink(HarbingerLED LED, int LEDIndex, int ColumnIndex) : HarbingerStripLink(LED, LEDIndex: LEDIndex);

public record HarbingerStripTurnLink(HarbingerLED LED, int LEDIndex, int TurnIndex) : HarbingerStripLink(LED, LEDIndex);

