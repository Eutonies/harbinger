using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Harbinger.Domain.Util;

public static class LanguageExtensions
{

    public static TOut Pipe<TIn, TOut>(this TIn inp, Func<TIn, TOut> map) => map(inp);


    public static TOut? Cipe<TIn, TOut>(this TIn inp, Func<TIn, bool> predicate, Func<TIn, TOut> map) where TOut : class =>
        predicate(inp) ?
        inp.Pipe(map) :
        null;
    public static TOut? CipeVal<TIn, TOut>(this TIn inp, Func<TIn, bool> predicate, Func<TIn, TOut> map) where TOut : struct =>
        predicate(inp) ?
        inp.Pipe(map) :
        null;

}
