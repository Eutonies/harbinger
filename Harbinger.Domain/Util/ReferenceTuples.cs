using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Util;

public record ReferencePair<TFst, TSnd>(TFst First, TSnd Second)
{
    public void Deconstruct(out TFst first, out TSnd second)
    {
        first = First;
        second = Second;
    }
}
