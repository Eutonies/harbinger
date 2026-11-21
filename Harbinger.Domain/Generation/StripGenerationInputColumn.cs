using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Generation;


public record StripGenerationInputColumn(int ColumnIndex, int NumberOfLEDs, int XCoordinate)
{
}
