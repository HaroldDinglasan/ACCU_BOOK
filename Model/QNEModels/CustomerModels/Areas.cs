namespace Accounting_V4.Model.QNEModels.CustomerModels;

public class Areas
{
    public int Id
    {
        get; set;
    }
    public string AreaCode
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    }
    public int AreaType
    {
        get; set;
    }
    public bool IsDefault
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
}
