using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.GeneralLedgerServices;

public class BankReconciliationServices
{
    private readonly QneConnectionString qneConnection;


    public BankReconciliationServices(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }




    public async Task SaveIntoBankReconciliation(List<GLTransactions> gLTransactionsList)
    {
        var bankReconciliations = new List<BankReconciliation>();

        foreach (var gl in gLTransactionsList)
        {
            bankReconciliations.Add(new BankReconciliation
            {
                EntryDate = gl.PostingDate,
                // PostingDate = gl.PostingDate,


                GLAccountId = gl.GLAccountId.Value,

                GLTransactionId = gl.Id,

                Description = gl.Description,
                // DetailDescription = gl.Description,

                // ProjectId = gl.ProjectId,
                // DepartmentId = gl.DepartmentId,

                CurrencyId = gl.CurrencyId,
                CurrencyRate = gl.CurrencyRate,

                Debit = gl.Debit.Value,
                Credit = gl.Credit.Value,
                DebitForeign = gl.DebitForeign.Value,
                CreditForeign = gl.CreditForeign.Value,

                TableId = gl.TableId,
                TransactionTypeId = gl.TransactionTypeId,
                TransactionId = gl.TransactionId,

                DocumentId = gl.DocumentId,
                DocumentCode = gl.DocumentCode,
                ReferenceNo = gl.ReferenceNo,

                IsPostDatedCheque = false,

                ObjectType = 29
            });
        }

        await SaveBankReconciliation(bankReconciliations);
    }



    public async Task<string> SaveBankReconciliation(List<BankReconciliation> bankReconciliationList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var validAccountsQuery = @"
        SELECT Id
        FROM GLAccounts
        WHERE Id IN @GLAccountIds
          AND SpecialAccountId = 3
          AND IsSubAccount <> 1";

        var checkExistingQuery = @"
        SELECT DISTINCT TransactionId
        FROM BankReconciliation
        WHERE TransactionId IN @TransactionIds";

        var deleteQuery = @"
        DELETE FROM BankReconciliation
        WHERE TransactionId = @TransactionId";

        var insertQuery = @"
        INSERT INTO BankReconciliation
        (
            Id, GLAccountId, OpeningPos, GLTransactionId, EntryDate, Description,
            CurrencyId, CurrencyRate, Debit, Credit, DebitForeign, CreditForeign,
            TableId, TransactionTypeId, TransactionId, DocumentId, DocumentCode,
            ReferenceNo, ClearDate, IsCancelled, IsPostDatedCheque,
            DebitRevaluation, CreditRevaluation, RateRevaluation,
            OptimisticLockField, ObjectType
        )
        VALUES
        (
            @Id, @GLAccountId, @OpeningPos, @GLTransactionId, @EntryDate, @Description,
            @CurrencyId, @CurrencyRate, @Debit, @Credit, @DebitForeign, @CreditForeign,
            @TableId, @TransactionTypeId, @TransactionId, @DocumentId, @DocumentCode,
            @ReferenceNo, @ClearDate, @IsCancelled, @IsPostDatedCheque,
            @DebitRevaluation, @CreditRevaluation, @RateRevaluation,
            @OptimisticLockField, @ObjectType
        );";

        try
        {
            // Step 1: Alamin kung aling GLAccount ang pasok sa condition
            var accountIds = bankReconciliationList
                .Select(x => x.GLAccountId)
                .Distinct()
                .ToList();

            var validAccountIds = (await connection.QueryAsync<Guid>(
                validAccountsQuery,
                new
                {
                    GLAccountIds = accountIds
                },
                transaction)).ToHashSet();

            // Step 2: I-skip ang mga hindi pasok
            var validList = bankReconciliationList
                .Where(x => validAccountIds.Contains(x.GLAccountId))
                .ToList();

            var skippedCount = bankReconciliationList.Count - validList.Count;

            if (validList.Count == 0)
            {
                transaction.Rollback();
                return $"Nothing saved: all {skippedCount} record(s) were skipped.";
            }

            // Step 3: Check existing TransactionId (valid rows lang)
            var distinctTransactionIds = validList
                .Where(x => x.TransactionId.HasValue)
                .Select(x => x.TransactionId!.Value)
                .Distinct()
                .ToList();

            var existingTransactionIds = distinctTransactionIds.Count == 0
                ? new HashSet<Guid>()
                : (await connection.QueryAsync<Guid>(
                    checkExistingQuery,
                    new
                    {
                        TransactionIds = distinctTransactionIds
                    },
                    transaction)).ToHashSet();

            // Step 4: I-delete ang existing
            foreach (var transactionId in existingTransactionIds)
            {
                await connection.ExecuteAsync(deleteQuery, new { TransactionId = transactionId }, transaction);
            }

            // Step 5: I-insert ang valid rows
            await connection.ExecuteAsync(insertQuery, validList, transaction);

            transaction.Commit();
            return $"Success: {validList.Count} saved, {skippedCount} skipped.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }
}
