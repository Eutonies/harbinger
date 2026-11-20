using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Model;

public record HarbingerStrip(IReadOnlyCollection<HarbingerStripLink> Links)
{

    public static HarbingerStripLink Generate(
        int width, 
        int height, 
        int numberOfLeds, 
        int distanceBetweenLeds, 
        int distanceBetweenColums, 
        HarLoc startCorner, 
        int? noOfColumnsInput = null, 
        IEnumerable<GenTurn>? inputTurns = null,
        IEnumerable<GenCol>? inputCols = null
        ) =>
        Generate(frame: new HarFram(MinX: 0, MinY: 0, MaxX: width, MaxY: height), numberOfLeds, distanceBetweenLeds, distanceBetweenColums, startCorner, noOfColumnsInput, inputTurns);


    public static HarbingerStripLink Generate(
        HarFram frame,
        int numberOfLeds,
        int distanceBetweenLeds,
        int distanceBetweenColums,
        HarLoc startCorner,
        int? noOfColumnsInput = null,
        IEnumerable<GenTurn>? inputTurnsInput = null,
        IEnumerable<GenCol>? inputCols = null) =>
            Generate(new GenerationInput(
                Frame: frame,
                NumberOfLeds: numberOfLeds,
                DistanceBetweenLeds: distanceBetweenLeds,
                DistanceBetweenColums: distanceBetweenColums,
                StartCorner: startCorner,
                NoOfColumnsInput: noOfColumnsInput,
                InputTurns: inputTurnsInput?.ToReadOnly() ?? [],
                InputCols: inputCols?.ToReadOnly() ?? []
                ));
    public static HarbingerStripLink Generate(
    GenerationInput input
    {
        var noOfCols = noOfColumnsInput ?? frame.Width / distanceBetweenColums;
    }


    private static IReadOnlyCollection<GenerationCol> GenerateColumns(GenerationInput inp)
    {
        var cols = Enumerable.Range(0, inp.NumberOfColumns)
            .Select(indx => GenerationCol.Col(indx, inp))
            .ToArray();
        var colsWithLEDCount = CalculateNoOfLEDsPerColumn(cols, inp);

    }

    private static GenerationColWLEDCount[] CalculateNoOfLEDsPerColumn(GenerationCol[] cols, GenerationInput inp)
    {
        var returnee = new GenerationColWLEDCount[cols.Length];
        var noOfLedsInInput = 0;
        for(var indx = 0; indx < cols.Length; indx++)
        {
            var col = cols[indx];
            foreach (var colInp in inp.ColNo(indx))
            {
                returnee[indx] = col.WithLEDCount(colInp.NumberOfLEDs);
                noOfLedsInInput += colInp.NumberOfLEDs;
            }
        }
        var ledsForDistribution = inp.NumberOfLeds - noOfLedsInInput;
        if (inp.InputCols.Count == inp.NumberOfColumns)
            return returnee;
        if (ledsForDistribution <= 0)
            throw new ArgumentException($"Calculations show {ledsForDistribution} LEDs left for distribution after reserving LEDs' for {inp.InputCols.Count} input columns");
        var indexesSpecedInInput = inp.InputCols
            .Select(_ => _.ColumnIndex)
            .ToHashSet();
        var lastIndexForDist = Enumerable.Range(0, inp.NumberOfColumns)
            .WhereNot(indexesSpecedInInput.Contains)
            .Max();
        var avgNoOfLEDsPerCol = ledsForDistribution / (inp.NumberOfColumns - indexesSpecedInInput.Count);
        for(var indx = 0; indx < inp.NumberOfColumns; indx++)
        {
            if (indexesSpecedInInput.Contains(indx))
                continue;
            var col = cols[indx];
            if (indx == lastIndexForDist)
                returnee[indx] = col.WithLEDCount(ledsForDistribution);
            else
            {
                returnee[indx] = col.WithLEDCount(avgNoOfLEDsPerCol);
                ledsForDistribution -= avgNoOfLEDsPerCol;
            }
        }
        return returnee;
    }

    private record GenerationCol(int ColumnIndex, HarLoc VerticalStartSide, HarLoc VerticalEndSide)
    {
        public readonly HarDir Orientation = VerticalEndSide.LeadsInto();

        public static GenerationCol Col(int colIndx, GenerationInput genInp) => (colIndx % 2 == 0 ? genInp.StartCorner.Vertical() : genInp.StartCorner.Vertical().Opposite())
            .Pipe(strtSid => strtSid.Opposite()
                .Pipe(endSid => new GenerationCol(colIndx, VerticalStartSide: strtSid, VerticalEndSide: endSid))
            );
        public GenerationColWLEDCount WithLEDCount(int count) => new GenerationColWLEDCount(ColumnIndex, VerticalStartSide, VerticalEndSide, LEDCount: count);
    }

    private record GenerationColWLEDCount(int ColumnIndex, HarLoc VerticalStartSide, HarLoc VerticalEndSide, int LEDCount) : GenerationCol(ColumnIndex, VerticalStartSide, VerticalEndSide)
    {
    }

    private record GenerationInput(HarFram Frame, int NumberOfLeds, int DistanceBetweenLeds, int DistanceBetweenColums, HarLoc StartCorner, int? NoOfColumnsInput, IReadOnlyCollection<GenTurn> InputTurns, IReadOnlyCollection<GenCol> InputCols)
    {
        public readonly int NumberOfColumns = NoOfColumnsInput ??
            Convert.ToInt32(Math.Round((decimal)Frame.Width / ((decimal)DistanceBetweenColums)));
        public readonly (IReadOnlyCollection<GenTurn> StartSideTurns, IReadOnlyCollection<GenTurn> OppositeSideTurns) Turns = 
            InputTurns.Partition(_ => _.TurnIndex % 2 == 0);

        private IReadOnlyCollection<GenTurn>? _startSideTurns;
        public IReadOnlyCollection<GenTurn> StartSideTurns => _startSideTurns ??= Turns.StartSideTurns;
        private IReadOnlyCollection<GenTurn>? _oppositeSideTurns;
        public IReadOnlyCollection<GenTurn> OppositeSideTurns => _oppositeSideTurns ??= Turns.OppositeSideTurns;

        public Option<GenTurn> TurnNo(int turnIndx) => InputTurns
            .FirstOption(_ => _.TurnIndex == turnIndx);

        public Option<GenCol> ColNo(int colIndx) => InputCols
            .FirstOption(_ => _.ColumnIndex == colIndx);

        public HarDir HorizontalToEnd = StartCorner switch 
        {
            HarbingerLocation.BottomLeftCorner | HarbingerLocation.TopLeftCorner => HarDir.Right,
            HarbingerLocation.BottomRightCorner | HarbingerLocation.TopRightCorner => HarDir.Left,
            _ => throw new ArgumentException($"Start corner: {StartCorner} is not a corner yo!")
        };

        public HarDir VerticalToStartSide = StartCorner switch
        {
            HarbingerLocation.BottomLeftCorner | HarbingerLocation.BottomRightCorner => HarDir.Down,
            HarbingerLocation.TopLeftCorner | HarbingerLocation.TopRightCorner => HarDir.Up,
            _ => throw new ArgumentException($"Start corner: {StartCorner} is not a corner yo!")
        };

        private HarDir? _verticialToOpposite;
        public HarDir VerticalToOpposite => _verticialToOpposite ??= VerticalToStartSide.Opposite();
    }

}

public record HarbingerStripGenerationInputTurn(int TurnIndex, int LEDIndex, bool IsOnStartSide, XY? InputCoordinate = null)
{
    protected virtual XY? Coordinate => InputCoordinate;
}

public record HarbingerStripGenerationInputColumn(int ColumnIndex, int NumberOfLEDs)
{
}



public abstract record HarbingerStripLink(HarbingerLED LED, int Index);

public record HarbingerStripVerticalLink(HarbingerLED LED, int Index) : HarbingerStripLink(LED, Index);

public record HarbingerStripTurnLink(HarbingerLED LED, int Index) : HarbingerStripLink(LED, Index);

