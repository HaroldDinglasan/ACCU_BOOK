using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Accounting_V4.Model.ABMaintenance;

public class SystemUser_SystemRoles
{
    [Key]
    public Guid Oid
    {
        get; set;
    } = Guid.NewGuid();
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
    } = 0;
}
