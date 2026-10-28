using Microsoft.AspNetCore.Components;
using Radzen;

namespace Accounting_V4.Services.DialogServices;

public class SidePanelService
{

    public event Action OnChange;
    public event Action OnClose;

    // Ginawang RenderFragment<SidePanelService> para suportahan ang 'ds => ...' syntax
    public RenderFragment<SidePanelService> Content
    {
        get; private set;
    }
    public string Title
    {
        get; private set;
    }
    public string Width { get; private set; } = "400px";
    public bool IsVisible
    {
        get; private set;
    }

  
    public bool ShowMask
    {
        get; private set;
    }

    private TaskCompletionSource<object> _taskCompletionSource;

    /// <summary>
    /// Bubukas ang side panel na may dynamic content.
    /// </summary>
    /// <param name="title">Ang pamagat ng panel.</param>
    /// <param name="content">Ang UI content (e.g., ds => @<RadzenStack>...</RadzenStack>).</param>
    /// <param name="options">Opsyonal na configuration tulad ng Width.</param>
    public Task<dynamic> OpenSideAsync(string title, RenderFragment<SidePanelService> content, SidePanelOptions options)
    {
      
        Title = title;
        Content = content;
        Width = options?.Width ?? "400px";

        IsVisible = true;

        _taskCompletionSource = new TaskCompletionSource<object>();

        NotifyStateChanged();

        return _taskCompletionSource.Task;
    }

    /// <summary>
    /// Isasara ang panel at ibabalik ang resulta sa tumawag ng OpenSideAsync.
    /// </summary>
    public void Close(object result = null)
    {
        IsVisible = false;
        _taskCompletionSource?.TrySetResult(result);
        NotifyStateChanged();
        OnClose?.Invoke();
    }


    private void NotifyStateChanged() => OnChange?.Invoke();
}

public class SidePanelOptions
{
    public string Title
    {
        get; set;
    }
    public RenderFragment Content
    {
        get; set;
    }
    public string Width
    {
        get; set;
    }

}
