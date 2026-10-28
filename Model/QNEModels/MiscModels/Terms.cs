namespace Accounting_V4.Model.QNEModels.MiscModels;

public class Terms
{
    public int Id
    {
        get; set;
    }
    public string Term
    {
        get; set;
    }
    public int TermDays
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public int TermType { get; set; } = 0;
    public int TermMonths { get; set; } = 0;
    public bool IsDefault { get; set; } = false;
    public int OptimisticLockField { get; set; } = 0;
}
