namespace Accounting_V4.ConnectionServices;

public class AppConnectionString
{
    public string Value
    {
        get; set;
    }
    public AppConnectionString(string value) => Value = value;
}
