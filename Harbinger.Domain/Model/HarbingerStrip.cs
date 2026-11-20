using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Model;

public record HarbingerStrip(IReadOnlyCollection<HarbingerStripLink> Links)
{

    public static HarbingerStripLink Generate(int width, int height, int numberOfLeds, int distanceBetweenLeds, int distanceBetweenColums, int? noOfColumnsInput = null, params IEnumerable<GenTurn> inputTurns) =>
        Generate(frame: new HarFram(MinX: 0, MinY: 0, MaxX: width, MaxY: height), numberOfLeds, distanceBetweenLeds, distanceBetweenColums, noOfColumnsInput, inputTurns);


    public static HarbingerStripLink Generate(HarFram frame, int numberOfLeds, int distanceBetweenLeds, int distanceBetweenColums, HarLoc startCorner, int? noOfColumnsInput = null, params IEnumerable<GenTurn> inputTurns)
    {
        var noOfCols = noOfColumnsInput ?? frame.Width / distanceBetweenColums;
    }



    private static IReadOnlyCollection<GenerationCol> GenerateColumns(GenerationInput inp)
    {

    }

    private record GenerationCol(int StartTurnIndex, int EndTurnIndex, XY EndTurnCoordinate);
    private record GenerationInput(HarFram Frame, int NumberOfLeds, int DistanceBetweenLeds, int DistanceBetweenColums, HarLoc StartCorner, int? NoOfColumnsInput, IReadOnlyCollection<GenTurn> InputTurns)
    {
        public readonly int NumberOfColumns = NoOfColumnsInput ??
            Convert.ToInt32(Math.Round((decimal)Frame.Width / ((decimal)DistanceBetweenColums)));
        public readonly (IReadOnlyCollection<GenTurn> StartSideTurns, IReadOnlyCollection<GenTurn> OppositeSideTurns) Turns = 
            InputTurns.Partition(_ => _.TurnIndex % 2 == 0);

        private IReadOnlyCollection<GenTurn>? _startSideTurns;
        public IReadOnlyCollection<GenTurn> StartSideTurns => _startSideTurns ??= Turns.StartSideTurns;
        private IReadOnlyCollection<GenTurn>? _oppositeSideTurns;
        public IReadOnlyCollection<GenTurn> OppositeSideTurns => _oppositeSideTurns ??= Turns.OppositeSideTurns;

        public Option<GenTurn> TurnNo(int turnIndx) => turnIndx % 2 == 0 ? 
            StartSideTurns.FirstOpt

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


public abstract record HarbingerStripLink(HarbingerLED LED, int Index);

public record HarbingerStripVerticalLink(HarbingerLED LED, int Index) : HarbingerStripLink(LED, Index);

public record HarbingerStripTurnLink(HarbingerLED LED, int Index) : HarbingerStripLink(LED, Index);

