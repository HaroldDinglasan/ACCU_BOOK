using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.TransferServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.PurchasesServices;

public class PurchaseOrdersService
{
    private readonly QneConnectionString qneConnection;
    public PurchaseTransferLinkServices purchaseTransferLinkServices;
    public CheckExistServices checkExistServices;

    public PurchaseOrdersService(QneConnectionString qneConnection,
        PurchaseTransferLinkServices purchaseTransferLinkServices,
        CheckExistServices checkExistService)
    {
        this.qneConnection = qneConnection;
        this.purchaseTransferLinkServices = purchaseTransferLinkServices;
        this.checkExistServices = checkExistService;
    }

    public async Task<IEnumerable<VW_PurchaseOrdersListing>> GetPurchaseOrdersList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"
                SELECT
                    P.Id,
                    P.PurchaseOrderCode,
                    P.PurchaseOrderDate,
                    P.CreditorName AS PurchaseTo,
                    ISNULL(P.Address1, '') + ' ' + ISNULL(P.Address2, '') + ' ' + ISNULL(P.Address3, '') + ' ' + ISNULL(P.Address4, '') AS Address,
                    P.Attention,
                    P.Phone,
                    P.Fax,
                    P.ReferenceNo,
                    P.RequireDate,
                    P.DeliveryTerm,
                    P.Title,
                    P.Ref1,
                    P.Ref2,
                    P.Remark1,
                    P.Remark2,
                    P.DeliverContact,
                    P.DeliverPhone,
                    P.DeliverFax,
                    ISNULL(P.DeliverAddress1, '') + ' ' + ISNULL(P.DeliverAddress2, '') + ' ' + ISNULL(P.DeliverAddress3, '') + ' ' + ISNULL(P.DeliverAddress4, '') AS DeliverAddress,
                    P.TotalAmount,
                    P.NetTotalAmountLocal,
                    P.IsCancelled,
                    P.IsClosed,
                    G.GLAccountCode AS CreditorCompanyCode,
                    G.Description AS CreditorCompanyName,
                    A.AreaCode,
                    A.Description AS AreaDescription,
                    CC.CategoryCode,
                    CC.Description AS CategoryDescription,
                    CCAC.GLAccountCode AS ControlAccountCode,
                    Pu.StaffCode AS PurchaserCode,
                    Pu.Name AS PurchaserName,
                    T.Term AS TermCode,
                    T.Description AS TermDescription,
                    Pr.ProjectCode,
                    Pr.Description AS ProjectDescription,
                    D.DepartmentCode,
                    D.Description AS DepartmentDescription,
                    S.LocationCode,
                    S.Description AS StockLocationDescription,
                    Cu.CurrencyCode,
                    Cu.Description AS CurrencyDescription,
                    Cu.Sign AS CurrencySign,
                    P.CurrencyRate,
                    P.Address1,
                    P.Address2,
                    P.Address3,
                    P.Address4,
                    P.DeliverAddress1,
                    P.DeliverAddress2,
                    P.DeliverAddress3,
                    P.DeliverAddress4,
                    P.WTaxTotalAmountLocal,
                    P.WTaxTotalAmount,
                    P.TaxExclusiveTotalAmount,
                    P.TaxExclusiveTotalAmountLocal,
                    P.IsTaxInclusive,
                    P.TaxTotalAmountLocal,
                    P.TaxTotalAmount,
                    P.NetTotalAmount,
                    P.TransferToInfo
                FROM dbo.PurchaseOrders AS P WITH (NoLock)
                LEFT OUTER JOIN dbo.Creditors AS C WITH (NoLock) ON P.CreditorId = C.Id
                LEFT OUTER JOIN dbo.GLAccounts AS G WITH (NoLock) ON C.GLAccountId = G.Id
                LEFT OUTER JOIN dbo.Areas AS A WITH (NoLock) ON C.AreaId = A.Id
                LEFT OUTER JOIN dbo.CreditorCategory AS CC WITH (NoLock) ON C.CategoryId = CC.Id
                LEFT OUTER JOIN (
                    SELECT Id, GLAccountCode
                    FROM dbo.GLAccounts
                    WHERE (SpecialAccountId = 6)
                ) AS CCAC ON G.ParentId = CCAC.Id
                LEFT OUTER JOIN dbo.Purchasers AS Pu WITH (NoLock) ON P.PurchaserId = Pu.Id
                LEFT OUTER JOIN dbo.Terms AS T WITH (NoLock) ON P.TermId = T.Id
                LEFT OUTER JOIN dbo.Projects AS Pr WITH (NoLock) ON P.ProjectId = Pr.Id
                LEFT OUTER JOIN dbo.Departments AS D WITH (NoLock) ON P.DepartmentId = D.Id
                LEFT OUTER JOIN dbo.StockLocations AS S WITH (NoLock) ON P.StockLocationId = S.Id
                LEFT OUTER JOIN dbo.Currencies AS Cu WITH (NoLock) ON P.CurrencyId = Cu.Id
                ORDER BY P.PurchaseOrderDate DESC
                ";

