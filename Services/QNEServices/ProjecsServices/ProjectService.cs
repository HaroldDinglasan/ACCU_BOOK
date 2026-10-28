using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.ProjectModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.ProjecsServices;

public class ProjectService
{
    private readonly QneConnectionString qneConnection;

    public ProjectService(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }
    public async Task<IEnumerable<Projects>> GetProjectsLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Projects WHERE IsActive != 0 ORDER BY ProjectCode ASC";
        var model = await connection.QueryAsync<Projects>(query, commandType: CommandType.Text);
        return model;

    }
}
