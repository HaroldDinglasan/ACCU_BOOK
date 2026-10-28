using Accounting_V4.Services.SystemServices.TimeZoneServices;

namespace Accounting_V4.Model.QNEModels.DeliveryOrderModels;

public class DeliveryOrdersHeader
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DeliveryOrderCode
    {
        get; set;
    }
    public DateTime DeliveryOrderDate { get; set; } = TimeService.ServerTime;


    public Guid? DebtorId
    {
        get; set;
    }
    public string DebtorName
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
    public string ReferenceNo
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
    public string DeliveryTerm
    {
        get; set;
    }
    public string Title
    {
        get; set;
    }
    public string Title2
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
    public string Ref3
    {
        get; set;
    }
    public string Ref4
    {
        get; set;
    }
    public string Ref5
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
    public string Remark3
    {
        get; set;
    }
    public string Remark4
    {
        get; set;
    }
    public string Remark5
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
    public int CurrencyId
    {
        get; set;
    }
    public decimal CurrencyRate
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
    public bool IsCancelled
    {
        get; set;
    }
    public bool IsDelivered
    {
        get; set;
    }
    public bool IsClosed
    {
        get; set;
    }
    //public DateTime? RequireDate
    //{
    //    get; set;
    //}
    public string DOBranchCode
    {
        get; set;
    }
    public string DOBranchName
    {
        get; set;
    }
    public string DOContact
    {
        get; set;
    }
    public string DOPhone
    {
        get; set;
    }
    public string DOFax
    {
        get; set;
    }
    public string DOAddress1
    {
        get; set;
    }
    public string DOAddress2
    {
        get; set;
    }
    public string DOAddress3
    {
        get; set;
    }
    public string DOAddress4
    {
        get; set;
    }
    public int? CostCentreId
    {
        get; set;
    }
    public string Discount
    {
        get; set;
    }
    public decimal DiscountAmount
    {
        get; set;
    }
    public decimal NetTotalAmount
    {
        get; set;
    }
    public decimal DiscountAmountLocal
    {
        get; set;
    }
    public decimal TotalAmountLocal
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
    public decimal SubtotalAmount
    {
        get; set;
    }
    public decimal SubtotalAmountLocal
    {
        get; set;
    }
    public string ExportCountry
    {
        get; set;
    }
    public string Notes
    {
        get; set;
    }
    public bool IsTaxInclusive
    {
        get; set;
    }
    public DateTime TaxDate { get; set; } = TimeService.ServerTime;
    public decimal TaxExclusiveTotalAmount
    {
        get; set;
    }
    public decimal TaxExclusiveTotalAmountLocal
    {
        get; set;
    }
    public bool IsTaxInclusiveOnly
    {
        get; set;
    }
    public Guid? RoundingAdjustmentAccount
    {
        get; set;
    } = null;
    public decimal RoundingAdjustment
    {
        get; set;
    }
    public decimal RoundingAdjustmentLocal
    {
        get; set;
    }
    public bool IsRounding
    {
        get; set;
    }
    public bool IsTaxDeclared
    {
        get; set;
    }
    public int OptimisticLockField
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
    public string DORegistationNo
    {
        get; set;
    }
    public string DOGSTRegNo
    {
        get; set;
    }
    public string DOPhone2
    {
        get; set;
    }
    public string DOEmail
    {
        get; set;
    }
    public string DORemark
    {
        get; set;
    }
    public int? DeliveryArea
    {
        get; set;
    }
    public decimal SgstTotalAmount
    {
        get; set;
    }
    public decimal SgstTotalAmountLocal
    {
        get; set;
    }
    public decimal CgstTotalAmount
    {
        get; set;
    }
    public decimal CgstTotalAmountLocal
    {
        get; set;
    }
    public decimal IgstTotalAmount
    {
        get; set;
    }
    public decimal IgstTotalAmountLocal
    {
        get; set;
    }
    public string TransferFromInfo
    {
        get; set;
    }
    public string TransferToInfo
    {
        get; set;
    }
    public bool IsApproved
    {
        get; set;
    } = true;
    public decimal ReverseRate
    {
        get; set;
    }
    public string SIRef1
    {
        get; set;
    }
    public string SIRef2
    {
        get; set;
    }
    public decimal TotalServiceAmount
    {
        get; set;
    }
    public decimal TotalServiceAmountLocal
    {
        get; set;
    }
    public int? FixedCommissionInfoId
    {
        get; set;
    }
    public bool IsNonTaxInvoice
    {
        get; set;
    }

    public string BillerCode
    {
        get; set;
    }
    public int? JomPayRefType1
    {
        get; set;
    }
    public string JomPayRef1
    {
        get; set;
    }
    public int? JomPayRefType2
    {
        get; set;
    }
    public string JomPayRef2
    {
        get; set;
    }
    public bool UseCreditCard
    {
        get; set;
    }
    public int? PackingListId
    {
        get; set;
    }
    public bool IsPast
    {
        get; set;
    }
    public bool IsSelfBearWTax
    {
        get; set;
    }
    public string ZipCode
    {
        get; set;
    }
    public string City
    {
        get; set;
    }
    public int? StateId
    {
        get; set;
    }
    public int? CountryId
    {
        get; set;
    }
    public string TinType
    {
        get; set;
    }
    public string CustomerTIN
    {
        get; set;
    }
    public int? DimensionId
    {
        get; set;
    }
    public decimal TotalAmountTaxExempted
    {
        get; set;
    }
    public decimal TotalAmountTaxExemptedLocal
    {
        get; set;
    }
    public bool IsEinvoiceReject
    {
        get; set;
    }
}
