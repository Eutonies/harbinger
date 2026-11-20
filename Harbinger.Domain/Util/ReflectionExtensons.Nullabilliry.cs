using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Harbinger.Domain.Util;

public static partial class ReflectionExtensions
{
    public static bool IsNullable(this MemberInfo inf)
    {
        var membTyp = inf.MembersType();
        if (membTyp.IsValueType)
            return Nullable.GetUnderlyingType(membTyp) != null;
        var nullInf = inf.NullabilityContext();
        return nullInf?.Pipe(_ => _.WriteState == NullabilityState.Nullable || _.ReadState == NullabilityState.Nullable) ?? true;
    }

    public static bool IsNullable(this ParameterInfo inf)
    {
        if (inf.ParameterType.IsValueType)
            return Nullable.GetUnderlyingType(inf.ParameterType) != null;
        var cont = new NullabilityInfoContext();
        var nullInf = cont.Create(inf);
        return nullInf.Pipe(_ => _.WriteState == NullabilityState.Nullable || _.ReadState == NullabilityState.Nullable);
    }

    public static NullabilityInfo? NullabilityContext(this MemberInfo inf) => (new NullabilityInfoContext())
        .Pipe(cont => inf switch
        {
            PropertyInfo prop => cont.Create(prop),
            FieldInfo fld => cont.Create(fld),
            EventInfo ev => cont.Create(ev),
            _ => null
        });

    public static Type MembersType(this MemberInfo inf) => inf switch
    {
        PropertyInfo prop => prop.PropertyType,
        MethodInfo meth => meth.ReturnType,
        FieldInfo fld => fld.FieldType,
        Type typ => typ,
        EventInfo ev when ev.EventHandlerType == null => throw new Exception($"Know not how to get's the type of event: {ev.GetType().Name}"),
        EventInfo ev => ev.EventHandlerType!,
        _ => throw new Exception($"Make no mention of the type of: {inf.GetType()}")
    };

}
