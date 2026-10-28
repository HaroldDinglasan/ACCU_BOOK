using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.MenuModels;

public class SystemMenuObjectPermissionResult
{
    [Key]
    public Guid Oid
    {
        get; set;
    }
    public Guid Users
    {
        get; set;
    }
    public Guid RolesId
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public string RoleName
    {
        get; set;
    }
    public int MenuId
    {
        get; set;
    }
    public int ParentMenuId
    {
        get; set;
    }
    public string PageName
    {
        get; set;
    }
    public string MenuName
    {
        get; set;
    }
    public string IconNames
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    } = true;
}
