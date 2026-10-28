using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.DescriptionModels;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.ProjectModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Accounting_V4.Model.QNEModels.RunningNumberModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.StocksModels;
using Accounting_V4.Model.QNEModels.TransactionCodesModels;
using Accounting_V4.Pages.QNEPages.Sales.SalesOrder;
using Accounting_V4.Services.OptimisticLockFieldServices;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.TransferServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.SalesServices;

public class SalesOrdersServices
{
    private readonly QneConnectionString qneConnection;
    public SalesTransferLinkServices salesTransferLinkServices;
    public CheckExistServices checkExistServices;

    public SalesOrdersServices(QneConnectionString qneConnection,
        SalesTransferLinkServices salesTransferLinkServices,
        CheckExistServices checkExistServices)
    {
        this.qneConnection = qneConnection;
        this.salesTransferLinkServices = salesTransferLinkServices;
        this.checkExistServices = checkExistServices;
    }
    public async Task<IEnumerable<VW_SalesOrders>> GetSalesOrdersLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT 
            VWSales.Id, SalesOrderDate, SalesOrderCode, DebtorId, DebtorName,Address1,
            CustomerCode, CustomerName, CustomerName2, Term, Currencies.CurrencyCode,
            StaffCode, CurrencyId, CurrencyRate, TaxTotalAmount, TaxTotalAmountLocal,
            TotalAmount, NetTotalAmountLocal, WTaxTotalAmount, WTaxTotalAmountLocal, Attention,
            ReferenceNo, TermId, SalesPersonId, DeliveryTerm, ProjectId,
            DepartmentId, StockLocationId, IsCancelled, RequireDate, Discount,
            DiscountAmount, NetTotalAmount, DiscountAmountLocal, TotalAmountLocal, IsClosed,
            SubtotalAmount, SubtotalAmountLocal, Notes, IsTaxInclusive, TaxDate,
            TaxExclusiveTotalAmount, TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly, DORegistationNo, TransferFromInfo,
            TransferToInfo, IsApproved, SIRef1, SIRef2, AreaCode,
            CategoryCode, ControlAccountCode
        FROM VW_SalesOrders VWSales
        LEFT OUTER JOIN Currencies ON Currencies.Id = VWSales.CurrencyId
        ORDER BY SalesOrderDate DESC";
        var model = await connection.QueryAsync<VW_SalesOrders>(query, commandType: CommandType.Text);
        return model;

    }


    //GET ONE OF SalesOrders DATA
    public async Task<SalesOrdersHeader> GetSalesOrderHeaderById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM SalesOrders WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<SalesOrdersHeader>(query, new { Id });
        return model;

    }

    //GET ONE OF QUOTATIONSDETAILS DATA
    public async Task<IEnumerable<SalesOrderDetails>> GetSalesOrderDetailsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM SalesOrderDetails WHERE SalesOrderId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<SalesOrderDetails>(query, new { Id });
        return model;

    }

    //  SAVE SALES ORDER WITH TRANSFER(QT → SO if any)
    protected CheckExistModel checkExistModel = new CheckExistModel();
    protected CheckExistModel sodetailsCheckModel = new CheckExistModel();
    public async Task<string>SaveSalesOrdersWithTransferAsync(SalesOrdersHeader salesOrdersHeader, List<SalesOrderDetails> salesOrderDetails, ForTransferHeader forTransferHeader, List<ForTransferDetails> forTransferDetails)
    {
        checkExistModel.Id = salesOrdersHeader.Id;
        checkExistModel.TableName = "SalesOrders";

        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        //QUERY FOR HEADER INSERT & UPDATE
        var insertHeaderQuery = @"INSERT INTO SalesOrders (
        Id, SalesOrderCode, SalesOrderDate, DebtorId, DebtorName, Attention, Phone, Fax,
        Address1, Address2, Address3, Address4, ReferenceNo, TermId, SalesPersonId, DeliveryTerm,
        Title, Title2, Ref1, Ref2, Ref3, Ref4, Ref5,
        Remark1, Remark2, Remark3, Remark4, Remark5,
        ProjectId, DepartmentId, StockLocationId, CurrencyId, CurrencyRate,
        TotalAmount, NetTotalAmountLocal, IsCancelled, RequireDate,
        DOBranchCode, DOBranchName, DOContact, DOPhone, DOFax,
        DOAddress1, DOAddress2, DOAddress3, DOAddress4,
        CostCentreId, Discount, DiscountAmount, NetTotalAmount, DiscountAmountLocal, TotalAmountLocal,
        IsClosed, TaxTotalAmount, TaxTotalAmountLocal, SubtotalAmount, SubtotalAmountLocal,
        ExportCountry, Notes, IsTaxInclusive, TaxDate,
        TaxExclusiveTotalAmount, TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly,
        RoundingAdjustmentAccount, RoundingAdjustment, RoundingAdjustmentLocal, IsRounding,
        OptimisticLockField, WTaxTotalAmount, WTaxTotalAmountLocal,
        DORegistationNo, DOGSTRegNo, DOPhone2, DOEmail, DORemark, DeliveryArea,
        SgstTotalAmount, SgstTotalAmountLocal, CgstTotalAmount, CgstTotalAmountLocal,
        IgstTotalAmount, IgstTotalAmountLocal,
        TransferFromInfo, TransferToInfo, IsApproved, ReverseRate,
        SIRef1, SIRef2, TotalServiceAmount, TotalServiceAmountLocal,
        FixedCommissionInfoId, IsNonTaxInvoice, BillerCode,
        JomPayRefType1, JomPayRef1, JomPayRefType2, JomPayRef2,
        UseCreditCard, IsPast, TransferToPOInfo, IsSelfBearWTax,
        ZipCode, City, StateId, CountryId, TinType, CustomerTIN,
        DimensionId, TotalAmountTaxExempted, TotalAmountTaxExemptedLocal, IsEinvoiceReject
        ) VALUES (
        @Id, @SalesOrderCode, @SalesOrderDate, @DebtorId, @DebtorName, @Attention, @Phone, @Fax,
        @Address1, @Address2, @Address3, @Address4, @ReferenceNo, @TermId, @SalesPersonId, @DeliveryTerm,
        @Title, @Title2, @Ref1, @Ref2, @Ref3, @Ref4, @Ref5,
        @Remark1, @Remark2, @Remark3, @Remark4, @Remark5,
        @ProjectId, @DepartmentId, @StockLocationId, @CurrencyId, @CurrencyRate,
        @TotalAmount, @NetTotalAmountLocal, @IsCancelled, @RequireDate,
        @DOBranchCode, @DOBranchName, @DOContact, @DOPhone, @DOFax,
        @DOAddress1, @DOAddress2, @DOAddress3, @DOAddress4,
        @CostCentreId, @Discount, @DiscountAmount, @NetTotalAmount, @DiscountAmountLocal, @TotalAmountLocal,
        @IsClosed, @TaxTotalAmount, @TaxTotalAmountLocal, @SubtotalAmount, @SubtotalAmountLocal,
        @ExportCountry, @Notes, @IsTaxInclusive, @TaxDate,
        @TaxExclusiveTotalAmount, @TaxExclusiveTotalAmountLocal, @IsTaxInclusiveOnly,
        @RoundingAdjustmentAccount, @RoundingAdjustment, @RoundingAdjustmentLocal, @IsRounding,
        @OptimisticLockField, @WTaxTotalAmount, @WTaxTotalAmountLocal,
        @DORegistationNo, @DOGSTRegNo, @DOPhone2, @DOEmail, @DORemark, @DeliveryArea,
        @SgstTotalAmount, @SgstTotalAmountLocal, @CgstTotalAmount, @CgstTotalAmountLocal,
        @IgstTotalAmount, @IgstTotalAmountLocal,
        @TransferFromInfo, @TransferToInfo, @IsApproved, @ReverseRate,
        @SIRef1, @SIRef2, @TotalServiceAmount, @TotalServiceAmountLocal,
        @FixedCommissionInfoId, @IsNonTaxInvoice, @BillerCode,
        @JomPayRefType1, @JomPayRef1, @JomPayRefType2, @JomPayRef2,
        @UseCreditCard, @IsPast, @TransferToPOInfo, @IsSelfBearWTax,
        @ZipCode, @City, @StateId, @CountryId, @TinType, @CustomerTIN,
        @DimensionId, @TotalAmountTaxExempted, @TotalAmountTaxExemptedLocal, @IsEinvoiceReject
        );";
        var updateHeaderQuery = @"
        UPDATE SalesOrders SET
        SalesOrderCode=@SalesOrderCode, SalesOrderDate=@SalesOrderDate, DebtorId=@DebtorId, DebtorName=@DebtorName,
        Attention=@Attention, Phone=@Phone, Fax=@Fax,
        Address1=@Address1, Address2=@Address2, Address3=@Address3, Address4=@Address4,
        ReferenceNo=@ReferenceNo, TermId=@TermId, SalesPersonId=@SalesPersonId, DeliveryTerm=@DeliveryTerm,
        Title=@Title, Title2=@Title2,
        Ref1=@Ref1, Ref2=@Ref2, Ref3=@Ref3, Ref4=@Ref4, Ref5=@Ref5,
        Remark1=@Remark1, Remark2=@Remark2, Remark3=@Remark3, Remark4=@Remark4, Remark5=@Remark5,
        ProjectId=@ProjectId, DepartmentId=@DepartmentId, StockLocationId=@StockLocationId,
        CurrencyId=@CurrencyId, CurrencyRate=@CurrencyRate,
        TotalAmount=@TotalAmount, NetTotalAmountLocal=@NetTotalAmountLocal,
        IsCancelled=@IsCancelled, RequireDate=@RequireDate,
        DOBranchCode=@DOBranchCode, DOBranchName=@DOBranchName, DOContact=@DOContact,
        DOPhone=@DOPhone, DOFax=@DOFax,
        DOAddress1=@DOAddress1, DOAddress2=@DOAddress2, DOAddress3=@DOAddress3, DOAddress4=@DOAddress4,
        CostCentreId=@CostCentreId,
        Discount=@Discount, DiscountAmount=@DiscountAmount,
        NetTotalAmount=@NetTotalAmount, DiscountAmountLocal=@DiscountAmountLocal, TotalAmountLocal=@TotalAmountLocal,
        IsClosed=@IsClosed,
        TaxTotalAmount=@TaxTotalAmount, TaxTotalAmountLocal=@TaxTotalAmountLocal,
        SubtotalAmount=@SubtotalAmount, SubtotalAmountLocal=@SubtotalAmountLocal,
        ExportCountry=@ExportCountry, Notes=@Notes,
        IsTaxInclusive=@IsTaxInclusive, TaxDate=@TaxDate,
        TaxExclusiveTotalAmount=@TaxExclusiveTotalAmount,
        TaxExclusiveTotalAmountLocal=@TaxExclusiveTotalAmountLocal,
        IsTaxInclusiveOnly=@IsTaxInclusiveOnly,
        RoundingAdjustmentAccount=@RoundingAdjustmentAccount,
        RoundingAdjustment=@RoundingAdjustment,
        RoundingAdjustmentLocal=@RoundingAdjustmentLocal,
        IsRounding=@IsRounding,
        OptimisticLockField = OptimisticLockField + 1,
        WTaxTotalAmount=@WTaxTotalAmount,
        WTaxTotalAmountLocal=@WTaxTotalAmountLocal,
        DORegistationNo=@DORegistationNo,
        DOGSTRegNo=@DOGSTRegNo,
        DOPhone2=@DOPhone2,
        DOEmail=@DOEmail,
        DORemark=@DORemark,
        DeliveryArea=@DeliveryArea,
        SgstTotalAmount=@SgstTotalAmount,
        SgstTotalAmountLocal=@SgstTotalAmountLocal,
        CgstTotalAmount=@CgstTotalAmount,
        CgstTotalAmountLocal=@CgstTotalAmountLocal,
        IgstTotalAmount=@IgstTotalAmount,
        IgstTotalAmountLocal=@IgstTotalAmountLocal,
        TransferFromInfo=@TransferFromInfo,
        TransferToInfo=@TransferToInfo,
        IsApproved=@IsApproved,
        ReverseRate=@ReverseRate,
        SIRef1=@SIRef1,
        SIRef2=@SIRef2,
        TotalServiceAmount=@TotalServiceAmount,
        TotalServiceAmountLocal=@TotalServiceAmountLocal,
        FixedCommissionInfoId=@FixedCommissionInfoId,
        IsNonTaxInvoice=@IsNonTaxInvoice,
        BillerCode=@BillerCode,
        JomPayRefType1=@JomPayRefType1,
        JomPayRef1=@JomPayRef1,
        JomPayRefType2=@JomPayRefType2,
        JomPayRef2=@JomPayRef2,
        UseCreditCard=@UseCreditCard,
        IsPast=@IsPast,
        TransferToPOInfo=@TransferToPOInfo,
        IsSelfBearWTax=@IsSelfBearWTax,
        ZipCode=@ZipCode,
        City=@City,
        StateId=@StateId,
        CountryId=@CountryId,
        TinType=@TinType,
        CustomerTIN=@CustomerTIN,
        DimensionId=@DimensionId,
        TotalAmountTaxExempted=@TotalAmountTaxExempted,
        TotalAmountTaxExemptedLocal=@TotalAmountTaxExemptedLocal,
        IsEinvoiceReject=@IsEinvoiceReject
        WHERE Id=@Id";

        //QUERY FOR DETAILS INSERT & UPDATE
        var insertDetailQuery = @"
        INSERT INTO SalesOrderDetails
        (
        Id, Qty, UOMId, UnitPrice, Discount, SalesTax, StockLocationId,
        FromType, FromDocId, FromDtlId, IsPartialTransfer,
        SalesOrderId, StockId, Pos, Description, ProjectId, ReferenceNo,
        Amount, DepartmentId, Numbering, CostCentreId,
        AmountLocal, DiscountAmount, DiscountAmountLocal,
        NetAmount, NetAmountLocal,
        Note, ItemTypeCode, RequireDate,
        IsBundled, IsSubItem, BundledTransactionId,
        TaxCodeId, IsTaxInclusive, TaxRate, TaxClass, TaxClassValue,
        TaxAmount, TaxAmountLocal,
        SubAmount, SubAmountLocal,
        TaxExclusiveAmount, TaxExclusiveAmountLocal,
        TaxClassValueRef,
        TransferLink, Transferred, StockType,
        MSUnitCost, MSReferenceID, OptimisticLockField,
        WTaxCode, WTaxRate, WTaxAmount, WTaxAmountLocal,
        CancelledQty,
        SgstAmount, CgstAmount, IgstAmount,
        SgstAmountLocal, CgstAmountLocal, IgstAmountLocal,
        DebtorItemId, StockBatchNumberId,
        Ref, Ref2, Ref3, Ref4, Ref5,
        DateRef1, DateRef2,
        NumRef1, NumRef2,
        SerialNumber, SerialNumberRef1, SerialNumberRef2,
        TariffCodeId,
        LocalOneCentsDiff,
        ServiceAmount, ServiceAmountLocal,
        SubcodeId, CommissionGroupId, ServiceCost,
        PackingUOMId, Packing, PackingQty, PackingUOM1, PackingUOM2, HighestUOM,
        ProgressInvoicingRate,
        TransferToPOInfo,
        RefTransferFrom,
        DimensionId,
        TaxExemptNo, TaxExemptDetails,
        MarkupDiscountAmount, MarkupDiscountAmountLocal,
        AmountTaxExempted, AmountTaxExemptedLocal,
        MarkupDiscountPrice,
        StoredPoint, StoredPointQty
        )
        VALUES
        (
        @Id, @Qty, @UOMId, @UnitPrice, @Discount, @SalesTax, @StockLocationId,
        @FromType, @FromDocId, @FromDtlId, @IsPartialTransfer,
        @SalesOrderId, @StockId, @Pos, @Description, @ProjectId, @ReferenceNo,
        @Amount, @DepartmentId, @Numbering, @CostCentreId,
        @AmountLocal, @DiscountAmount, @DiscountAmountLocal,
        @NetAmount, @NetAmountLocal,
        @Note, @ItemTypeCode, @RequireDate,
        @IsBundled, @IsSubItem, @BundledTransactionId,
        @TaxCodeId, @IsTaxInclusive, @TaxRate, @TaxClass, @TaxClassValue,
        @TaxAmount, @TaxAmountLocal,
        @SubAmount, @SubAmountLocal,
        @TaxExclusiveAmount, @TaxExclusiveAmountLocal,
        @TaxClassValueRef,
        @TransferLink, @Transferred, @StockType,
        @MSUnitCost, @MSReferenceID, @OptimisticLockField,
        @WTaxCode, @WTaxRate, @WTaxAmount, @WTaxAmountLocal,
        @CancelledQty,
        @SgstAmount, @CgstAmount, @IgstAmount,
        @SgstAmountLocal, @CgstAmountLocal, @IgstAmountLocal,
        @DebtorItemId, @StockBatchNumberId,
        @Ref, @Ref2, @Ref3, @Ref4, @Ref5,
        @DateRef1, @DateRef2,
        @NumRef1, @NumRef2,
        @SerialNumber, @SerialNumberRef1, @SerialNumberRef2,
        @TariffCodeId,
        @LocalOneCentsDiff,
        @ServiceAmount, @ServiceAmountLocal,
        @SubcodeId, @CommissionGroupId, @ServiceCost,
        @PackingUOMId, @Packing, @PackingQty, @PackingUOM1, @PackingUOM2, @HighestUOM,
        @ProgressInvoicingRate,
        @TransferToPOInfo,
        @RefTransferFrom,
        @DimensionId,
        @TaxExemptNo, @TaxExemptDetails,
        @MarkupDiscountAmount, @MarkupDiscountAmountLocal,
        @AmountTaxExempted, @AmountTaxExemptedLocal,
        @MarkupDiscountPrice,
        @StoredPoint, @StoredPointQty)";
        var updateDetailQuery = @"
        UPDATE SalesOrderDetails SET
        Qty=@Qty, UOMId=@UOMId, UnitPrice=@UnitPrice, Discount=@Discount, SalesTax=@SalesTax, StockLocationId=@StockLocationId,
        FromType=@FromType, FromDocId=@FromDocId, FromDtlId=@FromDtlId, IsPartialTransfer=@IsPartialTransfer,
        SalesOrderId=@SalesOrderId, StockId=@StockId, Pos=@Pos, Description=@Description,
        ProjectId=@ProjectId, ReferenceNo=@ReferenceNo,
        Amount=@Amount, DepartmentId=@DepartmentId, Numbering=@Numbering, CostCentreId=@CostCentreId,
        AmountLocal=@AmountLocal,
        DiscountAmount=@DiscountAmount, DiscountAmountLocal=@DiscountAmountLocal,
        NetAmount=@NetAmount, NetAmountLocal=@NetAmountLocal,
        Note=@Note, ItemTypeCode=@ItemTypeCode, RequireDate=@RequireDate,
        IsBundled=@IsBundled, IsSubItem=@IsSubItem, BundledTransactionId=@BundledTransactionId,
        TaxCodeId=@TaxCodeId, IsTaxInclusive=@IsTaxInclusive,
        TaxRate=@TaxRate, TaxClass=@TaxClass, TaxClassValue=@TaxClassValue,
        TaxAmount=@TaxAmount, TaxAmountLocal=@TaxAmountLocal,
        SubAmount=@SubAmount, SubAmountLocal=@SubAmountLocal,
        TaxExclusiveAmount=@TaxExclusiveAmount,
        TaxExclusiveAmountLocal=@TaxExclusiveAmountLocal,
        TaxClassValueRef=@TaxClassValueRef,
        TransferLink=@TransferLink, Transferred=@Transferred,
        StockType=@StockType,
        MSUnitCost=@MSUnitCost, MSReferenceID=@MSReferenceID,
        OptimisticLockField = OptimisticLockField + 1,
        WTaxCode=@WTaxCode, WTaxRate=@WTaxRate,
        WTaxAmount=@WTaxAmount, WTaxAmountLocal=@WTaxAmountLocal,
        CancelledQty=@CancelledQty,
        SgstAmount=@SgstAmount, CgstAmount=@CgstAmount, IgstAmount=@IgstAmount,
        SgstAmountLocal=@SgstAmountLocal, CgstAmountLocal=@CgstAmountLocal, IgstAmountLocal=@IgstAmountLocal,
        DebtorItemId=@DebtorItemId,
        StockBatchNumberId=@StockBatchNumberId,
        Ref=@Ref, Ref2=@Ref2, Ref3=@Ref3, Ref4=@Ref4, Ref5=@Ref5,
        DateRef1=@DateRef1, DateRef2=@DateRef2,
        NumRef1=@NumRef1, NumRef2=@NumRef2,
        SerialNumber=@SerialNumber, SerialNumberRef1=@SerialNumberRef1, SerialNumberRef2=@SerialNumberRef2,
        TariffCodeId=@TariffCodeId,
        LocalOneCentsDiff=@LocalOneCentsDiff,
        ServiceAmount=@ServiceAmount, ServiceAmountLocal=@ServiceAmountLocal,
        SubcodeId=@SubcodeId,
        CommissionGroupId=@CommissionGroupId,
        ServiceCost=@ServiceCost,
        PackingUOMId=@PackingUOMId,
        Packing=@Packing,
        PackingQty=@PackingQty,
        PackingUOM1=@PackingUOM1,
        PackingUOM2=@PackingUOM2,
        HighestUOM=@HighestUOM,
        ProgressInvoicingRate=@ProgressInvoicingRate,
        TransferToPOInfo=@TransferToPOInfo,
        RefTransferFrom=@RefTransferFrom,
        DimensionId=@DimensionId,
        TaxExemptNo=@TaxExemptNo,
        TaxExemptDetails=@TaxExemptDetails,
        MarkupDiscountAmount=@MarkupDiscountAmount,
        MarkupDiscountAmountLocal=@MarkupDiscountAmountLocal,
        AmountTaxExempted=@AmountTaxExempted,
        AmountTaxExemptedLocal=@AmountTaxExemptedLocal,
        MarkupDiscountPrice=@MarkupDiscountPrice,
        StoredPoint=@StoredPoint,
        StoredPointQty=@StoredPointQty
        WHERE Id=@Id";

        try
        {
            var existheader = await checkExistServices.RecordExistsByIdAsync(checkExistModel);
            if (!existheader)
            {
                await connection.ExecuteAsync(insertHeaderQuery, salesOrdersHeader, transaction);

                var updateQTHeaderQuery = @"UPDATE Quotations
                SET TransferToInfo = 
                    CASE 
                        WHEN TransferToInfo IS NULL OR TransferToInfo = '' THEN @TransferToInfo
                        ELSE TransferToInfo + ', ' + @TransferToInfo
                    END
                WHERE Id = @QuotationsHeaderId";

                await connection.ExecuteAsync(updateQTHeaderQuery, forTransferHeader, transaction);

                // === IF TRANSFER MODE ===
                await salesTransferLinkServices.InsertSalesTransferLink(forTransferDetails);
            }
            else
            {
                await connection.ExecuteAsync(updateHeaderQuery, salesOrdersHeader, transaction);
            }

            for (int i = 0; i < salesOrderDetails.Count; i++)
            {
                var detail = salesOrderDetails[i];


                //sodetailsCheckModel
                sodetailsCheckModel.Id = detail.Id;
                sodetailsCheckModel.TableName = "SalesOrderDetails";
                var exists = await checkExistServices.RecordExistsByIdAsync(sodetailsCheckModel);
                if (!exists)
                {
                    // Set TransferLink for each SalesOrderDetails based on ForTransferDetails
                    if (i < forTransferDetails.Count)
                        detail.TransferLink = forTransferDetails[i].Id; // or .TransferLink kung meron

                    //UPDATE Quotation DETAILS Transferred field

                    var updateQTDetailsQuery = @"
                UPDATE QuotationDetails
                SET Transferred = @Transferred,
                    OptimisticLockField = OptimisticLockField + 1
                WHERE Id = @QuotationDetailId";
                    foreach (var qtdetail in forTransferDetails)
                    {
                        await connection.ExecuteAsync(updateQTDetailsQuery,
                        new
                        {
                            QuotationDetailId = qtdetail.QuotationDetailId,
                            Transferred = qtdetail.Transferred
                        },
                        transaction);
                    }
                    await connection.ExecuteAsync(insertDetailQuery, detail, transaction);
                }

                else
                {
                    await connection.ExecuteAsync(updateDetailQuery, detail, transaction);
                }
     
            }


            // ===  COMMIT ===
            transaction.Commit();
            return "Success: Sales Order saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }





    }



    public async Task<string> DeleteDetailRows(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var updatedetailsquery = @"DELETE FROM QuotationDetails WHERE Id=@Id";
        var p = new DynamicParameters();
        p.Add("@Id", Id);

        try
        {
            var deleteDetailRows = await connection.ExecuteAsync(updatedetailsquery, p, transaction);
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




    ///FOR TRANSFER SERVICE









}
