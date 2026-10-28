using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.SupplierModels;
using Accounting_V4.Pages.QNEPages.Supplier.Bill;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.GeneralLedgerServices;

public class JournalServices
{
    private readonly QneConnectionString qneConnection;
    public CheckExistServices checkExistServices;
    public GLTransactionsServices glTransactionsServices;
    public JournalServices(QneConnectionString qneConnection, CheckExistServices checkExistServices, GLTransactionsServices glTransactionsServices)
    {
        this.qneConnection = qneConnection;
        this.checkExistServices = checkExistServices;
        this.glTransactionsServices = glTransactionsServices;
    }

    public async Task<IEnumerable<VW_Journals>> GetVWJournalsList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT JOURNAL.Id
      ,JournalDate
      ,JournalCode
      ,JOURNAL.Description
      ,CRNCY.CurrencyCode
      ,PRJCT.ProjectCode
      ,CurrencyRate
      ,TotalDebit
      ,TotalCredit
      ,Notes
      ,IsCancelled
      ,IsTaxInclusive

      ,GSTRate
      ,IsRevaluationDR
      ,IsRevaluationCR
      ,IsRevaluationGL
      ,IsBadDebtRelief
      ,IsBadDebtRecovery
      ,IsUnpaidPurchase
      ,IsPaymentForUnpaidPurchase
      ,JournalEntryType
      ,IsTaxInclusiveOnly
      ,ObjectType
      ,TaxDate
      ,IsImportedServicePosted
      ,RowIndex
      ,ForecastDueDate
      ,JOURNAL.ReverseRate
      ,ProjectId
      ,IsSSTBadDebt
      ,CostCentreId
      ,IsESales
      ,ExcludeFromKnockoff
      ,IsEinvoiceReject
      ,DimensionId
  FROM Journals JOURNAL
  LEFT OUTER JOIN Currencies CRNCY
  ON CRNCY.Id = JOURNAL.CurrencyId
  LEFT OUTER JOIN Projects PRJCT
  ON PRJCT.Id = JOURNAL.ProjectId
  ORDER BY JOURNAL.JournalDate DESC";
        var model = await connection.QueryAsync<VW_Journals>(query, commandType: CommandType.Text);
        return model;
    }




    public async Task<Journals> GetHeadersById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Journals WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<Journals>(query, new { Id });
        return model;

    }

    //GET ONE OF QUOTATIONSDETAILS DATA
    public async Task<IEnumerable<JournalDetails>> GetDetailsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM JournalDetails WHERE JournalId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<JournalDetails>(query, new { Id });
        return model;

    }



    protected CheckExistModel headerCheck = new CheckExistModel();
    protected CheckExistModel detailsCheck = new CheckExistModel();
    public async Task<string> SaveJournalHeaderAndDetails(Journals journals, List<JournalDetails> journalDetailsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var inserheaderquery = @"
        INSERT INTO Journals
        (
            Id,
            JournalCode,
            JournalDate,
            CurrencyId,
            CurrencyRate,
            Description,
            Notes,
            IsCancelled,
            TotalDebit,
            TotalCredit,
            IsTaxInclusive,
            GSTRate,
            IsRevaluationDR,
            IsRevaluationCR,
            IsRevaluationGL,
            IsBadDebtRelief,
            IsBadDebtRecovery,
            IsUnpaidPurchase,
            IsPaymentForUnpaidPurchase,
            JournalEntryType,
            IsTaxInclusiveOnly,
            OptimisticLockField,
            ObjectType,
            TaxDate,
            IsImportedServicePosted,
            ForecastDueDate,
            ReverseRate,
            ProjectId,
            IsSSTBadDebt,
            CostCentreId,
            IsESales,
            ExcludeFromKnockoff,
            IsEinvoiceReject,
            DimensionId
        )
        VALUES
        (
            @Id,
            @JournalCode,
            @JournalDate,
            @CurrencyId,
            @CurrencyRate,
            @Description,
            @Notes,
            @IsCancelled,
            @TotalDebit,
            @TotalCredit,
            @IsTaxInclusive,
            @GSTRate,
            @IsRevaluationDR,
            @IsRevaluationCR,
            @IsRevaluationGL,
            @IsBadDebtRelief,
            @IsBadDebtRecovery,
            @IsUnpaidPurchase,
            @IsPaymentForUnpaidPurchase,
            @JournalEntryType,
            @IsTaxInclusiveOnly,
            @OptimisticLockField,
            @ObjectType,
            @TaxDate,
            @IsImportedServicePosted,
            @ForecastDueDate,
            @ReverseRate,
            @ProjectId,
            @IsSSTBadDebt,
            @CostCentreId,
            @IsESales,
            @ExcludeFromKnockoff,
            @IsEinvoiceReject,
            @DimensionId
        );";
        var updateheaderquery = @"
        UPDATE Journals
        SET
            JournalCode = @JournalCode,
            JournalDate = @JournalDate,
            CurrencyId = @CurrencyId,
            CurrencyRate = @CurrencyRate,
            Description = @Description,
            Notes = @Notes,
            IsCancelled = @IsCancelled,
            TotalDebit = @TotalDebit,
            TotalCredit = @TotalCredit,
            IsTaxInclusive = @IsTaxInclusive,
            GSTRate = @GSTRate,
            IsRevaluationDR = @IsRevaluationDR,
            IsRevaluationCR = @IsRevaluationCR,
            IsRevaluationGL = @IsRevaluationGL,
            IsBadDebtRelief = @IsBadDebtRelief,
            IsBadDebtRecovery = @IsBadDebtRecovery,
            IsUnpaidPurchase = @IsUnpaidPurchase,
            IsPaymentForUnpaidPurchase = @IsPaymentForUnpaidPurchase,
            JournalEntryType = @JournalEntryType,
            IsTaxInclusiveOnly = @IsTaxInclusiveOnly,
            OptimisticLockField = OptimisticLockField + 1,
            ObjectType = @ObjectType,
            TaxDate = @TaxDate,
            IsImportedServicePosted = @IsImportedServicePosted,
            ForecastDueDate = @ForecastDueDate,
            ReverseRate = @ReverseRate,
            ProjectId = @ProjectId,
            IsSSTBadDebt = @IsSSTBadDebt,
            CostCentreId = @CostCentreId,
            IsESales = @IsESales,
            ExcludeFromKnockoff = @ExcludeFromKnockoff,
            IsEinvoiceReject = @IsEinvoiceReject,
            DimensionId = @DimensionId
        WHERE Id = @Id;";

        var insertdetailquery = @"
INSERT INTO JournalDetails
(
    Id,
    JournalId,
    Pos,
    GLAccountId,
    Description,
    ReferenceNo,
    Debit,
    Credit,
    ProjectId,
    DepartmentId,
    GSTType,
    GSTGroup,
    GSTAmount,
    GSTTaxAmount,
    TaxablePurchases,
    TaxableSales,
    TaxablePurchase,
    CostCentreId,
    DebitLocal,
    CreditLocal,
    Note,
    ItemTypeCode,
    TaxCodeId,
    IsTaxInclusive,
    TaxRate,
    TaxClass,
    TaxClassValue,
    TaxDebit,
    TaxDebitLocal,
    TaxCredit,
    TaxCreditLocal,
    NetDebit,
    NetCredit,
    NetDebitLocal,
    NetCreditLocal,
    IsOutputTax,
    BDJournalDetailId,
    BDInvoiceDetailId,
    BDARDNDetailId,
    BDBillDetailId,
    BDAPDNDetailId,
    RecoveredJournalDetailId,
    TaxClassValueRef,
    TaxExclusiveDebit,
    TaxExclusiveCredit,
    TaxExclusiveDebitLocal,
    TaxExclusiveCreditLocal,
    OptimisticLockField,
    BDPastInvoiceId,
    BDPastARDNId,
    BDPastBillId,
    BDPastAPDNId,
    BDBillId,
    GLBIRClsInfo,
    PaymentReceiptInfoId,
    SgstDebit,
    CgstDebit,
    IgstDebit,
    SgstDebitLocal,
    CgstDebitLocal,
    IgstDebitLocal,
    SgstCredit,
    CgstCredit,
    IgstCredit,
    SgstCreditLocal,
    CgstCreditLocal,
    IgstCreditLocal,
    TariffCodeId,
    RecoveredPastInvoiceId,
    BDJournalId,
    BDInvoiceId,
    BDARDNId,
    BDAPDNId,
    IsCreditable,
    WTaxCode,
    WTaxDebit,
    WTaxCredit,
    IsDeferredVAT,
    ServiceTaxCode,
    ServicePostingDate,
    ServiceTaxAmount,
    LocalOneCentsDiff,
    DimensionId,
    UniqueIdentifiedNo
)
VALUES
(
    @Id,
    @JournalId,
    @Pos,
    @GLAccountId,
    @Description,
    @ReferenceNo,
    @Debit,
    @Credit,
    @ProjectId,
    @DepartmentId,
    @GSTType,
    @GSTGroup,
    @GSTAmount,
    @GSTTaxAmount,
    @TaxablePurchases,
    @TaxableSales,
    @TaxablePurchase,
    @CostCentreId,
    @DebitLocal,
    @CreditLocal,
    @Note,
    @ItemTypeCode,
    @TaxCodeId,
    @IsTaxInclusive,
    @TaxRate,
    @TaxClass,
    @TaxClassValue,
    @TaxDebit,
    @TaxDebitLocal,
    @TaxCredit,
    @TaxCreditLocal,
    @NetDebit,
    @NetCredit,
    @NetDebitLocal,
    @NetCreditLocal,
    @IsOutputTax,
    @BDJournalDetailId,
    @BDInvoiceDetailId,
    @BDARDNDetailId,
    @BDBillDetailId,
    @BDAPDNDetailId,
    @RecoveredJournalDetailId,
    @TaxClassValueRef,
    @TaxExclusiveDebit,
    @TaxExclusiveCredit,
    @TaxExclusiveDebitLocal,
    @TaxExclusiveCreditLocal,
    @OptimisticLockField,
    @BDPastInvoiceId,
    @BDPastARDNId,
    @BDPastBillId,
    @BDPastAPDNId,
    @BDBillId,
    @GLBIRClsInfo,
    @PaymentReceiptInfoId,
    @SgstDebit,
    @CgstDebit,
    @IgstDebit,
    @SgstDebitLocal,
    @CgstDebitLocal,
    @IgstDebitLocal,
    @SgstCredit,
    @CgstCredit,
    @IgstCredit,
    @SgstCreditLocal,
    @CgstCreditLocal,
    @IgstCreditLocal,
    @TariffCodeId,
    @RecoveredPastInvoiceId,
    @BDJournalId,
    @BDInvoiceId,
    @BDARDNId,
    @BDAPDNId,
    @IsCreditable,
    @WTaxCode,
    @WTaxDebit,
    @WTaxCredit,
    @IsDeferredVAT,
    @ServiceTaxCode,
    @ServicePostingDate,
    @ServiceTaxAmount,
    @LocalOneCentsDiff,
    @DimensionId,
    @UniqueIdentifiedNo
)";
        var updatedetailquery = @"
UPDATE JournalDetails
SET
    JournalId = @JournalId,
    Pos = @Pos,
    GLAccountId = @GLAccountId,
    Description = @Description,
    ReferenceNo = @ReferenceNo,
    Debit = @Debit,
    Credit = @Credit,
    ProjectId = @ProjectId,
    DepartmentId = @DepartmentId,
    GSTType = @GSTType,
    GSTGroup = @GSTGroup,
    GSTAmount = @GSTAmount,
    GSTTaxAmount = @GSTTaxAmount,
    TaxablePurchases = @TaxablePurchases,
    TaxableSales = @TaxableSales,
    TaxablePurchase = @TaxablePurchase,
    CostCentreId = @CostCentreId,
    DebitLocal = @DebitLocal,
    CreditLocal = @CreditLocal,
    Note = @Note,
    ItemTypeCode = @ItemTypeCode,
    TaxCodeId = @TaxCodeId,
    IsTaxInclusive = @IsTaxInclusive,
    TaxRate = @TaxRate,
    TaxClass = @TaxClass,
    TaxClassValue = @TaxClassValue,
    TaxDebit = @TaxDebit,
    TaxDebitLocal = @TaxDebitLocal,
    TaxCredit = @TaxCredit,
    TaxCreditLocal = @TaxCreditLocal,
    NetDebit = @NetDebit,
    NetCredit = @NetCredit,
    NetDebitLocal = @NetDebitLocal,
    NetCreditLocal = @NetCreditLocal,
    IsOutputTax = @IsOutputTax,
    BDJournalDetailId = @BDJournalDetailId,
    BDInvoiceDetailId = @BDInvoiceDetailId,
    BDARDNDetailId = @BDARDNDetailId,
    BDBillDetailId = @BDBillDetailId,
    BDAPDNDetailId = @BDAPDNDetailId,
    RecoveredJournalDetailId = @RecoveredJournalDetailId,
    TaxClassValueRef = @TaxClassValueRef,
    TaxExclusiveDebit = @TaxExclusiveDebit,
    TaxExclusiveCredit = @TaxExclusiveCredit,
    TaxExclusiveDebitLocal = @TaxExclusiveDebitLocal,
    TaxExclusiveCreditLocal = @TaxExclusiveCreditLocal,
    OptimisticLockField = OptimisticLockField + 1,
    BDPastInvoiceId = @BDPastInvoiceId,
    BDPastARDNId = @BDPastARDNId,
    BDPastBillId = @BDPastBillId,
    BDPastAPDNId = @BDPastAPDNId,
    BDBillId = @BDBillId,
    GLBIRClsInfo = @GLBIRClsInfo,
    PaymentReceiptInfoId = @PaymentReceiptInfoId,
    SgstDebit = @SgstDebit,
    CgstDebit = @CgstDebit,
    IgstDebit = @IgstDebit,
    SgstDebitLocal = @SgstDebitLocal,
    CgstDebitLocal = @CgstDebitLocal,
    IgstDebitLocal = @IgstDebitLocal,
    SgstCredit = @SgstCredit,
    CgstCredit = @CgstCredit,
    IgstCredit = @IgstCredit,
    SgstCreditLocal = @SgstCreditLocal,
    CgstCreditLocal = @CgstCreditLocal,
    IgstCreditLocal = @IgstCreditLocal,
    TariffCodeId = @TariffCodeId,
    RecoveredPastInvoiceId = @RecoveredPastInvoiceId,
    BDJournalId = @BDJournalId,
    BDInvoiceId = @BDInvoiceId,
    BDARDNId = @BDARDNId,
    BDAPDNId = @BDAPDNId,
    IsCreditable = @IsCreditable,
    WTaxCode = @WTaxCode,
    WTaxDebit = @WTaxDebit,
    WTaxCredit = @WTaxCredit,
    IsDeferredVAT = @IsDeferredVAT,
    ServiceTaxCode = @ServiceTaxCode,
    ServicePostingDate = @ServicePostingDate,
    ServiceTaxAmount = @ServiceTaxAmount,
    LocalOneCentsDiff = @LocalOneCentsDiff,
    DimensionId = @DimensionId,
    UniqueIdentifiedNo = @UniqueIdentifiedNo
WHERE Id = @Id";

        try
        {
            headerCheck.Id = journals.Id;
            headerCheck.TableName = "Journals";
            var existheader = await checkExistServices.RecordExistsByIdAsync(headerCheck);
            if (!existheader)
            {
                await connection.ExecuteAsync(inserheaderquery, journals, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateheaderquery, journals, transaction);
            }

            foreach (var journalDetails in journalDetailsList)
            {
                detailsCheck.Id = journalDetails.Id;
                detailsCheck.TableName = "JournalDetails";
                var existdetails = await checkExistServices.RecordExistsByIdAsync(detailsCheck);
                if (!existdetails)
                {
                    await connection.ExecuteAsync(insertdetailquery, journalDetails, transaction);

                }
                else
                {
                    await connection.ExecuteAsync(updatedetailquery, journalDetails, transaction);

                }
            }

            transaction.Commit();
            await SaveIntoGLTransaction(journals, journalDetailsList);
            return $"Success: Journals saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }



    protected async Task SaveIntoGLTransaction(Journals journals, List<JournalDetails> journalDetailsList)
    {
        List<GLTransactions> gLTransactions = new();


        // ============================
        // DETAILS
        // ============================
        foreach (var details in journalDetailsList)
        {
            gLTransactions.Add(new GLTransactions
            {
                Id = Guid.NewGuid(),
                TransactionDate = journals.JournalDate,
                PostingDate = journals.JournalDate,

                GLAccountId = details.GLAccountId,     // AP / Supplier Account

                Description = journals.Description,
                DetailDescription = details.Description,

                ProjectId = journals.ProjectId,
                //DepartmentId = journals.DepartmentId, NO DEPARTMENT ID IN JournalDetails

                CurrencyId = journals.CurrencyId,
                CurrencyRate = journals.CurrencyRate,

                Debit = details.Debit,///
                Credit = details.Credit,/////

                DebitForeign = details.Debit,////
                CreditForeign = details.Credit,///

                TableId = 1,///THIS IS OBJECT TABLE ID
                TransactionTypeId = 1, 
                TransactionId = details.Id,

                DocumentId = journals.Id,
                DocumentCode = journals.JournalCode,
                ReferenceNo = details.ReferenceNo,
                IsCancelled = journals.IsCancelled,
                //FromAccountId = journals.GLAccountId,
                IsPostDatedCheque = false,
                JournalType = "JV",
                DocType = "Journals",
                TaxCodeId = details.TaxCodeId,
                //Ref1 = journals.ReferenceNo


            });
        }

        await glTransactionsServices.SaveGLTransactions(gLTransactions);
    }

}
