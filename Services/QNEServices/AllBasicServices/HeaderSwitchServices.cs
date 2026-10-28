using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.AllBasicServices;

public class HeaderSwitchServices
{

    private readonly QneConnectionString qneConnection;

    public HeaderSwitchServices(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }

    public async Task<string> UpdateHeaderSwitch(HeaderSwitchModel model)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();


        try
        {
            var column = model.SwitchName switch
            {
                "IsDeferredVAT" => "IsDeferredVAT",
                "IsDetailPosting" => "IsDetailPosting",
                _ => throw new InvalidOperationException("Invalid switch.")
            };

            var value = model.SwitchName == "IsDeferredVAT"
                ? model.IsDeferredVAT
                : model.IsDetailPosting;

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
}
