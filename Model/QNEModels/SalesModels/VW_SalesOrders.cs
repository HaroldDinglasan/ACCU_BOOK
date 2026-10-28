namespace Accounting_V4.Model.QNEModels.SalesModels;

public class VW_SalesOrders
{
    public Guid Id
    {
        get; set;
    }
    public DateTime? SalesOrderDate
    {
        get; set;
    }
    public string? SalesOrderCode
    {
        get; set;
    }
    public Guid? DebtorId
    {
        get; set;
    }
    public string? DebtorName
    {
        get; set;
    }
    public string? Address1
    {
        get; set;
    }
    public string? CustomerCode
    {
        get; set;
    }
    public string? CustomerName
    {
        get; set;
    }
    public string? CustomerName2
    {
        get; set;
    }
    public string? Term
    {
        get; set;
    }
    public string? CurrencyCode
    {
        get; set;
    }
    public string? StaffCode
    {
        get; set;
    }
    public int? CurrencyId
    {
        get; set;
    }
    public decimal? CurrencyRate
    {
        get; set;
    }
    public decimal TaxTotalAmount
    {
        get; set;
    }
    public decimal TaxTotalAmountLocal
    {
        get; set;
    }
    public decimal TotalAmount
    {
        get; set;
    }
    public decimal NetTotalAmountLocal
    {
        get; set;
    }
    public decimal WTaxTotalAmount
    {
        get; set;
    }
    public decimal WTaxTotalAmountLocal
    {
        get; set;
    }
    public string? Attention
    {
        get; set;
    }
    public string? ReferenceNo
    {
        get; set;
    }
    public int? TermId
    {
        get; set;
    }
    public int? SalesPersonId
    {
        get; set;
    }
    public string? DeliveryTerm
    {
        get; set;
    }
    public int? ProjectId
    {
        get; set;
    }
    public int? DepartmentId
    {
        get; set;
    }
    public Guid? StockLocationId
    {
        get; set;
    }
    public bool IsCancelled
    {
        get; set;
    }
    public DateTime? RequireDate
    {
        get; set;
    }
    public decimal? Discount
    {
        get; set;
    }
    public decimal? DiscountAmount
    {
        get; set;
    }
    public decimal NetTotalAmount
    {
        get; set;
    }
    public decimal? DiscountAmountLocal
    {
        get; set;
    }
    public decimal? TotalAmountLocal
    {
        get; set;
    }
    public bool IsClosed
    {
        get; set;
    }
    public decimal? SubtotalAmount
    {
        get; set;
    }
    public decimal? SubtotalAmountLocal
    {
        get; set;
    }
    public string? Notes
    {
        get; set;
    }
    public bool? IsTaxInclusive
    {
        get; set;
    }
    public DateTime? TaxDate
    {
        get; set;
    }
    public decimal? TaxExclusiveTotalAmount
    {
        get; set;
    }
    public decimal? TaxExclusiveTotalAmountLocal
    {
        get; set;
    }
    public bool? IsTaxInclusiveOnly
    {
        get; set;
    }
    public string? DORegistationNo
    {
        get; set;
    }
    public string? TransferFromInfo
    {
        get; set;
    }
    public string? TransferToInfo
    {
        get; set;
    }
    public bool? IsApproved
    {
        get; set;
    }
    public string? SIRef1
    {
        get; set;
    }
    public string? SIRef2
    {
        get; set;
    }
    public string? AreaCode
    {
        get; set;
    }
    public string? CategoryCode
    {
        get; set;
    }
    public string? ControlAccountCode
    {
        get; set;
    }
}
