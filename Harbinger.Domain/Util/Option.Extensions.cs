using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Util;

public static class OptionExtensions
{
    public static T? OptVal<T>(this Option<T> opt) where T : struct =>
        opt.HasValue ? opt.Get : null;
    public static T? OptRef<T>(this Option<T> opt) where T : class =>
        opt.HasValue ? opt.Get : null;

    public static Option<T> ToOptVal<T>(this T? val) where T : struct => Option.FromVal(val);
    public static Option<T> ToOpt<T>(this T? val) where T : class => Option.From(val);


    public static Option<TOut> OptMap<T, TOut>(this Option<T> opt, Func<T, TOut?> mapper) => opt
        .FlatMap(inOpt => mapper(inOpt) switch
        {
            null => Option.None<TOut>(),
            TOut outi => Option.Some(outi)
        });

    public static Option<string> NotEmpty(this Option<string> opt) => opt
        .WhereNot(string.IsNullOrWhiteSpace);

    public static Option<T> Flatten<T>(this Option<Option<T>> opt) => opt switch
    {
        Some<Option<T>> sum => sum.Value,
        _ => Option.None<T>()
    };

    public static void AddOpt<T>(this List<T> list, Option<T> opt)
    {
        foreach (var ent in opt)
            list.Add(ent);
    }
    public static void AddOpt<T>(this HashSet<T> set, Option<T> opt)
    {
        foreach (var ent in opt)
            set.Add(ent);
    }

    private static Type? _typeSome;
    private static Type TypeSome => _typeSome ??= Option.TypeSome;
    private static Type? _typeGenericOption;
    private static Type TypeGenericOption => _typeGenericOption ??= typeof(Option<>);

    public static Option<object> CastAsObjectOpt(this object? obj)
    {
        if (obj == null)
            return Option.None<object>();
        var typ = obj.GetType();
        if (typ.IsGenericType && typ.GetGenericTypeDefinition() == TypeSome)
        {
            var valueProp = typ.GetProperty(nameof(Some<string>.Value));
            var gettedValue = valueProp?.GetValue(obj);
            var returnee = Option.From(gettedValue);
            return returnee;
        }
        return Option.None<object>();
    }


    public static Type NonNullableAndNonOptionType(this object obj) => obj.GetType().NonNullableAndNonOptionType();

    public static Type NonNullableAndNonOptionType(this Type curTyp)
    {
        curTyp = curTyp.AsNotNullableType();
        var returnee = curTyp.FindUnderlyingTypeOfOption(recurseOn: true, isWrappedInOption: false);
        return returnee.OrElse(curTyp);
    }

    public static Option<Type> UnderlyingTypeOfOption(this object? obj, bool recursive = false)
    {
        if (obj == null)
            return Option.None<Type>();
        var returnee = obj.GetType().FindUnderlyingTypeOfOption(recurseOn: recursive, isWrappedInOption: false);
        return returnee;
    }

    private static Option<Type> FindUnderlyingTypeOfOption(this Type curTyp, bool recurseOn, bool isWrappedInOption = false)
    {
        if (!recurseOn && isWrappedInOption)
            return Option.Some(curTyp);
        if (!curTyp.IsGenericType)
        {
            if (isWrappedInOption)
                return Option.Some(curTyp);
            return Option.None<Type>();
        }
        var typeArgs = curTyp.GenericTypeArguments;
        if (typeArgs.Length != 1 || typeArgs[0].IsNullable())
        {
            if (isWrappedInOption)
                return Option.Some(curTyp);
            return Option.None<Type>();
        }
        var underlyTyp = typeArgs[0];
        var typedOption = TypeGenericOption.MakeGenericType(typeArgs);
        if (!curTyp.IsAssignableTo(typedOption))
        {
            if (isWrappedInOption)
                return Option.Some(curTyp);
            return Option.None<Type>();
        }
        if (recurseOn)
            return underlyTyp.FindUnderlyingTypeOfOption(recurseOn, isWrappedInOption: true);
        return Option.Some(underlyTyp);
    }


    internal static Option<TVal> GetOption<TKey, TVal>(this IReadOnlyDictionary<TKey, TVal> inp, TKey key) where TKey : notnull =>
        inp.ContainsKey(key) ? Option.From(inp[key]) : Option.None<TVal>();




}
