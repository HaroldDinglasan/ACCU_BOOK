namespace Accounting_V4.Services.AppStateServices;

public class AppStateService
{
    public event Action OnChange;

    public void NotifyStateChanged() => OnChange?.Invoke();
}
