using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Model;

public static class HarbingerMath
{
    public static ushort Avg(this ushort fst, ushort snd) => Avg([fst, snd]);

    public static ushort Avg(params IEnumerable<ushort> inputs)
    {
        if (!inputs.Any())
            return 0;
        var returnee = 0L;
        foreach (var inp in inputs)
            returnee += inp;
        returnee = returnee / inputs.Count();
        return (ushort)returnee;
    }

    public static ushort Avg<TEnt>(this IEnumerable<TEnt> inputs, Func<TEnt, ushort> extractor) => inputs
        .Select(extractor)
        .Pipe(Avg);


}
