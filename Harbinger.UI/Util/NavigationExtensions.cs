using System.Reflection;

using Harbinger.Domain.Util;

using Microsoft.AspNetCore.Components;

namespace Harbinger.UI.Util;

public static class NavigationExtensions
{
    public static Option<string> Path(this Type pageTyp)
    {
        var routeAttribute = pageTyp.GetCustomAttributes<RouteAttribute>()
            .FirstOption();
        var returnee = routeAttribute.Map(_ => _.Template);
        return returnee;

    }

}
