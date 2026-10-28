using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Accounting_V4.Model.ABMaintenance;

public class SystemUser
{
    public Guid Oid
    {
        get; set;
    } = Guid.NewGuid();

    public string StoredPassword
    {
        get; set;
    } = "NTBIDEFAULT123";

    [Required(ErrorMessage = "OldPassword is Required")]
    public string OldStoredPassword
    {
        get; set;
    }
    public bool ChangePasswordOnFirstLogon
    {
        get; set;
    } = false;
    public string UserName
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    } = true;
    public int OptimisticLockField
    {
        get; set;
    } = 0;
    public string FullName
    {
        get; set;
    } = string.Empty;
    public string Remarks
    {
        get; set;
    } = string.Empty;
    public byte[]? Picture
    {
        get; set;
    }
    public byte[]? Signature
    {
        get; set;
    }
    public string? Email
    {
        get; set;
    }
    public string RoleType
    {
        get; set;
    }

    public int? GCRecord
    {
        get; set;
    }
    public DateTime? DateCreated
    {
        get; set;
    }

}
