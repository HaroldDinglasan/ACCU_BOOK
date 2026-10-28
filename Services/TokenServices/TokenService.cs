using System.Security.Cryptography;

namespace Accounting_V4.Services.TokenServices
{
    public class TokenService
    {
        public LoginToken GenerateToken(string username)
        {
            return new LoginToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), // random token
                Expiry = DateTime.UtcNow.AddDays(1), // 1-day duration
                Username = username
            };
        }

    }
}
