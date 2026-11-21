using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Generation;

public record StripGenerationInputTurn(int TurnIndex, int LEDIndex, XY? InputCoordinate = null)
{
    public virtual Option<XY> Coordinate => Option.From(InputCoordinate);
}

