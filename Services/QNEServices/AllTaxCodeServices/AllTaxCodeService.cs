using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllTaxCodeModels;
using Accounting_V4.Model.QNEModels.SupplierModels;
using Accounting_V4.Services.OptimisticLockFieldServices;
using Accounting_V4.Services.QNEServices.GLAccountServices;
using Dapper;
using Microsoft.Data.SqlClient;
using Radzen.Documents.Markdown;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Accounting_V4.Services.QNEServices.AllTaxCodeServices;

public class AllTaxCodeService
{
    private readonly QneConnectionString qneConnection;

    public AllTaxCodeService(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }
    public async Task<IEnumerable<WTaxCodes>> GetWTaxCodes()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM WTaxCodes WHERE IsActive != 0 AND PayableAccount IS NOT NULL ORDER BY Code ASC";
        var model = await connection.QueryAsync<WTaxCodes>(query, commandType: CommandType.Text);
        return model;

    }

    //FOR SUPPLIERS DEFAULT VAT CODE
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
                         AND TaxCode NOT LIKE 'OP '
                       AND TaxCode NOT LIKE 'N/A'
                       AND TaxCode NOT LIKE 'I-NA'
                       AND TaxCode NOT LIKE 'DST-6%'
                       AND TaxCode NOT LIKE '%I-N/A-%'
                       ORDER by TaxCode ASC";
        var model = await connection.QueryAsync<DefaultVatCode>(query, commandType: CommandType.Text);
        return model;

    }

    //FOR SUPPLIERS DEFAULT W-TAX CODE
    public async Task<IEnumerable<WTaxCodes>> GetDefaultWTaxCode()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT Id
           ,Code
           ,Description
           ,Rate
        ,FORMAT(Rate * 100, 'N2') + '%' AS TaxRate
           ,PayableAccount
           ,CreditableAccount
           ,OptimisticLockField
           ,Remark
           ,IsActive
           ,IsWVat
           ,IsSystemDefined
           ,SelfBearWTaxAccount
                     FROM WTaxCodes
                     WHERE CreditableAccount is not null
                    ORDER BY Code ASC";
        var model = await connection.QueryAsync<WTaxCodes>(query, commandType: CommandType.Text);
        return model;

    }

    //GET GetTaxCodes
    //GET GetDefaultTaxCodes
    public async Task<IEnumerable<TaxCodes>> GetTaxCodes(TaxCodeFilter taxCodefilter)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT TaxCode.Id
                            ,TaxTypeId
                            ,TaxCode
                            ,TaxType.TaxType AS VatType
                            ,TaxCode.Description
                            ,CONCAT(CAST(TaxRate * 100 AS DECIMAL(5, 2)), '%') AS Rate
                            ,TaxCode.TaxRate                            
                            ,IsOutputTax
                            ,TaxDescription
                            ,TaxDescriptionPH
                            ,TaxDescriptionLK
                            ,IsTaxInclusive
                            ,GLAccountId
                            ,IsActive
                            ,IsDefault
                            ,IsSystemData
                            ,TaxClass
                            ,InputTaxCodeId
                            ,OutputTaxCodeId
                            ,MaximumPurchaseAmount
                            ,MaximumTaxAmount
                            ,TaxClassValue
                            ,GSTGLAccountId
                            ,PostGSTExpenseToSameAccount
                            ,TaxCode.OptimisticLockField
                            ,TaxDescriptionIN
                            ,UseSGST
                            ,UseCGST
                            ,UseIGST
                            ,SGST
                            ,CGST
                            ,IGST
                            ,SgstPostingAccount
                            ,CgstPostingAccount
                            ,IgstPostingAccount
                            ,TaxDescriptionKH
                            ,TaxDescriptionMV
                            ,TaxDescriptionSG
                            ,GLAccountDeferredVatId
                            ,GLAccountServicePaymentId
                            ,GLAccountSelfRecipientId
                            ,TaxRateSelfRecipient
                    FROM TaxCodes AS TaxCode
                    LEFT OUTER JOIN TaxTypes AS TaxType
                    ON TaxCode.TaxTypeId = TaxType.Id
                    WHERE IsOutputTax LIKE @IsOrOutTax
                    AND IsActive LIKE @IsActive
                    AND TaxCode NOT LIKE 'OP '
                AND TaxCode NOT LIKE 'N/A'
                AND TaxCode NOT LIKE 'NONVAT'
                AND TaxCode NOT LIKE 'I-NA'
                AND TaxCode NOT LIKE 'DST-6%'
                AND TaxCode NOT LIKE '%I-N/A-%'
                    ORDER BY TaxCode ASC";

        var p = new DynamicParameters();
        p.Add("@IsOrOutTax", taxCodefilter.IsOutOrIn);
        p.Add("@IsActive", taxCodefilter?.IsActive.HasValue == true ? taxCodefilter.IsActive : "%%");

        var model = await connection.QueryAsync<TaxCodes>(query, p);
        return model;

    }


    public async Task<IEnumerable<TaxCodes>> GetAllTaxCodes()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT TaxCode.Id
                            ,TaxTypeId
                            ,TaxCode
                            ,TaxType.TaxType AS VatType
                            ,TaxCode.Description
                            ,CONCAT(CAST(TaxRate * 100 AS DECIMAL(5, 2)), '%') AS Rate
                            ,TaxCode.TaxRate                            
                            ,IsOutputTax
                            ,TaxDescription
                            ,TaxDescriptionPH
                            ,TaxDescriptionLK
                            ,IsTaxInclusive
                            ,GLAccountId
                            ,IsActive
                            ,IsDefault
                            ,IsSystemData
                            ,TaxClass
                            ,InputTaxCodeId
                            ,OutputTaxCodeId
                            ,MaximumPurchaseAmount
                            ,MaximumTaxAmount
                            ,TaxClassValue
                            ,GSTGLAccountId
                            ,PostGSTExpenseToSameAccount
                            ,TaxCode.OptimisticLockField
                            ,TaxDescriptionIN
                            ,UseSGST
                            ,UseCGST
                            ,UseIGST
                            ,SGST
                            ,CGST
                            ,IGST
                            ,SgstPostingAccount
                            ,CgstPostingAccount
                            ,IgstPostingAccount
                            ,TaxDescriptionKH
                            ,TaxDescriptionMV
                            ,TaxDescriptionSG
                            ,GLAccountDeferredVatId
                            ,GLAccountServicePaymentId
                            ,GLAccountSelfRecipientId
                            ,TaxRateSelfRecipient
                    FROM TaxCodes AS TaxCode
                    LEFT OUTER JOIN TaxTypes AS TaxType
                    ON TaxCode.TaxTypeId = TaxType.Id
                    ORDER BY TaxCode ASC";
        var model = await connection.QueryAsync<TaxCodes>(query);
        return model;

    }






    //this approach is depricated
    public async Task<IEnumerable<TaxCodes>> GetTaxCodesDepricated(TaxCodeFilter taxCodeFilter)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = $@"SELECT TaxCode.Id
                            ,TaxTypeId
                            ,TaxCode
                            ,TaxType.TaxType AS VatType
                            ,TaxCode.Description
                            ,CONCAT(CAST(TaxRate * 100 AS DECIMAL(5, 2)), '%') AS Rate
                            ,TaxCode.TaxRate
                            ,IsOutputTax
                            ,TaxDescription
                            ,TaxDescriptionPH
                            ,TaxDescriptionLK
                            ,IsTaxInclusive
                            ,GLAccountId
                            ,IsActive
                            ,IsDefault
                            ,IsSystemData
                            ,TaxClass
                            ,InputTaxCodeId
                            ,OutputTaxCodeId
                            ,MaximumPurchaseAmount
                            ,MaximumTaxAmount
                            ,TaxClassValue
                            ,GSTGLAccountId
                            ,PostGSTExpenseToSameAccount
                            ,TaxCode.OptimisticLockField
                            ,TaxDescriptionIN
                            ,UseSGST
                            ,UseCGST
                            ,UseIGST
                            ,SGST
                            ,CGST
                            ,IGST
                            ,SgstPostingAccount
                            ,CgstPostingAccount
                            ,IgstPostingAccount
                            ,TaxDescriptionKH
                            ,TaxDescriptionMV
                            ,TaxDescriptionSG
                            ,GLAccountDeferredVatId
                            ,GLAccountServicePaymentId
                            ,GLAccountSelfRecipientId
                            ,TaxRateSelfRecipient
                    FROM TaxCodes AS TaxCode
                    LEFT OUTER JOIN TaxTypes AS TaxType
                    ON TaxCode.TaxTypeId = TaxType.Id
                    WHERE IsOutputTax = @IsOutOrIn
                    AND IsActive = @IsActive
                    ORDER BY TaxCode ASC";

        var p = new DynamicParameters();
        p.Add("@IsOutOrIn", taxCodeFilter.IsOutOrIn);
        p.Add("@IsActive", taxCodeFilter.IsActive);

        var model = await connection.QueryAsync<TaxCodes>(query, p);
        return model;

    }


    //GET GetGLTaxCodesById
    public async Task<GLAccountTaxCodeLinks> GetGLTaxCodesById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM GLAccountTaxCodeLinks WHERE GLAccountId = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<GLAccountTaxCodeLinks>(query, new { Id });

        return model;
    }



    public async Task<FilterTaxCodeResult> GetFilteredTaxCodes()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        {
            var query = @"
        SELECT 
            TaxCode.Id,
            TaxTypeId,
            TaxCode,
            TaxType.TaxType AS VatType,
            TaxCode.Description,
            CONCAT(CAST(TaxRate * 100 AS DECIMAL(5, 2)), '%') AS Rate,
            TaxCode.TaxRate,
            IsOutputTax,
            TaxDescription,
            TaxDescriptionPH,
            TaxDescriptionLK,
            IsTaxInclusive,
            GLAccountId,
            IsActive,
            IsDefault,
            IsSystemData,
            TaxClass,
            InputTaxCodeId,
            OutputTaxCodeId,
            MaximumPurchaseAmount,
            MaximumTaxAmount,
            TaxClassValue,
            GSTGLAccountId,
            PostGSTExpenseToSameAccount,
            TaxCode.OptimisticLockField,
            TaxDescriptionIN,
            UseSGST,
            UseCGST,
            UseIGST,
            SGST,
            CGST,
            IGST,
            SgstPostingAccount,
            CgstPostingAccount,
            IgstPostingAccount,
            TaxDescriptionKH,
            TaxDescriptionMV,
            TaxDescriptionSG,
            GLAccountDeferredVatId,
            GLAccountServicePaymentId,
            GLAccountSelfRecipientId,
            TaxRateSelfRecipient
        FROM TaxCodes AS TaxCode
        LEFT OUTER JOIN TaxTypes AS TaxType 
            ON TaxCode.TaxTypeId = TaxType.Id
        ORDER BY TaxCode ASC;";

            var result = await connection.QueryAsync<TaxCodes>(query);

            var taxResult = new FilterTaxCodeResult
            {
                InputTaxCodes = result.Where(x => x.IsOutputTax == false).ToList(),
                OutputTaxCodes = result.Where(x => x.IsOutputTax == true).ToList()
            };


            return taxResult;
        }
    }





}
