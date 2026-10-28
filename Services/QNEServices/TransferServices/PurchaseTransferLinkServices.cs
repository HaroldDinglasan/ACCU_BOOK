using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Accounting_V4.Pages.QNEPages.Purchases.PurchaseOrder;
using Dapper;
using Microsoft.Data.SqlClient;
using static System.Collections.Specialized.BitVector32;

namespace Accounting_V4.Services.QNEServices.TransferServices;

public class PurchaseTransferLinkServices
{
    private readonly QneConnectionString qneConnection;
    private readonly TransferTable transferTable;

    public PurchaseTransferLinkServices(QneConnectionString qneConnection, TransferTable transferTable)
    {
        this.qneConnection = qneConnection;
        this.transferTable = transferTable;
    }

    /// <summary>
    /// Get Purchase Requisition List for Transfer
    /// </summary>
    /// <param name="CreditorId"></param>
    /// <returns></returns>
    //public async Task<IEnumerable<PurchasesTranferList>> GetPurchasesTranferLists(string CreditorId, string TableName)
    //{
    //    var connectionString = await qneConnection.GetConnectionStringAsync();
    //    var connection = new SqlConnection(connectionString);
    //    var config = transferTable.GetModuleTableConfig(TableName);

    //    if (config == null)
    //        return Enumerable.Empty<PurchasesTranferList>();

    //    var query = $@"SELECT DISTINCT 
    //        H.Id, 
    //        H.{config.DateField} AS DocDate, 
    //        H.{config.CodeField} AS DocCode, 
    //        H.CreditorId, 
    //        H.CreditorName, 
    //        PURCH.StaffCode AS Purchaser, 
    //        PURCH.Name AS RequestBy, 
    //        TERMS.Term, 
    //        H.TaxTotalAmount, 
    //        H.NetTotalAmount, 
    //        H.IsCancelled, 
    //        H.IsClosed, 
    //        H.IsApproved
    //    FROM {config.HeaderTable} H

    //    LEFT JOIN Terms TERMS ON H.TermId = TERMS.Id
    //    LEFT JOIN Purchasers PURCH ON H.PurchaserId = PURCH.Id

    //  INNER JOIN {config.DetailsTable} D 
    //        ON H.Id = D.{config.HeaderId}

    //    LEFT JOIN (
    //        SELECT SourceKey, SUM(TransferredBaseUnit) AS TotalTransferred 
    //        FROM PurchaseTransferLink
    //        GROUP BY SourceKey
    //    ) PTL 
    //        ON CAST(D.Id AS VARCHAR(50)) = PTL.SourceKey 

    //    WHERE 
    //        H.CreditorId = @CreditorId OR H.CreditorId IS NULL
    //        AND H.IsClosed != 1
    //        AND H.IsCancelled != 1
    //        AND ISNULL(D.Qty, 0) > 0
    //        AND ISNULL(D.ItemTypeCode, '') NOT IN ('S', 'D', 'T')
    //        AND (
    //            PTL.SourceKey IS NULL
    //            OR (D.Qty - ISNULL(PTL.TotalTransferred, 0)) > 0
    //        )
    //    ORDER BY H.{config.CodeField} ASC";

    //    return await connection.QueryAsync<PurchasesTranferList>(query, new { CreditorId });
    //    //return model;
    //}

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
            H.TaxExclusiveTotalAmountLocal,
            H.TaxTotalAmountLocal, 
			H.TaxTotalAmount,
            ISNULL(H.NetTotalAmount, 0) - ISNULL(H.WTaxTotalAmount, 0) AS NetTotalAmount, 
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
			AND(D.Qty - ISNULL(D.Transferred / U.Rate,0)>0)       


