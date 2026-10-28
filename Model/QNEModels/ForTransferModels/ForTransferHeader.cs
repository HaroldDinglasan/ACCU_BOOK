namespace Accounting_V4.Model.QNEModels.ForTransferModels;

public class ForTransferHeader
{
    public Guid Id
    {
        get; set;
    }

    /// <summary>
    /// PURCHASES MODELS
    /// </summary>
    public Guid? PurchaseRequisitionHeaderId
    {
        get; set;
    }


    public Guid? PurchaseOrderHeaderId
    {
        get; set;
    }

    public Guid? GoodsReceivedNoteHeaderId
    {
        get; set;
    }


    /// <summary>
    /// SALES MODELS
    /// </summary>
    /// 


    public Guid? QuotationsHeaderId
    {
        get; set;
    }
    public Guid? SalesOrdersHeaderId
    {
        get; set;
    }
    public Guid? DeliveryOrdersHeaderId
    {
        get; set;
    }



    public string? TransferToInfo
    {
        get; set;
    }

    public bool IsForTransfer
    {
        get; set;
    }

}
