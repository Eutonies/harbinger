using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Model;
using Harbinger.Domain.Util;

namespace Harbinger.Domain.Reach;

public record Reach<TEnt>(TEnt Placeable, IReadOnlyCollection<ReachRelation> Relations) where TEnt : IPlaceable
{

    public readonly XY Coordinate = Placeable.Coordinate;
    private readonly IReadOnlyDictionary<ulong, ReachRelation> _relatedDictionary = Relations
        .GroupBy(_ => _.Coordinate.AsLong)
        .ToDictionary(_ => _.Key, _ => _.First());

    private IReadOnlyCollection<ReachRelation>? _innerCirkel;
    public IReadOnlyCollection<ReachRelation> InnerCirkel => _innerCirkel ??= Relations
        .Where(_ => _.Closeness == 1)
        .ToReadOnly();

    private IReadOnlyCollection<ReachRelation>? _outerCirkel;
    public IReadOnlyCollection<ReachRelation> OuterCirkel => _outerCirkel ??= CalculateOuterCirkel(stepsTaken: [new ReachRelation(Coordinate, Closeness: 0)], currentLocation: Coordinate);
    

    private IReadOnlyCollection<ReachRelation> CalculateOuterCirkel(IReadOnlyCollection<ReachRelation> stepsTaken, XY currentLocation)
    {
        var allPossible = currentLocation.InAllDirections;
        if (stepsTaken.Count > 3 && allPossible.Contains(Coordinate))
            return stepsTaken;
        var stepsNotTaken = currentLocation.InAllDirections
            .Where(poss => _relatedDictionary.ContainsKey(poss.AsLong))
            .WhereNot(_ => stepsTaken.Any(st => st.Coordinate == _))
            .Select(_ => _relatedDictionary[_.AsLong])
            .ToReadOnly();
        if (stepsNotTaken.Empty())
            return stepsTaken;
        var bestCandidate = stepsNotTaken
            .OrderByDescending(_ => _.Closeness)
            .ThenByDescending(_ => _.Coordinate.DistanceTo(Coordinate))
            .FirstOption();
        foreach(var cand in bestCandidate)
        {
            return CalculateOuterCirkel(stepsTaken.Append(cand).ToReadOnly(), currentLocation: cand.Coordinate);
        }
        return stepsTaken;
    }



}
