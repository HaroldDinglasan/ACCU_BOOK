namespace Accounting_V4.Model.QNEModels.PuchasesModels;

public class VW_PurchaseOrdersListing
{
    public Guid Id
    {
        get; set;
    }

    public string PurchaseOrderCode
    {
        get; set;
    }

    public DateTime PurchaseOrderDate
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

    public int AreaCode
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

    public decimal CurrencyRate { get; set; } = 0.00000000M;

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




    public Guid CreditorId
    {
        get; set;
    }

    public string CreditorName
    {
        get; set;
    }

    public int PurchaserId
    {
        get; set;
    }

    public int TermId
    {
        get; set;
    }

    public int ProjectId
    {
        get; set;
    }

    public int? DepartmentId
    {
        get; set;
    }

    public Guid StockLocationId
    {
        get; set;
    }

    public int CurrencyId
    {
        get; set;
    }

    public decimal TotalAmountLocal { get; set; } = 0.00M;

    public int? CostCentreId
    {
        get; set;
    }

    public decimal Discount
    {
        get; set;
    }

    public decimal DiscountAmount { get; set; } = 0.00000000M;

    public decimal DiscountAmountLocal { get; set; } = 0.00000000M;

    public decimal NetTotalAmount { get; set; } = 0.00M;

    public DateTime DueDate
    {
        get; set;
    }

    public decimal TaxTotalAmount { get; set; } = 0.00M;

    public decimal TaxTotalAmountLocal { get; set; } = 0.00M;

    public decimal SubtotalAmount { get; set; } = 0.00M;

    public decimal SubtotalAmountLocal { get; set; } = 0.00M;

    public string Notes
    {
        get; set;
    }

    public bool IsTaxInclusive
    {
        get; set;
    }

    public DateTime TaxDate
    {
        get; set;
    }

    public decimal TaxExclusiveTotalAmount { get; set; } = 0.00M;

    public decimal TaxExclusiveTotalAmountLocal { get; set; } = 0.00M;

    public bool IsTaxInclusiveOnly
    {
        get; set;
    } // Naka NULL lahat

    public decimal RoundingAdjustmentAccount
    {
        get; set;
    } // Naka NULL lahat

    public decimal RoundingAdjustment { get; set; } = 0.00M;

    public decimal RoundingAdjustmentLocal { get; set; } = 0.00M;

    public bool IsRounding
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

    public decimal SgstTotalAmount { get; set; } = 0.00M;

    public decimal SgstTotalAmountLocal { get; set; } = 0.00M;

    public decimal CgstTotalAmount { get; set; } = 0.00M;

    public decimal CgstTotalAmountLocal { get; set; } = 0.00M;

    public decimal IgstTotalAmount { get; set; } = 0.00M;

    public decimal IgstTotalAmountLocal { get; set; } = 0.00M;

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
    }

    public string TransferFromSOInfo
    {
        get; set;
    }

    public decimal ReverseRate
    {
        get; set;
    }

    public bool IsNonTaxInvoice
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

    public decimal TotalServiceAmount
    {
        get; set;
    }

    public decimal TotalServiceAmountLocal
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

    public int? DimensionId
    {
        get; set;
    } // Naka NULL lahat

    public string FullAddress
    {
        get; set;
    }

    public string SupplierAddress1
    {
        get; set;
    }

    public string SupplierAddress2
    {
        get; set;
    }

    public string SupplierAddress3
    {
        get; set;
    }

    public string SupplierAddress4
    {
        get; set;
    }

    public string PhoneNo1
    {
        get; set;
    }

    public string PhoneNo2
    {
        get; set;
    }

    public string FaxNo1
    {
        get; set;
    }

    public string FaxNo2
    {
        get; set;
    }

    public string ContactPerson
    {
        get; set;
    }

    public string SupplierGSTRegNo
    {
        get; set;
    }

    public string Term
    {
        get; set;
    }

    public string CurrencyName
    {
        get; set;
    }

    public string LocationDescription
    {
        get; set;
    }

    public string Numbering
    {
        get; set;
    }

    public int Pos
    {
        get; set;
    }

    public string ItemDescription
    {
        get; set;
    }

    public decimal ItemQty { get; set; } = 0.00000000M;

    public decimal ItemUnitPrice { get; set; } = 0.00000000M;

    public decimal ItemAmount { get; set; } = 0.00M;

    public string ItemReferenceNo
    {
        get; set;
    } // Naka NULL lahat

    public DateTime ItemRequireDate
    {
        get; set;
    }

    public decimal CancelledQty { get; set; } = 0.00000000M;

    public string ItemDiscount
    {
        get; set;
    }

    public string SupplierItemCode
    {
        get; set;
    } // Naka NULL lahat

    public string ItemTypeCode
    {
        get; set;
    }

    public string StockCode
    {
        get; set;
    }

    public string StockName
    {
        get; set;
    }

    public string UOMCode
    {
        get; set;
    }

    public string ItemLocationCode
    {
        get; set;
    }

    public string ItemLocationDescription
    {
        get; set;
    }

    public string ItemProjectCode
    {
        get; set;
    }

    public string ItemProjectDescription
    {
        get; set;
    }

    public string ItemDepartmentCode
    {
        get; set;
    }

    public string ItemDepartmentDescription
    {
        get; set;
    }

    public string ItemFurtherDescription
    {
        get; set;
    }

    public decimal ItemNetAmount { get; set; } = 0.00M;

    public decimal ItemNetAmountLocal { get; set; } = 0.00M;

    public decimal ItemSubAmount { get; set; } = 0.00M;

    public decimal ItemSubAmountLocal { get; set; } = 0.00M;

    public string TaxType
    {
        get; set;
    }

    public string TaxCode
    {
        get; set;
    }

    public decimal TaxRate { get; set; } = 0.0000M;

    public decimal ItemTaxAmount { get; set; } = 0.00000000M;

    public decimal ItemTaxAmountLocal { get; set; } = 0.00000000M;
}
