namespace Accounting_V4.Services.TokenServices;

public class CompanyToken
{
    public string Token
    {
        get; set;
    }
    public DateTime Expiry
    {
        get; set;
    }
    public string CompanyName
    {
        get; set;
    }
}
