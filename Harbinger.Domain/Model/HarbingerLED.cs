using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Model;

public record HarbingerLED(XY Coordinate) : IPlaceable
{
}
