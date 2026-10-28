using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.DeliveryOrderModels;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Model.QNEModels.InvoiceModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.SupplierModels;
using Accounting_V4.Pages.QNEPages.Supplier.Bill;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.GeneralLedgerServices;
using Accounting_V4.Services.QNEServices.InvoicesServices;
using Accounting_V4.Services.QNEServices.TransferServices;
using Accounting_V4.Services.QNEServices.GLAccountServices;
using Dapper;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Accounting_V4.Services.QNEServices.SalesServices;

public class SalesInvoiceServices
{
    private readonly QneConnectionString qneConnection;
    public SalesTransferLinkServices salesTransferLinkServices;
    public CheckExistServices checkExistServices;
    public GLTransactionsServices glTransactionsServices;
    public InvoiceServices invoiceServices;
    public GLAccountsServices glAccountServices;

    public SalesInvoiceServices(QneConnectionString qneConnection,
        SalesTransferLinkServices salesTransferLinkServices,
        CheckExistServices checkExistServices,
        GLTransactionsServices glTransactionsServices,
        InvoiceServices invoiceServices,
        GLAccountsServices glAccountServices)
    {
        this.qneConnection = qneConnection;
        this.salesTransferLinkServices = salesTransferLinkServices;
        this.checkExistServices = checkExistServices;
        this.glTransactionsServices = glTransactionsServices;
        this.invoiceServices = invoiceServices;
        this.glAccountServices = glAccountServices;
    }


