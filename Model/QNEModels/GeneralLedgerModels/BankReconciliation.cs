namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class BankReconciliation
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();
    public Guid GLAccountId
    {
        get; set;
    }
    public int OpeningPos
    {
        get; set;
    } = 0;
    public Guid? GLTransactionId
    {
        get; set;
    }
    public DateTime EntryDate
    {
        get; set;
    }
    public string? Description
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
    public decimal Debit
    {
        get; set;
    } = 0.00M;
    public decimal Credit
    {
        get; set;
    } = 0.00M;
    public decimal DebitForeign
    {
        get; set;
    } = 0.00M;
    public decimal CreditForeign
    {
        get; set;
    } = 0.00M;
    public int TableId
    {
        get; set;
    }
    public int? TransactionTypeId
    {
        get; set;
    }
    public Guid? TransactionId
    {
        get; set;
    }
    public Guid? DocumentId
    {
        get; set;
    }
    public string? DocumentCode
    {
        get; set;
    }
    public string? ReferenceNo
    {
        get; set;
    }
    public DateTime? ClearDate
    {
        get; set;
    }
    public bool? IsCancelled
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
    public int OptimisticLockField
    {
        get; set;
    } = 0;
    public int? ObjectType
    {
        get; set;
    }
    public int RowIndex
    {
        get; set;
    }
}
