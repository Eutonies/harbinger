using Harbinger.Domain.Model;
using Harbinger.Domain.Util;

namespace Harbinger.UI.Pages.Frame;

public record FrameVmo(int Width, int Height, HarFram FromFrame)
{
    private readonly HarFram Frame = new HarFram(width: Width, height: Height);

    private HarMap? _mapping;
    public HarMap Mapping => _mapping ??= Frame / FromFrame;

    public IReadOnlyCollection<HarLED> Map(IEnumerable<HarLED> leds) => leds
        .Select(MapLED)
        .ToReadOnly();
    public IReadOnlyCollection<HarStripLinkCol> Map(IEnumerable<HarStripLinkCol> cols) => cols
        .Select(MapCol)
        .ToReadOnly();
    public IReadOnlyCollection<HarStripLinkTurn> Map(IEnumerable<HarStripLinkTurn> turns) => turns
        .Select(MapTurn)
        .ToReadOnly();

    public HarLED MapLED(HarLED led) => Map(led, (led, xy) => new HarLED(xy));
    public HarStripLink MapLink(HarStripLink lnk) => lnk switch
    {
        HarStripLinkCol col => MapCol(col),
        HarStripLinkTurn turn => MapTurn(turn)
    };

    public HarStripLinkCol MapCol(HarStripLinkCol col) => Map(col, (co, xy) => new HarStripLinkCol(new HarLED(xy), LEDIndex: co.LEDIndex, ColumnIndex: co.ColumnIndex));
    public HarStripLinkTurn MapTurn(HarStripLinkTurn turn) => Map(turn, (tu, xy) => new HarStripLinkTurn(new HarLED(xy), LEDIndex: tu.LEDIndex, TurnIndex: tu.TurnIndex));

    public IReadOnlyCollection<TEnt> Map<TEnt>(IEnumerable<TEnt> entities, Func<TEnt, XY, TEnt> entCreator) where TEnt : IPlaceable => entities
        .Select(ent => Map(ent, entCreator))
        .ToReadOnly();

    public TEnt Map<TEnt>(TEnt entity, Func<TEnt, XY, TEnt> entCreator) where TEnt : IPlaceable => Mapping[entity.Coordinate]
        .Pipe(xy => entCreator(entity, xy));


    public static FrameStripVmo operator *(FrameVmo frame, HarStrip strip) => new FrameStripVmo(frame, strip);

}

public static class FrameVmoExtensions
{
    public const int MaxSideLength = 20_000;
    public static FrameVmo ToVmo(this HarFram frame, int lengthOfLongestSide = MaxSideLength)
    {
        var (width, height) = (frame.WidthDecimal, frame.HeightDecimal);
        var (outWidth, outHeight) = (lengthOfLongestSide.ToDecimal(), lengthOfLongestSide.ToDecimal());
        if(width > height)
        {
            var ratio = lengthOfLongestSide.ToDecimal() / width;
            outHeight = ratio * height;
        }
        else
        {
            var ratio = lengthOfLongestSide.ToDecimal() / height;
            outWidth = ratio * width;
        }
        var returnee = new FrameVmo(outWidth.ToInt(), outHeight.ToInt(), FromFrame: frame);
        return returnee;
    }
}