    public async Task<IEnumerable<VW_SalesInvoices>> GetSalesInvoiceLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT 
             VWSI.Id, SalesInvoiceDate, SalesInvoiceCode, DebtorId, DebtorName, Address1,
            CompanyCode, CompanyName, CompanyName2, Term, Currencies.CurrencyCode,
            StaffCode, CurrencyId, CurrencyRate, TaxTotalAmount, TaxTotalAmountLocal,
            TotalAmount, NetTotalAmountLocal, WTaxTotalAmount, WTaxTotalAmountLocal, Attention,
            ReferenceNo, TermId, SalesPersonId, DeliveryTerm, ProjectId,
            DepartmentId, StockLocationId, IsCancelled, Discount,
            DiscountAmount, NetTotalAmount, DiscountAmountLocal, TotalAmountLocal,
            SubtotalAmount, SubtotalAmountLocal, Notes, IsTaxInclusive, TaxDate,
            TaxExclusiveTotalAmount, TaxExclusiveTotalAmountLocal, IsTaxInclusiveOnly, DORegistationNo, TransferFromInfo,
            IsApproved, SIRef1, SIRef2, AreaCode,
            CategoryCode, ControlAccountCode
        FROM VW_SalesInvoices VWSI
        LEFT OUTER JOIN Currencies ON Currencies.Id = VWSI.CurrencyId
        ORDER BY SalesInvoiceDate DESC";
        var model = await connection.QueryAsync<VW_SalesInvoices>(query, commandType: CommandType.Text);
        return model;
    }



    public async Task<SalesInvoicesHeader> GetSalesInvoiceHeaderById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM SalesInvoices WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<SalesInvoicesHeader>(query, new { Id });
        return model;

    }

    public async Task<IEnumerable<SalesInvoiceDetails>> GetSalesInvoiceDetailsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM SalesInvoiceDetails WHERE SalesInvoiceId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<SalesInvoiceDetails>(query, new { Id });
        return model;

    }


    protected CheckExistModel checkExistModel = new CheckExistModel();
    protected CheckExistModel sidetailsCheckModel = new CheckExistModel();
    public async Task<string> SaveSalesInvoiceWithTransferAsync(SalesInvoicesHeader salesInvoicesHeader, List<SalesInvoiceDetails> salesInvoiceDetails, ForTransferHeader forTransferHeader, List<ForTransferDetails> forTransferDetails)
    {
        checkExistModel.Id = salesInvoicesHeader.Id;
        checkExistModel.TableName = "SalesInvoices";

        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var insertHeaderQuery = @"
INSERT INTO SalesInvoices (
    Id,
    SalesInvoiceCode,
    SalesInvoiceDate,
    DebtorId,
    DebtorName,
    Attention,
    Phone,
    Fax,
    Address1,
    Address2,
    Address3,
    Address4,
    ReferenceNo,
    OurDONO,
    TermId,
    DueDate,
    SalesPersonId,
    DeliveryTerm,
    Title,
    Title2,
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
    DOBranchCode,
    DOBranchName,
    DOContact,
    DOPhone,
    DOFax,
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
    IsReliefTax,
    IsTaxInclusive,
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
    DORegistationNo,
    DOGSTRegNo,
    DOPhone2,
    DOEmail,
    DORemark,
    DeliveryArea,
    SgstTotalAmount,
    SgstTotalAmountLocal,
    CgstTotalAmount,
    CgstTotalAmountLocal,
    IgstTotalAmount,
    IgstTotalAmountLocal,
    TransferFromInfo,
    IsApproved,
    IsDeferredVAT,
    IsDifferentPOS,
    PlaceOfSupplyId,
    ReverseCharge,
    IsPaidUnderPA,
    EGSTIN,
    IsAmendedInvoice,
    ReverseRate,
    SIRef1,
    SIRef2,
    TotalServiceAmount,
    TotalServiceAmountLocal,
    FixedCommissionInfoId,
    IsNonTaxInvoice,
    PostingDate,
    TotalPaid,
    AdjustmentAmount,
    TotalMatchedAmount,
    GlobalProgressInvoicingRate,
    PackingListId,
    BillerCode,
    JomPayRefType1,
    JomPayRef1,
    JomPayRefType2,
    JomPayRef2,
    UseCreditCard,
    IsSSTBadDebt,
    WVatTotalAmount,
    WVatTotalAmountLocal,
    IsESales,
    IsSelfBearWTax,
    IsDetailPosting,
    ZipCode,
    City,
    StateId,
    CountryId,
    TinType,
    CustomerTIN,
    DimensionId,
    TotalAmountTaxExempted,
    TotalAmountTaxExemptedLocal,
    IsEinvoiceReject,
    CorrectionCode,
    EISId,
    PostingAccountGroupId,
    SiteGiantOrderId,
    DebtorRegistrationNo,
    DebtorTINType,
    DebtorTIN,
    CustomsReferenceNo,
    FTAInformation,
    CustomsFormNo2,
    Incoterms,
    ShippingCountryId,
    OtherCharges,
    CertifiedExporterNo,
    EInvoiceStatus,
    SubmittedById,
    SubmissionDate,
    ValidatedOn,
    BillingFrom,
    BillingTo,
    CancelledById,
    CancellationDate,
    CancellationReason,
    UniqueIdentifiedNo,
    TaxExemptionNo,
    TaxExemptionDetails,
    ConsolidatedEInvoiceId,
    CustomerCountryId,
    CustomerCity,
    CustomerCountryStateId,
    CustomerZipCode,
    CustomerSalesTaxRegNo,
    CustomerServiceTaxRegNo
) VALUES (
    @Id,
    @SalesInvoiceCode,
    @SalesInvoiceDate,
    @DebtorId,
    @DebtorName,
    @Attention,
    @Phone,
    @Fax,
    @Address1,
    @Address2,
    @Address3,
    @Address4,
    @ReferenceNo,
    @OurDONO,
    @TermId,
    @DueDate,
    @SalesPersonId,
    @DeliveryTerm,
    @Title,
    @Title2,
    @Ref1,
    @Ref2,
    @Ref3,
    @Ref4,
    @Ref5,
    @Remark1,
    @Remark2,
    @Remark3,
    @Remark4,
    @Remark5,
    @ProjectId,
    @DepartmentId,
    @StockLocationId,
    @CurrencyId,
    @CurrencyRate,
    @TotalAmount,
    @NetTotalAmountLocal,
    @IsCancelled,
    @DOBranchCode,
    @DOBranchName,
    @DOContact,
    @DOPhone,
    @DOFax,
    @DOAddress1,
    @DOAddress2,
    @DOAddress3,
    @DOAddress4,
    @CostCentreId,
    @Discount,
    @DiscountAmount,
    @NetTotalAmount,
    @DiscountAmountLocal,
    @TotalAmountLocal,
    @TaxTotalAmount,
    @TaxTotalAmountLocal,
    @SubtotalAmount,
    @SubtotalAmountLocal,
    @ExportCountry,
    @Notes,
    @IsReliefTax,
    @IsTaxInclusive,
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
    @DORegistationNo,
    @DOGSTRegNo,
    @DOPhone2,
    @DOEmail,
    @DORemark,
    @DeliveryArea,
    @SgstTotalAmount,
    @SgstTotalAmountLocal,
    @CgstTotalAmount,
    @CgstTotalAmountLocal,
    @IgstTotalAmount,
    @IgstTotalAmountLocal,
    @TransferFromInfo,
    @IsApproved,
    @IsDeferredVAT,
    @IsDifferentPOS,
    @PlaceOfSupplyId,
    @ReverseCharge,
    @IsPaidUnderPA,
    @EGSTIN,
    @IsAmendedInvoice,
    @ReverseRate,
    @SIRef1,
    @SIRef2,
    @TotalServiceAmount,
    @TotalServiceAmountLocal,
    @FixedCommissionInfoId,
    @IsNonTaxInvoice,
    @PostingDate,
    @TotalPaid,
    @AdjustmentAmount,
    @TotalMatchedAmount,
    @GlobalProgressInvoicingRate,
    @PackingListId,
    @BillerCode,
    @JomPayRefType1,
    @JomPayRef1,
    @JomPayRefType2,
    @JomPayRef2,
    @UseCreditCard,
    @IsSSTBadDebt,
    @WVatTotalAmount,
    @WVatTotalAmountLocal,
    @IsESales,
    @IsSelfBearWTax,
    @IsDetailPosting,
    @ZipCode,
    @City,
    @StateId,
    @CountryId,
    @TinType,
    @CustomerTIN,
    @DimensionId,
    @TotalAmountTaxExempted,
    @TotalAmountTaxExemptedLocal,
    @IsEinvoiceReject,
    @CorrectionCode,
    @EISId,
    @PostingAccountGroupId,
    @SiteGiantOrderId,
    @DebtorRegistrationNo,
    @DebtorTINType,
    @DebtorTIN,
    @CustomsReferenceNo,
    @FTAInformation,
    @CustomsFormNo2,
    @Incoterms,
    @ShippingCountryId,
    @OtherCharges,
    @CertifiedExporterNo,
    @EInvoiceStatus,
    @SubmittedById,
    @SubmissionDate,
    @ValidatedOn,
    @BillingFrom,
    @BillingTo,
    @CancelledById,
    @CancellationDate,
    @CancellationReason,
    @UniqueIdentifiedNo,
    @TaxExemptionNo,
    @TaxExemptionDetails,
    @ConsolidatedEInvoiceId,
    @CustomerCountryId,
    @CustomerCity,
    @CustomerCountryStateId,
    @CustomerZipCode,
    @CustomerSalesTaxRegNo,
    @CustomerServiceTaxRegNo
);";
        var updateHeaderQuery = @"
UPDATE SalesInvoices SET 
    SalesInvoiceCode = @SalesInvoiceCode,
    SalesInvoiceDate = @SalesInvoiceDate,
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
    OurDONO = @OurDONO,
    TermId = @TermId,
    DueDate = @DueDate,
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
    IsReliefTax = @IsReliefTax,
    IsTaxInclusive = @IsTaxInclusive,
    TaxDate = @TaxDate,
    TaxExclusiveTotalAmount = @TaxExclusiveTotalAmount,
    TaxExclusiveTotalAmountLocal = @TaxExclusiveTotalAmountLocal,
    IsTaxInclusiveOnly = @IsTaxInclusiveOnly,
    RoundingAdjustmentAccount = @RoundingAdjustmentAccount,
    RoundingAdjustment = @RoundingAdjustment,
    RoundingAdjustmentLocal = @RoundingAdjustmentLocal,
    IsRounding = @IsRounding,
    OptimisticLockField = OptimisticLockField + 1,
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
    IsApproved = @IsApproved,
    IsDeferredVAT = @IsDeferredVAT,
    IsDifferentPOS = @IsDifferentPOS,
    PlaceOfSupplyId = @PlaceOfSupplyId,
    ReverseCharge = @ReverseCharge,
    IsPaidUnderPA = @IsPaidUnderPA,
    EGSTIN = @EGSTIN,
    IsAmendedInvoice = @IsAmendedInvoice,
    ReverseRate = @ReverseRate,
    SIRef1 = @SIRef1,
    SIRef2 = @SIRef2,
    TotalServiceAmount = @TotalServiceAmount,
    TotalServiceAmountLocal = @TotalServiceAmountLocal,
    FixedCommissionInfoId = @FixedCommissionInfoId,
    IsNonTaxInvoice = @IsNonTaxInvoice,
    PostingDate = @PostingDate,
    TotalPaid = @TotalPaid,
    AdjustmentAmount = @AdjustmentAmount,
    TotalMatchedAmount = @TotalMatchedAmount,
    GlobalProgressInvoicingRate = @GlobalProgressInvoicingRate,
    PackingListId = @PackingListId,
    BillerCode = @BillerCode,
    JomPayRefType1 = @JomPayRefType1,
    JomPayRef1 = @JomPayRef1,
    JomPayRefType2 = @JomPayRefType2,
    JomPayRef2 = @JomPayRef2,
    UseCreditCard = @UseCreditCard,
    IsSSTBadDebt = @IsSSTBadDebt,
    WVatTotalAmount = @WVatTotalAmount,
    WVatTotalAmountLocal = @WVatTotalAmountLocal,
    IsESales = @IsESales,
    IsSelfBearWTax = @IsSelfBearWTax,
    IsDetailPosting = @IsDetailPosting,
    ZipCode = @ZipCode,
    City = @City,
    StateId = @StateId,
    CountryId = @CountryId,
    TinType = @TinType,
    CustomerTIN = @CustomerTIN,
    DimensionId = @DimensionId,
    TotalAmountTaxExempted = @TotalAmountTaxExempted,
    TotalAmountTaxExemptedLocal = @TotalAmountTaxExemptedLocal,
    IsEinvoiceReject = @IsEinvoiceReject,
    CorrectionCode = @CorrectionCode,
    EISId = @EISId,
    PostingAccountGroupId = @PostingAccountGroupId,
    SiteGiantOrderId = @SiteGiantOrderId,
    DebtorRegistrationNo = @DebtorRegistrationNo,
    DebtorTINType = @DebtorTINType,
    DebtorTIN = @DebtorTIN,
    CustomsReferenceNo = @CustomsReferenceNo,
    FTAInformation = @FTAInformation,
    CustomsFormNo2 = @CustomsFormNo2,
    Incoterms = @Incoterms,
    ShippingCountryId = @ShippingCountryId,
    OtherCharges = @OtherCharges,
    CertifiedExporterNo = @CertifiedExporterNo,
    EInvoiceStatus = @EInvoiceStatus,
    SubmittedById = @SubmittedById,
    SubmissionDate = @SubmissionDate,
    ValidatedOn = @ValidatedOn,
    BillingFrom = @BillingFrom,
    BillingTo = @BillingTo,
    CancelledById = @CancelledById,
    CancellationDate = @CancellationDate,
    CancellationReason = @CancellationReason,
    UniqueIdentifiedNo = @UniqueIdentifiedNo,
    TaxExemptionNo = @TaxExemptionNo,
    TaxExemptionDetails = @TaxExemptionDetails,
    ConsolidatedEInvoiceId = @ConsolidatedEInvoiceId,
    CustomerCountryId = @CustomerCountryId,
    CustomerCity = @CustomerCity,
    CustomerCountryStateId = @CustomerCountryStateId,
    CustomerZipCode = @CustomerZipCode,
    CustomerSalesTaxRegNo = @CustomerSalesTaxRegNo,
    CustomerServiceTaxRegNo = @CustomerServiceTaxRegNo
WHERE Id = @Id;
";
        var insertDetailQuery = @"INSERT INTO SalesInvoiceDetails (
      Id
    , Qty
    , UOMId
    , UnitPrice
    , Discount
    , SalesTax
    , StockLocationId
    , FromType
    , FromDocId
    , FromDtlId
    , IsPartialTransfer
    , StockTransactionId
    , SalesInvoiceId
    , StockId
    , Pos
    , GLAccountId
    , Description
    , ProjectId
    , ReferenceNo
    , Amount
    , DepartmentId
    , Numbering
    , CostCentreId
    , AmountLocal
    , DiscountAmount
    , DiscountAmountLocal
    , NetAmount
    , NetAmountLocal
    , Note
    , ItemTypeCode
    , IsBundled
    , IsSubItem
    , BundledTransactionId
    , TaxCodeId
    , IsTaxInclusive
    , TaxRate
    , TaxClass
    , TaxClassValue
    , TaxAmount
    , TaxAmountLocal
    , SubAmount
    , SubAmountLocal
    , TaxExclusiveAmount
    , TaxExclusiveAmountLocal
    , TaxClassValueRef
    , TransferLink
    , StockType
    , MSUnitCost
    , MSReferenceID
    , OptimisticLockField
    , WTaxCode
    , WTaxRate
    , WTaxAmount
    , WTaxAmountLocal
    , SgstAmount
    , CgstAmount
    , IgstAmount
    , SgstAmountLocal
    , CgstAmountLocal
    , IgstAmountLocal
    , DebtorItemId
    , StockBatchNumberId
    , Ref
    , Ref2
    , Ref3
    , Ref4
    , Ref5
    , DateRef1
    , DateRef2
    , NumRef1
    , NumRef2
    , ServiceCost
    , SerialNumber
    , SerialNumberRef1
    , SerialNumberRef2
    , TariffCodeId
    , LocalOneCentsDiff
    , ServiceAmount
    , ServiceAmountLocal
    , SubcodeId
    , CommissionGroupId
    , PackingUOMId
    , Packing
    , PackingQty
    , PackingUOM1
    , PackingUOM2
    , HighestUOM
    , ProgressInvoicingRate
    , WVatCode
    , WVatRate
    , WVatAmount
    , WVatAmountLocal
    , ATCCodeId
    , RefTransferFrom
    , PriorProgressInvoicingRate
    , DimensionId
    , TaxExemptNo
    , TaxExemptDetails
    , MarkupDiscountAmount
    , MarkupDiscountAmountLocal
    , AmountTaxExempted
    , AmountTaxExemptedLocal
    , MarkupDiscountPrice
    , StoredPoint
    , StoredPointQty
    , Classification
) VALUES (
      @Id
    , @Qty
    , @UOMId
    , @UnitPrice
    , @Discount
    , @SalesTax
    , @StockLocationId
    , @FromType
    , @FromDocId
    , @FromDtlId
    , @IsPartialTransfer
    , @StockTransactionId
    , @SalesInvoiceId
    , @StockId
    , @Pos
    , @GLAccountId
    , @Description
    , @ProjectId
    , @ReferenceNo
    , @Amount
    , @DepartmentId
    , @Numbering
    , @CostCentreId
    , @AmountLocal
    , @DiscountAmount
    , @DiscountAmountLocal
    , @NetAmount
    , @NetAmountLocal
    , @Note
    , @ItemTypeCode
    , @IsBundled
    , @IsSubItem
    , @BundledTransactionId
    , @TaxCodeId
    , @IsTaxInclusive
    , @TaxRate
    , @TaxClass
    , @TaxClassValue
    , @TaxAmount
    , @TaxAmountLocal
    , @SubAmount
    , @SubAmountLocal
    , @TaxExclusiveAmount
    , @TaxExclusiveAmountLocal
    , @TaxClassValueRef
    , @TransferLink
    , @StockType
    , @MSUnitCost
    , @MSReferenceID
    , @OptimisticLockField
    , @WTaxCode
    , @WTaxRate
    , @WTaxAmount
    , @WTaxAmountLocal
    , @SgstAmount
    , @CgstAmount
    , @IgstAmount
    , @SgstAmountLocal
    , @CgstAmountLocal
    , @IgstAmountLocal
    , @DebtorItemId
    , @StockBatchNumberId
    , @Ref
    , @Ref2
    , @Ref3
    , @Ref4
    , @Ref5
    , @DateRef1
    , @DateRef2
    , @NumRef1
    , @NumRef2
    , @ServiceCost
    , @SerialNumber
    , @SerialNumberRef1
    , @SerialNumberRef2
    , @TariffCodeId
    , @LocalOneCentsDiff
    , @ServiceAmount
    , @ServiceAmountLocal
    , @SubcodeId
    , @CommissionGroupId
    , @PackingUOMId
    , @Packing
    , @PackingQty
    , @PackingUOM1
    , @PackingUOM2
    , @HighestUOM
    , @ProgressInvoicingRate
    , @WVatCode
    , @WVatRate
    , @WVatAmount
    , @WVatAmountLocal
    , @ATCCodeId
    , @RefTransferFrom
    , @PriorProgressInvoicingRate
    , @DimensionId
    , @TaxExemptNo
    , @TaxExemptDetails
    , @MarkupDiscountAmount
    , @MarkupDiscountAmountLocal
    , @AmountTaxExempted
    , @AmountTaxExemptedLocal
    , @MarkupDiscountPrice
    , @StoredPoint
    , @StoredPointQty
    , @Classification
);";
        var updateDetailQuery = @"UPDATE SalesInvoiceDetails SET
      Qty = @Qty
    , UOMId = @UOMId
    , UnitPrice = @UnitPrice
    , Discount = @Discount
    , SalesTax = @SalesTax
    , StockLocationId = @StockLocationId
    , FromType = @FromType
    , FromDocId = @FromDocId
    , FromDtlId = @FromDtlId
    , IsPartialTransfer = @IsPartialTransfer
    , StockTransactionId = @StockTransactionId
    , SalesInvoiceId = @SalesInvoiceId
    , StockId = @StockId
    , Pos = @Pos
    , GLAccountId = @GLAccountId
    , Description = @Description
    , ProjectId = @ProjectId
    , ReferenceNo = @ReferenceNo
    , Amount = @Amount
    , DepartmentId = @DepartmentId
    , Numbering = @Numbering
    , CostCentreId = @CostCentreId
    , AmountLocal = @AmountLocal
    , DiscountAmount = @DiscountAmount
    , DiscountAmountLocal = @DiscountAmountLocal
    , NetAmount = @NetAmount
    , NetAmountLocal = @NetAmountLocal
    , Note = @Note
    , ItemTypeCode = @ItemTypeCode
    , IsBundled = @IsBundled
    , IsSubItem = @IsSubItem
    , BundledTransactionId = @BundledTransactionId
    , TaxCodeId = @TaxCodeId
    , IsTaxInclusive = @IsTaxInclusive
    , TaxRate = @TaxRate
    , TaxClass = @TaxClass
    , TaxClassValue = @TaxClassValue
    , TaxAmount = @TaxAmount
    , TaxAmountLocal = @TaxAmountLocal
    , SubAmount = @SubAmount
    , SubAmountLocal = @SubAmountLocal
    , TaxExclusiveAmount = @TaxExclusiveAmount
    , TaxExclusiveAmountLocal = @TaxExclusiveAmountLocal
    , TaxClassValueRef = @TaxClassValueRef
    , TransferLink = @TransferLink
    , StockType = @StockType
    , MSUnitCost = @MSUnitCost
    , MSReferenceID = @MSReferenceID
    , OptimisticLockField = OptimisticLockField + 1
    , WTaxCode = @WTaxCode
    , WTaxRate = @WTaxRate
    , WTaxAmount = @WTaxAmount
    , WTaxAmountLocal = @WTaxAmountLocal
    , SgstAmount = @SgstAmount
    , CgstAmount = @CgstAmount
    , IgstAmount = @IgstAmount
    , SgstAmountLocal = @SgstAmountLocal
    , CgstAmountLocal = @CgstAmountLocal
    , IgstAmountLocal = @IgstAmountLocal
    , DebtorItemId = @DebtorItemId
    , StockBatchNumberId = @StockBatchNumberId
    , Ref = @Ref
    , Ref2 = @Ref2
    , Ref3 = @Ref3
    , Ref4 = @Ref4
    , Ref5 = @Ref5
    , DateRef1 = @DateRef1
    , DateRef2 = @DateRef2
    , NumRef1 = @NumRef1
    , NumRef2 = @NumRef2
    , ServiceCost = @ServiceCost
    , SerialNumber = @SerialNumber
    , SerialNumberRef1 = @SerialNumberRef1
    , SerialNumberRef2 = @SerialNumberRef2
    , TariffCodeId = @TariffCodeId
    , LocalOneCentsDiff = @LocalOneCentsDiff
    , ServiceAmount = @ServiceAmount
    , ServiceAmountLocal = @ServiceAmountLocal
    , SubcodeId = @SubcodeId
    , CommissionGroupId = @CommissionGroupId
    , PackingUOMId = @PackingUOMId
    , Packing = @Packing
    , PackingQty = @PackingQty
    , PackingUOM1 = @PackingUOM1
    , PackingUOM2 = @PackingUOM2
    , HighestUOM = @HighestUOM
    , ProgressInvoicingRate = @ProgressInvoicingRate
    , WVatCode = @WVatCode
    , WVatRate = @WVatRate
    , WVatAmount = @WVatAmount
    , WVatAmountLocal = @WVatAmountLocal
    , ATCCodeId = @ATCCodeId
    , RefTransferFrom = @RefTransferFrom
    , PriorProgressInvoicingRate = @PriorProgressInvoicingRate
    , DimensionId = @DimensionId
    , TaxExemptNo = @TaxExemptNo
    , TaxExemptDetails = @TaxExemptDetails
    , MarkupDiscountAmount = @MarkupDiscountAmount
    , MarkupDiscountAmountLocal = @MarkupDiscountAmountLocal
    , AmountTaxExempted = @AmountTaxExempted
    , AmountTaxExemptedLocal = @AmountTaxExemptedLocal
    , MarkupDiscountPrice = @MarkupDiscountPrice
    , StoredPoint = @StoredPoint
    , StoredPointQty = @StoredPointQty
    , Classification = @Classification
