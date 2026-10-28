using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.AllBasicServices;

public class CheckExistServices
{

    private readonly QneConnectionString qneConnection;

    public CheckExistServices(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }
    public async Task<bool> RecordExistsByIdAsync(CheckExistModel model)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        //var query = $"SELECT COUNT(1) FROM {model.TableName} WHERE Id = @Id";
        var query = $@"
IF EXISTS (
    SELECT 1
    FROM {model.TableName}
    WHERE Id = @Id
)
    SELECT CAST(1 AS BIT);
ELSE
    SELECT CAST(0 AS BIT);";
        var count = await connection.ExecuteScalarAsync<int>(query, new { Id = model.Id });
        return count > 0;
    }

    public async Task<bool> GetExistDocCode(CheckExistModel model)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = $"SELECT COUNT(1) FROM {model.TableName} WHERE {model.ColumnName} = @DocCode";
        var count = await connection.ExecuteScalarAsync<int>(query, new { DocCode = model.DocCode });
        return count > 0;
    }
}