using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.MenuModels;

public class MenuInfo
{
    [Key]
    public int MenuId
    {
        get; set;
    }
    public int ParentMenuId
    {
        get; set;
    }

    //Page Name Rooute: /Home
    public string PageName
    {
        get; set;
    }

    //Menu Name: Home
    public string MenuName
    {
        get; set;
    }

    //CSS IconName
    public string IconNames
    {
        get; set;
    }

    //Menu Active
    public bool IsActive
    {
        get; set;
    }
}
