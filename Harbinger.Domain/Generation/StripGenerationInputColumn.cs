using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Generation;


public record StripGenerationInputColumn(int ColumnIndex, int NumberOfLEDs, int XCoordinate)
{

    public bool IsSameAs(GenCol oth) => ColumnIndex == oth.ColumnIndex && NumberOfLEDs == oth.NumberOfLEDs && XCoordinate == oth.XCoordinate;

}
