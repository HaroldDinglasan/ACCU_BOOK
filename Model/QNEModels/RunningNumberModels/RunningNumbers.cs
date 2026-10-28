using Accounting_V4.Services.SystemServices.TimeZoneServices;

namespace Accounting_V4.Model.QNEModels.RunningNumberModels;

public class RunningNumbers
{
    public string CompanyCode
    {
        get; set;
    }
    public string TableName
    {
        get; set;
    }
    public string ColumnName
    {
        get; set;
    }
    public string Format
    {
        get; set;
    }
    public string NextCode
    {
        get; set;
    }
    public DateTime DocDate { get; set; } = TimeService.ServerTime;
}
