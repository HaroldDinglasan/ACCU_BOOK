using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.ABMaintenance;
using Accounting_V4.Model.MenuModels;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.MaintenanceModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.UserModels;
using Accounting_V4.Services.Security;
using Accounting_V4.Services.SystemServices.TimeZoneServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.MaintenanceServices;

public class SystemUserServices
{
    private readonly QneConnectionString qneConnection;
    private readonly string accubookConnection;

    public SystemUserServices(QneConnectionString qneConnection, IConfiguration config)
    {
        this.qneConnection = qneConnection;
        this.accubookConnection = config.GetConnectionString("DefaultConnectionString");
      
    }



    ///ACCUBOOK DATA

    public async Task<IEnumerable<SystemUser>> GetSystemUserLists()
    {
        //var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(accubookConnection);
        var query = @"SELECT * FROM SystemUser WHERE GCRecord IS NULL ORDER BY UserName ASC";
        var model = await connection.QueryAsync<SystemUser>(query, commandType: CommandType.Text);
        return model;
    }


    public async Task<SystemUser> GetSystemUserById(string Id)
    {
        //var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(accubookConnection);
        var query = @"SELECT * FROM SystemUser WHERE Oid LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<SystemUser>(query, new { Id });
        return model;
    }

    public async Task<IEnumerable<SystemRole>> GetSystemRoleById(string Id)
    {
        // var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(accubookConnection);
        var query = @"SELECT SR.Oid
        ,SSR.Users
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
  FROM SystemRole SR
  INNER JOIN SystemUser_SystemRoles SSR
  ON SSR.RolesId = SR.Oid
  WHERE GCRecord IS NULL
   AND SSR.Users LIKE @Id
    ORDER BY Name";
        var model = await connection.QueryAsync<SystemRole>(query, new { Id });
        return model;

    }





    public async Task<string> UpdateUserPassword(SystemUser userModel)
    {

        var connection = new SqlConnection(accubookConnection);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var sql = @"UPDATE SystemUser SET StoredPassword = @StoredPassword, Optimisticlockfield = Optimisticlockfield + 1, ChangePasswordOnFirstLogon = 0 WHERE Oid = @Oid AND StoredPassword = @OldStoredPassword";

        try
        {
            var data = await connection.ExecuteAsync(sql,
                new
                {
                    Oid = userModel.Oid,
                    StoredPassword = userModel.StoredPassword,
                    OldStoredPassword = userModel.OldStoredPassword
                }
                , transaction);
            transaction.Commit();
            return $"{data.ToString()}";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }


    }





    /// <summary>
    /// QNE DATA
    /// </summary>
    /// <returns></returns>

