using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Harbinger.Domain.Util;

internal struct ULongId
{
    private readonly ulong[][] _id;
    private readonly int[] _nulls;
    private readonly int[] _emptys;

    internal ULongId(ulong[][] id, int[] nulls, int[] empties)
    {
        _id = id;
        _nulls = nulls;
        _emptys = empties;
    }


    internal bool AreSame(ULongId oth)
    {
        if (!AreSame(_nulls, oth._nulls) || !AreSame(_emptys, oth._emptys))
            return false;
        if (_id.Length != oth._id.Length)
            return false;
        for (var indx = 0; indx < _id.Length; indx++)
        {
            if (!AreSame(_id[indx], oth._id[indx]))
                return false;
        }
        return true;
    }



    private static bool AreSame(int[] fst, int[] snd)
    {
        if (fst.Length != snd.Length)
            return false;
        for (var indx = 0; indx < fst.Length; indx++)
            if (fst[indx] != snd[indx])
                return false;
        return true;
    }

    private static bool AreSame(ulong[] fst, ulong[] snd)
    {
        if (fst.Length != snd.Length)
            return false;
        for (var indx = 0; indx < fst.Length; indx++)
            if (fst[indx] != snd[indx])
                return false;
        return true;
    }

    public string AsText() => AsText(_id, _emptys, _nulls);

    private static string AsText(ulong[][] numberData, int[] empties, int[] nulls) => numberData
        .Select(arr => arr
            .Select(_ => _.ToString("X16"))
            .Pipe(_ => string.Join("", _))
        ).Pipe(_ => string.Join("_", _))
        .Pipe(datStr => $"{datStr}|{empties.Select(_ => _.ToString("X8"))}|{nulls.Select(_ => _.ToString("X8")).Pipe(_ => string.Join("", _))}");

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj == null)
            return false;
        if (!(obj is ULongId))
            return false;
        var oth = (ULongId)obj;
        if (!AreSame(_nulls, oth._nulls) || !AreSame(_emptys, oth._emptys))
            return false;
        if (_id.Length != oth._id.Length)
            return false;
        for (var indx = 0; indx < _id.Length; indx++)
        {
            if (!AreSame(_id[indx], oth._id[indx]))
                return false;
        }
        return true;
    }


    public override int GetHashCode()
    {
        ulong retVal = 0L;
        foreach (var arr in _id)
            foreach (var ul in arr)
                retVal ^= ul;
        foreach (var i in _nulls)
            retVal ^= ((ulong)i);
        foreach (var i in _emptys)
            retVal ^= ((ulong)i);
        return retVal.GetHashCode();

    }


}

