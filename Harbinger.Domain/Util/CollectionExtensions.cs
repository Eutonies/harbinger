using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Harbinger.Domain.Util;

public static class CollectionExtensions
{

    public static bool Empty<T>(this IEnumerable<T> inp) => !inp.Any();

    public static bool Empty<T>(this IEnumerable<T> inp, Func<T, bool> predicate) =>
        !inp.Any(predicate);

    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> inp) => inp.ToList();

    public static IReadOnlyCollection<T> WhereNot<T>(this IEnumerable<T> inp, Func<T, bool> predicate) => inp
        .Where(_ => !predicate(_))
        .ToReadOnly();

    public static IReadOnlyCollection<T> WhereNotNull<T>(this IEnumerable<T?> inp) where T : class => inp
        .Where(_ => _ != null)
        .Select(_ => _!)
        .ToReadOnly();

    public static IReadOnlyCollection<T> WhereNotNullVal<T>(this IEnumerable<T?> inp) where T : struct => inp
        .Where(_ => _ != null)
        .Select(_ => _!.Value)
        .ToReadOnly();

    public static IReadOnlyCollection<T>? NullIfEmpty<T>(this IEnumerable<T> coll) =>
        coll.Empty() ? null : coll.ToReadOnly();

    public static IReadOnlyCollection<TSub> Collects<T, TSub>(this IEnumerable<T> inp) where TSub : class =>
        inp.Select(_ => _ as TSub)
        .WhereNotNull();
    public static IReadOnlyCollection<TSub> Collects<TSub>(this IEnumerable<object> inp) where TSub : class =>
        inp.Select(_ => _ as TSub)
        .WhereNotNull();


    public static IReadOnlyCollection<TOut> WhereMap<TIn, TOut>(this IEnumerable<TIn> inp, Func<TIn, bool> predicate, Func<TIn, TOut> mapper) => inp
        .Where(predicate)
        .Select(mapper)
        .ToReadOnly();


    public static IReadOnlyCollection<(TIn Fst, TVal Snd, TKey Key)> WhereMap<TIn, TKey, TVal>(
        this IEnumerable<TIn> inp,
        IReadOnlyDictionary<TKey, TVal> map,
        Func<TIn, TKey> keyer) where TKey : notnull => inp.WhereMap(map, keyer: keyer, valuer: (en, key, val) => (en, val, key));

    public static IReadOnlyCollection<TOut> WhereMap<TIn, TKey, TVal, TOut>(
         this IEnumerable<TIn> inp,
         IReadOnlyDictionary<TKey, TVal> map,
         Func<TIn, TKey> keyer,
         Func<TIn, TKey, TVal, TOut> valuer) where TKey : notnull
    {
        var returnee = new List<TOut>();
        foreach (var en in inp)
        {
            var key = keyer(en);
            if (map.TryGetValue(key, out var val))
                returnee.Add(valuer(en, key, val));
        }
        return returnee;
    }

    public static void RemoveWhere<T>(this List<T> input, Func<T, bool> predicate)
    {
        var toRemove = input
            .Where(predicate)
            .ToReadOnly();
        if (toRemove.Empty())
            return;
        foreach (var ent in toRemove)
            input.Remove(ent);
    }
    public static void RemoveWhere<T>(this HashSet<T> input, Func<T, bool> predicate)
    {
        var toRemove = input
            .Where(predicate)
            .ToReadOnly();
        if (toRemove.Empty())
            return;
        foreach (var ent in toRemove)
            input.Remove(ent);
    }


    public static (IReadOnlyCollection<T> First, IReadOnlyCollection<T> Second) Partition<T>(this IEnumerable<T> inp, Func<T, bool> pred)
    {
        var (fst, snd) = Tuples.InitTuple2(() => new List<T>());
        foreach (var en in inp)
            if (pred(en))
                fst.Add(en);
            else snd.Add(en);
        return (fst.ToReadOnly(), snd.ToReadOnly());
    }

    public static (IReadOnlyCollection<T> First, IReadOnlyCollection<T> Second, IReadOnlyCollection<T> Third) Partition<T>(
        this IEnumerable<T> inp,
        Func<T, bool> predFirst,
        Func<T, bool> predSnd) => inp.Partition(predFirst)
            .Pipe(pa => pa.Second.Partition(predSnd)
                .Pipe(pb => (pa.First, pb.First, pb.Second))
            );

    public static IReadOnlyCollection<(T Entity, int Index)> ZipWithIndex<T>(this IEnumerable<T> inp) => inp
        .Select((T ent, int indx) => (ent, indx))
        .ToReadOnly();


    public static string MakeString(this IEnumerable<string> inp, string separator = "", string? prefix = null, string? suffix = null)
    {
        var sb = new StringBuilder();
        prefix?.Pipa(_ => sb.Append(_));
        var inpLength = inp.Count();
        foreach (var (ent, indx) in inp.Select((ent, indx) => (ent, indx)))
        {
            sb.Append(ent);
            if (indx < inpLength - 1)
                sb.Append(separator);
        }
        suffix?.Pipa(_ => sb.Append(_));
        return sb.ToString();

    }

    public static IReadOnlyCollection<object> ToObjectEnumerable(this IEnumerable enumerable)
    {
        var returnee = new List<object>();
        var enumer = enumerable.GetEnumerator();
        while (enumer.MoveNext())
        {
            enumer.Current?.Pipa(returnee.Add);
        }
        return returnee;
    }


    public static TStat Fold<TIn, TStat>(
        this TStat initial,
        IEnumerable<TIn> applicants,
        Func<TStat, TIn, TStat> toApply
        )
    {
        var returnee = initial;
        foreach (var app in applicants)
            returnee = toApply(returnee, app);
        return returnee;
    }

    public static IReadOnlyCollection<IReadOnlyCollection<T>> Columnize<T>(this IEnumerable<T> inputs, int noOfColumns)
    {
        var inArr = inputs.ToArray();
        var returnee = Enumerable.Range(0, noOfColumns)
            .Select(_ => new List<T>())
            .ToArray();
        for (var i = 0; i < inputs.Count(); i++)
        {
            var col = i % noOfColumns;
            returnee[col].Add(inArr[i]);
        }
        return returnee
            .Select(_ => _.ToReadOnly())
            .ToReadOnly();

    }

    public static IReadOnlyCollection<string> PrependIfNotNullOrEmpty(this IEnumerable<string> inp, string? value) =>
        (value?.Nipe(string.IsNullOrWhiteSpace, val => inp.Prepend(val)) ?? inp).ToReadOnly();

    public static IReadOnlyCollection<string> AppendIfNotNullOrEmpty(this IEnumerable<string> inp, string? value) =>
        (value?.Nipe(string.IsNullOrWhiteSpace, val => inp.Append(val)) ?? inp).ToReadOnly();


    public static (IReadOnlyCollection<TFst> First, IReadOnlyCollection<TIn> Others) TypePartition<TIn, TFst>(this IEnumerable<TIn> inp)
        where TFst : TIn => TypePartition<TIn, TFst, TIn, TIn, TIn, TIn, TIn, TIn, TIn, TIn>(inp)
            .Pipe(res => (res.First, res.ConcatOthers()));

    public static (IReadOnlyCollection<TFst> First, IReadOnlyCollection<TSnd> Second, IReadOnlyCollection<TIn> Others) TypePartition<TIn, TFst, TSnd>(this IEnumerable<TIn> inp)
        where TFst : TIn
        where TSnd : TIn => TypePartition<TIn, TFst, TSnd, TIn, TIn, TIn, TIn, TIn, TIn, TIn>(inp)
            .Pipe(res => (res.First, res.Second, res.ConcatOthers()));

    public static TypeSplitResult<TIn, TFst, TSnd, TThrd, TFrth, TFth, TSxth, TSnth, TGth, TNth> TypePartition<TIn, TFst, TSnd, TThrd, TFrth, TFth, TSxth, TSnth, TGth, TNth>(this IEnumerable<TIn> input)
        where TFst : TIn
        where TSnd : TIn
        where TThrd : TIn
        where TFrth : TIn
        where TFth : TIn
        where TSxth : TIn
        where TSnth : TIn
        where TGth : TIn
        where TNth : TIn
    {
        var (fst, snd, thrd, frth, ffthm) = (new List<TFst>(), new List<TSnd>(), new List<TThrd>(), new List<TFrth>(), new List<TFth>());
        var (sxth, svnth, gth, nth) = (new List<TSxth>(), new List<TSnth>(), new List<TGth>(), new List<TNth>());
        var theRest = new List<TIn>();
        foreach (var en in input)
        {
            if (en is TFst fs) fst.Add(fs);
            else if (en is TSnd sn) snd.Add(sn);
            else if (en is TThrd tr) thrd.Add(tr);
            else if (en is TFrth four) frth.Add(four);
            else if (en is TFth fiv) ffthm.Add(fiv);
            else if (en is TSxth siz) sxth.Add(siz);
            else if (en is TSnth sev) svnth.Add(sev);
            else if (en is TGth eigt) gth.Add(eigt);
            else if (en is TNth nin) nth.Add(nin);
            else theRest.Add(en);
        }
        return new(fst, snd, thrd, frth, ffthm, sxth, svnth, gth, nth, theRest);

    }

    public record TypeSplitResult<TIn, TFst, TSnd, TThrd, TFrth, TFth, TSxth, TSnth, TGth, TNth>(
        IReadOnlyCollection<TFst> First,
        IReadOnlyCollection<TSnd> Second,
        IReadOnlyCollection<TThrd> Third,
        IReadOnlyCollection<TFrth> Fourth,
        IReadOnlyCollection<TFth> Fifth,
        IReadOnlyCollection<TSxth> Sixth,
        IReadOnlyCollection<TSnth> Seventh,
        IReadOnlyCollection<TGth> Eigth,
        IReadOnlyCollection<TNth> Ninth,
        IReadOnlyCollection<TIn> Others
        )
        where TFst : TIn
        where TSnd : TIn
        where TThrd : TIn
        where TFrth : TIn
        where TFth : TIn
        where TSxth : TIn
        where TSnth : TIn
        where TGth : TIn
        where TNth : TIn
    {
        public IReadOnlyCollection<TIn> ConcatOthers()
        {
            var returnee = Others.ToList();
            var toTry = new List<(Type ActTyp, IReadOnlyCollection<TIn>)>
            {
                (typeof(TNth), Cast(Ninth)),
                (typeof(TGth), Cast(Eigth)),
                (typeof(TSnth), Cast(Seventh)),
                (typeof(TSxth), Cast(Sixth)),
                (typeof(TFth), Cast(Fifth)),
                (typeof(TFrth), Cast(Fourth)),
                (typeof(TThrd), Cast(Third)),
                (typeof(TSnd), Cast(Second)),
                (typeof(TFst), Cast(First))

            };
            var inTyp = typeof(TIn);
            foreach (var (typ, coll) in toTry)
            {
                if (typ != inTyp)
                    return returnee;
                returnee.AddRange(coll);
            }
            return returnee;

        }
        private static IReadOnlyCollection<TIn> Cast<TOth>(IReadOnlyCollection<TOth> oth) where TOth : TIn =>
            oth.Select(_ => (TIn)_).ToReadOnly();

    }

    public static bool TryFind<T>(this IEnumerable<T> inp, Func<T, bool> predicate, [NotNullWhen(returnValue: true)] T? result) where T : class
    {
        foreach (var ent in inp)
        {
            if (predicate(ent))
            {
                result = ent;
                return true;
            }
        }
        result = null;
        return false;
    }

    public static bool TryFindVal<T>(this IEnumerable<T> inp, Func<T, bool> predicate, [NotNullWhen(returnValue: true)] T? result) where T : struct
    {
        foreach (var ent in inp)
        {
            if (predicate(ent))
            {
                result = ent;
                return true;
            }
        }
        result = null;
        return false;
    }

    public static IReadOnlyCollection<T> WhereNotNone<T, TKey>(this IEnumerable<T> inp, Func<T, Option<TKey>> keyer)
        where TKey : notnull =>
        inp.Select(_ => (keyer(_), _))
           .WhereNotNone();

    public static IReadOnlyCollection<T> WhereNotNone<T, TKey>(this IEnumerable<(Option<TKey> Key, T Entity)> inp) => inp
        .Where(_ => _.Key.HasValue)
        .Select(_ => _.Entity)
        .ToReadOnly();

    public static IReadOnlyCollection<T> WhereNotNone<T>(this IEnumerable<Option<T>> inp) => inp
        .Where(_ => _.HasValue)
        .Select(_ => _.Get!)
        .ToReadOnly();

    public static IReadOnlyCollection<TOut> WhereMap<TIn, TOut>(this IEnumerable<TIn> inp, Func<TIn, Option<TOut>> mapper) => inp
        .Select(mapper)
        .Where(_ => _.HasValue)
        .Select(_ => _.Get)
        .ToReadOnly();

    public static IReadOnlyCollection<TOut> WhereMap<TIn, TKey, TOut>(this IEnumerable<TIn> inp, Func<TIn, Option<TKey>> keyer, Func<TIn, TKey, TOut> mapper) => inp
        .Select(_ => (Inp: _, Key: keyer(_)))
        .Where(_ => _.Key.HasValue)
        .Select(_ => mapper(_.Inp, _.Key.Get))
        .ToReadOnly();


    public static IReadOnlyCollection<(TIn Entity, TKey Key)> SelectWithKeyOpt<TIn, TKey>(this IEnumerable<TIn> inp, Func<TIn, Option<TKey>> keyer)
        where TKey : notnull => inp
            .Select(_ => (Key: keyer(_), Entity: _))
            .Where(_ => _.Key.HasValue)
            .Select(_ => (_.Entity, Key: _.Key.Get))
            .ToReadOnly();


    public static Option<T> FirstSome<T>(this IEnumerable<Option<T>> inp) => inp.FirstOrDefault(_ => _.HasValue)?
        .Pipe(_ => Option.From(_.Get)) ?? Option.None<T>();

    public static Option<TOut> FirstSome<TIn, TOut>(this IEnumerable<Option<TIn>> inp, Func<TIn, Option<TOut>> flatMapper)
        where TIn : notnull where TOut : notnull
    {
        foreach (var inpt in inp.Where(_ => _.HasValue))
        {
            var outP = inpt.FlatMap(flatMapper);
            if (outP.HasValue)
                return outP;
        }
        return Option.None<TOut>();
    }

    public static Option<T> FirstOption<T>(this IEnumerable<T> inp) => inp.Any() ? Option.Some(inp.First()) : Option.None<T>();

    public static Option<T> FirstOption<T>(this IEnumerable<T> inp, Func<T, bool> predicate)
    {
        foreach (var ent in inp)
        {
            if (predicate(ent))
                return Option.From(ent);
        }
        return Option.None<T>();
    }

    public static Option<TOut> CollectOpt<TIn, TOut>(this IEnumerable<TIn> inp, Func<TIn, Option<TOut>> collector)
    {
        foreach (var inpt in inp)
        {
            var mapped = collector(inpt);
            if (mapped.HasValue)
                return mapped;
        }
        return Option.None<TOut>();
    }


    public static bool TryFindOption<T>(this IEnumerable<T> inp, Func<T, bool> predicate, out Option<T> result)
    {
        foreach (var ent in inp)
        {
            if (predicate(ent))
            {
                result = Option.From(ent);
                return true;
            }
        }
        result = Option.None<T>();
        return false;
    }

    public static IReadOnlyCollection<ReferencePair<T, int>> ZipWithIndexRef<T>(this IEnumerable<T> inp) => inp
    .Select((T ent, int indx) => new ReferencePair<T, int>(ent, indx))
    .ToReadOnly();

    public static IReadOnlyCollection<IGrouping<TKey, TIn>> GroupOpt<TKey, TIn>(this IEnumerable<TIn> inp, Func<TIn, Option<TKey>> grouper)
        where TKey : notnull => inp
            .Select(_ => (Entry: _, KeyOpt: grouper(_)))
            .Where(_ => _.KeyOpt.HasValue)
            .Select(_ => (Key: _.KeyOpt.Get, _.Entry))
            .GroupBy(_ => _.Key, _ => _.Entry)
            .ToReadOnly();


    public static IReadOnlyCollection<TEnt> ConcatOpt<TEnt>(this IEnumerable<TEnt> coll, Option<IEnumerable<TEnt>> opt) =>
        coll.ConcatOpt<TEnt, IEnumerable<TEnt>>(opt);

    public static IReadOnlyCollection<TEnt> ConcatOpt<TEnt, TColl>(this IEnumerable<TEnt> coll, Option<TColl> opt) where TColl : IEnumerable<TEnt> =>
        opt.Map(othCol => coll.Concat(othCol)).OrElse(coll).ToReadOnly();


}
