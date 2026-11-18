using System;
using System.Collections.Generic;
using System.Text;

using Harbinger.Domain.Util;

namespace Harbinger.Domain.Generation;

public record StripGenerationInputTurn(int TurnIndex, int LEDIndex, XY? InputCoordinate = null)
{
    public virtual Option<XY> Coordinate => Option.From(InputCoordinate);
    public bool IsSameAs(GenTurn oth) => TurnIndex == oth.TurnIndex && LEDIndex == oth.LEDIndex && ((InputCoordinate, oth.InputCoordinate) switch
    {
        (null, null) => true,
        (null,_) => false,
        (_, null) => false,
        (XY myXy, XY othXy) => myXy.Equals(othXy)
    } );

}