WHERE Id = @Id";

        try
        {
            var existheader = await checkExistServices.RecordExistsByIdAsync(checkExistModel);
            if (!existheader)
            {
                await connection.ExecuteAsync(insertHeaderQuery, salesInvoicesHeader);

                //var updateDOHeaderQuery = @"UPDATE DeliveryOrders
                //SET TransferToInfo = 
                //    CASE 
                //        WHEN TransferToInfo IS NULL OR TransferToInfo = '' THEN @TransferToInfo
                //        ELSE TransferToInfo + ', ' + @TransferToInfo
                //    END
                //WHERE Id = @DeliveryOrdersHeaderId";

                //await connection.ExecuteAsync(updateDOHeaderQuery, forTransferHeader, transaction);

                //// === IF TRANSFER MODE ===
                //await salesTransferLinkServices.InsertSalesTransferLink(forTransferDetails);
            }
            else
            {
                await connection.ExecuteAsync(updateHeaderQuery, salesInvoicesHeader);
            }

            for (int i = 0; i < salesInvoiceDetails.Count; i++)
            {
                var detail = salesInvoiceDetails[i];


                sidetailsCheckModel.Id = detail.Id;
                sidetailsCheckModel.TableName = "SalesInvoiceDetails";
                var exists = await checkExistServices.RecordExistsByIdAsync(sidetailsCheckModel);
                if (!exists)
                {
                    //    if (i < forTransferDetails.Count)
                    //        detail.TransferLink = forTransferDetails[i].Id; // or .TransferLink kung meron

                    //    var updateDODetailsQuery = @"
                    //UPDATE DeliveryOrderDetails
                    //SET Transferred = @Transferred,TransferLink = @TransferLink,
                    //    OptimisticLockField = OptimisticLockField + 1
                    //WHERE Id = @DeliveryOrderDetailId";
                    //    foreach (var qtdetail in forTransferDetails)
                    //    {
                    //        await connection.ExecuteAsync(updateDODetailsQuery,
                    //        new
                    //        {
                    //            DeliveryOrderDetailId = qtdetail.DeliveryOrderDetailId,
                    //            Transferred = qtdetail.Transferred,
                    //            TransferLink = qtdetail.TransferLink
                    //        },
                    //        transaction);
                    //    }
                    await connection.ExecuteAsync(insertDetailQuery, detail);
                }

                else
                {
                    await connection.ExecuteAsync(updateDetailQuery, detail);
                }


            }

            // ===  COMMIT ===
            await SaveIntoInvoices(salesInvoicesHeader, salesInvoiceDetails);
            return "Success: Sales Invoice saved successfully.";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    protected async Task SaveIntoInvoices(SalesInvoicesHeader salesInvoicesHeader, List<SalesInvoiceDetails> salesInvoiceDetails)
    {

        //TRANSFER HEADER
        InvoicesHeader invoiceHeader = new InvoicesHeader();

        ///FOR GLTRANSACTIONS
        invoiceHeader.glaccountTAXSALESACCOUNT = salesInvoicesHeader.glaccountTAXSALESACCOUNT;
        invoiceHeader.glaccountOUPUTVAT = salesInvoicesHeader.glaccountOUPUTVAT;

        invoiceHeader.Id = salesInvoicesHeader.Id;
        invoiceHeader.InvoiceCode = salesInvoicesHeader.SalesInvoiceCode;
        invoiceHeader.InvoiceDate = salesInvoicesHeader.SalesInvoiceDate;
        invoiceHeader.InvoiceTo = salesInvoicesHeader.DebtorName;
        invoiceHeader.DebtorId = salesInvoicesHeader.DebtorId;
        invoiceHeader.ReferenceNo = salesInvoicesHeader.ReferenceNo;
        invoiceHeader.TermId = salesInvoicesHeader.TermId;
        invoiceHeader.DueDate = salesInvoicesHeader.DueDate;
        invoiceHeader.SalesPersonId = salesInvoicesHeader.SalesPersonId;
        invoiceHeader.ProjectId = salesInvoicesHeader.ProjectId;
        invoiceHeader.CurrencyId = salesInvoicesHeader.CurrencyId;
        invoiceHeader.CurrencyRate = salesInvoicesHeader.CurrencyRate;
        invoiceHeader.Description = salesInvoicesHeader.Title;
        invoiceHeader.TotalAmount = salesInvoicesHeader.TotalAmount;
        invoiceHeader.IsCancelled = salesInvoicesHeader.IsCancelled;
        //invoiceHeader.IsDetailForGLPosting
        invoiceHeader.TotalAmountLocal = salesInvoicesHeader.TotalAmountLocal;
        //invoiceHeader.GSTTypeId
        //invoiceHeader.GSTTaxRate
        //invoiceHeader.GSTAmount
        //invoiceHeader.GSTTaxAmount
        //invoiceHeader.GSTTaxAmountLocal
        //invoiceHeader.Balance
        invoiceHeader.DepartmentId = salesInvoicesHeader.DepartmentId;
        invoiceHeader.CostCentreId = salesInvoicesHeader.CostCentreId;
        invoiceHeader.FromDocType = "SINV";
        invoiceHeader.FromDocId = salesInvoicesHeader.Id;
        invoiceHeader.TaxTotalAmount = salesInvoicesHeader.TaxTotalAmount;
        invoiceHeader.TaxTotalAmountLocal = salesInvoicesHeader.TaxTotalAmountLocal;
        invoiceHeader.NetTotalAmount = salesInvoicesHeader.NetTotalAmount;
        invoiceHeader.NetTotalAmountLocal = salesInvoicesHeader.NetTotalAmountLocal;
        invoiceHeader.SubtotalAmount = salesInvoicesHeader.SubtotalAmount;
        invoiceHeader.SubtotalAmountLocal = salesInvoicesHeader.SubtotalAmountLocal;
        //invoiceHeader.ExportCountry
        invoiceHeader.Notes = salesInvoicesHeader.Notes;
        //invoiceHeader.ItemDescriptions
        invoiceHeader.IsTaxInclusive = salesInvoicesHeader.IsTaxInclusive;
        invoiceHeader.ObjectType = 1;
        invoiceHeader.TaxDate = salesInvoicesHeader.TaxDate;
        invoiceHeader.TaxExclusiveTotalAmount = salesInvoicesHeader.TaxExclusiveTotalAmount;
        invoiceHeader.TaxExclusiveTotalAmountLocal = salesInvoicesHeader.TaxExclusiveTotalAmountLocal;
        invoiceHeader.IsTaxInclusiveOnly = salesInvoicesHeader.IsTaxInclusiveOnly;
        invoiceHeader.RoundingAdjustmentAccount = salesInvoicesHeader.RoundingAdjustmentAccount;
        invoiceHeader.RoundingAdjustment = salesInvoicesHeader.RoundingAdjustment;
        invoiceHeader.RoundingAdjustmentLocal = salesInvoicesHeader.RoundingAdjustmentLocal;
        invoiceHeader.IsRounding = salesInvoicesHeader.IsRounding;
        invoiceHeader.OptimisticLockField = salesInvoicesHeader.OptimisticLockField;
        invoiceHeader.WTaxTotalAmount = salesInvoicesHeader.WTaxTotalAmount;
        invoiceHeader.WTaxTotalAmountLocal = salesInvoicesHeader.WTaxTotalAmountLocal;
        invoiceHeader.SgstTotalAmount = salesInvoicesHeader.SgstTotalAmount;
        invoiceHeader.SgstTotalAmountLocal = salesInvoicesHeader.SgstTotalAmountLocal;
        invoiceHeader.CgstTotalAmount = salesInvoicesHeader.CgstTotalAmount;
        invoiceHeader.CgstTotalAmountLocal = salesInvoicesHeader.CgstTotalAmountLocal;
        invoiceHeader.IgstTotalAmount = salesInvoicesHeader.IgstTotalAmount;
        invoiceHeader.IgstTotalAmountLocal = salesInvoicesHeader.IgstTotalAmountLocal;
        //invoiceHeader.GSTReverseDate
        invoiceHeader.IsDeferredVAT = salesInvoicesHeader.IsDeferredVAT;
        // invoiceHeader.DocTaxRate
        // invoiceHeader.DocTaxCodeId
        invoiceHeader.IsDifferentPOS = salesInvoicesHeader.IsDifferentPOS;
        invoiceHeader.PlaceOfSupplyId = salesInvoicesHeader.PlaceOfSupplyId;
        invoiceHeader.ReverseCharge = salesInvoicesHeader.ReverseCharge;
        invoiceHeader.IsPaidUnderPA = salesInvoicesHeader.IsPaidUnderPA;
        invoiceHeader.EGSTIN = salesInvoicesHeader.EGSTIN;
        invoiceHeader.IsAmendedInvoice = salesInvoicesHeader.IsAmendedInvoice;
        //invoiceHeader.IsLinkedWithAmendedInvoice
        //invoiceHeader.RowIndex
        invoiceHeader.ReverseRate = salesInvoicesHeader.ReverseRate;
        invoiceHeader.IsNonTaxInvoice = salesInvoicesHeader.IsNonTaxInvoice;
        invoiceHeader.IsSSTBadDebt = salesInvoicesHeader.IsSSTBadDebt;
        invoiceHeader.PostingDate = salesInvoicesHeader.PostingDate;
        //invoiceHeader.ForecastDueDate
        invoiceHeader.TotalServiceAmount = salesInvoicesHeader.TotalServiceAmount;
        invoiceHeader.TotalServiceAmountLocal = salesInvoicesHeader.TotalServiceAmountLocal;
        invoiceHeader.WVatTotalAmount = salesInvoicesHeader.WVatTotalAmount;
        invoiceHeader.WVatTotalAmountLocal = salesInvoicesHeader.WVatTotalAmountLocal;
        invoiceHeader.TotalPaid = salesInvoicesHeader.TotalPaid;
        invoiceHeader.AdjustmentAmount = salesInvoicesHeader.AdjustmentAmount;
        invoiceHeader.TotalMatchedAmount = salesInvoicesHeader.TotalMatchedAmount;
        //invoiceHeader.IsFinanceChargeInv
        //invoiceHeader.PropertyManagementBillingId
        //invoiceHeader.IsMigratedBadDebtRelief
        //invoiceHeader.MigratedReliefAmount
        invoiceHeader.IsESales = salesInvoicesHeader.IsESales;
        invoiceHeader.IsSelfBearWTax = salesInvoicesHeader.IsSelfBearWTax;
        invoiceHeader.IsDetailPosting = salesInvoicesHeader.IsDetailPosting;
        //invoiceHeader.ExcludeFromKnockoff
        invoiceHeader.IsEinvoiceReject = salesInvoicesHeader.IsEinvoiceReject;
        invoiceHeader.DimensionId = salesInvoicesHeader.DimensionId;
        invoiceHeader.CorrectionCode = salesInvoicesHeader.CorrectionCode;
        invoiceHeader.EISId = salesInvoicesHeader.EISId;


        List<InvoiceDetails> invoiceDetails = new();


        //TRANSFER DETAILS
        var detailRows = salesInvoiceDetails.OrderBy(x => x.Pos).ToList();

        bool IsSubtotal(dynamic d) =>
            ((string)d.Description ?? "").Trim().Equals("SUBTOTAL", StringComparison.OrdinalIgnoreCase);

        bool IsDiscount(dynamic d) =>
            ((string)d.Description ?? "").Trim().ToUpper().Contains("DISCOUNT");

        var itemRows = detailRows.Where(d => !IsSubtotal(d) && !IsDiscount(d)).ToList();
        var discountRows = detailRows.Where(d => !IsSubtotal(d) && IsDiscount(d)).ToList();

        // Group lang ang may laman: item row at/o discount row
        var groups = new[]
       {
        (Rows: discountRows, IsDiscount: true),
        (Rows: itemRows, IsDiscount: false)
    }
       .Where(g => g.Rows.Any())
       .ToList();

        invoiceDetails = groups
            .Select((grp, index) =>
            {
                var g = grp.Rows;
                var first = g.First();
                bool isDisc = grp.IsDiscount;

                return new InvoiceDetails
                {
                    InvoiceId = salesInvoicesHeader.Id,
                    //Pos = index + 1,
                    GLAccountId = first.GLAccountId,

                    //Description = string.Join(", ",
                    //    g.Select(x => x.Description?.Trim())
                    //     .Where(x => !string.IsNullOrWhiteSpace(x))
                    //     .Distinct()
                    //     .OrderBy(x => x, StringComparer.Ordinal)),

                    Description = string.Join(", ",
    g.Select(x => x.Description?.Trim())
     .Where(x => !string.IsNullOrWhiteSpace(x))
     .Distinct()),

                    ReferenceNo = first.ReferenceNo,
                    ProjectId = first.ProjectId,
                    DepartmentId = first.DepartmentId,
                    CostCentreId = first.CostCentreId,
                    Note = first.Note,
                    ItemTypeCode = first.ItemTypeCode,
                    TaxCodeId = first.TaxCodeId,
                    IsTaxInclusive = first.IsTaxInclusive,
                    TaxRate = first.TaxRate,
                    TaxClass = first.TaxClass,
                    TaxClassValue = first.TaxClassValue,
                    TaxClassValueRef = first.TaxClassValueRef,
                    OptimisticLockField = first.OptimisticLockField,
                    WTaxCode = first.WTaxCode,
                    WTaxRate = first.WTaxRate,
                    WVatCode = first.WVatCode,
                    WVatRate = first.WVatRate,
                    TariffCodeId = first.TariffCodeId,
                    ATCCodeId = first.ATCCodeId,
                    DimensionId = first.DimensionId,

                    Amount = isDisc ? 0 : g.Sum(x => x.Amount),
                    AmountLocal = isDisc ? 0 : g.Sum(x => x.AmountLocal),
                    TaxAmount = isDisc ? 0 : g.Sum(x => x.TaxAmount),
                    TaxAmountLocal = isDisc ? 0 : g.Sum(x => x.TaxAmountLocal),
                    NetAmount = isDisc ? 0 : g.Sum(x => x.NetAmount),
                    NetAmountLocal = isDisc ? 0 : g.Sum(x => x.NetAmountLocal),
                    SubAmount = isDisc ? 0 : g.Sum(x => x.SubAmount),
                    SubAmountLocal = isDisc ? 0 : g.Sum(x => x.SubAmountLocal),
                    TaxExclusiveAmount = isDisc ? 0 : g.Sum(x => x.TaxExclusiveAmount),
                    TaxExclusiveAmountLocal = isDisc ? 0 : g.Sum(x => x.TaxExclusiveAmountLocal),
                    WTaxAmount = isDisc ? 0 : g.Sum(x => x.WTaxAmount),
                    WTaxAmountLocal = isDisc ? 0 : g.Sum(x => x.WTaxAmountLocal),
                    SgstAmount = isDisc ? 0 : g.Sum(x => x.SgstAmount),
                    CgstAmount = isDisc ? 0 : g.Sum(x => x.CgstAmount),
                    IgstAmount = isDisc ? 0 : g.Sum(x => x.IgstAmount),
                    SgstAmountLocal = isDisc ? 0 : g.Sum(x => x.SgstAmountLocal),
                    CgstAmountLocal = isDisc ? 0 : g.Sum(x => x.CgstAmountLocal),
                    IgstAmountLocal = isDisc ? 0 : g.Sum(x => x.IgstAmountLocal),
                    LocalOneCentsDiff = isDisc ? 0 : g.Sum(x => x.LocalOneCentsDiff),
                    ServiceAmount = isDisc ? 0 : g.Sum(x => x.ServiceAmount),
                    ServiceAmountLocal = isDisc ? 0 : g.Sum(x => x.ServiceAmountLocal),
                    WVatAmount = isDisc ? 0 : g.Sum(x => x.WVatAmount),
                    WVatAmountLocal = isDisc ? 0 : g.Sum(x => x.WVatAmountLocal)
                };
            })
            .ToList();



        var invoice = await invoiceServices.SaveInvoicesWithTransferAsync(invoiceHeader, invoiceDetails);
        await SaveIntoGLTransaction(invoiceHeader, invoiceDetails);
    }



    protected async Task SaveIntoGLTransaction(InvoicesHeader invoicesHeader, List<InvoiceDetails> invoiceDetails)
    {
       


        List<GLTransactions> gLTransactions = new();

        foreach (var details in invoiceDetails)
        {
            // Laktawan ang lines na walang amount (hal. SALES DISCOUNT na 0.00)
            if (details.NetAmount == 0.00M)
                continue;

            // Amount na WALANG tax (28,285.71)
            // Palitan kung iba ang pangalan ng property mo (hal. details.TaxExclusiveAmount)
            var amountExclTax = details.NetAmount - details.TaxAmount;

            // ============================
            // 1) PRODUCT / SALES ROW (tax-exclusive)
            // ============================

     
            gLTransactions.Add(new GLTransactions
            {
                TransactionDate = invoicesHeader.InvoiceDate,
                PostingDate = invoicesHeader.InvoiceDate,

                GLAccountId = invoicesHeader.glaccountTAXSALESACCOUNT,           // Sales/Product account

                Description = invoicesHeader.InvoiceTo,
                DetailDescription = details.Description,

                CurrencyId = invoicesHeader.CurrencyId,
                CurrencyRate = invoicesHeader.CurrencyRate,

                Debit = 0.00M,
                Credit = amountExclTax,
                DebitForeign = 0.00M,
                CreditForeign = amountExclTax,

                TableId = 8,
                TransactionTypeId = 6,
                TransactionId = details.Id,

                DocumentId = invoicesHeader.Id,
                DocumentCode = invoicesHeader.InvoiceCode,
                ReferenceNo = invoicesHeader.ReferenceNo,
                IsCancelled = invoicesHeader.IsCancelled,
                FromAccountId = invoicesHeader.DebtorId,     // <-- Debtor
                IsPostDatedCheque = false,
                JournalType = "INV",
                DocType = "InvoiceDetails",
                TaxCodeId = details.TaxCodeId
                // Ref1 = walang laman
            });

            // ============================
            // 2) TAX ROW (per detail na may tax)
            // ============================
            if (details.TaxAmount != 0.00M)
            {
                // GL account ng tax code (hal. Output VAT). Kunin mula sa TaxCodes table/service mo.


        

                gLTransactions.Add(new GLTransactions
                {
                    TransactionDate = invoicesHeader.InvoiceDate,
                    PostingDate = invoicesHeader.InvoiceDate,

                    GLAccountId = invoicesHeader.glaccountOUPUTVAT,            // <-- Tax account

                    Description = invoicesHeader.InvoiceTo,
                    DetailDescription = details.Description,

                    CurrencyId = invoicesHeader.CurrencyId,
                    CurrencyRate = invoicesHeader.CurrencyRate,

                    Debit = 0.00M,
                    Credit = details.TaxAmount,              // 3,394.29
                    DebitForeign = 0.00M,
                    CreditForeign = details.TaxAmount,

                    TableId = 8,
                    TransactionTypeId = 6,
                    TransactionId = details.Id,

                    DocumentId = invoicesHeader.Id,
                    DocumentCode = invoicesHeader.InvoiceCode,
                    ReferenceNo = invoicesHeader.ReferenceNo,
                    IsCancelled = invoicesHeader.IsCancelled,
                    FromAccountId = invoicesHeader.glaccountTAXSALESACCOUNT,     // <-- Product account
                    IsPostDatedCheque = false,
                    JournalType = "INV",
                    DocType = "InvoiceDetails",
                    TaxCodeId = details.TaxCodeId,
                    Ref1 = invoicesHeader.ReferenceNo        // <-- may Ref1 dito
                });
            }
        }

        // ============================
        // 3) HEADER ENTRY (Debtor)
        // ============================
        var allDescriptions = string.Join(", ",
            invoiceDetails
                .OrderBy(x => (x.Description ?? "").ToUpper().Contains("DISCOUNT")) // discount sa dulo
                .Select(x => x.Description?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        gLTransactions.Add(new GLTransactions
        {
            TransactionDate = invoicesHeader.InvoiceDate,
            PostingDate = invoicesHeader.InvoiceDate,

            GLAccountId = invoicesHeader.DebtorId,

            Description = "SALES",
            DetailDescription = allDescriptions,

            CurrencyId = invoicesHeader.CurrencyId,
            CurrencyRate = invoicesHeader.CurrencyRate,

            Debit = invoicesHeader.NetTotalAmount,           // 31,680.00
            Credit = 0.00M,
            DebitForeign = invoicesHeader.NetTotalAmount,
            CreditForeign = 0.00M,

            TableId = 7,
            TransactionTypeId = 6,
            TransactionId = invoicesHeader.Id,

            DocumentId = invoicesHeader.Id,
            DocumentCode = invoicesHeader.InvoiceCode,
            ReferenceNo = invoicesHeader.ReferenceNo,
            FromAccountId = invoicesHeader.DebtorId,
            IsPostDatedCheque = false,
            JournalType = "INV",
            DocType = "Invoices"
        });

        var result = await glTransactionsServices.SaveInvoiceGLTransactions(gLTransactions, invoicesHeader.Id);

        Console.WriteLine($"GLTransactions Count: {gLTransactions.Count}");
        Console.WriteLine($"Save Result: {result}");

        if (result.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
            throw new Exception(result);
    }



    //protected async Task SaveIntoGLTransaction(InvoicesHeader invoicesHeader, List<InvoiceDetails> invoiceDetails)
    //{
    //    List<GLTransactions> gLTransactions = new();

    //    // ============================
    //    // DETAILS (SalesInvoiceDetails) TAXECLUSIVE HAS VALUE
    //    // ============================

    //    if (invoicesHeader.TaxExclusiveTotalAmount != 0.00M)
    //    {
    //        foreach (var details in invoiceDetails)
    //        {
    //            gLTransactions.Add(new GLTransactions
    //            {
    //                TransactionDate = invoicesHeader.InvoiceDate,
    //                PostingDate = invoicesHeader.InvoiceDate,

    //                GLAccountId = details.GLAccountId,     // AP / Supplier Account

    //                Description = invoicesHeader.InvoiceTo,
    //                DetailDescription = details.Description,


    //                CurrencyId = invoicesHeader.CurrencyId,
    //                CurrencyRate = invoicesHeader.CurrencyRate,

    //                Debit = 0.00M,///
    //                Credit = details.NetAmount,/////

    //                DebitForeign = 0.00M,////
    //                CreditForeign = details.NetAmount,///

    //                TableId = 8,///THIS ID IS FROM TransactionTables BillDetails
    //                TransactionTypeId = 6, ///THIS ID IS FROM TransactionTypes
    //                TransactionId = details.Id,

    //                DocumentId = invoicesHeader.Id,
    //                DocumentCode = invoicesHeader.InvoiceCode,
    //                ReferenceNo = invoicesHeader.ReferenceNo,
    //                IsCancelled = invoicesHeader.IsCancelled,
    //                // FromAccountId = invoicesHeader.GLAccountId, // HINDI KO MAKITA YUN SOURCE ID
    //                IsPostDatedCheque = false,
    //                JournalType = "INV",
    //                DocType = "InvoiceDetails",
    //                TaxCodeId = details.TaxCodeId,
    //                Ref1 = invoicesHeader.ReferenceNo


    //            });
    //        }
    //    }

    //    // ============================
    //    // DETAILS (SalesInvoiceDetails) TAXTOTALAMOUNT HAS VALUE
    //    // ============================

    //    if (invoicesHeader.TaxTotalAmount != 0.00M)
    //    {
    //        foreach (var details in invoiceDetails)
    //        {
    //            gLTransactions.Add(new GLTransactions
    //            {
    //                TransactionDate = invoicesHeader.InvoiceDate,
    //                PostingDate = invoicesHeader.InvoiceDate,

    //                GLAccountId = details.GLAccountId,     // AP / Supplier Account

    //                Description = invoicesHeader.InvoiceTo,
    //                DetailDescription = details.Description,

    //                CurrencyId = invoicesHeader.CurrencyId,
    //                CurrencyRate = invoicesHeader.CurrencyRate,

    //                Debit = 0.00M,///
    //                Credit = details.NetAmount,/////

    //                DebitForeign = 0.00M,////
    //                CreditForeign = details.NetAmount,///

    //                TableId = 8,///THIS ID IS FROM TransactionTables BillDetails
    //                TransactionTypeId = 6, ///THIS ID IS FROM TransactionTypes
    //                TransactionId = details.Id,

    //                DocumentId = invoicesHeader.Id,
    //                DocumentCode = invoicesHeader.InvoiceCode,
    //                ReferenceNo = invoicesHeader.ReferenceNo,
    //                IsCancelled = invoicesHeader.IsCancelled,
    //                // FromAccountId = invoicesHeader.GLAccountId, // HINDI KO MAKITA YUN SOURCE ID
    //                IsPostDatedCheque = false,
    //                JournalType = "INV",
    //                DocType = "InvoiceDetails",
    //                TaxCodeId = details.TaxCodeId,
    //                Ref1 = invoicesHeader.ReferenceNo


    //            });
    //        }
    //    }


    //    // ============================
    //    // HEADER ENTRY (SalesInvoice) 
    //    // ============================

    //    var allDescriptions = string.Join(", ",
    //invoiceDetails
    //    .OrderByDescending(x => (x.Description ?? "").ToUpper().Contains("DISCOUNT"))
    //    .Select(x => x.Description?.Trim())
    //    .Where(x => !string.IsNullOrWhiteSpace(x)));
    //    gLTransactions.Add(new GLTransactions
    //    {
    //        TransactionDate = invoicesHeader.InvoiceDate,
    //        PostingDate = invoicesHeader.InvoiceDate,

    //        GLAccountId = invoicesHeader.DebtorId,     // AP / Supplier Account

    //        Description = "SALES",
    //        DetailDescription = allDescriptions,

    //        CurrencyId = invoicesHeader.CurrencyId,
    //        CurrencyRate = invoicesHeader.CurrencyRate,

    //        Debit = invoicesHeader.NetTotalAmount,///
    //        Credit = 0.00M,/////

    //        DebitForeign = invoicesHeader.NetTotalAmount,////
    //        CreditForeign = 0.00M,///

    //        TableId = 7,///THIS ID IS FROM TransactionTables Bill
    //        TransactionTypeId = 6, ///THIS ID IS FROM TransactionTypes
    //        TransactionId = invoicesHeader.Id,

    //        DocumentId = invoicesHeader.Id,
    //        DocumentCode = invoicesHeader.InvoiceCode,
    //        ReferenceNo = invoicesHeader.ReferenceNo,
    //        // IsCancelled = invoicesHeader.IsCancelled,
    //        FromAccountId = invoicesHeader.DebtorId,
    //        IsPostDatedCheque = false,
    //        JournalType = "INV",
    //        DocType = "Invoices",
    //        //Ref1 = invoicesHeader.ReferenceNo
    //    });
    //    var result = await glTransactionsServices
    //       .SaveGLTransactions(gLTransactions);

    //    Console.WriteLine(
    //        $"GLTransactions Count: {gLTransactions.Count}");

    //    Console.WriteLine($"Save Result: {result}");

    //    if (result.StartsWith(
    //        "Error:", StringComparison.OrdinalIgnoreCase))
    //    {
    //        throw new Exception(result);
    //    }
    //}












    public async Task<string> DeleteDetailRows(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var updatedetailsquery = @"DELETE FROM SalesInvoiceDetails WHERE Id=@Id";
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