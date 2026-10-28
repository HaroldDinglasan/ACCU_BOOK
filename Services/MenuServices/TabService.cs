using Accounting_V4.Model.MenuModels;

namespace Accounting_V4.Services.MenuServices;

public class TabService
{
    public event Action<TabInfo>? OnOpenPage;
    public event Action<TabInfo>? OnClosePage;

    public void OpenPage(string pageName, string title, string icon = "description", Dictionary<string, object>? parameters = null)
    {
        var pageType = PageResolver.ResolvePageType(pageName);
        if (pageType == null)
        {
            Console.WriteLine($"⚠ Page not found: {pageName}");
            return;
        }

        var tab = new TabInfo
        {
            tabId = Guid.NewGuid().ToString(),
            title = title,
            icon = icon,
            componentType = pageType,
            parameters = parameters ?? new Dictionary<string, object>()
        };

        OnOpenPage?.Invoke(tab);
    }

    public void ClosePage(TabInfo tab) => OnClosePage?.Invoke(tab);
}

