using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.AllBasicServices;

public class DeleteServices
{
    private readonly QneConnectionString qneConnection;
    public DeleteServices(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }

    public async Task<string> DeleteReceiptDetailRows(List<DeleteModel> deleteModels)
    {
        if (deleteModels == null || deleteModels.Count == 0)
            return "(0) rows deleted!";


        var HeaderId = deleteModels.Select(d => d.Id).Distinct().ToList();
        var DetailsIds = deleteModels.Select(d => d.DetailsId).Distinct().ToList();
        var forceDeleteIds = deleteModels.Where(d => d.DeleteDetailRow).Select(d => d.DetailsId).Distinct().ToList();


        var connectionString = await qneConnection.GetConnectionStringAsync();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();


        try
        {
            await connection.ExecuteAsync(
                "DELETE FROM BankReconciliation WHERE TransactionId IN @DetailsIds;",
                new
                {
                    DetailsIds
                }, transaction);

            await connection.ExecuteAsync(
                "DELETE FROM GLTransactions WHERE TransactionId IN @DetailsIds;",
                new
                {
                    DetailsIds
                }, transaction);


            //var deleted = await connection.ExecuteAsync(
            //    "DELETE FROM ReceiptDetails WHERE Id IN @DetailsIds;",
            //    new
            //    {
            //        DetailsIds
            //    }, transaction);

            var deleted = 0;
            if (forceDeleteIds.Any())
            {
                deleted = await connection.ExecuteAsync(
                    "DELETE FROM ReceiptDetails WHERE Id IN @forceDeleteIds;",
                    new
                    {
                        forceDeleteIds
                    }, transaction);
            }


            await connection.ExecuteAsync(
                "UPDATE Receipts SET OptimisticLockField =  OptimisticLockField + 1 WHERE Id LIKE @Id",
                new
                {
                    Id = HeaderId
                }, transaction);

            await transaction.CommitAsync();
            return $"({deleted}) rows deleted!";
        }
        catch (SqlException ex)
        {
            await transaction.RollbackAsync();
            return ex.Message;
        }

    }





    public async Task<string> DeleteDataOnDetailRows(DeleteModel deleteModel)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var deletequery = @$"DELETE FROM {deleteModel.TableName} WHERE Id=@Id";
        try
        {
            var deleteDetailRows = await connection.ExecuteAsync(deletequery, new { Id = deleteModel.Id }, transaction);
            transaction.Commit();
            return $"({deleteDetailRows}) rows deleted!";

        }
        catch (SqlException ex)
        {

            transaction.Rollback();
            return ex.Message;
            throw;
        }

    }

}
