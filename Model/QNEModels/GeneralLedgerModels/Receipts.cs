using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.SystemServices.TimeZoneServices;

namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class Receipts
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();
    public string ReceiptCode
    {
        get; set;
    }
    public DateTime ReceiptDate
    {
        get; set;
    } = TimeService.ServerTime;

    public Guid? GLAccountId
    {
        get; set;
    }
    public string? ReferenceNo
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
    } = 1;
    public decimal? CurrencyRate
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
    public string? Description
    {
        get; set;
    }

    ///for details description, Pinagsamang Deposit to Na account name at description lahat yan galing headers na data
    public string? DetailsDescription
    {
        get; set;
    }

    public decimal TotalAmount
    {
        get; set;
    } = 0.00M;
    public bool IsCancelled
    {
        get; set;
    } = false;
    public bool? IsDetailForGLPosting
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

    public decimal TotalAmountLocal
    {
        get; set;
    } = 0.00M;
    public decimal? GSTAmountLocal
    {
        get; set;
    }
    public decimal? WTaxAmount
    {
        get; set;
    }

    public bool? IsPostDatedCheque
    {
        get; set;
    } = false;
    public DateTime? ChequePreparedDate
    {
        get; set;
    }
    public DateTime ChequeReceivedDate
    {
        get; set;
    } = TimeService.ServerTime;

    public bool? IsBouncedCheque
    {
        get; set;
    } = false;
    public DateTime? BouncedChequeDate
    {
        get; set;
    }

    public bool? IsMultiPayment
    {
        get; set;
    }
    public decimal? Balance
    {
        get; set;
    }

    public int? SalesPersonId
    {
        get; set;
    }
    public int? DepartmentId
    {
        get; set;
    }
    public int? CostCentreId
    {
        get; set;
    }

    public decimal TaxTotalAmount
    {
        get; set;
    } = 0.00M;
    public decimal TaxTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public decimal NetTotalAmount
    {
        get; set;
    } = 0.00M;
    public decimal NetTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public decimal SubtotalAmount
    {
        get; set;
    } = 0.00M;
    public decimal SubtotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public decimal? BankChargesAmountLocal
    {
        get; set;
    }

    public string? Notes
    {
        get; set;
    }

    public int ObjectType
    {
        get; set;
    } 
    ///--Receipt Payment Object Id 21 / Receipt Voucher Id 7

    public DateTime TaxDate
    {
        get; set;
    } = TimeService.ServerTime;

    public Guid? BankCharges
    {
        get; set;
    }
    public Guid? PaymentReceiptInfoId
    {
        get; set;
    }

    public decimal TaxExclusiveTotalAmount
    {
        get; set;
    } = 00.00M;
    public decimal TaxExclusiveTotalAmountLocal
    {
        get; set;
    } = 00.00M;

    public bool IsDepositDoc
    {
        get; set;
    } = false;
    public bool IsTaxInclusive
    {
        get; set;
    } = false;
    public bool? IsTaxInclusiveOnly
    {
        get; set;
    }

    public Guid? RoundingAdjustmentAccount
    {
        get; set;
    }
    public decimal RoundingAdjustment
    {
        get; set;
    } = 0.00M;
    public decimal RoundingAdjustmentLocal
    {
        get; set;
    } = 0.00M;

    public bool IsRounding
    {
        get; set;
    } = false;

    public int OptimisticLockField
    {
        get; set;
    } = 0;

    public Guid? TaxCodeId
    {
        get; set;
    }
    public decimal? TaxRate
    {
        get; set;
    }

    public decimal WTaxTotalAmount
    {
        get; set;
    } = 0.00M;
    public decimal WTaxTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public bool PostGLDescription
    {
        get; set;
    } = false;
    public bool UseMultiPayment
    {
        get; set;
    } = false;

    public decimal? UnmatchAmount
    {
        get; set;
    }
    public decimal? Cost
    {
        get; set;
    }
    public decimal? CostLocal
    {
        get; set;
    }

    public DateTime? ClearedDate
    {
        get; set;
    }

    public int RowIndex
    {
        get; set;
    }

    public Guid? PaymentMethodMaintenanceId
    {
        get; set;
    }

    public decimal? TotalCashAmount
    {
        get; set;
    } = 0.00M;
    public decimal? TotalChequeAmount
    {
        get; set;
    } = 0.00M;

    public decimal? ReverseRate
    {
        get; set;
    } = 0.00000000M;

    public string? Ref1
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

    public string? Remark1
    {
        get; set;
    }
    public string? Remark2
    {
        get; set;
    }
    public string? Remark3
    {
        get; set;
    }
    public string? Remark4
    {
        get; set;
    }
    public string? Remark5
    {
        get; set;
    }

    public DateTime? ChequeDate
    {
        get; set;
    }

    public decimal? TotalWtaxAmount
    {
        get; set;
    }

    public bool? HasDeposit
    {
        get; set;
    }

    public DateTime? WTaxDate
    {
        get; set;
    }

    public decimal WVatTotalAmount
    {
        get; set;
    } = 0.00M;
    public decimal WVatTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public bool IsESales
    {
        get; set;
    } = false;
    public bool IsDetailPosting
    {
        get; set;
    } = false;
    public bool? ExcludeFromKnockoff
    {
        get; set;
    }
    public bool IsEinvoiceReject
    {
        get; set;
    } = false;

    public int? DimensionId
    {
        get; set;
    }

    public Guid? CorrectionCode
    {
        get; set;
    }

    public string EISId
    {
        get; set;
    } = EISIdService.Generate();

    public decimal? TotalWVatAmount
    {
        get; set;
    }
    public decimal? TotalRefund
    {
        get; set;
    }
}
