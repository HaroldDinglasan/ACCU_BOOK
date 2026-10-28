namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class BankCharges
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? GLAccount
    {
        get; set;
    }
    public bool UseBankAccountCurrency { get; set; } = false;
    public int Currency { get; set; } = 1;
    public decimal CurrencyRate { get; set; } = 1.00000000M;
    public Guid? TaxCode
    {
        get; set;
    }
    public decimal TaxRate { get; set; } = 0.00000000M;
    public decimal Amount { get; set; } = 0.00M;
    public decimal AmountLocal { get; set; } = 0.00M;
    public decimal TaxAmount { get; set; } = 0.00M;
    public decimal TaxAmountLocal { get; set; } = 0.00M;
    public int PostingStrategy { get; set; } = 0;
    public int OptimisticLockField { get; set; } = 0;
}
