using System.Security.Cryptography;

namespace Accounting_V4.Model.ABMaintenance;

public class SystemRole
{
    public Guid Oid
    {
        get; set;
    } = Guid.NewGuid();

    public int OptimisticLockField
    {
        get; set;
    } = 0;

    public int? GCRecord
    {
        get; set;
    }
    public int? ObjectType
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
    } = false;
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
    } = false;
    public bool ModifyLayout
    {
        get; set;
    } = false;

    public bool NeedChangePasswordPeriodically
    {
        get; set;
    } = false;
    public int ChangePasswordInDays
    {
        get; set;
    }
   

}
