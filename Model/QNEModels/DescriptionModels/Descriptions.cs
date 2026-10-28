namespace Accounting_V4.Model.QNEModels.DescriptionModels;

public class Descriptions
{
    public int Id
    {
        get; set;
    }
    public string OwnerCode
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public bool IsDefault { get; set; } = false;
    public int OptimisticLockField { get; set; } = 0;
}