    public async Task<IEnumerable<SecuritySystemUser>> GetSecuritySystemUserLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT 
               Oid
              ,StoredPassword
              ,ChangePasswordOnFirstLogon
              ,UserName
              ,IsActive
              ,OptimisticLockField
              ,GCRecord
              ,ObjectType
              ,FullName
              ,Remarks
              ,Picture
              ,Signature
              ,Email
              ,ApiToken
        FROM SecuritySystemUser WHERE GCRecord IS NULL";
        var model = await connection.QueryAsync<SecuritySystemUser>(query, commandType: CommandType.Text);
        return model;
    }


    public async Task<IEnumerable<SecuritySystemUser>> GetSystemUserSettings()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT Oid
                    ,StoredPassword
                    ,ChangePasswordOnFirstLogon
                    ,UserName
                    ,IsActive
                    ,OptimisticLockField
                    ,GCRecord
                    ,ObjectType
                    ,FullName
                    ,Remarks
                    ,Picture
                    ,Signature
                    ,Email
                    ,ApiToken
                FROM SecuritySystemUser
                ORDER BY UserName ASC";
        var model = await connection.QueryAsync<SecuritySystemUser>(query, commandType: CommandType.Text);
        return model;

    }


    public async Task<SecuritySystemUser> GetSecuritySystemUserById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM SecuritySystemUser WHERE Oid LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<SecuritySystemUser>(query, new { Id });
        return model;
    }


    public async Task<IEnumerable<SecuritySystemRole>> GetSecuritySystemRoleById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT S.Oid
        ,SSR.Users
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
  FROM SecuritySystemRole S
  INNER JOIN SecuritySystemUserUsers_SecuritySystemRoleRoles SSR
  ON SSR.Roles = S.Oid
  WHERE GCRecord IS NULL
  AND SSR.Users = @Id
    ORDER BY Name";
        var model = await connection.QueryAsync<SecuritySystemRole>(query, new { Id });
        return model;

    }


    public async Task<string> SaveSystemUser(
    SystemUser user,
    List<SystemUser_SystemRoles> systemUserSystemRoles)
    {
        try
        {

            using var connection = new SqlConnection(accubookConnection);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            // Bagong user = bagong Oid at DateCreated
            bool isNew = user.Oid == Guid.Empty;
            if (isNew)
            {
                user.Oid = Guid.NewGuid();
                user.DateCreated = DateTime.Now;
            }

            // ---------- UPSERT SystemUser ----------
            // Hindi ginagalaw ang StoredPassword, DateCreated, Picture, Signature sa UPDATE.
            const string upsertUserQuery = @"
        IF EXISTS (SELECT 1 FROM SystemUser WHERE Oid = @Oid)
        BEGIN
            UPDATE SystemUser
            SET ChangePasswordOnFirstLogon = @ChangePasswordOnFirstLogon,
                UserName                   = @UserName,
                IsActive                   = @IsActive,
                FullName                   = @FullName,
                Remarks                    = @Remarks,
                Email                      = @Email,
                RoleType                   = @RoleType,
                OptimisticLockField        = ISNULL(OptimisticLockField, 0) + 1
            WHERE Oid = @Oid;
        END
        ELSE
        BEGIN
            INSERT INTO SystemUser
            (
                Oid, StoredPassword, ChangePasswordOnFirstLogon, UserName, IsActive,
                OptimisticLockField, FullName, Remarks, Picture, Signature,
                Email, RoleType, GCRecord, DateCreated
            )
            VALUES
            (
                @Oid, @StoredPassword, @ChangePasswordOnFirstLogon, @UserName, @IsActive,
                0, @FullName, @Remarks, @Picture, @Signature,
                @Email, @RoleType, NULL, @DateCreated
            );
        END";

            await connection.ExecuteAsync(upsertUserQuery, new
            {
                user.Oid,
                user.StoredPassword,
                user.ChangePasswordOnFirstLogon,
                user.UserName,
                user.IsActive,
                user.FullName,
                user.Remarks,
                user.Picture,
                user.Signature,
                user.Email,
                user.RoleType,
                DateCreated = user.DateCreated == default ? TimeService.ServerTime : user.DateCreated
            }, transaction);

            // ---------- ROLES ----------
            var roles = systemUserSystemRoles ?? new List<SystemUser_SystemRoles>();
            var roleIds = roles.Select(x => x.RolesId).Distinct().ToList();

            // DELETE lang yung wala na sa UI
            if (roleIds.Count > 0)
            {
                await connection.ExecuteAsync(@"
                DELETE FROM SystemUser_SystemRoles
                WHERE Users = @Users AND RolesId NOT IN @RolesIds;",
                    new
                    {
                        Users = user.Oid,
                        RolesIds = roleIds
                    }, transaction);
            }
            else
            {
                await connection.ExecuteAsync(@"
                DELETE FROM SystemUser_SystemRoles WHERE Users = @Users;",
                    new
                    {
                        Users = user.Oid
                    }, transaction);
            }

            // INSERT lang yung wala pa sa DB (existing = skip)
            const string insertRoleQuery = @"
        IF NOT EXISTS (SELECT 1 FROM SystemUser_SystemRoles
                       WHERE Users = @Users AND RolesId = @RolesId)
        BEGIN
            INSERT INTO SystemUser_SystemRoles (Oid, Users, RolesId, OptimisticLockField)
            VALUES (@Oid, @Users, @RolesId, @OptimisticLockField);
        END";

            foreach (var r in roles.GroupBy(x => x.RolesId).Select(g => g.First()))
            {
                await connection.ExecuteAsync(insertRoleQuery, new
                {
                    Oid = r.Oid == Guid.Empty ? Guid.NewGuid() : r.Oid,
                    Users = user.Oid,
                    r.RolesId,
                    r.OptimisticLockField
                }, transaction);
            }

            transaction.Commit();
            return "System user saved successfully.";
        }
        catch (Exception ex)
        {
            // Auto-rollback kapag hindi na-commit ang transaction (dahil sa using)
            return $"Failed to save system user: {ex.Message}";
        }
    }

    public async Task<string> SaveSystemUserSystemRoles(Guid userId, List<SystemUser_SystemRoles> systemUserSystemRoles)
    {
        try
        {
            using var connection = new SqlConnection(accubookConnection);

            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            // DELETE roles na tinanggal sa UI
            if (systemUserSystemRoles != null &&
                systemUserSystemRoles.Count > 0)
            {
                var roleIds = systemUserSystemRoles
                    .Select(x => x.RolesId)
                    .Distinct()
                    .ToList();

                const string deleteQuery = @"
                DELETE FROM SystemUser_SystemRoles
                WHERE Users = @Users
                  AND RolesId NOT IN @RolesIds;
            ";

                await connection.ExecuteAsync(
                    deleteQuery,
                    new
                    {
                        Users = userId,
                        RolesIds = roleIds
                    },
                    transaction);
            }
            else
            {
                // Walang role sa UI = delete lahat
                const string deleteAllQuery = @"
                DELETE FROM SystemUser_SystemRoles
                WHERE Users = @Users;
            ";

                await connection.ExecuteAsync(
                    deleteAllQuery,
                    new
                    {
                        Users = userId
                    },
                    transaction);
            }

            // INSERT new roles
            if (systemUserSystemRoles != null)
            {
                const string insertQuery = @"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM SystemUser_SystemRoles
                    WHERE Users = @Users
                      AND RolesId = @RolesId
                )
                BEGIN
                    INSERT INTO SystemUser_SystemRoles
                    (
                        Oid,
                        Users,
                        RolesId,
                        OptimisticLockField
                    )
                    VALUES
                    (
                        @Oid,
                        @Users,
                        @RolesId,
                        @OptimisticLockField
                    );
                END
            ";

                foreach (var userRole in systemUserSystemRoles)
                {
                    await connection.ExecuteAsync(
                        insertQuery,
                        new
                        {
                            Oid = userRole.Oid == Guid.Empty
                                ? Guid.NewGuid()
                                : userRole.Oid,

                            Users = userId,

                            RolesId = userRole.RolesId,

                            OptimisticLockField =
                                userRole.OptimisticLockField
                        },
                        transaction);
                }
            }

            transaction.Commit();

            return "System user roles saved successfully.";
        }
        catch (Exception ex)
        {
            return $"Failed to save system user roles: {ex.Message}";
        }
    }



    //public async Task<string> SaveSystemUserSystemRoles(
    //  List<SystemUser_SystemRoles> systemUserSystemRoles)
    //{
    //    if (systemUserSystemRoles == null || systemUserSystemRoles.Count == 0)
    //        return "No system user roles to save.";

    //    try
    //    {
    //        using var connection = new SqlConnection(accubookConnection);

    //        await connection.OpenAsync();

    //        using var transaction = connection.BeginTransaction();

    //        // Assumption:
    //        // lahat ng records sa list ay para sa same user
    //        var userId = systemUserSystemRoles
    //            .FirstOrDefault()?.Users;

    //        if (userId == null)
    //            return "User is required.";

    //        // Remove roles that are no longer selected
    //        const string deleteQuery = @"
    //        DELETE FROM SystemUser_SystemRoles
    //        WHERE Users = @Users
    //          AND RolesId NOT IN @RolesIds;
    //    ";

    //        var roleIds = systemUserSystemRoles
    //            .Select(x => x.RolesId)
    //            .Distinct()
    //            .ToList();

    //        await connection.ExecuteAsync(
    //            deleteQuery,
    //            new
    //            {
    //                Users = userId,
    //                RolesIds = roleIds
    //            },
    //            transaction
    //        );

    //        // Insert only roles that don't exist yet
    //        const string insertQuery = @"
    //        INSERT INTO SystemUser_SystemRoles
    //        (
    //            Oid,
    //            Users,
    //            RolesId,
    //            OptimisticLockField
    //        )
    //        SELECT
    //            @Oid,
    //            @Users,
    //            @RolesId,
    //            @OptimisticLockField
    //        WHERE NOT EXISTS
    //        (
    //            SELECT 1
    //            FROM SystemUser_SystemRoles
    //            WHERE Users = @Users
    //              AND RolesId = @RolesId
    //        );
    //    ";

    //        foreach (var userRole in systemUserSystemRoles)
    //        {
    //            await connection.ExecuteAsync(
    //                insertQuery,
    //                new
    //                {
    //                    Oid = userRole.Oid == Guid.Empty
    //                        ? Guid.NewGuid()
    //                        : userRole.Oid,

    //                    Users = userRole.Users,
    //                    RolesId = userRole.RolesId,

    //                    OptimisticLockField =
    //                        userRole.OptimisticLockField
    //                },
    //                transaction
    //            );
    //        }

    //        transaction.Commit();

    //        return "System User SystemRoles saved successfully.";
    //    }
    //    catch (Exception ex)
    //    {
    //        return $"Failed to save system user roles: {ex.Message}";
    //    }
    //}




    //public async Task<string> DeleteSystemUserSystemRole(
    //Guid userId,
    //Guid roleId)
    //{
    //    try
    //    {
    //        using var connection = new SqlConnection(accubookConnection);

    //        const string query = @"
    //        DELETE FROM SystemUser_SystemRoles
    //        WHERE Users = @Users
    //          AND RolesId = @RolesId;
    //    ";

    //        await connection.ExecuteAsync(query, new
    //        {
    //            Users = userId,
    //            RolesId = roleId
    //        });

    //        return "System user role deleted successfully.";
    //    }
    //    catch (Exception ex)
    //    {
    //        return $"Failed to delete system user role: {ex.Message}";
    //    }
    //}


    //public async Task<string> DeleteSystemUserSystemRoles(Guid userId)
    //{
    //    try
    //    {
    //        using var connection = new SqlConnection(accubookConnection);

    //        const string query = @"
    //        DELETE FROM SystemUser_SystemRoles
    //        WHERE Users = @Users;
    //    ";

    //        await connection.ExecuteAsync(query, new
    //        {
    //            Users = userId
    //        });

    //        return "System user roles deleted successfully.";
    //    }
    //    catch (Exception ex)
    //    {
    //        return $"Failed to delete system user roles: {ex.Message}";
    //    }
    //}
}
