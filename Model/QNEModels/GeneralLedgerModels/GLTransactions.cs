namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class GLTransactions
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();

    public DateTime TransactionDate
    {
        get; set;
    }

    public DateTime PostingDate
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

    public string? DetailDescription
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

    public int CurrencyId
    {
        get; set;
    }

    public decimal? CurrencyRate
    {
        get; set;
    }

    public decimal? Debit
    {
        get; set;
    } = 0.00M;

    public decimal? Credit
    {
        get; set;
    } = 0.00M;

    public decimal? DebitForeign
    {
        get; set;
    } = 0.00M;

    public decimal? CreditForeign
    {
        get; set;
    } = 0.00M;

    public int TableId
    {
        get; set;
    }

    public int TransactionTypeId
    {
        get; set;
    }

    public Guid TransactionId
    {
        get; set;
    }

    public Guid? DocumentId
    {
        get; set;
    }

    public string DocumentCode { get; set; }

    public string? ReferenceNo
    {
        get; set;
    }

    public bool IsCancelled
    {
        get; set;
    } = false;

    public Guid? FromAccountId
    {
        get; set;
    }

    public bool IsPostDatedCheque
    {
        get; set;
    } = false;

    public decimal? DebitRevaluation
    {
        get; set;
    }

    public decimal? CreditRevaluation
    {
        get; set;
    }

    public decimal? RateRevaluation
    {
        get; set;
    }

    public int? CostCentreId
    {
        get; set;
    }

    public string? JournalType
    {
        get; set;
    }

    public string? DocType
    {
        get; set;
    }

    public int OptimisticLockField
    {
        get; set;
    } = 0;

    public Guid? TaxCodeId
    {
        get; set;
    }

    public string Ref1
    {
        get; set;
    }

    public int IntId
    {
        get; set;
    }

    public bool? IsBouncedCheque
    {
        get; set;
    } = false;

    public int? DimensionId
    {
        get; set;
    }
}
