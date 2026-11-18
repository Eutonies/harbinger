namespace Harbinger.UI.Persistence.Strip;

public record HarbingerStripColumnLinkSmo(
    int ColumnIndex,
    int LEDIndex,
    int X,
    int Y
    )
{
}


public static class HarbingerStripColumnLinkSmoExtensions
{
    public static HarStripLinkCol ToDomain(this HarbingerStripColumnLinkSmo link) => new HarStripLinkCol(
        LED: new HarLED(
            Coordinate: new XY(link.X, link.Y)
        ),
        LEDIndex: link.LEDIndex,
        ColumnIndex: link.ColumnIndex
    );

    public static HarbingerStripColumnLinkSmo ToSMO(this HarStripLinkCol link) => new HarbingerStripColumnLinkSmo(
        ColumnIndex: link.ColumnIndex,
        LEDIndex: link.LEDIndex,
        X: link.LED.Coordinate.X,
        Y: link.LED.Coordinate.Y
    );

}
