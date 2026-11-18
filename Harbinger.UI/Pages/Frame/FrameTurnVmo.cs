using Harbinger.Domain.Util;

namespace Harbinger.UI.Pages.Frame;

public record FrameTurnVmo(int TurnIndex, Option<int> LEDIndexInput, Option<XY> CoordinateInput, Action OnUpdate)
{

    private Option<int> _ledIndex = LEDIndexInput;
    public int? LEDIndex
    {
        get => _ledIndex.OptVal();
        set
        {
            var optVal = Option.FromVal(value?.CipeVal(_ => _ != 0));
            if (_ledIndex.IsNone && optVal.IsNone)
                return;
            foreach (var v in optVal)
                foreach (var exi in _ledIndex)
                    if (v == exi)
                        return;
            _ledIndex= optVal;
            OnUpdate();
        }
    }


    private Option<int> _x = CoordinateInput.MapVal(_ => _.X);
    public int? XCoordinate
    {
        get => _x.OptVal();
        set
        {
            var optVal = Option.FromVal(value);
            if (_x.IsNone && optVal.IsNone)
                return;
            foreach (var v in optVal)
                foreach (var exi in _x)
                    if (v == exi)
                        return;
            _x = optVal;
            OnUpdate();
        }
    }


    private Option<int> _y = CoordinateInput.MapVal(_ => _.Y);
    public int? YCoordinate
    {
        get => _y.OptVal();
        set
        {
            var optVal = Option.FromVal(value);
            if (_y.IsNone && optVal.IsNone)
                return;
            foreach (var v in optVal)
                foreach (var exi in _y)
                    if (v == exi)
                        return;
            _y = optVal;
            OnUpdate();
        }
    }

    private Option<XY> Coordinates => _x
        .FlatMap(x => _y
           .Map(y => new XY(x * 10, y * 10))
        );

    public Option<GenTurn> ToGenerationInput() => _ledIndex
        .Map(indx => new GenTurn(TurnIndex: TurnIndex, LEDIndex: indx, InputCoordinate: Coordinates.OptRef()));



}
