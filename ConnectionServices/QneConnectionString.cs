using Accounting_V4.Model.ConnectionModels;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.ConnectionServices;

public class QneConnectionString
{
    // INITIALIZED ON STARTUP.CS
    public static QneConnectionSettings Settings { get; set; } = new QneConnectionSettings();
    public static CompanyCodeString _companyCodes { get; set; } = new CompanyCodeString();

    private readonly ILocalStorageService localStorage;

    public QneConnectionString(ILocalStorageService _localStorage)
    {
        localStorage = _localStorage;
    }

    public async Task<string> GetConnectionStringAsync()
    {
        string companyCode = null;

        try
        {
            // safe JSInterop call
            companyCode = await localStorage.GetItemAsync<string>("selectedCompanyCode");
        }
        catch (Microsoft.JSInterop.JSDisconnectedException)
        {
            // circuit is already disposed, return fallback
            return Settings.NTBIConnection;
        }
        catch (Exception)
        {
            // any other error, still return fallback
            return Settings.NTBIConnection;
        }

        return companyCode switch
        {
            var code when code == _companyCodes.companyCode01 => Settings.NTBIConnection,
            var code when code == _companyCodes.companyCode02 => Settings.APHIConnection,
            var code when code == _companyCodes.companyCode03 => Settings.AVLIConnection,
            _ => Settings.NTBIConnection // fallback
        };
    }

    //FOR DISPLAY CONNECTION DATABASE ON FOOTER/ NIREMOVE YUNG IBANG DATA PARA SA LESS EXPOSURE NG DATABASE CONNECTION DETAILS LIKE, USERNAME AT PASSWORD
    public async Task<string> GetSanitizedConnectionStringAsync()
    {
        var fullCon = await GetConnectionStringAsync();
        var builder = new SqlConnectionStringBuilder(fullCon);

        //Rename info

        //builder["Initial Catalog"] = "DATABASE";


        // Remove sensitive info
        builder.Remove("Data Source");
        builder.Remove("User ID");
        builder.Remove("Password");
        builder.Remove("TrustServerCertificate");
        builder.Remove("Trusted_Connection");
        builder.Remove("MultipleActiveResultSets");

        return builder.ToString();
    }


}
