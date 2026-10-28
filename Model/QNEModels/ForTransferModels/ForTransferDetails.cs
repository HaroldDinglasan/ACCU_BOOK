namespace Accounting_V4.Model.QNEModels.ForTransferModels;

public class ForTransferDetails
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? TransferLink
    {
        get; set;
    }

    public Guid SourceKey
    {
        get; set;
    }
    public string RefTransferFrom
    {
        get; set;
    }


    public decimal TransferredBaseUnit
    {
        get; set;
    }

    public decimal Transferred
    {
        get; set;
    }

    /// <summary>
    /// FOR PURCHASER
    /// </summary>
    public Guid? PurchaseRequisitionDetailId
    {
        get; set;
    }


    public Guid? PurchaseOrderDetailId
    {
        get; set;
    }

    public Guid? GoodsReceivedNoteDetailId
    {
        get; set;
    }


    /// <summary>
    /// FOR SALES
    /// </summary>

    public Guid? QuotationDetailId
    {
        get; set;
    }
    public Guid? SalesOrderDetailId
    {
        get; set;
    }
    public Guid? DeliveryOrderDetailId
    {
        get; set;
    }


    public int OptimisticLockField { get; set; } = 1;



}

