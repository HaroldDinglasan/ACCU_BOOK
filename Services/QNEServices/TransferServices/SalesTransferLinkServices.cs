using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.DeliveryOrderModels;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.TransferServices;

public class SalesTransferLinkServices
{
    private readonly QneConnectionString qneConnection;
    private readonly TransferTable transferTable;

    public SalesTransferLinkServices(QneConnectionString qneConnection, TransferTable transferTable)
    {
        this.qneConnection = qneConnection;
        this.transferTable = transferTable;
    }

    public async Task<IEnumerable<QuotationDetails>> GetQuotationDetailsByIdFortransfer(string QTId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT 
            QD.Id,
            QD.Qty AS QuotationDetailsQty,
            (QD.Qty - ISNULL(QD.Transferred / UOMs.Rate, 0)) AS Qty,

            QD.UOMId,
            QD.UnitPrice,
            QD.Discount,
            QD.SalesTax,
            QD.QuotationId,
            QD.StockId,
            QD.Pos,
            QD.Description,
            QD.ProjectId,
            QD.ReferenceNo,
            QD.Amount,
            QD.DepartmentId,
            QD.Desc2,
            QD.Numbering,
            QD.StockLocationId,
            QD.CostCentreId,
            QD.AmountLocal,
            QD.DiscountAmount,
            QD.DiscountAmountLocal,
            QD.NetAmount,
            QD.NetAmountLocal,
            QD.Note,
            QD.ItemTypeCode,
            QD.IsBundled,
            QD.IsSubItem,
            QD.BundledTransactionId,
            QD.TaxCodeId,
            QD.IsTaxInclusive,
            QD.TaxRate,
            QD.TaxClass,
            QD.TaxClassValue,
            QD.TaxAmount,
            QD.TaxAmountLocal,
            QD.SubAmount,
            QD.SubAmountLocal,
            QD.TaxExclusiveAmount,
            QD.TaxExclusiveAmountLocal,
            QD.TaxClassValueRef,
            QD.Transferred,
            QD.MSUnitCost,
            QD.MSReferenceID,
            QD.OptimisticLockField,
            QD.WTaxCode,
            QD.WTaxRate,
            QD.WTaxAmount,
            QD.WTaxAmountLocal,
            QD.CancelledQty,
            QD.SgstAmount,
            QD.CgstAmount,
            QD.IgstAmount,
            QD.SgstAmountLocal,
            QD.CgstAmountLocal,
            QD.IgstAmountLocal,
            QD.DebtorItemId,
            QD.Ref,
            QD.Ref2,
            QD.Ref3,
            QD.Ref4,
            QD.Ref5,
            QD.DateRef1,
            QD.DateRef2,
            QD.NumRef1,
            QD.NumRef2,
            QD.TariffCodeId,
            QD.LocalOneCentsDiff,
            QD.ServiceAmount,
            QD.ServiceAmountLocal,
            QD.SubcodeId,
            QD.CommissionGroupId,
            QD.ServiceCost,
            QD.PackingUOMId,
            QD.Packing,
            QD.PackingQty,
            QD.PackingUOM1,
            QD.PackingUOM2,
            QD.HighestUOM,
            QD.ProgressInvoicingRate,
            QD.DimensionId,
            QD.TaxExemptNo,
            QD.TaxExemptDetails,
            QD.MarkupDiscountAmount,
            QD.MarkupDiscountAmountLocal,
            QD.AmountTaxExempted,
            QD.AmountTaxExemptedLocal,
            QD.MarkupDiscountPrice,
            QD.StoredPoint,
            QD.StoredPointQty

        FROM QuotationDetails QD
        LEFT JOIN UOMs ON UOMs.Id = QD.UOMId

        WHERE 
            QD.QuotationId = @QTId
        
        AND (
        QD.Description LIKE '%SUBTOTAL%'
		OR
		QD.Description LIKE '%DISCOUNT%'
		OR
		QD.StockId IS NULL
        OR
        (QD.Qty - ISNULL(QD.Transferred / UOMs.Rate, 0) > 0) )


        ORDER BY QD.Pos ASC";
        var model = await connection.QueryAsync<QuotationDetails>(query, new { QTId });
        return model;
    }


