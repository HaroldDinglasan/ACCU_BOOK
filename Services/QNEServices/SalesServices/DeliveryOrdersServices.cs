using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.DeliveryOrderModels;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Pages.QNEPages.Sales.DeliveryOrder;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.TransferServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.SalesServices;

public class DeliveryOrdersServices
{
    private readonly QneConnectionString qneConnection;
    public SalesTransferLinkServices salesTransferLinkServices;
    public CheckExistServices checkExistServices;

    public DeliveryOrdersServices(QneConnectionString qneConnection, SalesTransferLinkServices salesTransferLinkServices, CheckExistServices checkExistServices)
    {
        this.qneConnection = qneConnection;
        this.salesTransferLinkServices = salesTransferLinkServices;
        this.checkExistServices = checkExistServices;
    }

    public async Task<IEnumerable<VW_DeliveryOrders>> GetDeliveryOrderLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT 
            VWDO.Id, DeliveryOrderDate, DeliveryOrderCode, DebtorId, DebtorName, Address1,
            CustomerCode, CustomerName, CustomerName2, Term, Currencies.CurrencyCode,
            StaffCode, CurrencyId, CurrencyRate, TaxTotalAmount, TaxTotalAmountLocal,
            TotalAmount, NetTotalAmountLocal, WTaxTotalAmount, WTaxTotalAmountLocal, Attention,
            ReferenceNo, TermId, SalesPersonId, DeliveryTerm, ProjectId,
            DepartmentId, StockLocationId, IsCancelled, Discount,
            DiscountAmount, NetTotalAmount, DiscountAmountLocal, TotalAmountLocal, IsClosed,
            SubtotalAmount, SubtotalAmountLocal, Notes, IsTaxInclusive, TaxDate,
            TaxExclusiveTotalAmount, TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly, DORegistationNo, TransferFromInfo,
            TransferToInfo, IsApproved, SIRef1, SIRef2, AreaCode,
            CategoryCode, ControlAccountCode
        FROM VW_DeliveryOrders VWDO
        LEFT OUTER JOIN Currencies ON Currencies.Id = VWDO.CurrencyId
        ORDER BY DeliveryOrderDate DESC";
        var model = await connection.QueryAsync<VW_DeliveryOrders>(query, commandType: CommandType.Text);
        return model;

    }

    //GET ONE OF DeliveryOrderHeader DATA
    public async Task<DeliveryOrdersHeader> GetDeliveryOrderHeaderById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM DeliveryOrders WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<DeliveryOrdersHeader>(query, new { Id });
        return model;

    }

    //GET ONE OF DeliveryOrderDetails DATA
    public async Task<IEnumerable<DeliveryOrderDetails>> GetDeliveryOrderDetailsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM DeliveryOrderDetails WHERE DeliveryOrderId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<DeliveryOrderDetails>(query, new { Id });
        return model;

    }


    public async Task<string> SaveDeliveryOrderHeader(DeliveryOrdersHeader deliveryOrdersHeader)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var insertheaderquery = @"INSERT INTO DeliveryOrders (
    Id, DeliveryOrderCode, DeliveryOrderDate, DebtorId, DebtorName, Attention, Phone, Fax,
    Address1, Address2, Address3, Address4, ReferenceNo, TermId, SalesPersonId, DeliveryTerm,
    Title, Title2, Ref1, Ref2, Ref3, Ref4, Ref5, Remark1, Remark2, Remark3, Remark4, Remark5,
    ProjectId, DepartmentId, StockLocationId, CurrencyId, CurrencyRate, TotalAmount, NetTotalAmountLocal,
    IsCancelled, IsDelivered, IsClosed, DOBranchCode, DOBranchName, DOContact, DOPhone, DOFax,
    DOAddress1, DOAddress2, DOAddress3, DOAddress4, CostCentreId, Discount, DiscountAmount,
    NetTotalAmount, DiscountAmountLocal, TotalAmountLocal, TaxTotalAmount, TaxTotalAmountLocal,
    SubtotalAmount, SubtotalAmountLocal, ExportCountry, Notes, IsTaxInclusive, TaxDate,
    TaxExclusiveTotalAmount, TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly,
    RoundingAdjustmentAccount, RoundingAdjustment, RoundingAdjustmentLocal, IsRounding,
    IsTaxDeclared, OptimisticLockField, WTaxTotalAmount, WTaxTotalAmountLocal,
    DORegistationNo, DOGSTRegNo, DOPhone2, DOEmail, DORemark, DeliveryArea,
    SgstTotalAmount, SgstTotalAmountLocal, CgstTotalAmount, CgstTotalAmountLocal,
    IgstTotalAmount, IgstTotalAmountLocal, TransferFromInfo, TransferToInfo,
    IsApproved, ReverseRate, SIRef1, SIRef2, TotalServiceAmount, TotalServiceAmountLocal,
    FixedCommissionInfoId, IsNonTaxInvoice, BillerCode, JomPayRefType1, JomPayRef1,
    JomPayRefType2, JomPayRef2, UseCreditCard, PackingListId, IsPast, IsSelfBearWTax,
    ZipCode, City, StateId, CountryId, TinType, CustomerTIN, DimensionId,
    TotalAmountTaxExempted, TotalAmountTaxExemptedLocal, IsEinvoiceReject
) VALUES (
    @Id, @DeliveryOrderCode, @DeliveryOrderDate, @DebtorId, @DebtorName, @Attention, @Phone, @Fax,
    @Address1, @Address2, @Address3, @Address4, @ReferenceNo, @TermId, @SalesPersonId, @DeliveryTerm,
    @Title, @Title2, @Ref1, @Ref2, @Ref3, @Ref4, @Ref5, @Remark1, @Remark2, @Remark3, @Remark4, @Remark5,
    @ProjectId, @DepartmentId, @StockLocationId, @CurrencyId, @CurrencyRate, @TotalAmount, @NetTotalAmountLocal,
    @IsCancelled, @IsDelivered, @IsClosed, @DOBranchCode, @DOBranchName, @DOContact, @DOPhone, @DOFax,
    @DOAddress1, @DOAddress2, @DOAddress3, @DOAddress4, @CostCentreId, @Discount, @DiscountAmount,
    @NetTotalAmount, @DiscountAmountLocal, @TotalAmountLocal, @TaxTotalAmount, @TaxTotalAmountLocal,
    @SubtotalAmount, @SubtotalAmountLocal, @ExportCountry, @Notes, @IsTaxInclusive, @TaxDate,
    @TaxExclusiveTotalAmount, @TaxExclusiveTotalAmountLocal, @IsTaxInclusiveOnly,
    @RoundingAdjustmentAccount, @RoundingAdjustment, @RoundingAdjustmentLocal, @IsRounding,
    @IsTaxDeclared, @OptimisticLockField, @WTaxTotalAmount, @WTaxTotalAmountLocal,
    @DORegistationNo, @DOGSTRegNo, @DOPhone2, @DOEmail, @DORemark, @DeliveryArea,
    @SgstTotalAmount, @SgstTotalAmountLocal, @CgstTotalAmount, @CgstTotalAmountLocal,
    @IgstTotalAmount, @IgstTotalAmountLocal, @TransferFromInfo, @TransferToInfo,
    @IsApproved, @ReverseRate, @SIRef1, @SIRef2, @TotalServiceAmount, @TotalServiceAmountLocal,
    @FixedCommissionInfoId, @IsNonTaxInvoice, @BillerCode, @JomPayRefType1, @JomPayRef1,
    @JomPayRefType2, @JomPayRef2, @UseCreditCard, @PackingListId, @IsPast, @IsSelfBearWTax,
    @ZipCode, @City, @StateId, @CountryId, @TinType, @CustomerTIN, @DimensionId,
    @TotalAmountTaxExempted, @TotalAmountTaxExemptedLocal, @IsEinvoiceReject
)";
        var updateheaderquery = @"UPDATE DeliveryOrders SET
    DeliveryOrderCode = @DeliveryOrderCode,
    DeliveryOrderDate = @DeliveryOrderDate,
    DebtorId = @DebtorId,
    DebtorName = @DebtorName,
    Attention = @Attention,
    Phone = @Phone,
    Fax = @Fax,
    Address1 = @Address1,
    Address2 = @Address2,
    Address3 = @Address3,
    Address4 = @Address4,
    ReferenceNo = @ReferenceNo,
    TermId = @TermId,
    SalesPersonId = @SalesPersonId,
    DeliveryTerm = @DeliveryTerm,
    Title = @Title,
    Title2 = @Title2,
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
    ProjectId = @ProjectId,
    DepartmentId = @DepartmentId,
    StockLocationId = @StockLocationId,
    CurrencyId = @CurrencyId,
    CurrencyRate = @CurrencyRate,
    TotalAmount = @TotalAmount,
    NetTotalAmountLocal = @NetTotalAmountLocal,
    IsCancelled = @IsCancelled,
    IsDelivered = @IsDelivered,
    IsClosed = @IsClosed,
    DOBranchCode = @DOBranchCode,
    DOBranchName = @DOBranchName,
    DOContact = @DOContact,
    DOPhone = @DOPhone,
    DOFax = @DOFax,
    DOAddress1 = @DOAddress1,
    DOAddress2 = @DOAddress2,
    DOAddress3 = @DOAddress3,
    DOAddress4 = @DOAddress4,
    CostCentreId = @CostCentreId,
    Discount = @Discount,
    DiscountAmount = @DiscountAmount,
    NetTotalAmount = @NetTotalAmount,
    DiscountAmountLocal = @DiscountAmountLocal,
    TotalAmountLocal = @TotalAmountLocal,
    TaxTotalAmount = @TaxTotalAmount,
    TaxTotalAmountLocal = @TaxTotalAmountLocal,
    SubtotalAmount = @SubtotalAmount,
    SubtotalAmountLocal = @SubtotalAmountLocal,
    ExportCountry = @ExportCountry,
    Notes = @Notes,
    IsTaxInclusive = @IsTaxInclusive,
    TaxDate = @TaxDate,
    TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
    TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal,
    IsTaxInclusiveOnly = @IsTaxInclusiveOnly,
    RoundingAdjustmentAccount = @RoundingAdjustmentAccount,
    RoundingAdjustment = @RoundingAdjustment,
    RoundingAdjustmentLocal = @RoundingAdjustmentLocal,
    IsRounding = @IsRounding,
    IsTaxDeclared = @IsTaxDeclared,
    OptimisticLockField = @OptimisticLockField,
    WTaxTotalAmount = @WTaxTotalAmount,
    WTaxTotalAmountLocal = @WTaxTotalAmountLocal,
    DORegistationNo = @DORegistationNo,
    DOGSTRegNo = @DOGSTRegNo,
    DOPhone2 = @DOPhone2,
    DOEmail = @DOEmail,
    DORemark = @DORemark,
    DeliveryArea = @DeliveryArea,
    SgstTotalAmount = @SgstTotalAmount,
    SgstTotalAmountLocal = @SgstTotalAmountLocal,
    CgstTotalAmount = @CgstTotalAmount,
    CgstTotalAmountLocal = @CgstTotalAmountLocal,
    IgstTotalAmount = @IgstTotalAmount,
    IgstTotalAmountLocal = @IgstTotalAmountLocal,
    TransferFromInfo = @TransferFromInfo,
    TransferToInfo = @TransferToInfo,
    IsApproved = @IsApproved,
    ReverseRate = @ReverseRate,
    SIRef1 = @SIRef1,
    SIRef2 = @SIRef2,
    TotalServiceAmount = @TotalServiceAmount,
    TotalServiceAmountLocal = @TotalServiceAmountLocal,
    FixedCommissionInfoId = @FixedCommissionInfoId,
    IsNonTaxInvoice = @IsNonTaxInvoice,
    BillerCode = @BillerCode,
    JomPayRefType1 = @JomPayRefType1,
    JomPayRef1 = @JomPayRef1,
    JomPayRefType2 = @JomPayRefType2,
    JomPayRef2 = @JomPayRef2,
    UseCreditCard = @UseCreditCard,
    PackingListId = @PackingListId,
    IsPast = @IsPast,
    IsSelfBearWTax = @IsSelfBearWTax,
    ZipCode = @ZipCode,
    City = @City,
    StateId = @StateId,
    CountryId = @CountryId,
    TinType = @TinType,
    CustomerTIN = @CustomerTIN,
    DimensionId = @DimensionId,
    TotalAmountTaxExempted = @TotalAmountTaxExempted,
    TotalAmountTaxExemptedLocal = @TotalAmountTaxExemptedLocal,
    IsEinvoiceReject = @IsEinvoiceReject
