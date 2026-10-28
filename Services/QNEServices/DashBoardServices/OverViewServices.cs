using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.DashBoardModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.DashBoardServices;

public class OverViewServices
{

    //TWO GL ACCOUNTS CODE FOR MONTHLY SUMMARY 
    //4-001-000-000-000 TO 4-005-000-000-000
    //SALES - IN HOUSE
    //SALES - APTHEALTH
    //SALES - OTC VET TOLLING
    //SALES - TOLLING
    //SALES - NUTRATECH

    private readonly QneConnectionString qneConnection;

    public OverViewServices(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }

    //MOTHLY SALES
    public async Task<IEnumerable<MonthlySales>> GetMonthlySalesResult(MothlySalesFilter filter)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        //GET ACCOUNT ID CONTAINS SALES OR INC DESCRIPTION
        var queryAccount = @"SELECT Id, AccountCode, Description FROM Accounts WHERE AccountCode = 'INC'";
        var accounts = await connection.QueryAsync<Accounts>(queryAccount);

        if (accounts == null || !accounts.Any())
        {
            throw new InvalidOperationException("No accounts found with the description 'SALES'. Please check if the Accounts table contains the correct data.");
        }

        var account = accounts.First();

        var queryGLAccount = @"SELECT GLAccountCode, AccountId, Description FROM GLAccounts WHERE AccountId = @AccountId";
        var pGL = new DynamicParameters();
        pGL.Add("@AccountId", account.Id);
        var gLAccounts = await connection.QueryAsync<GLAccounts>(queryGLAccount, pGL);

        if (gLAccounts == null || !gLAccounts.Any())
        {
            throw new InvalidOperationException("No GL accounts found for the specified account.");
        }

        // Fix: Convert the list of GLAccountCodes to a comma-separated string
        filter.AccountCode = string.Join(",", gLAccounts.Select(gl => gl.GLAccountCode));


        var query = @"SELECT Period, SalesAmount
            FROM FN_MonthlySales(@DateFrom, @DateTo, @AccountCode);";
        var p = new DynamicParameters();
        p.Add("@DateFrom", filter.DateFrom);
        p.Add("@DateTo", filter.DateTo);
        p.Add("@AccountCode", filter.AccountCode); // Pass the comma-separated string of GLAccountCode values
        var model = await connection.QueryAsync<MonthlySales>(query, p);

        return model;
    }

    //BALANCE
    public async Task<IEnumerable<BankAccountBalance>> GetBankAccountBalanceResult(BankAccountBalanceFilter filter)
    {
        DateTime Date = DateTime.Now;
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT GLAccountCode, Description, Amount
            FROM FN_GetBankAccountBalance(@Date)
            ORDER BY GLAccountCode ASC";
        var p = new DynamicParameters();
        p.Add("@Date", filter.Date);
        var model = await connection.QueryAsync<BankAccountBalance>(query, p);
        return model;
    }


    //TOP 3 OVERDUE CUSTOMER
    public async Task<IEnumerable<TopOverdueCustomer>> GetTop3OverdueCustomer()
    {
        DateTime? DateTo = DateTime.Now;
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT CompanyCode, CompanyName, Outstanding, OutstandingLocal
            FROM FN_Top3OverdueCustomer(@Date)";
        var p = new DynamicParameters();
        p.Add("@Date", DateTo);
        var model = await connection.QueryAsync<TopOverdueCustomer>(query, p);
        return model;
    }



    //TOP 3 OVERDUE SUPPLIER
    public async Task<IEnumerable<TopOverdueSupplier>> GetTop3OverdueSupplier()
    {
        DateTime? DateTo = DateTime.Now;
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT CompanyCode, CompanyName, Outstanding, OutstandingLocal FROM FN_Top3OverdueSupplier(@Date)";
        var p = new DynamicParameters();
        p.Add("@Date", DateTo);
        var model = await connection.QueryAsync<TopOverdueSupplier>(query, p);
        return model;
    }


    //TOP 3 NEAR EXPIRYITEM
    public async Task<IEnumerable<TopNearExpiryItem>> GetTop3NearExpiryItem()
    {
        DateTime? DateTo = DateTime.Now;
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT StockCode, StockName, Qty, BatchNoExpiryDate FROM QFN_Top3NearExpiryItem(@Date)";
        var p = new DynamicParameters();
        p.Add("@Date", DateTo);
        var model = await connection.QueryAsync<TopNearExpiryItem>(query, p);
        return model;
    }


    //OUTSTANDING SALES ORDER BY REQUIRE DATE
    public async Task<IEnumerable<TopStockReorder>> GetTop3StockReorder()
    {
        DateTime? DateTo = DateTime.Now;
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT StockCode, StockName, MinQty, MaxQty, ReorderLevel, ReorderQty, CurrentBalance FROM QFN_Top3StockReorder(@Date)";
        var p = new DynamicParameters();
        p.Add("@Date", DateTo);
        var model = await connection.QueryAsync<TopStockReorder>(query, p);
        return model;
    }
}
