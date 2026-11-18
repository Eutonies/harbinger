using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Harbinger.Domain.Util;

public interface IOption
{
    object? OptionalObject { get; }
    bool HasValue { get; }
}
public interface IOption<out T>
{

}

public static partial class Option
{
    public static Option<T> Some<T>(T value) => new Some<T>(value);
    public static Option<T> None<T>() => ProduceNone<T>(typeof(T));
    public static Option<T> NoneVal<T>() where T : struct => NoneValue<T>.Instance;
    public static Option<T> NoneRef<T>() where T : class => NoneRefe<T>.Instance;

    public static Option<T> From<T>(T? valueOpt) => valueOpt == null ? ProduceNone<T>() : Some(valueOpt);
    public static Option<T> FromRef<T>(T? valueOpt) where T : class => valueOpt == null ? None<T>() : Some(valueOpt);
    public static Option<T> FromVal<T>(T? valueOpt) where T : struct => valueOpt == null ? NoneVal<T>() : Some(valueOpt.Value);

    internal static readonly Type TypeSome = typeof(Some<int>).GetGenericTypeDefinition();

    public static Some<T> ProduceSome<T>(T inp) => Util.Some.ProduceSome(inp);
    public static None<T> ProduceNone<T>() => Util.None.ProduceNone<T>(typeof(T));
    internal static None<T> ProduceNone<T>(Type typ) => Util.None.ProduceNone<T>(typ);
    internal static object ProduceNone(Type typ) => Util.None.ProduceNone(typ);
    internal static bool IsNone(object obj)
    {
        var typ = obj.GetType();
        var hasValProp = typ.GetProperty(nameof(Option<object>.HasValue));
        if (hasValProp == null)
            return false;
        var returnee = (bool)hasValProp.GetValue(obj);
        return returnee;
    }

    internal static object ProduceSome(object obj)
    {
        var objTyp = obj.GetType();
        var optTyp = TypeSome.MakeGenericType(objTyp);
        var constructor = optTyp.GetConstructor(new Type[] { objTyp })!;
        var returnee = constructor.Invoke(new object[] { obj });
        return returnee;
    }

    /// <summary>
    /// Input dun needs to be an option or we do be in troubles
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    internal static object Get(object obj)
    {
        var objOptTyp = obj.GetType();
        var getProperty = objOptTyp.GetProperty(nameof(Option<object>.Get))!;
        var returnee = getProperty.GetValue(obj)!;
        return returnee;
    }



}

public abstract class Option<T> : IStringEquatable, IOption, IEnumerable<T>
{
    public abstract bool HasValue { get; }
    public bool IsNone => !HasValue;
    public abstract T Get { get; }

    public object? OptionalObject => HasValue ? Get : null;

    public abstract bool TryGet(out T value);
    public virtual Option<T> Or(Option<T> oth) => (HasValue, oth.HasValue) switch
    {
        (true, _) => Option.Some(Get),
        (_, true) => Option.Some(oth.Get),
        _ => Option.ProduceNone<T>()
    };

    public virtual Option<T> Or(Func<Option<T>> othProducer)
    {
        if (HasValue)
            return this;
        var oth = othProducer();
        return oth;
    }


    public virtual T OrElse(T outp) => HasValue ? Get : outp;

    public virtual T OrElse(Func<T> outputCreator) => HasValue ? Get : outputCreator();

    public abstract Option<TOut> Map<TOut>(Func<T, TOut> mapper) where TOut : class;
    public abstract Option<TOut> FlatMap<TOut>(Func<T, Option<TOut>> mapper);
    public abstract Option<TOut> MapVal<TOut>(Func<T, TOut> mapper) where TOut : struct;

