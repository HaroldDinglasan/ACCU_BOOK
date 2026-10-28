using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.CustomerModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.GLAccountServices;

public class ChartOfAccountsServices
{
    private readonly QneConnectionString qneConnection;
    public ChartOfAccountsServices(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }
    //LIST Chart Of Accounts
    public async Task<IEnumerable<Accounts>> GetChartofAccounts(string companyCode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT Id 
                                ,AccountCode
                                , Description
                                , ChartSequence
                                , DRCR
                                , OptimisticLockField
                                , ChartSequencePH
                                , AccountCodePH
                                , RowIndex FROM ACCOUNTS
                ORDER BY ChartSequencePH ASC";
        var model = await connection.QueryAsync<Accounts>(query, commandType: CommandType.Text);
        return model;

    }

    //LIST Chart Of Accounts Tree
    public async Task<IEnumerable<ChartOfAccountTree>> GetChartofAccountTree()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"WITH CTE_GLA AS (
    SELECT
        A.ChartSequence,
        A.ChartSequencePH,
        G.AccountId,
        G.Id, 
        G.GLAccountCode, 
        C.CurrencyCode, 
        S.Id AS SpecialAccountId, 
        S.SpecialAccountCode, 
        S.Description AS SpecialAccount, 
        G.Description, 
        G.ParentId, 
        G.DRCR, 
        G.Notes,
        G.RowIndex,
        A.AccountCode, 
        A.Description AS Account,
        it.Id AS InputTaxId,
        ot.Id AS OutputTaxId,
        it.TaxCode AS InputTax, 
        ot.TaxCode AS OutputTax
    FROM GLAccounts G
    LEFT JOIN SpecialAccounts S ON G.SpecialAccountId = S.id
    JOIN Accounts A ON G.AccountId = A.Id
    LEFT JOIN Currencies C ON C.Id = G.CurrencyId
    LEFT JOIN GLAccountTaxCodeLinks link ON link.GLAccountId = G.Id
    LEFT JOIN TaxCodes it ON it.Id = link.DefaultInputTaxCodeId
    LEFT JOIN TaxCodes ot ON ot.Id = link.DefaultOutputTaxCodeId
    WHERE NOT EXISTS(
        SELECT 1
        FROM(
            SELECT '%B0%' AS acctcode
            UNION ALL
            SELECT '%B-0%'
            UNION ALL
            SELECT '%B1%'
            UNION ALL
            SELECT '%B-1%'
            UNION ALL
            SELECT '%N05%'
            UNION ALL
            SELECT '%BO5%'
            UNION ALL
            SELECT '%H05%'
        ) acctcodes
        WHERE G.GLAccountCode LIKE acctcodes.acctcode
    )
    -- 🟦 Exclude child rows of AR, OR, and AP
    AND G.ParentId NOT IN (
        SELECT Id FROM GLAccounts 
        WHERE Description IN ('Accounts Receivable','Other Receivable','Accounts Payable')
    )
)
SELECT * FROM CTE_GLA
UNION
SELECT
    A.ChartSequence,
    A.ChartSequencePH,
    NULL AS AccountId,
    A.Id, 
    A.AccountCode AS GLAccountCode, 
    NULL AS CurrencyCode, 
	NULL AS SpecialAccountId,
    NULL AS SpecialAccountCode, 
    NULL AS SpecialAccount, 
    A.Description, 
    NULL AS ParentId, 
    NULL AS DRCR, 
    NULL AS Notes,
    NULL AS RowIndex,
    A.AccountCode, 
    A.Description AS Account, 
    NULL AS InputTaxId,
    NULL AS OutputTaxId,
    NULL AS InputTax, 
    NULL AS OutputTax
