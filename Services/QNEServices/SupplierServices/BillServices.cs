using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.SupplierModels;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.GeneralLedgerServices;
using Accounting_V4.Services.QNEServices.GLAccountServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.SupplierServices;

public class BillServices
{
    private readonly QneConnectionString qneConnection;
    public CheckExistServices checkExistServices;
    public GLTransactionsServices glTransactionsServices;
    public BillServices(QneConnectionString qneConnection, CheckExistServices checkExistServices, GLTransactionsServices glTransactionsServices)
    {
        this.qneConnection = qneConnection;
        this.checkExistServices = checkExistServices;
        this.glTransactionsServices = glTransactionsServices;
    }

    public async Task<IEnumerable<VW_Bills>> GetVWBillList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT  B.Id
              ,BillDate
            ,BillCode
            ,CREDITOR.CompanyCode AS SupplierCode
            ,CREDITOR.CompanyName AS BillFrom
            ,CREDITOR.Address1 AS Address
            ,ReferenceNo
            ,TERMS.Term
            ,DueDate
            ,PURCH.Name AS PurchaserName
            ,PURCH.StaffCode AS PurchaserCode
            ,B.CreditorInvNo AS SupplierINV
            ,PROJECT.ProjectCode
            ,CRNCY.CurrencyCode
            ,B.CurrencyId
            ,CurrencyRate
            ,B.Description
            ,B.Description2
            ,TotalAmount
            ,TaxTotalAmount
            ,NetTotalAmount
            ,IsCancelled
            ,IsDetailForGLPosting
            ,IsReceived
            ,PostDate
            ,TotalAmountLocal
            ,B.GSTTypeId
            ,GSTTaxRate
            ,GSTAmount
            ,GSTTaxAmount
            ,GSTAmountLocal
            ,GSTTaxAmountLocal
            ,DepartmentId
            ,CostCentreId
            ,FromDocType
            ,FromDocId
            ,TaxTotalAmountLocal
            ,NetTotalAmountLocal
            ,SubtotalAmount
            ,SubtotalAmountLocal
            ,ImportDeclarationNo
            ,B.Notes AS Note
        FROM Bills B
        LEFT OUTER JOIN Currencies CRNCY
        ON CRNCY.Id = B.CurrencyId
        LEFT OUTER JOIN Creditors CREDITOR
        ON CREDITOR.Id = B.CreditorId
        LEFT OUTER JOIN Terms TERMS
        ON TERMS.Id = B.TermId
        LEFT OUTER JOIN Purchasers PURCH
        ON PURCH.Id = B.PurchaserId
        LEFT OUTER JOIN Projects PROJECT
        ON PROJECT.Id = B.ProjectId
        ORDER BY BillDate DESC";
        var model = await connection.QueryAsync<VW_Bills>(query, commandType: CommandType.Text);
        return model;
    }

    public async Task<Bill> GetBillsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Bills WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<Bill>(query, new { Id });
        return model;

    }

    //GET ONE OF QUOTATIONSDETAILS DATA
    public async Task<IEnumerable<BillDetails>> GetBillsDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM BillDetails WHERE BillId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<BillDetails>(query, new { Id });
        return model;

    }



    protected CheckExistModel headerCheck = new CheckExistModel();
    protected CheckExistModel detailsCheck = new CheckExistModel();
    public async Task<string> SaveBillHeaderAndDetails(Bill bill, List<BillDetails> billDetailsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var insertheaderquery = @"
        INSERT INTO Bills
        (
            Id,BillCode,BillDate,BillFrom,CreditorId,ReferenceNo,
            TermId,DueDate,PurchaserId,ProjectId,CurrencyId,CurrencyRate,
            Description,Description2,TotalAmount,IsCancelled,IsDetailForGLPosting,IsReceived,
            PostDate,TotalAmountLocal,GSTTypeId,GSTTaxRate,GSTAmount,GSTTaxAmount,
            GSTAmountLocal,GSTTaxAmountLocal,DepartmentId,CostCentreId,FromDocType,FromDocId,
            TaxTotalAmount,TaxTotalAmountLocal,NetTotalAmount,NetTotalAmountLocal,
            SubtotalAmount,SubtotalAmountLocal,ImportDeclarationNo,Notes,ItemDescriptions,
            IsTaxInclusive,CreditorInvNo,ObjectType,TaxExclusiveTotalAmount,
            TaxExclusiveTotalAmountLocal,TaxDate,IsTaxInclusiveOnly,RoundingAdjustmentAccount,
            RoundingAdjustment,RoundingAdjustmentLocal,IsRounding,IsK1GoodsRemoved,
            K1GoodsRemovedId,OptimisticLockField,WTaxTotalAmount,WTaxTotalAmountLocal,
            SgstTotalAmount,SgstTotalAmountLocal,CgstTotalAmount,CgstTotalAmountLocal,
            IgstTotalAmount,IgstTotalAmountLocal,IsImportedServicePosted,IsDeferredVAT,
            DocTaxCodeId,DocTaxRate,ReverseRate,IsNonTaxInvoice,ForecastDueDate,
            TotalPaid,IsReliefImported,IsSelfBearWTax,IsDetailPosting,
            ExcludeFromKnockoff,IsEinvoiceReject,DimensionId,TotalServiceAmount,
            TotalServiceAmountLocal,UniqueIdentifiedNo,ValidatedDate
        )
        VALUES
        (
            @Id,@BillCode,@BillDate,@BillFrom,@CreditorId,@ReferenceNo,
            @TermId,@DueDate,@PurchaserId,@ProjectId,@CurrencyId,@CurrencyRate,
            @Description,@Description2,@TotalAmount,@IsCancelled,@IsDetailForGLPosting,@IsReceived,
            @PostDate,@TotalAmountLocal,@GSTTypeId,@GSTTaxRate,@GSTAmount,@GSTTaxAmount,
            @GSTAmountLocal,@GSTTaxAmountLocal,@DepartmentId,@CostCentreId,@FromDocType,@FromDocId,
            @TaxTotalAmount,@TaxTotalAmountLocal,@NetTotalAmount,@NetTotalAmountLocal,
            @SubtotalAmount,@SubtotalAmountLocal,@ImportDeclarationNo,@Notes,@ItemDescriptions,
            @IsTaxInclusive,@CreditorInvNo,@ObjectType,@TaxExclusiveTotalAmount,
            @TaxExclusiveTotalAmountLocal,@TaxDate,@IsTaxInclusiveOnly,@RoundingAdjustmentAccount,
            @RoundingAdjustment,@RoundingAdjustmentLocal,@IsRounding,@IsK1GoodsRemoved,
            @K1GoodsRemovedId,@OptimisticLockField,@WTaxTotalAmount,@WTaxTotalAmountLocal,
            @SgstTotalAmount,@SgstTotalAmountLocal,@CgstTotalAmount,@CgstTotalAmountLocal,
            @IgstTotalAmount,@IgstTotalAmountLocal,@IsImportedServicePosted,@IsDeferredVAT,
            @DocTaxCodeId,@DocTaxRate,@ReverseRate,@IsNonTaxInvoice,@ForecastDueDate,
            @TotalPaid,@IsReliefImported,@IsSelfBearWTax,@IsDetailPosting,
            @ExcludeFromKnockoff,@IsEinvoiceReject,@DimensionId,@TotalServiceAmount,
            @TotalServiceAmountLocal,@UniqueIdentifiedNo,@ValidatedDate
        );";
        var updateheaderquery = @"
UPDATE Bills
SET
    BillCode=@BillCode,BillDate=@BillDate,BillFrom=@BillFrom,CreditorId=@CreditorId,ReferenceNo=@ReferenceNo,
    TermId=@TermId,DueDate=@DueDate,PurchaserId=@PurchaserId,ProjectId=@ProjectId,CurrencyId=@CurrencyId,
    CurrencyRate=@CurrencyRate,Description=@Description,Description2=@Description2,TotalAmount=@TotalAmount,
    IsCancelled=@IsCancelled,IsDetailForGLPosting=@IsDetailForGLPosting,IsReceived=@IsReceived,
    PostDate=@PostDate,TotalAmountLocal=@TotalAmountLocal,GSTTypeId=@GSTTypeId,GSTTaxRate=@GSTTaxRate,
    GSTAmount=@GSTAmount,GSTTaxAmount=@GSTTaxAmount,GSTAmountLocal=@GSTAmountLocal,
    GSTTaxAmountLocal=@GSTTaxAmountLocal,DepartmentId=@DepartmentId,CostCentreId=@CostCentreId,
    FromDocType=@FromDocType,FromDocId=@FromDocId,TaxTotalAmount=@TaxTotalAmount,
    TaxTotalAmountLocal=@TaxTotalAmountLocal,NetTotalAmount=@NetTotalAmount,
    NetTotalAmountLocal=@NetTotalAmountLocal,SubtotalAmount=@SubtotalAmount,
    SubtotalAmountLocal=@SubtotalAmountLocal,ImportDeclarationNo=@ImportDeclarationNo,
    Notes=@Notes,ItemDescriptions=@ItemDescriptions,IsTaxInclusive=@IsTaxInclusive,
    CreditorInvNo=@CreditorInvNo,ObjectType=@ObjectType,
    TaxExclusiveTotalAmount=@TaxExclusiveTotalAmount,
    TaxExclusiveTotalAmountLocal=@TaxExclusiveTotalAmountLocal,TaxDate=@TaxDate,
    IsTaxInclusiveOnly=@IsTaxInclusiveOnly,RoundingAdjustmentAccount=@RoundingAdjustmentAccount,
    RoundingAdjustment=@RoundingAdjustment,RoundingAdjustmentLocal=@RoundingAdjustmentLocal,
    IsRounding=@IsRounding,IsK1GoodsRemoved=@IsK1GoodsRemoved,
    K1GoodsRemovedId=@K1GoodsRemovedId,OptimisticLockField= OptimisticLockField + 1,
    WTaxTotalAmount=@WTaxTotalAmount,WTaxTotalAmountLocal=@WTaxTotalAmountLocal,
    SgstTotalAmount=@SgstTotalAmount,SgstTotalAmountLocal=@SgstTotalAmountLocal,
    CgstTotalAmount=@CgstTotalAmount,CgstTotalAmountLocal=@CgstTotalAmountLocal,
    IgstTotalAmount=@IgstTotalAmount,IgstTotalAmountLocal=@IgstTotalAmountLocal,
    IsImportedServicePosted=@IsImportedServicePosted,IsDeferredVAT=@IsDeferredVAT,
    DocTaxCodeId=@DocTaxCodeId,DocTaxRate=@DocTaxRate,ReverseRate=@ReverseRate,
    IsNonTaxInvoice=@IsNonTaxInvoice,ForecastDueDate=@ForecastDueDate,
    TotalPaid=@TotalPaid,IsReliefImported=@IsReliefImported,
    IsSelfBearWTax=@IsSelfBearWTax,IsDetailPosting=@IsDetailPosting,
    ExcludeFromKnockoff=@ExcludeFromKnockoff,IsEinvoiceReject=@IsEinvoiceReject,
    DimensionId=@DimensionId,TotalServiceAmount=@TotalServiceAmount,
    TotalServiceAmountLocal=@TotalServiceAmountLocal,
    UniqueIdentifiedNo=@UniqueIdentifiedNo,ValidatedDate=@ValidatedDate
WHERE Id=@Id;";
        var insertdetailquery = @"
        INSERT INTO BillDetails
        (
            Id,BillId,Pos,GLAccountId,Description,ProjectId,
            ReferenceNo,Amount,DepartmentId,CostCentreId,AmountLocal,Note,
            ItemTypeCode,TaxCodeId,IsTaxInclusive,TaxRate,TaxClass,TaxClassValue,
            TaxAmount,TaxAmountLocal,NetAmount,NetAmountLocal,SubAmount,SubAmountLocal,
            TaxExclusiveAmount,TaxExclusiveAmountLocal,TaxClassValueRef,OptimisticLockField,
            WTaxCode,WTaxRate,WTaxAmount,WTaxAmountLocal,SgstAmount,CgstAmount,
            IgstAmount,SgstAmountLocal,CgstAmountLocal,IgstAmountLocal,K1GoodsRemovedId,
            TariffCodeId,LocalOneCentsDiff,FromReliefImportation,DimensionId,
            ServiceAmount,ServiceAmountLocal
        )
        VALUES
        (
            @Id,@BillId,@Pos,@GLAccountId,@Description,@ProjectId,
            @ReferenceNo,@Amount,@DepartmentId,@CostCentreId,@AmountLocal,@Note,
            @ItemTypeCode,@TaxCodeId,@IsTaxInclusive,@TaxRate,@TaxClass,@TaxClassValue,
            @TaxAmount,@TaxAmountLocal,@NetAmount,@NetAmountLocal,@SubAmount,@SubAmountLocal,
            @TaxExclusiveAmount,@TaxExclusiveAmountLocal,@TaxClassValueRef,@OptimisticLockField,
            @WTaxCode,@WTaxRate,@WTaxAmount,@WTaxAmountLocal,@SgstAmount,@CgstAmount,
            @IgstAmount,@SgstAmountLocal,@CgstAmountLocal,@IgstAmountLocal,@K1GoodsRemovedId,
            @TariffCodeId,@LocalOneCentsDiff,@FromReliefImportation,@DimensionId,
            @ServiceAmount,@ServiceAmountLocal
        );";
        var updatedetailquery = @"
        UPDATE BillDetails
        SET
            BillId=@BillId,Pos=@Pos,GLAccountId=@GLAccountId,Description=@Description,
            ProjectId=@ProjectId,ReferenceNo=@ReferenceNo,Amount=@Amount,
            DepartmentId=@DepartmentId,CostCentreId=@CostCentreId,AmountLocal=@AmountLocal,
            Note=@Note,ItemTypeCode=@ItemTypeCode,TaxCodeId=@TaxCodeId,
            IsTaxInclusive=@IsTaxInclusive,TaxRate=@TaxRate,TaxClass=@TaxClass,
            TaxClassValue=@TaxClassValue,TaxAmount=@TaxAmount,
            TaxAmountLocal=@TaxAmountLocal,NetAmount=@NetAmount,
            NetAmountLocal=@NetAmountLocal,SubAmount=@SubAmount,
            SubAmountLocal=@SubAmountLocal,TaxExclusiveAmount=@TaxExclusiveAmount,
            TaxExclusiveAmountLocal=@TaxExclusiveAmountLocal,
            TaxClassValueRef=@TaxClassValueRef,OptimisticLockField= OptimisticLockField + 1,
            WTaxCode=@WTaxCode,WTaxRate=@WTaxRate,WTaxAmount=@WTaxAmount,
            WTaxAmountLocal=@WTaxAmountLocal,SgstAmount=@SgstAmount,
            CgstAmount=@CgstAmount,IgstAmount=@IgstAmount,
            SgstAmountLocal=@SgstAmountLocal,CgstAmountLocal=@CgstAmountLocal,
            IgstAmountLocal=@IgstAmountLocal,K1GoodsRemovedId=@K1GoodsRemovedId,
            TariffCodeId=@TariffCodeId,LocalOneCentsDiff=@LocalOneCentsDiff,
            FromReliefImportation=@FromReliefImportation,DimensionId=@DimensionId,
            ServiceAmount=@ServiceAmount,ServiceAmountLocal=@ServiceAmountLocal
        WHERE Id=@Id;";

        try
        {
            headerCheck.Id = bill.Id;
            headerCheck.TableName = "Bills";
            var existheader = await checkExistServices.RecordExistsByIdAsync(headerCheck);
            if (!existheader)
            {
                await connection.ExecuteAsync(insertheaderquery, bill, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateheaderquery, bill, transaction);
            }

            ///dito yung issue ko sa details
            foreach (var billDetails in billDetailsList)
            {
                var existdetails = (await connection.ExecuteScalarAsync<int>("SELECT Id FROM BillDetails WHERE BillId = @BillId", new { BillId = billDetails.Id }, transaction) > 0);
                //var existdetails = await checkExistServices.RecordExistsByIdAsync(detailsCheck);
                if (!existdetails)
                {
                    await connection.ExecuteAsync(insertdetailquery, billDetails, transaction);

                }
                else
                {
                    await connection.ExecuteAsync(updatedetailquery, billDetails, transaction);

                }
            }
            transaction.Commit();
             await SaveIntoGLTransaction(bill,billDetailsList);
            return $"Success: Bills saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }


    protected async Task SaveIntoGLTransaction(Bill bill, List<BillDetails> billDetailsList)
    {
        List<GLTransactions> gLTransactions = new();


        // ============================
        // HEADER ENTRY (Supplier/AP)
        // ============================
        gLTransactions.Add(new GLTransactions
        {
            Id = Guid.NewGuid(),
            TransactionDate = bill.BillDate,
            PostingDate = bill.PostDate,

            GLAccountId = bill.GLAccountId,     // AP / Supplier Account

            Description = bill.Description,
            DetailDescription = bill.Description,

            ProjectId = bill.ProjectId,
            DepartmentId = bill.DepartmentId,

            CurrencyId = bill.CurrencyId,
            CurrencyRate = bill.CurrencyRate,

            Debit = 0.00M,///
            Credit = bill.NetTotalAmount,/////

            DebitForeign = 0.00M,////
            CreditForeign = bill.NetTotalAmount,///

            TableId = 20,///THIS ID IS FROM TransactionTables Bill
            TransactionTypeId = 7, ///THIS ID IS FROM TransactionTypes
            TransactionId = bill.Id,

            DocumentId = bill.Id,
            DocumentCode = bill.BillCode,
            ReferenceNo = bill.ReferenceNo,
            IsCancelled = bill.IsCancelled,
            FromAccountId = bill.GLAccountId,
            IsPostDatedCheque = false,
            JournalType = "BIL",
            DocType = "Bills",
            TaxCodeId = bill.DocTaxCodeId,
            //Ref1 = bill.ReferenceNo



        });

        // ============================
        // DETAILS
        // ============================
        foreach (var details in billDetailsList)
        {
            gLTransactions.Add(new GLTransactions
            {
                Id = Guid.NewGuid(),
                TransactionDate = bill.BillDate,
                PostingDate = bill.PostDate,

                GLAccountId = details.GLAccountId,     // AP / Supplier Account

                Description = bill.Description,
                DetailDescription = bill.BillFrom,

                ProjectId = bill.ProjectId,
                DepartmentId = bill.DepartmentId,

                CurrencyId = bill.CurrencyId,
                CurrencyRate = bill.CurrencyRate,

                Debit = details.NetAmount,///
                Credit = 0.00M,/////

                DebitForeign = details.NetAmount,////
                CreditForeign = 0.00M,///

                TableId = 21,///THIS ID IS FROM TransactionTables BillDetails
                TransactionTypeId = 7, ///THIS ID IS FROM TransactionTypes
                TransactionId = details.Id,

                DocumentId = bill.Id,
                DocumentCode = bill.BillCode,
                ReferenceNo = bill.ReferenceNo,
                IsCancelled = bill.IsCancelled,
                FromAccountId = bill.GLAccountId,
                IsPostDatedCheque = false,
                JournalType = "BIL",
                DocType = "BillDetails",
                TaxCodeId = bill.DocTaxCodeId,
                //Ref1 = bill.ReferenceNo


            });
        }

        await glTransactionsServices.SaveGLTransactions(gLTransactions);
    }


    public async Task<string> DeleteBillDetailRows(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var deletedetailsquery = @"DELETE FROM BillDetails WHERE Id=@Id";
        var p = new DynamicParameters();
        p.Add("@Id", Id);

        try
        {
            var deleteDetailRows = await connection.ExecuteAsync(deletedetailsquery, p, transaction);
            //return $"Success {affectedDetailsInsertRows} added!";
            transaction.Commit();
            return $"({deleteDetailRows}) rows deleted!";

        }
        catch (SqlException ex)
        {

            transaction.Rollback();
            return ex.Message;
            throw;
        }

    }
}
