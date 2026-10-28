using Radzen;

namespace Accounting_V4.Services.StatusTableServices;

public static class StatusHelper
{
    public static (string Text, BadgeStyle Style) Get(bool isClosed, bool isCancelled, string transferInfo)
    {
        var list = new[]
        {
            isClosed ? "Closed" : null,
            isCancelled ? "Cancelled" : null
        }
        .Where(x => x != null)
        .ToList();

        if (list.Any())
            return ($"{string.Join(", ", list)}", BadgeStyle.Danger);

        if (!string.IsNullOrWhiteSpace(transferInfo))
            return ("Transferred", BadgeStyle.Success);

        return ("Pending", BadgeStyle.Info);
    }
}
