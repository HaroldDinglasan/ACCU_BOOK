namespace Accounting_V4.Model.MenuModels;

public class TabInfo
{
    public string tabId { get; set; } = string.Empty;
    public string title { get; set; } = string.Empty;
    public string icon { get; set; } = string.Empty;
    public Type? componentType
    {
        get; set;
    }
    public Dictionary<string, object> parameters { get; set; } = new();
}
