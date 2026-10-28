using Radzen;

namespace Accounting_V4.Services.MessageHandlerServices;

public class RequestMessageHandler
{
    NotificationService _notificationService;
    public bool isTrueFalse;
    public RequestMessageHandler(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public void isSucccess(bool isSuccess)
    {
        if (isSuccess == true)
        {
            CustomNotificationMessage.Message(_notificationService, true);
            isTrueFalse = true;
        }
        else
        {
            CustomNotificationMessage.Message(_notificationService, false);
            isTrueFalse = false;
        }
    }

}
