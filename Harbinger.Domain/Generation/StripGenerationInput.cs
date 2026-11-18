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

    public readonly decimal DistanceWidthTurnToColumn = DistanceBetweenColumns / 2m;

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
        HarbingerLocation.BottomLeftCorner or HarbingerLocation.TopLeftCorner => HarDir.Right,
        HarbingerLocation.BottomRightCorner or HarbingerLocation.TopRightCorner => HarDir.Left,
        _ => throw new ArgumentException($"Start corner: {StartCorner} is not a corner yo!")
    };

    public HarDir VerticalToStartSide = StartCorner switch
    {
        HarbingerLocation.BottomLeftCorner or HarbingerLocation.BottomRightCorner => HarDir.Down,
        HarbingerLocation.TopLeftCorner or HarbingerLocation.TopRightCorner => HarDir.Up,
        _ => throw new ArgumentException($"Start corner: {StartCorner} is not a corner yo!")
    };

    private HarDir? _verticialToOpposite;
    public HarDir VerticalToOpposite => _verticialToOpposite ??= VerticalToStartSide.Opposite();

    public bool IsSameAs(GenIn oth)
    {
        if (!Frame.IsSameAs(oth.Frame))
            return false;
        if (NumberOfLeds != oth.NumberOfLeds)
            return false;
        if (Math.Abs(DistanceBetweenLeds - oth.DistanceBetweenLeds) > 0.0001m)
            return false;
        if (Math.Abs(DistanceBetweenColumns - oth.DistanceBetweenColumns) > 0.0001m)
            return false;
        if (StartCorner != oth.StartCorner)
            return false;
        if (!AreSame(NoOfColumnsInput, oth.NoOfColumnsInput))
            return false;
        if (InputTurns.Any(myTurn => !oth.InputTurns.Any(othTurn => othTurn.IsSameAs(myTurn))))
            return false;
        if (oth.InputTurns.Any(othTurn => !InputTurns.Any(myTurn => othTurn.IsSameAs(myTurn))))
            return false;
        if (InputCols.Any(myCol => !oth.InputCols.Any(othCol => othCol.IsSameAs(myCol))))
            return false;
        if (oth.InputCols.Any(othCol => !InputCols.Any(myCol => othCol.IsSameAs(myCol))))
            return false;
        return true;
    }


    private bool AreSame(int? fst, int? oth) => (fst, oth) switch
    {
        (null, null) => true,
        (null, _) => false,
        (_, null) => false,
        (int f, int s) => f == s
    };

}

