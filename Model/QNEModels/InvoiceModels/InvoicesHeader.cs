using Accounting_V4.Services.SystemServices.TimeZoneServices;

namespace Accounting_V4.Model.QNEModels.InvoiceModels;

public class InvoicesHeader
{
    public Guid Id
    {
        get; set;
    }
    public string InvoiceCode
    {
        get; set;
    }
    public DateTime InvoiceDate
    {
        get; set;
    } = TimeService.ServerTime;
    public string? InvoiceTo
    {
        get; set;
    }
    public Guid? DebtorId
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
    public DateTime? DueDate
    {
        get; set;
    }
    public int? SalesPersonId
    {
        get; set;
    }
    public int? ProjectId
    {
        get; set;
    }
    public int CurrencyId
    {
        get; set;
    }
    public decimal? CurrencyRate
    {
        get; set;
    }
    public string? Description
    {
        get; set;
    }
    public decimal? TotalAmount
    {
        get; set;
    }
    public bool IsCancelled
    {
        get; set;
    }
    public bool IsDetailForGLPosting
    {
        get; set;
    } = false;
    public decimal? TotalAmountLocal
    {
        get; set;
    }
    public int? GSTTypeId
    {
        get; set;
    }
    public decimal? GSTTaxRate
    {
        get; set;
    }
    public decimal? GSTAmount
    {
        get; set;
    }
    public decimal? GSTTaxAmount
    {
        get; set;
    }
    public decimal? GSTAmountLocal
    {
        get; set;
    }
    public decimal? GSTTaxAmountLocal
    {
        get; set;
    }
    public decimal Balance
    {
        get; set;
    } = 0.00M;
    public int? DepartmentId
    {
        get; set;
    }
    public int? CostCentreId
    {
        get; set;
    }
    public string? FromDocType
    {
        get; set;
    }
    public Guid? FromDocId
    {
        get; set;
    }
    public decimal? TaxTotalAmount
    {
        get; set;
    }
    public decimal? TaxTotalAmountLocal
    {
        get; set;
    }
    public decimal? NetTotalAmount
    {
        get; set;
    }
    public decimal? NetTotalAmountLocal
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
    public string? ExportCountry
    {
        get; set;
    }
    public string? Notes
    {
        get; set;
    }
    public string? ItemDescriptions
    {
        get; set;
    }
    public bool? IsTaxInclusive
    {
        get; set;
    }
    public int? ObjectType
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
    public Guid? RoundingAdjustmentAccount
    {
        get; set;
    }
    public decimal? RoundingAdjustment
    {
        get; set;
    } = 0.00M;
    public decimal? RoundingAdjustmentLocal
    {
        get; set;
    } = 0.00M;
    public bool? IsRounding
    {
        get; set;
    } = false;
    public int OptimisticLockField
    {
        get; set;
    } = 0;
    public decimal? WTaxTotalAmount
    {
        get; set;
    }
    public decimal? WTaxTotalAmountLocal
    {
        get; set;
    }
    public decimal SgstTotalAmount
    {
        get; set;
    } = 0.00M;
    public decimal SgstTotalAmountLocal
    {
        get; set;
    } = 0.00M;
    public decimal CgstTotalAmount
    {
        get; set;
    } = 0.00M;
    public decimal CgstTotalAmountLocal
    {
        get; set;
    } = 0.00M;
    public decimal IgstTotalAmount
    {
        get; set;
    } = 0.00M;
    public decimal IgstTotalAmountLocal
    {
        get; set;
    } = 0.00M;
    public DateTime? GSTReverseDate
    {
        get; set;
    }
    public bool IsDeferredVAT
    {
        get; set;
    } = false;
    public Guid? DocTaxCodeId
    {
        get; set;
    }
    public decimal? DocTaxRate
    {
        get; set;
    }
    public bool? IsDifferentPOS
    {
        get; set;
    } = false;
    public int? PlaceOfSupplyId
    {
        get; set;
    }
    public bool? ReverseCharge
    {
        get; set;
    } = false;
    public bool? IsPaidUnderPA
    {
        get; set;
    } = false;
    public string? EGSTIN
    {
        get; set;
    }
    public bool? IsAmendedInvoice
    {
        get; set;
    } = false;
    public bool? IsLinkedWithAmendedInvoice
    {
        get; set;
    }
    public int RowIndex
    {
        get; set;
    }
    public decimal? ReverseRate
    {
        get; set;
    } = 0.00000000M;
    public bool? IsNonTaxInvoice
    {
        get; set;
    }
    public bool? IsSSTBadDebt
    {
        get; set;
    } = false; // Checked
    public DateTime? PostingDate
    {
        get; set;
    }
    public DateTime? ForecastDueDate
    {
        get; set;
    }
    public decimal? TotalServiceAmount
    {
        get; set;
    } = 0.00M;
    public decimal? TotalServiceAmountLocal
    {
        get; set;
    } = 0.00M;
    public decimal? WVatTotalAmount
    {
        get; set;
    } = 0.00M;// Checked
    public decimal? WVatTotalAmountLocal
    {
        get; set;
    } = 0.00M;// Checked
    public decimal? TotalPaid
    {
        get; set;
    } = 0.00M;
    public decimal? AdjustmentAmount
    {
        get; set;
    } = 0.00M;
    public decimal? TotalMatchedAmount
    {
        get; set;
    } = 0.00M;
    public bool? IsFinanceChargeInv
    {
        get; set;
    }
    public int? PropertyManagementBillingId
    {
        get; set;
    }
    public bool? IsMigratedBadDebtRelief
    {
        get; set;
    }
    public decimal? MigratedReliefAmount
    {
        get; set;
    }
    public bool? IsESales
    {
        get; set;
    } = false; // Checked
    public bool? IsSelfBearWTax
    {
        get; set;
    } = false; // Checked
    public bool? IsDetailPosting
    {
        get; set;
    } = false; // Checked
    public bool? ExcludeFromKnockoff
    {
        get; set;
    }
    public bool? IsEinvoiceReject
    {
        get; set;
    }
    public int? DimensionId
    {
        get; set;
    }
    public Guid? CorrectionCode
    {
        get; set;
    }

    public string? EISId
    {
        get; set;
    }


    public Guid? glaccountTAXSALESACCOUNT
    {
        get; set;
    }
    public Guid? glaccountOUPUTVAT
    {
        get; set;
    }


}
