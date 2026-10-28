using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.CurrenciesModels;
using Accounting_V4.Services.OptimisticLockFieldServices;
using Accounting_V4.Services.QNEServices.GLAccountServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.CurrenciesServices;

public class CurrenciesService
{

    private readonly QneConnectionString qneConnection;


    public CurrenciesService(QneConnectionString qneConnection)
    {
       
        this.qneConnection = qneConnection;
    }

    public async Task<IEnumerable<Currencies>> GetCurrencies()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM Currencies";
        var model = await connection.QueryAsync<Currencies>(query, commandType: CommandType.Text);
        return model;

    }

}
