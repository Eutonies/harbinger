using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Util;

public static class Tuples
{


    public static (TFirst? First, TSecond? Second) Nulls<TFirst, TSecond>() where TFirst : class where TSecond : class => (null, null);

    public static (TFirst? First, TSecond? Second, TThird? Third) Nulls<TFirst, TSecond, TThird>()
        where TFirst : class
        where TSecond : class
        where TThird : class
        => (null, null, null);

    public static (TFirst? First, TSecond? Second, TThird? Third, TFourth? Forth) Nulls<TFirst, TSecond, TThird, TFourth>()
        where TFirst : class
        where TSecond : class
        where TThird : class
        where TFourth : class
        => (null, null, null, null);


    public static (T First, T Second) InitTuple2<T>(this Func<T> initer) =>
        (initer(), initer());
    public static (T First, T Second, T Third) InitTuple3<T>(this Func<T> initer) =>
        (initer(), initer(), initer());
    public static (T First, T Second, T Third, T Fourth) InitTuple4<T>(this Func<T> initer) =>
        (initer(), initer(), initer(), initer());
    public static (T First, T Second, T Third, T Fourth, T Fifth) InitTuple5<T>(this Func<T> initer) =>
        (initer(), initer(), initer(), initer(), initer());
    public static (T First, T Second, T Third, T Fourth, T Fifth, T Sixth) InitTuple6<T>(this Func<T> initer) =>
        (initer(), initer(), initer(), initer(), initer(), initer());
    public static (T First, T Second, T Third, T Fourth, T Fifth, T Sixth, T Seventh) InitTuple7<T>(this Func<T> initer) =>
       (initer(), initer(), initer(), initer(), initer(), initer(), initer());
    public static (T First, T Second, T Third, T Fourth, T Fifth, T Sixth, T Seventh, T Eighth) InitTuple8<T>(this Func<T> initer) =>
        (initer(), initer(), initer(), initer(), initer(), initer(), initer(), initer());
    public static (T First, T Second, T Third, T Fourth, T Fifth, T Sixth, T Seventh, T Eighth, T Ninth) InitTuple9<T>(this Func<T> initer) =>
        (initer(), initer(), initer(), initer(), initer(), initer(), initer(), initer(), initer());


    public static (IReadOnlyCollection<TOut> First, IReadOnlyCollection<TOut> Second) Select<TIn, TOut>(this (IEnumerable<TIn> First, IEnumerable<TIn> Second) inps, Func<TIn, TOut> mapper) =>
        (inps.First.Select(mapper).ToReadOnly(), inps.Second.Select(mapper).ToReadOnly());
    public static (IReadOnlyCollection<TOut> First, IReadOnlyCollection<TOut> Second, IReadOnlyCollection<TOut> Third) Select<TIn, TOut>(this (IEnumerable<TIn> First, IEnumerable<TIn> Second, IEnumerable<TIn> Third) inps, Func<TIn, TOut> mapper) =>
        (inps.First.Select(mapper).ToReadOnly(), inps.Second.Select(mapper).ToReadOnly(), inps.Third.Select(mapper).ToReadOnly());



    public static (TOut First, TOut Second) Pipe<TIn, TOut>(this (TIn First, TIn Second) tuppie, Func<TIn, TOut> map) =>
        tuppie.Map(map);

    public static (TOut First, TOut Second) Map<TIn, TOut>(this (TIn First, TIn Second) tuppie, Func<TIn, TOut> map) =>
        (First: map(tuppie.First), Second: map(tuppie.Second));

    public static (TOut First, TOut Second, TOut Third) Map<TIn, TOut>(this (TIn First, TIn Second, TIn Third) tuppie, Func<TIn, TOut> map) =>

        (First: map(tuppie.First), Second: map(tuppie.Second), Third: map(tuppie.Third));

