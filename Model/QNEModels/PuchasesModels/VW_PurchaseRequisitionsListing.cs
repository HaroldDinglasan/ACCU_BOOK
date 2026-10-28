namespace Accounting_V4.Model.QNEModels.PuchasesModels;

public class VW_PurchaseRequisitionsListing
{
    public Guid Id
    {
        get; set;
    }
    public string PurchaseRequisitionCode
    {
        get; set;
    }
    public DateTime PurchaseRequisitionDate
    {
        get; set;
    }
    public string PurchaseTo
    {
        get; set;
    }
    public string Address
    {
        get; set;
    }
    public string Attention
    {
        get; set;
    }
    public string Phone
    {
        get; set;
    }
    public string Fax
    {
        get; set;
    }
    public string ReferenceNo
    {
        get; set;
    }
    public DateTime? RequireDate
    {
        get; set;
    }
    public string DeliveryTerm
    {
        get; set;
    }
    public string Title
    {
        get; set;
    }
    public string Ref1
    {
        get; set;
    }
    public string Ref2
    {
        get; set;
    }
    public string Remark1
    {
        get; set;
    }
    public string Remark2
    {
        get; set;
    }
    public string DeliverContact
    {
        get; set;
    }
    public string DeliverPhone
    {
        get; set;
    }
    public string DeliverFax
    {
        get; set;
    }
    public string DeliverAddress
    {
        get; set;
    }
    public decimal TotalAmount { get; set; } = 0.00M;
    public decimal NetTotalAmount { get; set; } = 0.00M;
    public decimal NetTotalAmountLocal { get; set; } = 0.00M;
    public bool IsCancelled
    {
        get; set;
    }
    public bool IsClosed
    {
        get; set;
    }
    public string CreditorCompanyCode
    {
        get; set;
    }
    public string CreditorCompanyName
    {
        get; set;
    }
    public string AreaCode
    {
        get; set;
    }
    public string AreaDescription
    {
        get; set;
    }
    public string CategoryCode
    {
        get; set;
    }
    public string CategoryDescription
    {
        get; set;
    }
    public string ControlAccountCode
    {
        get; set;
    }
    public string PurchaserCode
    {
        get; set;
    }
    public string PurchaserName
    {
        get; set;
    }
    public string TermCode
    {
        get; set;
    }
    public string TermDescription
    {
        get; set;
    }
    public string ProjectCode
    {
        get; set;
    }
    public string ProjectDescription
    {
        get; set;
    }
    public string DepartmentCode
    {
        get; set;
    }
    public string DepartmentDescription
    {
        get; set;
    }
    public string LocationCode
    {
        get; set;
    }
    public string StockLocationDescription
    {
        get; set;
    }
    public string CurrencyCode
    {
        get; set;
    }
    public string CurrencyDescription
    {
        get; set;
    }
    public string CurrencySign
    {
        get; set;
    }
    public decimal CurrencyRate
    {
        get; set;
    }
    public string RequestByUserName
    {
        get; set;
    }
    public string RequestByFullName
    {
        get; set;
    }
    public string RequestBySignature
    {
        get; set;
    }
    public string Address1
    {
        get; set;
    }
    public string Address2
    {
        get; set;
    }
    public string Address3
    {
        get; set;
    }
    public string Address4
    {
        get; set;
    }
    public string DeliverAddress1
    {
        get; set;
    }
    public string DeliverAddress2
    {
        get; set;
    }
    public string DeliverAddress3
    {
        get; set;
    }
    public string DeliverAddress4
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
    public decimal TaxExclusiveTotalAmount
    {
        get; set;
    }
    public decimal TaxExclusiveTotalAmountLocal
    {
        get; set;
    }
    public DateTime TaxDate
    {
        get; set;
    }
    public decimal WTaxTotalAmountLocal
    {
        get; set;
    }
    public decimal WTaxTotalAmount
    {
        get; set;
    }


    public string TransferToInfo
    {
        get; set;
    }
}
