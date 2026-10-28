using Accounting_V4.Model.QNEModels.TransactionCodesModels;

namespace Accounting_V4.Model.QNEModels.SalesModels;

public class QuotationDetails : TransDetail
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();
    public decimal Qty
    {
        get; set;
    }
    public Guid? UOMId
    {
        get; set;
    }
    public decimal UnitPrice { get; set; } = 0.00000000M;
    public string Discount
    {
        get; set;
    }
    public string SalesTax
    {
        get; set;
    }
    public Guid QuotationId
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
    public string Description
    {
        get; set;
    }
    public int? ProjectId
    {
        get; set;
    }
    public string ReferenceNo
    {
        get; set;
    }
    public decimal Amount
    {
        get; set;
    } = 0.00M;
    public int? DepartmentId
    {
        get; set;
    }
    public string Desc2
    {
        get; set;
    }
    public string Numbering
    {
        get; set;
    }
    public Guid? StockLocationId
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
    public string Note
    {
        get; set;
    }
    public string ItemTypeCode
    {
        get; set;
    }
    public bool IsBundled
    {
        get; set;
    } = false;
    public bool IsSubItem
    {
        get; set;
    } = false;
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
    public string TaxClassValue
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
    public string TaxClassValueRef
    {
        get; set;
    }
    public decimal Transferred
    {
        get; set;
    } = 0.00M;
    public decimal? MSUnitCost
    {
        get; set;
    } = 0.00000000M;
    public Guid? MSReferenceID
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public Guid? WTaxCode
    {
        get; set;
    }
    public decimal? WTaxRate { get; set; } = 0.00000000M;
    public decimal WTaxAmount { get; set; } = 0.00000000M;
    public decimal WTaxAmountLocal { get; set; } = 0.00000000M;
    public decimal? CancelledQty { get; set; } = 0.00000000M;
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
    public string Ref
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
    public int? TariffCodeId
    {
        get; set;
    }
    public decimal? LocalOneCentsDiff
    {
        get; set;
    } = 0.00M;
    public decimal? ServiceAmount
    {
        get; set;
    } = 0.00M;
    public decimal? ServiceAmountLocal
    {
        get; set;
    } = 0.00M;
    public int? SubcodeId
    {
        get; set;
    }
    public int? CommissionGroupId
    {
        get; set;
    }
    public decimal? ServiceCost
    {
        get; set;
    } = 0.00M;
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
    } = 0.00000000M;
    public decimal? ProgressInvoicingRate
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
    public string HighestUOM
    {
        get; set;
    }
    public int? DimensionId
    {
        get; set;
    }
    public string TaxExemptNo
    {
        get; set;
    }
    public string TaxExemptDetails
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
    public string MarkupDiscountPrice
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
}
