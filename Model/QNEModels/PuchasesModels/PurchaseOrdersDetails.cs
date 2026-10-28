using Accounting_V4.Model.QNEModels.TransactionCodesModels;

namespace Accounting_V4.Model.QNEModels.PuchasesModels;

public class PurchaseOrdersDetails : TransDetail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PurchaseOrderId
    {
        get; set;
    }

    public int Pos
    {
        get; set;
    }

    public string Numbering
    {
        get; set;
    }

    public Guid? StockId
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

    public int? DepartmentId
    {
        get; set;
    }

    public Guid? StockLocationId
    {
        get; set;
    }

    public decimal Qty
    {
        get; set;
    }

    public Guid? UOMId
    {
        get; set;
    }

    public decimal UnitPrice
    {
        get; set;
    }

    public string Discount
    {
        get; set;
    }

    public decimal Amount
    {
        get; set;
    }

    public DateTime? RequireDate
    {
        get; set;
    }

    public string SupplierItemCode
    {
        get; set;
    }

    public string ReferenceNo
    {
        get; set;
    }

    public string FromType
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

    public int? CostCentreId
    {
        get; set;
    }

    public decimal AmountLocal
    {
        get; set;
    }

    public decimal DiscountAmount
    {
        get; set;
    }

    public decimal DiscountAmountLocal
    {
        get; set;
    }

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

    public int TaxClass
    {
        get; set;
    } = 0;

    public string TaxClassValue
    {
        get; set;
    }

    public decimal TaxAmount
    {
        get; set;
    }

    public decimal? TaxAmountLocal
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

    public Guid? TransferLink
    {
        get; set;
    }

    public decimal Transferred
    {
        get; set;
    }

    public int? OptimisticLockField
    {
        get; set;
    }

    public Guid? WTaxCode
    {
        get; set;
    }

    public decimal? WTaxRate { get; set; } = 0.00M;
    public decimal WTaxAmount { get; set; } = 0.00M;
    public decimal WTaxAmountLocal { get; set; } = 0.00M;
    public decimal? CancelledQty { get; set; } = 0.00M;
    public decimal SgstAmount { get; set; } = 0.00M;
    public decimal CgstAmount { get; set; } = 0.00M;
    public decimal IgstAmount { get; set; } = 0.00M;
    public decimal SgstAmountLocal { get; set; } = 0.00M;
    public decimal? CgstAmountLocal { get; set; } = 0.00M;
    public decimal? IgstAmountLocal { get; set; } = 0.00M;

    public Guid? CreditorItemId
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
    }

    public decimal? NumRef2
    {
        get; set;
    }

    public Guid? SOTOPOTransferLink
    {
        get; set;
    }

    public int TariffCodeId
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

    public decimal? Packing
    {
        get; set;
    }

    public decimal? PackingQty
    {
        get; set;
    }

    public decimal? PackingUOM1
    {
        get; set;
    }

    public decimal? PackingUOM2
    {
        get; set;
    }

    public string HighestUOM
    {
        get; set;
    }

    public string RefTransferFrom
    {
        get; set;
    }

    public string RefSOTOPOTransferFrom
    {
        get; set;
    }

    public int? DimensionId
    {
        get; set;
    } // NAKA NULL

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


    public Guid? PurchaseRequisitionDetailId
    {
        get; set;
    }


    public Guid? PurchaseOrderDetailId
    {
        get; set;
    }

    public Guid? GoodsReceivedNoteDetailId
    {
        get; set;
    }

}
