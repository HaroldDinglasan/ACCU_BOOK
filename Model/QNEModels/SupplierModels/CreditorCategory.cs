namespace Accounting_V4.Model.QNEModels.SupplierModels;

public class CreditorCategory
{
    public int Id
    {
        get; set;
    }
    public string CategoryCode
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