    public static async Task<(TFst First, TSnd Second)> Await<TFst, TSnd>(this (Task<TFst> Fst, Task<TSnd> Snd) inp)
    {
        await Task.WhenAll(inp.Fst, inp.Snd);
        return (inp.Fst.Result, inp.Snd.Result);
    }
    public static async Task<(TFst First, TSnd Second, TThrd Third)> Await<TFst, TSnd, TThrd>(this (Task<TFst> Fst, Task<TSnd> Snd, Task<TThrd> Thrd) inp)
    {
        await Task.WhenAll(inp.Fst, inp.Snd, inp.Thrd);
        return (inp.Fst.Result, inp.Snd.Result, inp.Thrd.Result);
    }
    public static async Task<(TFst First, TSnd Second, TThrd Third, TFth Fourth)> Await<TFst, TSnd, TThrd, TFth>
            (this (Task<TFst> Fst, Task<TSnd> Snd, Task<TThrd> Thrd, Task<TFth> Frth) inp)
    {
        await Task.WhenAll(inp.Fst, inp.Snd, inp.Thrd, inp.Frth);
        return (inp.Fst.Result, inp.Snd.Result, inp.Thrd.Result, inp.Frth.Result);
    }
    public static async Task<(TFst First, TSnd Second, TThrd Third, TFth Fourth, TFfth Fifth)> Await<TFst, TSnd, TThrd, TFth, TFfth>
            (this (Task<TFst> Fst, Task<TSnd> Snd, Task<TThrd> Thrd, Task<TFth> Frth, Task<TFfth> Ffth) inp)
    {
        await Task.WhenAll(inp.Fst, inp.Snd, inp.Thrd, inp.Frth, inp.Ffth);
        return (inp.Fst.Result, inp.Snd.Result, inp.Thrd.Result, inp.Frth.Result, inp.Ffth.Result);
    }
    public static async Task<(TFst First, TSnd Second, TThrd Third, TFth Fourth, TFfth Fifth, TSxth Sixth)> Await<TFst, TSnd, TThrd, TFth, TFfth, TSxth>
            (this (Task<TFst> Fst, Task<TSnd> Snd, Task<TThrd> Thrd, Task<TFth> Frth, Task<TFfth> Ffth, Task<TSxth> Sxth) inp)
    {
        await Task.WhenAll(inp.Fst, inp.Snd, inp.Thrd, inp.Frth, inp.Ffth, inp.Sxth);
        return (inp.Fst.Result, inp.Snd.Result, inp.Thrd.Result, inp.Frth.Result, inp.Ffth.Result, inp.Sxth.Result);
    }
    public static async Task<(TFst First, TSnd Second, TThrd Third, TFth Fourth, TFfth Fifth, TSxth Sixth, TSvnth Seventh)> Await<TFst, TSnd, TThrd, TFth, TFfth, TSxth, TSvnth>
            (this (Task<TFst> Fst, Task<TSnd> Snd, Task<TThrd> Thrd, Task<TFth> Frth, Task<TFfth> Ffth, Task<TSxth> Sxth, Task<TSvnth> Svnth) inp)
    {
        await Task.WhenAll(inp.Fst, inp.Snd, inp.Thrd, inp.Frth, inp.Ffth, inp.Sxth, inp.Svnth);
        return (inp.Fst.Result, inp.Snd.Result, inp.Thrd.Result, inp.Frth.Result, inp.Ffth.Result, inp.Sxth.Result, inp.Svnth.Result);
    }

    public static async Task<(TFst First, TSnd Second, TThrd Third, TFth Fourth, TFfth Fifth, TSxth Sixth, TSvnth Seventh, TEght Eighth)> Await<TFst, TSnd, TThrd, TFth, TFfth, TSxth, TSvnth, TEght>
            (this (Task<TFst> Fst, Task<TSnd> Snd, Task<TThrd> Thrd, Task<TFth> Frth, Task<TFfth> Ffth, Task<TSxth> Sxth, Task<TSvnth> Svnth, Task<TEght> Egth) inp)
    {
        await Task.WhenAll(inp.Fst, inp.Snd, inp.Thrd, inp.Frth, inp.Ffth, inp.Sxth, inp.Svnth, inp.Egth);
        return (inp.Fst.Result, inp.Snd.Result, inp.Thrd.Result, inp.Frth.Result, inp.Ffth.Result, inp.Sxth.Result, inp.Svnth.Result, inp.Egth.Result);
    }

    public static async Task<(TFst First, TSnd Second, TThrd Third, TFth Fourth, TFfth Fifth, TSxth Sixth, TSvnth Seventh, TEght Eighth, TNnth Nineth)> Await<TFst, TSnd, TThrd, TFth, TFfth, TSxth, TSvnth, TEght, TNnth>
            (this (Task<TFst> Fst, Task<TSnd> Snd, Task<TThrd> Thrd, Task<TFth> Frth, Task<TFfth> Ffth, Task<TSxth> Sxth, Task<TSvnth> Svnth, Task<TEght> Egth, Task<TNnth> Nnnth) inp)
    {
        await Task.WhenAll(inp.Fst, inp.Snd, inp.Thrd, inp.Frth, inp.Ffth, inp.Sxth, inp.Svnth, inp.Egth, inp.Nnnth);
        return (inp.Fst.Result, inp.Snd.Result, inp.Thrd.Result, inp.Frth.Result, inp.Ffth.Result, inp.Sxth.Result, inp.Svnth.Result, inp.Egth.Result, inp.Nnnth.Result);
    }
}
