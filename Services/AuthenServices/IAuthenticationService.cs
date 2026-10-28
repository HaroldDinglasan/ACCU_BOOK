using Accounting_V4.Model.UserModels;

namespace Accounting_V4.Services.AuthenServices;

public interface IAuthenticationService
{
    Task<AuthenticatedUserModel> Login(UserModel userForAuthentication);
    Task LogOut();
}
