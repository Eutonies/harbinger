using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Model;
using Harbinger.Domain.Util;

namespace Harbinger.Domain.Reach;

public static class ReachCalculator
{

    public static ReachResult<TEnt> CalculateReach<TEnt>(this IEnumerable<TEnt> entities, HarFram frame) where TEnt : IPlaceable
    {
        var state = new CalculationState<TEnt>(frame, entities.ToReadOnly());
        while(state.CanRunAnotherRound)
        {
            var noOfCoordinatesAdded = state.RunRound();
            Console.WriteLine($"During round {state.CurrentRoundNo}, {noOfCoordinatesAdded} coordinates were added to result");
        }
        var returnReaches = state.Entities
            .Select(_ => _.ToReach())
            .ToArray();
        var returnee = new ReachResult<TEnt>(returnReaches, frame);
        return returnee;
    }




    private record CalculationState<TEnt>(HarFram Frame, IReadOnlyCollection<TEnt> InputEntities) where TEnt : IPlaceable
    {
        private int _roundNo = 0;
        public int CurrentRoundNo => _roundNo;
        private bool _canRunAnotherRound = true;
        public bool CanRunAnotherRound => _canRunAnotherRound;

        private readonly Dictionary<ulong, CalculationEntity<TEnt>> _pointsTaken = new Dictionary<ulong, CalculationEntity<TEnt>>();

        public readonly CalculationEntity<TEnt>[] Entities = InputEntities
            .Select(ent => new CalculationEntity<TEnt>(ent, ent.Coordinate))
            .ToArray();

        private bool IsWithinBounds(XY coord) => Frame.IsWithinFrame(coord);
        private bool IsUpForGrabs(XY coord) => !_pointsTaken.ContainsKey(coord.AsLong) && IsWithinBounds(coord);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>No of coordinates added this round</returns>
        public int RunRound()
        {
            var noOfCoordinatesAddedThisRound = 0;
            var evenRound = _roundNo % 2 == 0;
            var sortedEntities = (Entities.Cipe(_ => evenRound, coll => coll.OrderBy(_ => _.Coordinate.AsLong)) ?? Entities.OrderByDescending(_ => _.Coordinate.AsLong))
                .ToReadOnly();
            foreach(var ent in sortedEntities)
            {
                var possibles = ent.PossibleNextSteps(Frame);
                var filteredPossibles = possibles
                    .Where(IsUpForGrabs)
                    .DistinctBy(_ => _.AsLong)
                    .ToArray();
                foreach (var poss in filteredPossibles)
                {
                    ent.Add(poss);
                    _pointsTaken[poss.AsLong] = ent;
                    noOfCoordinatesAddedThisRound += 1;
                }
            }
            if (noOfCoordinatesAddedThisRound == 0)
                return UpdateCanRunAndReturn(noOfCoordinatesAddedThisRound);
            var addedThisRoundByEntitiesCount = 0;
            foreach(var ent in sortedEntities)
                addedThisRoundByEntitiesCount += ent.NextRound();
            _roundNo += 1;
            return UpdateCanRunAndReturn(addedThisRoundByEntitiesCount);
        }
        private int UpdateCanRunAndReturn(int addedThisRound)
        {
            _canRunAnotherRound = addedThisRound > 0;
            return addedThisRound;
        }

    }


    private record CalculationEntity<TEnt>(TEnt Entity, XY Coordinate) where TEnt : IPlaceable
    {

        public readonly ulong Id = Coordinate.AsLong;
        private int _roundNo = 0;
        private readonly List<ReachRelation> _relations = new List<ReachRelation>();

        private List<XY> _addedThisRound = new List<XY>();
        private IReadOnlyCollection<XY> _addedLastRound = [Coordinate];

        public IReadOnlyCollection<XY> PossibleNextSteps(HarFram fram)
        {
            var suggestionDict = new Dictionary<ulong, XY>();
            foreach (var lassi in _addedLastRound)
                foreach (var nxt in lassi.InAllDirections.WhereNot(sugg => suggestionDict.ContainsKey(sugg.AsLong)))
                    if (fram.IsWithinFrame(nxt))
                        suggestionDict[nxt.AsLong] = nxt;
            return suggestionDict.Values;
        }

        public Reach<TEnt> ToReach() => new Reach<TEnt>(Entity, _relations.ToArray());

        public void Add(XY xy)
        {
            _relations.Add(new ReachRelation(xy, Closeness: _roundNo));
            _addedThisRound.Add(xy);
        }
        /// <summary>
        /// Returns no of relations added in the round just finished
        /// </summary>
        /// <returns></returns>
        public int NextRound()
        {
            var returnee = _addedThisRound.Count;
            _addedLastRound = _addedThisRound;
            _addedThisRound = new List<XY>();
            _roundNo += 1;
            return returnee;
        }

    }


}
