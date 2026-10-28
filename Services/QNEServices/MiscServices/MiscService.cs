using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.CustomerModels;
using Accounting_V4.Model.QNEModels.MiscModels;
using Accounting_V4.Services.QNEServices.GLAccountServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.MiscServices;

public class MiscService
{
    private readonly QneConnectionString qneConnection;

    public MiscService(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }

    //GET LIST OF TERMS FROM db.Terms
    public async Task<IEnumerable<Terms>> GetTerms()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Terms ORDER BY Term ASC";
            var model = await connection.QueryAsync<Terms>(query, commandType: CommandType.Text);
            return model;
        

    }


    public async Task<IEnumerable<Areas>> GetAreas()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Areas WHERE IsActive != 0 ORDER BY AreaCode ASC";
        var model = await connection.QueryAsync<Areas>(query, commandType: CommandType.Text);
        return model;

    }
}
