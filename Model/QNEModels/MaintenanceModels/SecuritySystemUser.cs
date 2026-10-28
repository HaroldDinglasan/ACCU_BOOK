namespace Accounting_V4.Model.QNEModels.MaintenanceModels;


//DEPRICATE NO LONGER USED
public class SecuritySystemUser
{
    public Guid Oid
    {
        get; set;
    } = Guid.NewGuid();

    //THIS ID IS FOR ID LISTING ONLY, NOT FOR DATABASE PRIMARY KEY

    public string? StoredPassword
    {
        get; set;
    }
    public bool ChangePasswordOnFirstLogon { get; set; } = false;
    public string UserName
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    } = true;
    public int OptimisticLockField { get; set; } = 0;
    public int? GCRecord
    {
        get; set;
    }
    public int ObjectType { get; set; } = 16;
    public string FullName
    {
        get; set;
    }
    public string? Remarks
    {
        get; set;
    }
    //public string Picture { get; set; }
    //public byte Signature { get; set; }
    public string? Email
    {
        get; set;
    }
    public string? ApiToken
    {
        get; set;
    }



    public string OiddAsString => Oid.ToString();
}