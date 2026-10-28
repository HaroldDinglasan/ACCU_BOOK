using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.DeliveryOrderModels;
using Accounting_V4.Model.QNEModels.MaintenanceModels;
using Accounting_V4.Model.UserModels;
using Accounting_V4.Services.AuthenServices;
using Accounting_V4.Services.MenuServices;
using Accounting_V4.Services.Security;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.UserServices;

public class UserData : IUserData
{
    //private readonly QneConnectionString qneConnection;
    private readonly ISqlDataAccess db;
    private readonly IDevExpressPasswordService _passwords;
   // private readonly AppConnectionString appConnectionString;
    private readonly QneConnectionString qneConnection;
    public UserData(ISqlDataAccess db, AppConnectionString appConnectionString, IDevExpressPasswordService passwords, QneConnectionString qneConnection)
    {
        this.db = db;
        _passwords = passwords;
        this.qneConnection = qneConnection;
    }
    public async Task<List<UserModel>> GetUsers()
    {
        const string sql = @"SELECT UserAccount.*,Roles.* FROM UserAccount JOIN UserRoles on UserAccount.Id = UserRoles.UserId JOIN Roles on UserRoles.RoleId = Roles.RoleId";

        return await db.LoadData<UserModel, dynamic>(sql, new { });
    }

    public async Task<UserModel> GetUserById(int UserId)
    {
        const string sql = @"SELECT UserAccount.*,Roles.* FROM UserAccount JOIN UserRoles on UserAccount.Id = UserRoles.UserId JOIN Roles on UserRoles.RoleId = Roles.RoleId WHERE UserAccount.Id = @UserId";
        return await db.LoadSingleData<UserModel>(sql, new { UserId });
    }
    public async Task<List<UserModel>> GetUserByRole(string RoleName, string AccessType)
    {
        const string sql = @"SELECT UserAccount.*,Roles.* FROM UserAccount JOIN UserRoles on UserAccount.Id = UserRoles.UserId JOIN Roles on UserRoles.RoleId = Roles.RoleId
                                WHERE Roles.RoleName = @RoleName AND AccessType = @AccessType";

        return await db.LoadData<UserModel, dynamic>(sql, new { RoleName, AccessType });
    }

    //Log in Query
    //public async Task<AuthenticatedUserAccount> LoginUser(UserModel userModel)
    //{
    //    string sql = @"SELECT UserAccount.Id, username, RoleName, Name, Password, UserAccount.AccessType, UserAccount.IsPasswordDefault 
    //                      FROM UserAccount
    //                      JOIN UserRoles ON UserAccount.Id = UserRoles.UserId
    //                      JOIN Roles ON UserRoles.RoleId = Roles.RoleId
    //                      WHERE IsActive = 'True' AND UserAccount.Username = @Username AND UserAccount.Password = @Password";

    //    var model = await db.LoadSingleData<AuthenticatedUserAccount>(sql, new { userModel.Username, userModel.Password });

    //    if (model != null)
    //    {
    //        return model;
    //    }
    //    return null;
    //}


    public async Task<AuthenticatedUserAccount> LoginUser(UserModel userModel)
    {
        if (userModel == null ||
            string.IsNullOrWhiteSpace(userModel.UserName) ||
            string.IsNullOrEmpty(userModel.StoredPassword))
            return null;

        // Username lang ang sa WHERE. Wala na ang StoredPassword filter.
        string sql = @"SELECT SU.Oid,
                              StoredPassword,
                              ChangePasswordOnFirstLogon,
                              UserName,
                              IsActive,
                              FullName,
                              Remarks,
                              Picture,
                              Signature,
                              Email,
                              RoleType,
                              SR.Name AS RoleName,
                              SSR.RolesId AS RoleId
                       FROM SystemUser SU
                       LEFT JOIN SystemUser_SystemRoles SSR ON SSR.Users = SU.Oid
                       LEFT JOIN SystemRole SR ON SR.Oid = SSR.RolesId
                       WHERE SU.IsActive = 1 AND UserName = @UserName";

        var model = await db.LoadSingleData<AuthenticatedUserAccount>(
            sql, new
            {
                userModel.UserName
            });

        if (model == null)
            return null;

        // I-verify ang plain password laban sa hash sa DB
        if (!_passwords.Verify(userModel.StoredPassword, model.StoredPassword))
            return null;

        // Huwag nang dalhin ang hash sa ibang parte ng app
        model.StoredPassword = null;

        return model;
    }
    public async Task<AuthenticatedUserAccount> LoginUserOLDDD(UserModel userModel)
    {
        string sql = @"SELECT SU.Oid,
                StoredPassword,
                 ChangePasswordOnFirstLogon,
                 UserName,
                 IsActive,
                 FullName,
                 Remarks,
                 Picture,
                 Signature,
                 Email,
                 RoleType,
        SR.Name as RoleName,
              SSR.RolesId as RoleId
              FROM SystemUser SU
             LEFT JOIN SystemUser_SystemRoles SSR
              ON  SSR.Users = SU.Oid
        LEFT JOIN SystemRole SR
        ON SR.Oid = SSR.RolesId
              WHERE SU.IsActive = 1 AND UserName = @UserName AND StoredPassword = @StoredPassword";



        var model = await db.LoadSingleData<AuthenticatedUserAccount>(sql, new { userModel.UserName, userModel.StoredPassword });

        if (model != null)
        {
            return model;
        }
        return null;
    }




    ///QNE DATABASE SECURITYSYSTEMUSER
    ///




}
