using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllTaxCodeModels;
using Accounting_V4.Model.QNEModels.CustomerModels;
using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Model.QNEModels.PuchasesModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.SupplierModels;
using Accounting_V4.Pages.QNEPages.Supplier;
using Accounting_V4.Services.QNEServices.GLAccountServices;
using Dapper;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Accounting_V4.Services.QNEServices.SupplierServices;

public class SuppliersServices
{

    public GLAccountsServices glAccountServices
    {
        get; set;
    }

    private readonly QneConnectionString qneConnection;

    public SuppliersServices(QneConnectionString qneConnection, GLAccountsServices glAccountServices)
    {
        this.qneConnection = qneConnection;
        this.glAccountServices = glAccountServices;
    }

    public async Task<CreditorCategory> GetCreditorCategoryById(int? Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM CreditorCategory WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<CreditorCategory>(query, new { Id });
        return model;
    }

    public async Task<IEnumerable<CreditorCategory>> GetCreditorCategoryLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM CreditorCategory ORDER BY Id ASC";
        var model = await connection.QueryAsync<CreditorCategory>(query, commandType: CommandType.Text);
        return model;
    }

    public async Task<string> SaveCreditorCategory(CreditorCategory creditorCategory)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var inserheaderquery = @"INSERT INTO CreditorCategory
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
        UPDATE CreditorCategory SET
            CategoryCode = @CategoryCode,
            Description = @Description,
            IsDefault = @IsDefault,
            OptimisticLockField = OptimisticLockField + 1
        WHERE Id = @Id;";
        try
        {
            var query = @"SELECT COUNT(1) FROM CreditorCategory WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<bool>(query, new { Id = creditorCategory.Id }, transaction);

            if (!count)
            {
                await connection.ExecuteAsync(inserheaderquery, creditorCategory, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateheaderquery, creditorCategory, transaction);
            }

            transaction.Commit();
            return $"Success: Supplier Category saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }



    public async Task<IEnumerable<Creditors>> GetSupplierLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Creditors ORDER BY CompanyCode ASC";
        var model = await connection.QueryAsync<Creditors>(query, commandType: CommandType.Text);
        return model;


    }
    //LIST OF SPECIAL ACCOUNTS
    public async Task<IEnumerable<VW_CreditorListing>> GetSuppliers()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM VW_CreditorListing ORDER BY CompanyCode ASC";
        var model = await connection.QueryAsync<VW_CreditorListing>(query, commandType: CommandType.Text);
        return model;
    }


    //Get Category from CreditorCategory
    public async Task<IEnumerable<CreditorCategory>> GetCreditorCategory()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM CreditorCategory";
        var model = await connection.QueryAsync<CreditorCategory>(query, commandType: CommandType.Text);
        return model;

    }

    //Get Category from CreditorCategory
    public async Task<IEnumerable<DefaultVatCode>> GetDefaultVatCode()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT TaxCodes.Id
                 ,TaxTypeId
                 ,TaxCode
                 ,FORMAT(TaxCodes.TaxRate * 100, 'N2') + '%' AS TaxRate
                 ,TaxTypes.TaxType
                 ,IsOutputTax
                 ,TaxCodes.Description
                FROM TaxCodes LEFT OUTER JOIN TaxTypes
                ON TaxCodes.TaxTypeId = TaxTypes.Id
                WHERE IsOutputTax != 1
                ORDER by TaxCodes.TaxRate ASC";
        var model = await connection.QueryAsync<DefaultVatCode>(query, commandType: CommandType.Text);
        return model;

    }


    public async Task<IEnumerable<WTaxCodes>> GetDefaultWTaxCodes() //DefaultWTaxCodes
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT  Id
                    ,Code
                    ,Description
                    ,Rate
                 ,FORMAT(Rate * 100, 'N2') + '%' AS Rates
                    ,PayableAccount
                    ,CreditableAccount
                    ,OptimisticLockField
                    ,Remark
                    ,IsActive
                    ,IsWVat
                    ,IsSystemDefined
                    ,SelfBearWTaxAccount
                FROM WTaxCodes
                WHERE PayableAccount IS NOT NULL
                ORDER BY Code ASC";
        var model = await connection.QueryAsync<WTaxCodes>(query, commandType: CommandType.Text);
        return model;

    }

    //Get Customer From db.Debtors By Id
    public async Task<Creditors> GetSupplierById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Creditors WHERE Id = @Id";

        var model = await connection.QueryFirstOrDefaultAsync<Creditors>(query, new { Id });

        return model;

    }


    public string result;
    public async Task<string> SaveSupplier(Creditors suppliers, string Id)
    {

        if (!string.IsNullOrWhiteSpace(Id))
        {
            var data = await UpdateSupplier(suppliers);
            result = data;
        }
        else
        {
            var data = await InsertSupplier(suppliers);
            result = data;
        }

        return result;
    }



    public async Task<string> InsertSupplier(Creditors suppliers)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();
        var query = @"INSERT INTO Creditors (
                        Id, CompanyCode, CompanyName, CompanyName2, ControlAccountId, GLAccountId, 
                        RegistrationNo, Address1, Address2, Address3, Address4, ContactPerson, 
                        PhoneNo1, PhoneNo2, FaxNo1, FaxNo2, Email, Homepage, BusinessNature,
                        IsProspect, CategoryId, DeliveryAddress1, DeliveryAddress2, DeliveryAddress3, 
                        DeliveryAddress4, AreaId, PurchaserId, CurrencyId, TermId, CreditLimit, 
                        IsExceedCreditAllowed, IsTaxExempted, StatementTypeId, BillingCycleId, 
                        StatementCycleId, BillingContactPerson, BillingContactPhoneNo, BillingContactEmail, 
                        AccountContactPerson, AccountContactPhoneNo, AccountContactEmail, ManagementContactPerson, 
                        ManagementContactPhoneNo, ManagementContactEmail, GuarantorName, GuarantorIdNo, 
                        GuarantorGender, GuarantorRace, GuarantorCitizenship, CurrentBalance, GSTTypeId, 
                        GSTExemptionNo, GSTExemptionExpiryDate, PurchasesTypeId, StartDate, SMSGroupCode, 
                        SMSNumber, GSTRegNo, DefaultTaxCodeId, PriceGroupId, GSTStatusVerifiedDate, Notes, 
                        OptimisticLockField, BIRClassification, DefaultWTaxCodeId, BranchCode, PostingAccountGroupId, 
                        SalesTaxRegNo, ServiceTaxRegNo, IsDeferredVAT, ZipCode, GroupId, Status, 
                        Maybank2ePaymentMethodId, PaymentBankId, PaymentBankAccNo, PaymentNewId, 
                        PaymentOldId, PaymentOtherId, PaymentBillerCode, PaymentReference1, PaymentReference2, 
                        SalesTaxExemptionNo, SalesTaxExemptionExpDate
                    ) VALUES (
                        @Id, @CompanyCode, @CompanyName, @CompanyName2, @ControlAccountId, @GLAccountId, 
                        @RegistrationNo, @Address1, @Address2, @Address3, @Address4, @ContactPerson, 
                        @PhoneNo1, @PhoneNo2, @FaxNo1, @FaxNo2, @Email, @Homepage, @BusinessNature,
                        @IsProspect, @CategoryId, @DeliveryAddress1, @DeliveryAddress2, @DeliveryAddress3, 
                        @DeliveryAddress4, @AreaId, @PurchaserId, @CurrencyId, @TermId, @CreditLimit, 
                        @IsExceedCreditAllowed, @IsTaxExempted, @StatementTypeId, @BillingCycleId, 
                        @StatementCycleId, @BillingContactPerson, @BillingContactPhoneNo, @BillingContactEmail, 
                        @AccountContactPerson, @AccountContactPhoneNo, @AccountContactEmail, @ManagementContactPerson, 
                        @ManagementContactPhoneNo, @ManagementContactEmail, @GuarantorName, @GuarantorIdNo, 
                        @GuarantorGender, @GuarantorRace, @GuarantorCitizenship, @CurrentBalance, @GSTTypeId, 
                        @GSTExemptionNo, @GSTExemptionExpiryDate, @PurchasesTypeId, @StartDate, @SMSGroupCode, 
                        @SMSNumber, @GSTRegNo, @DefaultTaxCodeId, @PriceGroupId, @GSTStatusVerifiedDate, @Notes, 
                        @OptimisticLockField, @BIRClassification, @DefaultWTaxCodeId, @BranchCode, @PostingAccountGroupId, 
                        @SalesTaxRegNo, @ServiceTaxRegNo, @IsDeferredVAT, @ZipCode, @GroupId, @Status, 
                        @Maybank2ePaymentMethodId, @PaymentBankId, @PaymentBankAccNo, @PaymentNewId, 
                        @PaymentOldId, @PaymentOtherId, @PaymentBillerCode, @PaymentReference1, @PaymentReference2, 
                        @SalesTaxExemptionNo, @SalesTaxExemptionExpDate
                    );";
        try
        {
            // GetControlAccountId base on selected on dropdown
            var glaccountAccountId = await glAccountServices.GetGLAccountsById(suppliers.ControlAccountId.Value.ToString());

            GLAccounts useraccount = new GLAccounts
            {
                Id = Guid.NewGuid(),
                ParentId = suppliers.ControlAccountId,
                GLAccountCode = suppliers.CompanyCode,
                AccountId = glaccountAccountId.AccountId,
                SpecialAccountId = 15,
                Description = suppliers.CompanyName,
                Description2 = suppliers.CompanyName2
            };
            // Create GL Account
            var affectedrow = await glAccountServices.InsertGLAccounts(useraccount);

            var affectedRowCreditor = await connection.ExecuteAsync(query,
                new
                {
                    suppliers.Id,
                    suppliers.CompanyCode,
                    suppliers.CompanyName,
                    suppliers.CompanyName2,
                    suppliers.ControlAccountId,
                    GLAccountId = useraccount.Id, // galing sa na-insert na GL account
                    suppliers.RegistrationNo,
                    suppliers.Address1,
                    suppliers.Address2,
                    suppliers.Address3,
                    suppliers.Address4,
                    suppliers.ContactPerson,
                    suppliers.PhoneNo1,
                    suppliers.PhoneNo2,
                    suppliers.FaxNo1,
                    suppliers.FaxNo2,
                    suppliers.Email,
                    suppliers.Homepage,
                    suppliers.BusinessNature,
                    suppliers.IsProspect,
                    suppliers.CategoryId,
                    suppliers.DeliveryAddress1,
                    suppliers.DeliveryAddress2,
                    suppliers.DeliveryAddress3,
                    suppliers.DeliveryAddress4,
                    suppliers.AreaId,
                    suppliers.PurchaserId,
                    suppliers.CurrencyId,
                    suppliers.TermId,
                    suppliers.CreditLimit,
                    suppliers.IsExceedCreditAllowed,
                    suppliers.IsTaxExempted,
                    suppliers.StatementTypeId,
                    suppliers.BillingCycleId,
                    suppliers.StatementCycleId,
                    suppliers.BillingContactPerson,
                    suppliers.BillingContactPhoneNo,
                    suppliers.BillingContactEmail,
                    suppliers.AccountContactPerson,
                    suppliers.AccountContactPhoneNo,
                    suppliers.AccountContactEmail,
                    suppliers.ManagementContactPerson,
                    suppliers.ManagementContactPhoneNo,
                    suppliers.ManagementContactEmail,
                    suppliers.GuarantorName,
                    suppliers.GuarantorIdNo,
                    suppliers.GuarantorGender,
                    suppliers.GuarantorRace,
                    suppliers.GuarantorCitizenship,
                    suppliers.CurrentBalance,
                    suppliers.GSTTypeId,
                    suppliers.GSTExemptionNo,
                    suppliers.GSTExemptionExpiryDate,
                    suppliers.PurchasesTypeId,
                    suppliers.StartDate,
                    suppliers.SMSGroupCode,
                    suppliers.SMSNumber,
                    suppliers.GSTRegNo,
                    suppliers.DefaultTaxCodeId,
                    suppliers.PriceGroupId,
                    suppliers.GSTStatusVerifiedDate,
                    suppliers.Notes,
                    suppliers.OptimisticLockField,
                    suppliers.BIRClassification,
                    suppliers.DefaultWTaxCodeId,
                    suppliers.BranchCode,
                    suppliers.PostingAccountGroupId,
                    suppliers.SalesTaxRegNo,
                    suppliers.ServiceTaxRegNo,
                    suppliers.IsDeferredVAT,
                    suppliers.ZipCode,
                    suppliers.GroupId,
                    suppliers.Status,
                    suppliers.Maybank2ePaymentMethodId,
                    suppliers.PaymentBankId,
                    suppliers.PaymentBankAccNo,
                    suppliers.PaymentNewId,
                    suppliers.PaymentOldId,
                    suppliers.PaymentOtherId,
                    suppliers.PaymentBillerCode,
                    suppliers.PaymentReference1,
                    suppliers.PaymentReference2,
                    suppliers.SalesTaxExemptionNo,
                    suppliers.SalesTaxExemptionExpDate
                }, transaction);

            // Commit sabay
            transaction.Commit();
            return $"Insert {affectedRowCreditor} Supplier(s) Success!";
        }

        //catch mo yun error dito
        catch (SqlException ex)
        {
            transaction.Rollback();
            if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
            {

                if (ex.Message.Contains("Creditors"))
                {
                    return $"Duplicate value detected on Creditors. Supplier Code must be unique.";
                }
                return "Duplicate value detected. Values must be unique.";
            }
            throw;
        }

    }


    public async Task<string> UpdateSupplier(Creditors suppliers)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();
        try
        {
            // GetControlAccountId base on selected on dropdown
            var glaccountAccountId = await glAccountServices.GetGLAccountsById(suppliers.ControlAccountId.Value.ToString());

            GLAccounts useraccount = new GLAccounts
            {
                Id = suppliers.GLAccountId,
                ParentId = suppliers.ControlAccountId,
                GLAccountCode = suppliers.CompanyCode,
                AccountId = glaccountAccountId.AccountId,
                SpecialAccountId = 15,
                Description = suppliers.CompanyName,
                Description2 = suppliers.CompanyName2
            };

            // Create GL Account
            var rowaffectedGLAccount = await glAccountServices.UpdateGLAccounts(useraccount);



            // Query para sa Creditors
            var query = @"UPDATE Creditors SET
            CompanyCode = @CompanyCode,
            CompanyName = @CompanyName,
            CompanyName2 = @CompanyName2,
            ControlAccountId = @ControlAccountId,
            GLAccountId = @GLAccountId,
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
            IsProspect = @IsProspect,
            CategoryId = @CategoryId,
            DeliveryAddress1 = @DeliveryAddress1,
            DeliveryAddress2 = @DeliveryAddress2,
            DeliveryAddress3 = @DeliveryAddress3,
            DeliveryAddress4 = @DeliveryAddress4,
            AreaId = @AreaId,
            PurchaserId = @PurchaserId,
            CurrencyId = @CurrencyId,
            TermId = @TermId,
            CreditLimit = @CreditLimit,
            IsExceedCreditAllowed = @IsExceedCreditAllowed,
            IsTaxExempted = @IsTaxExempted,
            StatementTypeId = @StatementTypeId,
            BillingCycleId = @BillingCycleId,
            StatementCycleId = @StatementCycleId,
            BillingContactPerson = @BillingContactPerson,
            BillingContactPhoneNo = @BillingContactPhoneNo,
            BillingContactEmail = @BillingContactEmail,
            AccountContactPerson = @AccountContactPerson,
            AccountContactPhoneNo = @AccountContactPhoneNo,
            AccountContactEmail = @AccountContactEmail,
            ManagementContactPerson = @ManagementContactPerson,
            ManagementContactPhoneNo = @ManagementContactPhoneNo,
            ManagementContactEmail = @ManagementContactEmail,
            GuarantorName = @GuarantorName,
            GuarantorIdNo = @GuarantorIdNo,
            GuarantorGender = @GuarantorGender,
            GuarantorRace = @GuarantorRace,
            GuarantorCitizenship = @GuarantorCitizenship,
            CurrentBalance = @CurrentBalance,
            GSTTypeId = @GSTTypeId,
            GSTExemptionNo = @GSTExemptionNo,
            GSTExemptionExpiryDate = @GSTExemptionExpiryDate,
            PurchasesTypeId = @PurchasesTypeId,
            StartDate = @StartDate,
            SMSGroupCode = @SMSGroupCode,
            SMSNumber = @SMSNumber,
            GSTRegNo = @GSTRegNo,
            DefaultTaxCodeId = @DefaultTaxCodeId,
            PriceGroupId = @PriceGroupId,
            GSTStatusVerifiedDate = @GSTStatusVerifiedDate,
            Notes = @Notes,
            OptimisticLockField = @OptimisticLockField,
            BIRClassification = @BIRClassification,
            DefaultWTaxCodeId = @DefaultWTaxCodeId,
            BranchCode = @BranchCode,
            PostingAccountGroupId = @PostingAccountGroupId,
            SalesTaxRegNo = @SalesTaxRegNo,
            ServiceTaxRegNo = @ServiceTaxRegNo,
            IsDeferredVAT = @IsDeferredVAT,
            ZipCode = @ZipCode,
            GroupId = @GroupId,
            Status = @Status,
            Maybank2ePaymentMethodId = @Maybank2ePaymentMethodId,
            PaymentBankId = @PaymentBankId,
            PaymentBankAccNo = @PaymentBankAccNo,
            PaymentNewId = @PaymentNewId,
            PaymentOldId = @PaymentOldId,
            PaymentOtherId = @PaymentOtherId,
            PaymentBillerCode = @PaymentBillerCode,
            PaymentReference1 = @PaymentReference1,
            PaymentReference2 = @PaymentReference2,
            SalesTaxExemptionNo = @SalesTaxExemptionNo,
            SalesTaxExemptionExpDate = @SalesTaxExemptionExpDate
        WHERE Id = @Id;";

            var affectedRowCreditor = await connection.ExecuteAsync(query, suppliers, transaction);

            // Commit pag walang error
            transaction.Commit();
            return $"Update {affectedRowCreditor} Supplier(s) Success!"; ;
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"{ex.Message}";
            throw;
        }
    }



    public async Task<string> DeleteSupplier(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        var queryCreditors = @"DELETE FROM Creditors WHERE GLAccountId = @Id";
        var queryGLAccounts = @"DELETE FROM GLAccounts WHERE Id = @Id";

        using var transaction = connection.BeginTransaction();
        try
        {
            // Unahin i-delete yung Creditors bago GLAccounts
            int rowsCreditors = await connection.ExecuteAsync(queryCreditors, new { Id }, transaction);
            int rowsGLAccounts = await connection.ExecuteAsync(queryGLAccounts, new { Id }, transaction);

            transaction.Commit();
            return $"Deleted {rowsCreditors} creditor(s) and {rowsGLAccounts} GL account(s).";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();

            // Check kung FK conflict yung error
            if (ex.Message.Contains("REFERENCE constraint"))
            {
                string tableName = "";

                if (ex.Message.Contains("GoodsReceivedNotes"))
                    tableName = "Goods Received Notes";

                if (!string.IsNullOrEmpty(tableName))
                {
                    return $"This supplier account is being used in {tableName}.";
                }

                return "This supplier account is being used in other transactions.";
            }

            throw; // ibang error, ipasa ulit
        }
    }

    public async Task<IEnumerable<Purchasers>> GetPurchasersLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Purchasers  WHERE IsActive != 0 ORDER BY StaffCode ASC";
        var model = await connection.QueryAsync<Purchasers>(query, commandType: CommandType.Text);
        return model;

    }

    public async Task<Purchasers> GetPurchasersById(int? Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Purchasers WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<Purchasers>(query, new { Id });
        return model;
    }

    public async Task<string> SavePurchasers(Purchasers purchasers)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertheaderquery = @"
        INSERT INTO Purchasers
        (
            StaffCode,
            Name,
            IDNo,
            DateJoined,
            DateLeft,
            MobileNo,
            Email,
            Salary,
            SOCSO,
            EPF,
            TaxFile,
            Remarks,
            Gender,
            IsActive,
            IsManager,
            DefaultTeam,
            IsDefault,
            OptimisticLockField
        )
        VALUES
        (
            @StaffCode,
            @Name,
            @IDNo,
            @DateJoined,
            @DateLeft,
            @MobileNo,
            @Email,
            @Salary,
            @SOCSO,
            @EPF,
            @TaxFile,
            @Remarks,
            @Gender,
            @IsActive,
            @IsManager,
            @DefaultTeam,
            @IsDefault,
            @OptimisticLockField
        );";
        var updateheaderquery = @"
        UPDATE Purchasers
        SET
            StaffCode = @StaffCode,
            Name = @Name,
            IDNo = @IDNo,
            DateJoined = @DateJoined,
            DateLeft = @DateLeft,
            MobileNo = @MobileNo,
            Email = @Email,
            Salary = @Salary,
            SOCSO = @SOCSO,
            EPF = @EPF,
            TaxFile = @TaxFile,
            Remarks = @Remarks,
            Gender = @Gender,
            IsActive = @IsActive,
            IsManager = @IsManager,
            DefaultTeam = @DefaultTeam,
            IsDefault = @IsDefault,
            OptimisticLockField = OptimisticLockField + 1
        WHERE Id = @Id";

        try
        {
            var query = @"SELECT COUNT(1) FROM SalesPersons WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<bool>(query, new { Id = purchasers.Id }, transaction);

            if (!count)
            {
                await connection.ExecuteAsync(insertheaderquery, purchasers, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateheaderquery, purchasers, transaction);
            }

            transaction.Commit();
            return $"Success: Purchasers saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }


}
