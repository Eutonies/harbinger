using Harbinger.Domain.Util;

namespace Harbinger.UI.Persistence.Strip;

public record HarbingerStripSmo(
    int FrameMinX,
    int FrameMinY,
    int FrameMaxX,
    int FrameMaxY,
    IReadOnlyCollection<HarbingerStripColumnLinkSmo> Columns,
    IReadOnlyCollection<HarbingerStripTurnLinkSmo> Turns
    )
{


}



public static class HarbingerStripSmoExtensions
{
    public static HarStrip ToDomain(this HarbingerStripSmo strip) => new HarStrip(
        Frame: new HarFram(MinX: strip.FrameMinX, MinY: strip.FrameMinY, MaxX: strip.FrameMaxX, MaxY: strip.FrameMaxY),
        Links: strip.Columns
            .Select(_ => _.ToDomain() as HarStripLink)
            .Concat(strip.Turns
                .Select(_ => _.ToDomain())
            ).OrderBy(_ => _.LEDIndex)
        .ToArray()
        );

    public static HarbingerStripSmo ToSMO(this HarStrip strip) => strip.Frame
        .Pipe(fr => new HarbingerStripSmo(
            FrameMinX: fr.MinX,
            FrameMinY: fr.MinY,
            FrameMaxX: fr.MaxX,
            FrameMaxY: fr.MaxY,
            Columns: strip.Links
                .OfType<HarStripLinkCol>()
                .Select(_ => _.ToSMO())
                .ToArray(),
            Turns: strip.Links
                .OfType<HarStripLinkTurn>()
                .Select(_ => _.ToSMO())
                .ToArray()
        ));

}


