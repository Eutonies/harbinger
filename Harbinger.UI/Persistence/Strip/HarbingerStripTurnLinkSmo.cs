namespace Harbinger.UI.Persistence.Strip;

public record HarbingerStripTurnLinkSmo(
    int TurnIndex,
    int LEDIndex,
    int X,
    int Y

    )
{
}

public static class HarbingerStripTurnLinkSmoExtensions
{
    public static HarStripLinkTurn ToDomain(this HarbingerStripTurnLinkSmo link) => new HarStripLinkTurn(
        LED: new HarLED(
            Coordinate: new XY(link.X, link.Y)
        ),
        LEDIndex: link.LEDIndex,
        TurnIndex: link.TurnIndex
    );

    public static HarbingerStripTurnLinkSmo ToSMO(this HarStripLinkTurn link) => new HarbingerStripTurnLinkSmo(
        TurnIndex: link.TurnIndex,
        LEDIndex: link.LEDIndex,
        X: link.LED.Coordinate.X,
        Y: link.LED.Coordinate.Y
    );

}
