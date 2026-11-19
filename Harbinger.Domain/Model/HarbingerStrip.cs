using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Model;

public record HarbingerStrip(IReadOnlyCollection<HarbingerStripLink> Links)
{

    public static HarbingerStripLink Generate(int width, int height, int numberOfLeds, int distanceBetweenLeds, int distanceBetweenColums, int? noOfColumnsInput = null, params IEnumerable<GenTurn> inputTurns) =>
        Generate(frame: new HarFram(MinX: 0, MinY: 0, MaxX: width, MaxY: height), numberOfLeds, distanceBetweenLeds, distanceBetweenColums, noOfColumnsInput, inputTurns);


    public static HarbingerStripLink Generate(HarFram frame, int numberOfLeds, int distanceBetweenLeds, int distanceBetweenColums, int? noOfColumnsInput = null, params IEnumerable<GenTurn> inputTurns)
    {
        var noOfCols = noOfColumnsInput ?? frame.Width / distanceBetweenColums;
    }

    private record GenerationCol(int StartTurnIndex, int EndTurnIndex)


}

public record HarbingerStripGenerationInputTurn(int TurnIndex, int LEDIndex, XY? InputCoordinate = null)
{
    protected virtual XY? Coordinate => InputCoordinate;
}


public abstract record HarbingerStripLink(HarbingerLED LED, int Index);

public record HarbingerStripVerticalLink(HarbingerLED LED, int Index) : HarbingerStripLink(LED, Index);

public record HarbingerStripTurnLink(HarbingerLED LED, int Index) : HarbingerStripLink(LED, Index);

