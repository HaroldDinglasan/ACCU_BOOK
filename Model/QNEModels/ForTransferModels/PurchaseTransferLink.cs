namespace Accounting_V4.Model.QNEModels.ForTransferModels;

public class PurchaseTransferLink
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();
    public decimal TransferredBaseUnit
    {
        get; set;
    }
    public Guid SourceKey
    {
        get; set;
    }
    public Guid? PurchaseRequisitionDetail
    {
        get; set;
    }
    public Guid?PurchaseOrderDetail
    {
        get; set;
    }
    public Guid? GoodsReceivedNoteDetail
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public string TableName { get; set; }
}