    public async Task<IEnumerable<SalesOrderDetails>> GetSalesOrderDetailsByIdFortransfer(string SOId)
    {

        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"  SELECT 
  SOD.ItemTypeCode,
            SOD.Id,
            SOD.Qty AS QuotationDetailsQty,
			(SOD.Qty - ISNULL(SOD.Transferred / UOMs.Rate, 0)) AS Qty,
            SOD.UOMId,
            SOD.UnitPrice,
            SOD.Discount,
            SOD.SalesTax,
            SOD.SalesOrderId,
            SOD.StockId,
            SOD.Pos,
            SOD.Description,
            SOD.ProjectId,
            SOD.ReferenceNo,
            SOD.Amount,
            SOD.DepartmentId,
            SOD.Numbering,
            SOD.StockLocationId,
            SOD.CostCentreId,
            SOD.AmountLocal,
            SOD.DiscountAmount,
            SOD.DiscountAmountLocal,
            SOD.NetAmount,
            SOD.NetAmountLocal,
            SOD.Note,
            SOD.ItemTypeCode,
            SOD.IsBundled,
            SOD.IsSubItem,
            SOD.BundledTransactionId,
            SOD.TaxCodeId,
            SOD.IsTaxInclusive,
            SOD.TaxRate,
            SOD.TaxClass,
            SOD.TaxClassValue,
            SOD.TaxAmount,
            SOD.TaxAmountLocal,
            SOD.SubAmount,
            SOD.SubAmountLocal,
            SOD.TaxExclusiveAmount,
            SOD.TaxExclusiveAmountLocal,
            SOD.TaxClassValueRef,
            SOD.Transferred,
            SOD.MSUnitCost,
            SOD.MSReferenceID,
            SOD.OptimisticLockField,
            SOD.WTaxCode,
            SOD.WTaxRate,
            SOD.WTaxAmount,
            SOD.WTaxAmountLocal,
            SOD.CancelledQty,
            SOD.SgstAmount,
            SOD.CgstAmount,
            SOD.IgstAmount,
            SOD.SgstAmountLocal,
            SOD.CgstAmountLocal,
            SOD.IgstAmountLocal,
            SOD.DebtorItemId,
            SOD.Ref,
            SOD.Ref2,
            SOD.Ref3,
            SOD.Ref4,
            SOD.Ref5,
            SOD.DateRef1,
            SOD.DateRef2,
            SOD.NumRef1,
            SOD.NumRef2,
            SOD.TariffCodeId,
            SOD.LocalOneCentsDiff,
            SOD.ServiceAmount,
            SOD.ServiceAmountLocal,
            SOD.SubcodeId,
            SOD.CommissionGroupId,
            SOD.ServiceCost,
            SOD.PackingUOMId,
            SOD.Packing,
            SOD.PackingQty,
            SOD.PackingUOM1,
            SOD.PackingUOM2,
            SOD.HighestUOM,
            SOD.ProgressInvoicingRate,
            SOD.DimensionId,
            SOD.TaxExemptNo,
            SOD.TaxExemptDetails,
            SOD.MarkupDiscountAmount,
            SOD.MarkupDiscountAmountLocal,
            SOD.AmountTaxExempted,
            SOD.AmountTaxExemptedLocal,
            SOD.MarkupDiscountPrice,
            SOD.StoredPoint,
            SOD.StoredPointQty

        FROM SalesOrderDetails SOD
		        LEFT JOIN UOMs ON UOMs.Id = SOD.UOMId
        WHERE 
            SOD.SalesOrderId = @SOId

    AND (
        SOD.Description LIKE '%SUBTOTAL%'
		OR
		SOD.Description LIKE '%DISCOUNT%'
		OR
		SOD.StockId IS NULL
        OR
        (SOD.Qty - ISNULL(SOD.Transferred / UOMs.Rate, 0) > 0)
    )
        ORDER BY SOD.Pos ASC";

        var model = await connection.QueryAsync<SalesOrderDetails>(query, new { SOId });
        return model;
    }


