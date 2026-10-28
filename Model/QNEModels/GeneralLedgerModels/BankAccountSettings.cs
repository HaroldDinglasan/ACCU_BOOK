namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class BankAccountSettings
{
    public int Id
    {
        get; set;
    }
    public Guid GLAccountId
    {
        get; set;
    }
    public decimal OverDraftLimit { get; set; } = 0.00M;
    public Guid? BankChargesAccountId
    {
        get; set;
    }
    public bool UseOwnOR { get; set; } = false;
    public bool UseOwnPV { get; set; } = false;
    public bool UseChequeAutoNo { get; set; } = false;
    public bool IsChequeAllowEdit { get; set; } = false;
    public int? FormatORId
    {
        get; set;
    }
    public int? FormatPVId
    {
        get; set;
    }
    public int? ChequeFormatId
    {
        get; set;
    }
    public bool? AllowEditChequeAutoNo
    {
        get; set;
    }
    public string FormatOR { get; set; } = "N/A";
    public int NextNumberOR { get; set; } = 0;
    public bool IsResetMonthlyRunningOR { get; set; } = false;
    public bool IsResetPerProjectRunningOR { get; set; } = false;
    public string FormatPV { get; set; } = "N/A";
    public int NextNumberPV { get; set; } = 0;
    public bool IsResetMonthlyRunningPV { get; set; } = false;
    public bool IsResetPerProjectRunningPV { get; set; } = false;
    public string ChequePrefix
    {
        get; set;
    }
    public int ChequeNextNo { get; set; } = 0;
    public int OptimisticLockField { get; set; } = 0;
    public string BankCharges
    {
        get; set;
    }
}
