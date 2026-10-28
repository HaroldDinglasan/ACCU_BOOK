using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.CustomerModels;
using Accounting_V4.Model.QNEModels.DescriptionModels;
using Accounting_V4.Model.QNEModels.MiscModels;
using Accounting_V4.Model.QNEModels.ProjectModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Accounting_V4.Model.QNEModels.RunningNumberModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.StocksModels;
using Accounting_V4.Model.QNEModels.TransactionCodesModels;
using Accounting_V4.Pages.QNEPages.GeneralLedger.GLAccount;
using Accounting_V4.Services.OptimisticLockFieldServices;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Accounting_V4.Services.QNEServices.SalesServices;

public class QuotationServices
{
    private readonly QneConnectionString qneConnection;
    public CheckExistServices checkExistServices;

    protected OptimisticLockField optimisticLockField
    {
        get; set;
    }
    public QuotationServices(QneConnectionString qneConnection, 
        OptimisticLockField optimisticLockField, 
        CheckExistServices checkExistServices)
    {
        this.qneConnection = qneConnection;
        this.optimisticLockField = optimisticLockField;
        this.checkExistServices = checkExistServices;
    }

    public async Task<IEnumerable<VW_Quotations>> GetVWQuotationsList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT 
                VWQ.Id,
                CRNCY.CurrencyCode,
                QuotationCode,
                QuotationDate,
                DebtorId,
                DebtorName,
                Address1,
                Address2,
                Address3,
                Address4,
                ReferenceNo,
                TermId,
                SalesPersonId,
                DeliveryTerm,
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
                ProjectId,
                DepartmentId,
                StockLocationId,
                CurrencyId,
                CurrencyRate,
                TotalAmount,
                NetTotalAmountLocal,
                IsCancelled,
                IsClosed,
                DOAddress1,
                DOAddress2,
                DOAddress3,
                DOAddress4,
                CostCentreId,
                Discount,
                DiscountAmount,
                NetTotalAmount,
                DiscountAmountLocal,
                TotalAmountLocal,
                TaxTotalAmount,
                TaxTotalAmountLocal,
                SubtotalAmount,
                SubtotalAmountLocal,
                ExportCountry,
                Notes,
                IsTaxInclusive,
                TaxDate,
                TaxExclusiveTotalAmount,
                TaxExclusiveTotalAmountLocal,
                IsTaxInclusiveOnly,
                DeliveryArea,
                SgstTotalAmount,
                SgstTotalAmountLocal,
                CgstTotalAmount,
                CgstTotalAmountLocal,
                IgstTotalAmount,
                IgstTotalAmountLocal,
                TransferToInfo,
                IsApproved,
                VWQ.ReverseRate,
                SIRef1,
                SIRef2,
                TotalServiceAmount,
                TotalServiceAmountLocal,
                CustomerCode,
                CustomerName,
                CustomerName2,
                Term,
                StaffCode,
                AreaCode,
                CategoryCode,
                ControlAccountCode
                 FROM VW_Quotations AS VWQ
    LEFT OUTER JOIN Currencies AS CRNCY
    ON VWQ.CurrencyId = CRNCY.Id
    ORDER BY QuotationDate DESC";
        var model = await connection.QueryAsync<VW_Quotations>(query);
        return model;
    }

    //GET ONE OF QUOTATIONS DATA
    public async Task<QuotationsModel> GetQuotationsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Quotations WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<QuotationsModel>(query, new { Id });
        return model;

    }

    //GET ONE OF QUOTATIONSDETAILS DATA
    public async Task<IEnumerable<QuotationDetails>> GetQuotationsDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM QuotationDetails WHERE QuotationId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<QuotationDetails>(query, new { Id });
        return model;

    }


    //GET LIST OF LOCATIOLIST FROM db.StockLocations
    public async Task<IEnumerable<StockLocations>> GetLocationLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockLocations WHERE IsActive != 0 ORDER BY LocationCode ASC";
        var model = await connection.QueryAsync<StockLocations>(query, commandType: CommandType.Text);
        return model;


    }


    //GET GetMaxCode SCALA VALUE
    public async Task<string> GetMaxQuotationCodeCode()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT MAX(QuotationCode) AS QuotationCode FROM Quotations";
        //var query = @"";
        var model = await connection.ExecuteScalarAsync<string>(query);
        return model;

    }
    protected CheckExistModel headerCheck = new CheckExistModel();
    protected CheckExistModel detailsCheck = new CheckExistModel();
    public async Task<string> SaveQuotationHeaderAndDetails(QuotationsModel quotationsModel, List<QuotationDetails> quotationsDetailsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var inserheaderquery = @"INSERT INTO Quotations (
    Id, QuotationCode, QuotationDate, DebtorId, DebtorName, Attention, Phone, Fax,
    Address1, Address2, Address3, Address4, ReferenceNo, TermId, SalesPersonId, DeliveryTerm,
    Title, Title2, Ref1, Ref2, Ref3, Ref4, Ref5, Remark1, Remark2, Remark3, Remark4, Remark5,
    ProjectId, DepartmentId, StockLocationId, CurrencyId, CurrencyRate, TotalAmount, NetTotalAmountLocal,
    IsCancelled, Validity, DOBranchCode, DOBranchName, DOContact, DOPhone, DOFax, DOAddress1, DOAddress2,
    DOAddress3, DOAddress4, CostCentreId, Discount, DiscountAmount, NetTotalAmount, DiscountAmountLocal,
    TotalAmountLocal, IsClosed, TaxTotalAmount, TaxTotalAmountLocal, SubtotalAmount, SubtotalAmountLocal,
    ExportCountry, Notes, IsTaxInclusive, TaxDate, TaxExclusiveTotalAmount, TaxExclusiveTotalAmountLocal,
    IsTaxInclusiveOnly, RoundingAdjustmentAccount, RoundingAdjustment, RoundingAdjustmentLocal, IsRounding,
    OptimisticLockField, WTaxTotalAmount, WTaxTotalAmountLocal, DORegistationNo, DOGSTRegNo, DOPhone2, DOEmail,
    DORemark, DeliveryArea, SgstTotalAmount, SgstTotalAmountLocal, CgstTotalAmount, CgstTotalAmountLocal,
    IgstTotalAmount, IgstTotalAmountLocal, TransferToInfo, IsApproved, ReverseRate, SIRef1, SIRef2,
    TotalServiceAmount, TotalServiceAmountLocal, FixedCommissionInfoId, IsNonTaxInvoice, IsPast, BillerCode,
    JomPayRefType1, JomPayRef1, JomPayRefType2, JomPayRef2, UseCreditCard, IsSelfBearWTax, ZipCode, City,
    StateId, CountryId, TinType, CustomerTIN, DimensionId, TotalAmountTaxExempted, TotalAmountTaxExemptedLocal,
    IsEinvoiceReject, FreshsalesDealId
) VALUES (
    @Id, @QuotationCode, @QuotationDate, @DebtorId, @DebtorName, @Attention, @Phone, @Fax,
    @Address1, @Address2, @Address3, @Address4, @ReferenceNo, @TermId, @SalesPersonId, @DeliveryTerm,
    @Title, @Title2, @Ref1, @Ref2, @Ref3, @Ref4, @Ref5, @Remark1, @Remark2, @Remark3, @Remark4, @Remark5,
    @ProjectId, @DepartmentId, @StockLocationId, @CurrencyId, @CurrencyRate, @TotalAmount, @NetTotalAmountLocal,
    @IsCancelled, @Validity, @DOBranchCode, @DOBranchName, @DOContact, @DOPhone, @DOFax, @DOAddress1, @DOAddress2,
    @DOAddress3, @DOAddress4, @CostCentreId, @Discount, @DiscountAmount, @NetTotalAmount, @DiscountAmountLocal,
    @TotalAmountLocal, @IsClosed, @TaxTotalAmount, @TaxTotalAmountLocal, @SubtotalAmount, @SubtotalAmountLocal,
    @ExportCountry, @Notes, @IsTaxInclusive, @TaxDate, @TaxExclusiveTotalAmount, @TaxExclusiveTotalAmountLocal,
    @IsTaxInclusiveOnly, @RoundingAdjustmentAccount, @RoundingAdjustment, @RoundingAdjustmentLocal, @IsRounding,
    @OptimisticLockField, @WTaxTotalAmount, @WTaxTotalAmountLocal, @DORegistationNo, @DOGSTRegNo, @DOPhone2, @DOEmail,
    @DORemark, @DeliveryArea, @SgstTotalAmount, @SgstTotalAmountLocal, @CgstTotalAmount, @CgstTotalAmountLocal,
    @IgstTotalAmount, @IgstTotalAmountLocal, @TransferToInfo, @IsApproved, @ReverseRate, @SIRef1, @SIRef2,
    @TotalServiceAmount, @TotalServiceAmountLocal, @FixedCommissionInfoId, @IsNonTaxInvoice, @IsPast, @BillerCode,
    @JomPayRefType1, @JomPayRef1, @JomPayRefType2, @JomPayRef2, @UseCreditCard, @IsSelfBearWTax, @ZipCode, @City,
    @StateId, @CountryId, @TinType, @CustomerTIN, @DimensionId, @TotalAmountTaxExempted, @TotalAmountTaxExemptedLocal,
    @IsEinvoiceReject, @FreshsalesDealId)";
        var updateheaderquery = @"UPDATE Quotations SET
    QuotationCode=@QuotationCode, QuotationDate=@QuotationDate, DebtorId=@DebtorId, DebtorName=@DebtorName,
    Attention=@Attention, Phone=@Phone, Fax=@Fax, Address1=@Address1, Address2=@Address2, Address3=@Address3,
    Address4=@Address4, ReferenceNo=@ReferenceNo, TermId=@TermId, SalesPersonId=@SalesPersonId, DeliveryTerm=@DeliveryTerm,
    Title=@Title, Title2=@Title2, Ref1=@Ref1, Ref2=@Ref2, Ref3=@Ref3, Ref4=@Ref4, Ref5=@Ref5, Remark1=@Remark1,
    Remark2=@Remark2, Remark3=@Remark3, Remark4=@Remark4, Remark5=@Remark5, ProjectId=@ProjectId, DepartmentId=@DepartmentId,
    StockLocationId=@StockLocationId, CurrencyId=@CurrencyId, CurrencyRate=@CurrencyRate, TotalAmount=@TotalAmount,
    NetTotalAmountLocal=@NetTotalAmountLocal, IsCancelled=@IsCancelled, Validity=@Validity, DOBranchCode=@DOBranchCode,
    DOBranchName=@DOBranchName, DOContact=@DOContact, DOPhone=@DOPhone, DOFax=@DOFax, DOAddress1=@DOAddress1,
    DOAddress2=@DOAddress2, DOAddress3=@DOAddress3, DOAddress4=@DOAddress4, CostCentreId=@CostCentreId, Discount=@Discount,
    DiscountAmount=@DiscountAmount, NetTotalAmount=@NetTotalAmount, DiscountAmountLocal=@DiscountAmountLocal,
    TotalAmountLocal=@TotalAmountLocal, IsClosed=@IsClosed, TaxTotalAmount=@TaxTotalAmount, TaxTotalAmountLocal=@TaxTotalAmountLocal,
    SubtotalAmount=@SubtotalAmount, SubtotalAmountLocal=@SubtotalAmountLocal, ExportCountry=@ExportCountry, Notes=@Notes,
    IsTaxInclusive=@IsTaxInclusive, TaxDate=@TaxDate, TaxExclusiveTotalAmount=@TaxExclusiveTotalAmount,
    TaxExclusiveTotalAmountLocal=@TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly=@IsTaxInclusiveOnly,
    RoundingAdjustmentAccount=@RoundingAdjustmentAccount, RoundingAdjustment=@RoundingAdjustment,
    RoundingAdjustmentLocal=@RoundingAdjustmentLocal, IsRounding=@IsRounding, OptimisticLockField = OptimisticLockField + 1,
    WTaxTotalAmount=@WTaxTotalAmount, WTaxTotalAmountLocal=@WTaxTotalAmountLocal, DORegistationNo=@DORegistationNo,
    DOGSTRegNo=@DOGSTRegNo, DOPhone2=@DOPhone2, DOEmail=@DOEmail, DORemark=@DORemark, DeliveryArea=@DeliveryArea,
    SgstTotalAmount=@SgstTotalAmount, SgstTotalAmountLocal=@SgstTotalAmountLocal, CgstTotalAmount=@CgstTotalAmount,
    CgstTotalAmountLocal=@CgstTotalAmountLocal, IgstTotalAmount=@IgstTotalAmount, IgstTotalAmountLocal=@IgstTotalAmountLocal,
    TransferToInfo=@TransferToInfo, IsApproved=@IsApproved, ReverseRate=@ReverseRate, SIRef1=@SIRef1, SIRef2=@SIRef2,
    TotalServiceAmount=@TotalServiceAmount, TotalServiceAmountLocal=@TotalServiceAmountLocal, FixedCommissionInfoId=@FixedCommissionInfoId,
    IsNonTaxInvoice=@IsNonTaxInvoice, IsPast=@IsPast, BillerCode=@BillerCode, JomPayRefType1=@JomPayRefType1, JomPayRef1=@JomPayRef1,
    JomPayRefType2=@JomPayRefType2, JomPayRef2=@JomPayRef2, UseCreditCard=@UseCreditCard, IsSelfBearWTax=@IsSelfBearWTax,
    ZipCode=@ZipCode, City=@City, StateId=@StateId, CountryId=@CountryId, TinType=@TinType, CustomerTIN=@CustomerTIN,
    DimensionId=@DimensionId, TotalAmountTaxExempted=@TotalAmountTaxExempted, TotalAmountTaxExemptedLocal=@TotalAmountTaxExemptedLocal,
    IsEinvoiceReject=@IsEinvoiceReject, FreshsalesDealId=@FreshsalesDealId
