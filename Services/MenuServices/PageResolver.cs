using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Accounting_V4.Services.MenuServices;

public static class PageResolver
{
    private static readonly Assembly _assembly = typeof(PageResolver).Assembly;

    /// <summary>
    /// Finds a Razor Component type by its class name or @page route.
    /// Example: "GLAccountNew", "GLAccount/Edit/123", "overview"
    /// </summary>
    public static Type? ResolvePageType(string pageName)
    {
        if (string.IsNullOrWhiteSpace(pageName))
            return null;

        // Normalize input (remove spaces, slashes, lowercase)
        var normalized = pageName.Replace("/", "").Replace(".", "").Replace(" ", "").ToLower();

        return _assembly.GetTypes()
            .FirstOrDefault(t =>
                typeof(IComponent).IsAssignableFrom(t) &&
                (t.Name.Replace(".", "").Replace(" ", "").ToLower() == normalized ||
                 t.FullName?.Replace(".", "").Replace(" ", "").ToLower().EndsWith(normalized) == true));
    }
    //public static Type ResolvePageType(string routeOrName)
    //{
    //    if (string.IsNullOrWhiteSpace(routeOrName))
    //        return typeof(NotFound); // fallback

    //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();

    //    // 1. Try to match against route (@page "/overview")
    //    var routeMatch = assemblies
    //        .SelectMany(a => a.GetTypes())
    //        .FirstOrDefault(t =>
    //            typeof(IComponent).IsAssignableFrom(t) &&
    //            t.GetCustomAttributes(typeof(RouteAttribute), true)
    //             .Cast<RouteAttribute>()
    //             .Any(r => string.Equals(r.Template.TrimStart('/'),
    //                                      routeOrName.TrimStart('/'),
    //                                      StringComparison.OrdinalIgnoreCase)));

    //    if (routeMatch != null)
    //        return routeMatch;

    //    // 2. Fallback: match against component class name
    //    var nameMatch = assemblies
    //        .SelectMany(a => a.GetTypes())
    //        .FirstOrDefault(t =>
    //            typeof(IComponent).IsAssignableFrom(t) &&
    //            t.Name.Equals(routeOrName, StringComparison.OrdinalIgnoreCase));

    //    return nameMatch ?? typeof(NotFound);
    //}
}