WHERE Id = @Id";

        // Check if header data exists
        var existheader = await checkExistServices.RecordExistsByIdAsync(checkExistModel);
        //var existheader = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM DeliveryOrders WHERE Id = @Id", new { deliveryOrdersHeader.Id }, transaction) > 0;

        try
        {
            //icheck yun Id if 0-000-000-000 means Empty then insert else update
            if (!existheader)
            {
                var affectedHeaderInsertRows = await connection.ExecuteAsync(insertheaderquery, deliveryOrdersHeader, transaction);
                transaction.Commit();
                return $"Success {affectedHeaderInsertRows} added!";
            }
            else
            {
                var affectedHeaderUpdateRows = await connection.ExecuteAsync(updateheaderquery, deliveryOrdersHeader, transaction);
                transaction.Commit();
                return $"Success {affectedHeaderUpdateRows} updated!";
            }
        }
        catch (SqlException ex)
        {

            transaction.Rollback();
            if (ex.Message.Contains("Cannot insert duplicate key row in object"))
            {

                if (ex.Message.Contains("Quotations"))
                {
                    return $"{deliveryOrdersHeader.DeliveryOrderCode} already exists. Deliver yOrders No. must be unique.";
                }
                return "Duplicate value detected. Values must be unique.";
            }
            throw;
        }

    }


    public async Task<string> SaveDeliveryOrderDetails(List<DeliveryOrderDetails> deliveryOrdersDetails)
    {

        //var data = salesOrderDetails.ToList();
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertdetailsquery = @"INSERT INTO DeliveryOrderDetails (
    Id, Qty, UOMId, UnitPrice, Discount, SalesTax, StockLocationId, FromType, FromDocId, FromDtlId,
    IsPartialTransfer, DeliveryOrderId, StockId, Pos, Description, ProjectId, ReferenceNo, Amount,
    DepartmentId, Numbering, CostCentreId, AmountLocal, DiscountAmount, DiscountAmountLocal,
    NetAmount, NetAmountLocal, Note, ItemTypeCode, IsBundled, IsSubItem,
    BundledTransactionId, TaxCodeId, IsTaxInclusive, TaxRate, TaxClass, TaxClassValue,
    TaxAmount, TaxAmountLocal, SubAmount, SubAmountLocal, TaxExclusiveAmount, TaxExclusiveAmountLocal,
    TaxClassValueRef, TransferLink, Transferred, StockType, MSUnitCost, MSReferenceID,
    OptimisticLockField, WTaxCode, WTaxRate, WTaxAmount, WTaxAmountLocal, CancelledQty,
    SgstAmount, CgstAmount, IgstAmount, SgstAmountLocal, CgstAmountLocal, IgstAmountLocal,
    DebtorItemId, StockBatchNumberId, Ref, Ref2, Ref3, Ref4, Ref5, DateRef1, DateRef2,
    NumRef1, NumRef2, SerialNumber, SerialNumberRef1, SerialNumberRef2, TariffCodeId,
    LocalOneCentsDiff, ServiceAmount, ServiceAmountLocal, SubcodeId, CommissionGroupId,
    ServiceCost, PackingUOMId, Packing, PackingQty, PackingUOM1, PackingUOM2, HighestUOM,
    ProgressInvoicingRate, CancelledSerialNumber, RefTransferFrom, DimensionId, TaxExemptNo,
    TaxExemptDetails, MarkupDiscountAmount, MarkupDiscountAmountLocal, AmountTaxExempted,
    AmountTaxExemptedLocal, MarkupDiscountPrice, StoredPoint, StoredPointQty
) VALUES (
    @Id, @Qty, @UOMId, @UnitPrice, @Discount, @SalesTax, @StockLocationId, @FromType, @FromDocId, @FromDtlId,
    @IsPartialTransfer, @DeliveryOrderId, @StockId, @Pos, @Description, @ProjectId, @ReferenceNo, @Amount,
    @DepartmentId, @Numbering, @CostCentreId, @AmountLocal, @DiscountAmount, @DiscountAmountLocal,
    @NetAmount, @NetAmountLocal, @Note, @ItemTypeCode, @IsBundled, @IsSubItem,
    @BundledTransactionId, @TaxCodeId, @IsTaxInclusive, @TaxRate, @TaxClass, @TaxClassValue,
    @TaxAmount, @TaxAmountLocal, @SubAmount, @SubAmountLocal, @TaxExclusiveAmount, @TaxExclusiveAmountLocal,
    @TaxClassValueRef, @TransferLink, @Transferred, @StockType, @MSUnitCost, @MSReferenceID,
    @OptimisticLockField, @WTaxCode, @WTaxRate, @WTaxAmount, @WTaxAmountLocal, @CancelledQty,
    @SgstAmount, @CgstAmount, @IgstAmount, @SgstAmountLocal, @CgstAmountLocal, @IgstAmountLocal,
    @DebtorItemId, @StockBatchNumberId, @Ref, @Ref2, @Ref3, @Ref4, @Ref5, @DateRef1, @DateRef2,
    @NumRef1, @NumRef2, @SerialNumber, @SerialNumberRef1, @SerialNumberRef2, @TariffCodeId,
    @LocalOneCentsDiff, @ServiceAmount, @ServiceAmountLocal, @SubcodeId, @CommissionGroupId,
    @ServiceCost, @PackingUOMId, @Packing, @PackingQty, @PackingUOM1, @PackingUOM2, @HighestUOM,
    @ProgressInvoicingRate, @CancelledSerialNumber, @RefTransferFrom, @DimensionId, @TaxExemptNo,
    @TaxExemptDetails, @MarkupDiscountAmount, @MarkupDiscountAmountLocal, @AmountTaxExempted,
    @AmountTaxExemptedLocal, @MarkupDiscountPrice, @StoredPoint, @StoredPointQty
);";

        var updatedetailsquery = @"UPDATE DeliveryOrderDetails SET
    Qty = @Qty,
    UOMId = @UOMId,
    UnitPrice = @UnitPrice,
    Discount = @Discount,
    SalesTax = @SalesTax,
    StockLocationId = @StockLocationId,
    FromType = @FromType,
    FromDocId = @FromDocId,
    FromDtlId = @FromDtlId,
    IsPartialTransfer = @IsPartialTransfer,
    DeliveryOrderId = @DeliveryOrderId,
    StockId = @StockId,
    Pos = @Pos,
    Description = @Description,
    ProjectId = @ProjectId,
    ReferenceNo = @ReferenceNo,
    Amount = @Amount,
    DepartmentId = @DepartmentId,
    Numbering = @Numbering,
    CostCentreId = @CostCentreId,
    AmountLocal = @AmountLocal,
    DiscountAmount = @DiscountAmount,
    DiscountAmountLocal = @DiscountAmountLocal,
    NetAmount = @NetAmount,
    NetAmountLocal = @NetAmountLocal,
    Note = @Note,
    ItemTypeCode = @ItemTypeCode,
    IsBundled = @IsBundled,
    IsSubItem = @IsSubItem,
    BundledTransactionId = @BundledTransactionId,
    TaxCodeId = @TaxCodeId,
    IsTaxInclusive = @IsTaxInclusive,
    TaxRate = @TaxRate,
    TaxClass = @TaxClass,
    TaxClassValue = @TaxClassValue,
    TaxAmount = @TaxAmount,
    TaxAmountLocal = @TaxAmountLocal,
    SubAmount = @SubAmount,
    SubAmountLocal = @SubAmountLocal,
    TaxExclusiveAmount = @TaxExclusiveAmount,
    TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal,
    TaxClassValueRef = @TaxClassValueRef,
    TransferLink = @TransferLink,
    Transferred = @Transferred,
    StockType = @StockType,
    MSUnitCost = @MSUnitCost,
    MSReferenceID = @MSReferenceID,
    OptimisticLockField = @OptimisticLockField,
    WTaxCode = @WTaxCode,
    WTaxRate = @WTaxRate,
    WTaxAmount = @WTaxAmount,
    WTaxAmountLocal = @WTaxAmountLocal,
    CancelledQty = @CancelledQty,
    SgstAmount = @SgstAmount,
    CgstAmount = @CgstAmount,
    IgstAmount = @IgstAmount,
    SgstAmountLocal = @SgstAmountLocal,
    CgstAmountLocal = @CgstAmountLocal,
    IgstAmountLocal = @IgstAmountLocal,
    DebtorItemId = @DebtorItemId,
    StockBatchNumberId = @StockBatchNumberId,
    Ref = @Ref,
    Ref2 = @Ref2,
    Ref3 = @Ref3,
    Ref4 = @Ref4,
    Ref5 = @Ref5,
    DateRef1 = @DateRef1,
    DateRef2 = @DateRef2,
    NumRef1 = @NumRef1,
    NumRef2 = @NumRef2,
    SerialNumber = @SerialNumber,
    SerialNumberRef1 = @SerialNumberRef1,
    SerialNumberRef2 = @SerialNumberRef2,
    TariffCodeId = @TariffCodeId,
    LocalOneCentsDiff = @LocalOneCentsDiff,
    ServiceAmount = @ServiceAmount,
    ServiceAmountLocal = @ServiceAmountLocal,
    SubcodeId = @SubcodeId,
    CommissionGroupId = @CommissionGroupId,
    ServiceCost = @ServiceCost,
    PackingUOMId = @PackingUOMId,
    Packing = @Packing,
    PackingQty = @PackingQty,
    PackingUOM1 = @PackingUOM1,
    PackingUOM2 = @PackingUOM2,
    HighestUOM = @HighestUOM,
    ProgressInvoicingRate = @ProgressInvoicingRate,
    CancelledSerialNumber = @CancelledSerialNumber,
    RefTransferFrom = @RefTransferFrom,
    DimensionId = @DimensionId,
    TaxExemptNo = @TaxExemptNo,
    TaxExemptDetails = @TaxExemptDetails,
    MarkupDiscountAmount = @MarkupDiscountAmount,
    MarkupDiscountAmountLocal = @MarkupDiscountAmountLocal,
    AmountTaxExempted = @AmountTaxExempted,
    AmountTaxExemptedLocal = @AmountTaxExemptedLocal,
    MarkupDiscountPrice = @MarkupDiscountPrice,
    StoredPoint = @StoredPoint,
    StoredPointQty = @StoredPointQty
