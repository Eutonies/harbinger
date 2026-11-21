using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Model;

public record HarbingerStrip(HarFram Frame, IReadOnlyCollection<HarbingerStripLink> Links)
{
    public static HarStripLinkTurn Turn(int index, XY xy) => new HarStripLinkTurn(
        LED: new HarbingerLED(xy),
        Index: index
        );
    public static HarStripLinkCol Col(int index, XY xy) => new HarStripLinkCol(
        LED: new HarbingerLED(xy),
        Index: index
        );

}




public abstract record HarbingerStripLink(HarbingerLED LED, int Index);

public record HarbingerStripVerticalLink(HarbingerLED LED, int Index) : HarbingerStripLink(LED, Index);

public record HarbingerStripTurnLink(HarbingerLED LED, int Index) : HarbingerStripLink(LED, Index);

