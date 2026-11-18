using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Util;

internal class OptionEnumerator<TEnt> : IEnumerator<TEnt> where TEnt : notnull
{
    internal OptionEnumerator(Option<TEnt> opt)
    {
        _opt = opt;
    }
    private Option<TEnt> _opt;
    private EnumerState _state = EnumerState.Initial;
    public TEnt Current => (_state, _opt) switch
    {
        (_, None<TEnt>) => throw new NotSupportedException("None has not!"),
        (EnumerState.BeforeReturn, Some<TEnt> sum) => sum.Get,
        _ => throw new NotSupportedException("Has no more elemnents")
    };

    object IEnumerator.Current => Current;

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
        if (_opt.HasValue && _state == EnumerState.Initial)
        {
            _state = Next(_state);
            return true;
        }
        _state = Next(_state);
        return false;
    }

    public void Reset()
    {
        _state = EnumerState.Initial;
    }

    private static EnumerState Next(EnumerState stat) => stat switch
    {
        EnumerState.Initial => EnumerState.BeforeReturn,
        EnumerState.BeforeReturn => EnumerState.HasReturned,
        _ => EnumerState.HasReturned
    };


    private enum EnumerState
    {
        Initial = 0,
        BeforeReturn = 10,
        HasReturned = 30
    }


}