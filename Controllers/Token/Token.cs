using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Accounting_V4.Model.UserModels;
using Accounting_V4.Services.AuthenServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Accounting_V4.Controllers.Token;

public class Token
{
    private readonly IConfiguration _config;
    private readonly IUserData userData;
    // private UserModel userForAuthentication { get; set; }
    public Token(IConfiguration configuration, IUserData userData)
    {
        _config = configuration;
        this.userData = userData;
    }

    public async Task<TokenResponse> PutToken(UserModel userForAuthentication)
    {
        // Isang beses lang: valid ang user at password kung may ibinalik na account
        var account = await userData.LoginUser(userForAuthentication);

        if (account == null)
        {
            return new TokenResponse { Success = false };
        }

        var token = GenerateToken(account);

        return new TokenResponse
        {
            Success = true,
            Access_Token = token.Access_Token,
            UserName = token.UserName
        };
    }




    private (string Access_Token, string UserName) GenerateToken(AuthenticatedUserAccount account)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, account.UserName),
            new Claim(ClaimTypes.GivenName, account.FullName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            new Claim(ClaimTypes.Role, account.RoleType ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, account.Oid.ToString()),
            new Claim("AccessName", account.RoleName ?? string.Empty),
            new Claim("RoleId", account.RoleId.ToString()),
            new Claim("ChangePasswordOnFirstLogon", account.ChangePasswordOnFirstLogon.ToString())
            //new Claim("DefaultParphase", account.StoredPassword.ToString())
        };

        //var token = new JwtSecurityToken(
        //    new JwtHeader(
        //        new SigningCredentials(
        //            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
        //            SecurityAlgorithms.HmacSha256)),
        //    new JwtPayload(claims));

        var token = new JwtSecurityToken(
    claims: claims,   // alisin ang Nbf/Exp sa claims list
    notBefore: DateTime.UtcNow,
    expires: DateTime.UtcNow.AddDays(1),
    signingCredentials: new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
        SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), account.UserName);
    }






    //private async Task<bool> IsValidUsernameAndPassword(string username, string password)
    //{
    //    var model = new UserModel
    //    {
    //        UserName = username,
    //        StoredPassword = password
    //    };

    //    var user = await userData.LoginUser(model);
    //    if (user == null)
    //    {
    //        return false;
    //    }
    //    return true;
    //}


    //public async Task<TokenResponse> PutTokenOLDDDDD(UserModel userForAuthentication)
    //{
    //    if (await IsValidUsernameAndPassword(userForAuthentication.UserName, userForAuthentication.StoredPassword))
    //    {
    //        var token = await GenerateToken(userForAuthentication.UserName, userForAuthentication.StoredPassword);

    //        return new TokenResponse
    //        {
    //            Success = true,
    //            Access_Token = token.Access_Token,
    //            UserName = token.UserName
    //        };
    //    }

    //    return new TokenResponse
    //    {
    //        Success = false
    //    };
    //}

    //private async Task<dynamic> GenerateTokenOLDDD(string UserName, string StoredPassword)
    //{
    //    var model = new UserModel
    //    {
    //        UserName = UserName,
    //        StoredPassword = StoredPassword
    //    };

    //    AuthenticatedUserAccount authenticatedUserAccount = await userData.LoginUser(model);

    //    var claims = new List<Claim>
    //        {
    //        new Claim(ClaimTypes.Name, model.UserName),
    //        new Claim(ClaimTypes.GivenName, authenticatedUserAccount.FullName),
    //        new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
    //        new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
    //        new Claim(ClaimTypes.Role, authenticatedUserAccount.RoleType),
    //        new Claim(ClaimTypes.NameIdentifier, authenticatedUserAccount.Oid.ToString()),
    //        new Claim("AccessName", authenticatedUserAccount.RoleName),
    //        new Claim("RoleId", authenticatedUserAccount.RoleId.ToString()),
    //        new Claim("ChangePasswordOnFirstLogon", authenticatedUserAccount.ChangePasswordOnFirstLogon.ToString())

    //    };


    //    var token = new JwtSecurityToken(
    //                    new JwtHeader(
    //                        new SigningCredentials(
    //                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])), SecurityAlgorithms.HmacSha256)),
    //                                new JwtPayload(claims));


    //    var output = new
    //    {
    //        Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
    //        UserName = authenticatedUserAccount.UserName
    //    };

    //    return output;
    //}
}
