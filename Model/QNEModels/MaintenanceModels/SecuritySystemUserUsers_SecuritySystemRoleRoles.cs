using Accounting_V4.Services.OptimisticLockFieldServices;

namespace Accounting_V4.Model.QNEModels.MaintenanceModels;

//DEPRICATE NO LONGER USED
public class SecuritySystemUserUsers_SecuritySystemRoleRoles
{
    public Guid OID
    {
        get; set;
    } = Guid.NewGuid();
    public Guid Roles
    {
        get; set;
    }
    public Guid Users
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    } = 0;


}
