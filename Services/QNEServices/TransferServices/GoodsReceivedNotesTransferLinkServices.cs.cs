using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.TransferServices
{
    public class GoodsReceivedNotesTransferLinkServices
    {
        private readonly QneConnectionString qneConnection;
        private readonly TransferTable transferTable;

        public GoodsReceivedNotesTransferLinkServices(QneConnectionString qneConnection, TransferTable transferTable)
        {
            this.qneConnection = qneConnection;
            this.transferTable = transferTable;
        }

        public async Task<IEnumerable<PurchasesTranferList>> GetPurchasesTranferLists(string CreditorId, string TableName)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();
            using var connection = new SqlConnection(connectionString);

            var config = transferTable.GetModuleTableConfig(TableName);
            if (config == null)
                return Enumerable.Empty<PurchasesTranferList>();

            var query = $@"SELECT DISTINCT
                H.Id,
                H.{config.DateField} AS DocDate,
                H.{config.CodeField} AS DocCode,
                H.CreditorId, 
                H.CreditorName,
                PURCH.StaffCode AS Purchaser, 
                PURCH.Name AS RequestBy, 
                TERMS.Term,
                H.TaxTotalAmount, 
                H.NetTotalAmount,
                H.WTaxTotalAmount,
                H.IsCancelled, 
                H.IsClosed, 
                H.IsApproved
            FROM {config.HeaderTable} H

            INNER JOIN {config.DetailsTable} D
            ON H.Id = D.{config.HeaderId}

            LEFT JOIN Terms TERMS ON H.TermId = TERMS.Id
            LEFT JOIN Purchasers PURCH ON H.PurchaserId = PURCH.Id
            LEFT JOIN UOMs U ON D.UOMId = U.Id

            WHERE
                (H.CreditorId = @CreditorId OR H.CreditorId IS NULL)
                AND ISNULL(H.IsClosed, 0) = 0
                AND ISNULL(H.IsCancelled, 0) = 0
                AND ISNULL(H.IsApproved, 1) = 1
                AND D.UOMId IS NOT NULL
                AND ISNULL(D.ItemTypeCode, '') NOT IN ('S', 'D', 'T')
                AND (D.Qty - ISNULL(D.Transferred / U.Rate, 0) > 0)

            ORDER BY H.{config.CodeField} ASC";

            return await connection.QueryAsync<PurchasesTranferList>(query, new { CreditorId });
        }

        public async Task<PurchaseOrdersModel> GetPurchaseOrderHeaderIdForTransfer(string POId)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"SELECT * FROM PurchaseOrders WHERE Id = @POId";

            var model = await connection.QueryFirstOrDefaultAsync<PurchaseOrdersModel>(query, new { POId });
            return model;
        }

        public async Task<IEnumerable<PurchaseOrdersDetails>> GetPurchaseOrderDetailsByIdForTransfer(string POId)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"SELECT 
                PODetails.Id,
                PODetails.PurchaseOrderId,
                PODetails.Qty AS PurchaseOrderDetailsQty,
                (PODetails.Qty-ISNULL(PODetails.Transferred / UOMs.Rate, 0)) AS Qty,
                PODetails.Pos,
                PODetails.Numbering,
                PODetails.StockId,
                PODetails.Description,
                PODetails.ProjectId,
                PODetails.DepartmentId,
                PODetails.StockLocationId,
                PODetails.UOMId,
                PODetails.UnitPrice,
                PODetails.Discount,
                PODetails.Amount,
                PODetails.RequireDate,
                PODetails.SupplierItemCode,
                PODetails.ReferenceNo,
                PODetails.CancelledQty,
                PODetails.FromType,
                PODetails.FromDocId,
                PODetails.FromDtlId,
                PODetails.IsPartialTransfer,
                PODetails.CostCentreId,
                PODetails.AmountLocal,
                PODetails.DiscountAmount,
                PODetails.DiscountAmountLocal,
                PODetails.NetAmount,
                PODetails.NetAmountLocal,
                PODetails.Note,
                PODetails.ItemTypeCode,
                PODetails.IsBundled,
                PODetails.IsSubItem,
                PODetails.BundledTransactionId,
                PODetails.TaxCodeId,
                PODetails.IsTaxInclusive,
                PODetails.TaxRate,
                PODetails.TaxClass,
                PODetails.TaxClassValue,
                PODetails.TaxAmount,
                PODetails.TaxAmountLocal,
                PODetails.SubAmount,
                PODetails.SubAmountLocal,
                PODetails.TaxExclusiveAmount,
                PODetails.TaxExclusiveAmountLocal,
                PODetails.TaxClassValueRef,
                PODetails.TransferLink,
                PODetails.Transferred,
                PODetails.OptimisticLockField,
                PODetails.WTaxCode,
                PODetails.WTaxRate,
                PODetails.WTaxAmount,
                PODetails.WTaxAmountLocal,
                PODetails.SgstAmount,
                PODetails.CgstAmount,
                PODetails.IgstAmount,
                PODetails.SgstAmountLocal,
                PODetails.CgstAmountLocal,
                PODetails.IgstAmountLocal,
                PODetails.CreditorItemId,
                PODetails.Ref,
                PODetails.Ref2,
                PODetails.Ref3,
                PODetails.Ref4,
                PODetails.Ref5,
                PODetails.DateRef1,
                PODetails.DateRef2,
                PODetails.NumRef1,
                PODetails.NumRef2,
                PODetails.SOTOPOTransferLink,
                PODetails.TariffCodeId,
                PODetails.LocalOneCentsDiff,
                PODetails.ServiceAmount,
                PODetails.ServiceAmountLocal,
                PODetails.SubcodeId,
                PODetails.CommissionGroupId,
                PODetails.PackingUOMId,
                PODetails.Packing,
                PODetails.PackingQty,
                PODetails.PackingUOM1,
                PODetails.PackingUOM2,
                PODetails.HighestUOM,
                PODetails.RefTransferFrom,
                PODetails.RefSOTOPOTransferFrom,
                PODetails.DimensionId,
                PODetails.TaxExemptNo,
                PODetails.TaxExemptDetails,
                PODetails.MarkupDiscountAmount,
                PODetails.MarkupDiscountAmountLocal,
                PODetails.AmountTaxExempted,
                PODetails.AmountTaxExemptedLocal,
                PODetails.MarkupDiscountPrice
            FROM PurchaseOrderDetails PODetails
            LEFT JOIN UOMs ON PODetails.UOMId = UOMs.Id
            WHERE PODetails.PurchaseOrderId = @POId
            AND (PODetails.Qty-ISNULL(PODetails.Transferred / UOMs.Rate, 0) > 0)
            ORDER BY PODetails.Pos ASC";

            var model = await connection.QueryAsync<PurchaseOrdersDetails>(query, new { POId });
            return model;
        }




    }
}