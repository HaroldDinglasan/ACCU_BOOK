using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.DescriptionModels;
using Accounting_V4.Model.QNEModels.TransactionCodesModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.TransactionCodeServices;

public class TransactionCodeService
{
    private readonly QneConnectionString qneConnection;

    public TransactionCodeService(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }

    public async Task<IEnumerable<TransactionsCode>> GetTransactionNo(TransactionCodesFilter transCodefilter)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @" SELECT Id
                      ,TableId
                      ,Name
                      ,Format
                      ,SuffixFillChar
                      ,NextNumber
                      ,IsDefault
                      ,DateType
                      ,ResetMonthly
                      ,UseProjectCode
                      ,OptimisticLockField
                      ,ModuleType
                      ,UseCounterCode
                      ,SeriesFrom
                      ,SeriesTo
                      ,DisplayRemarks1
                      ,Remarks1
                      ,DisplayRemarks2
                      ,Remarks2
                FROM TransactionCodes WHERE TableId = @TableId";

        var p = new DynamicParameters();
        p.Add("@TableId", transCodefilter.TableId);
        var model = await connection.QueryAsync<TransactionsCode>(query, p);
        return model;
    }
}