    public async Task<IEnumerable<DeliveryOrderDetails>> GetDeliveryOrderDetailsByIdFortransfer(string DOId)
    {

        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT 
  DOD.ItemTypeCode,
            DOD.Id,
            DOD.Qty AS QuotationDetailsQty,
			(DOD.Qty - ISNULL(DOD.Transferred / UOMs.Rate, 0)) AS Qty,
          DOD.UOMId,
            DOD.UnitPrice,
            DOD.Discount,
            DOD.SalesTax,
            DOD.StockLocationId,
            DOD.FromType,
            DOD.FromDocId,
            DOD.FromDtlId,
            IsPartialTransfer,
            DOD.DeliveryOrderId,
            DOD.StockId,
            DOD.Pos,
            DOD.Description,
            DOD.ProjectId,
            DOD.ReferenceNo,
            DOD.Amount,
            DOD.DepartmentId,
            DOD.Numbering,
            DOD.CostCentreId,
            DOD.AmountLocal,
            DOD.DiscountAmount,
            DOD.DiscountAmountLocal,
            DOD.NetAmount,
            DOD.NetAmountLocal,
            DOD.Note,
            DOD.ItemTypeCode,
            DOD.IsBundled,
            DOD.IsSubItem,
            DOD.BundledTransactionId,
            DOD.TaxCodeId,
            DOD.IsTaxInclusive,
            DOD.TaxRate,
            DOD.TaxClass,
            DOD.TaxClassValue,
            DOD.TaxAmount,
            DOD.TaxAmountLocal,
            DOD.SubAmount,
            DOD.SubAmountLocal,
            DOD.TaxExclusiveAmount,
            DOD.TaxExclusiveAmountLocal,
            DOD.TaxClassValueRef,
            DOD.TransferLink,
            DOD.Transferred,
            DOD.MSUnitCost,
            DOD.MSReferenceID,
            DOD.OptimisticLockField,
            DOD.WTaxCode,
            DOD.WTaxRate,
            DOD.WTaxAmount,
            DOD.WTaxAmountLocal,
            DOD.CancelledQty,
            DOD.SgstAmount,
            DOD.CgstAmount,
            DOD.IgstAmount,
            DOD.SgstAmountLocal,
            DOD.CgstAmountLocal,
            DOD.IgstAmountLocal,
            DOD.DebtorItemId,
            DOD.StockBatchNumberId,
            DOD.Ref,
            DOD.Ref2,
            DOD.Ref3,
            DOD.Ref4,
            DOD.Ref5,
            DOD.DateRef1,
            DOD.DateRef2,
            DOD.NumRef1,
            DOD.NumRef2,
            DOD.TariffCodeId,
            DOD.LocalOneCentsDiff,
            DOD.ServiceAmount,
            DOD.ServiceAmountLocal,
            DOD.SubcodeId,
            DOD.CommissionGroupId,
            DOD.ServiceCost,
            DOD.PackingUOMId,
            DOD.Packing,
            DOD.PackingQty,
            DOD.PackingUOM1,
            DOD.PackingUOM2,
            DOD.HighestUOM,
            DOD.ProgressInvoicingRate,
            DOD.CancelledSerialNumber,
            DOD.RefTransferFrom,
            DOD.DimensionId,
            DOD.TaxExemptNo,
            DOD.TaxExemptDetails,
            DOD.MarkupDiscountAmount,
            DOD.MarkupDiscountAmountLocal,
            DOD.AmountTaxExempted,
            DOD.AmountTaxExemptedLocal,
            DOD.MarkupDiscountPrice,
            DOD.StoredPoint,
            DOD.StoredPointQty

        FROM DeliveryOrderDetails DOD
		        LEFT JOIN UOMs ON UOMs.Id = DOD.UOMId
        WHERE 
            DOD.DeliveryOrderId = @DOId

    AND (
        DOD.Description LIKE '%SUBTOTAL%'
		OR
		DOD.Description LIKE '%DISCOUNT%'
		OR
		DOD.StockId IS NULL
        OR
        (DOD.Qty - ISNULL(DOD.Transferred / UOMs.Rate, 0) > 0)
    )
        ORDER BY DOD.Pos ASC";

        var model = await connection.QueryAsync<DeliveryOrderDetails>(query, new { DOId });
        return model;
    }




