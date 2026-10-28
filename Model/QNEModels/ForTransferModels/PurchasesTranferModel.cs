namespace Accounting_V4.Model.QNEModels.ForTransferModels;

public class PurchasesTranferModel
{
    public Guid Id
    {
        get; set;
    }                 // PR.Id
    public string DocCode
    {
        get; set;
    }         // PurchaseRequisitionCode
    public DateTime? DocDate
    {
        get; set;
    }      // PurchaseRequisitionDate (nullable kung pwedeng null)

    // Creditor
    public Guid? CreditorId
    {
        get; set;
    }       // CREDITOR.Id (nullable kung walang creditor)
    public string CreditorName
    {
        get; set;
    }    // CREDITOR.CompanyName
    public string CreditorCode
    {
        get; set;
    }    // CREDITOR.CompanyCode

    // Purchaser / RequestBy / Terms
    public string Purchaser
    {
        get; set;
    }       // PRCHASERS.Name
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
