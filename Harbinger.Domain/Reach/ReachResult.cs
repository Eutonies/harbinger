using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Model;

namespace Harbinger.Domain.Reach;

public record ReachResult<TEnt>(
    IReadOnlyCollection<Reach<TEnt>> Reaches,
    HarFram Frame
    )where TEnt : IPlaceable
{




}
