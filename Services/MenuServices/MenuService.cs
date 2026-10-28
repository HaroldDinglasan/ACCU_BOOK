using Accounting_V4.Model;
using Accounting_V4.Model.MenuModels;

namespace Accounting_V4.Services.MenuServices;

public class MenuService
{
    private readonly ISqlDataAccess dataAccess;

    public MenuService(ISqlDataAccess _dataAccess)
    {
        dataAccess = _dataAccess;
    }

    public async Task<int> SaveMenuInfo(MenuInfo menuInfo)
    {
        const string query = @"
        IF @MenuId = 0
        BEGIN
            INSERT INTO Menu
            (
                ParentMenuId,
                PageName,
                MenuName,
                IconNames,
                IsActive
            )
            VALUES
            (
                @ParentMenuId,
                @PageName,
                @MenuName,
                @IconNames,
                @IsActive
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            UPDATE Menu
            SET
                ParentMenuId = @ParentMenuId,
                PageName = @PageName,
                MenuName = @MenuName,
                IconNames = @IconNames,
                IsActive = @IsActive
            WHERE MenuId = @MenuId;

            SELECT @MenuId;
        END";

        var parameters = new
        {
            menuInfo.MenuId,
            menuInfo.ParentMenuId,
            menuInfo.PageName,
            menuInfo.MenuName,
            menuInfo.IconNames,
            menuInfo.IsActive
        };

        return await dataAccess.SaveData(query, parameters );
    }

    public async Task<IEnumerable<MenuInfo>> GetMenuLists()
    {
        //const string query = @"SELECT * FROM Menu WHERE ParentMenuId != 0 ORDER BY MenuId ASC";
        const string query = @"SELECT * FROM Menu ORDER BY MenuId ASC";
        return await dataAccess.LoadData<MenuInfo, dynamic>(query, new { });
    }

    public async Task<IEnumerable<Company>> GetCompany()
    {
        const string query = @"SELECT * FROM Company";
        return await dataAccess.LoadData<Company, dynamic>(query, new { });
    }


    //FOR MENU ACESS BY USER ROLES



    public async Task<IEnumerable<SystemMenuObjectPermissionResult>> GetSystemMenuObjectById(string Users)
    {
        const string query = @"
    ;WITH Permitted AS (
        SELECT DISTINCT
               M.MenuId, M.ParentMenuId, M.PageName,
               M.MenuName, M.IconNames, M.IsActive
        FROM AccuBookDB.dbo.SystemMenuObjectPermission SMP
        INNER JOIN Menu M ON M.MenuId = SMP.MenuId
        INNER JOIN SystemUser_SystemRoles SSR ON SSR.RolesId = SMP.Owner
        WHERE M.IsActive = 1
          AND SMP.AllowNavigate = 1
          AND SSR.Users = @Users
    )
    -- Parent: naka-activate at may kahit isang naka-activate na child
    SELECT P.MenuId, P.ParentMenuId, P.PageName, P.MenuName, P.IconNames, P.IsActive
    FROM Permitted P
    WHERE P.ParentMenuId = 0
      AND EXISTS (SELECT 1 FROM Permitted C WHERE C.ParentMenuId = P.MenuId)

    UNION

    -- Child: naka-activate at ang parent niya ay naka-activate din
    SELECT C.MenuId, C.ParentMenuId, C.PageName, C.MenuName, C.IconNames, C.IsActive
    FROM Permitted C
    WHERE C.ParentMenuId <> 0
      AND EXISTS (SELECT 1 FROM Permitted P WHERE P.MenuId = C.ParentMenuId AND P.ParentMenuId = 0)";

        return await dataAccess.LoadData<SystemMenuObjectPermissionResult, dynamic>(query, new { Users });
    }
    public async Task<IEnumerable<SystemMenuObjectPermissionResult>> GetSystemMenuObjectById_OLD(string Users)
    {
        //const string query = @"SELECT * FROM Company WHERE CompanyId = @CompanyId";
        const string query = @"SELECT SMP.Oid
      ,M.MenuId
      ,M.ParentMenuId
      ,M.PageName
      ,M.MenuName
      ,M.IconNames
      ,M.IsActive
  FROM AccuBookDB.dbo.SystemMenuObjectPermission SMP
  LEFT JOIN Menu M ON M.MenuId = SMP.MenuId
  LEFT JOIN SystemUser_SystemRoles SSR ON SSR.RolesId = SMP.Owner
  LEFT JOIN SystemRole SR ON SR.Oid = SSR.RolesId
  WHERE M.IsActive = 1
  AND SSR.Users = @Users";
        const string queryold = @"SELECT  SSR.Oid
            ,Users
            ,RolesId
            ,SSR.OptimisticLockField
         ,SR.Name
         ,MenuId
         ,ParentMenuId
         ,PageName
         ,MenuName
        ,IconNames
        FROM SystemUser_SystemRoles SSR 
        LEFT OUTER JOIN SystemMenuObjectPermission SMOB
        ON SMOB.Owner = SSR.RolesId
        LEFT OUTER JOIN SystemRole SR
        ON SR.Oid = SSR.RolesId
        WHERE Users = @Users
        AND AllowNavigate = 1";
        return await dataAccess.LoadData<SystemMenuObjectPermissionResult, dynamic>(query, new { Users });
    }




    public async Task<IEnumerable<SystemMenuObjectPermission>> GetSystemMenuObjectPermissionId(string RoleId)
    {
        //const string query = @"SELECT * FROM Company WHERE CompanyId = @CompanyId";
        const string query = @"SELECT Oid,
       M.MenuId,
       M.ParentMenuId,
       M.PageName,
       M.MenuName,
       M.IconNames,
       M.IsActive,
       AllowNavigate,
       AllowRead,
       AllowWrite,
       AllowCreate,
       AllowDelete,
       AllowExport,
       Owner,
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
        FROM SystemMenuObjectPermission SMP
        LEFT JOIN Menu M ON M.MenuId = SMP.MenuId
       WHERE M.IsActive = 1
       AND Owner = @RoleId";
        return await dataAccess.LoadData<SystemMenuObjectPermission, dynamic>(query, new { RoleId });
    }
}
