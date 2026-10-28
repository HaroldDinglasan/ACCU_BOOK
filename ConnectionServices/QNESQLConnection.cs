using Accounting_V4.Model;
using Accounting_V4.Model.ConnectionModels;
using Accounting_V4.Services.TokenServices;
using Blazored.LocalStorage;
using Microsoft.Extensions.Options;

namespace Accounting_V4.ConnectionServices;

public class QNESQLConnection
{
    //private readonly QneConnectionSettings _qneConnections;
    //private readonly CompanyCodeString _companyCodes;
    ////private readonly ILocalStorageService _localStorage;

    //public QNESQLConnection(
    //    IOptions<QneConnectionSettings> qneConnections,
    //    CompanyCodeString companyCodes,
    //    IHttpContextAccessor httpContextAccessor
    //    /*ILocalStorageService localStorage*/)
    //{
    //    _companyCodes = companyCodes;
    //    //_localStorage = localStorage;
    //    _qneConnections = qneConnections.Value;
    //}



    //public async Task<string> GetConnectionStringAsync()
    //{
    //    var companyCode = await _localStorage.GetItemAsync<Company>("companyCode");
    //    //var companyCode = _httpContextAccessor.HttpContext?.User?.FindFirst("CompanyCode")?.Value;

    //    if (companyCode != null)
    //        return _qneConnections.NTBIConnection; // fallback default

    //    return companyCode!.ToString() switch
    //    {
    //        var code when code == _companyCodes.companyCode01 => _qneConnections.NTBIConnection,
    //        var code when code == _companyCodes.companyCode02 => _qneConnections.APHIConnection,
    //        var code when code == _companyCodes.companyCode03 => _qneConnections.AVLIConnection,
    //        _ => throw new System.InvalidOperationException("NO QNE DATABASE CONNECTION SELECTED FOR COMPANY_CODE: " + companyCode)
    //    };
    //}
}
