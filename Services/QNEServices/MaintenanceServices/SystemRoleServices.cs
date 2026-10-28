using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.ABMaintenance;
using Accounting_V4.Model.MenuModels;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.MaintenanceModels;
using Accounting_V4.Pages.QNEPages.AccuBookMaintenance;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.MaintenanceServices;

public class SystemRoleServices
{
    private readonly QneConnectionString qneConnection;
    private readonly string accubookConnection;
    public CheckExistServices checkExistServices;
    public SystemRoleServices(QneConnectionString qneConnection, IConfiguration config, CheckExistServices checkExistServices)
    {
        this.qneConnection = qneConnection;
        this.accubookConnection = config.GetConnectionString("DefaultConnectionString");
        this.checkExistServices = checkExistServices;
    }



    ///AccuBook Data
    public async Task<SystemRole> GetSystemRoleById(string Id)
    {
        //var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(accubookConnection);
        var query = @"SELECT * FROM SystemRole WHERE Oid LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<SystemRole>(query, new { Id });
        return model;

    }

    ///AccuBook Data
    public async Task<IEnumerable<SystemRole>> GetSystemRoleLists()
    {
        //var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(accubookConnection);
        var query = @"SELECT * FROM SystemRole WHERE GCRecord IS NULL ORDER BY Name ASC";
        var model = await connection.QueryAsync<SystemRole>(query, commandType: CommandType.Text);
        return model;

    }



