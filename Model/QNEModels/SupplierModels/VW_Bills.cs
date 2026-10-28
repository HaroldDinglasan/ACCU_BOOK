namespace Accounting_V4.Model.QNEModels.SupplierModels;

public class VW_Bills
{
    public Guid Id
    {
        get; set;
    } 
    public DateTime BillDate
    {
        get; set;
    }
    public string BillCode
    {
        get; set;
    }
    public string SupplierCode
    {
        get; set;
    }
    public string Address
    {
        get; set;
    }
    public string BillFrom
    {
        get; set;
    }

    public string ReferenceNo
    {
        get; set;
    }
    public string Term
    {
        get; set;
    }
    public DateTime DueDate
    {
        get; set;
    }
    public string PurchaserName
    {
        get; set;
    }
    public string PurchaserCode
    {
        get; set;
    }
    public string SupplierINV
    {
        get; set;
    }
    public string ProjectCode
    {
        get; set;
    }
    public string CurrencyCode
    {
        get; set;
    }
    public decimal TotalAmount
    {
        get; set;
    }
    public decimal TaxTotalAmount
    {
        get; set;
    }
    public decimal NetTotalAmount
    {
        get; set;
    }
    public bool IsCancelled
    {
        get; set;
    }
    public Guid? FromDocId
    {
        get; set;
    }

    public string Note
    {
        get; set;
    }

}
