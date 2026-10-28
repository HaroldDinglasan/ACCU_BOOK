using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.DescriptionModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Model.QNEModels.SupplierModels;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.GeneralLedgerServices;

public class ReceiptServices
{
    private readonly QneConnectionString qneConnection;
    public CheckExistServices checkExistServices;
    public BankChargesServices bankChargesServices;
    public GLTransactionsServices glTransactionsServices;

    public ReceiptServices(QneConnectionString qneConnection, CheckExistServices checkExistServices, 
        GLTransactionsServices glTransactionsServices, BankReconciliationServices bankReconciliationServices)
    {
        this.qneConnection = qneConnection;
        this.glTransactionsServices = glTransactionsServices;
        

    }

    /// <summary>
    /// LIST OF ALL RECEIPTS BY OBJECT TYPE, OBJECT TYPE 1 = RECEIPT, OBJECT TYPE 2 = PAYMENT
    /// </summary>
    /// <param name="ObjectType"></param>
    /// <returns></returns>
    public async Task<IEnumerable<VW_Receipt>> GetVWReceiptList(int ObjectType)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT
        R.Id,
        ReceiptDate, 
        ReceiptCode,
        DebtorName,
        P.MethodCode, 
        GL.Description, 
        ReferenceNo, 
        CRNCY.CurrencyCode, 
        BankChargesAmountLocal,
        BankCharges, 
        TotalAmount, 
        NetTotalAmount,
        R.Notes, 
        IsCancelled,
        ObjectType
       FROM Receipts R
LEFT OUTER JOIN PaymentMethodMaintenance P ON P.Id = R.PaymentMethodMaintenanceId
LEFT OUTER JOIN GLAccounts GL ON GL.Id = R.GLAccountId
LEFT OUTER JOIN Currencies CRNCY ON CRNCY.Id = R.CurrencyId
WHERE ObjectType = @ObjectType
ORDER BY R.ReceiptDate DESC";
        var model = await connection.QueryAsync<VW_Receipt>(query, new { ObjectType });
        return model;
    }


    public async Task<Receipts> GetHeadersById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Receipts WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<Receipts>(query, new { Id });
        return model;

    }

    //GET ONE OF QUOTATIONSDETAILS DATA
    public async Task<IEnumerable<ReceiptDetails>> GetDetailsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM ReceiptDetails WHERE ReceiptId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<ReceiptDetails>(query, new { Id });
        return model;

    }


    public async Task<IEnumerable<PaymentReceiptInfo>> GetPaymentReceiptInfo()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM PaymentReceiptInfo ORDER BY RowIndex DESC";
        var model = await connection.QueryAsync<PaymentReceiptInfo>(query);
        return model;
    }

    protected CheckExistModel headerCheck = new CheckExistModel();
    protected CheckExistModel detailsCheck = new CheckExistModel();
    public async Task<string> SaveReceiptHeaderAndDetails(Receipts receipts, List<ReceiptDetails> receiptDetailsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

       // await bankChargesServices.SaveBankCharges(bankCharges);

        var inserheaderquery = @"INSERT INTO Receipts
    (
        Id,
        ReceiptCode,
        ReceiptDate,
        GLAccountId,
        ReferenceNo,
        ProjectId,
        CurrencyId,
        CurrencyRate,
        DebtorId,
        DebtorName,
        Description,
        TotalAmount,
        IsCancelled,
        IsDetailForGLPosting,
        GSTTypeId,
        GSTTaxRate,
        GSTAmount,
        GSTTaxAmount,
        TotalAmountLocal,
        GSTAmountLocal,
        WTaxAmount,
        IsPostDatedCheque,
        ChequePreparedDate,
        ChequeReceivedDate,
        IsBouncedCheque,
        BouncedChequeDate,
        IsMultiPayment,
        Balance,
        SalesPersonId,
        DepartmentId,
        CostCentreId,
        TaxTotalAmount,
        TaxTotalAmountLocal,
        NetTotalAmount,
        NetTotalAmountLocal,
        SubtotalAmount,
        SubtotalAmountLocal,
        BankChargesAmountLocal,
        Notes,
        ObjectType,
        TaxDate,
        BankCharges,
        PaymentReceiptInfoId,
        TaxExclusiveTotalAmount,
        TaxExclusiveTotalAmountLocal,
        IsDepositDoc,
        IsTaxInclusive,
        IsTaxInclusiveOnly,
        RoundingAdjustmentAccount,
        RoundingAdjustment,
        RoundingAdjustmentLocal,
        IsRounding,
        OptimisticLockField,
        TaxCodeId,
        TaxRate,
        WTaxTotalAmount,
        WTaxTotalAmountLocal,
        PostGLDescription,
        UseMultiPayment,
        UnmatchAmount,
        Cost,
        CostLocal,
        ClearedDate,
        PaymentMethodMaintenanceId,
        TotalCashAmount,
        TotalChequeAmount,
        ReverseRate,
        Ref1,
        Ref2,
        Ref3,
        Ref4,
        Ref5,
        Remark1,
        Remark2,
        Remark3,
        Remark4,
        Remark5,
        ChequeDate,
        TotalWtaxAmount,
        HasDeposit,
        WTaxDate,
        WVatTotalAmount,
        WVatTotalAmountLocal,
        IsESales,
        IsDetailPosting,
        ExcludeFromKnockoff,
        IsEinvoiceReject,
        DimensionId,
        CorrectionCode,
        EISId,
        TotalWVatAmount,
        TotalRefund
    )
    VALUES
    (
        @Id,
        @ReceiptCode,
        @ReceiptDate,
        @GLAccountId,
        @ReferenceNo,
        @ProjectId,
        @CurrencyId,
        @CurrencyRate,
        @DebtorId,
        @DebtorName,
        @Description,
        @TotalAmount,
        @IsCancelled,
        @IsDetailForGLPosting,
        @GSTTypeId,
        @GSTTaxRate,
        @GSTAmount,
        @GSTTaxAmount,
        @TotalAmountLocal,
        @GSTAmountLocal,
        @WTaxAmount,
        @IsPostDatedCheque,
        @ChequePreparedDate,
        @ChequeReceivedDate,
        @IsBouncedCheque,
        @BouncedChequeDate,
        @IsMultiPayment,
        @Balance,
        @SalesPersonId,
        @DepartmentId,
        @CostCentreId,
        @TaxTotalAmount,
        @TaxTotalAmountLocal,
        @NetTotalAmount,
        @NetTotalAmountLocal,
        @SubtotalAmount,
        @SubtotalAmountLocal,
        @BankChargesAmountLocal,
        @Notes,
        @ObjectType,
        @TaxDate,
        @BankCharges,
        @PaymentReceiptInfoId,
        @TaxExclusiveTotalAmount,
        @TaxExclusiveTotalAmountLocal,
        @IsDepositDoc,
        @IsTaxInclusive,
        @IsTaxInclusiveOnly,
        @RoundingAdjustmentAccount,
        @RoundingAdjustment,
        @RoundingAdjustmentLocal,
        @IsRounding,
        @OptimisticLockField,
        @TaxCodeId,
        @TaxRate,
        @WTaxTotalAmount,
        @WTaxTotalAmountLocal,
        @PostGLDescription,
        @UseMultiPayment,
        @UnmatchAmount,
        @Cost,
        @CostLocal,
        @ClearedDate,
        @PaymentMethodMaintenanceId,
        @TotalCashAmount,
        @TotalChequeAmount,
        @ReverseRate,
        @Ref1,
        @Ref2,
        @Ref3,
        @Ref4,
        @Ref5,
        @Remark1,
        @Remark2,
        @Remark3,
        @Remark4,
        @Remark5,
        @ChequeDate,
        @TotalWtaxAmount,
        @HasDeposit,
        @WTaxDate,
        @WVatTotalAmount,
        @WVatTotalAmountLocal,
        @IsESales,
        @IsDetailPosting,
        @ExcludeFromKnockoff,
        @IsEinvoiceReject,
        @DimensionId,
        @CorrectionCode,
        @EISId,
        @TotalWVatAmount,
        @TotalRefund
    );";
        var updateheaderquery = @"UPDATE Receipts
    SET
        ReceiptCode = @ReceiptCode,
        ReceiptDate = @ReceiptDate,
        GLAccountId = @GLAccountId,
        ReferenceNo = @ReferenceNo,
        ProjectId = @ProjectId,
        CurrencyId = @CurrencyId,
        CurrencyRate = @CurrencyRate,
        DebtorId = @DebtorId,
        DebtorName = @DebtorName,
        Description = @Description,
        TotalAmount = @TotalAmount,
        IsCancelled = @IsCancelled,
        IsDetailForGLPosting = @IsDetailForGLPosting,
        GSTTypeId = @GSTTypeId,
        GSTTaxRate = @GSTTaxRate,
        GSTAmount = @GSTAmount,
        GSTTaxAmount = @GSTTaxAmount,
        TotalAmountLocal = @TotalAmountLocal,
        GSTAmountLocal = @GSTAmountLocal,
        WTaxAmount = @WTaxAmount,
        IsPostDatedCheque = @IsPostDatedCheque,
        ChequePreparedDate = @ChequePreparedDate,
        ChequeReceivedDate = @ChequeReceivedDate,
        IsBouncedCheque = @IsBouncedCheque,
        BouncedChequeDate = @BouncedChequeDate,
        IsMultiPayment = @IsMultiPayment,
        Balance = @Balance,
        SalesPersonId = @SalesPersonId,
        DepartmentId = @DepartmentId,
        CostCentreId = @CostCentreId,
        TaxTotalAmount = @TaxTotalAmount,
        TaxTotalAmountLocal = @TaxTotalAmountLocal,
        NetTotalAmount = @NetTotalAmount,
        NetTotalAmountLocal = @NetTotalAmountLocal,
        SubtotalAmount = @SubtotalAmount,
        SubtotalAmountLocal = @SubtotalAmountLocal,
        BankChargesAmountLocal = @BankChargesAmountLocal,
        Notes = @Notes,
        ObjectType = @ObjectType,
        TaxDate = @TaxDate,
        BankCharges = @BankCharges,
        PaymentReceiptInfoId = @PaymentReceiptInfoId,
        TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
        TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal,
        IsDepositDoc = @IsDepositDoc,
        IsTaxInclusive = @IsTaxInclusive,
        IsTaxInclusiveOnly = @IsTaxInclusiveOnly,
        RoundingAdjustmentAccount = @RoundingAdjustmentAccount,
        RoundingAdjustment = @RoundingAdjustment,
        RoundingAdjustmentLocal = @RoundingAdjustmentLocal,
        IsRounding = @IsRounding,
        OptimisticLockField = OptimisticLockField + 1,
        TaxCodeId = @TaxCodeId,
        TaxRate = @TaxRate,
        WTaxTotalAmount = @WTaxTotalAmount,
        WTaxTotalAmountLocal = @WTaxTotalAmountLocal,
        PostGLDescription = @PostGLDescription,
        UseMultiPayment = @UseMultiPayment,
        UnmatchAmount = @UnmatchAmount,
        Cost = @Cost,
        CostLocal = @CostLocal,
        ClearedDate = @ClearedDate,
        PaymentMethodMaintenanceId = @PaymentMethodMaintenanceId,
        TotalCashAmount = @TotalCashAmount,
        TotalChequeAmount = @TotalChequeAmount,
        ReverseRate = @ReverseRate,
        Ref1 = @Ref1,
        Ref2 = @Ref2,
        Ref3 = @Ref3,
        Ref4 = @Ref4,
        Ref5 = @Ref5,
        Remark1 = @Remark1,
        Remark2 = @Remark2,
        Remark3 = @Remark3,
        Remark4 = @Remark4,
        Remark5 = @Remark5,
        ChequeDate = @ChequeDate,
        TotalWtaxAmount = @TotalWtaxAmount,
        HasDeposit = @HasDeposit,
        WTaxDate = @WTaxDate,
        WVatTotalAmount = @WVatTotalAmount,
        WVatTotalAmountLocal = @WVatTotalAmountLocal,
        IsESales = @IsESales,
        IsDetailPosting = @IsDetailPosting,
        ExcludeFromKnockoff = @ExcludeFromKnockoff,
        IsEinvoiceReject = @IsEinvoiceReject,
        DimensionId = @DimensionId,
        CorrectionCode = @CorrectionCode,
        EISId = @EISId,
        TotalWVatAmount = @TotalWVatAmount,
        TotalRefund = @TotalRefund
    WHERE Id = @Id;";

        var insertdetailquery = @"
        INSERT INTO ReceiptDetails
        (   Id,
            ReceiptId,
            Pos,
            GLAccountId,
            Description,
            ProjectId,
            DepartmentId,
            ReferenceNo,
            Amount,
            IsProcessed,
            WTaxTypeId,
            WTaxAmount,
            GSTTypeId,
            GSTAmount,
            TaxableSales,
            CostCentreId,
            AmountLocal,
            Note,
            ItemTypeCode,
            TaxCodeId,
            IsTaxInclusive,
            TaxRate,
            TaxClass,
            TaxClassValue,
            TaxAmount,
            TaxAmountLocal,
            NetAmount,
            NetAmountLocal,
            SubAmount,
            SubAmountLocal,
            TaxClassValueRef,
            TaxExclusiveAmount,
            TaxExclusiveAmountLocal,
            OptimisticLockField,
            WTaxCode,
            WTaxRate,
            WTaxAmountLocal,
            PaymentReceiptInfoId,
            GLBIRClsInfo,
            TariffCodeId,
            ServicePostingDate,
            ATCCodeId,
            WVatCode,
            WVatRate,
            WVatAmount,
            WVatAmountLocal,
            DimensionId
        )
        VALUES
        (
            @Id,
            @ReceiptId,
            @Pos,
            @GLAccountId,
            @Description,
            @ProjectId,
            @DepartmentId,
            @ReferenceNo,
            @Amount,
            @IsProcessed,
            @WTaxTypeId,
            @WTaxAmount,
            @GSTTypeId,
            @GSTAmount,
            @TaxableSales,
            @CostCentreId,
            @AmountLocal,
            @Note,
            @ItemTypeCode,
            @TaxCodeId,
            @IsTaxInclusive,
            @TaxRate,
            @TaxClass,
            @TaxClassValue,
            @TaxAmount,
            @TaxAmountLocal,
            @NetAmount,
            @NetAmountLocal,
            @SubAmount,
            @SubAmountLocal,
            @TaxClassValueRef,
            @TaxExclusiveAmount,
            @TaxExclusiveAmountLocal,
            @OptimisticLockField,
            @WTaxCode,
            @WTaxRate,
            @WTaxAmountLocal,
            @PaymentReceiptInfoId,
            @GLBIRClsInfo,
            @TariffCodeId,
            @ServicePostingDate,
            @ATCCodeId,
            @WVatCode,
            @WVatRate,
            @WVatAmount,
            @WVatAmountLocal,
            @DimensionId
        )";
        var updatedetailquery = @"
        UPDATE ReceiptDetails
        SET
            ReceiptId = @ReceiptId,
            Pos = @Pos,
            GLAccountId = @GLAccountId,
            Description = @Description,
            ProjectId = @ProjectId,
            DepartmentId = @DepartmentId,
            ReferenceNo = @ReferenceNo,
            Amount = @Amount,
            IsProcessed = @IsProcessed,
            WTaxTypeId = @WTaxTypeId,
            WTaxAmount = @WTaxAmount,
            GSTTypeId = @GSTTypeId,
            GSTAmount = @GSTAmount,
            TaxableSales = @TaxableSales,
            CostCentreId = @CostCentreId,
            AmountLocal = @AmountLocal,
            Note = @Note,
            ItemTypeCode = @ItemTypeCode,
            TaxCodeId = @TaxCodeId,
            IsTaxInclusive = @IsTaxInclusive,
            TaxRate = @TaxRate,
            TaxClass = @TaxClass,
            TaxClassValue = @TaxClassValue,
            TaxAmount = @TaxAmount,
            TaxAmountLocal = @TaxAmountLocal,
            NetAmount = @NetAmount,
            NetAmountLocal = @NetAmountLocal,
            SubAmount = @SubAmount,
            SubAmountLocal = @SubAmountLocal,
            TaxClassValueRef = @TaxClassValueRef,
            TaxExclusiveAmount = @TaxExclusiveAmount,
            TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal,
            OptimisticLockField = OptimisticLockField + 1,
            WTaxCode = @WTaxCode,
            WTaxRate = @WTaxRate,
            WTaxAmountLocal = @WTaxAmountLocal,
            PaymentReceiptInfoId = @PaymentReceiptInfoId,
            GLBIRClsInfo = @GLBIRClsInfo,
            TariffCodeId = @TariffCodeId,
            ServicePostingDate = @ServicePostingDate,
            ATCCodeId = @ATCCodeId,
            WVatCode = @WVatCode,
            WVatRate = @WVatRate,
            WVatAmount = @WVatAmount,
            WVatAmountLocal = @WVatAmountLocal,
            DimensionId = @DimensionId
        WHERE Id = @Id;";

        try
        {
            //headerCheck.Id = receipts.Id;
            //headerCheck.TableName = "Receipts";
            //var existheader = await checkExistServices.RecordExistsByIdAsync(headerCheck);

            var existCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Receipts WHERE Id = @Id",
                new
                {
                    receipts.Id
                },transaction);

            var existheader = existCount > 0;
            if (!existheader)
            {
                await connection.ExecuteAsync(inserheaderquery, receipts, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateheaderquery, receipts, transaction);
            }

            foreach (var receiptDetails in receiptDetailsList)
            {
                //detailsCheck.Id = receiptDetails.Id;
                //detailsCheck.TableName = "ReceiptDetails";
                //var existdetails = await checkExistServices.RecordExistsByIdAsync(detailsCheck);
                var existDetailsCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM ReceiptDetails WHERE Id = @Id",new{receiptDetails.Id },transaction);
                var existdetails = existDetailsCount > 0;
                    if (!existdetails)
                {
                    await connection.ExecuteAsync(insertdetailquery, receiptDetails, transaction);

                }
                else
                {
                    await connection.ExecuteAsync(updatedetailquery, receiptDetails, transaction);

                }
            }


            if (receiptDetailsList != null && receiptDetailsList.Any() && receiptDetailsList.All(x => x.Amount != 0))
            {
                await glTransactionsServices.SaveIntoGLTransaction(receipts, receiptDetailsList);
            }

            //await glTransactionsServices.SaveIntoGLTransaction(receipts, receiptDetailsList);
           
            transaction.Commit();
           

            return $"Success: Receipts saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }



    //protected async Task SaveIntoGLTransaction(Receipts receipts, List<ReceiptDetails> receiptDetailsList)
    //{
    //    var glTransactions = new List<GLTransactions>();

    //    // ============================
    //    // DETAIL LINES (AP / Supplier Account)
    //    // ============================
    //    glTransactions.AddRange(receiptDetailsList.Select(details => CreateGLTransaction(
    //        receipts,
    //        glAccountId: details.GLAccountId,
    //        debit: 0.00M,
    //        credit: details.NetAmount,
    //        tableId: 6,                 // TransactionTables: BillDetails
    //        transactionTypeId: 1,        // TransactionTypes
    //        transactionId: details.Id,
    //        fromAccountId: receipts.GLAccountId,
    //        docType: "ReceiptDetails",
    //        taxCodeId: details.TaxCodeId,
    //        ref1: details.ReferenceNo)));

    //    // ============================
    //    // HEADER LINE(S)
    //    // Posted per-detail-line if IsDetailPosting, otherwise as a single line.
    //    // ============================
    //    if (receipts.IsDetailPosting)
    //    {
    //        glTransactions.AddRange(receiptDetailsList.Select(details => CreateGLTransaction(
    //            receipts,
    //            glAccountId: receipts.GLAccountId,
    //            debit: receipts.NetTotalAmount,
    //            credit: 0.00M,
    //            tableId: 3,               // 3 = RCV, Receipts
    //            transactionTypeId: 1,     // 1 = JV, Journals
    //            transactionId: receipts.Id,
    //            fromAccountId: details.GLAccountId,   // details' GLAccountId used as FromAccountId per detail posting
    //            docType: "Receipts")));
    //    }
    //    else
    //    {
    //        glTransactions.Add(CreateGLTransaction(
    //            receipts,
    //            glAccountId: receipts.GLAccountId,
    //            debit: receipts.NetTotalAmount,
    //            credit: 0.00M,
    //            tableId: 3,               // 3 = RCV, Receipts
    //            transactionTypeId: 1,     // 1 = JV, Journals
    //            transactionId: receipts.Id,
    //            fromAccountId: receipts.GLAccountId,  // header's own GLAccountId used as FromAccountId
    //            docType: "Receipts"));
    //    }

    //    await glTransactionsServices.SaveGLTransactions(glTransactions);
    //}

    ///// <summary>
    ///// Builds a single GLTransactions record, filling in all fields shared across
    ///// detail and header postings and taking only the values that actually vary.
    ///// </summary>
    //private static GLTransactions CreateGLTransaction(
    //    Receipts receipts,
    //    Guid? glAccountId,
    //    decimal debit,
    //    decimal credit,
    //    int tableId,
    //    int transactionTypeId,
    //    Guid transactionId,
    //    Guid? fromAccountId,
    //    string docType,
    //    Guid? taxCodeId = null,
    //    string ref1 = null)
    //{
    //    return new GLTransactions
    //    {
    //        Id = Guid.NewGuid(),
    //        TransactionDate = receipts.ReceiptDate,
    //        PostingDate = receipts.ReceiptDate,

    //        GLAccountId = glAccountId,

    //        Description = receipts.Description,
    //        DetailDescription = receipts.Description,

    //        ProjectId = receipts.ProjectId,
    //        DepartmentId = receipts.DepartmentId,

    //        CurrencyId = receipts.CurrencyId,
    //        CurrencyRate = receipts.CurrencyRate,

    //        Debit = debit,
    //        Credit = credit,
    //        DebitForeign = debit,
    //        CreditForeign = credit,

    //        TableId = tableId,
    //        TransactionTypeId = transactionTypeId,
    //        TransactionId = transactionId,

    //        DocumentId = receipts.Id,
    //        DocumentCode = receipts.ReceiptCode,
    //        ReferenceNo = receipts.ReferenceNo,
    //        IsCancelled = receipts.IsCancelled,

    //        FromAccountId = fromAccountId,

    //        IsPostDatedCheque = false,
    //        JournalType = "OR",
    //        DocType = docType,
    //        TaxCodeId = taxCodeId,
    //       // Ref1 = ref1
    //    };
    //}



    ///OLD LONG APPROACH
    //protected async Task SaveIntoGLTransaction(Receipts receipts, List<ReceiptDetails> receiptDetailsList)
    //{
    //    List<GLTransactions> gLTransactions = new();

    //    // ============================
    //    // DETAILS DATA
    //    // ============================
    //    foreach (var details in receiptDetailsList)
    //    {
    //        gLTransactions.Add(new GLTransactions
    //        {
    //            Id = Guid.NewGuid(),
    //            TransactionDate = receipts.ReceiptDate,
    //            PostingDate = receipts.ReceiptDate,

    //            GLAccountId = details.GLAccountId,     // AP / Supplier Account

    //            Description = receipts.Description,
    //            DetailDescription = receipts.Description,

    //            ProjectId = receipts.ProjectId,
    //            DepartmentId = receipts.DepartmentId,

    //            CurrencyId = receipts.CurrencyId,
    //            CurrencyRate = receipts.CurrencyRate,

    //            Debit = 0.00M,
    //            Credit = details.NetAmount,/////

    //            DebitForeign = 0.00M,////
    //            CreditForeign = details.NetAmount,///

    //            TableId = 21,///THIS ID IS FROM TransactionTables BillDetails
    //            TransactionTypeId = 7, ///THIS ID IS FROM TransactionTypes
    //            TransactionId = details.Id,

    //            DocumentId = receipts.Id,
    //            DocumentCode = receipts.ReceiptCode,
    //            ReferenceNo = receipts.ReferenceNo,
    //            IsCancelled = receipts.IsCancelled,

    //            FromAccountId = receipts.GLAccountId,

    //            IsPostDatedCheque = false,
    //            JournalType = "OR",
    //            DocType = "ReceiptDetails",
    //            TaxCodeId = details.TaxCodeId,
    //            Ref1 = details.ReferenceNo


    //        });
    //    }

    //    // ============================
    //    // HEADER DATA
    //    // ============================
    //    ///CONDITION IF ISDETAILPOSTING IS TRUE OR FALSE, THE HEADER ENTRY WILL BE POSTED TO GL


    //    if (receipts.IsDetailPosting == true)
    //    {
    //        foreach (var details in receiptDetailsList)
    //        {
    //            gLTransactions.Add(new GLTransactions
    //            {
    //                Id = Guid.NewGuid(),
    //                TransactionDate = receipts.ReceiptDate,
    //                PostingDate = receipts.ReceiptDate,

    //                GLAccountId = receipts.GLAccountId,     // AP / Supplier Account

    //                Description = receipts.Description,
    //                DetailDescription = receipts.Description,

    //                ProjectId = receipts.ProjectId,
    //                DepartmentId = receipts.DepartmentId,

    //                CurrencyId = receipts.CurrencyId,
    //                CurrencyRate = receipts.CurrencyRate,

    //                Debit = receipts.NetTotalAmount,///
    //                Credit = 0.00000000M,/////

    //                DebitForeign = receipts.NetTotalAmount,////
    //                CreditForeign = 0.00000000M,///

    //                TableId = 3,///THIS ID IS - 3	RCV	Receipts
    //                TransactionTypeId = 1, ///THIS ID IS - 1	JV	Journals
    //                TransactionId = receipts.Id,

    //                DocumentId = receipts.Id,
    //                DocumentCode = receipts.ReceiptCode,
    //                ReferenceNo = receipts.ReferenceNo,
    //                IsCancelled = receipts.IsCancelled,

    //                //GLAccountId ng details ang gagamitin sa FromAccountId para sa details posting
    //                FromAccountId = details.GLAccountId,

    //                IsPostDatedCheque = false,
    //                JournalType = "OR",
    //                DocType = "Receipts"

    //            });
    //        }


    //    }
    //    else if (receipts.IsDetailPosting == false)
    //    {
    //        gLTransactions.Add(new GLTransactions
    //        {
    //            Id = Guid.NewGuid(),
    //            TransactionDate = receipts.ReceiptDate,
    //            PostingDate = receipts.ReceiptDate,

    //            GLAccountId = receipts.GLAccountId,     // AP / Supplier Account

    //            Description = receipts.Description,
    //            DetailDescription = receipts.Description,

    //            ProjectId = receipts.ProjectId,
    //            DepartmentId = receipts.DepartmentId,

    //            CurrencyId = receipts.CurrencyId,
    //            CurrencyRate = receipts.CurrencyRate,

    //            Debit = receipts.NetTotalAmount,///
    //            Credit = 0.00000000M,/////

    //            DebitForeign = receipts.NetTotalAmount,////
    //            CreditForeign = 0.00000000M,///

    //            TableId = 3,///THIS ID IS - 3	RCV	Receipts
    //            TransactionTypeId = 1, ///THIS ID IS - 1	JV	Journals
    //            TransactionId = receipts.Id,

    //            DocumentId = receipts.Id,
    //            DocumentCode = receipts.ReceiptCode,
    //            ReferenceNo = receipts.ReferenceNo,
    //            IsCancelled = receipts.IsCancelled,

    //            //GLAccountId ng header ang gagamitin sa FromAccountId para sa header posting
    //            FromAccountId = receipts.GLAccountId,

    //            IsPostDatedCheque = false,
    //            JournalType = "OR",
    //            DocType = "Receipts"

    //        });
    //    }



    //    await glTransactionsServices.SaveGLTransactions(gLTransactions);
    //}


}
