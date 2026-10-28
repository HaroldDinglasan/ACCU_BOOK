using Radzen;

namespace Accounting_V4.Services.MessageHandlerServices;

public class CustomNotificationMessage
{
    public static void ShowMessage(NotificationService notificationService, NotificationSeverity severity, string summary, string details)
    {
        NotificationMessage message = new NotificationMessage { Severity = severity, Summary = summary, Detail = details, Duration = 3000, CloseOnClick = true, Payload = DateTime.Now };
        ShowNotification(notificationService, message);
    }

    public static void ShowNotification(NotificationService notificationService, NotificationMessage message)
    {
        notificationService.Notify(message);
    }

    public static void Message(NotificationService notificationService, bool Success)
    {

        if (Success == true)
        {
            ShowMessage(notificationService, NotificationSeverity.Success, "Success!", "Operation completed!");
        }
        else
        {
            ShowMessage(notificationService, NotificationSeverity.Error, "Error!", "Error occur while saving data");
        }

    }

    public static void AlertMessages(NotificationService notificationService, string alertType)
    {
        switch (alertType)
        {
            case "Success":
                ShowMessage(notificationService, NotificationSeverity.Success, "Success!", "Operation completed!");
                break;
            case "Error":
                ShowMessage(notificationService, NotificationSeverity.Error, "Error!", "Error occur while saving data");
                break;
        }
    }
}
