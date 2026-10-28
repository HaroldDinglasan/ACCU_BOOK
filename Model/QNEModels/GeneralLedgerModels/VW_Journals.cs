using Accounting_V4.Services.SystemServices.TimeZoneServices;

namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class VW_Journals
{
    public Guid Id
    {
        get; set;
    }
    public DateTime JournalDate
    {
        get; set;
    }
    public string JournalCode
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public string CurrencyCode
    {
        get; set;
    }
    public string ProjectCode
    {
        get; set;
    }
    public decimal? CurrencyRate
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
    } 
    public decimal TotalDebit
    {
        get; set;
    } 
    public decimal TotalCredit
    {
        get; set;
    } 
    public bool IsTaxInclusive
    {
        get; set;
    }


    public decimal? GSTRate
    {
        get; set;
    }
    public bool IsRevaluationDR
    {
        get; set;
    } 
    public bool IsRevaluationCR
    {
        get; set;
    } 
    public bool IsRevaluationGL
    {
        get; set;
    } 
    public bool IsBadDebtRelief
    {
        get; set;
    } 
    public bool IsBadDebtRecovery
    {
        get; set;
    } 
    public bool IsUnpaidPurchase
    {
        get; set;
    } 
    public bool IsPaymentForUnpaidPurchase
    {
        get; set;
    } 
    public int JournalEntryType
    {
        get; set;
    } 
    public bool IsTaxInclusiveOnly
    {
        get; set;
    } 
    public int OptimisticLockField
    {
        get; set;
    } 
    public int ObjectType
    {
        get; set;
    } 
    public DateTime TaxDate
    {
        get; set;
    } 
    public bool IsImportedServicePosted
    {
        get; set;
    } 
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
    } 
    public int? ProjectId
    {
        get; set;
    }
    public bool IsSSTBadDebt
    {
        get; set;
    } 
    public int? CostCentreId
    {
        get; set;
    }
    public bool? IsESales
    {
        get; set;
    }
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
}
