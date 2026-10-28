namespace Accounting_V4.Model.QNEModels.ForTransferModels;

public class SalesTransferList
{
    public Guid Id
    {
        get; set;
    }                
    public string DocCode
    {
        get; set;
    }         
    public DateTime? DocDate
    {
        get; set;
    }    

    // Creditor
    public Guid? DebtorId
    {
        get; set;
    }      
    public string DebtorName
    {
        get; set;
    }  
    public string Agent
    {
        get; set;
    }      
    public string RequestBy
    {
        get; set;
    }       // SYSR.UserName
    public string Term
    {
        get; set;
    }            // TERM.Term

    // Amounts
    public decimal TaxTotalAmount
    {
        get; set;
    } = 0.00M;// TaxTotalAmount
    public decimal NetTotalAmount
    {
        get; set;
    } = 0.00M;// NetTotalAmount

    // Status flags
    public bool IsCancelled
    {
        get; set;
    }       // IsCancelled
    public bool IsClosed
    {
        get; set;
    }          // IsClosed
    public bool IsApproved
    {
        get; set;
    }        // PR.IsApproved
}
