using System.Reflection;

namespace Accounting_V4.Services.QNEServices;

public static class SqlHelper
{
    public static IEnumerable<PropertyInfo> GetScaffoldableProperties<T>()
    {
        return typeof(T).GetProperties()
            .Where(p => p.CanRead && p.CanWrite);
    }


}
