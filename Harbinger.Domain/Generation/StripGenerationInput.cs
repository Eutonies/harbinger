using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Model;
using Harbinger.Domain.Util;

namespace Harbinger.Domain.Generation;

public record StripGenerationInput(HarFram Frame, int NumberOfLeds, decimal DistanceBetweenLeds, decimal DistanceBetweenColumns, HarLoc StartCorner, int? NoOfColumnsInput, IReadOnlyCollection<GenTurn> InputTurns, IReadOnlyCollection<GenCol> InputCols)
{
    public readonly int NumberOfColumns = NoOfColumnsInput ??
        Convert.ToInt32(Math.Round((decimal)Frame.Width / ((decimal)DistanceBetweenColumns)));

    public readonly decimal DistanceTurnToSide = DistanceBetweenColumns / 2m;

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

