namespace Accounting_V4.Model.QNEModels.AllBasicModels;

public class HeaderSwitchModel
{
    public Guid? Id
    {
        get; set;
    }
    public bool IsDeferredVAT
    {
        get; set;
    }
    public bool IsDetailPosting
    {
        get; set;
    }
    public string TableName
    {
        get; set;
    }
    public string SwitchName
    {
        get; set;
    }
}
