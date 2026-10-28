using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.InvoiceModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.TransferServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.InvoicesServices;

public class InvoiceServices
{
    private readonly QneConnectionString qneConnection;
    public CheckExistServices checkExistServices;

    protected CheckExistModel checkExistModel = new CheckExistModel();
    protected CheckExistModel sidetailsCheckModel = new CheckExistModel();

    public InvoiceServices(QneConnectionString qneConnection, CheckExistServices checkExistServices)
    {
        this.qneConnection = qneConnection;
        this.checkExistServices = checkExistServices;
    }


    public async Task<string> SaveInvoicesWithTransferAsync(InvoicesHeader invoicesHeader, List<InvoiceDetails> invoiceDetails)
    {
        checkExistModel.Id = invoicesHeader.Id;
        checkExistModel.TableName = "Invoices";

        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
       

        ///FIX QUERY HERE LACK OF TABLES AND REMOVED RowIndex 


        var insertHeaderQuery = @"
        INSERT INTO Invoices
        (
            Id,
            InvoiceCode,
            InvoiceDate,
            InvoiceTo,
            DebtorId,
            ReferenceNo,
            TermId,
            DueDate,
            SalesPersonId,
            ProjectId,
            CurrencyId,
            CurrencyRate,
            Description,
            TotalAmount,
            IsCancelled,
            IsDetailForGLPosting,
            TotalAmountLocal,
            GSTTypeId,
            GSTTaxRate,
            GSTAmount,
            GSTTaxAmount,
            GSTAmountLocal,
            GSTTaxAmountLocal,
            Balance,
            DepartmentId,
            CostCentreId,
            FromDocType,
            FromDocId,
            TaxTotalAmount,
            TaxTotalAmountLocal,
            NetTotalAmount,
            NetTotalAmountLocal,
            SubtotalAmount,
            SubtotalAmountLocal,
            ExportCountry,
            Notes,
            ItemDescriptions,
            IsTaxInclusive,
            ObjectType,
            TaxDate,
            TaxExclusiveTotalAmount,
            TaxExclusiveTotalAmountLocal,
            IsTaxInclusiveOnly,
            RoundingAdjustmentAccount,
            RoundingAdjustment,
            RoundingAdjustmentLocal,
            IsRounding,
            OptimisticLockField,
            WTaxTotalAmount,
            WTaxTotalAmountLocal,
            SgstTotalAmount,
            SgstTotalAmountLocal,
            CgstTotalAmount,
            CgstTotalAmountLocal,
            IgstTotalAmount,
            IgstTotalAmountLocal,
            GSTReverseDate,
            IsDeferredVAT,
            DocTaxCodeId,
            DocTaxRate,
            IsDifferentPOS,
            PlaceOfSupplyId,
            ReverseCharge,
            IsPaidUnderPA,
            EGSTIN,
            IsAmendedInvoice,
            IsLinkedWithAmendedInvoice,
            ReverseRate,
            IsNonTaxInvoice,
            IsSSTBadDebt,
            PostingDate,
            ForecastDueDate,
            TotalServiceAmount,
            TotalServiceAmountLocal,
            WVatTotalAmount,
            WVatTotalAmountLocal,
            TotalPaid,
            AdjustmentAmount,
            TotalMatchedAmount,
            IsFinanceChargeInv,
            PropertyManagementBillingId,
            IsMigratedBadDebtRelief,
            MigratedReliefAmount,
            IsESales,
            IsSelfBearWTax,
            IsDetailPosting,
            ExcludeFromKnockoff,
            IsEinvoiceReject,
            DimensionId,
            CorrectionCode,
            EISId
        )
        VALUES
        (
            @Id,
            @InvoiceCode,
            @InvoiceDate,
            @InvoiceTo,
            @DebtorId,
            @ReferenceNo,
            @TermId,
            @DueDate,
            @SalesPersonId,
            @ProjectId,
            @CurrencyId,
            @CurrencyRate,
            @Description,
            @TotalAmount,
            @IsCancelled,
            @IsDetailForGLPosting,
            @TotalAmountLocal,
            @GSTTypeId,
            @GSTTaxRate,
            @GSTAmount,
            @GSTTaxAmount,
            @GSTAmountLocal,
            @GSTTaxAmountLocal,
            @Balance,
            @DepartmentId,
            @CostCentreId,
            @FromDocType,
            @FromDocId,
            @TaxTotalAmount,
            @TaxTotalAmountLocal,
            @NetTotalAmount,
            @NetTotalAmountLocal,
            @SubtotalAmount,
            @SubtotalAmountLocal,
            @ExportCountry,
            @Notes,
            @ItemDescriptions,
            @IsTaxInclusive,
            @ObjectType,
            @TaxDate,
            @TaxExclusiveTotalAmount,
            @TaxExclusiveTotalAmountLocal,
            @IsTaxInclusiveOnly,
            @RoundingAdjustmentAccount,
            @RoundingAdjustment,
            @RoundingAdjustmentLocal,
            @IsRounding,
            @OptimisticLockField,
            @WTaxTotalAmount,
            @WTaxTotalAmountLocal,
            @SgstTotalAmount,
            @SgstTotalAmountLocal,
            @CgstTotalAmount,
            @CgstTotalAmountLocal,
            @IgstTotalAmount,
            @IgstTotalAmountLocal,
            @GSTReverseDate,
            @IsDeferredVAT,
            @DocTaxCodeId,
            @DocTaxRate,
            @IsDifferentPOS,
            @PlaceOfSupplyId,
            @ReverseCharge,
            @IsPaidUnderPA,
            @EGSTIN,
            @IsAmendedInvoice,
            @IsLinkedWithAmendedInvoice,
            @ReverseRate,
            @IsNonTaxInvoice,
            @IsSSTBadDebt,
            @PostingDate,
            @ForecastDueDate,
            @TotalServiceAmount,
            @TotalServiceAmountLocal,
            @WVatTotalAmount,
            @WVatTotalAmountLocal,
            @TotalPaid,
            @AdjustmentAmount,
            @TotalMatchedAmount,
            @IsFinanceChargeInv,
            @PropertyManagementBillingId,
            @IsMigratedBadDebtRelief,
            @MigratedReliefAmount,
            @IsESales,
            @IsSelfBearWTax,
            @IsDetailPosting,
            @ExcludeFromKnockoff,
            @IsEinvoiceReject,
            @DimensionId,
            @CorrectionCode,
            @EISId
        );";
        var updateHeaderQuery = @"
        UPDATE Invoices
        SET
            InvoiceCode = @InvoiceCode,
            InvoiceDate = @InvoiceDate,
            InvoiceTo = @InvoiceTo,
            DebtorId = @DebtorId,
            ReferenceNo = @ReferenceNo,
            TermId = @TermId,
            DueDate = @DueDate,
            SalesPersonId = @SalesPersonId,
            ProjectId = @ProjectId,
            CurrencyId = @CurrencyId,
            CurrencyRate = @CurrencyRate,
            Description = @Description,
            TotalAmount = @TotalAmount,
            IsCancelled = @IsCancelled,
            IsDetailForGLPosting = @IsDetailForGLPosting,
            TotalAmountLocal = @TotalAmountLocal,
            GSTTypeId = @GSTTypeId,
            GSTTaxRate = @GSTTaxRate,
            GSTAmount = @GSTAmount,
            GSTTaxAmount = @GSTTaxAmount,
            GSTAmountLocal = @GSTAmountLocal,
            GSTTaxAmountLocal = @GSTTaxAmountLocal,
            Balance = @Balance,
            DepartmentId = @DepartmentId,
            CostCentreId = @CostCentreId,
            FromDocType = @FromDocType,
            FromDocId = @FromDocId,
            TaxTotalAmount = @TaxTotalAmount,
            TaxTotalAmountLocal = @TaxTotalAmountLocal,
            NetTotalAmount = @NetTotalAmount,
            NetTotalAmountLocal = @NetTotalAmountLocal,
            SubtotalAmount = @SubtotalAmount,
            SubtotalAmountLocal = @SubtotalAmountLocal,
            ExportCountry = @ExportCountry,
            Notes = @Notes,
            ItemDescriptions = @ItemDescriptions,
            IsTaxInclusive = @IsTaxInclusive,
            ObjectType = @ObjectType,
            TaxDate = @TaxDate,
            TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
            TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal,
            IsTaxInclusiveOnly = @IsTaxInclusiveOnly,
            RoundingAdjustmentAccount = @RoundingAdjustmentAccount,
            RoundingAdjustment = @RoundingAdjustment,
            RoundingAdjustmentLocal = @RoundingAdjustmentLocal,
            IsRounding = @IsRounding,
            OptimisticLockField = @OptimisticLockField,
            WTaxTotalAmount = @WTaxTotalAmount,
            WTaxTotalAmountLocal = @WTaxTotalAmountLocal,
            SgstTotalAmount = @SgstTotalAmount,
            SgstTotalAmountLocal = @SgstTotalAmountLocal,
            CgstTotalAmount = @CgstTotalAmount,
            CgstTotalAmountLocal = @CgstTotalAmountLocal,
            IgstTotalAmount = @IgstTotalAmount,
            IgstTotalAmountLocal = @IgstTotalAmountLocal,
            GSTReverseDate = @GSTReverseDate,
            IsDeferredVAT = @IsDeferredVAT,
            DocTaxCodeId = @DocTaxCodeId,
            DocTaxRate = @DocTaxRate,
            IsDifferentPOS = @IsDifferentPOS,
            PlaceOfSupplyId = @PlaceOfSupplyId,
            ReverseCharge = @ReverseCharge,
            IsPaidUnderPA = @IsPaidUnderPA,
            EGSTIN = @EGSTIN,
            IsAmendedInvoice = @IsAmendedInvoice,
            IsLinkedWithAmendedInvoice = @IsLinkedWithAmendedInvoice,
            ReverseRate = @ReverseRate,
            IsNonTaxInvoice = @IsNonTaxInvoice,
            IsSSTBadDebt = @IsSSTBadDebt,
            PostingDate = @PostingDate,
            ForecastDueDate = @ForecastDueDate,
            TotalServiceAmount = @TotalServiceAmount,
            TotalServiceAmountLocal = @TotalServiceAmountLocal,
            WVatTotalAmount = @WVatTotalAmount,
            WVatTotalAmountLocal = @WVatTotalAmountLocal,
            TotalPaid = @TotalPaid,
            AdjustmentAmount = @AdjustmentAmount,
            TotalMatchedAmount = @TotalMatchedAmount,
            IsFinanceChargeInv = @IsFinanceChargeInv,
            PropertyManagementBillingId = @PropertyManagementBillingId,
            IsMigratedBadDebtRelief = @IsMigratedBadDebtRelief,
            MigratedReliefAmount = @MigratedReliefAmount,
            IsESales = @IsESales,
            IsSelfBearWTax = @IsSelfBearWTax,
            IsDetailPosting = @IsDetailPosting,
            ExcludeFromKnockoff = @ExcludeFromKnockoff,
            IsEinvoiceReject = @IsEinvoiceReject,
            DimensionId = @DimensionId,
            CorrectionCode = @CorrectionCode,
            EISId = @EISId
        WHERE Id = @Id;
        ";


        var insertDetailQuery = @"
        INSERT INTO InvoiceDetails
        (
            Id,
            InvoiceId,
            Pos,
            GLAccountId,
            Description,
            ReferenceNo,
            ProjectId,
            DepartmentId,
            CostCentreId,
            Amount,
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
            TaxExclusiveAmount,
            TaxExclusiveAmountLocal,
            TaxClassValueRef,
            OptimisticLockField,
            WTaxCode,
            WTaxRate,
            WTaxAmount,
            WTaxAmountLocal,
            SgstAmount,
            CgstAmount,
            IgstAmount,
            SgstAmountLocal,
            CgstAmountLocal,
            IgstAmountLocal,
            TariffCodeId,
            LocalOneCentsDiff,
            ServiceAmount,
            ServiceAmountLocal,
            WVatCode,
            WVatRate,
            WVatAmount,
            WVatAmountLocal,
            ATCCodeId,
            DimensionId
        )
        VALUES
        (
            @Id,
            @InvoiceId,
            @Pos,
            @GLAccountId,
            @Description,
            @ReferenceNo,
            @ProjectId,
            @DepartmentId,
            @CostCentreId,
            @Amount,
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
            @TaxExclusiveAmount,
            @TaxExclusiveAmountLocal,
            @TaxClassValueRef,
            @OptimisticLockField,
            @WTaxCode,
            @WTaxRate,
            @WTaxAmount,
            @WTaxAmountLocal,
            @SgstAmount,
            @CgstAmount,
            @IgstAmount,
            @SgstAmountLocal,
            @CgstAmountLocal,
            @IgstAmountLocal,
            @TariffCodeId,
            @LocalOneCentsDiff,
            @ServiceAmount,
            @ServiceAmountLocal,
            @WVatCode,
            @WVatRate,
            @WVatAmount,
            @WVatAmountLocal,
            @ATCCodeId,
            @DimensionId
        )";
        var updateDetailQuery = @"
        UPDATE InvoiceDetails
        SET
            InvoiceId = @InvoiceId,
            Pos = @Pos,
            GLAccountId = @GLAccountId,
            Description = @Description,
            ReferenceNo = @ReferenceNo,
            ProjectId = @ProjectId,
            DepartmentId = @DepartmentId,
            CostCentreId = @CostCentreId,
            Amount = @Amount,
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
            TaxExclusiveAmount = @TaxExclusiveAmount,
            TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal,
            TaxClassValueRef = @TaxClassValueRef,
            OptimisticLockField = @OptimisticLockField,
            WTaxCode = @WTaxCode,
            WTaxRate = @WTaxRate,
            WTaxAmount = @WTaxAmount,
            WTaxAmountLocal = @WTaxAmountLocal,
            SgstAmount = @SgstAmount,
            CgstAmount = @CgstAmount,
            IgstAmount = @IgstAmount,
            SgstAmountLocal = @SgstAmountLocal,
            CgstAmountLocal = @CgstAmountLocal,
            IgstAmountLocal = @IgstAmountLocal,
            TariffCodeId = @TariffCodeId,
            LocalOneCentsDiff = @LocalOneCentsDiff,
            ServiceAmount = @ServiceAmount,
            ServiceAmountLocal = @ServiceAmountLocal,
            WVatCode = @WVatCode,
            WVatRate = @WVatRate,
            WVatAmount = @WVatAmount,
            WVatAmountLocal = @WVatAmountLocal,
            ATCCodeId = @ATCCodeId,
            DimensionId = @DimensionId
        WHERE Id = @Id
        ";

        try
        {
            var existheader = await checkExistServices.RecordExistsByIdAsync(checkExistModel);
            if (!existheader)
            {
                await connection.ExecuteAsync(insertHeaderQuery, invoicesHeader);
            }
            else
            {
                await connection.ExecuteAsync(updateHeaderQuery, invoicesHeader);
            }

            for (int i = 0; i < invoiceDetails.Count; i++)
            {
                sidetailsCheckModel.Id = invoiceDetails[i].Id;
                sidetailsCheckModel.TableName = "InvoiceDetails";
                var exists = await checkExistServices.RecordExistsByIdAsync(sidetailsCheckModel);
                if (!exists)
                    await connection.ExecuteAsync(insertDetailQuery, invoiceDetails[i]);
                else
                    await connection.ExecuteAsync(updateDetailQuery, invoiceDetails[i]);

            }

            // ===  COMMIT ===
            return "Success: Invoices saved successfully.";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

}
