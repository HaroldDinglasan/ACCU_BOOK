namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class VW_Receipt
{
    public Guid Id
    {
        get; set;
    }

    public DateTime ReceiptDate
    {
        get; set;
    }
    public DateTime ReceiptTime
    {
        get; set;
    }

    public string ReceiptCode
    {
        get; set;
    }
    public string DebtorName
    {
        get; set;
    }
    public string MethodCode
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public string ReferenceNo
    {
        get; set;
    }
    public string CurrencyCode
    {
        get; set;
    }

    public decimal BankChargesAmountLocal
    {
        get; set;
    }

    public Guid BankCharges
    {
        get; set;
    }

    public decimal TotalAmount
    {
        get; set;
    }

    public decimal NetTotalAmount
    {
        get; set;
    }

    public string Notes
    {
        get; set;
    }
    public bool IsCancelled
    {
        get; set;
    }


}
