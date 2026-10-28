namespace Accounting_V4.Model.QNEModels.AllTaxCodeModels;

public class WTaxCodes
{
    public Guid Id
    {
        get; set;
    }
    public string Code
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public decimal Rate
    {
        get; set;
    }
    public string TaxRate
    {
        get; set;
    }
    public string Rates
    {
        get; set;
    }
    public Guid? PayableAccount
    {
        get; set;
    }
    public Guid? CreditableAccount
    {
        get; set;
    }
    public int OptimisticLockField { get; set; } = 0;
    public string Remark
    {
        get; set;
    }
    public bool IsActive { get; set; } = true;
    public bool IsWVat { get; set; } = false;
    public bool? IsSystemDefined
    {
        get; set;
    }
    public Guid? SelfBearWTaxAccount
    {
        get; set;
    }
}