         ORDER BY H.{config.CodeField} ASC";
        return await connection.QueryAsync<PurchasesTranferList>(query, new { CreditorId });
    }


    public async Task<List<Guid>> InsertPurchaseTransferLink(List<ForTransferDetails> forTransferDetails)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        //  var TableName = "PurchaseTransferLink";

        var insertQuery = @"
        INSERT INTO  PurchaseTransferLink
        (
            Id,
            TransferredBaseUnit,
            SourceKey,
            PurchaseRequisitionDetail,
            PurchaseOrderDetail,
            GoodsReceivedNoteDetail,
            OptimisticLockField
        )
        OUTPUT INSERTED.Id
        VALUES
        (
            @Id,
            @TransferredBaseUnit,
            @SourceKey,
            @PurchaseRequisitionDetail,
            @PurchaseOrderDetail,
            @GoodsReceivedNoteDetail,
            @OptimisticLockField
        )";

        var insertedIds = new List<Guid>();

        try
        {
            foreach (var transferLink in forTransferDetails)
            {
                var insertedId = await connection.ExecuteScalarAsync<Guid>(insertQuery,
                    new
                    {
                        Id = transferLink.Id,
                        TransferredBaseUnit = transferLink.TransferredBaseUnit,
                        SourceKey = transferLink.SourceKey,
                        PurchaseRequisitionDetail = transferLink.PurchaseRequisitionDetailId,
                        PurchaseOrderDetail = transferLink.PurchaseOrderDetailId,
                        GoodsReceivedNoteDetail = transferLink.GoodsReceivedNoteDetailId,
                        OptimisticLockField = transferLink.OptimisticLockField
                    }, transaction);

                transferLink.TransferLink = insertedId;
                insertedIds.Add(insertedId);
            }

            await transaction.CommitAsync();
            return insertedIds;
        }
        catch (SqlException)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }



    //FOR TRANSFER FUNCTION SERVICES
    //GET THE PR DETAILS THAT HAS NOT YET TRANSFER OR PARTIAL TRANSFERED QTY
    public async Task<IEnumerable<PurchaseRequisitionDetails>> GetPurchaseRequisitionsDetailByIdFortransfer(string PRId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT 
            PRDetails.Id,
            PRDetails.PurchaseRequisitionId,
            PRDetails.Qty AS PurchaseRequisitionDetailsQty,
           (PRDetails.Qty-ISNULL(PRDetails.Transferred / UOMs.Rate,0)) AS Qty,
            PRDetails.Pos,
            PRDetails.Numbering,
            PRDetails.StockId,
            PRDetails.Description,
            PRDetails.ProjectId,
            PRDetails.DepartmentId,
            PRDetails.StockLocationId,


            PRDetails.UOMId,
            PRDetails.UnitPrice,
            PRDetails.Discount,
            PRDetails.Amount,
            PRDetails.RequireDate,
            PRDetails.SupplierItemCode,
            PRDetails.ReferenceNo,
            PRDetails.CancelledQty,
            PRDetails.CostCentreId,
            PRDetails.AmountLocal,
            PRDetails.DiscountAmount,
            PRDetails.DiscountAmountLocal,
            PRDetails.NetAmount,
            PRDetails.NetAmountLocal,
            PRDetails.Note,
            PRDetails.ItemTypeCode,
            PRDetails.IsBundled,
            PRDetails.IsSubItem,
            PRDetails.BundledTransactionId,
            PRDetails.TaxCodeId,
            PRDetails.IsTaxInclusive,
            PRDetails.TaxRate,
            PRDetails.TaxClass,
            PRDetails.TaxClassValue,
            PRDetails.TaxAmount,
            PRDetails.TaxAmountLocal,
            PRDetails.SubAmount,
            PRDetails.SubAmountLocal,
            PRDetails.TaxExclusiveAmount,
            PRDetails.TaxExclusiveAmountLocal,
            PRDetails.TaxClassValueRef,
            PRDetails.Transferred,
            PRDetails.OptimisticLockField,
            PRDetails.WTaxCode,
            PRDetails.WTaxRate,
            PRDetails.WTaxAmount,
            PRDetails.WTaxAmountLocal,
            PRDetails.SgstAmount,
            PRDetails.CgstAmount,
            PRDetails.IgstAmount,
            PRDetails.SgstAmountLocal,
            PRDetails.CgstAmountLocal,
            PRDetails.IgstAmountLocal,
            PRDetails.CreditorItemId,
            PRDetails.Ref,
            PRDetails.Ref2,
            PRDetails.Ref3,
            PRDetails.Ref4,
            PRDetails.Ref5,
            PRDetails.DateRef1,
            PRDetails.DateRef2,
            PRDetails.NumRef1,
            PRDetails.NumRef2,
            PRDetails.TariffCodeId,
            PRDetails.LocalOneCentsDiff,
            PRDetails.ServiceAmount,
            PRDetails.ServiceAmountLocal,
            PRDetails.SubcodeId,
            PRDetails.CommissionGroupId,
            PRDetails.PackingUOMId,
            PRDetails.Packing,
            PRDetails.PackingQty,
            PRDetails.PackingUOM1,
            PRDetails.PackingUOM2,
            PRDetails.HighestUOM,
            PRDetails.SOTOPOTransferLink,
            PRDetails.TransferLink,
            PRDetails.RefSOTOPOTransferFrom,
            PRDetails.RefTransferFrom,
            PRDetails.DimensionId,
            PRDetails.TaxExemptNo,
            PRDetails.TaxExemptDetails,
            PRDetails.MarkupDiscountAmount,
            PRDetails.MarkupDiscountAmountLocal,
            PRDetails.AmountTaxExempted,
            PRDetails.AmountTaxExemptedLocal,
            PRDetails.MarkupDiscountPrice
        FROM PurchaseRequisitionDetails PRDetails
        LEFT JOIN UOMs ON PRDetails.UOMId = UOMs.Id
        WHERE PRDetails.PurchaseRequisitionId = @PRId
        AND (PRDetails.Qty-ISNULL(PRDetails.Transferred / UOMs.Rate,0) > 0)
        ORDER BY PRDetails.Pos ASC";

        var model = await connection.QueryAsync<PurchaseRequisitionDetails>(query, new { PRId });
        return model;
    }

    // GET ONE PURCHASE REQUISITION DETAILS DATA BY ID
    public async Task<PurchaseRequisitionModel> GetPurchaseRequisitionsHeaderIdFortransfer(string PRId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM PurchaseRequisitions WHERE Id = @PRId";
        var model = await connection.QueryFirstOrDefaultAsync<PurchaseRequisitionModel>(query, new { PRId });
        return model;
    }


    public async Task<IEnumerable<PurchaseOrderTransferList>> GetPurchaseOrderTransferLists(string CreditorId, string TableName)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var config = transferTable.GetModuleTableConfig(TableName);

        if (config == null)
            return Enumerable.Empty<PurchaseOrderTransferList>();

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
            ISNULL(H.NetTotalAmount, 0) - ISNULL(H.WTaxTotalAmount, 0) AS NetTotalAmount,
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
			AND(D.Qty - ISNULL(D.Transferred / U.Rate,0)>0)       


         ORDER BY H.{config.CodeField} ASC";
        return await connection.QueryAsync<PurchaseOrderTransferList>(query, new { CreditorId });
    }


}
