using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllTaxCodeModels;
using Accounting_V4.Model.QNEModels.MiscModels;
using Accounting_V4.Model.QNEModels.ProjectModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.SupplierModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.PurchasesServices;

public class PurchaseRequisitionsService
{

    private readonly QneConnectionString qneConnection;

    public PurchaseRequisitionsService(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }


    public async Task<PurchaseRequisitionModel> GetPurchaseRequisitions(string id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);

        var headerQuery = @"SELECT * 
                        FROM PurchaseRequisitions 
                        WHERE Id = @Id";

        var detailsQuery = @"SELECT * 
                         FROM PurchaseRequisitionDetails 
                         WHERE PurchaseRequisitionId = @Id
                         ORDER BY Pos";

        var header = await connection.QueryFirstOrDefaultAsync<PurchaseRequisitionModel>(
            headerQuery,
            new
            {
                Id = id
            });

        if (header == null)
            return null;

        var details = await connection.QueryAsync<PurchaseRequisitionDetails>(
            detailsQuery,
            new
            {
                Id = id
            });

        header.Details = details.ToList();

        return header;
    }

    public async Task<IEnumerable<Creditors>> GetSupplierLists(string companyCode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Creditors ORDER BY CompanyCode ASC";
        var model = await connection.QueryAsync<Creditors>(query, commandType: CommandType.Text);
        return model;
    }



    //GET LIST OF TERMS FROM db.Terms
    public async Task<IEnumerable<Terms>> GetTermsLists(string companyCode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Terms ORDER BY Term ASC";
        var model = await connection.QueryAsync<Terms>(query, commandType: CommandType.Text);
        return model;

    }





    public async Task<IEnumerable<Projects>> GetProjectsLists(string companyCode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Projects WHERE IsActive != 0 ORDER BY ProjectCode ASC";
        var model = await connection.QueryAsync<Projects>(query, commandType: CommandType.Text);
        return model;
    }



    public async Task<IEnumerable<Purchasers>> GetPurchaserLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Purchasers  WHERE IsActive != 0 ORDER BY StaffCode ASC";
        var model = await connection.QueryAsync<Purchasers>(query, commandType: CommandType.Text);
        return model;
    }



    //GET PurchaseRequisitionsById From db.PurchaseRequisitions
    public async Task<PurchaseRequisitionModel> GetPurchaseRequisitionsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM PurchaseRequisitions WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<PurchaseRequisitionModel>(query, new { Id });
        return model;
    }

    // GET ONE PURCHASE REQUISITION DETAILS DATA BY ID
    public async Task<IEnumerable<PurchaseRequisitionDetails>> GetPurchaseRequisitionsDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM PurchaseRequisitionDetails WHERE PurchaseRequisitionId = @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<PurchaseRequisitionDetails>(query, new { Id });
        return model;
    }




    //Get List Of UOMs by Id
    //public async Task<IEnumerable<UOMs>> GetUOMById(string companyCode, Guid Id)
    //{
    //    var sqlcon = QneConnectionString.ChooseConnection(companyCode);
    //    using (IDbConnection con = new SqlConnection(sqlcon))
    //    {
    //        var query = @"SELECT * FROM UOMs WHERE IsActive = 1 AND StockId = @Id ORDER BY UOMCode ASC";
    //        var model = await connection.QueryAsync<UOMs>(query, new { Id });
    //        return model;
    //    }
    //}

    //DEPRICATED
    //public async Task<IEnumerable<WTaxCodes>> GetWTaxCodes(string companyCode)
    //{
    //    var connectionString = await qneConnection.GetConnectionStringAsync();
    //    var connection = new SqlConnection(connectionString);
    //    var query = @"SELECT * FROM WTaxCodes WHERE IsActive != 0 AND PayableAccount IS NOT NULL ORDER BY Code ASC";
    //        var model = await connection.QueryAsync<WTaxCodes>(query, commandType: CommandType.Text);
    //        return model;

    //}



    public async Task<IEnumerable<VW_PurchaseRequisitionsListing>> GetPurchaseRequisitionList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @" SELECT
                    P.Id,
                    P.PurchaseRequisitionCode,
                    P.PurchaseRequisitionDate,
                    P.CreditorName AS PurchaseTo,
                    ISNULL(P.Address1, N'') + ' ' + ISNULL(P.Address2, N'') + ' ' + ISNULL(P.Address3, N'') + ' ' + ISNULL(P.Address4, N'') AS Address,
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
                    ISNULL(P.DeliverAddress1, N'') + ' ' + ISNULL(P.DeliverAddress2, N'') + ' ' + ISNULL(P.DeliverAddress3, N'') + ' ' + ISNULL(P.DeliverAddress4, N'') AS DeliverAddress,
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
                    SU.UserName AS RequestByUserName,
                    SU.FullName AS RequestByFullName,
                    SU.Signature AS RequestBySignature,
                    P.TaxTotalAmount,
                    P.TaxTotalAmountLocal,
                    P.TaxExclusiveTotalAmount,
                    P.TaxExclusiveTotalAmountLocal,
                    P.TaxDate,
                    P.WTaxTotalAmountLocal,
                    P.WTaxTotalAmount,
                    P.CostCentreId,
                    P.Discount,
                    P.DiscountAmount,
                    P.DiscountAmountLocal,
                    P.NetTotalAmount,
                    P.DueDate,
                    P.SubtotalAmount,
                    P.SubtotalAmountLocal,
                    P.ImportDeclarationNo,
                    P.Notes,
                    P.RequestByUserId,
                    P.IsTaxInclusive,
                    P.IsTaxInclusiveOnly,
                    P.RoundingAdjustmentAccount,
                    P.RoundingAdjustment,
                    P.RoundingAdjustmentLocal,
                    P.IsRounding,
                    P.IsApproved,
                    P.OptimisticLockField,
                    P.SgstTotalAmount,
                    P.SgstTotalAmountLocal,
                    P.CgstTotalAmount,
                    P.CgstTotalAmountLocal,
                    P.IgstTotalAmount,
                    P.IgstTotalAmountLocal,
                    P.TransferToInfo,
                    P.ReverseRate,
                    P.IsNonTaxInvoice,
                    P.TransferFromSOInfo,
                    P.IsPast,
                    P.IsSelfBearWTax,
                    P.Address1,
                    P.Address2,
                    P.Address3,
                    P.Address4,
                    P.DeliverAddress1,
                    P.DeliverAddress2,
                    P.DeliverAddress3,
                    P.DeliverAddress4
                FROM dbo.PurchaseRequisitions AS P WITH (NoLock)
                LEFT OUTER JOIN dbo.Creditors AS C WITH (NoLock) ON P.CreditorId = C.Id
                LEFT OUTER JOIN dbo.GLAccounts AS G WITH (NoLock) ON C.GLAccountId = G.Id
                LEFT OUTER JOIN dbo.Areas AS A WITH (NoLock) ON C.AreaId = A.Id
                LEFT OUTER JOIN dbo.CreditorCategory AS CC WITH (NoLock) ON C.CategoryId = CC.Id
                LEFT OUTER JOIN (
                    SELECT Id, GLAccountCode
                    FROM dbo.GLAccounts
                    WHERE SpecialAccountId = 6
                ) AS CCAC ON G.ParentId = CCAC.Id
                LEFT OUTER JOIN dbo.Purchasers AS Pu WITH (NoLock) ON P.PurchaserId = Pu.Id
                LEFT OUTER JOIN dbo.Terms AS T WITH (NoLock) ON P.TermId = T.Id
                LEFT OUTER JOIN dbo.Projects AS Pr WITH (NoLock) ON P.ProjectId = Pr.Id
                LEFT OUTER JOIN dbo.Departments AS D WITH (NoLock) ON P.DepartmentId = D.Id
                LEFT OUTER JOIN dbo.StockLocations AS S WITH (NoLock) ON P.StockLocationId = S.Id
                LEFT OUTER JOIN dbo.Currencies AS Cu WITH (NoLock) ON P.CurrencyId = Cu.Id
                LEFT OUTER JOIN dbo.SecuritySystemUser AS SU WITH (NoLock) ON P.RequestByUserId = CAST(SU.Oid AS NVARCHAR(100))
                ORDER BY P.PurchaseRequisitionDate DESC
                ";

        var model = await connection.QueryAsync<VW_PurchaseRequisitionsListing>(query, commandType: CommandType.Text);
        return model;

    }



    //GET GetMaxCode SCALA VALUE
    public async Task<string> GetMaxPurchaseRequisitionCode(string companyCode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT MAX(PurchaseRequisitionCode) AS PurchaseRequisitionCode FROM PurchaseRequisitions";
        var model = await connection.ExecuteScalarAsync<string>(query);
        return model;

    }

    public async Task<string> GetMonthlyPurchaseRequisitionCode(string companyCode)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT 
                MAX(PurchaseRequisitionCode) AS PurchaseRequisitionCode
                FROM PurchaseRequisitions
                WHERE PurchaseRequisitionCode LIKE 'PR2105%'
                ORDER BY PurchaseRequisitionCode DESC";
        var model = await connection.ExecuteScalarAsync<string>(query);
        return model;

    }



    // SAVE OR UPDATE PURCHASE REQUISITION HEADER
  
    public async Task<string> SavePurchaseRequisitionHeader(PurchaseRequisitionModel purchaserequisitionModel)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var insertQuery = @"
                    INSERT INTO PurchaseRequisitions (
                        Id, PurchaseRequisitionCode, PurchaseRequisitionDate, CreditorId, CreditorName,
                        Attention, Phone, Fax, ReferenceNo, PurchaserId, TermId, RequireDate, DeliveryTerm,
                        Title, Ref1, Ref2, Remark1, Remark2, DeliverContact, DeliverPhone, DeliverFax, 
                        ProjectId, DepartmentId, StockLocationId, CurrencyId, CurrencyRate, TotalAmount, 
                        TotalAmountLocal, IsCancelled, IsClosed, CostCentreId, Address1, Address2, Address3,
                        Address4, DeliverAddress1, DeliverAddress2, DeliverAddress3, DeliverAddress4, 
                        Discount, DiscountAmount, DiscountAmountLocal, NetTotalAmount, NetTotalAmountLocal, 
                        DueDate, TaxTotalAmount, TaxTotalAmountLocal, SubtotalAmount, SubtotalAmountLocal, 
                        ImportDeclarationNo, Notes, RequestByUserId, IsTaxInclusive, TaxDate, TaxExclusiveTotalAmount,
                        TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly, RoundingAdjustmentAccount, RoundingAdjustment,
                        RoundingAdjustmentLocal, IsRounding, IsApproved, OptimisticLockField, WTaxTotalAmount, 
                        WTaxTotalAmountLocal, SgstTotalAmount, SgstTotalAmountLocal, CgstTotalAmount, CgstTotalAmountLocal,
                        IgstTotalAmount, IgstTotalAmountLocal, TransferToInfo, ReverseRate, IsNonTaxInvoice, 
                        TransferFromSOInfo, IsPast, IsSelfBearWTax  
                    ) VALUES (
                        @Id, @PurchaseRequisitionCode, @PurchaseRequisitionDate, @CreditorId, @CreditorName,
                        @Attention, @Phone, @Fax, @ReferenceNo, @PurchaserId, @TermId, @RequireDate, @DeliveryTerm,
                        @Title, @Ref1, @Ref2, @Remark1, @Remark2, @DeliverContact, @DeliverPhone, @DeliverFax, 
                        @ProjectId, @DepartmentId, @StockLocationId, @CurrencyId, @CurrencyRate, @TotalAmount, 
                        @TotalAmountLocal, @IsCancelled, @IsClosed, @CostCentreId, @Address1, @Address2, @Address3,
                        @Address4, @DeliverAddress1, @DeliverAddress2, @DeliverAddress3, @DeliverAddress4, 
                        @Discount, @DiscountAmount, @DiscountAmountLocal, @NetTotalAmount, @NetTotalAmountLocal, 
                        @DueDate, @TaxTotalAmount, @TaxTotalAmountLocal, @SubtotalAmount, @SubtotalAmountLocal, 
                        @ImportDeclarationNo, @Notes, @RequestByUserId, @IsTaxInclusive, @TaxDate, @TaxExclusiveTotalAmount,
                        @TaxExclusiveTotalAmountLocal, @IsTaxInclusiveOnly, @RoundingAdjustmentAccount, @RoundingAdjustment,
                        @RoundingAdjustmentLocal, @IsRounding, @IsApproved, @OptimisticLockField, @WTaxTotalAmount, 
                        @WTaxTotalAmountLocal, @SgstTotalAmount, @SgstTotalAmountLocal, @CgstTotalAmount, @CgstTotalAmountLocal,
                        @IgstTotalAmount, @IgstTotalAmountLocal, @TransferToInfo, @ReverseRate, @IsNonTaxInvoice, 
                        @TransferFromSOInfo, @IsPast, @IsSelfBearWTax  
                    )";
        var updateQuery = @"
                    UPDATE PurchaseRequisitions SET
                        PurchaseRequisitionCode = @PurchaseRequisitionCode, PurchaseRequisitionDate = @PurchaseRequisitionDate, CreditorId = @CreditorId, CreditorName = @CreditorName,
                        Attention = @Attention, Phone = @Phone, Fax = @Fax, ReferenceNo = @ReferenceNo, PurchaserId = @PurchaserId, TermId = @TermId, RequireDate = @RequireDate, DeliveryTerm = @DeliveryTerm,
                        Title = @Title, Ref1 = @Ref1, Ref2 = @Ref2, Remark1 = @Remark1, Remark2 = @Remark2, DeliverContact = @DeliverContact, DeliverPhone = @DeliverPhone, DeliverFax = @DeliverFax, 
                        ProjectId = @ProjectId, DepartmentId = @DepartmentId, StockLocationId = @StockLocationId, CurrencyId = @CurrencyId, CurrencyRate = @CurrencyRate, TotalAmount = @TotalAmount, 
                        TotalAmountLocal = @TotalAmountLocal, IsCancelled = @IsCancelled, IsClosed = @IsClosed, CostCentreId = @CostCentreId, Address1 = @Address1, Address2 = @Address2, Address3 = @Address3,
                        Address4 = @Address4, DeliverAddress1 = @DeliverAddress1, DeliverAddress2 = @DeliverAddress2, DeliverAddress3 = @DeliverAddress3, DeliverAddress4 = @DeliverAddress4, 
                        Discount = @Discount, DiscountAmount = @DiscountAmount, DiscountAmountLocal = @DiscountAmountLocal, NetTotalAmount = @NetTotalAmount, NetTotalAmountLocal = @NetTotalAmountLocal, 
                        DueDate = @DueDate, TaxTotalAmount = @TaxTotalAmount, TaxTotalAmountLocal = @TaxTotalAmountLocal, SubtotalAmount = @SubtotalAmount, SubtotalAmountLocal = @SubtotalAmountLocal, 
                        ImportDeclarationNo = @ImportDeclarationNo, Notes = @Notes, RequestByUserId = @RequestByUserId, IsTaxInclusive = @IsTaxInclusive, TaxDate = @TaxDate, TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
                        TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly = @IsTaxInclusiveOnly, RoundingAdjustmentAccount = @RoundingAdjustmentAccount, RoundingAdjustment = @RoundingAdjustment,
                        RoundingAdjustmentLocal = @RoundingAdjustmentLocal, IsRounding = @IsRounding, IsApproved = @IsApproved, OptimisticLockField = OptimisticLockField + 1, WTaxTotalAmount = @WTaxTotalAmount, 
                        WTaxTotalAmountLocal = @WTaxTotalAmountLocal, SgstTotalAmount = @SgstTotalAmount, SgstTotalAmountLocal = @SgstTotalAmountLocal, CgstTotalAmount = @CgstTotalAmount, CgstTotalAmountLocal = @CgstTotalAmountLocal,
                        IgstTotalAmount = @IgstTotalAmount, IgstTotalAmountLocal = @IgstTotalAmountLocal, TransferToInfo = @TransferToInfo, ReverseRate = @ReverseRate, IsNonTaxInvoice = @IsNonTaxInvoice, 
                        TransferFromSOInfo = @TransferFromSOInfo, IsPast = @IsPast, IsSelfBearWTax = @IsSelfBearWTax
                    WHERE Id = @Id";

        // Check if header data exists
        var existheader = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM PurchaseRequisitions WHERE Id = @Id", new { purchaserequisitionModel.Id }, transaction) > 0;

        try
        {
            if (!existheader)
            {
                var affectedHeaderInsertRows = await connection.ExecuteAsync(insertQuery, purchaserequisitionModel, transaction);
                transaction.Commit();
                return $"Success {affectedHeaderInsertRows} added!";
            }
            else
            {
                var affectedHeaderUpdateRows = await connection.ExecuteAsync(updateQuery, purchaserequisitionModel, transaction);
                transaction.Commit();
                return $"Success {affectedHeaderUpdateRows} updated!";
            }
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        } 
    }




    // SAVE OR UPDATE PURCHASE REQUISITION DETAILS
    public async Task<string> SavePurchaseRequisitionDetails(List<PurchaseRequisitionDetails> purchaseRequisitionDetails)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        connection.Open();  // Open connection before the transaction begins
        using var transaction = connection.BeginTransaction();

        var insertdetailquery = @"
                                INSERT INTO PurchaseRequisitionDetails (
                                    Id, PurchaseRequisitionId, Pos, Numbering, StockId, Description, ProjectId, DepartmentId, 
                                    StockLocationId, Qty, UOMId, UnitPrice, Discount, Amount, RequireDate, SupplierItemCode, 
                                    ReferenceNo, CancelledQty, CostCentreId, AmountLocal, DiscountAmount, DiscountAmountLocal, 
                                    NetAmount, NetAmountLocal, Note, ItemTypeCode, IsBundled, IsSubItem, BundledTransactionId,
                                    TaxCodeId, IsTaxInclusive, TaxRate, TaxClass, TaxClassValue, TaxAmount, TaxAmountLocal, 
                                    SubAmount, SubAmountLocal, TaxExclusiveAmount, TaxExclusiveAmountLocal, TaxClassValueRef, 
                                    Transferred, OptimisticLockField, WTaxCode, WTaxRate, WTaxAmount, WTaxAmountLocal, 
                                    SgstAmount, CgstAmount, IgstAmount, SgstAmountLocal, CgstAmountLocal, IgstAmountLocal,
                                    CreditorItemId, Ref, Ref2, Ref3, Ref4, Ref5, DateRef1, DateRef2, NumRef1, NumRef2, 
                                    LocalOneCentsDiff, ServiceAmount, ServiceAmountLocal, SubcodeId, CommissionGroupId, 
                                    PackingUOMId, Packing, PackingQty, PackingUOM1, PackingUOM2, HighestUOM, SOTOPOTransferLink, 
                                    TransferLink, RefSOTOPOTransferFrom, RefTransferFrom  
                                ) VALUES (
                                    @Id, @PurchaseRequisitionId, @Pos, @Numbering, @StockId, @Description, @ProjectId, @DepartmentId, 
                                    @StockLocationId, @Qty, @UOMId, @UnitPrice, @Discount, @Amount, @RequireDate, @SupplierItemCode, 
                                    @ReferenceNo, @CancelledQty, @CostCentreId, @AmountLocal, @DiscountAmount, @DiscountAmountLocal, 
                                    @NetAmount, @NetAmountLocal, @Note, @ItemTypeCode, @IsBundled, @IsSubItem, @BundledTransactionId,
                                    @TaxCodeId, @IsTaxInclusive, @TaxRate, @TaxClass, @TaxClassValue, @TaxAmount, @TaxAmountLocal, 
                                    @SubAmount, @SubAmountLocal, @TaxExclusiveAmount, @TaxExclusiveAmountLocal, @TaxClassValueRef, 
                                    @Transferred, @OptimisticLockField, @WTaxCode, @WTaxRate, @WTaxAmount, @WTaxAmountLocal, 
                                    @SgstAmount, @CgstAmount, @IgstAmount, @SgstAmountLocal, @CgstAmountLocal, @IgstAmountLocal,
                                    @CreditorItemId, @Ref, @Ref2, @Ref3, @Ref4, @Ref5, @DateRef1, @DateRef2, @NumRef1, @NumRef2, 
                                    @LocalOneCentsDiff, @ServiceAmount, @ServiceAmountLocal, @SubcodeId, @CommissionGroupId, 
                                    @PackingUOMId, @Packing, @PackingQty, @PackingUOM1, @PackingUOM2, @HighestUOM, @SOTOPOTransferLink, 
                                    @TransferLink, @RefSOTOPOTransferFrom, @RefTransferFrom  
                                )";
        var updatedetailquery = @"
                                UPDATE PurchaseRequisitionDetails SET
                                    PurchaseRequisitionId = @PurchaseRequisitionId, Pos = @Pos, Numbering = @Numbering, StockId = @StockId, Description = @Description, ProjectId = @ProjectId, DepartmentId = @DepartmentId, 
                                    StockLocationId = @StockLocationId, Qty = @Qty, UOMId = @UOMId, UnitPrice = @UnitPrice, Discount = @Discount, Amount = @Amount, RequireDate = @RequireDate, SupplierItemCode = @SupplierItemCode, 
                                    ReferenceNo = @ReferenceNo, CancelledQty = @CancelledQty, CostCentreId = @CostCentreId, AmountLocal = @AmountLocal, DiscountAmount = @DiscountAmount, DiscountAmountLocal = @DiscountAmountLocal, 
                                    NetAmount = @NetAmount, NetAmountLocal = @NetAmountLocal, Note = @Note, ItemTypeCode = @ItemTypeCode, IsBundled = @IsBundled, IsSubItem = @IsSubItem, BundledTransactionId = @BundledTransactionId,
                                    TaxCodeId = @TaxCodeId, IsTaxInclusive = @IsTaxInclusive, TaxRate = @TaxRate, TaxClass = @TaxClass, TaxClassValue = @TaxClassValue, TaxAmount = @TaxAmount, TaxAmountLocal = @TaxAmountLocal, 
                                    SubAmount = @SubAmount, SubAmountLocal = @SubAmountLocal, TaxExclusiveAmount = @TaxExclusiveAmount, TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal, TaxClassValueRef = @TaxClassValueRef, 
                                    Transferred = @Transferred, OptimisticLockField = OptimisticLockField + 1, WTaxCode = @WTaxCode, WTaxRate = @WTaxRate, WTaxAmount = @WTaxAmount, WTaxAmountLocal = @WTaxAmountLocal, 
                                    SgstAmount = @SgstAmount, CgstAmount = @CgstAmount, IgstAmount = @IgstAmount, SgstAmountLocal = @SgstAmountLocal, CgstAmountLocal = @CgstAmountLocal, IgstAmountLocal = @IgstAmountLocal,
                                    CreditorItemId = @CreditorItemId, Ref = @Ref, Ref2 = @Ref2, Ref3 = @Ref3, Ref4 = @Ref4, Ref5 = @Ref5, DateRef1 = @DateRef1, DateRef2 = @DateRef2, NumRef1 = @NumRef1, NumRef2 = @NumRef2, 
                                    LocalOneCentsDiff = @LocalOneCentsDiff, ServiceAmount = @ServiceAmount, ServiceAmountLocal = @ServiceAmountLocal, SubcodeId = @SubcodeId, CommissionGroupId = @CommissionGroupId, 
                                    PackingUOMId = @PackingUOMId, Packing = @Packing, PackingQty = @PackingQty, PackingUOM1 = @PackingUOM1, PackingUOM2 = @PackingUOM2, HighestUOM = @HighestUOM, SOTOPOTransferLink = @SOTOPOTransferLink, 
                                    TransferLink = @TransferLink, RefSOTOPOTransferFrom = @RefSOTOPOTransferFrom, RefTransferFrom  = @RefTransferFrom 
                                WHERE Id = @Id";

        try
        {
            // Recived List of Purchase Requisition Details in (details)
            // Loops through PurchaseRequisitionDetails object in this list
            foreach (var purchaseRequisitionsDetails in purchaseRequisitionDetails)
            {
                // Check if detail exists

                var exists = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1) FROM PurchaseRequisitionDetails WHERE Id = @Id", new { purchaseRequisitionsDetails.Id }, transaction) > 0;
                if (!exists)
                {
                    // Insert Detail
                    await connection.ExecuteAsync(insertdetailquery, purchaseRequisitionsDetails, transaction);
                }
                else
                {
                    // Update Detail
                    await connection.ExecuteAsync(updatedetailquery, purchaseRequisitionsDetails, transaction);
                }
            }
            transaction.Commit();
            return $"Success!";

        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return ex.Message;
            throw;
        }

    }

    public async Task<string> DeletePurchaseRequisitionDetailsRows(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var updatedetailsquery = @"DELETE FROM PurchaseRequisitionDetails WHERE Id=@Id";
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

}
