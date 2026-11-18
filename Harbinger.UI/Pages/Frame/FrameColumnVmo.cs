using Harbinger.Domain.Util;

namespace Harbinger.UI.Pages.Frame;

public record FrameColumnVmo(int ColumnIndex, Option<int> NumberOfLEDsInput, Option<int> XCoordinateInput, Action OnUpdate, FrameSessionVmo Session)
{
    public Option<int> NumberOfLEDSuggestion => Session.ColumnLEDCountSuggestionFor(ColumnIndex);
    private Option<int> _numberOfLEDs = NumberOfLEDsInput;
    public int? NumberOfLEDs
    {
        get => _numberOfLEDs.OptVal();
        set
        {
            var optVal = Option.FromVal(value?.CipeVal(_ => _ != 0));
            if (_numberOfLEDs.IsNone && optVal.IsNone)
                return;
            foreach (var v in optVal)
                foreach (var exi in _numberOfLEDs)
                    if (v == exi)
                        return;
            _numberOfLEDs = optVal;
            OnUpdate();
        }
    }

    private Option<int> _xCoordinate = XCoordinateInput;
    public int? XCoordinate
    {
        get => _xCoordinate.OptVal();
        set
        {
            var optVal = Option.FromVal(value?.CipeVal(_ => _ != 0));
            if (_xCoordinate.IsNone && optVal.IsNone)
                return;
            foreach (var v in optVal)
                foreach (var exi in _xCoordinate)
                    if (v == exi)
                        return;
            _xCoordinate = optVal;
            OnUpdate();
        }
    }


    public Option<GenCol> ToGenerationInput() => _xCoordinate
        .FlatMap(x => _numberOfLEDs
            .Map(num => new GenCol(ColumnIndex: ColumnIndex, NumberOfLEDs: num, XCoordinate: x * 10)
            )
        );


}
