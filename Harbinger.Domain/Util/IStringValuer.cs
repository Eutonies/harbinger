using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Util;

public interface IStringValuer
{
    bool StringCollectionsAreEqual(IEnumerable<string?> fst, IEnumerable<string?> snd);
    string Aggregate(IEnumerable<string?> input);

    public int HashCodeFor(IEnumerable<string?> input) => Aggregate(input).GetHashCode();

}
