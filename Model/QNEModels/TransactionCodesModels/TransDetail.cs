namespace Accounting_V4.Model.QNEModels.TransactionCodesModels;

public interface TransDetail
{
    public bool IsTaxInclusive
    {
        get; set;
    }
    decimal Qty
    {
        get; set;
    }
    decimal UnitPrice
    {
        get; set;
    }
    string Discount
    {
        get; set;
    }
    decimal Amount
    {
        get; set;
    }
    decimal AmountLocal
    {
        get; set;
    }
    decimal DiscountAmount
    {
        get; set;
    }
    decimal DiscountAmountLocal
    {
        get; set;
    }
    decimal? TaxRate
    {
        get; set;
    }
    decimal TaxAmount
    {
        get; set;
    }
    decimal? TaxAmountLocal
    {
        get; set;
    }
    decimal? WTaxRate
    {
        get; set;
    }
    decimal WTaxAmount
    {
        get; set;
    }
    decimal WTaxAmountLocal
    {
        get; set;
    }
    decimal? TaxExclusiveAmount
    {
        get; set;
    }
    decimal? TaxExclusiveAmountLocal
    {
        get; set;
    }
    decimal NetAmount
    {
        get; set;
    }
    decimal NetAmountLocal
    {
        get; set;
    }
    public decimal SubAmount
    {
        get; set;
    }
    public decimal SubAmountLocal
    {
        get; set;
    }
}
