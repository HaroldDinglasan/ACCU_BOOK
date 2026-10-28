namespace Accounting_V4.Services.TokenServices
{
    public class LoginToken
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public string Username { get; set; }
    }
}
