using System.Text;

using Harbinger.Domain.Util;
using Harbinger.UI.Util;

using Microsoft.AspNetCore.Components;

namespace Harbinger.UI.Pages.Frame;

public partial class FrameComponent
{
    private const int NoOfMarginSpaces = 17;
    private static readonly string MarginSpaces = NoOfMarginSpaces.ToSpaces();

    [Parameter]
    public FrameStripVmo Strip { get; set; }

    private int Width => Strip.Frame.Width;
    private int Height => Strip.Frame.Height;

    private IReadOnlyCollection<HarStripLink> Points => Strip;

    private string DrawStrip()
    {
        var sb = new StringBuilder();
        foreach(var (lnk, indx) in Points.ZipWithIndex())
        {
            if (indx == 0)
                sb.Append(lnk.ToMoveCommand());
            else
                sb.Append($"\n{MarginSpaces}{lnk.ToLineCommand()}");
        }
        //sb.Append(" Z");
        return sb.ToString();

    }


    private IReadOnlyCollection<CoordinateText> CollectTextPoints()
    {
        var returnee = new List<CoordinateText>();
        var cols = Strip
            .OfType<HarStripLinkCol>()
            .GroupBy(_ => _.ColumnIndex)
            .Select(grp => (ColIndx: grp.Key, AvgIndx: grp.Count() / 2, Col: grp.ToArray()))
            .Select(pa => pa.Col[pa.AvgIndx])
            .ToArray();
        returnee.AddRange(cols.Select(_ => new CoordinateText(_)));
        var turnPoints = Strip
            .OfType<HarStripLinkTurn>()
            .ToArray();
        returnee.AddRange(turnPoints.Select(_ => new CoordinateText(_)));
        return returnee;
    }


    private IReadOnlyCollection<CoordinateHighlight> CollectHightlights()
    {
        var returnee = new List<CoordinateHighlight>();
        returnee.AddRange(Points.OfType<HarStripLinkTurn>().Select(_ => new CoordinateHighlight(_)));
        var cols = Points
            .OfType<HarStripLinkCol>()
            .GroupBy(_ => _.ColumnIndex)
            .Select(_ => _.OrderBy(_ => _.LEDIndex))
            .SelectMany(grp => grp.Take(10).Concat(grp.Reverse().Take(10)))
            .ToArray();
        returnee.AddRange(cols.Select(_ => new CoordinateHighlight(_)));
        return returnee;
    }

    private record CoordinateHighlight(HarStripLink Link)
    {
        public readonly XY XY = Link.LED.Coordinate;

        private readonly bool IsTurn = Link is HarStripLinkTurn;

        private int? _x;
        public int X => _x ??= XY.X;

        private int? _y;
        public int Y => _y ??= XY.Y;

        private int? _r;
        public int R => _r ??= TurnCol(t => 203, c => 70);

        private string? _class;
        public string Class => _class ??= "frame-drawing-dot"
            .Pipe(cl => $"{cl} {cl}-{TurnCol(t => "turn", c => "col")}");


        private T TurnCol<T>(Func<HarStripLinkTurn, T> ifTurn, Func<HarStripLinkCol, T> ifCol) => Link switch
        {
            HarStripLinkTurn t => ifTurn(t),
            HarStripLinkCol c => ifCol(c)
        };



    }

    private record CoordinateText(HarStripLink Link)
    {
        public readonly XY XY = Link.LED.Coordinate;

        public readonly int X = Link.LED.Coordinate.X;
        public readonly int Y = Link.LED.Coordinate.Y + ExtraYDistance(Link);

        private static int ExtraYDistance(HarStripLink lnk) => lnk switch
        {
            HarStripLinkTurn t => ((t.TurnIndex % 6) + 1) * 500 * (t.TurnIndex % 2 == 0 ? 1 : -1),
            HarStripLinkCol c => ((c.ColumnIndex % 6) + 1) * 500 * (c.ColumnIndex % 2 == 0 ? 1 : -1),
            
        };
        
        public readonly int LEDIndex = Link.LEDIndex;
        public readonly int TypeIndex = Link switch { 
            HarStripLinkCol col => col.ColumnIndex,
            HarStripLinkTurn trn => trn.TurnIndex
        };

        public readonly bool IsTurn = Link is HarStripLinkTurn;

        private string? _class;
        public string Class => _class ??= Option.From("turn")
            .Where(_ => IsTurn)
            .OrElse("col")
            .Pipe(yp => $"frame-drawing-text frame-drawing-text-{yp}");


        private string? _text;
        public string Text => _text ??= (Link switch
        {
            HarStripLinkCol col => ColText(col),
            HarStripLinkTurn trn => TurnText(trn)
        });


        private string ColText(HarStripLinkCol col) => $"X:{col.Coordinate.X} [{col.ColumnIndex}] ({col.LEDIndex})";
        private string TurnText(HarStripLinkTurn turn) => $"({turn.Coordinate.X},{turn.Coordinate.Y}) [{turn.TurnIndex}] ({turn.LEDIndex})";
    }

}
