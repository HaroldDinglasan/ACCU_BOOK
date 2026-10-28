using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.MaintenanceModels;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Accounting_V4.Services.QNEServices.MaintenanceServices
{
    public class ValidPostingServices
    {
        private readonly QneConnectionString qneConnection;

        public ValidPostingServices(QneConnectionString qneConnection)
        {
            this.qneConnection = qneConnection;
        }

        // GET VALID POSTING LIST
        public async Task<IEnumerable<ValidPostingModel>> GetValidPostList()
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();
            var connection = new SqlConnection(connectionString);
            var query = @"SELECT * FROM ValidPostingPeriods ORDER BY UserName ASC";
            var model = await connection.QueryAsync<ValidPostingModel>(query, commandType: CommandType.Text);
            return model;
        }

        // GET USER LIST
        public async Task<IEnumerable<SecuritySystemUser>> GetUserList()
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();
            var connection = new SqlConnection(connectionString);
            var query = @"SELECT * FROM SecuritySystemUser ORDER BY UserName ASC";
            var model = await connection.QueryAsync<SecuritySystemUser>(query, commandType: CommandType.Text);
            return model;
        }


    }
}