WHERE Id = @Id;";

        //issue here
        //walang transfer from at transfer to na data - issue
        try
        {
            //var message = string.Empty;
            foreach (var detail in deliveryOrdersDetails)
            {
                var existheader = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM DeliveryOrderDetails WHERE Id = @Id", new { detail.Id }, transaction) > 0;
                if (!existheader)
                {
                    // var affectedDetailsInsertRows = 
                    await connection.ExecuteAsync(insertdetailsquery, detail, transaction);

                }
                else
                {
                    //var affectedDetailsUpdateRows = 
                    await connection.ExecuteAsync(updatedetailsquery, detail, transaction);

                }
            }
            transaction.Commit();
            return $"Success updated!";
            //  return $"{message}";

        }
        catch (SqlException ex)
        {

            transaction.Rollback();
            return ex.Message;
            throw;
        }
    }

    protected CheckExistModel checkExistModel = new CheckExistModel();
    protected CheckExistModel dodetailsCheckModel = new CheckExistModel();
    public async Task<string> SaveDeliveryOrderWithTransferAsync(DeliveryOrdersHeader deliveryOrdersHeader, List<DeliveryOrderDetails> deliveryOrderDetails, ForTransferHeader forTransferHeader, List<ForTransferDetails> forTransferDetails)
    {
        checkExistModel.Id = deliveryOrdersHeader.Id;
        checkExistModel.TableName = "DeliveryOrders";

        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertHeaderQuery = @"INSERT INTO DeliveryOrders (
    Id, DeliveryOrderCode, DeliveryOrderDate, DebtorId, DebtorName, Attention, Phone, Fax,
    Address1, Address2, Address3, Address4, ReferenceNo, TermId, SalesPersonId, DeliveryTerm,
    Title, Title2, Ref1, Ref2, Ref3, Ref4, Ref5, Remark1, Remark2, Remark3, Remark4, Remark5,
    ProjectId, DepartmentId, StockLocationId, CurrencyId, CurrencyRate, TotalAmount, NetTotalAmountLocal,
    IsCancelled, IsDelivered, IsClosed, DOBranchCode, DOBranchName, DOContact, DOPhone, DOFax,
    DOAddress1, DOAddress2, DOAddress3, DOAddress4, CostCentreId, Discount, DiscountAmount,
    NetTotalAmount, DiscountAmountLocal, TotalAmountLocal, TaxTotalAmount, TaxTotalAmountLocal,
    SubtotalAmount, SubtotalAmountLocal, ExportCountry, Notes, IsTaxInclusive, TaxDate,
    TaxExclusiveTotalAmount, TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly,
    RoundingAdjustmentAccount, RoundingAdjustment, RoundingAdjustmentLocal, IsRounding,
    IsTaxDeclared, OptimisticLockField, WTaxTotalAmount, WTaxTotalAmountLocal,
    DORegistationNo, DOGSTRegNo, DOPhone2, DOEmail, DORemark, DeliveryArea,
    SgstTotalAmount, SgstTotalAmountLocal, CgstTotalAmount, CgstTotalAmountLocal,
    IgstTotalAmount, IgstTotalAmountLocal, TransferFromInfo, TransferToInfo,
    IsApproved, ReverseRate, SIRef1, SIRef2, TotalServiceAmount, TotalServiceAmountLocal,
    FixedCommissionInfoId, IsNonTaxInvoice, BillerCode, JomPayRefType1, JomPayRef1,
    JomPayRefType2, JomPayRef2, UseCreditCard, PackingListId, IsPast, IsSelfBearWTax,
    ZipCode, City, StateId, CountryId, TinType, CustomerTIN, DimensionId,
    TotalAmountTaxExempted, TotalAmountTaxExemptedLocal, IsEinvoiceReject
) VALUES (
    @Id, @DeliveryOrderCode, @DeliveryOrderDate, @DebtorId, @DebtorName, @Attention, @Phone, @Fax,
    @Address1, @Address2, @Address3, @Address4, @ReferenceNo, @TermId, @SalesPersonId, @DeliveryTerm,
    @Title, @Title2, @Ref1, @Ref2, @Ref3, @Ref4, @Ref5, @Remark1, @Remark2, @Remark3, @Remark4, @Remark5,
    @ProjectId, @DepartmentId, @StockLocationId, @CurrencyId, @CurrencyRate, @TotalAmount, @NetTotalAmountLocal,
    @IsCancelled, @IsDelivered, @IsClosed, @DOBranchCode, @DOBranchName, @DOContact, @DOPhone, @DOFax,
    @DOAddress1, @DOAddress2, @DOAddress3, @DOAddress4, @CostCentreId, @Discount, @DiscountAmount,
    @NetTotalAmount, @DiscountAmountLocal, @TotalAmountLocal, @TaxTotalAmount, @TaxTotalAmountLocal,
    @SubtotalAmount, @SubtotalAmountLocal, @ExportCountry, @Notes, @IsTaxInclusive, @TaxDate,
    @TaxExclusiveTotalAmount, @TaxExclusiveTotalAmountLocal, @IsTaxInclusiveOnly,
    @RoundingAdjustmentAccount, @RoundingAdjustment, @RoundingAdjustmentLocal, @IsRounding,
    @IsTaxDeclared, @OptimisticLockField, @WTaxTotalAmount, @WTaxTotalAmountLocal,
    @DORegistationNo, @DOGSTRegNo, @DOPhone2, @DOEmail, @DORemark, @DeliveryArea,
    @SgstTotalAmount, @SgstTotalAmountLocal, @CgstTotalAmount, @CgstTotalAmountLocal,
    @IgstTotalAmount, @IgstTotalAmountLocal, @TransferFromInfo, @TransferToInfo,
    @IsApproved, @ReverseRate, @SIRef1, @SIRef2, @TotalServiceAmount, @TotalServiceAmountLocal,
    @FixedCommissionInfoId, @IsNonTaxInvoice, @BillerCode, @JomPayRefType1, @JomPayRef1,
    @JomPayRefType2, @JomPayRef2, @UseCreditCard, @PackingListId, @IsPast, @IsSelfBearWTax,
    @ZipCode, @City, @StateId, @CountryId, @TinType, @CustomerTIN, @DimensionId,
    @TotalAmountTaxExempted, @TotalAmountTaxExemptedLocal, @IsEinvoiceReject
)";
        var updateHeaderQuery = @"UPDATE DeliveryOrders SET
    DeliveryOrderCode = @DeliveryOrderCode,
    DeliveryOrderDate = @DeliveryOrderDate,
    DebtorId = @DebtorId,
    DebtorName = @DebtorName,
    Attention = @Attention,
    Phone = @Phone,
    Fax = @Fax,
    Address1 = @Address1,
    Address2 = @Address2,
    Address3 = @Address3,
    Address4 = @Address4,
    ReferenceNo = @ReferenceNo,
    TermId = @TermId,
    SalesPersonId = @SalesPersonId,
    DeliveryTerm = @DeliveryTerm,
    Title = @Title,
    Title2 = @Title2,
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
    ProjectId = @ProjectId,
    DepartmentId = @DepartmentId,
    StockLocationId = @StockLocationId,
    CurrencyId = @CurrencyId,
    CurrencyRate = @CurrencyRate,
    TotalAmount = @TotalAmount,
    NetTotalAmountLocal = @NetTotalAmountLocal,
    IsCancelled = @IsCancelled,
    IsDelivered = @IsDelivered,
    IsClosed = @IsClosed,
    DOBranchCode = @DOBranchCode,
    DOBranchName = @DOBranchName,
    DOContact = @DOContact,
    DOPhone = @DOPhone,
    DOFax = @DOFax,
    DOAddress1 = @DOAddress1,
    DOAddress2 = @DOAddress2,
    DOAddress3 = @DOAddress3,
    DOAddress4 = @DOAddress4,
    CostCentreId = @CostCentreId,
    Discount = @Discount,
    DiscountAmount = @DiscountAmount,
    NetTotalAmount = @NetTotalAmount,
    DiscountAmountLocal = @DiscountAmountLocal,
    TotalAmountLocal = @TotalAmountLocal,
    TaxTotalAmount = @TaxTotalAmount,
    TaxTotalAmountLocal = @TaxTotalAmountLocal,
    SubtotalAmount = @SubtotalAmount,
    SubtotalAmountLocal = @SubtotalAmountLocal,
    ExportCountry = @ExportCountry,
    Notes = @Notes,
    IsTaxInclusive = @IsTaxInclusive,
    TaxDate = @TaxDate,
    TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
    TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal,
    IsTaxInclusiveOnly = @IsTaxInclusiveOnly,
    RoundingAdjustmentAccount = @RoundingAdjustmentAccount,
    RoundingAdjustment = @RoundingAdjustment,
    RoundingAdjustmentLocal = @RoundingAdjustmentLocal,
    IsRounding = @IsRounding,
    IsTaxDeclared = @IsTaxDeclared,
    OptimisticLockField = @OptimisticLockField,
    WTaxTotalAmount = @WTaxTotalAmount,
    WTaxTotalAmountLocal = @WTaxTotalAmountLocal,
    DORegistationNo = @DORegistationNo,
    DOGSTRegNo = @DOGSTRegNo,
    DOPhone2 = @DOPhone2,
    DOEmail = @DOEmail,
    DORemark = @DORemark,
    DeliveryArea = @DeliveryArea,
    SgstTotalAmount = @SgstTotalAmount,
    SgstTotalAmountLocal = @SgstTotalAmountLocal,
    CgstTotalAmount = @CgstTotalAmount,
    CgstTotalAmountLocal = @CgstTotalAmountLocal,
    IgstTotalAmount = @IgstTotalAmount,
    IgstTotalAmountLocal = @IgstTotalAmountLocal,
    TransferFromInfo = @TransferFromInfo,
    TransferToInfo = @TransferToInfo,
    IsApproved = @IsApproved,
    ReverseRate = @ReverseRate,
    SIRef1 = @SIRef1,
    SIRef2 = @SIRef2,
    TotalServiceAmount = @TotalServiceAmount,
    TotalServiceAmountLocal = @TotalServiceAmountLocal,
    FixedCommissionInfoId = @FixedCommissionInfoId,
    IsNonTaxInvoice = @IsNonTaxInvoice,
    BillerCode = @BillerCode,
    JomPayRefType1 = @JomPayRefType1,
    JomPayRef1 = @JomPayRef1,
    JomPayRefType2 = @JomPayRefType2,
    JomPayRef2 = @JomPayRef2,
    UseCreditCard = @UseCreditCard,
    PackingListId = @PackingListId,
    IsPast = @IsPast,
    IsSelfBearWTax = @IsSelfBearWTax,
    ZipCode = @ZipCode,
    City = @City,
    StateId = @StateId,
    CountryId = @CountryId,
    TinType = @TinType,
    CustomerTIN = @CustomerTIN,
    DimensionId = @DimensionId,
    TotalAmountTaxExempted = @TotalAmountTaxExempted,
    TotalAmountTaxExemptedLocal = @TotalAmountTaxExemptedLocal,
    IsEinvoiceReject = @IsEinvoiceReject
