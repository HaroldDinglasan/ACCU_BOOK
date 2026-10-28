using Radzen;

namespace Accounting_V4.Services.MessageHandlerServices;

public class NotificationHelper
{
    public static void ShowWarning(NotificationService notificationService, string summary, string detail)
    {
        notificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Summary = summary,
            Detail = detail,
            Duration = 4000, // optional,
            ShowProgress = true // optional

        });
    }

    public static void ShowSuccess(NotificationService notificationService, string summary, string detail)
    {
        notificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = summary,
            Detail = detail,
            Duration = 4000,
            ShowProgress = true // optional
        });
    }

    public static void ShowError(NotificationService notificationService, string summary, string detail)
    {
        notificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Error,
            Summary = summary,
            Detail = detail,
            Duration = 4000,
            ShowProgress = true // optional
        });
    }

    public static void ShowInfo(NotificationService notificationService, string summary, string detail)
    {
        notificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Info,
            Summary = summary,
            Detail = detail,
            Duration = 4000,
            ShowProgress = true // optional
        });
    }
}
