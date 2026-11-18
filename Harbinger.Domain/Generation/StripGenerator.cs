using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Model;
using Harbinger.Domain.Util;

using Col = Harbinger.Domain.Generation.StripGenerator.GenerationCol;
using ColLED = Harbinger.Domain.Generation.StripGenerator.GenerationColWLEDCount;
using ColX = Harbinger.Domain.Generation.StripGenerator.GenerationColWXCoordinate;
using ColPop = Harbinger.Domain.Generation.StripGenerator.GenerationColPopulated;

using Turn = Harbinger.Domain.Generation.StripGenerator.GenerationTurn;

namespace Harbinger.Domain.Generation;

public static class StripGenerator
{

    private const decimal TurnXMoveRatio = 0.3m;
    private const decimal MarginToFrameRatio = 0.005m;

    public static HarStrip Generate(
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


    public static HarStrip Generate(
        HarFram frame,
        int numberOfLeds,
        int distanceBetweenLeds,
        int distanceBetweenColums,
        HarLoc startCorner,
        int? noOfColumnsInput = null,
        IEnumerable<GenTurn>? inputTurnsInput = null,
        IEnumerable<GenCol>? inputCols = null) =>
            Generate(new GenIn(
                Frame: frame,
                NumberOfLeds: numberOfLeds,
                DistanceBetweenLeds: distanceBetweenLeds,
                DistanceBetweenColumns: distanceBetweenColums,
                StartCorner: startCorner,
                NoOfColumnsInput: noOfColumnsInput,
                InputTurns: inputTurnsInput?.ToReadOnly() ?? [],
                InputCols: inputCols?.ToReadOnly() ?? []
                ));
    public static HarStrip Generate(this GenIn input)
    {
        var cols = input.GenerateColumns();
        var runningIndex = 0;
        var leds = new List<HarStripLink>();
        foreach(var (col, colIndx) in cols.ZipWithIndex())
        {
            for(var indx = 0; indx < col.ColumnLEDCoordinates.Length; indx++)
            {
                var xy = col.ColumnLEDCoordinates[indx];
                if (col.ColumnIndex != input.NumberOfColumns - 1 && indx == col.ColumnLEDCoordinates.Length - 1)
                    leds.Add(HarStrip.Turn(runningIndex, xy, turnIndex: colIndx));
                else leds.Add(HarStrip.Col(runningIndex, xy, columnIndex: colIndx));
                runningIndex += 1;
            }
        }
        var returnee = new HarStrip(Frame: input.Frame, Links: leds);
        return returnee;
    }


    private static IReadOnlyCollection<ColPop> GenerateColumns(this GenIn inp)
    {
        var cols = Enumerable.Range(0, inp.NumberOfColumns)
            .Select(indx => Col.Col(indx, inp))
            .ToArray();
        var colsWithLEDCount = CalculateNoOfLEDsPerColumn(cols, inp);
        var turns = inp.CalculateTurns();
        var colsWithX = colsWithLEDCount.CalculateColumnXCoordinates(turns, inp);
        var populatedColumns = colsWithX.CalculateColumnPopulations(turns, inp);
        return populatedColumns;
    }

    private static ColPop[] CalculateColumnPopulations(this ColX[] cols, Turn[] turns, GenIn inp) => cols
        .Select(col => col.CalculateColumnPopulation(priorTurn: turns.FirstOption(_ => _.TurnIndex == col.ColumnIndex - 1), followingTurn: turns.FirstOption(_ => _.TurnIndex == col.ColumnIndex), inp))
        .ToArray();

    private static ColPop CalculateColumnPopulation(this ColX col, Option<Turn> priorTurn, Option<Turn> followingTurn, GenIn inp) => (col.ColumnIndex, priorTurn, followingTurn) switch
    {
        (0, _, Some<Turn> sumTurn) => col.CalculateColumnPopulation(col.CalculateFirstLedPositionForFirstColumn(inp), followingTurn: sumTurn.Value, inp),
        (int indx, Some<Turn> sumPrior, _) when col.ColumnIndex == inp.NumberOfColumns - 1 => col.CalculateColumnPopulationLastColumn(col.CalculateFirstLedPositionForOtherColumns(sumPrior.Value, inp), inp),
        (_, Some<Turn> sumPrior, Some<Turn> sumNext) => col.CalculateColumnPopulation(startCoord: col.CalculateFirstLedPositionForOtherColumns(priorTurn: sumPrior.Value, inp), followingTurn: sumNext.Value, inp),
        _ => throw new Exception($"We be missin' sum' info in order to populate this 'ere columnus! Column index: {col.ColumnIndex}, {priorTurn.Map(turn => $"prior turn: {turn.TurnIndex}").OrElse("NO PRIOR TURN!")}, {followingTurn.Map(turn => $"next turn: {turn.TurnIndex}").OrElse("NO FOLLOWING TURN!")}")
    };

    private static ColPop CalculateColumnPopulation(this ColX col, XY startCoord, Turn followingTurn, GenIn inp)
    {
        var endCoord = col.CalculateLastLedPositionByNextTurn(followingTurn, inp);
        var heightToDistribute = endCoord.Y.ToDecimal() - startCoord.Y.ToDecimal();
        var segmentsForDistribution = col.LEDCount - 2; // subtract 1 as per default and 1 for turn LED
        var heightPerLed = heightToDistribute / segmentsForDistribution.ToDecimal();
        var leds = new XY[col.LEDCount];
        leds[0] = startCoord;
        var dir = col.VerticalOrientation;
        var shiftVector = new XY(X: 0, Y: heightPerLed.ToInt());
        var previousLed = startCoord;
        for(var indx = 1; indx < leds.Length; indx++)
        {
            if (indx == leds.Length - 1)
                leds[indx] = followingTurn.XY;
            else
                previousLed = leds[indx] = previousLed + shiftVector;
        }
        var returnee = col.WithLEDs(leds);
        return returnee;
    }

    private static ColPop CalculateColumnPopulationLastColumn(this ColX col, XY startCoord, GenIn inp)
    {
        var dir = col.VerticalOrientation;
        var shiftVector = new XY(X: 0, Y: inp.DistanceBetweenLeds.ToInt())*dir;
        var previousLed = startCoord;
        var leds = new XY[col.LEDCount];
        leds[0] = startCoord;
        for (var indx = 1; indx < leds.Length; indx++)
            previousLed = leds[indx] = previousLed + shiftVector;
        var returnee = col.WithLEDs(leds);
        return returnee;
    }


    private static XY CalculateFirstLedPositionForFirstColumn(this ColX col, GenIn inp)
    {
        var margin = inp.Frame.HeightDecimal * MarginToFrameRatio;
        var yCoord = inp.StartCorner.Vertical() switch
        {
            HarbingerLocation.Top => margin.ToInt(),
            _ => (inp.Frame.Height - margin).ToInt()
        };
        var returnee = new XY(X: col.XCoordinate, Y: yCoord);
        return returnee;
    }

    private static XY CalculateFirstLedPositionForOtherColumns(this ColX col, Turn priorTurn, GenIn inp)
    {
        var direction = col.VerticalOrientation.With(HarDir.Right);
        var returnee = priorTurn.XY.ShiftDiagonallyWithKnownWidth(lengthOfDiagonal: inp.DistanceBetweenLeds, lengthOfDiagonalWidth: inp.DistanceWidthTurnToColumn, direction: direction);
        return returnee;
    }
    private static XY CalculateLastLedPositionByNextTurn(this ColX col, Turn nextTurn, GenIn inp)
    {
        var direction = col.VerticalOrientation.Opposite().With(HarDir.Left);
        var returnee = nextTurn.XY.ShiftDiagonallyWithKnownWidth(lengthOfDiagonal: inp.DistanceBetweenLeds, lengthOfDiagonalWidth: inp.DistanceWidthTurnToColumn, direction: direction);
        return returnee;
    }




    private static ColLED[] CalculateNoOfLEDsPerColumn(this Col[] cols, GenIn inp)
    {
        var returnee = new ColLED[cols.Length];
        var noOfLedsInInput = 0;
        for (var indx = 0; indx < cols.Length; indx++)
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
        for (var indx = 0; indx < inp.NumberOfColumns; indx++)
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

    private static ColX[] CalculateColumnXCoordinates(this ColLED[] cols, Turn[] turns, GenIn inp)
    {
        var returnee = new ColX[cols.Length];
        var xMargin = inp.Frame.WidthDecimal * MarginToFrameRatio;
        for(var indx = 0; indx < returnee.Length; indx++)
        {
            var byDef = inp.ColNo(indx)
                .MapVal(_ => _.XCoordinate);
            var col = cols[indx];
            var xCoord = (byDef, indx) switch
            {
                (Some<int> sum, _) => sum.Value,
                (_, 0) => xMargin.ToInt(),
                (_, int i) when i == returnee.Length - 1 => (inp.Frame.WidthDecimal - xMargin).ToInt(),
                _ => turns[indx].XY.X.ToDecimal() - (inp.DistanceWidthTurnToColumn)
            };
            returnee[indx] = col.WithX(xCoord.ToInt());
        }
        return returnee;
    }

    private static Turn[] CalculateTurns(this GenIn inp)
    {
        var returnee = new Turn[inp.NumberOfColumns - 1];
        var forYCalculation = new List<Turn>();
        indx:  for (var indx = 0; indx < returnee.Length; indx++)
        {
            var fromInput = inp.TurnNo(indx);
            foreach (var inpTurn in fromInput)
            {
                foreach (var xy in inpTurn.Coordinate)
                {
                    var insertee = new Turn(TurnIndex: indx, XY: xy);
                    returnee[indx] = insertee;
                    forYCalculation.Add(insertee);
                    break indx;
                }
            }
        }
        if (forYCalculation.Count == returnee.Length)
            return returnee;
        var (startSideTurns, oppositeSideTurns) = forYCalculation.Partition(_ => _.TurnIndex % 2 == 0, distinguisherTrue: true, distinguisherFalse: false);
        var (startY, oppositeY) = (startSideTurns, oppositeSideTurns)
            .Map(_ => _.Coll.CalculateTurnSideY(inp, _.Dist));
        var (startX, endX) = inp.CalculateStartAndEndTurnXs();
        var indexesAlreadyCalculated = forYCalculation
            .Select(_ => _.TurnIndex)
            .ToHashSet();
        for(var indx = 0; indx < returnee.Length; indx++)
        {
            if (indexesAlreadyCalculated.Contains(indx))
                continue;
            var isStartSide = indx % 2 == 0;
            var yValue = isStartSide ? startY : oppositeY;
            var xValue = indx switch
            {
                0 => startX,
                _ => returnee[indx - 1].XY.X.ToDecimal().Pipe(x => x + inp.DistanceBetweenColumns).ToInt()  
            };
            returnee[indx] = new Turn(TurnIndex: indx, XY: new XY(xValue, yValue));
        }
        return returnee;
    }

    private static int CalculateTurnSideY(this IEnumerable<Turn> turns, GenIn inp, bool isStartSide)
    {
        if(turns.Any())
        {
            var yAverage = turns.AvgWithDecimals(_ => _.XY.Y);
            return yAverage;
        }
        var marginPoints = inp.Frame.HeightDecimal * MarginToFrameRatio;
        var isTop = (inp.StartCorner.Vertical(), isStartSide) switch
        {
            (HarLoc.Top, true) => false,
            (HarLoc.Top, _) => true,
            (_, true) /* Start location: bottom */ => true,
            _ => false
        };
        var returneeDec = isTop switch
        {
            true => marginPoints,
            _ => inp.Frame.Height - marginPoints
        };
        var returnee = returneeDec.ToInt();
        return returnee;
    }

    private static (int StartX, int EndX) CalculateStartAndEndTurnXs(this GenIn inp)
    {
        var (startIndx, endIndx) = (0, inp.NumberOfColumns - 1);
        var indxs = (startIndx, endIndx);
        var (startDef, endDef) = indxs
            .Map(inp.TurnNo)
            .Map(_ => _.FlatMap(_ => _.Coordinate.MapVal(_ => _.Y)));
        foreach (var st in startDef)
            foreach (var end in endDef)
                return (st, end);
        var (startCol, endCol) = indxs
            .Map(inp.ColNo);
        var (startByCol, endByCol) = (startCol, endCol)
            .Map(_ => _.MapVal(_ => _.XCoordinate.ToDecimal() + (inp.DistanceWidthTurnToColumn)).MapVal(_ => _.ToInt()));
        var margDist = inp.Frame.WidthDecimal * MarginToFrameRatio + (inp.DistanceWidthTurnToColumn);
        var (colsBetweenStartAndEnd, distBetweenStartAndEnd) = (endIndx - startIndx)
            .Pipe(noOfCols => (noOfCols, noOfCols.ToDecimal() * inp.DistanceBetweenColumns));
        var (calcStart, calcEnd) = (margDist.ToInt(), (margDist + distBetweenStartAndEnd).ToInt());
        var returnee = (startDef.Or(startByCol).OrElse(calcStart), endDef.Or(endByCol).OrElse(calcEnd));
        return returnee;
    }





    internal record GenerationCol(int ColumnIndex, HarLoc VerticalStartSide, HarLoc VerticalEndSide)
    {
        public readonly HarDir VerticalOrientation = VerticalEndSide.Vertical().LeadsInto();

        public static Col Col(int colIndx, GenIn genInp) => (colIndx % 2 == 0 ? genInp.StartCorner.Vertical() : genInp.StartCorner.Vertical().Opposite())
            .Pipe(strtSid => strtSid.Opposite()
                .Pipe(endSid => new Col(colIndx, VerticalStartSide: strtSid, VerticalEndSide: endSid))
            );
        public ColLED WithLEDCount(int count) => new ColLED(ColumnIndex, VerticalStartSide, VerticalEndSide, LEDCount: count);
    }

    internal record GenerationColWLEDCount(int ColumnIndex, HarLoc VerticalStartSide, HarLoc VerticalEndSide, int LEDCount) : Col(ColumnIndex, VerticalStartSide, VerticalEndSide)
    {
        public ColX WithX(int xCoordinate) => new ColX(ColumnIndex, VerticalStartSide: VerticalStartSide, VerticalEndSide: VerticalEndSide, LEDCount: LEDCount, XCoordinate: xCoordinate);

    }

    internal record GenerationColWXCoordinate(int ColumnIndex, HarLoc VerticalStartSide, HarLoc VerticalEndSide, int LEDCount, int XCoordinate) : ColLED(ColumnIndex, VerticalStartSide, VerticalEndSide, LEDCount)
    {
        public ColPop WithLEDs(XY[] leds) => new ColPop(ColumnIndex: ColumnIndex, VerticalStartSide: VerticalStartSide, VerticalEndSide: VerticalEndSide, LEDCount: LEDCount, XCoordinate: XCoordinate, ColumnLEDCoordinates: leds);
    }

    internal record GenerationColPopulated(int ColumnIndex, HarLoc VerticalStartSide, HarLoc VerticalEndSide, int LEDCount, int XCoordinate, XY[] ColumnLEDCoordinates) : ColX(ColumnIndex, VerticalStartSide, VerticalEndSide, LEDCount, XCoordinate)
    {
    }


    internal record GenerationTurn(int TurnIndex, XY XY);


}


