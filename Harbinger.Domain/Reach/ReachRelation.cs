using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Reach;

public record ReachRelation(XY Coordinate, int Closeness) : 
    IComparable<ReachRelation>, 
    IComparisonOperators<ReachRelation, ReachRelation, bool>, 
    IComparisonOperators
{

    public int CompareTo(ReachRelation? other) => (other!)
        .Pipe(oth => Closeness.CompareTo(oth.Closeness));

    public static bool operator <(ReachRelation left, ReachRelation right) => left.Closeness < right.Closeness;

    public static bool operator <=(ReachRelation left, ReachRelation right) => left.Closeness <= right.Closeness;

    public static bool operator >(ReachRelation left, ReachRelation right) => left.Closeness > right.Closeness;

    public static bool operator >=(ReachRelation left, ReachRelation right) => left.Closeness >= right.Closeness;

}
