using Accounting_V4.Model.MenuModels;
using Microsoft.AspNetCore.Components;

namespace Accounting_V4.Services.MenuServices;

public class PermissionService
{
    private readonly MenuService menuService;
    private readonly NavigationManager navManager;

    public PermissionService(
      MenuService menuService,
      NavigationManager navManager)
    {
        this.menuService = menuService;
        this.navManager = navManager;
    }

    public async Task<SystemMenuObjectPermission?> GetPermissionAsync(
        string roleId,
        string menuName,
        bool autoRedirect = true)
    {
        var permissions =
            await menuService.GetSystemMenuObjectPermissionId(roleId);

        if (permissions is null)
        {
            if (autoRedirect)
                navManager.NavigateTo("/NotAuthorized");

            return null;
        }

        var result = permissions
            .FirstOrDefault(x => x.MenuName == menuName);

        if (result == null || result.AllowNavigate == false)
        {
            if (autoRedirect)
                navManager.NavigateTo("/NotAuthorized");

            return null;
        }

        return result;
    }
}
