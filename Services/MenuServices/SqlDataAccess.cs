using System.Data;
using Accounting_V4.ConnectionServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.MenuServices;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly string connectionString;
   // private readonly AppConnectionString appConnectionString;
   
    public SqlDataAccess(IConfiguration config,AppConnectionString _appConnectionString)
    {
        connectionString = config.GetConnectionString("DefaultConnectionString");
        //appConnectionString = _appConnectionString;
    }

    public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        var data = await connection.QueryAsync<T>(sql, parameters);
        return data.ToList();
    }

    public async Task<T> LoadSingleData<T>(string sql, object parameters = null)
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        var data = await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
        return data;
    }

    public async Task<int> SaveData<T>(string sql, T parameters)
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<int> SaveDataReturnId<T>(string sql, T parameters)
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        var result = await connection.ExecuteScalarAsync(sql, parameters);
        int NewId;
        if (result != null)
        {
            NewId = Convert.ToInt32(result);
        }
        else
        {
            NewId = 0;
        }
        return NewId;
    }

    public async Task SpSaveData<T>(string sql, T parameters)
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
    }
}
