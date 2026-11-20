using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Harbinger.Domain.Util;

public struct StringId
{
    public static readonly IStringValuer StringValuer = new StringIdValuer();
    private readonly ULongId _serialized;

    public StringId(IEnumerable<string?> inps)
    {
        var withIndx = WithIndex(inps);
        var nulls = withIndx
            .Where(_ => _.Str == null)
            .Select(_ => _.Indx)
            .ToArray();
        var empties = withIndx
            .Where(_ => _.Str == "")
            .Select(_ => _.Indx)
            .ToArray();
        var id = new ulong[inps.Count()][];
        foreach (var (inp, indx) in withIndx)
        {
            if (nulls.Contains(indx) || empties.Contains(indx))
            {
                id[indx] = [];
                continue;
            }
            var strBytes = Encoding.UTF8.GetBytes(inp!);

            var inpLeng = strBytes.Length;
            var longArr = EncodeAsArray(inp!);
            id[indx] = longArr;
        }
        _serialized = new ULongId(id: id, nulls: nulls, empties: empties);
    }


    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj == null)
            return false;
        if (!(obj is StringId))
            return false;
        var oth = (StringId)obj;
        return oth._serialized.Equals(_serialized);
    }

    public override int GetHashCode() => _serialized.GetHashCode();



    public static bool operator ==(StringId fst, StringId snd) => fst.Equals(snd);
    public static bool operator !=(StringId fst, StringId snd) => !fst.Equals(snd);

    public string AsText() => _serialized.AsText();

    private static IReadOnlyCollection<(string? Str, int Indx)> WithIndex(IEnumerable<string?> inps) =>
        inps.Select((str, indx) => (str, indx)).ToArray();

    private record StringIdValuer : IStringValuer
    {
        public string Aggregate(IEnumerable<string?> input) => new StringId(input).AsText();
        public bool StringCollectionsAreEqual(IEnumerable<string?> fst, IEnumerable<string?> snd) =>
            Aggregate(fst) == Aggregate(snd);
    }

    public static ulong[] EncodeAsArray(string inp)
    {
        var strBytes = Encoding.UTF8.GetBytes(inp!);

        var inpLeng = strBytes.Length;
        var longArr = new ulong[inpLeng / 8 + 1];
        ulong curLong = 0;
        int longArrIndx = 0;
        var strIndx = 0;
        var longOffset = 0;
        while (strIndx < inpLeng)
        {
            curLong ^= ((ulong)strBytes[strIndx]) << (longOffset * 8);
            longOffset = (longOffset + 1) % 8;
            if (longOffset == 0)
            {
                longArr[longArrIndx] = curLong;
                longArrIndx += 1;
                curLong = 0;
            }
            strIndx += 1;
        }
        if (longOffset > 0)
            longArr[longArr.Length - 1] = curLong;
        return longArr;
    }

}

public static class StringIdExtensions
{
    public static StringId ToStringId(this IEnumerable<string?> coll, bool caseInsensitive) => (coll
        .Cipe(_ => caseInsensitive, _ => _.Select(_ => _.Trim().ToLower())) ?? coll)
            .Pipe(_ => new StringId(_));

    public static StringId ToStringId(this IEnumerable<string?> coll) => coll.ToStringId(caseInsensitive: false);


    public static StringId ToStringId(this string? str, bool caseInsensitive) => new List<string?> { str }.ToStringId(caseInsensitive);

    public static StringId ToStringId(this string? str) => str.ToStringId(caseInsensitive: false);


}