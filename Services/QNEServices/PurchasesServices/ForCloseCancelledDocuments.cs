using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.PurchasesServices;

public class ForCloseCancelledDocuments
{

    private readonly QneConnectionString qneConnection;

    public ForCloseCancelledDocuments(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }

    public async Task<string> UpdateForCloseOrCancelled(ForCloseCancelledDocumentsModel model)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
   

        try
        {
            var column = model.SwitchName switch
            {
                "IsClosed" => "IsClosed",
                "IsCancelled" => "IsCancelled",
                _ => throw new InvalidOperationException("Invalid switch.")
            };

            var value = model.SwitchName == "IsClosed"
                ? model.IsClosed
                : model.IsCancelled;

            var sql = $"""
        UPDATE {model.TableName}
        SET {column} = @Value
        WHERE Id = @Id
        """;

            var rows = await connection.ExecuteAsync(sql, new
            {
                model.Id,
                Value = value
            });
            return $"Success ({rows}) updated!";
        }
        catch (SqlException ex)
        {
           
            return $"{ex}";
            throw;
        }
    }
    public async Task<string> UpdateForCloseOrCancelledOld(ForCloseCancelledDocumentsModel forCloseCancelledDocumentsModel)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();


        var sqlIsClosed = @$"UPDATE {forCloseCancelledDocumentsModel.TableName} SET IsClosed = @IsClosed WHERE Id = @Id";
        var sqlIsCancelled = @$"UPDATE {forCloseCancelledDocumentsModel.TableName} SET IsCancelled = @IsCancelled  WHERE Id = @Id";

        var p = new DynamicParameters();
        p.Add("@Id", forCloseCancelledDocumentsModel.Id);
        p.Add("@IsClosed", forCloseCancelledDocumentsModel.IsClosed);
        p.Add("@IsCancelled", forCloseCancelledDocumentsModel.IsCancelled);

        try
        {
            if (forCloseCancelledDocumentsModel.SwitchName == "IsCancelled")
            {
                var data = await connection.ExecuteAsync(sqlIsCancelled, p, transaction);
                transaction.Commit();
                return $"Success {data} added!";
            }
            else
            {
                var data = await connection.ExecuteAsync(sqlIsClosed, p, transaction);
                transaction.Commit();
                return $"Success {data} added!";
            }

        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"{ex}";
            throw;
        }
    }
}