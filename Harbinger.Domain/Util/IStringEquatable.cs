using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Util;

public interface IStringEquatable
{
    IReadOnlyCollection<string?> AsOptionalStrings();

    public bool StringEquals(IStringEquatable? oth) => oth == null ? false :
        (GetType() != oth.GetType() ? false : StringCollectionsAreEqual(AsOptionalStrings(), oth.AsOptionalStrings()));

    int GetStringHashCode();

    bool StringCollectionsAreEqual(IEnumerable<string?> inFst, IEnumerable<string?> inSnd);
}
