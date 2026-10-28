using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.StocksForTransferModels;
using Accounting_V4.Model.QNEModels.StocksModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.TransferServices
{
    public class StocksTransferLinkServices
    {
        private readonly QneConnectionString qneConnection;

        public StocksTransferLinkServices(QneConnectionString qneConnection)
        {
            this.qneConnection = qneConnection;
        }

        public async Task<IEnumerable<StockAssemblyOrderDetail>> GetStockAssemblyOrderDetailsByIdForTransfer(string SAOId)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"
                SELECT
                    SAOD.Id,
                    SAOD.Qty AS StockAssemblyOrderDetailsQty,
                    (SAOD.Qty - ISNULL(SAOD.Transferred / UOMs.Rate, 0)) AS Qty,
                    SAOD.Numbering,
                    SAOD.StockId,
                    SAOD.Description,
                    SAOD.UOMId,
                    SAOD.DefaultQty,
                    SAOD.UnitCost,
                    SAOD.LabourCost,
                    SAOD.OverheadCost,
                    SAOD.StockLocationId,
                    SAOD.ProjectId,
                    SAOD.StockAssemblyOrderId,
                    SAOD.Pos,
                    SAOD.Ref1, SAOD.Ref2, SAOD.Ref3, SAOD.Ref4, SAOD.Ref5,
                    SAOD.Transferred,
                    SAOD.StockType,
                    SAOD.SerialNumber, SAOD.SerialNumberRef1, SAOD.SerialNumberRef2,
                    SAOD.StockBatchNumberId,
                    SAOD.OptimisticLockField,
                    SAOD.TransferLink
                FROM StockAssemblyOrderDetail SAOD
                LEFT JOIN UOMs ON UOMs.Id = SAOD.UOMId
                WHERE SAOD.StockAssemblyOrderId = @SAOId
                AND (
                    SAOD.StockId IS NULL
                    OR (SAOD.Qty - ISNULL(SAOD.Transferred / UOMs.Rate, 0) > 0)
                )
                ORDER BY SAOD.Pos ASC";
            var model = await connection.QueryAsync<StockAssemblyOrderDetail>(query, new { SAOId });
            return model;
        }

        public async Task<List<Guid>> InsertStocksTransferLink(List<StockAssemblyOrderForTransferDetails> stockAssemblyOrderForTransferDetails)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            await using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();

            var insertQuery = @"
            INSERT INTO StockTransferLink 
            (
                Id,
                TransferredBaseUnit,
                SourceKey,
                StockAssemblyOrderDetail,
                OptimisticLockField
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @Id,
                @TransferredBaseUnit,
                @SourceKey,
                @StockAssemblyOrderDetail,
                @OptimisticLockField
            )";

            var insertedIds = new List<Guid>();

            try
            {
                foreach (var transferLink in stockAssemblyOrderForTransferDetails)
                {
                    var insertedId = await connection.ExecuteScalarAsync<Guid>(insertQuery,
                        new
                        {
                            Id = transferLink.Id,
                            TransferredBaseUnit = transferLink.TransferredBaseUnit,
                            SourceKey = transferLink.SourceKey,
                            StockAssemblyOrderDetail = transferLink.StockAssemblyOrderDetailId,
                            OptimisticLockField = transferLink.OptimisticLockField
                        }, transaction);

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







}
