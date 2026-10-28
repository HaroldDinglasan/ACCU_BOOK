using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.DescriptionModels;
using Accounting_V4.Model.QNEModels.StocksModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.DescriptionServices;

public class DescriptionsService
{
    private readonly QneConnectionString qneConnection;

    public DescriptionsService(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }

    public async Task<IEnumerable<Descriptions>> GetDescriptions(DescriptionFilter filter)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT DISTINCT Description,
        OwnerCode,
        IsDefault,
        OptimisticLockField 
        FROM Descriptions
        WHERE OwnerCode = @OwnerCode";
        var p = new DynamicParameters();
        p.Add("@OwnerCode", filter.OwnerCode);
        var model = await connection.QueryAsync<Descriptions>(query, p);
        return model;
    }




}
