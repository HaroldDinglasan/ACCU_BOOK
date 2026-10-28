using Accounting_V4.Services.SystemServices.TimeZoneServices;

namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class Journals
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();

    public string JournalCode
    {
        get; set;
    }
    public DateTime JournalDate
    {
        get; set;
    } = TimeService.ServerTime;
    public int CurrencyId
    {
        get; set;
    }
    public decimal? CurrencyRate
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public string? Notes
    {
        get; set;
    }
    public bool IsCancelled
    {
        get; set;
    } = false;
    public decimal TotalDebit
    {
        get; set;
    } = 0.00M;
    public decimal TotalCredit
    {
        get; set;
    } = 0.00M;
    public bool IsTaxInclusive
    {
        get; set;
    } = true;
    public decimal? GSTRate
    {
        get; set;
    }
    public bool IsRevaluationDR
    {
        get; set;
    } = false;
    public bool IsRevaluationCR
    {
        get; set;
    } = false;
    public bool IsRevaluationGL
    {
        get; set;
    } = false;
    public bool IsBadDebtRelief
    {
        get; set;
    } = false;
    public bool IsBadDebtRecovery
    {
        get; set;
    } = false;
    public bool IsUnpaidPurchase
    {
        get; set;
    } = false;
    public bool IsPaymentForUnpaidPurchase
    {
        get; set;
    } = false;
    public int JournalEntryType
    {
        get; set;
    } = 0;
    public bool? IsTaxInclusiveOnly
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    } = 0;
    public int ObjectType
    {
        get; set;
    } = 120;
    public DateTime TaxDate
    {
        get; set;
    } = TimeService.ServerTime;
    public bool IsImportedServicePosted
    {
        get; set;
    } = false;
    public string RowIndex
    {
        get; set;
    }
    public DateTime? ForecastDueDate
    {
        get; set;
    }
    public decimal ReverseRate
    {
        get; set;
    } = 0.00000000M;
    public int? ProjectId
    {
        get; set;
    }
    public bool IsSSTBadDebt
    {
        get; set;
    } = false;
    public int? CostCentreId
    {
        get; set;
    }
    public bool? IsESales
    {
        get; set;
    } = false;
    public bool? ExcludeFromKnockoff
    {
        get; set;
    }
    public bool? IsEinvoiceReject
    {
        get; set;
    } = false;
    public int? DimensionId
    {
        get; set;
    }


}
