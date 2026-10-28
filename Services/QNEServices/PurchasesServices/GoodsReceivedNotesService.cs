using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.TransferServices;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Accounting_V4.Services.QNEServices.PurchasesServices
{
    public class GoodsReceivedNotesService
    {
        private readonly QneConnectionString qneConnection;
        public PurchaseTransferLinkServices purchaseTransferLinkServices;
        public CheckExistServices checkExistServices;

        public GoodsReceivedNotesService(
            QneConnectionString qneConnection,
            CheckExistServices checkExistServices,
            PurchaseTransferLinkServices purchaseTransferLinkServices)
        {
            this.qneConnection = qneConnection;
            this.checkExistServices = checkExistServices;
            this.purchaseTransferLinkServices = purchaseTransferLinkServices;
        }

        public async Task<IEnumerable<GoodsReceivedNotesModel>> GetGoodsReceivedNotesList()
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"
                    SELECT 
                    GRN.Id,
	                GRN.GoodsReceivedNoteDate,
	                GRN.GoodsReceivedNoteCode,
                    CD.CompanyCode AS CompanyCode,
	                GRN.CreditorName,
	                PC.Name AS Name,
                    CR.CurrencyCode AS CurrencyCode,
                    GRN.WTaxTotalAmountLocal,
                    GRN.TaxTotalAmountLocal,
                    GRN.NetTotalAmountLocal,
                    GRN.IsClosed,
                    GRN.IsCancelled,
                    GRN.TransferFromInfo,
                    GRN.TransferToInfo
                FROM GoodsReceivedNotes AS GRN 

                LEFT JOIN Creditors AS CD
                    ON GRN.CreditorId = CD.Id

                LEFT JOIN Purchasers AS PC
                    ON GRN.PurchaserId = PC.Id

                LEFT JOIN Currencies AS CR 
                    ON GRN.CurrencyId = CR.Id

                ORDER BY GoodsReceivedNoteDate DESC
                ";

            var model = await connection.QueryAsync<GoodsReceivedNotesModel>(query, commandType: CommandType.Text);
            return model;
        }


        // GET GOODS RECEIVED NOTES BY ID
        public async Task<GoodsReceivedNotesModel> GetGoodsReceivedNotesById(string Id)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"SELECT * FROM GoodsReceivedNotes WHERE Id = @Id";

            var model = await connection.QueryFirstOrDefaultAsync<GoodsReceivedNotesModel>(query, new { Id });

            return model;
        }

        // GET GET GOODS RECEIVED NOTES DETAILS DATA BY ID
        public async Task<IEnumerable<GoodsReceivedNotesDetails>> GetGoodsReceivedNotesDetailById(string Id)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            var query = @"SELECT * FROM GoodsReceivedNoteDetails WHERE GoodsReceivedNoteId = @Id ORDER BY Pos ASC";

            var model = await connection.QueryAsync<GoodsReceivedNotesDetails>(query, new { Id });

            return model;
        }

        public async Task<string> DeleteGoodsReceivedNotesDetails(string Id)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();

            var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            var updatedetailsquery = @"DELETE FROM GoodsReceivedNoteDetails WHERE Id=@Id";

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

        // SAVE GOODS RECEIVED NOTES WITH TRANSFER (PO -> GRN IF ANY)
        protected CheckExistModel checkExistModel = new CheckExistModel();

        protected CheckExistModel checkdetailsCheckModel = new CheckExistModel();

        public async Task<string> SaveGoodsReceivedNoteWithTransferAsync(
            GoodsReceivedNotesModel goodsReceivedNotes,
            List<GoodsReceivedNotesDetails> goodsReceivedNotesDetails,
            ForTransferHeader forTransferHeader,
            List<ForTransferDetails> forTransferDetails)
        {
            checkExistModel.Id = goodsReceivedNotes.Id;

            checkExistModel.TableName = "GoodsReceivedNotes";

            var connectionString = await qneConnection.GetConnectionStringAsync();

            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            // QUERY FOR HEADER INSERT AND UPDATE
            var insertHeaderQuery = @"
                INSERT INTO GoodsReceivedNotes (
                    Id, GoodsReceivedNoteCode, GoodsReceivedNoteDate, CreditorId, CreditorName, Attention,
                    Phone, Fax, ReferenceNo, CreditorDONo, PurchaserId, TermId, DeliveryTerm, Title,
                    Ref1, Ref2, Remark1, Remark2, DeliverContact, DeliverPhone, DeliverFax, ProjectId,
                    DepartmentId, StockLocationId, CurrencyId, CurrencyRate, TotalAmount, TotalAmountLocal,
                    IsCancelled, IsClosed, CostCentreId, Address1, Address2, Address3, Address4, DeliverAddress1,
                    DeliverAddress2, DeliverAddress3, DeliverAddress4, Discount, DiscountAmount, DiscountAmountLocal,
                    NetTotalAmount, NetTotalAmountLocal, DueDate, TaxTotalAmount, TaxTotalAmountLocal, SubtotalAmount,
                    SubtotalAmountLocal, ImportDeclarationNo, Notes, IsTaxInclusive, TaxDate, TaxExclusiveTotalAmount,
                    TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly, RoundingAdjustmentAccount, RoundingAdjustment,
                    RoundingAdjustmentLocal, IsRounding, OptimisticLockField, WTaxTotalAmount, WTaxTotalAmountLocal,
                    SgstTotalAmount, SgstTotalAmountLocal, CgstTotalAmount, CgstTotalAmountLocal, IgstTotalAmount,
                    IgstTotalAmountLocal, TransferFromInfo, TransferToInfo, IsApproved, ReverseRate,IsNonTaxInvoice,
                    IsPostAccruedAP, IsPast, IsSelfBearWTax, TotalServiceAmount, TotalServiceAmountLocal, TotalAmountTaxExempted,
                    TotalAmountTaxExemptedLocal, IsEinvoiceReject, DimensionId
                ) VALUES (
                    @Id, @GoodsReceivedNoteCode, @GoodsReceivedNoteDate, @CreditorId, @CreditorName, @Attention,
                    @Phone, @Fax, @ReferenceNo, @CreditorDONo, @PurchaserId, @TermId, @DeliveryTerm, @Title,
                    @Ref1, @Ref2, @Remark1, @Remark2, @DeliverContact, @DeliverPhone, @DeliverFax, @ProjectId,
                    @DepartmentId, @StockLocationId, @CurrencyId, @CurrencyRate, @TotalAmount, @TotalAmountLocal,
                    @IsCancelled, @IsClosed, @CostCentreId, @Address1, @Address2, @Address3, @Address4, @DeliverAddress1,
                    @DeliverAddress2, @DeliverAddress3, @DeliverAddress4, @Discount, @DiscountAmount, @DiscountAmountLocal,
                    @NetTotalAmount, @NetTotalAmountLocal, @DueDate, @TaxTotalAmount, @TaxTotalAmountLocal, @SubtotalAmount,
                    @SubtotalAmountLocal, @ImportDeclarationNo, @Notes, @IsTaxInclusive, @TaxDate, @TaxExclusiveTotalAmount,
                    @TaxExclusiveTotalAmountLocal, @IsTaxInclusiveOnly, @RoundingAdjustmentAccount, @RoundingAdjustment,
                    @RoundingAdjustmentLocal, @IsRounding, @OptimisticLockField, @WTaxTotalAmount, @WTaxTotalAmountLocal,
                    @SgstTotalAmount, @SgstTotalAmountLocal, @CgstTotalAmount, @CgstTotalAmountLocal, @IgstTotalAmount,
                    @IgstTotalAmountLocal, @TransferFromInfo, @TransferToInfo, @IsApproved, @ReverseRate, @IsNonTaxInvoice,
                    @IsPostAccruedAP, @IsPast, @IsSelfBearWTax, @TotalServiceAmount, @TotalServiceAmountLocal, @TotalAmountTaxExempted,
                    @TotalAmountTaxExemptedLocal, @IsEinvoiceReject, @DimensionId)";
            var updateHeaderQuery = @"
                UPDATE GoodsReceivedNotes SET
                    GoodsReceivedNoteCode = @GoodsReceivedNoteCode, GoodsReceivedNoteDate = @GoodsReceivedNoteDate, CreditorId = @CreditorId, CreditorName = @CreditorName, Attention = @Attention,
                    Phone = @Phone, Fax = @Fax, ReferenceNo = @ReferenceNo, CreditorDONo = @CreditorDONo, PurchaserId = @PurchaserId, TermId = @TermId, DeliveryTerm = @DeliveryTerm, Title = @Title,
                    Ref1 = @Ref1, Ref2 = @Ref2, Remark1 = @Remark1, Remark2 = @Remark2, DeliverContact = @DeliverContact, DeliverPhone = @DeliverPhone, DeliverFax = @DeliverFax, ProjectId = @ProjectId,
                    DepartmentId = @DepartmentId, StockLocationId = @StockLocationId, CurrencyId = @CurrencyId, CurrencyRate = @CurrencyRate, TotalAmount = @TotalAmount, TotalAmountLocal = @TotalAmountLocal,
                    IsCancelled = @IsCancelled, IsClosed = @IsClosed, CostCentreId = @CostCentreId, Address1 = @Address1, Address2 = @Address2, Address3 = @Address3, Address4 = @Address4, DeliverAddress1 = @DeliverAddress1,
                    DeliverAddress2 = @DeliverAddress2, DeliverAddress3 = @DeliverAddress3, DeliverAddress4 = @DeliverAddress4, Discount = @Discount, DiscountAmount = @DiscountAmount, DiscountAmountLocal = @DiscountAmountLocal,
                    NetTotalAmount = @NetTotalAmount, NetTotalAmountLocal = @NetTotalAmountLocal, DueDate = @DueDate, TaxTotalAmount = @TaxTotalAmount, TaxTotalAmountLocal = @TaxTotalAmountLocal, SubtotalAmount = @SubtotalAmount,
                    SubtotalAmountLocal = @SubtotalAmountLocal, ImportDeclarationNo = @ImportDeclarationNo, Notes = @Notes, IsTaxInclusive = @IsTaxInclusive, TaxDate = @TaxDate, TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
                    TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly = @IsTaxInclusiveOnly, RoundingAdjustmentAccount = @RoundingAdjustmentAccount, RoundingAdjustment = @RoundingAdjustment,
                    RoundingAdjustmentLocal = @RoundingAdjustmentLocal, IsRounding = @IsRounding, OptimisticLockField = @OptimisticLockField, WTaxTotalAmount = @WTaxTotalAmount, WTaxTotalAmountLocal = @WTaxTotalAmountLocal,
                    SgstTotalAmount = @SgstTotalAmount, SgstTotalAmountLocal = @SgstTotalAmountLocal, CgstTotalAmount = @CgstTotalAmount, CgstTotalAmountLocal = @CgstTotalAmountLocal, IgstTotalAmount = @IgstTotalAmount,
                    IgstTotalAmountLocal = @IgstTotalAmountLocal, TransferFromInfo = @TransferFromInfo, TransferToInfo = @TransferToInfo, IsApproved = @IsApproved, ReverseRate = @ReverseRate, IsNonTaxInvoice = @IsNonTaxInvoice,
                    IsPostAccruedAP = @IsPostAccruedAP, IsPast = @IsPast, IsSelfBearWTax = @IsSelfBearWTax, TotalServiceAmount = @TotalServiceAmount, TotalServiceAmountLocal = @TotalServiceAmountLocal, TotalAmountTaxExempted = @TotalAmountTaxExempted,
                    TotalAmountTaxExemptedLocal = @TotalAmountTaxExemptedLocal, IsEinvoiceReject = @IsEinvoiceReject, DimensionId = @DimensionId
                    WHERE Id = @Id";

            var insertDetailQuery = @"
                    INSERT INTO GoodsReceivedNoteDetails (
                        Id, GoodsReceivedNoteId, Pos, Numbering, StockId, Description, ProjectId, DepartmentId, StockLocationId, Qty,
                        UOMId, UnitPrice, Discount, Amount, RequireDate, SupplierItemCode, ReferenceNo, CancelledQty, FromType, FromDocId,
                        FromDtlId, IsPartialTransfer, CostCentreId, AmountLocal, DiscountAmount, DiscountAmountLocal, NetAmount, NetAmountLocal,
                        Note, ItemTypeCode, IsBundled, IsSubItem, BundledTransactionId, TaxCodeId, IsTaxInclusive, TaxRate, TaxClass, TaxClassValue,
                        TaxAmount, TaxAmountLocal, SubAmount, SubAmountLocal, TaxExclusiveAmount, TaxExclusiveAmountLocal, TaxClassValueRef,
                        Transferred, TransferLink, StockType, OptimisticLockField, WTaxCode, WTaxRate, WTaxAmount, WTaxAmountLocal, CostA, CostB,
                        CostC,CostD,SgstAmount,CgstAmount,IgstAmount,SgstAmountLocal,CgstAmountLocal,IgstAmountLocal,CreditorItemId,StockBatchNumberId,
                        Ref, Ref2, Ref3, Ref4, Ref5, DateRef1, DateRef2, NumRef1, NumRef2, SerialNumber, SerialNumberRef1, SerialNumberRef2, LocalOneCentsDiff,
                        ServiceAmount, ServiceAmountLocal, SubcodeId, CommissionGroupId, PackingUOMId, Packing, PackingQty, CancelledSerialNumber, PackingUOM1,
                        PackingUOM2, HighestUOM, RefTransferFrom, DimensionId, TaxExemptNo, TaxExemptDetails, MarkupDiscountAmount, MarkupDiscountAmountLocal,
                        AmountTaxExempted, AmountTaxExemptedLocal, MarkupDiscountPrice
                    ) VALUES (
                        @Id, @GoodsReceivedNoteId, @Pos, @Numbering, @StockId, @Description, @ProjectId, @DepartmentId, @StockLocationId, @Qty,
                        @UOMId, @UnitPrice, @Discount, @Amount, @RequireDate, @SupplierItemCode, @ReferenceNo, @CancelledQty, @FromType, @FromDocId,
                        @FromDtlId, @IsPartialTransfer, @CostCentreId, @AmountLocal, @DiscountAmount, @DiscountAmountLocal, @NetAmount, @NetAmountLocal,
                        @Note, @ItemTypeCode, @IsBundled, @IsSubItem, @BundledTransactionId, @TaxCodeId, @IsTaxInclusive, @TaxRate, @TaxClass, @TaxClassValue,
                        @TaxAmount, @TaxAmountLocal, @SubAmount, @SubAmountLocal, @TaxExclusiveAmount, @TaxExclusiveAmountLocal, @TaxClassValueRef,
                        @Transferred, @TransferLink, @StockType, @OptimisticLockField, @WTaxCode, @WTaxRate, @WTaxAmount, @WTaxAmountLocal, @CostA, @CostB,
                        @CostC, @CostD, @SgstAmount, @CgstAmount, @IgstAmount, @SgstAmountLocal, @CgstAmountLocal, @IgstAmountLocal, @CreditorItemId, @StockBatchNumberId,
                        @Ref, @Ref2, @Ref3, @Ref4, @Ref5, @DateRef1, @DateRef2, @NumRef1, @NumRef2, @SerialNumber, @SerialNumberRef1, @SerialNumberRef2, @LocalOneCentsDiff,
                        @ServiceAmount, @ServiceAmountLocal, @SubcodeId, @CommissionGroupId, @PackingUOMId, @Packing, @PackingQty, @CancelledSerialNumber, @PackingUOM1,
                        @PackingUOM2, @HighestUOM, @RefTransferFrom, @DimensionId, @TaxExemptNo, @TaxExemptDetails, @MarkupDiscountAmount, @MarkupDiscountAmountLocal,
                        @AmountTaxExempted, @AmountTaxExemptedLocal, @MarkupDiscountPrice
                    )";
            var updateDetailQuery = @"
                    UPDATE GoodsReceivedNoteDetails SET
                        GoodsReceivedNoteId = @GoodsReceivedNoteId, Pos = @Pos, Numbering = @Numbering, StockId = @StockId, Description = @Description, ProjectId = @ProjectId, DepartmentId = @DepartmentId, StockLocationId = @StockLocationId, Qty = @Qty,
                        UOMId = @UOMId, UnitPrice = @UnitPrice, Discount = @Discount, Amount = @Amount, RequireDate = @RequireDate, SupplierItemCode = @SupplierItemCode, ReferenceNo = @ReferenceNo, CancelledQty = @CancelledQty, FromType = @FromType, FromDocId = @FromDocId,
                        FromDtlId = @FromDtlId, IsPartialTransfer = @IsPartialTransfer, CostCentreId = @CostCentreId, AmountLocal = @AmountLocal, DiscountAmount = @DiscountAmount, DiscountAmountLocal = @DiscountAmountLocal, NetAmount = @NetAmount, NetAmountLocal = @NetAmountLocal,
                        Note = @Note, ItemTypeCode = @ItemTypeCode, IsBundled = @IsBundled, IsSubItem = @IsSubItem, BundledTransactionId = @BundledTransactionId, TaxCodeId = @TaxCodeId, IsTaxInclusive = @IsTaxInclusive, TaxRate = @TaxRate, TaxClass = @TaxClass, TaxClassValue = @TaxClassValue,
                        TaxAmount = @TaxAmount, TaxAmountLocal = @TaxAmountLocal, SubAmount = @SubAmount, SubAmountLocal = @SubAmountLocal, TaxExclusiveAmount = @TaxExclusiveAmount, TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal, TaxClassValueRef = @TaxClassValueRef,
                        Transferred = @Transferred, TransferLink = @TransferLink, StockType = @StockType, OptimisticLockField = @OptimisticLockField, WTaxCode = @WTaxCode, WTaxRate = @WTaxRate, WTaxAmount = @WTaxAmount, WTaxAmountLocal = @WTaxAmountLocal, CostA = @CostA, CostB = @CostB,
                        CostC = @CostC, CostD = @CostD, SgstAmount = @SgstAmount, CgstAmount = @CgstAmount, IgstAmount = @IgstAmount, SgstAmountLocal = @SgstAmountLocal, CgstAmountLocal = @CgstAmountLocal, IgstAmountLocal = @IgstAmountLocal, CreditorItemId = @CreditorItemId, StockBatchNumberId = @StockBatchNumberId,
                        Ref = @Ref, Ref2 = @Ref2, Ref3 = @Ref3, Ref4 = @Ref4, Ref5 = @Ref5, DateRef1 = @DateRef1, DateRef2 = @DateRef2, NumRef1 = @NumRef1, NumRef2 = @NumRef2, SerialNumber = @SerialNumber, SerialNumberRef1 = @SerialNumberRef1, SerialNumberRef2 = @SerialNumberRef2, LocalOneCentsDiff = @LocalOneCentsDiff,
                        ServiceAmount = @ServiceAmount, ServiceAmountLocal = @ServiceAmountLocal, SubcodeId = @SubcodeId, CommissionGroupId = @CommissionGroupId, PackingUOMId = @PackingUOMId, Packing = @Packing, PackingQty = @PackingQty, CancelledSerialNumber = @CancelledSerialNumber, PackingUOM1 = @PackingUOM1,
                        PackingUOM2 = @PackingUOM2, HighestUOM = @HighestUOM, RefTransferFrom = @RefTransferFrom, DimensionId = @DimensionId, TaxExemptNo = @TaxExemptNo, TaxExemptDetails = @TaxExemptDetails, MarkupDiscountAmount = @MarkupDiscountAmount, MarkupDiscountAmountLocal = @MarkupDiscountAmountLocal,
                        AmountTaxExempted = @AmountTaxExempted, AmountTaxExemptedLocal = @AmountTaxExemptedLocal, MarkupDiscountPrice = @MarkupDiscountPrice
                    WHERE Id = @Id";

            forTransferDetails ??= new List<ForTransferDetails>();
            bool isTransfer = forTransferDetails.Any();

            try
            {
                var existheader = await checkExistServices.RecordExistsByIdAsync(checkExistModel);
                if (!existheader)
                {
                    await connection.ExecuteAsync(insertHeaderQuery, goodsReceivedNotes, transaction);

                    if (isTransfer)
                    {
                        var updatePOHeaderQuery = @"UPDATE PurchaseOrders
                        SET TransferToInfo =
                            CASE
                                WHEN TransferToInfo IS NULL OR TransferToInfo = '' THEN @TransferToInfo
                                ELSE TransferToInfo + ', ' + @TransferToInfo
                            END
                        WHERE Id = @PurchaseOrderHeaderId";

                        await connection.ExecuteAsync(updatePOHeaderQuery, forTransferHeader, transaction);
                        await purchaseTransferLinkServices.InsertPurchaseTransferLink(forTransferDetails);
                    }
                }
                else
                {
                    await connection.ExecuteAsync(updateHeaderQuery, goodsReceivedNotes, transaction);
                }

                for (int i = 0; i < goodsReceivedNotesDetails.Count; i++)
                {
                    var detail = goodsReceivedNotesDetails[i];

                    // skip the blank placeholder rows (the grid starts with 5 empty rows)
                    if (detail.StockId is null)
                        continue;

                    checkdetailsCheckModel.Id = detail.Id;
                    checkdetailsCheckModel.TableName = "GoodsReceivedNoteDetails";

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

                // update PO details ONCE, only when this GRN came from a PO transfer
                if (isTransfer)
                {
                    var updatePODetailsQuery = @"
                    UPDATE PurchaseOrderDetails
                    SET Transferred = @Transferred,
                        TransferLink = @TransferLink,
                        OptimisticLockField = OptimisticLockField + 1
                    WHERE Id = @PurchaseOrderDetails";

                    foreach (var podetail in forTransferDetails)
                    {
                        await connection.ExecuteAsync(updatePODetailsQuery, new
                        {
                            PurchaseOrderDetails = podetail.PurchaseOrderDetailId,
                            Transferred = podetail.Transferred,
                            TransferLink = podetail.TransferLink
                        }, transaction);
                    }
                }

                transaction.Commit();
                return "Success: Goods Received Notes saved successfully";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return $"Error: {ex.Message}";
            }
        }


        // GOODS RECEIVED NOTES -> StockTransactions (Qty stays POSITIVE, since receiving goods increases stock)
        public async Task<string> SaveGoodsReceivedNoteTransactions(
            GoodsReceivedNotesModel header,
            List<GoodsReceivedNotesDetails> details)
        {
            var connectionString = await qneConnection.GetConnectionStringAsync();
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            var insertQuery = @"
        INSERT INTO StockTransactions (
            Id, StockId, TransactionDate, TransactionTime, DocumentCode, DocType,
            Description, ItemDescription, Qty, Amount, NetAmount, AmountLocal,
            NetAmountLocal, TaxExclusiveAmountLocal, UOMQty, UOMPrice, DiscountAmount,
            UOMPriceExcLandingCost, LocationId, Projectid, CostCentreId, IsCancelled,
            DocumentId, UOMId, IsBundled
        ) VALUES (
            @Id, @StockId, @TransactionDate, @TransactionTime, @DocumentCode, @DocType,
            @Description, @ItemDescription, @Qty, @Amount, @NetAmount, @AmountLocal,
            @NetAmountLocal, @TaxExclusiveAmountLocal, @UOMQty, @UOMPrice, @DiscountAmount,
            @UOMPriceExcLandingCost, @LocationId, @Projectid, @CostCentreId, @IsCancelled,
            @DocumentId, @UOMId, @IsBundled
        )";

            try
            {
                foreach (var d in details)
                {
                    if (d.StockId is null || d.Qty == 0)
                        continue; // skip blank rows

                    if (d.ItemTypeCode is "T" or "D")
                        continue; // skip subtotal / discount lines — not real stock movements

                    var qty = Math.Abs(d.Qty);   // GRN = stock IN, always positive
                    var price = d.UnitPrice;
                    var amount = qty * price;

                    await connection.ExecuteAsync(insertQuery, new
                    {
                        Id = Guid.NewGuid(),
                        StockId = d.StockId,
                        TransactionDate = header.GoodsReceivedNoteDate,
                        TransactionTime = DateTime.Now.TimeOfDay,
                        DocumentCode = header.GoodsReceivedNoteCode,
                        DocType = "GoodsReceivedNotes",
                        Description = !string.IsNullOrWhiteSpace(header.Notes) ? header.Notes : d.Description,
                        ItemDescription = d.Description,
                        Qty = qty,
                        Amount = amount,
                        NetAmount = d.NetAmount,
                        AmountLocal = d.AmountLocal,
                        NetAmountLocal = d.NetAmountLocal,
                        TaxExclusiveAmountLocal = d.TaxExclusiveAmount,
                        UOMQty = qty,
                        UOMPrice = price,
                        DiscountAmount = d.DiscountAmount,
                        UOMPriceExcLandingCost = price,
                        LocationId = d.StockLocationId ?? header.StockLocationId,
                        Projectid = d.ProjectId ?? header.ProjectId,
                        CostCentreId = d.CostCentreId,
                        IsCancelled = header.IsCancelled,
                        DocumentId = header.Id,
                        UOMId = d.UOMId,
                        IsBundled = d.IsBundled ?? false
                    }, transaction);
                }

                transaction.Commit();
                return "Success: StockTransactions synced";
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                return $"Error: {ex.Message}";
            }
        }

    }
}
