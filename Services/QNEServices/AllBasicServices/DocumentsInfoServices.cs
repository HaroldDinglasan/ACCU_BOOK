using System.Data;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.DescriptionModels;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.TransactionCodesModels;
using Accounting_V4.Services.OptimisticLockFieldServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.AllBasicServices;

public class DocumentsInfoServices
{
    private readonly QneConnectionString qneConnection;
    private readonly CheckExistServices checkExistServices;
    public DocumentsInfoServices(QneConnectionString qneConnection, CheckExistServices checkExistServices)
    {
        this.qneConnection = qneConnection;
        this.checkExistServices = checkExistServices;
    }

    public async Task<DocumentInfo> GetDocumentInfoById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM DocumentInfo WHERE DocumentId = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<DocumentInfo>(query, new { Id });
        return model;

    }

    protected CheckExistModel docinfoCheck = new CheckExistModel();
    public async Task<string> SaveDocumentInfo(DocumentInfo documentInfo)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertquery = @"INSERT INTO DocumentInfo 
            (Id,DocumentId,DocType,FirstCreatedDate
           ,FirstCreatedUserId,LastModifiedDate
           ,LastModifiedUserId,PrintCount,OptimisticLockField
           ,EmailCount) 
            VALUES 
            (@Id,@DocumentId,@DocType,@FirstCreatedDate
           ,@FirstCreatedUserId,@LastModifiedDate
           ,@LastModifiedUserId,@PrintCount,@OptimisticLockField
           ,@EmailCount)";
        var updatequery = @"UPDATE DocumentInfo SET 
         DocumentId=@DocumentId
        ,DocType=@DocType
        ,LastModifiedDate=@LastModifiedDate
        ,LastModifiedUserId=@LastModifiedUserId
        ,PrintCount=@PrintCount
        ,OptimisticLockField = OptimisticLockField + 1
        ,EmailCount=@EmailCount
        WHERE Id=@Id";

        try
        {
            docinfoCheck.Id = documentInfo.Id;
            docinfoCheck.TableName = "DocumentInfo";
            var existheader = await checkExistServices.RecordExistsByIdAsync(docinfoCheck);
            if (!existheader)
            {
                await connection.ExecuteAsync(insertquery, documentInfo, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updatequery, documentInfo, transaction);
            }
            transaction.Commit();
            return $"Success";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }

}