        var model = await connection.QueryAsync<VW_PurchaseOrdersListing>(query, commandType: CommandType.Text);
        return model;

    }

    // GET PURCHASE ORDERS BY ID (HEADER AUTOFILL DATA)
    public async Task<PurchaseOrdersModel> GetPurchaseOrdersById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM PurchaseOrders WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<PurchaseOrdersModel>(query, new { Id });

        return model;

    }

    // GET ONE PURCHASE ORDER DETAILS DATA BY ID
    public async Task<IEnumerable<PurchaseOrdersDetails>> GetPurchaseOrdersDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        // FIX: Changed LIKE to = for proper GUID comparison
        var query = @"SELECT * FROM PurchaseOrderDetails WHERE PurchaseOrderId = @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<PurchaseOrdersDetails>(query, new { Id });
        return model;

    }

    //GET GetMaxCode SCALA VALUE
    public async Task<string> GetMaxPurchaseOrderCode(string companyCode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT MAX(PurchaseOrderCode) AS PurchaseOrderCode FROM PurchaseOrders";
        var model = await connection.ExecuteScalarAsync<string>(query);
        return model;

    }

    public async Task<string> GetMonthlyPurchaseOrderCode(string companyCode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT
                MAX(PurchaseOrderCode) AS PurchaseOrderCode
                FROM PurchaseOrders
                WHERE PurchaseOrderCode LIKE 'PR2105%'
                ORDER BY PurchaseOrderCode DESC";
        var model = await connection.ExecuteScalarAsync<string>(query);
        return model;

    }


    // SAVE OR UPDATE PURCHASE ORDER HEADER
    public async Task<string> SavePurchaseOrderHeader(PurchaseOrdersModel purchaseOrdersModel)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertdetailquery = @"
                INSERT INTO PurchaseOrders (
                    Id, PurchaseOrderCode, PurchaseOrderDate, CreditorId, CreditorName,
                    Attention, Phone, Fax, ReferenceNo, PurchaserId, TermId, RequireDate,
                    DeliveryTerm, Title, Ref1, Ref2, Remark1, Remark2, DeliverContact,
                    DeliverPhone, DeliverFax, ProjectId, DepartmentId, StockLocationId,
                    CurrencyId, CurrencyRate, TotalAmount, TotalAmountLocal, IsCancelled,
                    IsClosed, CostCentreId, Address1, Address2, Address3, Address4,
                    DeliverAddress1, DeliverAddress2, DeliverAddress3, DeliverAddress4,
                    Discount, DiscountAmount, DiscountAmountLocal, NetTotalAmount,
                    NetTotalAmountLocal, DueDate, TaxTotalAmount, TaxTotalAmountLocal,
                    SubTotalAmount, SubTotalAmountLocal, ImportDeclarationNo, Notes,
                    IsTaxInclusive, TaxDate, TaxExclusiveTotalAmount, TaxExclusiveTotalAmountLocal,
                    IsTaxInclusiveOnly, RoundingAdjustmentAccount, RoundingAdjustment,
                    RoundingAdjustmentLocal, IsRounding, OptimisticLockField, WTaxTotalAmount,
                    WTaxTotalAmountLocal, SgstTotalAmount, SgstTotalAmountLocal, CgstTotalAmount,
                    CgstTotalAmountLocal, IgstTotalAmount, IgstTotalAmountLocal, TransferFromInfo,
                    TransferToInfo, IsApproved, TransferFromSOInfo, ReverseRate, IsNonTaxInvoice,
                    IsPast, IsSelfBearWTax, TotalServiceAmount, TotalServiceAmountLocal,
                    TotalAmountTaxExempted, TotalAmountTaxExemptedLocal, IsEinvoiceReject, DimensionId
                ) VALUES (
                    @Id, @PurchaseOrderCode, @PurchaseOrderDate, @CreditorId, @CreditorName,
                    @Attention, @Phone, @Fax, @ReferenceNo, @PurchaserId, @TermId, @RequireDate,
                    @DeliveryTerm, @Title, @Ref1, @Ref2, @Remark1, @Remark2, @DeliverContact,
                    @DeliverPhone, @DeliverFax, @ProjectId, @DepartmentId, @StockLocationId,
                    @CurrencyId, @CurrencyRate, @TotalAmount, @TotalAmountLocal, @IsCancelled,
                    @IsClosed, @CostCentreId, @Address1, @Address2, @Address3, @Address4,
                    @DeliverAddress1, @DeliverAddress2, @DeliverAddress3, @DeliverAddress4,
                    @Discount, @DiscountAmount, @DiscountAmountLocal, @NetTotalAmount,
                    @NetTotalAmountLocal, @DueDate, @TaxTotalAmount, @TaxTotalAmountLocal,
                    @SubTotalAmount, @SubTotalAmountLocal, @ImportDeclarationNo, @Notes,
                    @IsTaxInclusive, @TaxDate, @TaxExclusiveTotalAmount, @TaxExclusiveTotalAmountLocal,
                    @IsTaxInclusiveOnly, @RoundingAdjustmentAccount, @RoundingAdjustment,
                    @RoundingAdjustmentLocal, @IsRounding, @OptimisticLockField, @WTaxTotalAmount,
                    @WTaxTotalAmountLocal, @SgstTotalAmount, @SgstTotalAmountLocal, @CgstTotalAmount,
                    @CgstTotalAmountLocal, @IgstTotalAmount, @IgstTotalAmountLocal, @TransferFromInfo,
                    @TransferToInfo, @IsApproved, @TransferFromSOInfo, @ReverseRate, @IsNonTaxInvoice,
                    @IsPast, @IsSelfBearWTax, @TotalServiceAmount, @TotalServiceAmountLocal,
                    @TotalAmountTaxExempted, @TotalAmountTaxExemptedLocal, @IsEinvoiceReject, @DimensionId
                )";
        var updatedetailquery = @"
                UPDATE PurchaseOrders SET
                    PurchaseOrderCode = @PurchaseOrderCode, PurchaseOrderDate = @PurchaseOrderDate,
                    CreditorId = @CreditorId, CreditorName = @CreditorName, Attention = @Attention,
                    Phone = @Phone, Fax = @Fax, ReferenceNo = @ReferenceNo, PurchaserId = @PurchaserId,
                    TermId = @TermId, RequireDate = @RequireDate, DeliveryTerm = @DeliveryTerm,
                    Title = @Title, Ref1 = @Ref1, Ref2 = @Ref2, Remark1 = @Remark1, Remark2 = @Remark2,
                    DeliverContact = @DeliverContact, DeliverPhone = @DeliverPhone, DeliverFax = @DeliverFax,
                    ProjectId = @ProjectId, DepartmentId = @DepartmentId, StockLocationId = @StockLocationId,
                    CurrencyId = @CurrencyId, CurrencyRate = @CurrencyRate, TotalAmount = @TotalAmount,
                    TotalAmountLocal = @TotalAmountLocal, IsCancelled = @IsCancelled, IsClosed = @IsClosed,
                    CostCentreId = @CostCentreId, Address1 = @Address1, Address2 = @Address2,
                    Address3 = @Address3, Address4 = @Address4, DeliverAddress1 = @DeliverAddress1,
                    DeliverAddress2 = @DeliverAddress2, DeliverAddress3 = @DeliverAddress3,
                    DeliverAddress4 = @DeliverAddress4, Discount = @Discount, DiscountAmount = @DiscountAmount,
                    DiscountAmountLocal = @DiscountAmountLocal, NetTotalAmount = @NetTotalAmount,
                    NetTotalAmountLocal = @NetTotalAmountLocal, DueDate = @DueDate,
                    TaxTotalAmount = @TaxTotalAmount, TaxTotalAmountLocal = @TaxTotalAmountLocal,
                    SubTotalAmount = @SubTotalAmount, SubTotalAmountLocal = @SubTotalAmountLocal,
                    ImportDeclarationNo = @ImportDeclarationNo, Notes = @Notes, IsTaxInclusive = @IsTaxInclusive,
                    TaxDate = @TaxDate, TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
                    TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal,
                    IsTaxInclusiveOnly = @IsTaxInclusiveOnly, RoundingAdjustmentAccount = @RoundingAdjustmentAccount,
                    RoundingAdjustment = @RoundingAdjustment, RoundingAdjustmentLocal = @RoundingAdjustmentLocal,
                    IsRounding = @IsRounding, OptimisticLockField = OptimisticLockField + 1,
                    WTaxTotalAmount = @WTaxTotalAmount, WTaxTotalAmountLocal = @WTaxTotalAmountLocal,
                    SgstTotalAmount = @SgstTotalAmount, SgstTotalAmountLocal = @SgstTotalAmountLocal,
                    CgstTotalAmount = @CgstTotalAmount, CgstTotalAmountLocal = @CgstTotalAmountLocal,
                    IgstTotalAmount = @IgstTotalAmount, IgstTotalAmountLocal = @IgstTotalAmountLocal,
                    TransferFromInfo = @TransferFromInfo, TransferToInfo = @TransferToInfo,
                    IsApproved = @IsApproved, TransferFromSOInfo = @TransferFromSOInfo,
                    ReverseRate = @ReverseRate, IsNonTaxInvoice = @IsNonTaxInvoice, IsPast = @IsPast,
                    IsSelfBearWTax = @IsSelfBearWTax, TotalServiceAmount = @TotalServiceAmount,
                    TotalServiceAmountLocal = @TotalServiceAmountLocal, TotalAmountTaxExempted = @TotalAmountTaxExempted,
                    TotalAmountTaxExemptedLocal = @TotalAmountTaxExemptedLocal, IsEinvoiceReject = @IsEinvoiceReject,
                    DimensionId = @DimensionId
                    WHERE Id = @Id";

        // Check if header data exists
        var existheader = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM PurchaseOrders WHERE Id = @Id", new { purchaseOrdersModel.Id }, transaction) > 0;
        try
        {
            if (!existheader)
            {
                // Insert header
                var affectedHeaderInsertRows = await connection.ExecuteAsync(insertdetailquery, purchaseOrdersModel, transaction);

                transaction.Commit();
                return $"Success {affectedHeaderInsertRows} added!";
            }
            else
            {
                // Update header
                var affectedHeaderUpdateRows = await connection.ExecuteAsync(updatedetailquery, purchaseOrdersModel, transaction);
                transaction.Commit();
                return $"Success {affectedHeaderUpdateRows} updated!";
            }

        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            if (ex.Message.Contains("Cannot insert duplicate key row in object"))
            {

                if (ex.Message.Contains("PurchaseOrders"))
                {
                    return $"{purchaseOrdersModel.PurchaseOrderCode} already exists. PO No. must be unique.";
                }
                return "Duplicate value detected. Values must be unique.";
            }
            throw;
        }


    }


    // SAVE OR UPDATE PURCHASE ORDER DETAILS
    public async Task<string> SavePurchaseOrderDetails(List<PurchaseOrdersDetails> purchaseOrdersDetails)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        connection.Open(); // Open connection before the transaction begins
        using var transaction = connection.BeginTransaction();

        var insertdetailquery = @"
                                INSERT INTO PurchaseOrderDetails (
                                    Id, PurchaseOrderId, Pos, Numbering, StockId, Description, ProjectId,
                                    DepartmentId, StockLocationId, Qty, UOMId, UnitPrice, Discount, Amount,
                                    RequireDate, SupplierItemCode, ReferenceNo, CancelledQty, FromType,
                                    FromDocId, FromDtlId, IsPartialTransfer, CostCentreId, AmountLocal,
                                    DiscountAmount, DiscountAmountLocal, NetAmount, NetAmountLocal, Note,
                                    ItemTypeCode, IsBundled, IsSubItem, BundledTransactionId, TaxCodeId,
                                    IsTaxInclusive, TaxRate, TaxClass, TaxClassValue, TaxAmount, TaxAmountLocal,
                                    SubAmount, SubAmountLocal, TaxExclusiveAmount, TaxExclusiveAmountLocal,
                                    TaxClassValueRef, TransferLink, Transferred, OptimisticLockField, WTaxCode,
                                    WTaxRate, WTaxAmount, WTaxAmountLocal, SgstAmount, CgstAmount, IgstAmount,
                                    SgstAmountLocal, CgstAmountLocal, IgstAmountLocal, CreditorItemId, Ref,
                                    Ref2, Ref3, Ref4, Ref5, DateRef1, DateRef2, NumRef1, NumRef2,
                                    SOTOPOTransferLink, LocalOneCentsDiff, ServiceAmount,
                                    ServiceAmountLocal, SubcodeId, CommissionGroupId, PackingUOMId, Packing,
                                    PackingQty, PackingUOM1, PackingUOM2, HighestUOM, RefTransferFrom,
                                    RefSOTOPOTransferFrom,
                                    DimensionId, TaxExemptNo, TaxExemptDetails,
                                    MarkupDiscountAmount, MarkupDiscountAmountLocal, AmountTaxExempted,
                                    AmountTaxExemptedLocal, MarkupDiscountPrice
                                ) VALUES (
                                    @Id, @PurchaseOrderId, @Pos, @Numbering, @StockId, @Description, @ProjectId,
                                    @DepartmentId, @StockLocationId, @Qty, @UOMId, @UnitPrice, @Discount, @Amount,
                                    @RequireDate, @SupplierItemCode, @ReferenceNo, @CancelledQty, @FromType,
                                    @FromDocId, @FromDtlId, @IsPartialTransfer, @CostCentreId, @AmountLocal,
                                    @DiscountAmount, @DiscountAmountLocal, @NetAmount, @NetAmountLocal, @Note,
                                    @ItemTypeCode, @IsBundled, @IsSubItem, @BundledTransactionId, @TaxCodeId,
                                    @IsTaxInclusive, @TaxRate, @TaxClass, @TaxClassValue, @TaxAmount, @TaxAmountLocal,
                                    @SubAmount, @SubAmountLocal, @TaxExclusiveAmount, @TaxExclusiveAmountLocal,
                                    @TaxClassValueRef, @TransferLink, @Transferred, @OptimisticLockField, @WTaxCode,
                                    @WTaxRate, @WTaxAmount, @WTaxAmountLocal, @SgstAmount, @CgstAmount, @IgstAmount,
                                    @SgstAmountLocal, @CgstAmountLocal, @IgstAmountLocal, @CreditorItemId, @Ref,
                                    @Ref2, @Ref3, @Ref4, @Ref5, @DateRef1, @DateRef2, @NumRef1, @NumRef2,
                                    @SOTOPOTransferLink, @LocalOneCentsDiff, @ServiceAmount,
                                    @ServiceAmountLocal, @SubcodeId, @CommissionGroupId, @PackingUOMId, @Packing,
                                    @PackingQty, @PackingUOM1, @PackingUOM2, @HighestUOM, @RefTransferFrom,
                                    @RefSOTOPOTransferFrom,
                                    @DimensionId, @TaxExemptNo, @TaxExemptDetails,
                                    @MarkupDiscountAmount, @MarkupDiscountAmountLocal, @AmountTaxExempted,
                                    @AmountTaxExemptedLocal, @MarkupDiscountPrice
                                )";
        var updatedetailquery = @"
                                UPDATE PurchaseOrderDetails SET
                                    PurchaseOrderId = @PurchaseOrderId, Pos = @Pos, Numbering = @Numbering,
                                    StockId = @StockId, Description = @Description, ProjectId = @ProjectId,
                                    DepartmentId = @DepartmentId, StockLocationId = @StockLocationId, Qty = @Qty,
                                    UOMId = @UOMId, UnitPrice = @UnitPrice, Discount = @Discount, Amount = @Amount,
                                    RequireDate = @RequireDate, SupplierItemCode = @SupplierItemCode,
                                    ReferenceNo = @ReferenceNo, CancelledQty = @CancelledQty, FromType = @FromType,
                                    FromDocId = @FromDocId, FromDtlId = @FromDtlId, IsPartialTransfer = @IsPartialTransfer,
                                    CostCentreId = @CostCentreId, AmountLocal = @AmountLocal, DiscountAmount = @DiscountAmount,
                                    DiscountAmountLocal = @DiscountAmountLocal, NetAmount = @NetAmount,
                                    NetAmountLocal = @NetAmountLocal, Note = @Note, ItemTypeCode = @ItemTypeCode,
                                    IsBundled = @IsBundled, IsSubItem = @IsSubItem, BundledTransactionId = @BundledTransactionId,
                                    TaxCodeId = @TaxCodeId, IsTaxInclusive = @IsTaxInclusive, TaxRate = @TaxRate,
                                    TaxClass = @TaxClass, TaxClassValue = @TaxClassValue, TaxAmount = @TaxAmount,
                                    TaxAmountLocal = @TaxAmountLocal, SubAmount = @SubAmount, SubAmountLocal = @SubAmountLocal,
                                    TaxExclusiveAmount = @TaxExclusiveAmount, TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal,
                                    TaxClassValueRef = @TaxClassValueRef, TransferLink = @TransferLink,
                                    Transferred = @Transferred, OptimisticLockField = @OptimisticLockField,
                                    WTaxCode = @WTaxCode, WTaxRate = @WTaxRate, WTaxAmount = @WTaxAmount,
                                    WTaxAmountLocal = @WTaxAmountLocal, SgstAmount = @SgstAmount, CgstAmount = @CgstAmount,
                                    IgstAmount = @IgstAmount, SgstAmountLocal = @SgstAmountLocal,
                                    CgstAmountLocal = @CgstAmountLocal, IgstAmountLocal = @IgstAmountLocal,
                                    CreditorItemId = @CreditorItemId, Ref = @Ref, Ref2 = @Ref2, Ref3 = @Ref3,
                                    Ref4 = @Ref4, Ref5 = @Ref5, DateRef1 = @DateRef1, DateRef2 = @DateRef2,
                                    NumRef1 = @NumRef1, NumRef2 = @NumRef2, SOTOPOTransferLink = @SOTOPOTransferLink,
                                    LocalOneCentsDiff = @LocalOneCentsDiff,
                                    ServiceAmount = @ServiceAmount, ServiceAmountLocal = @ServiceAmountLocal,
                                    SubcodeId = @SubcodeId, CommissionGroupId = @CommissionGroupId,
                                    PackingUOMId = @PackingUOMId, Packing = @Packing, PackingQty = @PackingQty,
                                    PackingUOM1 = @PackingUOM1, PackingUOM2 = @PackingUOM2, HighestUOM = @HighestUOM,
                                    RefTransferFrom = @RefTransferFrom, RefSOTOPOTransferFrom = @RefSOTOPOTransferFrom,
                                    DimensionId = @DimensionId, TaxExemptNo = @TaxExemptNo, TaxExemptDetails = @TaxExemptDetails,
                                    MarkupDiscountAmount = @MarkupDiscountAmount, MarkupDiscountAmountLocal = @MarkupDiscountAmountLocal,
                                    AmountTaxExempted = @AmountTaxExempted, AmountTaxExemptedLocal = @AmountTaxExemptedLocal,
                                    MarkupDiscountPrice = @MarkupDiscountPrice
                                WHERE Id = @Id";

        try
        {   // Recived List of Purchase Order Details in (details)
            // Loops through PurchaseOrdersDetails object in this list
            foreach (var detail in purchaseOrdersDetails)
            {
                // Check if detail exists
                var exists = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1) FROM PurchaseOrderDetails WHERE Id = @Id", new { detail.Id }, transaction) > 0;
                if (!exists)
                {
                    // Insert detail
                    await connection.ExecuteAsync(insertdetailquery, detail, transaction);
                }
                else
                {
                    // Update detail
                    await connection.ExecuteAsync(updatedetailquery, detail, transaction);
                }
            }
            transaction.Commit();
            return $"Success updated!";
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

    }



    //SAVE PURCHASE ORDER WITH TRANSFER (PR → PO if any)
    protected CheckExistModel checkExistModel = new CheckExistModel();
    protected CheckExistModel checkdetailsCheckModel = new CheckExistModel();
    public async Task<string> SavePurchaseOrderWithTransferAsync(PurchaseOrdersModel purchaseOrder, List<PurchaseOrdersDetails> orderDetails, ForTransferHeader forTransferHeader, List<ForTransferDetails> forTransferDetails)
    {
        checkExistModel.Id = purchaseOrder.Id;
        checkExistModel.TableName = "PurchaseOrders";
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        //QUERY FOR HEADER INSERT & UPDATE
        var insertHeaderQuery = @"
                INSERT INTO PurchaseOrders (
                    Id, PurchaseOrderCode, PurchaseOrderDate, CreditorId, CreditorName,
                    Attention, Phone, Fax, ReferenceNo, PurchaserId, TermId, RequireDate,
                    DeliveryTerm, Title, Ref1, Ref2, Remark1, Remark2, DeliverContact,
                    DeliverPhone, DeliverFax, ProjectId, DepartmentId, StockLocationId,
                    CurrencyId, CurrencyRate, TotalAmount, TotalAmountLocal, IsCancelled,
                    IsClosed, CostCentreId, Address1, Address2, Address3, Address4,
                    DeliverAddress1, DeliverAddress2, DeliverAddress3, DeliverAddress4,
                    Discount, DiscountAmount, DiscountAmountLocal, NetTotalAmount,
                    NetTotalAmountLocal, DueDate, TaxTotalAmount, TaxTotalAmountLocal,
                    SubTotalAmount, SubTotalAmountLocal, ImportDeclarationNo, Notes,
                    IsTaxInclusive, TaxDate, TaxExclusiveTotalAmount, TaxExclusiveTotalAmountLocal,
                    IsTaxInclusiveOnly, RoundingAdjustmentAccount, RoundingAdjustment,
                    RoundingAdjustmentLocal, IsRounding, OptimisticLockField, WTaxTotalAmount,
                    WTaxTotalAmountLocal, SgstTotalAmount, SgstTotalAmountLocal, CgstTotalAmount,
                    CgstTotalAmountLocal, IgstTotalAmount, IgstTotalAmountLocal, TransferFromInfo,
                    TransferToInfo, IsApproved, TransferFromSOInfo, ReverseRate, IsNonTaxInvoice,
                    IsPast, IsSelfBearWTax, TotalServiceAmount, TotalServiceAmountLocal,
                    TotalAmountTaxExempted, TotalAmountTaxExemptedLocal, IsEinvoiceReject, DimensionId
                ) VALUES (
                    @Id, @PurchaseOrderCode, @PurchaseOrderDate, @CreditorId, @CreditorName,
                    @Attention, @Phone, @Fax, @ReferenceNo, @PurchaserId, @TermId, @RequireDate,
                    @DeliveryTerm, @Title, @Ref1, @Ref2, @Remark1, @Remark2, @DeliverContact,
                    @DeliverPhone, @DeliverFax, @ProjectId, @DepartmentId, @StockLocationId,
                    @CurrencyId, @CurrencyRate, @TotalAmount, @TotalAmountLocal, @IsCancelled,
                    @IsClosed, @CostCentreId, @Address1, @Address2, @Address3, @Address4,
                    @DeliverAddress1, @DeliverAddress2, @DeliverAddress3, @DeliverAddress4,
                    @Discount, @DiscountAmount, @DiscountAmountLocal, @NetTotalAmount,
                    @NetTotalAmountLocal, @DueDate, @TaxTotalAmount, @TaxTotalAmountLocal,
                    @SubTotalAmount, @SubTotalAmountLocal, @ImportDeclarationNo, @Notes,
                    @IsTaxInclusive, @TaxDate, @TaxExclusiveTotalAmount, @TaxExclusiveTotalAmountLocal,
                    @IsTaxInclusiveOnly, @RoundingAdjustmentAccount, @RoundingAdjustment,
                    @RoundingAdjustmentLocal, @IsRounding, @OptimisticLockField, @WTaxTotalAmount,
                    @WTaxTotalAmountLocal, @SgstTotalAmount, @SgstTotalAmountLocal, @CgstTotalAmount,
                    @CgstTotalAmountLocal, @IgstTotalAmount, @IgstTotalAmountLocal, @TransferFromInfo,
                    @TransferToInfo, @IsApproved, @TransferFromSOInfo, @ReverseRate, @IsNonTaxInvoice,
                    @IsPast, @IsSelfBearWTax, @TotalServiceAmount, @TotalServiceAmountLocal,
                    @TotalAmountTaxExempted, @TotalAmountTaxExemptedLocal, @IsEinvoiceReject, @DimensionId)";
        var updateHeaderQuery = @"
                UPDATE PurchaseOrders SET
                    PurchaseOrderCode = @PurchaseOrderCode, PurchaseOrderDate = @PurchaseOrderDate,
                    CreditorId = @CreditorId, CreditorName = @CreditorName, Attention = @Attention,
                    Phone = @Phone, Fax = @Fax, ReferenceNo = @ReferenceNo, PurchaserId = @PurchaserId,
                    TermId = @TermId, RequireDate = @RequireDate, DeliveryTerm = @DeliveryTerm,
                    Title = @Title, Ref1 = @Ref1, Ref2 = @Ref2, Remark1 = @Remark1, Remark2 = @Remark2,
                    DeliverContact = @DeliverContact, DeliverPhone = @DeliverPhone, DeliverFax = @DeliverFax,
                    ProjectId = @ProjectId, DepartmentId = @DepartmentId, StockLocationId = @StockLocationId,
                    CurrencyId = @CurrencyId, CurrencyRate = @CurrencyRate, TotalAmount = @TotalAmount,
                    TotalAmountLocal = @TotalAmountLocal, IsCancelled = @IsCancelled, IsClosed = @IsClosed,
                    CostCentreId = @CostCentreId, Address1 = @Address1, Address2 = @Address2,
                    Address3 = @Address3, Address4 = @Address4, DeliverAddress1 = @DeliverAddress1,
                    DeliverAddress2 = @DeliverAddress2, DeliverAddress3 = @DeliverAddress3,
                    DeliverAddress4 = @DeliverAddress4, Discount = @Discount, DiscountAmount = @DiscountAmount,
                    DiscountAmountLocal = @DiscountAmountLocal, NetTotalAmount = @NetTotalAmount,
                    NetTotalAmountLocal = @NetTotalAmountLocal, DueDate = @DueDate,
                    TaxTotalAmount = @TaxTotalAmount, TaxTotalAmountLocal = @TaxTotalAmountLocal,
                    SubTotalAmount = @SubTotalAmount, SubTotalAmountLocal = @SubTotalAmountLocal,
                    ImportDeclarationNo = @ImportDeclarationNo, Notes = @Notes, IsTaxInclusive = @IsTaxInclusive,
                    TaxDate = @TaxDate, TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
                    TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal,
                    IsTaxInclusiveOnly = @IsTaxInclusiveOnly, RoundingAdjustmentAccount = @RoundingAdjustmentAccount,
                    RoundingAdjustment = @RoundingAdjustment, RoundingAdjustmentLocal = @RoundingAdjustmentLocal,
                    IsRounding = @IsRounding, OptimisticLockField = OptimisticLockField + 1,
                    WTaxTotalAmount = @WTaxTotalAmount, WTaxTotalAmountLocal = @WTaxTotalAmountLocal,
                    SgstTotalAmount = @SgstTotalAmount, SgstTotalAmountLocal = @SgstTotalAmountLocal,
                    CgstTotalAmount = @CgstTotalAmount, CgstTotalAmountLocal = @CgstTotalAmountLocal,
                    IgstTotalAmount = @IgstTotalAmount, IgstTotalAmountLocal = @IgstTotalAmountLocal,
                    TransferFromInfo = @TransferFromInfo, TransferToInfo = @TransferToInfo,
                    IsApproved = @IsApproved, TransferFromSOInfo = @TransferFromSOInfo,
                    ReverseRate = @ReverseRate, IsNonTaxInvoice = @IsNonTaxInvoice, IsPast = @IsPast,
                    IsSelfBearWTax = @IsSelfBearWTax, TotalServiceAmount = @TotalServiceAmount,
                    TotalServiceAmountLocal = @TotalServiceAmountLocal, TotalAmountTaxExempted = @TotalAmountTaxExempted,
                    TotalAmountTaxExemptedLocal = @TotalAmountTaxExemptedLocal, IsEinvoiceReject = @IsEinvoiceReject,
                    DimensionId = @DimensionId
                    WHERE Id = @Id";

        //QUERY FOR DETAILS INSERT & UPDATE
        var insertDetailQuery = @"
                                INSERT INTO PurchaseOrderDetails (
                                    Id, PurchaseOrderId, Pos, Numbering, StockId, Description, ProjectId,
                                    DepartmentId, StockLocationId, Qty, UOMId, UnitPrice, Discount, Amount,
                                    RequireDate, SupplierItemCode, ReferenceNo, CancelledQty, FromType,
                                    FromDocId, FromDtlId, IsPartialTransfer, CostCentreId, AmountLocal,
                                    DiscountAmount, DiscountAmountLocal, NetAmount, NetAmountLocal, Note,
                                    ItemTypeCode, IsBundled, IsSubItem, BundledTransactionId, TaxCodeId,
                                    IsTaxInclusive, TaxRate, TaxClass, TaxClassValue, TaxAmount, TaxAmountLocal,
                                    SubAmount, SubAmountLocal, TaxExclusiveAmount, TaxExclusiveAmountLocal,
                                    TaxClassValueRef, TransferLink, Transferred, OptimisticLockField, WTaxCode,
                                    WTaxRate, WTaxAmount, WTaxAmountLocal, SgstAmount, CgstAmount, IgstAmount,
                                    SgstAmountLocal, CgstAmountLocal, IgstAmountLocal, CreditorItemId, Ref,
                                    Ref2, Ref3, Ref4, Ref5, DateRef1, DateRef2, NumRef1, NumRef2,
                                    SOTOPOTransferLink, LocalOneCentsDiff, ServiceAmount,
                                    ServiceAmountLocal, SubcodeId, CommissionGroupId, PackingUOMId, Packing,
                                    PackingQty, PackingUOM1, PackingUOM2, HighestUOM, RefTransferFrom,
                                    RefSOTOPOTransferFrom,
                                    DimensionId, TaxExemptNo, TaxExemptDetails,
                                    MarkupDiscountAmount, MarkupDiscountAmountLocal, AmountTaxExempted,
                                    AmountTaxExemptedLocal, MarkupDiscountPrice
                                ) VALUES (
                                    @Id, @PurchaseOrderId, @Pos, @Numbering, @StockId, @Description, @ProjectId,
                                    @DepartmentId, @StockLocationId, @Qty, @UOMId, @UnitPrice, @Discount, @Amount,
                                    @RequireDate, @SupplierItemCode, @ReferenceNo, @CancelledQty, @FromType,
                                    @FromDocId, @FromDtlId, @IsPartialTransfer, @CostCentreId, @AmountLocal,
                                    @DiscountAmount, @DiscountAmountLocal, @NetAmount, @NetAmountLocal, @Note,
                                    @ItemTypeCode, @IsBundled, @IsSubItem, @BundledTransactionId, @TaxCodeId,
                                    @IsTaxInclusive, @TaxRate, @TaxClass, @TaxClassValue, @TaxAmount, @TaxAmountLocal,
                                    @SubAmount, @SubAmountLocal, @TaxExclusiveAmount, @TaxExclusiveAmountLocal,
                                    @TaxClassValueRef, @TransferLink, @Transferred, @OptimisticLockField, @WTaxCode,
                                    @WTaxRate, @WTaxAmount, @WTaxAmountLocal, @SgstAmount, @CgstAmount, @IgstAmount,
                                    @SgstAmountLocal, @CgstAmountLocal, @IgstAmountLocal, @CreditorItemId, @Ref,
                                    @Ref2, @Ref3, @Ref4, @Ref5, @DateRef1, @DateRef2, @NumRef1, @NumRef2,
                                    @SOTOPOTransferLink, @LocalOneCentsDiff, @ServiceAmount,
                                    @ServiceAmountLocal, @SubcodeId, @CommissionGroupId, @PackingUOMId, @Packing,
                                    @PackingQty, @PackingUOM1, @PackingUOM2, @HighestUOM, @RefTransferFrom,
                                    @RefSOTOPOTransferFrom,
                                    @DimensionId, @TaxExemptNo, @TaxExemptDetails,
                                    @MarkupDiscountAmount, @MarkupDiscountAmountLocal, @AmountTaxExempted,
                                    @AmountTaxExemptedLocal, @MarkupDiscountPrice
                                )";
        var updateDetailQuery = @"
                                UPDATE PurchaseOrderDetails SET
                                    PurchaseOrderId = @PurchaseOrderId, Pos = @Pos, Numbering = @Numbering,
                                    StockId = @StockId, Description = @Description, ProjectId = @ProjectId,
                                    DepartmentId = @DepartmentId, StockLocationId = @StockLocationId, Qty = @Qty,
                                    UOMId = @UOMId, UnitPrice = @UnitPrice, Discount = @Discount, Amount = @Amount,
                                    RequireDate = @RequireDate, SupplierItemCode = @SupplierItemCode,
                                    ReferenceNo = @ReferenceNo, CancelledQty = @CancelledQty, FromType = @FromType,
                                    FromDocId = @FromDocId, FromDtlId = @FromDtlId, IsPartialTransfer = @IsPartialTransfer,
                                    CostCentreId = @CostCentreId, AmountLocal = @AmountLocal, DiscountAmount = @DiscountAmount,
                                    DiscountAmountLocal = @DiscountAmountLocal, NetAmount = @NetAmount,
                                    NetAmountLocal = @NetAmountLocal, Note = @Note, ItemTypeCode = @ItemTypeCode,
                                    IsBundled = @IsBundled, IsSubItem = @IsSubItem, BundledTransactionId = @BundledTransactionId,
                                    TaxCodeId = @TaxCodeId, IsTaxInclusive = @IsTaxInclusive, TaxRate = @TaxRate,
                                    TaxClass = @TaxClass, TaxClassValue = @TaxClassValue, TaxAmount = @TaxAmount,
                                    TaxAmountLocal = @TaxAmountLocal, SubAmount = @SubAmount, SubAmountLocal = @SubAmountLocal,
                                    TaxExclusiveAmount = @TaxExclusiveAmount, TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal,
                                    TaxClassValueRef = @TaxClassValueRef, TransferLink = @TransferLink,
                                    Transferred = @Transferred, OptimisticLockField = OptimisticLockField + 1,
                                    WTaxCode = @WTaxCode, WTaxRate = @WTaxRate, WTaxAmount = @WTaxAmount,
                                    WTaxAmountLocal = @WTaxAmountLocal, SgstAmount = @SgstAmount, CgstAmount = @CgstAmount,
                                    IgstAmount = @IgstAmount, SgstAmountLocal = @SgstAmountLocal,
                                    CgstAmountLocal = @CgstAmountLocal, IgstAmountLocal = @IgstAmountLocal,
                                    CreditorItemId = @CreditorItemId, Ref = @Ref, Ref2 = @Ref2, Ref3 = @Ref3,
                                    Ref4 = @Ref4, Ref5 = @Ref5, DateRef1 = @DateRef1, DateRef2 = @DateRef2,
                                    NumRef1 = @NumRef1, NumRef2 = @NumRef2, SOTOPOTransferLink = @SOTOPOTransferLink,
                                    LocalOneCentsDiff = @LocalOneCentsDiff,
                                    ServiceAmount = @ServiceAmount, ServiceAmountLocal = @ServiceAmountLocal,
                                    SubcodeId = @SubcodeId, CommissionGroupId = @CommissionGroupId,
                                    PackingUOMId = @PackingUOMId, Packing = @Packing, PackingQty = @PackingQty,
                                    PackingUOM1 = @PackingUOM1, PackingUOM2 = @PackingUOM2, HighestUOM = @HighestUOM,
                                    RefTransferFrom = @RefTransferFrom, RefSOTOPOTransferFrom = @RefSOTOPOTransferFrom,
                                    DimensionId = @DimensionId, TaxExemptNo = @TaxExemptNo, TaxExemptDetails = @TaxExemptDetails,
                                    MarkupDiscountAmount = @MarkupDiscountAmount, MarkupDiscountAmountLocal = @MarkupDiscountAmountLocal,
                                    AmountTaxExempted = @AmountTaxExempted, AmountTaxExemptedLocal = @AmountTaxExemptedLocal,
                                    MarkupDiscountPrice = @MarkupDiscountPrice
                                WHERE Id = @Id";


        try
        {

            var existheader = await checkExistServices.RecordExistsByIdAsync(checkExistModel);
            if (!existheader)
            {
                await connection.ExecuteAsync(insertHeaderQuery, purchaseOrder, transaction);

                // STEP 3: UPDATE PurchaseRequisition HEADER
                var updatePRHeaderQuery = @"UPDATE PurchaseRequisitions
                SET TransferToInfo = 
                    CASE 
                        WHEN TransferToInfo IS NULL OR TransferToInfo = '' THEN @TransferToInfo
                        ELSE TransferToInfo + ', ' + @TransferToInfo
                    END
                WHERE Id = @PurchaseRequisitionHeaderId";

                //UPDATE PurchaseRequisition HEADER and DETAILS
                await connection.ExecuteAsync(updatePRHeaderQuery, forTransferHeader, transaction);

                // === IF TRANSFER MODE ===
                await purchaseTransferLinkServices.InsertPurchaseTransferLink(forTransferDetails);

            }
            else
            {
                await connection.ExecuteAsync(updateHeaderQuery, purchaseOrder, transaction);
            }

            for (int i = 0; i < orderDetails.Count; i++)
            {
                var detail = orderDetails[i];

              
                checkdetailsCheckModel.Id = detail.Id;
                checkdetailsCheckModel.TableName = "PurchaseOrderDetails";
                var exists = await checkExistServices.RecordExistsByIdAsync(checkdetailsCheckModel);
                if (!exists)
                {
                    if (i < forTransferDetails.Count)
                        detail.TransferLink = forTransferDetails[i].Id; // or .TransferLink kung meron

                    var updatePRDetailsQuery = @"
                UPDATE PurchaseRequisitionDetails
                SET Transferred = @Transferred,
                    TransferLink = @TransferLink, 
                    OptimisticLockField = OptimisticLockField + 1
                WHERE Id = @PurchaseRequisitionDetail";

                    foreach (var prdetail in forTransferDetails)
                    {
                        await connection.ExecuteAsync(updatePRDetailsQuery,
                        new
                        {
                            PurchaseRequisitionDetail = prdetail.PurchaseRequisitionDetailId,
                            Transferred = prdetail.Transferred,
                            TransferLink = prdetail.TransferLink
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
            return "Success: Purchase Order saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }


    public async Task<string> DeletePurchaseOrderDetailRows(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var updatedetailsquery = @"DELETE FROM PurchaseOrderDetails WHERE Id=@Id";
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


