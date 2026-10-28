using Accounting_V4.Model.QNEModels.TransactionCodesModels;

namespace Accounting_V4.Model.QNEModels.SalesModels;

public class SalesInvoiceDetails : TransDetail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Qty
    {
        get; set;
    }

    public Guid? UOMId
    {
        get; set;
    }

    public decimal UnitPrice { get; set; } = 0.00000000M;

    public string? Discount
    {
        get; set;
    }

    public string? SalesTax
    {
        get; set;
    }

    public Guid? StockLocationId
    {
        get; set;
    }

    public string? FromType
    {
        get; set;
    }

    public Guid? FromDocId
    {
        get; set;
    }

    public Guid? FromDtlId
    {
        get; set;
    }

    public bool? IsPartialTransfer
    {
        get; set;
    }

    public string? StockTransactionId
    {
        get; set;
    }

    public Guid SalesInvoiceId
    {
        get; set;
    }

    public Guid? StockId
    {
        get; set;
    }

    public int Pos
    {
        get; set;
    }

    public Guid? GLAccountId
    {
        get; set;
    }

    public string? Description
    {
        get; set;
    }

    public int? ProjectId
    {
        get; set;
    }

    public string? ReferenceNo
    {
        get; set;
    }

    public decimal Amount
    {
        get; set;
    }

    public int? DepartmentId
    {
        get; set;
    }

    public string? Numbering
    {
        get; set;
    }

    public int? CostCentreId
    {
        get; set;
    }

    public decimal AmountLocal { get; set; } = 0.00M;
    public decimal DiscountAmount { get; set; } = 0.00M;
    public decimal DiscountAmountLocal { get; set; } = 0.00M;

    public decimal NetAmount
    {
        get; set;
    }

    public decimal NetAmountLocal
    {
        get; set;
    }

    public string? Note
    {
        get; set;
    }

    public string? ItemTypeCode
    {
        get; set;
    }

    public bool? IsBundled
    {
        get; set;
    }

    public bool? IsSubItem
    {
        get; set;
    }

    public Guid? BundledTransactionId
    {
        get; set;
    }

    public Guid? TaxCodeId
    {
        get; set;
    }

    public bool IsTaxInclusive
    {
        get; set;
    }

    public decimal? TaxRate
    {
        get; set;
    }

    public int? TaxClass
    {
        get; set;
    }

    public string? TaxClassValue
    {
        get; set;
    }

    public decimal TaxAmount { get; set; } = 0.00000000M;
    public decimal? TaxAmountLocal { get; set; } = 0.00000000M;
    public decimal SubAmount { get; set; } = 0.00M;
    public decimal SubAmountLocal { get; set; } = 0.00M;
    public decimal? TaxExclusiveAmount
    {
        get; set;
    }

    public decimal? TaxExclusiveAmountLocal
    {
        get; set;
    }

    public string? TaxClassValueRef
    {
        get; set;
    }

    public Guid? TransferLink
    {
        get; set;
    }

    public int? StockType
    {
        get; set;
    }

    public decimal? MSUnitCost
    {
        get; set;
    }

    public Guid MSReferenceID
    {
        get; set;
    }

    public int OptimisticLockField
    {
        get; set;
    } = 0;

    public Guid? WTaxCode
    {
        get; set;
    }

    public decimal? WTaxRate { get; set; } = 0.00000000M;
    public decimal WTaxAmount { get; set; } = 0.00000000M;

    public decimal WTaxAmountLocal { get; set; } = 0.00000000M;

    public decimal SgstAmount { get; set; } = 0.00000000M;
    public decimal CgstAmount { get; set; } = 0.00000000M;

    public decimal IgstAmount { get; set; } = 0.00000000M;
    public decimal SgstAmountLocal { get; set; } = 0.00000000M;

    public decimal? CgstAmountLocal { get; set; } = 00.00000000M;
    public decimal? IgstAmountLocal { get; set; } = 0.00000000M;

    public Guid? DebtorItemId
    {
        get; set;
    }

    public Guid? StockBatchNumberId
    {
        get; set;
    }

    public string? Ref
    {
        get; set;
    }

    public string? Ref2
    {
        get; set;
    }

    public string? Ref3
    {
        get; set;
    }

    public string? Ref4
    {
        get; set;
    }

    public string? Ref5
    {
        get; set;
    }

    public DateTime? DateRef1
    {
        get; set;
    }

    public DateTime? DateRef2
    {
        get; set;
    }

    public decimal? NumRef1
    {
        get; set;
    } = 0.00000000M;
    public decimal? NumRef2
    {
        get; set;
    } = 0.00000000M;

    public decimal? ServiceCost
    {
        get; set;
    } = 0.00M;
    public string? SerialNumber
    {
        get; set;
    }

    public string? SerialNumberRef1
    {
        get; set;
    }

    public string? SerialNumberRef2
    {
        get; set;
    }

    public int? TariffCodeId
    {
        get; set;
    }

    public decimal? LocalOneCentsDiff
    {
        get; set;
    }

    public decimal? ServiceAmount
    {
        get; set;
    }

    public decimal? ServiceAmountLocal
    {
        get; set;
    }

    public int? SubcodeId
    {
        get; set;
    }

    public int? CommissionGroupId
    {
        get; set;
    }

    public Guid? PackingUOMId
    {
        get; set;
    }

    public decimal Packing
    {
        get; set;
    } = 0.00000000M;

    public decimal? PackingQty
    {
        get; set;
    }

    public decimal? PackingUOM1
    {
        get; set;
    } = 0.00M;
    public decimal? PackingUOM2
    {
        get; set;
    } = 0.00M;

    public string? HighestUOM
    {
        get; set;
    }

    public decimal? ProgressInvoicingRate
    {
        get; set;
    }

    public Guid? WVatCode
    {
        get; set;
    }

    public decimal WVatRate
    {
        get; set;
    } = 0.00M;

    public decimal WVatAmount
    {
        get; set;
    } = 0.00000000M;

    public decimal WVatAmountLocal
    {
        get; set;
    } = 0.00000000M;

    public Guid? ATCCodeId
    {
        get; set;
    }

    public string? RefTransferFrom
    {
        get; set;
    }

    public decimal? PriorProgressInvoicingRate
    {
        get; set;
    } = 0.00M;

    public int? DimensionId
    {
        get; set;
    }

    public string? TaxExemptNo
    {
        get; set;
    }

    public string? TaxExemptDetails
    {
        get; set;
    }

    public decimal? MarkupDiscountAmount
    {
        get; set;
    }

    public decimal? MarkupDiscountAmountLocal
    {
        get; set;
    }

    public decimal? AmountTaxExempted
    {
        get; set;
    }

    public decimal? AmountTaxExemptedLocal
    {
        get; set;
    }

    public string? MarkupDiscountPrice
    {
        get; set;
    }

    public decimal? StoredPoint
    {
        get; set;
    }

    public decimal? StoredPointQty
    {
        get; set;
    }

    public int? Classification
    {
        get; set;
    }


    public Guid? QuotationDetailsId
    {
        get; set;
    }


    public Guid? SalesOrderDetailsId
    {
        get; set;
    }

    public Guid? DeliveryOderDetailsId
    {
        get; set;
    }
}