    ///QNE DATA//DEPRICATED
    public async Task<IEnumerable<SecuritySystemRole>> GetSecuritySystemRoleLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT  Oid
      ,OptimisticLockField
      ,GCRecord
      ,ObjectType
      ,Name
      ,IsAdministrative
      ,CanEditModel
      ,Description
      ,HiddenNavigationItems
      ,AllowPrintControl
      ,ModifyLayout
      ,NeedChangePasswordPeriodically
      ,ChangePasswordInDays
  FROM SecuritySystemRole WHERE GCRecord IS NULL ORDER BY Name ASC";
        var model = await connection.QueryAsync<SecuritySystemRole>(query, commandType: CommandType.Text);
        return model;

    }


    ///QNE DATA//DEPRICATED
    public async Task<SecuritySystemRole> GetSecuritySystemRoleById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM SecuritySystemRole WHERE Oid LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<SecuritySystemRole>(query, new { Id });
        return model;

    }





    public async Task<string> SaveSystemRole(SystemRole systemRole)
    {
        var connection = new SqlConnection(accubookConnection);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();


        var insertquery = @"
        INSERT INTO SystemRole
        (
            Oid,
            OptimisticLockField,
            GCRecord,
            ObjectType,
            Name,
            IsAdministrative,
            CanEditModel,
            Description,
            HiddenNavigationItems,
            AllowPrintControl,
            ModifyLayout,
            NeedChangePasswordPeriodically,
            ChangePasswordInDays
        )
        VALUES
        (
            @Oid,
            @OptimisticLockField,
            @GCRecord,
            @ObjectType,
            @Name,
            @IsAdministrative,
            @CanEditModel,
            @Description,
            @HiddenNavigationItems,
            @AllowPrintControl,
            @ModifyLayout,
            @NeedChangePasswordPeriodically,
            @ChangePasswordInDays
        );";

        var updatequery = @"
        UPDATE SystemRole
        SET
            OptimisticLockField = OptimisticLockField + 1,
            GCRecord = @GCRecord,
            ObjectType = @ObjectType,
            Name = @Name,
            IsAdministrative = @IsAdministrative,
            CanEditModel = @CanEditModel,
            Description = @Description,
            HiddenNavigationItems = @HiddenNavigationItems,
            AllowPrintControl = @AllowPrintControl,
            ModifyLayout = @ModifyLayout,
            NeedChangePasswordPeriodically = @NeedChangePasswordPeriodically,
            ChangePasswordInDays = @ChangePasswordInDays
        WHERE Oid = @Oid;";

        try
        {
            var exists = await connection.ExecuteScalarAsync<bool>(@"IF EXISTS (SELECT 1 FROM SystemRole WHERE Oid = @Oid) SELECT CAST(1 AS BIT); ELSE SELECT CAST(0 AS BIT);", new { Oid = systemRole.Oid }, transaction);
            if (!exists)
            {
                await connection.ExecuteAsync(insertquery, systemRole, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updatequery, systemRole, transaction);
            }
            transaction.Commit();
            return $"Success";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }



    protected CheckExistModel sidetailsCheckModel = new CheckExistModel();
    public async Task<string> SaveSystemMenuObjectPermission(
    List<SystemMenuObjectPermission> permissions)
    {
        if (permissions == null || permissions.Count == 0)
            return "No permissions to save.";

        try
        {
            var connection = new SqlConnection(accubookConnection);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            const string query = @"
IF EXISTS
(
    SELECT 1
    FROM SystemMenuObjectPermission
    WHERE Owner = @Owner
      AND MenuId = @MenuId
)
BEGIN
    UPDATE SystemMenuObjectPermission
    SET
        AllowNavigate = @AllowNavigate,
        AllowRead = @AllowRead,
        AllowWrite = @AllowWrite,
        AllowCreate = @AllowCreate,
        AllowDelete = @AllowDelete,
        AllowExport = @AllowExport,
        AllowPrint = @AllowPrint,
        AllowCreditControl = @AllowCreditControl,
        AllowChangeDocNumbering = @AllowChangeDocNumbering,
        AllowChangeUnitCost = @AllowChangeUnitCost,
        AllowNegativeStockControl = @AllowNegativeStockControl,
        AllowStockReorderLevelControl = @AllowStockReorderLevelControl,
        AllowFilter = @AllowFilter,
        AllowPreview = @AllowPreview,
        AllowInquiry = @AllowInquiry,
        AllowDesign = @AllowDesign,
        AllowSelectDocNumbering = @AllowSelectDocNumbering,
        AllowChangeUnitPrice = @AllowChangeUnitPrice,
        AllowEInvoiceValidation = @AllowEInvoiceValidation
    WHERE Owner = @Owner
      AND MenuId = @MenuId;
END
ELSE
BEGIN
    INSERT INTO SystemMenuObjectPermission
    (
        Owner,
        MenuId,
        AllowNavigate,
        AllowRead,
        AllowWrite,
        AllowCreate,
        AllowDelete,
        AllowExport,
        AllowPrint,
        AllowCreditControl,
        AllowChangeDocNumbering,
        AllowChangeUnitCost,
        AllowNegativeStockControl,
        AllowStockReorderLevelControl,
        AllowFilter,
        AllowPreview,
        AllowInquiry,
        AllowDesign,
        AllowSelectDocNumbering,
        AllowChangeUnitPrice,
        AllowEInvoiceValidation
    )
    VALUES
    (
        @Owner,
        @MenuId,
        @AllowNavigate,
        @AllowRead,
        @AllowWrite,
        @AllowCreate,
        @AllowDelete,
        @AllowExport,
        @AllowPrint,
        @AllowCreditControl,
        @AllowChangeDocNumbering,
        @AllowChangeUnitCost,
        @AllowNegativeStockControl,
        @AllowStockReorderLevelControl,
        @AllowFilter,
        @AllowPreview,
        @AllowInquiry,
        @AllowDesign,
        @AllowSelectDocNumbering,
        @AllowChangeUnitPrice,
        @AllowEInvoiceValidation
    );
END
";

            foreach (var permission in permissions)
            {
                await connection.ExecuteAsync(query, permission, transaction);
            }

            transaction.Commit();

            return "Permissions saved successfully.";
        }
        catch (Exception ex)
        {
            return $"Failed to save permissions: {ex.Message}";
        }
    }
}
