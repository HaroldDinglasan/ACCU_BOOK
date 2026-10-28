using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Model.QNEModels.SupplierModels;
using Accounting_V4.Services.OptimisticLockFieldServices;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.GeneralLedgerServices;

public class GLTransactionsServices
{
    private readonly QneConnectionString qneConnection;
    //public CheckExistServices checkExistServices;
    public BankReconciliationServices bankReconciliationServices;

    public GLTransactionsServices(QneConnectionString qneConnection, CheckExistServices checkExistServices, BankReconciliationServices bankReconciliationServices)
    {

        this.qneConnection = qneConnection;
        this.bankReconciliationServices = bankReconciliationServices;
        // this.checkExistServices = checkExistServices;
    }

    //protected CheckExistModel headerCheck = new CheckExistModel();
    //protected CheckExistModel detailsCheck = new CheckExistModel();



    //SAVING FROM RECEIPTS AND RECEIPT DETAILS INTO GLTRANSACTIONS AND BANKRECONCILIATION
    public async Task SaveIntoGLTransaction(Receipts receipts, List<ReceiptDetails> receiptDetailsList)
    {
        var glTransactions = new List<GLTransactions>();

        // ============================
        // DETAIL LINES (AP / Supplier Account)
        // ============================
        glTransactions.AddRange(receiptDetailsList.Select(details => CreateGLTransaction(
            receipts,
            glAccountId: details.GLAccountId,
            debit: 0.00M,
            credit: details.NetAmount,
            tableId: 6,                 // TransactionTables: BillDetails
            transactionTypeId: 1,        // TransactionTypes
            transactionId: details.Id,
            fromAccountId: receipts.GLAccountId,
            docType: "ReceiptDetails",
            taxCodeId: details.TaxCodeId,
            ref1: details.ReferenceNo)));

        // ============================
        // HEADER LINE(S)
        // Posted per-detail-line if IsDetailPosting, otherwise as a single line.
        // ============================
        if (receipts.IsDetailPosting)
        {
            glTransactions.AddRange(receiptDetailsList.Select(details => CreateGLTransaction(
                receipts,
                glAccountId: receipts.GLAccountId,
                debit: receipts.NetTotalAmount,
                credit: 0.00M,
                tableId: 3,               // 3 = RCV, Receipts
                transactionTypeId: 1,     // 1 = JV, Journals
                transactionId: receipts.Id,
                fromAccountId: details.GLAccountId,   // details' GLAccountId used as FromAccountId per detail posting
                docType: "Receipts")));
        }
        else
        {
            glTransactions.Add(CreateGLTransaction(
                receipts,
                glAccountId: receipts.GLAccountId,
                debit: receipts.NetTotalAmount,
                credit: 0.00M,
                tableId: 3,               // 3 = RCV, Receipts
                transactionTypeId: 1,     // 1 = JV, Journals
                transactionId: receipts.Id,
                fromAccountId: receipts.GLAccountId,  // header's own GLAccountId used as FromAccountId
                docType: "Receipts"));
        }

        await SaveGLTransactions(glTransactions);
        await bankReconciliationServices.SaveIntoBankReconciliation(glTransactions);
    }

    /// <summary>
    /// Builds a single GLTransactions record, filling in all fields shared across
    /// detail and header postings and taking only the values that actually vary.
    /// </summary>
    private static GLTransactions CreateGLTransaction(
        Receipts receipts,
        Guid? glAccountId,
        decimal debit,
        decimal credit,
        int tableId,
        int transactionTypeId,
        Guid transactionId,
        Guid? fromAccountId,
        string docType,
        Guid? taxCodeId = null,
        string ref1 = null)
    {
        return new GLTransactions
        {
            TransactionDate = receipts.ReceiptDate,
            PostingDate = receipts.ReceiptDate,

            GLAccountId = glAccountId,

            Description = receipts.Description,
            DetailDescription = receipts.Description,

            ProjectId = receipts.ProjectId,
            DepartmentId = receipts.DepartmentId,

            CurrencyId = receipts.CurrencyId,
            CurrencyRate = receipts.CurrencyRate,

            Debit = debit,
            Credit = credit,
            DebitForeign = debit,
            CreditForeign = credit,

            TableId = tableId,
            TransactionTypeId = transactionTypeId,
            TransactionId = transactionId,

            DocumentId = receipts.Id,
            DocumentCode = receipts.ReceiptCode,
            ReferenceNo = receipts.ReferenceNo,
            IsCancelled = receipts.IsCancelled,

            FromAccountId = fromAccountId,

            IsPostDatedCheque = false,
            JournalType = "OR",
            DocType = docType,
            TaxCodeId = taxCodeId,
            // Ref1 = ref1
        };
    }


