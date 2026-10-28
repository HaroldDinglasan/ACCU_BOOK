using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class Accounts
{
    [Key]
    public Guid Id
    {
        get; set;
    }
    public string AccountCode
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public int ChartSequence
    {
        get; set;
    }
    public string DRCR
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public int ChartSequencePH
    {
        get; set;
    }
    public string AccountCodePH
    {
        get; set;
    }
    public int RowIndex
    {
        get; set;
    }

    // public List<GeneralLedgerAccount> GeneralLedgerAccounts { get; set; } = new List<GeneralLedgerAccount>();
}
