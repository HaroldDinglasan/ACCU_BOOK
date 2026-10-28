using System.Data;
using Accounting_V4.ConnectionServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.OptimisticLockFieldServices;

public class OptimisticLockField
{


    private readonly QneConnectionString qneConnection;

    public OptimisticLockField(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;

    }

    public async Task<int> UpdateOptimisticLockField(string table, Guid Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = $"UPDATE {table} SET OptimisticLockField = OptimisticLockField + 1 WHERE Id = @Id";

        var rowsAffected = await connection.ExecuteAsync(query, new { Id });
        return rowsAffected;

    }
}