    /// <summary>
    /// LIST OF SALES MODULE FOR TRANSFER > Quotation, SalesOrder, DeliveryOrder, SalesInvoice

    public async Task<IEnumerable<SalesTransferList>> GetSalesTransferList(string DebtorId, string TableName)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);

        var config = transferTable.GetModuleTableConfig(TableName);

        if (config == null)
            return Enumerable.Empty<SalesTransferList>();

        var query = $@"
        SELECT DISTINCT 
            H.Id, 
            H.{config.DateField} AS DocDate, 
            H.{config.CodeField} AS DocCode, 
            H.DebtorId, 
            H.DebtorName, 
            SALES.StaffCode AS Agent, 
            TERMS.Term, 
            H.TaxTotalAmount, 
            ISNULL(H.NetTotalAmount, 0) - ISNULL(H.WTaxTotalAmount, 0) AS NetTotalAmount,
            H.IsCancelled, 
            H.IsClosed, 
            H.IsApproved
        FROM {config.HeaderTable} H

        INNER JOIN {config.DetailsTable} D ON H.Id = D.{config.HeaderId}
        LEFT JOIN Terms TERMS ON H.TermId = TERMS.Id
        LEFT JOIN SalesPersons SALES ON H.SalesPersonId = SALES.Id
        LEFT JOIN UOMs U ON D.UOMId = U.Id

        WHERE 
           (H.DebtorId = @DebtorId OR H.DebtorId IS NULL)
           AND ISNULL(H.IsClosed, 0) = 0
            AND ISNULL(H.IsCancelled, 0) = 0
            AND ISNULL(H.IsApproved, 1) = 1
            AND D.UOMId IS NOT NULL
            AND ISNULL(D.ItemTypeCode, '') NOT IN ('S', 'D', 'T')
			AND(D.Qty - ISNULL(D.Transferred / U.Rate,0)>0)     

        ORDER BY H.{config.CodeField} ASC";

        return await connection.QueryAsync<SalesTransferList>(query, new { DebtorId });
    }



    public async Task<List<Guid>> InsertSalesTransferLink(List<ForTransferDetails> forTransferDetails)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();


        var insertQuery = @"
        INSERT INTO  SalesTransferLink
        (
            Id,
            TransferredBaseUnit,
            SourceKey,
            QuotationDetail,
            SalesOrderDetail,
            DeliveryOrderDetail,
            OptimisticLockField
        )
        OUTPUT INSERTED.Id
        VALUES
        (
            @Id,
            @TransferredBaseUnit,
            @SourceKey,
            @QuotationDetail,
            @SalesOrderDetail,
            @DeliveryOrderDetail,
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
                        QuotationDetail = transferLink.QuotationDetailId,
                        SalesOrderDetail = transferLink.SalesOrderDetailId,
                        DeliveryOrderDetail = transferLink.DeliveryOrderDetailId,
                        OptimisticLockField = transferLink.OptimisticLockField
                    }, transaction);

                transferLink.TransferLink = insertedId;
                insertedIds.Add(insertedId);
            }

            await transaction.CommitAsync();
            return insertedIds;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}



// public async Task<IEnumerable<SalesTransferList>> GetSalesTransferList(string DebtorId, string TableName)
//    {
//        var connectionString = await qneConnection.GetConnectionStringAsync();
//        var connection = new SqlConnection(connectionString);
//        string query = string.Empty;

