using Accounting_V4.Model.QNEModels.ForTransferModels;

namespace Accounting_V4.Services.QNEServices.TransferServices;

public class TransferTable
{
    public ModuleTableConfig GetModuleTableConfig(string tableName)
    {
        return tableName switch
        {
            "QT" => new ModuleTableConfig
            {
                HeaderTable = "Quotations",
                DetailsTable = "QuotationDetails",
                HeaderId = "QuotationId",
                DateField = "QuotationDate",
                CodeField = "QuotationCode"
            },

            "SO" => new ModuleTableConfig
            {
                HeaderTable = "SalesOrders",
                DetailsTable = "SalesOrderDetails",
                HeaderId = "SalesOrderId",
                DateField = "SalesOrderDate",
                CodeField = "SalesOrderCode"
            },


            "DO" => new ModuleTableConfig
            {
                HeaderTable = "DeliveryOrders",
                DetailsTable = "DeliveryOrderDetails",
                HeaderId = "DeliveryOrderId",
                DateField = "DeliveryOrderDate",
                CodeField = "DeliveryOrderCode"
            },

            "SI" => new ModuleTableConfig
            {
                HeaderTable = "SalesInvoices",
                DetailsTable = "SalesInvoiceDetails",
                HeaderId = "SalesInvoiceId",
                DateField = "SalesInvoiceDate",
                CodeField = "SalesInvoiceCode"
            },

            ///PURCHASE REQUISITION AND PURCHASE ORDER

            "PR" => new ModuleTableConfig
            {
                HeaderTable = "PurchaseRequisitions",
                DetailsTable = "PurchaseRequisitionDetails",
                HeaderId = "PurchaseRequisitionId",
                DateField = "PurchaseRequisitionDate",
                CodeField = "PurchaseRequisitionCode"
            },

            "PO" => new ModuleTableConfig
            {
                HeaderTable = "PurchaseOrders",
                DetailsTable = "PurchaseOrderDetails",
                HeaderId = "PurchaseOrderId",
                DateField = "PurchaseOrderDate",
                CodeField = "PurchaseOrderCode"
            },

            "GRN" => new ModuleTableConfig
            {
                HeaderTable = "GoodsReceivedNotes",
                DetailsTable = "GoodsReceivedNoteDetails",
                HeaderId = "GoodsReceivedNoteId",
                DateField = "GoodsReceivedNoteDate",
                CodeField = "GoodsReceivedNoteCode"
            },

            _ => null
        };
    }
}
