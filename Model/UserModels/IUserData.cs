using Accounting_V4.Services.AuthenServices;

namespace Accounting_V4.Model.UserModels;

public interface IUserData
{
    Task<List<UserModel>> GetUsers();
    //Task<int> InsertUser(UserModel userModel);
    Task<AuthenticatedUserAccount> LoginUser(UserModel userModel);
    //Task<List<Roles>> ListOfRoles();
    Task<UserModel> GetUserById(int UserId);
    //Task<int> UpdateUser(UserModel userModel);
    Task<List<UserModel>> GetUserByRole(string RoleName, string AccessType);

}