WHERE Id = @Id";

        var insertDetailQuery = @"INSERT INTO DeliveryOrderDetails (
    Id, Qty, UOMId, UnitPrice, Discount, SalesTax, StockLocationId, FromType, FromDocId, FromDtlId,
    IsPartialTransfer, DeliveryOrderId, StockId, Pos, Description, ProjectId, ReferenceNo, Amount,
    DepartmentId, Numbering, CostCentreId, AmountLocal, DiscountAmount, DiscountAmountLocal,
    NetAmount, NetAmountLocal, Note, ItemTypeCode, IsBundled, IsSubItem,
    BundledTransactionId, TaxCodeId, IsTaxInclusive, TaxRate, TaxClass, TaxClassValue,
    TaxAmount, TaxAmountLocal, SubAmount, SubAmountLocal, TaxExclusiveAmount, TaxExclusiveAmountLocal,
    TaxClassValueRef, TransferLink, Transferred, StockType, MSUnitCost, MSReferenceID,
    OptimisticLockField, WTaxCode, WTaxRate, WTaxAmount, WTaxAmountLocal, CancelledQty,
    SgstAmount, CgstAmount, IgstAmount, SgstAmountLocal, CgstAmountLocal, IgstAmountLocal,
    DebtorItemId, StockBatchNumberId, Ref, Ref2, Ref3, Ref4, Ref5, DateRef1, DateRef2,
    NumRef1, NumRef2, SerialNumber, SerialNumberRef1, SerialNumberRef2, TariffCodeId,
    LocalOneCentsDiff, ServiceAmount, ServiceAmountLocal, SubcodeId, CommissionGroupId,
    ServiceCost, PackingUOMId, Packing, PackingQty, PackingUOM1, PackingUOM2, HighestUOM,
    ProgressInvoicingRate, CancelledSerialNumber, RefTransferFrom, DimensionId, TaxExemptNo,
    TaxExemptDetails, MarkupDiscountAmount, MarkupDiscountAmountLocal, AmountTaxExempted,
    AmountTaxExemptedLocal, MarkupDiscountPrice, StoredPoint, StoredPointQty
) VALUES (
    @Id, @Qty, @UOMId, @UnitPrice, @Discount, @SalesTax, @StockLocationId, @FromType, @FromDocId, @FromDtlId,
    @IsPartialTransfer, @DeliveryOrderId, @StockId, @Pos, @Description, @ProjectId, @ReferenceNo, @Amount,
    @DepartmentId, @Numbering, @CostCentreId, @AmountLocal, @DiscountAmount, @DiscountAmountLocal,
    @NetAmount, @NetAmountLocal, @Note, @ItemTypeCode, @IsBundled, @IsSubItem,
    @BundledTransactionId, @TaxCodeId, @IsTaxInclusive, @TaxRate, @TaxClass, @TaxClassValue,
    @TaxAmount, @TaxAmountLocal, @SubAmount, @SubAmountLocal, @TaxExclusiveAmount, @TaxExclusiveAmountLocal,
    @TaxClassValueRef, @TransferLink, @Transferred, @StockType, @MSUnitCost, @MSReferenceID,
    @OptimisticLockField, @WTaxCode, @WTaxRate, @WTaxAmount, @WTaxAmountLocal, @CancelledQty,
    @SgstAmount, @CgstAmount, @IgstAmount, @SgstAmountLocal, @CgstAmountLocal, @IgstAmountLocal,
    @DebtorItemId, @StockBatchNumberId, @Ref, @Ref2, @Ref3, @Ref4, @Ref5, @DateRef1, @DateRef2,
    @NumRef1, @NumRef2, @SerialNumber, @SerialNumberRef1, @SerialNumberRef2, @TariffCodeId,
    @LocalOneCentsDiff, @ServiceAmount, @ServiceAmountLocal, @SubcodeId, @CommissionGroupId,
    @ServiceCost, @PackingUOMId, @Packing, @PackingQty, @PackingUOM1, @PackingUOM2, @HighestUOM,
    @ProgressInvoicingRate, @CancelledSerialNumber, @RefTransferFrom, @DimensionId, @TaxExemptNo,
    @TaxExemptDetails, @MarkupDiscountAmount, @MarkupDiscountAmountLocal, @AmountTaxExempted,
    @AmountTaxExemptedLocal, @MarkupDiscountPrice, @StoredPoint, @StoredPointQty
);";
        var updateDetailQuery = @"UPDATE DeliveryOrderDetails SET
    Qty = @Qty,
    UOMId = @UOMId,
    UnitPrice = @UnitPrice,
    Discount = @Discount,
    SalesTax = @SalesTax,
    StockLocationId = @StockLocationId,
    FromType = @FromType,
    FromDocId = @FromDocId,
    FromDtlId = @FromDtlId,
    IsPartialTransfer = @IsPartialTransfer,
    DeliveryOrderId = @DeliveryOrderId,
    StockId = @StockId,
    Pos = @Pos,
    Description = @Description,
    ProjectId = @ProjectId,
    ReferenceNo = @ReferenceNo,
    Amount = @Amount,
    DepartmentId = @DepartmentId,
    Numbering = @Numbering,
    CostCentreId = @CostCentreId,
    AmountLocal = @AmountLocal,
    DiscountAmount = @DiscountAmount,
    DiscountAmountLocal = @DiscountAmountLocal,
    NetAmount = @NetAmount,
    NetAmountLocal = @NetAmountLocal,
    Note = @Note,
    ItemTypeCode = @ItemTypeCode,
    IsBundled = @IsBundled,
    IsSubItem = @IsSubItem,
    BundledTransactionId = @BundledTransactionId,
    TaxCodeId = @TaxCodeId,
    IsTaxInclusive = @IsTaxInclusive,
    TaxRate = @TaxRate,
    TaxClass = @TaxClass,
    TaxClassValue = @TaxClassValue,
    TaxAmount = @TaxAmount,
    TaxAmountLocal = @TaxAmountLocal,
    SubAmount = @SubAmount,
    SubAmountLocal = @SubAmountLocal,
    TaxExclusiveAmount = @TaxExclusiveAmount,
    TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal,
    TaxClassValueRef = @TaxClassValueRef,
    TransferLink = @TransferLink,
    Transferred = @Transferred,
    StockType = @StockType,
    MSUnitCost = @MSUnitCost,
    MSReferenceID = @MSReferenceID,
    OptimisticLockField = @OptimisticLockField,
    WTaxCode = @WTaxCode,
    WTaxRate = @WTaxRate,
    WTaxAmount = @WTaxAmount,
    WTaxAmountLocal = @WTaxAmountLocal,
    CancelledQty = @CancelledQty,
    SgstAmount = @SgstAmount,
    CgstAmount = @CgstAmount,
    IgstAmount = @IgstAmount,
    SgstAmountLocal = @SgstAmountLocal,
    CgstAmountLocal = @CgstAmountLocal,
    IgstAmountLocal = @IgstAmountLocal,
    DebtorItemId = @DebtorItemId,
    StockBatchNumberId = @StockBatchNumberId,
    Ref = @Ref,
    Ref2 = @Ref2,
    Ref3 = @Ref3,
    Ref4 = @Ref4,
    Ref5 = @Ref5,
    DateRef1 = @DateRef1,
    DateRef2 = @DateRef2,
    NumRef1 = @NumRef1,
    NumRef2 = @NumRef2,
    SerialNumber = @SerialNumber,
    SerialNumberRef1 = @SerialNumberRef1,
    SerialNumberRef2 = @SerialNumberRef2,
    TariffCodeId = @TariffCodeId,
    LocalOneCentsDiff = @LocalOneCentsDiff,
    ServiceAmount = @ServiceAmount,
    ServiceAmountLocal = @ServiceAmountLocal,
    SubcodeId = @SubcodeId,
    CommissionGroupId = @CommissionGroupId,
    ServiceCost = @ServiceCost,
    PackingUOMId = @PackingUOMId,
    Packing = @Packing,
    PackingQty = @PackingQty,
    PackingUOM1 = @PackingUOM1,
    PackingUOM2 = @PackingUOM2,
    HighestUOM = @HighestUOM,
    ProgressInvoicingRate = @ProgressInvoicingRate,
    CancelledSerialNumber = @CancelledSerialNumber,
    RefTransferFrom = @RefTransferFrom,
    DimensionId = @DimensionId,
    TaxExemptNo = @TaxExemptNo,
    TaxExemptDetails = @TaxExemptDetails,
    MarkupDiscountAmount = @MarkupDiscountAmount,
    MarkupDiscountAmountLocal = @MarkupDiscountAmountLocal,
    AmountTaxExempted = @AmountTaxExempted,
    AmountTaxExemptedLocal = @AmountTaxExemptedLocal,
    MarkupDiscountPrice = @MarkupDiscountPrice,
    StoredPoint = @StoredPoint,
    StoredPointQty = @StoredPointQty