//        switch (TableName)
//        {

//            case "QuotationDetails":
//                query = @"SELECT DISTINCT 
//    QT.Id, 
//    QT.QuotationDate AS DocDate, 
//    QT.QuotationCode AS DocCode, 
//    QT.DebtorId, 
//    QT.DebtorName, 
//    SALES.StaffCode AS Agent, 
//    TERMS.Term, 
//    QT.TaxTotalAmount, 
//    QT.NetTotalAmount, 
//    QT.IsCancelled, 
//    QT.IsClosed, 
//    QT.IsApproved
//FROM Quotations QT

//LEFT JOIN Terms TERMS ON QT.TermId = TERMS.Id
//LEFT JOIN SalesPersons SALES ON QT.SalesPersonId = SALES.Id

//INNER JOIN QuotationDetails QTD 
//    ON QT.Id = QTD.QuotationId

//LEFT JOIN (
//    SELECT SourceKey, SUM(TransferredBaseUnit) AS TotalTransferred 
//    FROM SalesTransferLink
//    GROUP BY SourceKey
//) PTL 
//    ON CAST(QTD.Id AS VARCHAR(50)) = PTL.SourceKey 

//WHERE 
//    QT.DebtorId = @DebtorId
//    AND QT.IsClosed != 1
//    AND QT.IsCancelled != 1
//    AND ISNULL(QTD.Qty, 0) > 0
//    AND ISNULL(QTD.ItemTypeCode, '') NOT IN ('S', 'D', 'T')
//    AND (
//        PTL.SourceKey IS NULL
//        OR (QTD.Qty - ISNULL(PTL.TotalTransferred, 0)) > 0
//    )
//ORDER BY QT.QuotationDate ASC";
//                break;


//            case "SalesOrderDetails":
//                query = @"";
//                break;

//            case "DeliveryOrderDetails":
//                query = @"SELECT DISTINCT 
//    DO.Id, 
//    DO.DeliveryOrderDate AS DocDate, 
//    DO.DeliveryOrderCode AS DocCode, 
//    DO.DebtorId, 
//    DO.DebtorName, 
//    SALES.StaffCode AS Agent, 
//    TERMS.Term, 
//    DO.TaxTotalAmount, 
//    DO.NetTotalAmount, 
//    DO.IsCancelled, 
//    DO.IsClosed, 
//    DO.IsApproved
//FROM DeliveryOrders DO

//LEFT JOIN Terms TERMS ON DO.TermId = TERMS.Id
//LEFT JOIN SalesPersons SALES ON DO.SalesPersonId = SALES.Id

//INNER JOIN DeliveryOrderDetails DOD 
//    ON DO.Id = DOD.DeliveryOrderId

//LEFT JOIN (
//    SELECT SourceKey, SUM(TransferredBaseUnit) AS TotalTransferred 
//    FROM SalesTransferLink
//    GROUP BY SourceKey
//) PTL 
//    ON CAST(DOD.Id AS VARCHAR(50)) = PTL.SourceKey 

//WHERE 
//    DO.DebtorId = @DebtorId
//    AND DO.IsClosed != 1
//    AND DO.IsCancelled != 1
//    AND ISNULL(DOD.Qty, 0) > 0
//    AND ISNULL(DOD.ItemTypeCode, '') NOT IN ('S', 'D', 'T')
//    AND (
//        PTL.SourceKey IS NULL
//        OR (DOD.Qty - ISNULL(PTL.TotalTransferred, 0)) > 0
//    )
//ORDER BY DO.DeliveryOrderDate ASC";
//                break;

//            case "SalesInvoiceDetails":
//                query = @"";
//                break;


//            default:
//                query = string.Empty;
//                break;
//        }



//        var model = await connection.QueryAsync<SalesTransferList>(query, new { DebtorId });
//        return model;
//    }