FROM Accounts A
WHERE NOT EXISTS(
    SELECT 1
    FROM(
        SELECT '%B0%' AS acctcode
        UNION ALL
        SELECT '%B-0%'
        UNION ALL
        SELECT '%B1%'
        UNION ALL
        SELECT '%B-1%'
        UNION ALL
        SELECT '%N05%'
        UNION ALL
        SELECT '%BO5%'
        UNION ALL
        SELECT '%H05%'
    ) acctcodes
    WHERE A.AccountCode LIKE acctcodes.acctcode
)
ORDER BY ChartSequencePH ASC, RowIndex ASC;";
        var model = await connection.QueryAsync<ChartOfAccountTree>(query, commandType: CommandType.Text);
        return model;

    }

    //GET first or defaults data by Id from dbo.Accounts
    public async Task<ChartOfAccountTree> GetChartofAccountTreeById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"WITH CTE_GLA AS (
                        SELECT 
                            A.ChartSequence,
                            A.ChartSequencePH,
                            A.Id AS AcctId,
                            G.AccountId,
                            G.Id,
                            G.Id AS GLAccountId,
                            G.GLAccountCode,
						    G.CurrencyId,
						    G.GSTTypeId,
                            C.CurrencyCode, 
						    G.SpecialAccountId,
                            S.SpecialAccountCode, 
                            S.Description AS SpecialAccount, 
                            G.Description,
                            G.Description2,
						    A.Description AS AcctDescription,
						    G.Description AS GLDescription,
                            G.ParentId,
						    G.IsActive,
						    G.GSTGroup,
						    G.IsRevaluation,
                            G.DRCR,
						    G.IsCreditCard,
                            G.Notes,
						    G.IsSubAccount,
						    G.BankAccountNo,
						    G.GSTMSICCode,
						    G.OptimisticLockField,
						    G.SAC,
                            G.RowIndex,
						    G.HasChildInCoa,
						    G.IncludeInCashFlowForecastAdvisor,
						    G.SSTTariffCode,
						    G.TariffCodeId,
						    G.ATCCodeId,
                            A.AccountCode, 
                            A.Description AS Account,
                		    it.Id AS InputTaxId,
                		    ot.Id AS OutputTaxId,
                            it.TaxCode AS InputTax, 
                            ot.TaxCode AS OutputTax
                        FROM GLAccounts G 
                        LEFT JOIN SpecialAccounts S ON G.SpecialAccountId = S.id 
                        JOIN Accounts A ON G.AccountId = A.Id 
                        LEFT JOIN Currencies C ON C.Id = G.CurrencyId 
                        LEFT JOIN GLAccountTaxCodeLinks link ON link.GLAccountId = G.Id 
                        LEFT JOIN TaxCodes it ON it.Id = link.DefaultInputTaxCodeId 
                        LEFT JOIN TaxCodes ot ON ot.Id = link.DefaultOutputTaxCodeId
					    WHERE G.Id = @Id
                    )
                    SELECT * 
                    FROM CTE_GLA
                    UNION
                    SELECT 
                        A.ChartSequence,
                        A.ChartSequencePH,
                        A.Id AS AcctId,
                        NULL AS AccountId,
                        A.Id, 
                        NULL AS GLAccountId,
                        A.AccountCode AS GLAccountCode, 
					    NULL AS CurrencyId,
					    NULL AS GSTTypeId,
                        NULL AS CurrencyCode, 
					    NULL AS SpecialAccountId,
                        NULL AS SpecialAccountCode, 
                        NULL AS SpecialAccount, 
                        A.Description,
					    A.Description AS AcctDescription,
					    NULL AS GLDescription,
                        NULL Description2,
                        NULL AS ParentId, 
					    NULL AS IsActive,
					    NULL AS GSTGroup,
					    NULL AS IsRevaluation,
                        NULL AS DRCR, 
					    NULL AS IsCreditCard,
                        NULL AS Notes,
					    NULL AS IsSubAccount,
					    NULL AS BankAccountNo,
					    NULL AS GSTMSICCode,
					    NULL AS OptimisticLockField,
					    NULL AS SAC,
                        NULL AS RowIndex,
					    NULL AS HasChildInCoa,
					    NULL AS IncludeInCashFlowForecastAdvisor,
					    NULL AS SSTTariffCode,
					    NULL AS TariffCodeId,
					    NULL AS ATCCodeId,
                        A.AccountCode, 
                        A.Description AS Account, 
                	    NULL AS InputTaxId,
                	    NULL AS OutputTaxId,
                        NULL AS InputTax, 
                        NULL AS OutputTax
                    FROM Accounts A
				    WHERE A.Id = @Id
                    ORDER BY ChartSequencePH ASC, RowIndex ASC";
        var model = await connection.QueryFirstOrDefaultAsync<ChartOfAccountTree>(query, new { Id });


        var notes = await GetNotesById(Id);
        ChartOfAccountNotes chartOfAccountNotes;
        if (notes != null)
        {
            chartOfAccountNotes = notes;
        }
        return model;





    }

    //Update Accounts from dbo.Accounts
    public async Task<int> UpdateAccounts(Accounts accounts)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"UPDATE Accounts SET 
                                AccountCode = @AccountCode
                                ,Description = @Description
                                WHERE Id = @Id";
        var rowsAffected = await connection.ExecuteAsync(query, accounts);
        return rowsAffected;


        /*  var query = @"UPDATE Accounts SET 
                          AccountCode = @AccountCode
                          ,Description = @Description
                          WHERE Id = @Id";
          var p = new DynamicParameters();
          p.Add("@Id", accounts.Id);
          p.Add("@AccountCode", accounts.AccountCode);
          p.Add("@Description", accounts.Description);
          var rowsAffected = await connection.ExecuteAsync(query, p);
          return rowsAffected;*/

    }

    //Get Bank Account Settings from dbo.BankAccountSettings
    public async Task<BankAccountSettings> GetBankAccountSettingById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM BankAccountSettings WHERE GLAccountId = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<BankAccountSettings>(query, new { Id });
        return model;

    }



    public async Task<ChartOfAccountNotes> GetNotesById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Notes WHERE MasterId = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<ChartOfAccountNotes>(query, new { Id });
        return model;

    }

    public async Task<int> InsertNotes(ChartOfAccountNotes notes)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"INSERT INTO Notes (
                    MasterId, Notes
                ) VALUES (
                    @MasterId, @Notes
                )";
        /*  var p = new DynamicParameters();
          p.Add("@MasterId", notes.MasterId);
          p.Add("@Notes", notes.Notes);*/
        var rowsAffected = await connection.ExecuteAsync(query, notes);
        return rowsAffected;

    }

    public async Task<int> UpdateNotes(ChartOfAccountNotes notes)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"UPDATE Notes SET Notes = @Notes WHERE MasterId = @MasterId";
        var p = new DynamicParameters();
        p.Add("@MasterId", notes.MasterId);
        p.Add("@Notes", notes.Notes);
        var rowsAffected = await connection.ExecuteAsync(query, p);

        return rowsAffected;

    }

    //Insert Bank Account Settings from dbo.BankAccountSettings
    public async Task<int> InsertBankAccountSetting(BankAccountSettings BankAccountSetting)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"INSERT INTO BankAccountSettings (
                     GLAccountId, OverDraftLimit, BankChargesAccountId, UseOwnOR, UseOwnPV, UseChequeAutoNo, IsChequeAllowEdit,
                    FormatORId, FormatPVId, ChequeFormatId, AllowEditChequeAutoNo, FormatOR, NextNumberOR, IsResetMonthlyRunningOR,
                    IsResetPerProjectRunningOR, FormatPV, NextNumberPV, IsResetMonthlyRunningPV, IsResetPerProjectRunningPV, 
                    ChequePrefix, ChequeNextNo, OptimisticLockField, BankCharges
                ) VALUES (
                    @GLAccountId, @OverDraftLimit, @BankChargesAccountId, @UseOwnOR, @UseOwnPV, @UseChequeAutoNo, @IsChequeAllowEdit,
                    @FormatORId, @FormatPVId, @ChequeFormatId, @AllowEditChequeAutoNo, @FormatOR, @NextNumberOR, @IsResetMonthlyRunningOR,
                    @IsResetPerProjectRunningOR, @FormatPV, @NextNumberPV, @IsResetMonthlyRunningPV, @IsResetPerProjectRunningPV, 
                    @ChequePrefix, @ChequeNextNo, @OptimisticLockField, @BankCharges
                )";

        var model = await connection.ExecuteAsync(query, BankAccountSetting);
        return model;

    }

    //Update Bank Account Settings from dbo.BankAccountSettings
    public async Task<int> UpdateBankAccountSetting(BankAccountSettings BankAccountSetting)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"UPDATE BankAccountSettings SET 
                                OverDraftLimit = @OverDraftLimit
                                ,BankChargesAccountId = @BankChargesAccountId
                                ,UseOwnOR = @UseOwnOR
                                ,UseOwnPV = @UseOwnPV
                                ,UseChequeAutoNo = @UseChequeAutoNo
                                ,IsChequeAllowEdit = @IsChequeAllowEdit
                                ,FormatORId = @FormatORId
                                ,FormatPVId = @FormatPVId
                                ,ChequeFormatId = @ChequeFormatId
                                ,AllowEditChequeAutoNo = @AllowEditChequeAutoNo
                                ,FormatOR = @FormatOR
                                ,NextNumberOR = @NextNumberOR
                                ,IsResetMonthlyRunningOR = @IsResetMonthlyRunningOR
                                ,IsResetPerProjectRunningOR = @IsResetPerProjectRunningOR
                                ,FormatPV = @FormatPV
                                ,NextNumberPV = @NextNumberPV
                                ,IsResetMonthlyRunningPV = @IsResetMonthlyRunningPV
                                ,IsResetPerProjectRunningPV = @IsResetPerProjectRunningPV
                                ,ChequePrefix = @ChequePrefix
                                ,BankCharges = @BankCharges       
                                 WHERE Id = @Id";
        var rowsAffected = await connection.ExecuteAsync(query, BankAccountSetting);
        return rowsAffected;
    }

    //GET AccountsById
    //public async Task<int> GetAccountsIsBeingUsedById(ChartOfAccountTree account)
    //{
    //    //int totalRowsAffected = 0;
    //    var connectionString = await qneConnection.GetConnectionStringAsync();
    //    using var connection = new SqlConnection(connectionString);



    //    // check muna kung existing na
    //    var RecieptsUsed = await connection.ExecuteScalarAsync<int>(
    //        "SELECT COUNT(1) FROM Receipts WHERE GLAccountCode = @Id",
    //        new
    //        {
    //            account.Id
    //        });

    //    if (RecieptsUsed > 0)
    //    {
    //        return -1; // ibig sabihin duplicate
    //    }

    //    var DebtorsUsed = await connection.ExecuteScalarAsync<int>(
    //    "SELECT COUNT(1) FROM Debtors WHERE ControlAccountId = @Id",
    //    new
    //    {
    //        account.Id
    //    });

    //    if (DebtorsUsed > 0)
    //    {
    //        return -1; // ibig sabihin duplicate
    //    }

    //    return ;
    //}

    public async Task<string> GetAccountsIsBeingUsedById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);

        // check Receipts
        var receiptsUsed = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Receipts WHERE GLAccountId = @Id",
            new
            {
                Id
            }
        );

        if (receiptsUsed > 0)
        {
            return "This account is being used in transactions.";
        }

        // check Debtors
        var debtorsUsed = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Debtors WHERE ControlAccountId = @Id",
            new
            {
               Id
            }
        );

        if (debtorsUsed > 0)
        {
            return "This account is being used in Debtors.";
        }
        
        
        var monthlyStokcUsed = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM MonthlyStockValue WHERE GLAccountId = @Id",
            new
            {
                Id
            }
        );

        if (monthlyStokcUsed > 0)
        {
            return "This account is being used in Monthly Stock.";
        }

        return string.Empty; // ibig sabihin hindi ginagamit kahit saan
    }

}
