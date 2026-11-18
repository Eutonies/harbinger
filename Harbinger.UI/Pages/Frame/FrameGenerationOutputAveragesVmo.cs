using Harbinger.Domain.Util;

namespace Harbinger.UI.Pages.Frame;

public record FrameGenerationOutputAveragesVmo(
    GenIn GenerationInput,
    HarStrip Strip
    )
{
    private LinkSplit? _split;
    private LinkSplit Split => _split ??= Strip.Links.TypePartition<HarStripLink, HarStripLinkCol, HarStripLinkTurn>()
        .Pipe(pa => new LinkSplit(pa.First, pa.Second));


    private LinkColumn[]? _columns;
    private LinkColumn[] Columns => _columns ??= Split.ColumnLEDs
        .GroupBy(_ => _.ColumnIndex)
        .Pipe(colGrps => Enumerable.Range(0, GenerationInput.NumberOfColumns)
            .Select(indx => new LinkColumn(
                ColumnIndex: indx,
                ColumnLEDs: colGrps.FirstOption(_ => _.Key == indx).Map(_ => _.ToReadOnly()).OrElse([]),
                PreviousTurn: Split.TurnLEDs.FirstOption(_ => _.TurnIndex == indx - 1),
                NextTurn: Split.TurnLEDs.FirstOption(_ => _.TurnIndex == indx),
                Avg: this)
            )
        ).ToArray();

    private LinkTurn[]? _turns;
    private LinkTurn[] Turns => _turns ??= Enumerable.Range(0, GenerationInput.NumberOfColumns)
        .Select(indx => new LinkTurn(TurnIndex: indx, Turn: Split.TurnLEDs.FirstOption(_ => _.TurnIndex == indx)))
        .ToArray();


    public Option<int> NumberOfLEDsExclTurn(int columnIndx) => LookupColumnVal(columnIndx, _ => _.NumberOfColumnLEDs);

    public Option<decimal> AverageDistanceToOtherColumnInMm(int columnIndx) => LookupColumnRef(columnIndx, _ => _.AverageColumnDistanceInMm)
        .Flatten();

    public Option<HarStripLinkTurn> TurnNo(int indexNo) => Option.Some(indexNo)
        .Where(indx => indx >= 0 && Turns.Length > indx)
        .FlatMap(indx => Turns[indx].Turn);

    public Option<int> LEDNoForIndex(int indexNo) => TurnNo(indexNo)
        .MapVal(_ => _.LEDIndex);

    private Option<TVal> LookupColumnVal<TVal>(int indx, Func<LinkColumn, TVal> extractor) where TVal : struct => Option.Some(indx)
        .Where(i => i >= 0 && i < Columns.Length)
        .Map(i => Columns[i])
        .MapVal(extractor);

    private Option<TRef> LookupColumnRef<TRef>(int indx, Func<LinkColumn, TRef> extractor) where TRef : class => Option.Some(indx)
        .Where(i => i >= 0 && i < Columns.Length)
        .Map(i => Columns[i])
        .Map(extractor);


    

    private record LinkSplit(IReadOnlyCollection<HarStripLinkCol> ColumnLEDs, IReadOnlyCollection<HarStripLinkTurn> TurnLEDs);

    private record LinkTurn(
        int TurnIndex,
        Option<HarStripLinkTurn> Turn
    )
    {
        public readonly Option<XY> Coordinate = Turn.Map(_ => _.LED.Coordinate);
        public readonly Option<int> LEDIndex = Turn.MapVal(_ => _.LEDIndex);
    }

    private record LinkColumn(
        int ColumnIndex, 
        IReadOnlyCollection<HarStripLinkCol> ColumnLEDs, 
        Option<HarStripLinkTurn> PreviousTurn,
        Option<HarStripLinkTurn> NextTurn,
        FrameGenerationOutputAveragesVmo Avg)
    {
        public readonly int NumberOfColumnLEDs = ColumnLEDs.Count;
        public readonly bool HasNextTurn = NextTurn.Any();
        public readonly bool HasPreviousTurn = PreviousTurn.Any();

        public readonly Option<int> AverageColumnX = Option.From(ColumnLEDs)
            .Where(_ => _.Any())
            .MapVal(coll => coll
                .Select(_ => _.LED.Coordinate.X)
                .AvgWithDecimals()
            );

        private Option<decimal>? _averageColumnDistanceMm;
        public Option<decimal> AverageColumnDistanceInMm => _averageColumnDistanceMm ??= new List<Option<decimal>>
        {
            DistanceToPreviousColumnMm,
            DistanceToNextColumnMm
        }.WhereNotNone()
         .Cipe(
            coll => coll.Any(),
            coll => Option.FromVal<decimal>(coll.Average())
            ) ?? Option.None<decimal>();

        private Option<LinkColumn> PreviousColumn => Avg.Columns.FirstOption(_ => _.ColumnIndex == ColumnIndex - 1);
        private Option<LinkColumn> NextColumn => Avg.Columns.FirstOption(_ => _.ColumnIndex == ColumnIndex + 1);

        private Option<decimal>? _distanceToPreviousColumnMm;
        private Option<decimal> DistanceToPreviousColumnMm => _distanceToPreviousColumnMm ??= DistanceToOtherColumnMm(PreviousColumn);

        private Option<decimal>? _distanceToNextColumnMm;
        private Option<decimal> DistanceToNextColumnMm => _distanceToNextColumnMm ??= DistanceToOtherColumnMm(NextColumn);

        private Option<decimal> DistanceToOtherColumnMm(Option<LinkColumn> othCol) => othCol
            .FlatMap(oth => AverageColumnX
                .FlatMap(x => oth.AverageColumnX
                    .MapVal(othX => Math.Abs(othX.ToDecimal() - x.ToDecimal()))
                )
            );


    }



}