WHERE Id = @Id;";

        try
        {
            var existheader = await checkExistServices.RecordExistsByIdAsync(checkExistModel);
            if (!existheader)
            {
                await connection.ExecuteAsync(insertHeaderQuery, deliveryOrdersHeader, transaction);
                var updateSOHeaderQuery = @"UPDATE SalesOrders
                SET TransferToInfo = 
                    CASE 
                        WHEN TransferToInfo IS NULL OR TransferToInfo = '' THEN @TransferToInfo
                        ELSE TransferToInfo + ', ' + @TransferToInfo
                    END
                WHERE Id = @SalesOrdersHeaderId";
                await connection.ExecuteAsync(updateSOHeaderQuery, forTransferHeader, transaction);
                // === IF TRANSFER MODE ===
                await salesTransferLinkServices.InsertSalesTransferLink(forTransferDetails);
            }
            else
            {
                await connection.ExecuteAsync(updateHeaderQuery, deliveryOrdersHeader, transaction);
            }

            for (int i = 0; i < deliveryOrderDetails.Count; i++)
            {
                var detail = deliveryOrderDetails[i];

                dodetailsCheckModel.Id = detail.Id;
                dodetailsCheckModel.TableName = "DeliveryOrderDetails";
                var exists = await checkExistServices.RecordExistsByIdAsync(dodetailsCheckModel);
                if (!exists)
                {
                    // SET ID if TransferLink Column for DeliveryOrderDetails 
                    if (i < forTransferDetails.Count)
                        detail.TransferLink = forTransferDetails[i].Id; // or .TransferLink kung meron

                    var updateSODetailsQuery = @"
                UPDATE SalesOrderDetails
                SET Transferred = @Transferred,
                    OptimisticLockField = OptimisticLockField + 1
                WHERE Id = @SalesOrderDetailId";
                    foreach (var qtdetail in forTransferDetails)
                    {
                        await connection.ExecuteAsync(updateSODetailsQuery,
                        new
                        {
                            SalesOrderDetailId = qtdetail.SalesOrderDetailId,
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
            return "Success: Delivery Order saved successfully.";
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

        var updatedetailsquery = @"DELETE FROM DeliveryOrderDetails WHERE Id=@Id";
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

}