    public async Task<Option<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> map)
    {
        var mapped = Map(map);
        if (mapped is Some<Task<TOut>> sum)
        {
            var returnee = await sum.Value;
            return Option.Some(returnee);
        }
        return Option.None<TOut>();
    }

    public async Task<Option<TOut>> FlatMapAsync<TOut>(Func<T, Option<Task<TOut>>> map)
    {
        var mapped = FlatMap(map);
        if (mapped is Some<Task<TOut>> sum)
        {
            var returnee = await sum.Value;
            return Option.Some(returnee);
        }
        return Option.None<TOut>();
    }

    public Option<TOut> MapNone<TOut>(Func<TOut> mapper) => this switch
    {
        None<T> nun => Option.From(mapper()),
        _ => Option.None<TOut>()
    };

    public Option<TOut> FlatMapNone<TOut>(Func<Option<TOut>> mapper) => this switch
    {
        None<T> nun => mapper(),
        _ => Option.None<TOut>()
    };

    public IReadOnlyCollection<T> ToCollection() => HasValue ? new List<T> { Get } : new List<T>();

    public Option<TMap> As<TMap>()
    {
        if (IsNone)
            return Option.None<TMap>();
        var value = Get;
        if (value is TMap mappi)
            return Option.From(mappi);
        return Option.None<TMap>();
    }


    public Option<T> Where(Func<T, bool> predicat) => HasValue && predicat(Get) ? this : Option.None<T>();
    public Option<T> WhereNot(Func<T, bool> predicate) => Where(_ => !predicate(_));
    public virtual void Foreach(Action<T> action)
    {
        if (HasValue)
            action(Get);
    }
    private const string ValSome = "OptS";
    private const string ValNone = "OptN";
    public IReadOnlyCollection<string?> AsOptionalStrings() => this switch
    {
        None<T> => [ValNone],
        Some<T> sum when typeof(T).IsAssignableTo(typeof(IStringEquatable)) => [ValSome, .. ((IStringEquatable)sum.Value!).AsOptionalStrings()],
        _ => [ValSome, this.ToString()]

    };

    public override bool Equals(object? obj) => ((IStringEquatable)this).StringEquals(obj as Option<T>);
    public override int GetHashCode() => ((IStringEquatable)this).GetStringHashCode();


    public int GetStringHashCode() => StringId.StringValuer.HashCodeFor(AsOptionalStrings());

    public bool StringCollectionsAreEqual(IEnumerable<string?> inFst, IEnumerable<string?> inSnd) =>
        StringId.StringValuer.StringCollectionsAreEqual(inFst, inSnd);

    public IEnumerator<T> GetEnumerator() => new OptionEnumerator<T>(this);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Guaranteed to produce result, utilizing contained value if present
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="onSome"></param>
    /// <param name="onNone"></param>
    /// <returns></returns>
    public TOut Split<TOut>(Func<T, TOut> onSome, Func<TOut> onNone)
    {
        if (HasValue)
            return onSome(Get);
        return onNone();
    }

    /// <summary>
    /// Guaranteed to execute one branch or another, utilizing contained value if present
    /// </summary>
    /// <param name="onSome"></param>
    /// <param name="onNone"></param>
    public void SplitAction(Action<T> onSome, Action onNone)
    {
        if (HasValue)
            onSome(Get);
        else onNone();
    }


}

public static class Some
{
    public static Some<T> ProduceSome<T>(this T inp) => new Some<T>(inp);

    public static object ProduceSome(this Type underlyingType, object obj)
    {
        var genericTarget = typeof(Some<>).MakeGenericType([underlyingType]);
        var creatorMeth = genericTarget
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(_ => _.Name == nameof(Some<string>.Produce));
        var returnee = creatorMeth.Invoke(obj: null, [obj]);
        return returnee!;
    }

}

public class Some<T> : Option<T>
{
    public static Some<T> Produce(T inp) => new Some<T>(inp);

    private readonly T _value;
    public Some(T value)
    {
        _value = value;
    }
    public override bool HasValue => true;
    public override T Get => _value;
    public T Value => _value;

    public override Option<TOut> FlatMap<TOut>(Func<T, Option<TOut>> mapper) => mapper(_value);

    public override Option<TOut> Map<TOut>(Func<T, TOut> mapper) => Option.From(mapper(_value));

    public override Option<TOut> MapVal<TOut>(Func<T, TOut> mapper) => Option.Some(mapper(_value));

    public override bool TryGet(out T value)
    {
        value = _value;
        return true;
    }
}

public static class None
{
    public static Option<T> NoneVal<T>() where T : struct => NoneValue<T>.Instance;
    public static Option<T> NoneRef<T>() where T : class => NoneRefe<T>.Instance;

    private static readonly Type TypeSome = typeof(Some<int>).GetGenericTypeDefinition();
    private static readonly Type ValTypeNone = typeof(NoneValue<int>).GetGenericTypeDefinition();
    private static readonly Type RefTypeNone = typeof(NoneRefe<string>).GetGenericTypeDefinition();
    internal static None<T> ProduceNone<T>() => ProduceNone<T>(typeof(T));
    internal static None<T> ProduceNone<T>(Type typ) => (None<T>)ProduceNone(typ);
    internal static object ProduceNone(Type typ)
    {
        if (typ.IsValueType)
        {
            var valRetTyp = ValTypeNone.MakeGenericType(typ);
            var valInstanceProp = valRetTyp.GetField(nameof(NoneValue<int>.Instance), BindingFlags.Static | BindingFlags.Public)!;
            var valRet = valInstanceProp.GetValue(null)!;
            return valRet;
        }
        var refRetTyp = RefTypeNone.MakeGenericType(typ);
        var refInstanceProp = refRetTyp.GetField(nameof(NoneRefe<string>.Instance), BindingFlags.Static | BindingFlags.Public)!;
        var refRet = refInstanceProp.GetValue(null)!;
        return refRet;
    }
}

public abstract class None<T> : Option<T>
{
    public override bool HasValue => false;
    public override T Get => throw new NotSupportedException("This is none!");

    public override Option<TOut> FlatMap<TOut>(Func<T, Option<TOut>> mapper) => Option.None<TOut>();
    public override Option<TOut> Map<TOut>(Func<T, TOut> mapper) => Option.None<TOut>();
    public override Option<TOut> MapVal<TOut>(Func<T, TOut> mapper) => Option.NoneVal<TOut>();


}

internal class NoneRefe<T> : None<T> where T : class
{
    private NoneRefe() { }
    public static readonly NoneRefe<T> Instance = new NoneRefe<T>();


    public override bool TryGet(out T value)
    {
        value = default(T);
        return false;

    }
}

internal class NoneValue<T> : None<T> where T : struct
{
    private NoneValue() { }
    public static readonly NoneValue<T> Instance = new NoneValue<T>();

    public override bool TryGet(out T value)
    {
        value = default;
        return false;
    }
}


