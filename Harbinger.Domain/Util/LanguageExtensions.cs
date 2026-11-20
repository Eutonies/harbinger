using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Harbinger.Domain.Util;

public static class LanguageExtensions
{

    public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> pipe) => pipe(input);
    public static TOut? Stripe<TOut>(this string? str, Func<string, TOut?> pip) where TOut : class => string.IsNullOrWhiteSpace(str) ? null : str!.Pipe(pip);
    public static TOut? StripeVal<TOut>(this string? str, Func<string, TOut?> pip) where TOut : struct
        => string.IsNullOrWhiteSpace(str) ? null : str!.Pipe(pip);

    public static void Pipa<TIn>(this TIn input, Action<TIn> pipe) => pipe(input);


    public static TOut? Cipe<TIn, TOut>(this TIn input, Func<TIn, bool> condition, Func<TIn, TOut?> pipe) where TOut : class
        => input.Cipe(condition, pipe, null);

    public static TOut? CipeVal<TIn, TOut>(this TIn input, Func<TIn, bool> condition, Func<TIn, TOut> pipe) where TOut : struct
        => input.Cipe(condition, pipe, default);
    public static TOut? CipeValOpt<TIn, TOut>(this TIn input, Func<TIn, bool> condition, Func<TIn, TOut?> pipe) where TOut : struct
        => input.Cipe(condition, pipe, default);

    public static TIn? CipeVal<TIn>(this TIn input, Func<TIn, bool> condition) where TIn : struct => condition(input) ? input : null;

    public static TOut? Cipe<TIn, TOut>(this TIn input, Func<TIn, bool> condition, Func<TIn, TOut?> pipe, TOut? defaultVal) =>
        condition(input) ?
            pipe(input) :
            defaultVal;

    public static T? Cipe<T>(this T input, Func<T, bool> condition) where T : class => input.Cipe(condition, _ => _);

    public static TOut? Dipe<TIn, TKey, TOut>(this IReadOnlyDictionary<TKey, TIn> input, TKey key, Func<TIn, TOut> pipe)
        where TKey : notnull
        where TOut : class
        =>
        input.TryGetValue(key, out var value) ? pipe(value) : null;
    public static TOut? DipeVal<TIn, TKey, TOut>(this IReadOnlyDictionary<TKey, TIn> input, TKey key, Func<TIn, TOut> pipe)
        where TKey : notnull
        where TOut : struct
        =>
        input.TryGetValue(key, out var value) ? pipe(value) : null;

    public static void Dipa<TIn, TKey>(this IReadOnlyDictionary<TKey, TIn> input, TKey key, Action<TIn> pipa)
        where TKey : notnull
    {
        if (input.TryGetValue(key, out var value))
        {
            pipa(value);
        }
    }

    public static TIn Ripa<TIn>(this TIn input, Action<TIn> pipa)
    {
        pipa(input);
        return input;
    }

    public static TOut Ipe<TIn, TOut>(this TIn inp, TOut outp) => outp;

    public static (TOut First, TOut Second) Tipe<TIn, TOut>(this (TIn, TIn) input, Func<TIn, TOut> pipe) =>
        (input.Item1.Pipe(pipe), input.Item2.Pipe(pipe));
    public static (TOut First, TOut Second, TOut Third) Tipe<TIn, TOut>(this (TIn, TIn, TIn) input, Func<TIn, TOut> pipe) =>
        (input.Item1.Pipe(pipe), input.Item2.Pipe(pipe), input.Item3.Pipe(pipe));

    public static T? Nipe<T>(this T obj, Func<T, bool> pred) where T : class => !pred(obj) ? obj : null;
    public static T? NipeVal<T>(this T obj, Func<T, bool> pred) where T : struct => !pred(obj) ? obj : null;
    public static TOut? Nipe<T, TOut>(this T obj, Func<T, bool> pred, Func<T, TOut> map) where TOut : class => !pred(obj) ? map(obj) : null;
    public static TOut? NipeVal<T, TOut>(this T obj, Func<T, bool> pred, Func<T, TOut> map) where TOut : struct => !pred(obj) ? map(obj) : null;

    public static T Fipe<T>(this T obj, Func<T, bool> condition, Func<T, T> changer) =>
        condition(obj) ? changer(obj) : obj;

    public static T? AsNullable<T>(this T ent) where T : class => (T?)ent;

    public static Type AsNotNullableType(this Type typ) => Nullable.GetUnderlyingType(typ) ?? typ;

    public static T? AsNullableVal<T>(this T ent) where T : struct => (T?)ent;


    internal static List<object> ToObjectList(this Array arr)
    {
        var returnee = new List<object>();
        foreach (var el in arr)
            returnee.Add(el);
        return returnee;
    }
    internal static List<T> ToObjectList<T>(this Array arr) where T : class => arr
        .ToObjectList()
        .Select(_ => _ as T)
        .Where(_ => _ != null)
        .Select(_ => _!)
        .ToList();

    internal static List<T> ToValueList<T>(this Array arr) where T : struct => arr
        .ToObjectList()
        .Where(_ => _ is T)
        .Select(_ => (T)_)
        .Select(_ => _)
        .ToList();

    public static IReadOnlyCollection<object> ToReadOnly(this IEnumerator enumer)
    {
        var returnee = new List<object>();
        while (enumer.MoveNext())
            returnee.Add(enumer.Current);
        return returnee;
    }

    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerator<T> enumer)
    {
        var returnee = new List<T>();
        while (enumer.MoveNext())
            returnee.Add(enumer.Current);
        return returnee;
    }
    public static Option<T> Cope<T>(this T input, Func<T, bool> predicate) => predicate(input) ? Option.From(input) : Option.None<T>();

    public static T Fipe<T>(this T obj, Func<T, Option<T>> optMapper) =>
        optMapper(obj).OrElse(obj);

    public static Option<TSub> AsOpt<TSub>(this object obj) where TSub : class => Option.From(obj as TSub);

    public static TOut Decode<TIn, TOut>(this TIn inp, TIn fstCond, TOut fstRes, TOut defRes) where TIn : notnull
        => (inp.Mipe((_ => inp.Equals(fstCond), _ => S(fstRes))) ?? Option.None<TOut>()).OrElse(defRes);

    public static TOut Decode<TIn, TOut>(this TIn inp, TIn fstCond, TOut fstRes, TIn sndCond, TOut sndRes, TOut defRes) where TIn : notnull
        => (inp.Mipe(
              (_ => inp.Equals(fstCond), _ => S(fstRes)),
              (_ => inp.Equals(sndCond), _ => S(sndRes))
            ) ?? Option.None<TOut>()).OrElse(defRes);

    public static TOut Decode<TIn, TOut>(this TIn inp, TIn fstCond, TOut fstRes, TIn sndCond, TOut sndRes, TIn thrdCond, TOut thrdRes, TOut defRes) where TIn : notnull
        => (inp.Mipe(
              (_ => inp.Equals(fstCond), _ => S(fstRes)),
              (_ => inp.Equals(sndCond), _ => S(sndRes)),
              (_ => inp.Equals(thrdCond), _ => S(thrdRes))
            ) ?? Option.None<TOut>()).OrElse(defRes);

    private static Some<T> S<T>(this T inp) => Option.ProduceSome(inp);

    public static TOut? Mipe<TIn, TOut>(
        this TIn inp,
        Func<TIn, bool> predFst,
        Func<TIn, TOut> mapFst,
        Func<TIn, bool>? predSnd = null,
        Func<TIn, TOut>? mapSnd = null,
        Func<TIn, bool>? predThrd = null,
        Func<TIn, TOut>? mapThrd = null,
        Func<TIn, bool>? predFrth = null,
        Func<TIn, TOut>? mapFrth = null)
        where TOut : class => new List<(Func<TIn, bool>? Pred, Func<TIn, TOut>? Map)>
        {
            (predFst, mapFst),
            (predSnd, mapSnd),
            (predThrd, mapThrd),
            (predFrth, mapFrth)
        }.Where(_ => _.Pred != null && _.Map != null)
         .Select(_ => (_.Pred!, _.Map!))
         .ToReadOnly()
         .Pipe(coll => inp.MipeOption(coll).OptRef());

    public static TOut? MipeVal<TIn, TOut>(
        this TIn inp,
        Func<TIn, bool> predFst,
        Func<TIn, TOut> mapFst,
        Func<TIn, bool>? predSnd = null,
        Func<TIn, TOut>? mapSnd = null,
        Func<TIn, bool>? predThrd = null,
        Func<TIn, TOut>? mapThrd = null,
        Func<TIn, bool>? predFrth = null,
        Func<TIn, TOut>? mapFrth = null)
        where TOut : struct => new List<(Func<TIn, bool>? Pred, Func<TIn, TOut>? Map)>
        {
            (predFst, mapFst),
            (predSnd, mapSnd),
            (predThrd, mapThrd),
            (predFrth, mapFrth)
        }.Where(_ => _.Pred != null && _.Map != null)
         .Select(_ => (_.Pred!, _.Map!))
         .ToReadOnly()
         .Pipe(coll => inp.MipeOption(coll).OptVal());


    public static TOut? Mipe<TIn, TOut>(this TIn inp, params IEnumerable<(Func<TIn, bool> Predicate, Func<TIn, TOut> Map)> collectors)
        where TOut : class => inp.MipeOption(collectors).OptRef();

    public static TOut? MipeVal<TIn, TOut>(this TIn inp, params IEnumerable<(Func<TIn, bool> Predicate, Func<TIn, TOut> Map)> collectors)
        where TOut : struct => inp.MipeOption(collectors).OptVal();


    public static Option<TOut> MipeOption<TIn, TOut>(this TIn inp, params IEnumerable<(Func<TIn, bool> Predicate, Func<TIn, TOut> Map)> collectors)
        where TOut : notnull
    {
        foreach (var (pred, map) in collectors)
            if (pred(inp))
                return Option.Some(map(inp));
        return Option.None<TOut>();
    }




}
