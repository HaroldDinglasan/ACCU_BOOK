namespace Accounting_V4.Model.QNEModels.CurrenciesModels;

public class Currencies
{
    public int Id
    {
        get; set;
    }
    public string CurrencyCode
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public string Description2
    {
        get; set;
    }
    public string Sign
    {
        get; set;
    }
    public decimal DefaultPurchaseRate
    {
        get; set;
    }
    public float DefaultSalesRate
    {
        get; set;
    }
    public bool IsForeign { get; set; } = false;
    public string CentFormat
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public string ISOCurrencyCode
    {
        get; set;
    }
    public float DefaultPurchaseReverseRate
    {
        get; set;
    }
    public float DefaultSalesReverseRate
    {
        get; set;
    }
    public bool ReverseRate { get; set; } = false;
}
