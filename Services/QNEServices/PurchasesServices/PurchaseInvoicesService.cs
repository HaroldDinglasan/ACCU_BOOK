using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.TransferServices;
using Dapper;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Accounting_V4.Services.QNEServices.PurchasesServices
{
    public class PurchaseInvoicesService
    {
        private readonly QneConnectionString qneConnection;
        public PurchaseTransferLinkServices purchaseTransferLinkServices;
        public CheckExistServices checkExistServices;

        public PurchaseInvoicesService(
            QneConnectionString qneConnection,
            CheckExistServices checkExistServices,
            PurchaseTransferLinkServices purchaseTransferLinkServices)
        {
            this.qneConnection = qneConnection;
            this.checkExistServices = checkExistServices;
            this.purchaseTransferLinkServices = purchaseTransferLinkServices;
        }

        public async Task<IEnumerable<PurchaseInvoicesModel>> GetPurchaseInvoicesList()
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"
                        SELECT 
                        PI.Id,
                        PI.PurchaseInvoiceDate,
                        PI.PurchaseInvoiceCode,
                        CD.CompanyCode AS CompanyCode,
                        PI.CreditorName,
                        PC.Name AS Name,
                        CR.CurrencyCode AS CurrencyCode,
                        PI.WTaxTotalAmountLocal,
                        PI.TaxTotalAmountLocal,
                        PI.NetTotalAmountLocal

                    FROM PurchaseInvoices AS PI 

                    LEFT JOIN Creditors AS CD
                        ON PI.CreditorId = CD.Id

                    LEFT JOIN Purchasers AS PC
                        ON PI.PurchaserId = PC.Id

                    LEFT JOIN Currencies AS CR 
                        ON PI.CurrencyId = CR.Id

                    ORDER BY PurchaseInvoiceDate DESC   
                    ";
            var model = await connection.QueryAsync<PurchaseInvoicesModel>(query, commandType: CommandType.Text);
            return model;
        }

        // GET PURCHASE INVOICES BY ID
        public async Task<PurchaseInvoicesModel> GetPurchaseInvoicesById(string Id)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"SELECT * FROM PurchaseInvoices WHERE Id = @Id";

            var model = await connection.QueryFirstOrDefaultAsync<PurchaseInvoicesModel>(query, new { Id });

            return model;
        }

        // GET PURCHASE INVOICES DETAILS DATA BY ID
        public async Task<IEnumerable<PurchaseInvoiceDetails>> GetPurchaseInvoiceDetailById(string Id)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"SELECT * FROM PurchaseInvoiceDetails WHERE PurchaseInvoiceId = @Id ORDER BY Pos ASC";

            var model = await connection.QueryAsync<PurchaseInvoiceDetails>(query, new { Id });

            return model;
        }

        public async Task<string> DeletePurchaseInvoiceDetails(string Id)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            var updatedetailsquery = @"DELETE FROM PurchaseInvoiceDetails WHERE Id=@Id";

            var p = new DynamicParameters();
            p.Add("@Id", Id);

            try
            {
                var deleteDetailRows = await connection.ExecuteAsync(updatedetailsquery, p, transaction);
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


        // SAVE PURCHASE INVOICE WITH TRANSFER (GRN -> PI)
        protected CheckExistModel checkExistModel = new CheckExistModel();

        protected CheckExistModel checkdetailsCheckModel = new CheckExistModel();

        public async Task<string> SavePurchaseInvoiceWithTransferAsync(
            PurchaseInvoicesModel purchaseInvoices,
            List<PurchaseInvoiceDetails> purchaseInvoiceDetails,
            ForTransferHeader forTransferHeader,
            List<ForTransferDetails> forTransferDetails)
        {
            checkExistModel.Id = purchaseInvoices.Id;

            checkExistModel.TableName = "PurchaseInvoices";

            var connectionString = await qneConnection.GetConnectionStringAsync();

            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            // QUERY FOR HEADER INSERT AND UPDATE
            var insertHeaderQuery = @"
                INSERT INTO PurchaseInvoices (
                    Id, PurchaseInvoiceCode, PurchaseInvoiceDate, CreditorId, CreditorName, Attention, Phone,
                    Fax, ReferenceNo, CreditorDONo, CreditorInvNo, PurchaserId, TermId, DeliveryTerm,
                    Title, Ref1, Ref2, Remark1, Remark2, DeliverContact, DeliverPhone, DeliverFax,
                    ProjectId, DepartmentId, StockLocationId, CurrencyId, CurrencyRate, TotalAmount,
                    TotalAmountLocal, IsCancelled, IsReceived, PostingDate, CostCentreId, Address1,
                    Address2, Address3, Address4, DeliverAddress1, DeliverAddress2, DeliverAddress3,
                    DeliverAddress4, Discount, DiscountAmount, DiscountAmountLocal, NetTotalAmount,
                    NetTotalAmountLocal, DueDate, TaxTotalAmount, TaxTotalAmountLocal, SubtotalAmount,
                    SubtotalAmountLocal, ImportDeclarationNo, Notes, IsTaxInclusive, TaxDate, TaxExclusiveTotalAmount,
                    TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly, RoundingAdjustmentAccount, RoundingAdjustment,
                    RoundingAdjustmentLocal, IsRounding, OptimisticLockField, WTaxTotalAmount, WTaxTotalAmountLocal,
                    SgstTotalAmount, SgstTotalAmountLocal, CgstTotalAmount, CgstTotalAmountLocal, IgstTotalAmount,
                    IgstTotalAmountLocal, IsDeferredVAT, TransferFromInfo, IsImportedServicePosted, IsApproved,
                    ReverseRate, IsNonTaxInvoice, TotalPaid, IsSelfBearWTax, IsDetailPosting, TotalServiceAmount,
                    TotalServiceAmountLocal, TotalAmountTaxExempted, TotalAmountTaxExemptedLocal, IsEinvoiceReject,
                    DimensionId, PostingAccountGroupId, CustomsReferenceNo, FTAInformation, CustomsFormNo2, Incoterms,
                    CountryId, OtherCharges, CertifiedExporterNo, CreditorRegistrationNo, CreditorTINType, CreditorTIN,
                    EInvoiceStatus, SubmittedById, SubmissionDate, ValidatedOn, BillingFrom, BillingTo, UniqueIdentifiedNo,
                    CancelledById, CancellationDate, CancellationReason, SelfBillTypeId, TaxExemptionNo, TaxExemptionDetails,
                    ConsolidatedEInvoiceSelfBillId, CreditorCountryId, CreditorCity, CreditorCountryStateId, CreditorZipCode,
                    CreditorMSICId, CreditorSalesTaxRegNo, CreditorServiceTaxRegNo, CreditorTourismTaxRegNo
                ) VALUES (
                    @Id, @PurchaseInvoiceCode, @PurchaseInvoiceDate, @CreditorId, @CreditorName, @Attention, @Phone,
                    @Fax, @ReferenceNo, @CreditorDONo, @CreditorInvNo, @PurchaserId, @TermId, @DeliveryTerm,
                    @Title, @Ref1, @Ref2, @Remark1, @Remark2, @DeliverContact, @DeliverPhone, @DeliverFax,
                    @ProjectId, @DepartmentId, @StockLocationId, @CurrencyId, @CurrencyRate, @TotalAmount,
                    @TotalAmountLocal, @IsCancelled, @IsReceived, @PostingDate, @CostCentreId, @Address1,
                    @Address2, @Address3, @Address4, @DeliverAddress1, @DeliverAddress2, @DeliverAddress3,
                    @DeliverAddress4, @Discount, @DiscountAmount, @DiscountAmountLocal, @NetTotalAmount,
                    @NetTotalAmountLocal, @DueDate, @TaxTotalAmount, @TaxTotalAmountLocal, @SubtotalAmount,
                    @SubtotalAmountLocal, @ImportDeclarationNo, @Notes, @IsTaxInclusive, @TaxDate, @TaxExclusiveTotalAmount,
                    @TaxExclusiveTotalAmountLocal, @IsTaxInclusiveOnly, @RoundingAdjustmentAccount, @RoundingAdjustment,
                    @RoundingAdjustmentLocal, @IsRounding, @OptimisticLockField, @WTaxTotalAmount, @WTaxTotalAmountLocal,
                    @SgstTotalAmount, @SgstTotalAmountLocal, @CgstTotalAmount, @CgstTotalAmountLocal, @IgstTotalAmount,
                    @IgstTotalAmountLocal, @IsDeferredVAT, @TransferFromInfo, @IsImportedServicePosted, @IsApproved,
                    @ReverseRate, @IsNonTaxInvoice, @TotalPaid, @IsSelfBearWTax, @IsDetailPosting, @TotalServiceAmount,
                    @TotalServiceAmountLocal, @TotalAmountTaxExempted, @TotalAmountTaxExemptedLocal, @IsEinvoiceReject,
                    @DimensionId, @PostingAccountGroupId, @CustomsReferenceNo, @FTAInformation, @CustomsFormNo2, @Incoterms,
                    @CountryId, @OtherCharges, @CertifiedExporterNo, @CreditorRegistrationNo, @CreditorTINType, @CreditorTIN,
                    @EInvoiceStatus, @SubmittedById, @SubmissionDate, @ValidatedOn, @BillingFrom, @BillingTo, @UniqueIdentifiedNo,
                    @CancelledById, @CancellationDate, @CancellationReason, @SelfBillTypeId, @TaxExemptionNo, @TaxExemptionDetails,
                    @ConsolidatedEInvoiceSelfBillId, @CreditorCountryId, @CreditorCity, @CreditorCountryStateId, @CreditorZipCode,
                    @CreditorMSICId, @CreditorSalesTaxRegNo, @CreditorServiceTaxRegNo, @CreditorTourismTaxRegNo)";
            var updateHeaderQuery = @"
                UPDATE PurchaseInvoices SET
                    PurchaseInvoiceCode = @PurchaseInvoiceCode, PurchaseInvoiceDate = @PurchaseInvoiceDate, CreditorId = @CreditorId, CreditorName = @CreditorName, Attention = @Attention, Phone = @Phone,
                    Fax = @Fax, ReferenceNo = @ReferenceNo, CreditorDONo = @CreditorDONo, CreditorInvNo = @CreditorInvNo, PurchaserId = @PurchaserId, TermId = @TermId, DeliveryTerm = @DeliveryTerm,
                    Title = @Title, Ref1 = @Ref1, Ref2 = @Ref2, Remark1 = @Remark1, Remark2 = @Remark2, DeliverContact = @DeliverContact, DeliverPhone = @DeliverPhone, DeliverFax = @DeliverFax,
                    ProjectId = @ProjectId, DepartmentId = @DepartmentId, StockLocationId = @StockLocationId, CurrencyId = @CurrencyId, CurrencyRate = @CurrencyRate, TotalAmount = @TotalAmount,
                    TotalAmountLocal = @TotalAmountLocal, IsCancelled = @IsCancelled, IsReceived = @IsReceived, PostingDate = @PostingDate, CostCentreId = @CostCentreId, Address1 = @Address1,
                    Address2 = @Address2, Address3 = @Address3, Address4 = @Address4, DeliverAddress1 = @DeliverAddress1, DeliverAddress2 = @DeliverAddress2, DeliverAddress3 = @DeliverAddress3,
                    DeliverAddress4 = @DeliverAddress4, Discount = @Discount, DiscountAmount = @DiscountAmount, DiscountAmountLocal = @DiscountAmountLocal, NetTotalAmount = @NetTotalAmount,
                    NetTotalAmountLocal = @NetTotalAmountLocal, DueDate = @DueDate, TaxTotalAmount = @TaxTotalAmount, TaxTotalAmountLocal = @TaxTotalAmountLocal, SubtotalAmount = @SubtotalAmount,
                    SubtotalAmountLocal = @SubtotalAmountLocal, ImportDeclarationNo = @ImportDeclarationNo, Notes = @Notes, IsTaxInclusive = @IsTaxInclusive, TaxDate = @TaxDate, TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
                    TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly = @IsTaxInclusiveOnly, RoundingAdjustmentAccount = @RoundingAdjustmentAccount, RoundingAdjustment = @RoundingAdjustment,
                    RoundingAdjustmentLocal = @RoundingAdjustmentLocal, IsRounding = @IsRounding, OptimisticLockField = @OptimisticLockField, WTaxTotalAmount = @WTaxTotalAmount, WTaxTotalAmountLocal = @WTaxTotalAmountLocal,
                    SgstTotalAmount = @SgstTotalAmount, SgstTotalAmountLocal = @SgstTotalAmountLocal, CgstTotalAmount = @CgstTotalAmount, CgstTotalAmountLocal = @CgstTotalAmountLocal, IgstTotalAmount = @IgstTotalAmount,
                    IgstTotalAmountLocal = @IgstTotalAmountLocal, IsDeferredVAT = @IsDeferredVAT, TransferFromInfo = @TransferFromInfo, IsImportedServicePosted = @IsImportedServicePosted, IsApproved = @IsApproved,
                    ReverseRate = @ReverseRate, IsNonTaxInvoice = @IsNonTaxInvoice, TotalPaid = @TotalPaid, IsSelfBearWTax = @IsSelfBearWTax, IsDetailPosting = @IsDetailPosting, TotalServiceAmount = @TotalServiceAmount,
                    TotalServiceAmountLocal = @TotalServiceAmountLocal, TotalAmountTaxExempted = @TotalAmountTaxExempted, TotalAmountTaxExemptedLocal = @TotalAmountTaxExemptedLocal, IsEinvoiceReject = @IsEinvoiceReject,
                    DimensionId = @DimensionId, PostingAccountGroupId = @PostingAccountGroupId, CustomsReferenceNo = @CustomsReferenceNo, FTAInformation = @FTAInformation, CustomsFormNo2 = @CustomsFormNo2, Incoterms = @Incoterms,
                    CountryId = @CountryId, OtherCharges = @OtherCharges, CertifiedExporterNo = @CertifiedExporterNo, CreditorRegistrationNo = @CreditorRegistrationNo, CreditorTINType = @CreditorTINType, CreditorTIN = @CreditorTIN,
                    EInvoiceStatus = @EInvoiceStatus, SubmittedById = @SubmittedById, SubmissionDate = @SubmissionDate, ValidatedOn = @ValidatedOn, BillingFrom = @BillingFrom, BillingTo = @BillingTo, UniqueIdentifiedNo = @UniqueIdentifiedNo,
                    CancelledById = @CancelledById, CancellationDate = @CancellationDate, CancellationReason = @CancellationReason, SelfBillTypeId = @SelfBillTypeId, TaxExemptionNo = @TaxExemptionNo, TaxExemptionDetails = @TaxExemptionDetails,
                    ConsolidatedEInvoiceSelfBillId = @ConsolidatedEInvoiceSelfBillId, CreditorCountryId = @CreditorCountryId, CreditorCity = @CreditorCity, CreditorCountryStateId = @CreditorCountryStateId, CreditorZipCode = @CreditorZipCode,
                    CreditorMSICId = @CreditorMSICId, CreditorSalesTaxRegNo = @CreditorSalesTaxRegNo, CreditorServiceTaxRegNo = @CreditorServiceTaxRegNo, CreditorTourismTaxRegNo = @CreditorTourismTaxRegNo
                WHERE Id = @Id";
            var insertDetailQuery = @"
                INSERT INTO PurchaseInvoiceDetails (
                    Id, PurchaseInvoiceId, Pos, Numbering, StockId, Description, ProjectId, DepartmentId, StockLocationId, Qty, UOMId, UnitPrice,
                    Discount, Amount, SupplierItemCode, ReferenceNo, FromType, FromDocId, FromDtlId, IsPartialTransfer, CostCentreId, GLAccountId,
                    AmountLocal, DiscountAmount, DiscountAmountLocal, NetAmount, NetAmountLocal, Note, ItemTypeCode, IsBundled,
                    IsSubItem, BundledTransactionId, TaxCodeId, IsTaxInclusive, TaxRate, TaxClass, TaxClassValue, TaxAmount, TaxAmountLocal,
                    SubAmount, SubAmountLocal, TaxExclusiveAmount, TaxExclusiveAmountLocal, TaxClassValueRef, TransferLink , StockType,
                    OptimisticLockField, WTaxCode, WTaxRate, WTaxAmount, WTaxAmountLocal, CostA, CostB, CostC, CostD, SgstAmount, CgstAmount,
                    IgstAmount, SgstAmountLocal, CgstAmountLocal, IgstAmountLocal, CreditorItemId, StockBatchNumberId, Ref, Ref2, Ref3, Ref4,
                    Ref5, DateRef1, DateRef2, NumRef1, NumRef2, SerialNumber, SerialNumberRef1, SerialNumberRef2, TariffCodeId, LocalOneCentsDiff,
                    ServiceAmount, ServiceAmountLocal, SubcodeId, CommissionGroupId, PackingUOMId, Packing, PackingQty, PackingUOM1, PackingUOM2,
                    HighestUOM, RefTransferFrom, DimensionId, TaxExemptNo, TaxExemptDetails, MarkupDiscountAmount, MarkupDiscountAmountLocal,
                    AmountTaxExempted, AmountTaxExemptedLocal, MarkupDiscountPrice, Classification
                ) VALUES (
                    @Id, @PurchaseInvoiceId, @Pos, @Numbering, @StockId, @Description, @ProjectId, @DepartmentId, @StockLocationId, @Qty, @UOMId, @UnitPrice,
                    @Discount, @Amount, @SupplierItemCode, @ReferenceNo, @FromType, @FromDocId, @FromDtlId, @IsPartialTransfer, @CostCentreId, @GLAccountId,
                    @AmountLocal, @DiscountAmount, @DiscountAmountLocal, @NetAmount, @NetAmountLocal, @Note, @ItemTypeCode, @IsBundled,
                    @IsSubItem, @BundledTransactionId, @TaxCodeId, @IsTaxInclusive, @TaxRate, @TaxClass, @TaxClassValue, @TaxAmount, @TaxAmountLocal,
                    @SubAmount, @SubAmountLocal, @TaxExclusiveAmount, @TaxExclusiveAmountLocal, @TaxClassValueRef, @TransferLink , @StockType,
                    @OptimisticLockField, @WTaxCode, @WTaxRate, @WTaxAmount, @WTaxAmountLocal, @CostA, @CostB, @CostC, @CostD, @SgstAmount, @CgstAmount,
                    @IgstAmount, @SgstAmountLocal, @CgstAmountLocal, @IgstAmountLocal, @CreditorItemId, @StockBatchNumberId, @Ref, @Ref2, @Ref3, @Ref4,
                    @Ref5, @DateRef1, @DateRef2, @NumRef1, @NumRef2, @SerialNumber, @SerialNumberRef1, @SerialNumberRef2, @TariffCodeId, @LocalOneCentsDiff,
                    @ServiceAmount, @ServiceAmountLocal, @SubcodeId, @CommissionGroupId, @PackingUOMId, @Packing, @PackingQty, @PackingUOM1, @PackingUOM2,
                    @HighestUOM, @RefTransferFrom, @DimensionId, @TaxExemptNo, @TaxExemptDetails, @MarkupDiscountAmount, @MarkupDiscountAmountLocal,
                    @AmountTaxExempted, @AmountTaxExemptedLocal, @MarkupDiscountPrice, @Classification
                )";
            var updateDetailQuery = @"
                  UPDATE PurchaseInvoiceDetails SET
                    PurchaseInvoiceId = @PurchaseInvoiceId, Pos = @Pos, Numbering = @Numbering, StockId = @StockId, Description = @Description, ProjectId = @ProjectId, DepartmentId = @DepartmentId, StockLocationId = @StockLocationId, Qty = @Qty, UOMId = @UOMId, UnitPrice = @UnitPrice,
                    Discount = @Discount, Amount = @Amount, SupplierItemCode = @SupplierItemCode, ReferenceNo = @ReferenceNo, FromType = @FromType, FromDocId = @FromDocId, FromDtlId = @FromDtlId, IsPartialTransfer = @IsPartialTransfer, CostCentreId = @CostCentreId, GLAccountId = @GLAccountId,
                    AmountLocal = @AmountLocal, DiscountAmount = @DiscountAmount, DiscountAmountLocal = @DiscountAmountLocal, NetAmount = @NetAmount, NetAmountLocal = @NetAmountLocal, Note = @Note, ItemTypeCode = @ItemTypeCode, IsBundled = @IsBundled,
                    IsSubItem = @IsSubItem, BundledTransactionId = @BundledTransactionId, TaxCodeId = @TaxCodeId, IsTaxInclusive = @IsTaxInclusive, TaxRate = @TaxRate, TaxClass = @TaxClass, TaxClassValue = @TaxClassValue, TaxAmount = @TaxAmount, TaxAmountLocal = @TaxAmountLocal,
                    SubAmount = @SubAmount, SubAmountLocal = @SubAmountLocal, TaxExclusiveAmount = @TaxExclusiveAmount, TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal, TaxClassValueRef = @TaxClassValueRef, TransferLink = @TransferLink , StockType = @StockType,
                    OptimisticLockField = @OptimisticLockField, WTaxCode = @WTaxCode, WTaxRate = @WTaxRate, WTaxAmount = @WTaxAmount, WTaxAmountLocal = @WTaxAmountLocal, CostA = @CostA, CostB = @CostB, CostC = @CostC, CostD = @CostD, SgstAmount = @SgstAmount, CgstAmount = @CgstAmount,
                    IgstAmount = @IgstAmount, SgstAmountLocal = @SgstAmountLocal, CgstAmountLocal = @CgstAmountLocal, IgstAmountLocal = @IgstAmountLocal, CreditorItemId = @CreditorItemId, StockBatchNumberId = @StockBatchNumberId, Ref = @Ref, Ref2 = @Ref2, Ref3 = @Ref3, Ref4 = @Ref4,
                    Ref5 = @Ref5, DateRef1 = @DateRef1, DateRef2 = @DateRef2, NumRef1 = @NumRef1, NumRef2 = @NumRef2, SerialNumber = @SerialNumber, SerialNumberRef1 = @SerialNumberRef1, SerialNumberRef2 = @SerialNumberRef2, TariffCodeId = @TariffCodeId, LocalOneCentsDiff = @LocalOneCentsDiff,
                    ServiceAmount = @ServiceAmount, ServiceAmountLocal = @ServiceAmountLocal, SubcodeId = @SubcodeId, CommissionGroupId = @CommissionGroupId, PackingUOMId = @PackingUOMId, Packing = @Packing, PackingQty = @PackingQty, PackingUOM1 = @PackingUOM1, PackingUOM2 = @PackingUOM2,
                    HighestUOM = @HighestUOM, RefTransferFrom = @RefTransferFrom, DimensionId = @DimensionId, TaxExemptNo = @TaxExemptNo, TaxExemptDetails = @TaxExemptDetails, MarkupDiscountAmount = @MarkupDiscountAmount, MarkupDiscountAmountLocal = @MarkupDiscountAmountLocal,
                    AmountTaxExempted = @AmountTaxExempted, AmountTaxExemptedLocal = @AmountTaxExemptedLocal, MarkupDiscountPrice = @MarkupDiscountPrice, Classification = @Classification
                  WHERE Id = @Id";

            forTransferDetails ??= new List<ForTransferDetails>();
            bool isTransfer = forTransferDetails.Any();

            try
            {
                var existheader = await checkExistServices.RecordExistsByIdAsync(checkExistModel);
                if (!existheader)
                {
                    await connection.ExecuteAsync(insertHeaderQuery, purchaseInvoices, transaction);

                    if (isTransfer)
                    {
                        var updateGRNHeaderQuery = @"UPDATE GoodsReceivedNotes
                        SET TransferToInfo =
                            CASE
                                WHEN TransferToInfo IS NULL OR TransferToInfo = '' THEN @TransferToInfo
                                ELSE TransferToInfo + ', ' + @TransferToInfo
                            END
                        WHERE Id = @GoodsReceivedNoteHeaderId";

                        await connection.ExecuteAsync(updateGRNHeaderQuery, forTransferHeader, transaction);
                        await purchaseTransferLinkServices.InsertPurchaseTransferLink(forTransferDetails);
                    }
                }
                else
                {
                    await connection.ExecuteAsync(updateHeaderQuery, purchaseInvoices, transaction);
                }

                for (int i = 0; i < purchaseInvoiceDetails.Count; i++)
                {
                    var detail = purchaseInvoiceDetails[i];

                    // skip the blank placeholder rows
                    if (detail.StockId is null)
                        continue;

                    checkdetailsCheckModel.Id = detail.Id;
                    checkdetailsCheckModel.TableName = "PurchaseInvoiceDetails";

                    var exists = await checkExistServices.RecordExistsByIdAsync(checkdetailsCheckModel);
                    if (!exists)
                    {
                        if (isTransfer && i < forTransferDetails.Count)
                            detail.TransferLink = forTransferDetails[i].Id;

                        await connection.ExecuteAsync(insertDetailQuery, detail, transaction);
                    }
                    else
                    {
                        await connection.ExecuteAsync(updateDetailQuery, detail, transaction);
                    }
                }

                // update GRN details ONCE, only when this invoice came from a GRN transfer
                if (isTransfer)
                {
                    var updateGRNDetailsQuery = @"
                    UPDATE GoodsReceivedNoteDetails
                    SET Transferred = @Transferred,
                        TransferLink = @TransferLink,
                        OptimisticLockField = OptimisticLockField + 1
                    WHERE Id = @GoodsReceivedNoteDetails";

                    foreach (var grndetail in forTransferDetails)
                    {
                        await connection.ExecuteAsync(updateGRNDetailsQuery, new
                        {
                            GoodsReceivedNoteDetails = grndetail.GoodsReceivedNoteDetailId,
                            Transferred = grndetail.Transferred,
                            TransferLink = grndetail.TransferLink
                        }, transaction);
                    }
                }

                transaction.Commit();
                return "Success: Purchase Invoices saved successfully";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return $"Error: {ex.Message}";
            }
        }








    }
}
