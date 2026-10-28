using System.Data;
using System.Data.Common;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllTaxCodeModels;
using Accounting_V4.Model.QNEModels.CurrenciesModels;
using Accounting_V4.Model.QNEModels.CustomerModels;
using Accounting_V4.Model.QNEModels.DescriptionModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Pages.QNEPages.GeneralLedger.ChartOfAccounts;
using Accounting_V4.Pages.QNEPages.GeneralLedger.GLAccount;
using Accounting_V4.Services.OptimisticLockFieldServices;
using Blazored.LocalStorage;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.GLAccountServices;

public class GLAccountsServices
{
    protected ChartOfAccountsServices chartOfAccountsServices
    {
        get; set;
    }
    protected OptimisticLockField optimisticLockField
    {
        get; set;
    }

    private readonly ILocalStorageService localStorage;

    private readonly QneConnectionString qneConnection;

    public GLAccountsServices(ChartOfAccountsServices chartOfAccountsServices, OptimisticLockField optimisticLockField, QneConnectionString qneConnection, ILocalStorageService localStorage)
    {
        this.chartOfAccountsServices = chartOfAccountsServices;
        this.optimisticLockField = optimisticLockField;
        this.qneConnection = qneConnection;
        this.localStorage = localStorage;
    }

    //LIST OF SPECIAL ACCOUNTS
    public async Task<IEnumerable<SpecialAccounts>> GetSpecialAccounts()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM SpecialAccounts";
        var model = await connection.QueryAsync<SpecialAccounts>(query, commandType: CommandType.Text);
        return model;

    }


    //LIST OF CURRENCIES
    //DEPRICATED


    //LIST OF Accounts
    public async Task<IEnumerable<Accounts>> GetAccounts()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT Id, AccountCode, Description, ChartSequence, DRCR, OptimisticLockField, ChartSequencePH, AccountCodePH, RowIndex FROM Accounts ORDER BY AccountCode ASC";
        var model = await connection.QueryAsync<Accounts>(query, commandType: CommandType.Text);
        return model;

    }

    //LIST OF SUBACCOUNT
    public async Task<IEnumerable<SubAccounts>> GetSubAccounts()
    {

        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM GLAccounts ORDER BY GLAccountCode ASC";
        var model = await connection.QueryAsync<SubAccounts>(query, commandType: CommandType.Text);
        return model;


    }


    //GET LIST OF GLACCOUNTS
    public async Task<IEnumerable<GLAccounts>> GetGLAccounts()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        {
            var query = @"SELECT * FROM GLAccounts ORDER BY RowIndex ASC";
            var model = await connection.QueryAsync<GLAccounts>(query, commandType: CommandType.Text);
            return model;
        }

    }


    //ACCOUNTS AND GLACCOUNT table
    public async Task<IEnumerable<GeneralLedgerAccount>> GetGLAccountListView()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"
        WITH CTE_SubAccount AS (
            SELECT DISTINCT 
                ParentId AS Id, 
                CAST(1 AS BIT) AS IsSubAccount
            FROM dbo.GLAccounts
        ),
        DefaultTaxCode AS (
            SELECT 
                TaxCodes.Id,
                TaxCodes.GLAccountId,
                DefaultInputTaxCodeId,
                DefaultOutputTaxCodeId,
                TaxCodes.OptimisticLockField,
                TaxCode1.TaxCode AS DefaultInputTaxCode,
                TaxCode2.TaxCode AS DefaultOutputTaxCode,
                TaxCode1.Description AS InputTaxCodeDescription,
                TaxCode2.Description AS OutputTaxCodeDescription,
                GLAccounts.GLAccountCode,
                GLAccounts.Description
            FROM GLAccountTaxCodeLinks AS TaxCodes 
            LEFT JOIN TaxCodes AS TaxCode1 ON TaxCodes.DefaultInputTaxCodeId = TaxCode1.Id
            LEFT JOIN TaxCodes AS TaxCode2 ON TaxCodes.DefaultOutputTaxCodeId = TaxCode2.Id
            LEFT JOIN GLAccounts ON TaxCodes.GLAccountId = GLAccounts.Id
        )
        SELECT 
            GLAccounts_1.Id, 
            GLAccounts_1.GLAccountCode AS AccountNumber, 
            GLAccounts_1.Description AS AccountName, 
            Accounts.Description AS AccountType, 
            SpecialAccounts.SpecialAccountCode AS SpecialAccount, 
            GLAccounts_1.BankAccountNo, 
            GLAccounts_1.CurrencyId, 
            Currencies.CurrencyCode AS Currency, 
            GLAccounts_1.Description2, 
            CASE 
                WHEN GLAccounts_1.DRCR = 'DR' THEN 'Debit'
                WHEN GLAccounts_1.DRCR = 'CR' THEN 'Credit'
                ELSE GLAccounts_1.DRCR
            END AS DRCR,
            GLAccounts_1.IsActive, 
            GLAccounts_1.Notes, 
            GLAccounts_1.ParentId, 
            SpecialAccounts.Description AS SACode, 
            ISNULL(CTE_SubAccount_1.IsSubAccount, 0) AS IsSubAccount, 
            GLAccounts_1.SSTTariffCode, 
            GLAccounts_1.HasChildInCoa, 
            GLAccounts_1.IsCreditCard, 
            GLAccounts_1.GSTGroup, 
            GLAccounts_1.IsRevaluation, 
            Accounts.AccountCode, 
            GLAccounts_1.SpecialAccountId, 
            GLAccounts_1.AccountId, 
            GLAccounts_1.RowIndex, 
            GLAccounts_1.GSTTypeId,
            DefaultTaxCode.DefaultInputTaxCode,
            DefaultTaxCode.DefaultOutputTaxCode,
            DefaultTaxCode.InputTaxCodeDescription,
            DefaultTaxCode.OutputTaxCodeDescription
        FROM dbo.GLAccounts AS GLAccounts_1
        LEFT JOIN dbo.Accounts ON GLAccounts_1.AccountId = Accounts.Id 
        LEFT JOIN dbo.SpecialAccounts ON GLAccounts_1.SpecialAccountId = SpecialAccounts.Id 
        LEFT JOIN dbo.Currencies ON GLAccounts_1.CurrencyId = Currencies.Id 
        LEFT JOIN CTE_SubAccount AS CTE_SubAccount_1 ON GLAccounts_1.Id = CTE_SubAccount_1.Id
        LEFT JOIN DefaultTaxCode ON GLAccounts_1.Id = DefaultTaxCode.GLAccountId
        ORDER BY AccountNumber ASC;";

        // Adding a timeout to avoid long-running queries
        var commandTimeout = 60; // seconds

        var model = await connection.QueryAsync<GeneralLedgerAccount>(query, commandType: CommandType.Text, commandTimeout: commandTimeout);
        return model;

    }


    //GET GLAccountById
    public async Task<GLAccounts> GetGLAccountsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM dbo.GLAccounts WHERE Id = @Id";

        var model = await connection.QueryFirstOrDefaultAsync<GLAccounts>(query, new { Id });

        return model;
    }


    //GET AccountsById
    public async Task<Accounts> GetAccountsById(string Id)
    {

        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM dbo.Accounts WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<Accounts>(query, new { Id });

        return model;

    }




    //GET LIST OF GLACCOUNTS
    //public async Task<IEnumerable<GLAccounts>> GetGLAccounLists()
    //{
    //    var connectionString = await qneConnection.GetConnectionStringAsync();
    //    var connection = new SqlConnection(connectionString);
    //    var query = @"SELECT * FROM GLAccounts ORDER BY GLAccountCode ASC";
    //    var model = await connection.QueryAsync<GLAccounts>(query, commandType: CommandType.Text);
    //    return model;
    //}

    //GET GENERAL LEDGER ACCOUNT LIST
    public async Task<IEnumerable<GLAccounts>> GetGeneralLedgerAccountList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        {
            var query = @"
        SELECT 
               [Id],
               [ParentId],
               [GLAccountCode],
               [AccountId],
               [SpecialAccountId],
               [CurrencyId],
               [Description],
               [DRCR],
               [IsCreditCard],
               [IsActive],
               [GSTGroup],
               [IsRevaluation],
               [Notes],
               [IsSubAccount],
               [BankAccountNo],
               [GSTMSICCode],
               [OptimisticLockField],
               [SAC],
               [RowIndex],
               [HasChildInCoa],
               [IncludeInCashFlowForecastAdvisor],
               [SSTTariffCode],
               [TariffCodeId],
               [ATCCodeId],
               [Description2]
        FROM [GLAccounts]
        ORDER BY GLAccountCode ASC";

            var model = await connection.QueryAsync<GLAccounts>(query, commandType: CommandType.Text);
            return model;
        }
    }


    //CREATE QUERY // OLD APROACH
    //public async Task<int> InsertGLAccounts(GLAccounts account)
    //{

    //    var connectionString = await qneConnection.GetConnectionStringAsync();
    //    var connection = new SqlConnection(connectionString);
    //    var query = @"
    //            INSERT INTO GLAccounts (
    //                Id,
    //                ParentId,
    //                GLAccountCode,
    //                AccountId,
    //                SpecialAccountId,
    //                CurrencyId,
    //                GSTTypeId,
    //                Description,
    //                DRCR,
    //                IsCreditCard,
    //                IsActive,
    //                GSTGroup,
    //                IsRevaluation,
    //                Notes,
    //                IsSubAccount,
    //                BankAccountNo,
    //                GSTMSICCode,
    //                OptimisticLockField,
    //                SAC,
    //                HasChildInCoa,
    //                IncludeInCashFlowForecastAdvisor,
    //                SSTTariffCode,
    //                TariffCodeId,
    //                ATCCodeId,
    //                Description2
    //            ) VALUES (
    //                @Id,
    //                @ParentId,
    //                @GLAccountCode,
    //                @AccountId,
    //                @SpecialAccountId,
    //                @CurrencyId,
    //                @GSTTypeId,
    //                @Description,
    //                @DRCR,
    //                @IsCreditCard,
    //                @IsActive,
    //                @GSTGroup,
    //                @IsRevaluation,
    //                @Notes,
    //                @IsSubAccount,
    //                @BankAccountNo,
    //                @GSTMSICCode,
    //                @OptimisticLockField,
    //                @SAC,
    //                @HasChildInCoa,
    //                @IncludeInCashFlowForecastAdvisor,
    //                @SSTTariffCode,
    //                @TariffCodeId,
    //                @ATCCodeId,
    //                @Description2
    //            )";

    //    var p = new DynamicParameters();
    //    p.Add("@Id", account.Id);
    //    p.Add("@ParentId", account.ParentId);
    //    p.Add("@GLAccountCode", account.GLAccountCode);
    //    p.Add("@AccountId", account.AccountId);
    //    p.Add("@SpecialAccountId", account.SpecialAccountId);
    //    p.Add("@CurrencyId", account.CurrencyId);
    //    p.Add("@GSTTypeId", account.GSTTypeId);
    //    p.Add("@Description", account.Description);
    //    p.Add("@DRCR", account.DRCR);
    //    p.Add("@IsCreditCard", account.IsCreditCard);
    //    p.Add("@IsActive", account.IsActive);
    //    p.Add("@GSTGroup", account.GSTGroup);
    //    p.Add("@IsRevaluation", account.IsRevaluation);
    //    p.Add("@Notes", account.Notes);
    //    p.Add("@IsSubAccount", account.IsSubAccount);
    //    p.Add("@BankAccountNo", account.BankAccountNo);
    //    p.Add("@GSTMSICCode", account.GSTMSICCode);
    //    p.Add("@OptimisticLockField", account.OptimisticLockField);
    //    p.Add("@SAC", account.SAC);
    //    p.Add("@HasChildInCoa", account.HasChildInCoa);
    //    p.Add("@IncludeInCashFlowForecastAdvisor", account.IncludeInCashFlowForecastAdvisor);
    //    p.Add("@SSTTariffCode", account.SSTTariffCode);
    //    p.Add("@TariffCodeId", account.TariffCodeId);
    //    p.Add("@ATCCodeId", account.ATCCodeId);
    //    p.Add("@Description2", account.Description2);
    //    var model = await connection.ExecuteAsync(query, account);

    //    return model;

    //}

    //UpdateGLAccount


    public async Task<string> InsertGLAccounts(GLAccounts account)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var query = @"INSERT INTO GLAccounts (
                     Id, ParentId, GLAccountCode, AccountId, SpecialAccountId,
                     CurrencyId, GSTTypeId, Description, DRCR, IsCreditCard, 
                     IsActive, GSTGroup, IsRevaluation, Notes, IsSubAccount, 
                     BankAccountNo, GSTMSICCode, OptimisticLockField, SAC, 
                     HasChildInCoa, IncludeInCashFlowForecastAdvisor, 
                     SSTTariffCode, TariffCodeId, ATCCodeId, Description2
                 ) VALUES (
                     @Id, @ParentId, @GLAccountCode, @AccountId, @SpecialAccountId,
                     @CurrencyId, @GSTTypeId, @Description, @DRCR, @IsCreditCard,
                     @IsActive, @GSTGroup, @IsRevaluation, @Notes, @IsSubAccount,
                     @BankAccountNo, @GSTMSICCode, @OptimisticLockField, @SAC,
                     @HasChildInCoa, @IncludeInCashFlowForecastAdvisor,
                     @SSTTariffCode, @TariffCodeId, @ATCCodeId, @Description2
                 )";
        using var transaction = connection.BeginTransaction();
        try
        {
            var result = await connection.ExecuteAsync(query, account, transaction);
            transaction.Commit();
            return $"Insert {result} GLAccounts(s) Success!";
        }
        //catch mo yun error dito
        catch (SqlException ex)
        {
            transaction.Rollback();


            if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
            {

                if (ex.Message.Contains("GLAccount"))
                {
                    return $"Duplicate value detected on GLAccount. GLAccount Code must be unique.";
                }
                return "Duplicate value detected. Values must be unique.";
            }
            throw; // para makita mo pa rin yung error
        }



    }

    public async Task<int> UpdateGLAccounts(GLAccounts gLaccount)
    {

        var existingNotes = await chartOfAccountsServices.GetNotesById(gLaccount.Id.ToString());

        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"UPDATE GLAccounts SET 
                 ParentId = @ParentId,
                 GLAccountCode = @GLAccountCode, 
                 AccountId = @AccountId,
                 SpecialAccountId = @SpecialAccountId,
                 CurrencyId = @CurrencyId,
                 GSTTypeId = @GSTTypeId,
                 Description = @Description,
                 DRCR = @DRCR,
                 IsCreditCard = @IsCreditCard,
                 IsActive = @IsActive,
                 GSTGroup = @GSTGroup,
                 IsRevaluation = @IsRevaluation,
                 Notes = @Notes,
                 IsSubAccount = @IsSubAccount,
                 BankAccountNo = @BankAccountNo,
                 GSTMSICCode = @GSTMSICCode,
                 OptimisticLockField = OptimisticLockField + 1,
                 SAC = @SAC,
                 HasChildInCoa = @HasChildInCoa,
                 IncludeInCashFlowForecastAdvisor = @IncludeInCashFlowForecastAdvisor,
                 SSTTariffCode = @SSTTariffCode,
                 TariffCodeId = @TariffCodeId,
                 ATCCodeId = @ATCCodeId,
                 Description2 = @Description2 
             WHERE Id = @Id";
        var p = new DynamicParameters();
        p.Add("@Id", gLaccount.Id);
        p.Add("@ParentId", gLaccount.ParentId);
        p.Add("@GLAccountCode", gLaccount.GLAccountCode);
        p.Add("@AccountId", gLaccount.AccountId);
        p.Add("@SpecialAccountId", gLaccount.SpecialAccountId);
        p.Add("@CurrencyId", gLaccount.CurrencyId);
        p.Add("@GSTTypeId", gLaccount.GSTTypeId);
        p.Add("@Description", gLaccount.Description);
        p.Add("@DRCR", gLaccount.DRCR);
        p.Add("@IsCreditCard", gLaccount.IsCreditCard);
        p.Add("@IsActive", gLaccount.IsActive);
        p.Add("@GSTGroup", gLaccount.GSTGroup);
        p.Add("@IsRevaluation", gLaccount.IsRevaluation);
        p.Add("@Notes", gLaccount.Notes);
        p.Add("@IsSubAccount", gLaccount.IsSubAccount);
        p.Add("@BankAccountNo", gLaccount.BankAccountNo);
        p.Add("@GSTMSICCode", gLaccount.GSTMSICCode);
        p.Add("@SAC", gLaccount.SAC);
        p.Add("@HasChildInCoa", gLaccount.HasChildInCoa);
        p.Add("@IncludeInCashFlowForecastAdvisor", gLaccount.IncludeInCashFlowForecastAdvisor);
        p.Add("@SSTTariffCode", gLaccount.SSTTariffCode);
        p.Add("@TariffCodeId", gLaccount.TariffCodeId);
        p.Add("@ATCCodeId", gLaccount.ATCCodeId);
        p.Add("@Description2", gLaccount.Description2);
        var rowsAffected = await connection.ExecuteAsync(query, p);


        await optimisticLockField.UpdateOptimisticLockField("GLAccounts", gLaccount.Id);
        return rowsAffected;




    }


    public async Task<Debtors> GetCustomerById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Debtors WHERE Id = @Id";

        var model = await connection.QueryFirstOrDefaultAsync<Debtors>(query, new { Id });

        return model;
    }


    //DELETE GLAccount with TaxCodeLinks and BankAccountsSettings
    //public async Task<int> DeleteGLAccount(string Id)
    //{
    //    int totalRowsAffected = 0;
    //    // Delete child records
    //    if (await GetGLTaxCodesById(Id) != null)
    //        //totalRowsAffected += await DeleteTaxCodes(Id);

    //    if (await chartOfAccountsServices.GetBankAccountSettingById(Id) != null)
    //       // totalRowsAffected += await DeleteBankAccountSettings(Id);

    //    if (await chartOfAccountsServices.GetNotesById(Id) != null)
    //        //totalRowsAffected += await DeleteChartOfAccountNotes(Id);

    //    if (await GetCustomerById(Id) != null)
    //    {
    //        totalRowsAffected += await DeleteCustomer(Id);
    //        if (totalRowsAffected > 0)
    //            totalRowsAffected += await DeleteGLAccounts(Id);
    //    }
    //    else
    //    {
    //        totalRowsAffected += await DeleteGLAccounts(Id);
    //    }

    //    return totalRowsAffected;
    //}


    public async Task<string> DeleteGLAccounts(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);


        await connection.OpenAsync();
        var queryGLAccountTaxCodeLinks = "DELETE FROM GLAccountTaxCodeLinks WHERE GLAccountId=@Id";
        var queryBankAccountSettings = "DELETE FROM BankAccountSettings WHERE GLAccountId=@Id";
        var queryNotes = "DELETE FROM Notes WHERE MasterId = @Id";
        var queryDebtors = @"DELETE FROM Debtors WHERE GLAccountId=@Id";
        var queryGLAccounts = "DELETE FROM GLAccounts WHERE Id=@Id";



        using var transaction = connection.BeginTransaction();
        try
        {
            var rowsGLAccountTaxCodeLinks = await connection.ExecuteAsync(queryGLAccountTaxCodeLinks, new { Id }, transaction);
            var rowsBankAccountSettings = await connection.ExecuteAsync(queryBankAccountSettings, new { Id }, transaction);
            var rowsNotes = await connection.ExecuteAsync(queryNotes, new { Id }, transaction);
            var rowsDebtors = await connection.ExecuteAsync(queryDebtors, new { Id }, transaction);
            var rowsGLAccount = await connection.ExecuteAsync(queryGLAccounts, new { Id }, transaction);
            transaction.Commit();
            return $"Deleted {rowsGLAccount} GL account(s).";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            if (ex.Message.Contains("REFERENCE constraint"))
            {
                string tableName = "";

                if (ex.Message.Contains("Debtors"))
                    tableName = "Debtors Control Account";
                else if (ex.Message.Contains("MonthlyStockValue"))
                    tableName = "Monthly Stock";
                else if (ex.Message.Contains("Creditors"))
                    tableName = "Creditor Control Account";
                if (!string.IsNullOrEmpty(tableName))
                {
                    return $"This account is being used in {tableName}.";
                }

                return "This account is being used in other transactions.";
            }
            throw; // ibang error, ipasa ulit
        }


    }

    //DELETE GLAccountTaxCodeLinks
    public async Task<int> DeleteTaxCodes(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = "DELETE FROM GLAccountTaxCodeLinks WHERE GLAccountId=@Id";
        var p = new DynamicParameters();
        p.Add("@Id", Id);
        var rowsAffected = await connection.ExecuteAsync(query, p);
        return rowsAffected;

    }

    //DELETE BankAccountSettings
    public async Task<int> DeleteBankAccountSettings(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = "DELETE FROM BankAccountSettings WHERE GLAccountId=@Id";
        var p = new DynamicParameters();
        p.Add("@Id", Id);
        var rowsAffected = await connection.ExecuteAsync(query, p);
        return rowsAffected;
    }

    //DELETE Customer from db.Debtors
    public async Task<int> DeleteCustomer(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"DELETE FROM Debtors WHERE GLAccountId=@Id";
        var p = new DynamicParameters();
        p.Add("@Id", Id);
        return await connection.ExecuteAsync(query, p);

    }




    //DELETE BankAccountSettings
    public async Task<int> DeleteChartOfAccountNotes(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = "DELETE FROM Notes WHERE MasterId = @Id";
        var p = new DynamicParameters();
        p.Add("@Id", Id);
        var rowsAffected = await connection.ExecuteAsync(query, p);
        return rowsAffected;

    }






    //GET GetGLTaxCodesById
    public async Task<GLAccountTaxCodeLinks> GetGLTaxCodesById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM GLAccountTaxCodeLinks WHERE GLAccountId = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<GLAccountTaxCodeLinks>(query, new { Id });

        return model;

    }


    public async Task<int> InsertGLAccountTaxCodes(GLAccountTaxCodeLinks glaccounttaxcode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"
                INSERT INTO GLAccountTaxCodeLinks (
                    Id,
                    GLAccountId,
                    DefaultInputTaxCodeId,
                    DefaultOutputTaxCodeId,
                    OptimisticLockField
                ) VALUES (
                    @Id,
                    @GLAccountId,
                    @DefaultInputTaxCodeId,
                    @DefaultOutputTaxCodeId,
                    @OptimisticLockField
                )";

        //var p = new DynamicParameters();
        //p.Add("@Id", glaccounttaxcode.Id);
        //p.Add("@GLAccountId", glaccounttaxcode.GLAccountId);
        //p.Add("@DefaultInputTaxCodeId", glaccounttaxcode.DefaultInputTaxCodeId);
        //p.Add("@DefaultOutputTaxCodeId", glaccounttaxcode.DefaultOutputTaxCodeId);
        //p.Add("@OptimisticLockField", glaccounttaxcode.OptimisticLockField);

        var model = await connection.ExecuteAsync(query, glaccounttaxcode);
        return model;

    }


    //UpdateGLAccountTaxCode
    public async Task<int> UpdateGLAccountTaxCodes(GLAccountTaxCodeLinks glaccounttaxcode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"UPDATE GLAccountTaxCodeLinks SET
                GLAccountId = @GLAccountId,
                DefaultInputTaxCodeId = @DefaultInputTaxCodeId,
                DefaultOutputTaxCodeId = @DefaultOutputTaxCodeId,
                OptimisticLockField = @OptimisticLockField
                WHERE Id = @Id";

        var p = new DynamicParameters();
        p.Add("@Id", glaccounttaxcode.Id);
        p.Add("@GLAccountId", glaccounttaxcode.GLAccountId);
        p.Add("@DefaultInputTaxCodeId", glaccounttaxcode.DefaultInputTaxCodeId);
        p.Add("@DefaultOutputTaxCodeId", glaccounttaxcode.DefaultOutputTaxCodeId);
        p.Add("@OptimisticLockField", glaccounttaxcode.OptimisticLockField + 1);
        var rowsAffected = await connection.ExecuteAsync(query, p);
        return rowsAffected;

    }



    public async Task<IEnumerable<GLAccounts>> GetGLAccountList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT  * FROM GLAccounts
          WHERE IsSubAccount = 0
        AND ISNULL(SpecialAccountId, 0) NOT IN (12, 14,15)
        ORDER BY GLAccountCode ASC";
        var model = await connection.QueryAsync<GLAccounts>(query, commandType: CommandType.Text);
        return model;
    }


    public async Task<IEnumerable<GLAccounts>> GetDepositAccountList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM GLAccounts
        WHERE DRCR = 'DR'
        AND IsSubAccount !=1
        AND SpecialAccountId IN (5,3)
        ORDER BY GLAccountCode ASC";
        var model = await connection.QueryAsync<GLAccounts>(query, commandType: CommandType.Text);
        return model;
    }


    public async Task<IEnumerable<GLAccounts>> GetGLAccountAccountOnlyList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT  * FROM GLAccounts
          WHERE IsSubAccount = 0
          AND IsActive = 1
        ORDER BY GLAccountCode ASC";
        var model = await connection.QueryAsync<GLAccounts>(query, commandType: CommandType.Text);
        return model;
    }


    public async Task<Guid?> GetGLAccountTAXSALESACCOUNT()
    {
        var company = await localStorage.GetItemAsync<string>("selectedCompanyName");
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"
        SELECT TOP 1 Id
        FROM GLAccounts
        WHERE Description LIKE @Company";

        var p = new DynamicParameters();
        p.Add("@Company", $"%{company}%");

        return await connection.QueryFirstOrDefaultAsync<Guid?>(query, p);
    }


    public async Task<Guid?> GetGLAccountOUTPUTVATACCOUNT()
    {
        var company = await localStorage.GetItemAsync<string>("selectedCompanyName");
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM GLAccounts WHERE GLAccountCode LIKE '2-009-000-000-000'";

        var p = new DynamicParameters();
        p.Add("@Company", $"%{company}%");

        return await connection.QueryFirstOrDefaultAsync<Guid?>(query, p);
    }

}