    ///USE ON BILLS, JOURNAL ENTRIES,
    public async Task<string> SaveGLTransactions(List<GLTransactions> gLTransactionsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var checkExistingQuery = @"
        SELECT DISTINCT TransactionId
        FROM GLTransactions
        WHERE TransactionId IN @TransactionIds";

        var deleteQuery = @"
        SET XACT_ABORT ON;
        BEGIN TRANSACTION;

        DELETE FROM BankReconciliation
        WHERE TransactionId = @TransactionId;

        DELETE FROM GLTransactions
        WHERE TransactionId = @TransactionId;

        COMMIT TRANSACTION;";

        var insertHeaderQuery = @"  
        INSERT INTO GLTransactions
        (
            Id,TransactionDate,PostingDate,GLAccountId,Description,DetailDescription,
            ProjectId,DepartmentId,CurrencyId,CurrencyRate,Debit,Credit,
            DebitForeign,CreditForeign,TableId,TransactionTypeId,TransactionId,DocumentId,
            DocumentCode,ReferenceNo,IsCancelled,FromAccountId,IsPostDatedCheque,
            DebitRevaluation,CreditRevaluation,RateRevaluation,CostCentreId,
            JournalType,DocType,OptimisticLockField,TaxCodeId,Ref1,
            IsBouncedCheque,DimensionId
        )
        VALUES
        (
            @Id,@TransactionDate,@PostingDate,@GLAccountId,@Description,@DetailDescription,
            @ProjectId,@DepartmentId,@CurrencyId,@CurrencyRate,@Debit,@Credit,
            @DebitForeign,@CreditForeign,@TableId,@TransactionTypeId,@TransactionId,@DocumentId,
            @DocumentCode,@ReferenceNo,@IsCancelled,@FromAccountId,@IsPostDatedCheque,
            @DebitRevaluation,@CreditRevaluation,@RateRevaluation,@CostCentreId,
            @JournalType,@DocType,@OptimisticLockField,@TaxCodeId,@Ref1,
            @IsBouncedCheque,@DimensionId
        )";

        try
        {
            var distinctTransactionIds = gLTransactionsList
                .Select(x => x.TransactionId)
                .Distinct()
                .ToList();

            // Step 1: Check kung anong mga TransactionId ang existing na sa DB
            var existingTransactionIds = (await connection.QueryAsync<Guid>(
                checkExistingQuery,
                new
                {
                    TransactionIds = distinctTransactionIds
                })).ToHashSet();

            // Step 2: I-delete lang ang mga existing na TransactionId
            foreach (var transactionId in existingTransactionIds)
            {
                await connection.ExecuteAsync(deleteQuery, new { TransactionId = transactionId });
            }

            // Step 3: I-insert ang bagong data (existing man dati o hindi)
            foreach (var gLTransactions in gLTransactionsList)
            {
                await connection.ExecuteAsync(insertHeaderQuery, gLTransactions);
            }

            return "Success: GLTransactions saved successfully.";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }



    ///USE IN SALESINVOICES
    public async Task<string> SaveInvoiceGLTransactions(
        List<GLTransactions> gLTransactionsList,
        Guid invoiceId)
    {
        if (gLTransactionsList == null || gLTransactionsList.Count == 0)
            return "Error: Walang invoice GL transactions na ise-save.";

        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            // Delete ONLY GL entries belonging to this invoice.
            // Hindi gagalawin ang Bills at Journal Entries.
            var deleteQuery = @"
            DELETE FROM GLTransactions
            WHERE DocumentId = @InvoiceId
              AND DocType IN ('InvoiceDetails', 'Invoices');";

            await connection.ExecuteAsync(
                deleteQuery,
                new
                {
                    InvoiceId = invoiceId
                },
                transaction);

            // Ensure unique primary key per GL row.
            foreach (var item in gLTransactionsList)
            {
                if (item.Id == Guid.Empty)
                    item.Id = Guid.NewGuid();
            }

            var insertQuery = @"
            INSERT INTO GLTransactions
            (
                Id, TransactionDate, PostingDate, GLAccountId,
                Description, DetailDescription, ProjectId, DepartmentId,
                CurrencyId, CurrencyRate, Debit, Credit,
                DebitForeign, CreditForeign, TableId, TransactionTypeId,
                TransactionId, DocumentId, DocumentCode, ReferenceNo,
                IsCancelled, FromAccountId, IsPostDatedCheque,
                DebitRevaluation, CreditRevaluation, RateRevaluation,
                CostCentreId, JournalType, DocType, OptimisticLockField,
                TaxCodeId, Ref1, IsBouncedCheque, DimensionId
            )
            VALUES
            (
                @Id, @TransactionDate, @PostingDate, @GLAccountId,
                @Description, @DetailDescription, @ProjectId, @DepartmentId,
                @CurrencyId, @CurrencyRate, @Debit, @Credit,
                @DebitForeign, @CreditForeign, @TableId, @TransactionTypeId,
                @TransactionId, @DocumentId, @DocumentCode, @ReferenceNo,
                @IsCancelled, @FromAccountId, @IsPostDatedCheque,
                @DebitRevaluation, @CreditRevaluation, @RateRevaluation,
                @CostCentreId, @JournalType, @DocType, @OptimisticLockField,
                @TaxCodeId, @Ref1, @IsBouncedCheque, @DimensionId
            );";

            foreach (var item in gLTransactionsList)
            {
                await connection.ExecuteAsync(
                    insertQuery,
                    item,
                    transaction);
            }

            transaction.Commit();

            return $"Success: {gLTransactionsList.Count} invoice GL rows saved.";
        }
        catch (Exception ex)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Panatilihin ang original exception.
            }

