using System.Text.Json;

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

    private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions { WriteIndented = true };
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
    public static Option<string> SavedFrameFile => Option.Some(PersistenceConstants.FrameFile)
            .Where(File.Exists)
            .Map(File.ReadAllText);

    private static Option<int>? _savedFrameHash;
    public static Option<int> SavedFrameHash => _savedFrameHash ??= SavedFrameFile
        .MapVal(_ => _.ToStringId().GetHashCode());

    public static void SaveFrame(HarStrip strip)
    {
        var jsonOpt = strip.ToSMO()
            .ToOpt()
            .Map(smo => JsonSerializer.Serialize(smo));
        foreach(var json in jsonOpt)
        {
            var newHashCode = json.HashCode();
            foreach (var oldJsHash in SavedFrameHash)
                if (oldJsHash == newHashCode)
                    return;
            File.WriteAllText(PersistenceConstants.FrameFile, json);
            _savedFrameHash = null;
        }
    }

    private static int HashCode(this string str) => str.ToStringId().GetHashCode();
}


