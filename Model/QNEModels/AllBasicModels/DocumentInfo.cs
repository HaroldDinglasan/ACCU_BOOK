using Accounting_V4.Services.SystemServices.TimeZoneServices;

namespace Accounting_V4.Model.QNEModels.AllBasicModels;

public class DocumentInfo
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public string? DocumentId
    {
        get; set;
    }
    public string? DocType
    {
        get; set;
    }
    public DateTime FirstCreatedDate
    {
        get; set;
    } = TimeService.ServerTime;
    public string FirstCreatedUserId
    {
        get; set;
    } 
    public DateTime LastModifiedDate
    {
        get; set;
    } = TimeService.ServerTime;
    public string? LastModifiedUserId
    {
        get; set;
    }
    public int PrintCount
    {
        get; set;
    } = 0;
    public int OptimisticLockField
    {
        get; set;
    } = 0;
    public int EmailCount
    {
        get; set;
    } = 0;
}
