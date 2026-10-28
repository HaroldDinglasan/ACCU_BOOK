using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.MiscModels;
using Accounting_V4.Services.OptimisticLockFieldServices;
using Accounting_V4.Services.QNEServices.GLAccountServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.TermServices;

public class TermService
{

    private readonly QneConnectionString qneConnection;

    public TermService(QneConnectionString qneConnection)
    {

        this.qneConnection = qneConnection;

    }
    //GET LIST OF TERMS FROM db.Terms
    public async Task<IEnumerable<Terms>> GetTermsLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Terms ORDER BY Term ASC";
        var model = await connection.QueryAsync<Terms>(query, commandType: CommandType.Text);
        return model;


    }
}