            return $"Error saving invoice GL: {ex.Message}";
        }
    }




    //public async Task<string> SaveGLTransactions(List<GLTransactions> gLTransactionsList)
    //{
    //    var connectionString = await qneConnection.GetConnectionStringAsync();
    //    using var connection = new SqlConnection(connectionString);
    //    await connection.OpenAsync();
    //    using var transaction = connection.BeginTransaction();

    //    var checkExistingQuery = @"
    //    SELECT DISTINCT TransactionId
    //    FROM GLTransactions
    //    WHERE TransactionId IN @TransactionIds";

    //    var deleteQuery = @"
    //    SET XACT_ABORT ON;
    //    BEGIN TRANSACTION;

    //    DELETE FROM BankReconciliation
    //    WHERE TransactionId = @TransactionId;

    //    DELETE FROM GLTransactions
    //    WHERE TransactionId = @TransactionId;

    //    COMMIT TRANSACTION;";

    //    var insertHeaderQuery = @"
    //    INSERT INTO GLTransactions
    //    (
    //        Id,TransactionDate,PostingDate,GLAccountId,Description,DetailDescription,
    //        ProjectId,DepartmentId,CurrencyId,CurrencyRate,Debit,Credit,
    //        DebitForeign,CreditForeign,TableId,TransactionTypeId,TransactionId,DocumentId,
    //        DocumentCode,ReferenceNo,IsCancelled,FromAccountId,IsPostDatedCheque,
    //        DebitRevaluation,CreditRevaluation,RateRevaluation,CostCentreId,
    //        JournalType,DocType,OptimisticLockField,TaxCodeId,Ref1,
    //        IsBouncedCheque,DimensionId
    //    )
    //    VALUES
    //    (
    //        @Id,@TransactionDate,@PostingDate,@GLAccountId,@Description,@DetailDescription,
    //        @ProjectId,@DepartmentId,@CurrencyId,@CurrencyRate,@Debit,@Credit,
    //        @DebitForeign,@CreditForeign,@TableId,@TransactionTypeId,@TransactionId,@DocumentId,
    //        @DocumentCode,@ReferenceNo,@IsCancelled,@FromAccountId,@IsPostDatedCheque,
    //        @DebitRevaluation,@CreditRevaluation,@RateRevaluation,@CostCentreId,
    //        @JournalType,@DocType,@OptimisticLockField,@TaxCodeId,@Ref1,
    //        @IsBouncedCheque,@DimensionId
    //    )";

    //    try
    //    {
    //        var distinctTransactionIds = gLTransactionsList
    //            .Select(x => x.TransactionId)
    //            .Distinct()
    //            .ToList();

    //        // Step 1: Check kung anong mga TransactionId ang existing na sa DB
    //        var existingTransactionIds = (await connection.QueryAsync<Guid>(
    //            checkExistingQuery,
    //            new
    //            {
    //                TransactionIds = distinctTransactionIds
    //            },
    //            transaction)).ToHashSet();

    //        // Step 2: I-delete lang ang mga existing na TransactionId
    //        foreach (var transactionId in existingTransactionIds)
    //        {
    //            await connection.ExecuteAsync(deleteQuery, new { TransactionId = transactionId }, transaction);
    //        }

    //        // Step 3: I-insert ang bagong data (existing man dati o hindi)
    //        foreach (var gLTransactions in gLTransactionsList)
    //        {
    //            await connection.ExecuteAsync(insertHeaderQuery, gLTransactions, transaction);
    //        }
    //        transaction.Commit();

    //        return "Success: GLTransactions saved successfully.";
    //    }
    //    catch (Exception ex)
    //    {
    //        transaction.Rollback();
    //        return $"Error: {ex.Message}";
    //    }
    //}


    //public async Task<string> SaveGLTransactions(List<GLTransactions> gLTransactionsList)
    //{
    //    var connectionString = await qneConnection.GetConnectionStringAsync();
    //    var connection = new SqlConnection(connectionString);
    //    await connection.OpenAsync();
    //    using var transaction = connection.BeginTransaction();
    //    var inserheaderquery = @"
    //    INSERT INTO GLTransactions
    //    (
    //        Id,TransactionDate,PostingDate,GLAccountId,Description,DetailDescription,
    //        ProjectId,DepartmentId,CurrencyId,CurrencyRate,Debit,Credit,
    //        DebitForeign,CreditForeign,TableId,TransactionTypeId,TransactionId,DocumentId,
    //        DocumentCode,ReferenceNo,IsCancelled,FromAccountId,IsPostDatedCheque,
    //        DebitRevaluation,CreditRevaluation,RateRevaluation,CostCentreId,
    //        JournalType,DocType,OptimisticLockField,TaxCodeId,Ref1,
    //        IsBouncedCheque,DimensionId
    //    )
    //    VALUES
    //    (
    //        @Id,@TransactionDate,@PostingDate,@GLAccountId,@Description,@DetailDescription,
    //        @ProjectId,@DepartmentId,@CurrencyId,@CurrencyRate,@Debit,@Credit,
    //        @DebitForeign,@CreditForeign,@TableId,@TransactionTypeId,@TransactionId,@DocumentId,
    //        @DocumentCode,@ReferenceNo,@IsCancelled,@FromAccountId,@IsPostDatedCheque,
    //        @DebitRevaluation,@CreditRevaluation,@RateRevaluation,@CostCentreId,
    //        @JournalType,@DocType,@OptimisticLockField,@TaxCodeId,@Ref1,
    //        @IsBouncedCheque,@DimensionId
    //    )";
    //    //var updateheaderquery = @"
    //    //UPDATE GLTransactions
    //    //SET
    //    //    TransactionDate=@TransactionDate,PostingDate=@PostingDate,GLAccountId=@GLAccountId,
    //    //    Description=@Description,DetailDescription=@DetailDescription,
    //    //    ProjectId=@ProjectId,DepartmentId=@DepartmentId,CurrencyId=@CurrencyId,
    //    //    CurrencyRate=@CurrencyRate,Debit=@Debit,Credit=@Credit,
    //    //    DebitForeign=@DebitForeign,CreditForeign=@CreditForeign,
    //    //    TableId=@TableId,TransactionTypeId=@TransactionTypeId,
    //    //    TransactionId=@TransactionId,DocumentId=@DocumentId,
    //    //    DocumentCode=@DocumentCode,ReferenceNo=@ReferenceNo,
    //    //    IsCancelled=@IsCancelled,FromAccountId=@FromAccountId,
    //    //    IsPostDatedCheque=@IsPostDatedCheque,DebitRevaluation=@DebitRevaluation,
    //    //    CreditRevaluation=@CreditRevaluation,RateRevaluation=@RateRevaluation,
    //    //    CostCentreId=@CostCentreId,JournalType=@JournalType,DocType=@DocType,
    //    //    OptimisticLockField= OptimisticLockField + 1,TaxCodeId=@TaxCodeId,
    //    //    Ref1=@Ref1,IsBouncedCheque=@IsBouncedCheque,
    //    //    DimensionId=@DimensionId
    //    //WHERE Id=@Id";


    //    try
    //    {
    //        foreach (var gLTransactions in gLTransactionsList)
    //        {
    //            //detailsCheck.Id = gLTransactions.Id;
    //            //detailsCheck.TableName = "GLTransactions";
    //            //var existdetails = await checkExistServices.RecordExistsByIdAsync(detailsCheck);
    //            //if (!existdetails)
    //            //{
    //            //    await connection.ExecuteAsync(inserheaderquery, gLTransactions, transaction);

    //            //}
    //            //else
    //            //{
    //            //    await connection.ExecuteAsync(updateheaderquery, gLTransactions, transaction);
    //            //}
    //            await connection.ExecuteAsync(inserheaderquery, gLTransactions, transaction);
    //        }


    //        transaction.Commit();
    //        return $"Success: GLTransactions saved successfully.";
    //    }
    //    catch (Exception ex)
    //    {
    //        transaction.Rollback();
    //        return $"Error: {ex.Message}";
    //    }

    //}

}
