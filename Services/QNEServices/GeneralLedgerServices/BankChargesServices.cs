using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.GeneralLedgerServices;

public class BankChargesServices
{
    private readonly QneConnectionString qneConnection;
    public CheckExistServices checkExistServices;
    public BankChargesServices(QneConnectionString qneConnection, CheckExistServices checkExistServices)
    {
        this.qneConnection = qneConnection;
        this.checkExistServices = checkExistServices;
    }
    public async Task<BankCharges> GetBankChargesById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM BankCharges WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<BankCharges>(query, new { Id });
        return model;

    }




    protected CheckExistModel headerCheck = new CheckExistModel();
    public async Task<string> SaveBankCharges(BankCharges bankCharges)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var inserheaderquery = @"INSERT INTO BankCharges
            (
                Id,
                GLAccount,
                UseBankAccountCurrency,
                Currency,
                CurrencyRate,
                TaxCode,
                TaxRate,
                Amount,
                AmountLocal,
                TaxAmount,
                TaxAmountLocal,
                PostingStrategy,
                OptimisticLockField
            )
            VALUES
            (   @Id,
                @GLAccount,
                @UseBankAccountCurrency,
                @Currency,
                @CurrencyRate,
                @TaxCode,
                @TaxRate,
                @Amount,
                @AmountLocal,
                @TaxAmount,
                @TaxAmountLocal,
                @PostingStrategy,
                @OptimisticLockField
            )
        ";

        var updateheaderquery = @"UPDATE BankCharges
           SET
               GLAccount = @GLAccount,
               UseBankAccountCurrency = @UseBankAccountCurrency,
               Currency = @Currency,
               CurrencyRate = @CurrencyRate,
               TaxCode = @TaxCode,
               TaxRate = @TaxRate,
               Amount = @Amount,
               AmountLocal = @AmountLocal,
               TaxAmount = @TaxAmount,
               TaxAmountLocal = @TaxAmountLocal,
               PostingStrategy = @PostingStrategy,
               OptimisticLockField = OptimisticLockField + 1
           WHERE Id = @Id;";



        try
        {
            headerCheck.Id = bankCharges.Id;
            headerCheck.TableName = "BankCharges";
            var existheader = await checkExistServices.RecordExistsByIdAsync(headerCheck);
            if (!existheader)
            {
                await connection.ExecuteAsync(inserheaderquery, bankCharges, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateheaderquery, bankCharges, transaction);
            }

            transaction.Commit();
            return $"Success: Bank Charges saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }
}
