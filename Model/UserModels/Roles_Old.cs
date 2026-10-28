namespace Accounting_V4.Model.UserModels;

public class Roles_Old
{
    public Guid Oid
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public int GCRecord
    {
        get; set;
    }
    public int ObjectType
    {
        get; set;
    }
    public string Name
    {
        get; set;
    }
    public bool IsAdministrative
    {
        get; set;
    } = false;
    public bool CanEditModel
    {
        get; set;
    } = true;
    public string Description
    {
        get; set;
    }
    public string HiddenNavigationItems
    {
        get; set;
    }
    public bool AllowPrintControl
    {
        get; set;
    }
    public bool ModifyLayout
    {
        get; set;
    }
    public bool NeedChangePasswordPeriodically
    {
        get; set;
    }
    public int ChangePasswordInDays
    {
        get; set;
    }


    //public int RoleId
    //{
    //    get; set;
    //}
    //public string RoleName
    //{
    //    get; set;
    //}
}
