using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Accounting_V4.Pages.QNEPages.Purchases.GoodsReceivedNotes;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.TransferServices
{
    public class PurchaseInvoiceTransferLinkServices
    {
        private readonly QneConnectionString qneConnection;

        private readonly TransferTable transferTable;

        public PurchaseInvoiceTransferLinkServices(QneConnectionString qneConnection, TransferTable transferTable)
        {
            this.qneConnection = qneConnection;
            this.transferTable = transferTable;
        }

        public async Task<GoodsReceivedNotesModel> GetGoodReceivedNoteHeaderIdForTransfer(string GRNId)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"SELECT * FROM GoodsReceivedNotes WHERE Id = @GRNId";

            var model = await connection.QueryFirstOrDefaultAsync<GoodsReceivedNotesModel>(query, new { GRNId });
            return model;
        }

        public async Task<IEnumerable<GoodsReceivedNotesDetails>> GetGoodReceivedNoteDetailsByIdForTransfer(string GRNId)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();
            using var connection = new SqlConnection(connectionString);

            var query = @"SELECT
                GRNDetails.Id,
                GRNDetails.GoodsReceivedNoteId,
                (GRNDetails.Qty - ISNULL(GRNDetails.Transferred / UOMs.Rate, 0)) AS Qty,
                GRNDetails.Pos,
                GRNDetails.Numbering,
                GRNDetails.StockId,
                GRNDetails.Description,
                GRNDetails.ProjectId,
                GRNDetails.DepartmentId,
                GRNDetails.StockLocationId,
                GRNDetails.UOMId,
                GRNDetails.UnitPrice,
                GRNDetails.Discount,
                GRNDetails.Amount,
                GRNDetails.RequireDate,
                GRNDetails.SupplierItemCode,
                GRNDetails.ReferenceNo,
                GRNDetails.CancelledQty,
                GRNDetails.FromType,
                GRNDetails.FromDocId,
                GRNDetails.FromDtlId,
                GRNDetails.IsPartialTransfer,
                GRNDetails.CostCentreId,
                GRNDetails.AmountLocal,
                GRNDetails.DiscountAmount,
                GRNDetails.DiscountAmountLocal,
                GRNDetails.NetAmount,
                GRNDetails.NetAmountLocal,
                GRNDetails.Note,
                GRNDetails.ItemTypeCode,
                GRNDetails.IsBundled,
                GRNDetails.IsSubItem,
                GRNDetails.BundledTransactionId,
                GRNDetails.TaxCodeId,
                GRNDetails.IsTaxInclusive,
                GRNDetails.TaxRate,
                GRNDetails.TaxClass,
                GRNDetails.TaxClassValue,
                GRNDetails.TaxAmount,
                GRNDetails.TaxAmountLocal,
                GRNDetails.SubAmount,
                GRNDetails.SubAmountLocal,
                GRNDetails.TaxExclusiveAmount,
                GRNDetails.TaxExclusiveAmountLocal,
                GRNDetails.TaxClassValueRef,
                GRNDetails.Transferred,
                GRNDetails.TransferLink,
                GRNDetails.StockType,
                GRNDetails.OptimisticLockField,
                GRNDetails.WTaxCode,
                GRNDetails.WTaxRate,
                GRNDetails.WTaxAmount,
                GRNDetails.WTaxAmountLocal,
                GRNDetails.CostA,
                GRNDetails.CostB,
                GRNDetails.CostC,
                GRNDetails.CostD,
                GRNDetails.SgstAmount,
                GRNDetails.CgstAmount,
                GRNDetails.IgstAmount,
                GRNDetails.SgstAmountLocal,
                GRNDetails.CgstAmountLocal,
                GRNDetails.IgstAmountLocal,
                GRNDetails.CreditorItemId,
                GRNDetails.StockBatchNumberId,
                GRNDetails.Ref,
                GRNDetails.Ref2,
                GRNDetails.Ref3,
                GRNDetails.Ref4,
                GRNDetails.Ref5,
                GRNDetails.DateRef1,
                GRNDetails.DateRef2,
                GRNDetails.NumRef1,
                GRNDetails.NumRef2,
                GRNDetails.SerialNumber,
                GRNDetails.SerialNumberRef1,
                GRNDetails.SerialNumberRef2,
                GRNDetails.LocalOneCentsDiff,
                GRNDetails.ServiceAmount,
                GRNDetails.ServiceAmountLocal,
                GRNDetails.SubcodeId,
                GRNDetails.CommissionGroupId,
                GRNDetails.PackingUOMId,
                GRNDetails.Packing,
                GRNDetails.PackingQty,
                GRNDetails.CancelledSerialNumber,
                GRNDetails.PackingUOM1,
                GRNDetails.PackingUOM2,
                GRNDetails.HighestUOM,
                GRNDetails.RefTransferFrom,
                GRNDetails.DimensionId,
                GRNDetails.TaxExemptNo,
                GRNDetails.TaxExemptDetails,
                GRNDetails.MarkupDiscountAmount,
                GRNDetails.MarkupDiscountAmountLocal,
                GRNDetails.AmountTaxExempted,
                GRNDetails.AmountTaxExemptedLocal,
                GRNDetails.MarkupDiscountPrice
            FROM GoodsReceivedNoteDetails GRNDetails
            LEFT JOIN UOMs ON GRNDetails.UOMId = UOMs.Id
            WHERE GRNDetails.GoodsReceivedNoteId = @GRNId
              AND (GRNDetails.Qty - ISNULL(GRNDetails.Transferred / UOMs.Rate, 0) > 0)
            ORDER BY GRNDetails.Pos ASC";

            return await connection.QueryAsync<GoodsReceivedNotesDetails>(query, new { GRNId });
        }




    }
}