WHERE Id=@Id";
        var insertdetailquery = @"INSERT INTO QuotationDetails (
    Id, Qty, UOMId, UnitPrice, Discount, SalesTax, QuotationId, StockId, Pos, Description, ProjectId, ReferenceNo,
    Amount, DepartmentId, Desc2, Numbering, StockLocationId, CostCentreId, AmountLocal, DiscountAmount, DiscountAmountLocal,
    NetAmount, NetAmountLocal, Note, ItemTypeCode, IsBundled, IsSubItem, BundledTransactionId, TaxCodeId, IsTaxInclusive,
    TaxRate, TaxClass, TaxClassValue, TaxAmount, TaxAmountLocal, SubAmount, SubAmountLocal, TaxExclusiveAmount,
    TaxExclusiveAmountLocal, TaxClassValueRef, Transferred, MSUnitCost, MSReferenceID, OptimisticLockField, WTaxCode,
    WTaxRate, WTaxAmount, WTaxAmountLocal, CancelledQty, SgstAmount, CgstAmount, IgstAmount, SgstAmountLocal,
    CgstAmountLocal, IgstAmountLocal, DebtorItemId, Ref, Ref2, Ref3, Ref4, Ref5, DateRef1, DateRef2, NumRef1, NumRef2,
    TariffCodeId, LocalOneCentsDiff, ServiceAmount, ServiceAmountLocal, SubcodeId, CommissionGroupId, ServiceCost,
    PackingUOMId, Packing, PackingQty, ProgressInvoicingRate, PackingUOM1, PackingUOM2, HighestUOM, DimensionId,
    TaxExemptNo, TaxExemptDetails, MarkupDiscountAmount, MarkupDiscountAmountLocal, AmountTaxExempted, AmountTaxExemptedLocal,
    MarkupDiscountPrice, StoredPoint, StoredPointQty
) VALUES (
    @Id, @Qty, @UOMId, @UnitPrice, @Discount, @SalesTax, @QuotationId, @StockId, @Pos, @Description, @ProjectId, @ReferenceNo,
    @Amount, @DepartmentId, @Desc2, @Numbering, @StockLocationId, @CostCentreId, @AmountLocal, @DiscountAmount, @DiscountAmountLocal,
    @NetAmount, @NetAmountLocal, @Note, @ItemTypeCode, @IsBundled, @IsSubItem, @BundledTransactionId, @TaxCodeId, @IsTaxInclusive,
    @TaxRate, @TaxClass, @TaxClassValue, @TaxAmount, @TaxAmountLocal, @SubAmount, @SubAmountLocal, @TaxExclusiveAmount,
    @TaxExclusiveAmountLocal, @TaxClassValueRef, @Transferred, @MSUnitCost, @MSReferenceID, @OptimisticLockField, @WTaxCode,
    @WTaxRate, @WTaxAmount, @WTaxAmountLocal, @CancelledQty, @SgstAmount, @CgstAmount, @IgstAmount, @SgstAmountLocal,
    @CgstAmountLocal, @IgstAmountLocal, @DebtorItemId, @Ref, @Ref2, @Ref3, @Ref4, @Ref5, @DateRef1, @DateRef2, @NumRef1, @NumRef2,
    @TariffCodeId, @LocalOneCentsDiff, @ServiceAmount, @ServiceAmountLocal, @SubcodeId, @CommissionGroupId, @ServiceCost,
    @PackingUOMId, @Packing, @PackingQty, @ProgressInvoicingRate, @PackingUOM1, @PackingUOM2, @HighestUOM, @DimensionId,
    @TaxExemptNo, @TaxExemptDetails, @MarkupDiscountAmount, @MarkupDiscountAmountLocal, @AmountTaxExempted, @AmountTaxExemptedLocal,
    @MarkupDiscountPrice, @StoredPoint, @StoredPointQty
)
";
        var updatedetailquery = @"UPDATE QuotationDetails SET
    Qty=@Qty, UOMId=@UOMId, UnitPrice=@UnitPrice, Discount=@Discount, SalesTax=@SalesTax, QuotationId=@QuotationId,
    StockId=@StockId, Pos=@Pos, Description=@Description, ProjectId=@ProjectId, ReferenceNo=@ReferenceNo, Amount=@Amount,
    DepartmentId=@DepartmentId, Desc2=@Desc2, Numbering=@Numbering, StockLocationId=@StockLocationId, CostCentreId=@CostCentreId,
    AmountLocal=@AmountLocal, DiscountAmount=@DiscountAmount, DiscountAmountLocal=@DiscountAmountLocal, NetAmount=@NetAmount,
    NetAmountLocal=@NetAmountLocal, Note=@Note, ItemTypeCode=@ItemTypeCode, IsBundled=@IsBundled, IsSubItem=@IsSubItem,
    BundledTransactionId=@BundledTransactionId, TaxCodeId=@TaxCodeId, IsTaxInclusive=@IsTaxInclusive, TaxRate=@TaxRate,
    TaxClass=@TaxClass, TaxClassValue=@TaxClassValue, TaxAmount=@TaxAmount, TaxAmountLocal=@TaxAmountLocal, SubAmount=@SubAmount,
    SubAmountLocal=@SubAmountLocal, TaxExclusiveAmount=@TaxExclusiveAmount, TaxExclusiveAmountLocal=@TaxExclusiveAmountLocal,
    TaxClassValueRef=@TaxClassValueRef, Transferred=@Transferred, MSUnitCost=@MSUnitCost, MSReferenceID=@MSReferenceID,
    OptimisticLockField = OptimisticLockField + 1, WTaxCode=@WTaxCode, WTaxRate=@WTaxRate, WTaxAmount=@WTaxAmount,
    WTaxAmountLocal=@WTaxAmountLocal, CancelledQty=@CancelledQty, SgstAmount=@SgstAmount, CgstAmount=@CgstAmount,
    IgstAmount=@IgstAmount, SgstAmountLocal=@SgstAmountLocal, CgstAmountLocal=@CgstAmountLocal, IgstAmountLocal=@IgstAmountLocal,
    DebtorItemId=@DebtorItemId, Ref=@Ref, Ref2=@Ref2, Ref3=@Ref3, Ref4=@Ref4, Ref5=@Ref5, DateRef1=@DateRef1, DateRef2=@DateRef2,
    NumRef1=@NumRef1, NumRef2=@NumRef2, TariffCodeId=@TariffCodeId, LocalOneCentsDiff=@LocalOneCentsDiff, ServiceAmount=@ServiceAmount,
    ServiceAmountLocal=@ServiceAmountLocal, SubcodeId=@SubcodeId, CommissionGroupId=@CommissionGroupId, ServiceCost=@ServiceCost,
    PackingUOMId=@PackingUOMId, Packing=@Packing, PackingQty=@PackingQty, ProgressInvoicingRate=@ProgressInvoicingRate,
    PackingUOM1=@PackingUOM1, PackingUOM2=@PackingUOM2, HighestUOM=@HighestUOM, DimensionId=@DimensionId, TaxExemptNo=@TaxExemptNo,
    TaxExemptDetails=@TaxExemptDetails, MarkupDiscountAmount=@MarkupDiscountAmount, MarkupDiscountAmountLocal=@MarkupDiscountAmountLocal,
    AmountTaxExempted=@AmountTaxExempted, AmountTaxExemptedLocal=@AmountTaxExemptedLocal, MarkupDiscountPrice=@MarkupDiscountPrice,
    StoredPoint=@StoredPoint, StoredPointQty=@StoredPointQty
WHERE Id=@Id
";

        try
        {
            headerCheck.Id = quotationsModel.Id;
            headerCheck.TableName = "Quotations";
            var existheader = await checkExistServices.RecordExistsByIdAsync(headerCheck);
            if (!existheader)
            {
                await connection.ExecuteAsync(inserheaderquery, quotationsModel, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateheaderquery, quotationsModel, transaction);
            }

            foreach (var quotationsDetails in quotationsDetailsList)
            {
                detailsCheck.Id = quotationsDetails.Id;
                detailsCheck.TableName = "QuotationDetails";
                var existdetails = await checkExistServices.RecordExistsByIdAsync(detailsCheck);
                if (!existdetails)
                {
                    await connection.ExecuteAsync(insertdetailquery, quotationsDetails, transaction);

                }
                else
                {
                    await connection.ExecuteAsync(updatedetailquery, quotationsDetails, transaction);

                }
            }

            transaction.Commit();
            return $"Success: Quotation saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }
 
    public async Task<string> DeleteQuotationDetailRows(string Id)
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






}
