using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.CustomerModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.SalesServices;

public class SalesService
{

    //GET Aget or SalesPerson List


    private readonly QneConnectionString qneConnection;

    public SalesService(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }
    public async Task<IEnumerable<SalesPersons>> GetSalesPersonsLists()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM SalesPersons  WHERE IsActive != 0 ORDER BY StaffCode ASC";
        var model = await connection.QueryAsync<SalesPersons>(query, commandType: CommandType.Text);
        return model;

    }

    //GET ONE OF SALES PERSONS DATA
    public async Task<SalesPersons> GetSalesPersonsById(int? Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM SalesPersons WHERE Id LIKE @Id";
        var model = await connection.QueryFirstOrDefaultAsync<SalesPersons>(query, new { Id });
        return model;
    }


    public async Task<string> SaveSalesPersons(SalesPersons salesPersons)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
       
        var inserheaderquery = @"
        INSERT INTO SalesPersons
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
            OptimisticLockField,
            CommissionType,
            CommissionByDate,
            FixedCommissionDescription,
            FixedCommissionRate
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
            @OptimisticLockField,
            @CommissionType,
            @CommissionByDate,
            @FixedCommissionDescription,
            @FixedCommissionRate
        )";
        var updateheaderquery = @"
        UPDATE SalesPersons
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
            OptimisticLockField = OptimisticLockField + 1,
            CommissionType = @CommissionType,
            CommissionByDate = @CommissionByDate,
            FixedCommissionDescription = @FixedCommissionDescription,
            FixedCommissionRate = @FixedCommissionRate
        WHERE Id = @Id";
        try
        {
            var query = @"SELECT COUNT(1) FROM SalesPersons WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<bool>(query, new { Id = salesPersons.Id }, transaction);

            if (!count)
            {
                await connection.ExecuteAsync(inserheaderquery, salesPersons, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateheaderquery, salesPersons, transaction);
            }

            transaction.Commit();
            return $"Success: Agent saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }
}
