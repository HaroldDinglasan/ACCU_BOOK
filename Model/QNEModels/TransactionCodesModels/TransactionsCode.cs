namespace Accounting_V4.Model.QNEModels.TransactionCodesModels;

public class TransactionsCode
{
    public int Id
    {
        get; set;
    }
    public int TableId
    {
        get; set;
    }
    public string Name
    {
        get; set;
    }
    public string Format
    {
        get; set;
    }
    public char SuffixFillChar
    {
        get; set;
    }
    public int NextNumber
    {
        get; set;
    }
    public bool IsDefault { get; set; } = true;
    public string DateType
    {
        get; set;
    }
    public int ResetMonthly
    {
        get; set;
    }
    public bool UseProjectCode
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public string ModuleType
    {
        get; set;
    }
    public bool UseCounterCode
    {
        get; set;
    }
    public string SeriesFrom
    {
        get; set;
    }
    public string SeriesTo
    {
        get; set;
    }
    public bool DisplayRemarks1
    {
        get; set;
    }
    public string Remarks1
    {
        get; set;
    }
    public bool DisplayRemarks2
    {
        get; set;
    }
    public string Remarks2
    {
        get; set;
    }

    // Dagdag
    public string Sample
    {
        get; set;
    }

    // Dagdag
    public string Table
    {
        get; set;
    }
}
