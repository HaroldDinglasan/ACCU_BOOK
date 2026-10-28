namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class SpecialAccounts
{
    public int Id
    {
        get; set;
    }
    public Guid AccountId
    {
        get; set;
    }
    public string SpecialAccountCode
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public string AccountCode
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public int RowIndex
    {
        get; set;
    }
}
