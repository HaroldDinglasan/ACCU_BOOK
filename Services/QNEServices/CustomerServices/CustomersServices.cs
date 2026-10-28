using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.AllTaxCodeModels;
using Accounting_V4.Model.QNEModels.CustomerModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.GLAccountServices;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Accounting_V4.Services.QNEServices.CustomerServices;

public class CustomersServices
{

    private readonly QneConnectionString qneConnection;
    public GLAccountsServices glAccountServices;
    public CheckExistServices checkExistServices;
    public CustomersServices(QneConnectionString qneConnection, GLAccountsServices gLAccountsServices, CheckExistServices checkExistServices)
    {
        this.qneConnection = qneConnection;
        this.glAccountServices = gLAccountsServices;
        this.checkExistServices = checkExistServices;
    }


    public async Task<IEnumerable<DebtorCategory>> GetDebtorCategoryLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM DebtorCategory ORDER BY Id ASC";
        var model = await connection.QueryAsync<DebtorCategory>(query, commandType: CommandType.Text);
        return model;
    }





    //LIST OF Customers From Debtors table
    public async Task<IEnumerable<Debtors>> GetCustomersList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM Debtors";
        var model = await connection.QueryAsync<Debtors>(query, commandType: CommandType.Text);
        return model;

    }

    //LIST OF Customers From VW_Debtors table
    public async Task<IEnumerable<VW_Debtors>> GetCustomersListing()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM VW_Debtors";
        var model = await connection.QueryAsync<VW_Debtors>(query, commandType: CommandType.Text);
        return model;

    }

    //Get Control Account from GL_Accounts
    public async Task<IEnumerable<GLAccounts>> GetControlAccounts()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM GLAccounts WHERE IsActive != 0 ORDER BY GLAccountCode ASC";
        var model = await connection.QueryAsync<GLAccounts>(query, commandType: CommandType.Text);
        return model;
    }


    //Get Category from DebtorCategory
    public async Task<IEnumerable<DebtorCategory>> GetDebtorCategory()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM DebtorCategory";
        var model = await connection.QueryAsync<DebtorCategory>(query, commandType: CommandType.Text);
        return model;

    }
    //Get Category from DebtorCategory
    public async Task<DebtorCategory> GetDebtorCategoryById(int? Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM DebtorCategory WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<DebtorCategory>(query, new { Id });
        return model;
    }

    public async Task<string> SaveDebtorCategory(DebtorCategory debtorCategory)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var inserheaderquery = @"INSERT INTO DebtorCategory
        (
            CategoryCode,
            Description,
            IsDefault,
            OptimisticLockField
        )
        VALUES
        (
            @CategoryCode,
            @Description,
            @IsDefault,
            @OptimisticLockField
        )";
        var updateheaderquery = @"
        UPDATE DebtorCategory SET
            CategoryCode = @CategoryCode,
            Description = @Description,
            IsDefault = @IsDefault,
            OptimisticLockField = OptimisticLockField + 1
        WHERE Id = @Id;";
        try
        {
            var query = @"SELECT COUNT(1) FROM DebtorCategory WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<bool>(query, new { Id = debtorCategory.Id }, transaction);
           
            if (!count)
            {
                await connection.ExecuteAsync(inserheaderquery, debtorCategory, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateheaderquery, debtorCategory, transaction);
            }

            transaction.Commit();
            return $"Success: Debtor Category saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }


    //Get Agent from SalesPerson
    public async Task<IEnumerable<SalesPersons>> GetSalesPersons()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM SalesPersons WHERE IsActive != 0 ORDER BY StaffCode ASC";
        var model = await connection.QueryAsync<SalesPersons>(query, commandType: CommandType.Text);
        return model;

    }


    //Get Defaul WVaT Code from db.WTaxCodes
    public async Task<IEnumerable<WTaxCodes>> GetDefaulWVaTCode()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        Guid CreditableAccount = Guid.Empty;
        var query = @"SELECT Id
                   ,Code
                   ,Description
                ,FORMAT(Rate * 100, 'N2') + '%' AS Rates
                ,Rate   
                   ,PayableAccount
                   ,CreditableAccount
                   ,OptimisticLockField
                   ,Remark
                   ,IsActive
                   ,IsWVat
                   ,IsSystemDefined
                   ,SelfBearWTaxAccount
                FROM WTaxCodes
                   WHERE IsActive LIKE 1 AND CreditableAccount IS NOT NULL ORDER BY Code ASC";
        var model = await connection.QueryAsync<WTaxCodes>(query, commandType: CommandType.Text);
        return model;

    }


    //--THIS SECTION FOR PATCH DATA BY ID

    //Get Customer From db.Debtors By Id
    public async Task<Debtors> GetCustomerById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Debtors WHERE Id = @Id";

        var model = await connection.QueryFirstOrDefaultAsync<Debtors>(query, new { Id });

        return model;


    }


    public async Task<string> InsertCustomer(Debtors customers)
    {

        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var query = @"
        INSERT INTO Debtors (
            Id, CompanyCode, CompanyName, CompanyName2, GLAccountId, ControlAccountId,
            RegistrationNo, Address1, Address2, Address3, Address4,
            ContactPerson, PhoneNo1, PhoneNo2, FaxNo1, FaxNo2,
            Email, Homepage, BusinessNature, CategoryId, AreaId, SalesPersonId, CurrencyId, TermId,
            CreditLimit, IsExceedCreditAllowed, IsTaxExempted, BillingCycleId, StatementCycleId,
            BillingContactPerson, BillingContactPhone, BillingContactEmail,
            AccountContactPerson, AccountContactPhone, AccountContactEmail,
            ManagementContactPerson, ManagementContactPhone, ManagementContactEmail,
            GuarantorName, GuarantorIdNo, GuarantorGender, GuarantorRace, GuarantorCitizenship,
            CurrentBalance, StatementTypeId, GSTTypeId, GSTExemptionNo, GSTExpiryDate,
            AdditionalCreditLimit, AdditionalCreditLimitExpiryDate,
            SalesTaxExemptionNo, SalesTaxExemptionExpiryDate,
            UserPriceGroupId, PriceGroupId, UserPriceTag, PriceTag, SalesTypeId,
            StartDate, SourceOfLead,
            MemberName, MemberOccupation, MemberSince, MemberIdNo, MemberBirthday,
            MemberEmail, MemberMobileNo, MemberRace, MemberReligion, MemberValidThru,
            SMSGroupCode, SMSNo, Status, Agent,
            GSTRegNo, DefaultTaxCodeId, Notes, OptimisticLockField,
            BIRClassification, GSTRegTag, DefaultWTaxCodeId, StateId,
            IsGroupCompany, GroupCompanyId, BranchCode, PostingAccountGroupId,
            UseLocalCreditControl, ExcludeSOCreditControl, ExcludeDOCreditControl,
            BusinessStyle, SalesTaxRegNo, ServiceTaxRegNo, SalesTaxExemptionExpDate,
            IsDeferredVAT, ZipCode, GroupId, DefaultWVatCodeId,
            PointCategoryId, PointStatus, PointExpiryDate
        )
        VALUES (
            @Id, @CompanyCode, @CompanyName, @CompanyName2, @GLAccountId, @ControlAccountId,
            @RegistrationNo, @Address1, @Address2, @Address3, @Address4,
            @ContactPerson, @PhoneNo1, @PhoneNo2, @FaxNo1, @FaxNo2,
            @Email, @Homepage, @BusinessNature, @CategoryId, @AreaId, @SalesPersonId, @CurrencyId, @TermId,
            @CreditLimit, @IsExceedCreditAllowed, @IsTaxExempted, @BillingCycleId, @StatementCycleId,
            @BillingContactPerson, @BillingContactPhone, @BillingContactEmail,
            @AccountContactPerson, @AccountContactPhone, @AccountContactEmail,
            @ManagementContactPerson, @ManagementContactPhone, @ManagementContactEmail,
            @GuarantorName, @GuarantorIdNo, @GuarantorGender, @GuarantorRace, @GuarantorCitizenship,
            @CurrentBalance, @StatementTypeId, @GSTTypeId, @GSTExemptionNo, @GSTExpiryDate,
            @AdditionalCreditLimit, @AdditionalCreditLimitExpiryDate,
            @SalesTaxExemptionNo, @SalesTaxExemptionExpiryDate,
            @UserPriceGroupId, @PriceGroupId, @UserPriceTag, @PriceTag, @SalesTypeId,
            @StartDate, @SourceOfLead,
            @MemberName, @MemberOccupation, @MemberSince, @MemberIdNo, @MemberBirthday,
            @MemberEmail, @MemberMobileNo, @MemberRace, @MemberReligion, @MemberValidThru,
            @SMSGroupCode, @SMSNo, @Status, @Agent,
            @GSTRegNo, @DefaultTaxCodeId, @Notes, @OptimisticLockField,
            @BIRClassification, @GSTRegTag, @DefaultWTaxCodeId, @StateId,
            @IsGroupCompany, @GroupCompanyId, @BranchCode, @PostingAccountGroupId,
            @UseLocalCreditControl, @ExcludeSOCreditControl, @ExcludeDOCreditControl,
            @BusinessStyle, @SalesTaxRegNo, @ServiceTaxRegNo, @SalesTaxExemptionExpDate,
            @IsDeferredVAT, @ZipCode, @GroupId, @DefaultWVatCodeId,
            @PointCategoryId, @PointStatus, @PointExpiryDate
        )";

        try
        {
            var glaccountAccountId = await glAccountServices.GetGLAccountsById(customers.ControlAccountId.Value.ToString());
            GLAccounts useraccount = new GLAccounts
            {
                //  Id = Guid.NewGuid(),
                ParentId = customers.ControlAccountId,
                GLAccountCode = customers.CompanyCode,
                AccountId = glaccountAccountId.AccountId,
                //modify this for settings config on AccubookSystemSetting
                SpecialAccountId = 15,
                Description = customers.CompanyName,
                //GLAccountTaxCode = null

            };
            var affectedrow = await glAccountServices.InsertGLAccounts(useraccount);
            if (affectedrow != null)
                customers.GLAccountId = useraccount.Id;
            var affectedRowDebtor = await connection.ExecuteAsync(query, customers, transaction);
            transaction.Commit();
            return $"Insert {affectedRowDebtor} Customer(s) Success!";


        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
            {

                if (ex.Message.Contains("Debtors"))
                {
                    return $"Duplicate value detected on Debtors. Customer Code must be unique.";
                }
                return "Duplicate value detected. Values must be unique.";
            }
            throw;
        }



    }


    public async Task<int> UpdateCustomer(Debtors customers)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"UPDATE Debtors SET
                    CompanyCode = @CompanyCode,
                    CompanyName = @CompanyName,
                    CompanyName2 = @CompanyName2,
                    GLAccountId = @GLAccountId,
                    ControlAccountId = @ControlAccountId,
                    RegistrationNo = @RegistrationNo,
                    Address1 = @Address1,
                    Address2 = @Address2,
                    Address3 = @Address3,
                    Address4 = @Address4,
                    ContactPerson = @ContactPerson,
                    PhoneNo1 = @PhoneNo1,
                    PhoneNo2 = @PhoneNo2,
                    FaxNo1 = @FaxNo1,
                    FaxNo2 = @FaxNo2,
                    Email = @Email,
                    Homepage = @Homepage,
                    BusinessNature = @BusinessNature,
                    Picture = @Picture,
                    CategoryId = @CategoryId,
                    AreaId = @AreaId,
                    SalesPersonId = @SalesPersonId,
                    CurrencyId = @CurrencyId,
                    TermId = @TermId,
                    CreditLimit = @CreditLimit,
                    IsExceedCreditAllowed = @IsExceedCreditAllowed,
                    IsTaxExempted = @IsTaxExempted,
                    BillingCycleId = @BillingCycleId,
                    StatementCycleId = @StatementCycleId,
                    BillingContactPerson = @BillingContactPerson,
                    BillingContactPhone = @BillingContactPhone,
                    BillingContactEmail = @BillingContactEmail,
                    AccountContactPerson = @AccountContactPerson,
                    AccountContactPhone = @AccountContactPhone,
                    AccountContactEmail = @AccountContactEmail,
                    ManagementContactPerson = @ManagementContactPerson,
                    ManagementContactPhone = @ManagementContactPhone,
                    ManagementContactEmail = @ManagementContactEmail,
                    GuarantorName = @GuarantorName,
                    GuarantorIdNo = @GuarantorIdNo,
                    GuarantorGender = @GuarantorGender,
                    GuarantorRace = @GuarantorRace,
                    GuarantorCitizenship = @GuarantorCitizenship,
                    CurrentBalance = @CurrentBalance,
                    StatementTypeId = @StatementTypeId,
                    GSTTypeId = @GSTTypeId,
                    GSTExemptionNo = @GSTExemptionNo,
                    GSTExpiryDate = @GSTExpiryDate,
                    AdditionalCreditLimit = @AdditionalCreditLimit,
                    AdditionalCreditLimitExpiryDate = @AdditionalCreditLimitExpiryDate,
                    SalesTaxExemptionNo = @SalesTaxExemptionNo,
                    SalesTaxExemptionExpiryDate = @SalesTaxExemptionExpiryDate,
                    UserPriceGroupId = @UserPriceGroupId,
                    PriceGroupId = @PriceGroupId,
                    UserPriceTag = @UserPriceTag,
                    PriceTag = @PriceTag,
                    SalesTypeId = @SalesTypeId,
                    StartDate = @StartDate,
                    SourceOfLead = @SourceOfLead,
                    MemberName = @MemberName,
                    MemberOccupation = @MemberOccupation,
                    MemberSince = @MemberSince,
                    MemberIdNo = @MemberIdNo,
                    MemberBirthday = @MemberBirthday,
                    MemberEmail = @MemberEmail,
                    MemberMobileNo = @MemberMobileNo,
                    MemberRace = @MemberRace,
                    MemberReligion = @MemberReligion,
                    MemberValidThru = @MemberValidThru,
                    SMSGroupCode = @SMSGroupCode,
                    SMSNo = @SMSNo,
                    Status = @Status,
                    Agent = @Agent,
                    GSTRegNo = @GSTRegNo,
                    DefaultTaxCodeId = @DefaultTaxCodeId,
                    Notes = @Notes,
                    OptimisticLockField = @OptimisticLockField,
                    BIRClassification = @BIRClassification,
                    GSTRegTag = @GSTRegTag,
                    DefaultWTaxCodeId = @DefaultWTaxCodeId,
                    StateId = @StateId,
                    IsGroupCompany = @IsGroupCompany,
                    GroupCompanyId = @GroupCompanyId,
                    BranchCode = @BranchCode,
                    PostingAccountGroupId = @PostingAccountGroupId,
                    UseLocalCreditControl = @UseLocalCreditControl,
                    ExcludeSOCreditControl = @ExcludeSOCreditControl,
                    ExcludeDOCreditControl = @ExcludeDOCreditControl,
                    BusinessStyle = @BusinessStyle,
                    SalesTaxRegNo = @SalesTaxRegNo,
                    ServiceTaxRegNo = @ServiceTaxRegNo,
                    SalesTaxExemptionExpDate = @SalesTaxExemptionExpDate,
                    IsDeferredVAT = @IsDeferredVAT,
                    ZipCode = @ZipCode,
                    GroupId = @GroupId,
                    DefaultWVatCodeId = @DefaultWVatCodeId,
                    PointCategoryId = @PointCategoryId,
                    PointStatus = @PointStatus,
                    PointExpiryDate = @PointExpiryDate
                WHERE Id = @Id";

        //var p = new DynamicParameters();
        //p.Add("@Id", customers.Id);
        return await connection.ExecuteAsync(query, customers);

    }


    public async Task<string> DeleteCustomer(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        var query = @"DELETE FROM Debtors WHERE Id=@Id";
        using var transaction = connection.BeginTransaction();

        try
        {
            var rowDebtors = await connection.ExecuteAsync(query, new { Id }, transaction);
            transaction.Commit();
            return $"Deleted {rowDebtors} Debtors(s).";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            if (ex.Message.Contains("REFERENCE constraint"))
            {
                var tableName = "";

                if (ex.Message.Contains("Debtors"))
                    tableName = "Debtors Control Account";
                else if (ex.Message.Contains("MonthlyStockValue"))
                    tableName = "Monthly Stock";
                else if (ex.Message.Contains("Creditors"))
                    tableName = "Creditor Control Account";
                if (!string.IsNullOrEmpty(tableName))
                {
                    return $"This account is being used in {tableName}.";
                }

                return "This account is being used in other transactions.";
            }
            throw; // ibang error, ipasa ulit
        }




    }
}
