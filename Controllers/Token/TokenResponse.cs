namespace Accounting_V4.Controllers.Token;

public class TokenResponse
{
    public bool Success
    {
        get; set;
    }
    public string Access_Token
    {
        get; set;
    }
    public string UserName
    {
        get; set;
    }

    public string UserRole
    {
        get; set;
    }
}
