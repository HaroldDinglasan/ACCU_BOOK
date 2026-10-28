using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.AllBasicModels;
using Accounting_V4.Model.QNEModels.ForTransferModels;
using Accounting_V4.Model.QNEModels.StocksForTransferModels;
using Accounting_V4.Model.QNEModels.StocksModels;
using Accounting_V4.Model.QNEModels.TransactionCodesModels;
using Accounting_V4.Pages.QNEPages.Stocks;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.TransferServices;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Accounting_V4.Services.QNEServices.StockServices;

public class StockItemsService
{
    private readonly QneConnectionString qneConnection;
    private readonly TransferTable transferTable;
    public StocksTransferLinkServices stocksTransferLinkServices;
    public CheckExistServices checkExistServices;

    public StockItemsService(
        QneConnectionString qneConnection,
        TransferTable transferTable,
        StocksTransferLinkServices stocksTransferLinkServices,
        CheckExistServices checkExistServices)
    {
        this.qneConnection = qneConnection;
        this.transferTable = transferTable;
        this.stocksTransferLinkServices = stocksTransferLinkServices;
        this.checkExistServices = checkExistServices;
    }
    //GET STOCKS LIST from db.Stocks
    public async Task<IEnumerable<Stocks>> GetStockList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Stocks WHERE IsActive != 0 ORDER BY StockCode ASC";
        var model = await connection.QueryAsync<Stocks>(query, commandType: CommandType.Text);
        return model;
    }

    // GET VW_STOCK ITEM LIST
    public async Task<IEnumerable<VW_StocksListing>> GetStocksItemList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"
            SELECT 
                [CategoryCode],
                [CategoryName],
                [LocationCode],
                [LocationName],
                [ClassCode],
                [ClassName],
                [GroupCode],
                [GroupName],
                [Id],
                [StockCode],
                [StockName],
                [StockName2],
                [Description],
                [BaseUOM],
                [MinQty],
                [MaxQty],
                [ReorderLevel],
                [ReorderQty],
                [SalesTax],
                [OpeningQty],
                [OpeningCost],
                [ListPrice],
                [MinPrice],
                [LastSellingPrice],
                [SalesDiscount],
                [PurchasePrice],
                [PurchaseDiscount],
                [LastPurchasePrice],
                [BarCode],
                [Weight],
                [WeightUOM],
                [Notes],
                [CurrentBalance],
                [LastPurchaseDate],
                [LastSellingDate],
                [IsActive],
                [IsBundled],
                [LastPurchasePriceWithLandingCost],
                [CreateDate],
                [StockControl],
                [ItemTypeCode],
                [FurtherDescription],
                [UseSerialNo],
                [SerialNoPrefix],
                [SerialNoSuffix],
                [Remark1],
                [Remark2],
                [Remark3],
                [Remark4],
                [Remark5],
                [UseBatchNo],
                [RowIndex],
                [Volumn],
                [VolumnUOM]
            FROM [dbo].[VW_Stocks]
            ORDER BY StockCode ASC";
        var model = await connection.QueryAsync<VW_StocksListing>(query, commandType: CommandType.Text);
        return model;

    }


    public async Task<IEnumerable<StockUnitPrice>> GetStocksPriceList(string ModuleName)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        //string query = string.Empty;
        var config = transferTable.GetModuleTableConfig(ModuleName);

        string query = $@"SELECT UnitPrice, StockId, Description
                  FROM {config.DetailsTable} d
                  INNER JOIN {config.HeaderTable} h ON d.{config.HeaderId} = h.Id
                  WHERE UnitPrice <> 0
                  ORDER BY h.{config.DateField} DESC";

        var model = await connection.QueryAsync<StockUnitPrice>(query);
        return model;
    }

    // GET STOCK ITEMS BY ID 
    public async Task<Stocks> GetStockItemsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM Stocks WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<Stocks>(query, new { Id });
        return model;
    }

    // SAVE OR UPDATE STOCK HEADER
    public async Task<string> SaveStockHeader(Stocks stocks)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var insertQuery = @"
                INSERT INTO Stocks (
                Id, StockCode, ItemTypeId, StockName, StockName2, Description, CategoryId, ClassId,
                GroupId, BaseUOM, MinQty, MaxQty, ReorderLevel, ReorderQty, SalesTax, SalesTaxGLAccountId,
                OpeningQty, OpeningCost, ListPrice, MinPrice, LastSellingPrice, SalesDiscount, PurchasePrice,
                PurchaseDiscount, LastPurchasePrice, BarCode, Weight, WeightUOM, Volumn, VolumnUOM, DefaultLocationId,
                Notes, CurrentBalance, LastSellingDate, LastPurchaseDate, IsBundled, LastPurchasePriceWithLandingCost,
                StockAccountPresetId, CreateDate, ItemTypeCode, FurtherDescription, DefaultInputTaxCodeId, DefaultOutputTaxCodeId,
                UseSerialNo, SerialNoPrefix, SerialNoSuffix, OptimisticLockField, ObjectType, Remark1, Remark2, Remark3, Remark4, Remark5, HSN, 
                UseBatchNo, UseDefaultUnsalablePeriod, UnsalablePeriod, CommissionGroupId, AllowNegativeStock, SSTTariffCode, TariffCodeId, 
                DiscountApplyToQty, FloorPrice, DisplayInModules, PointMgtQty, PointMgtPointsPerQty    
            ) VALUES (
                @Id, @StockCode, @ItemTypeId, @StockName, @StockName2, @Description, @CategoryId, @ClassId,
                @GroupId, @BaseUOM, @MinQty, @MaxQty, @ReorderLevel, @ReorderQty, @SalesTax, @SalesTaxGLAccountId,
                @OpeningQty, @OpeningCost, @ListPrice, @MinPrice, @LastSellingPrice, @SalesDiscount, @PurchasePrice,
                @PurchaseDiscount, @LastPurchasePrice, @BarCode, @Weight, @WeightUOM, @Volumn, @VolumnUOM, @DefaultLocationId,
                @Notes, @CurrentBalance, @LastSellingDate, @LastPurchaseDate, @IsBundled, @LastPurchasePriceWithLandingCost,
                @StockAccountPresetId, @CreateDate, @ItemTypeCode, @FurtherDescription, @DefaultInputTaxCodeId, @DefaultOutputTaxCodeId,
                @UseSerialNo, @SerialNoPrefix, @SerialNoSuffix, @OptimisticLockField, @ObjectType, @Remark1, @Remark2, @Remark3, @Remark4, @Remark5, @HSN, 
                @UseBatchNo, @UseDefaultUnsalablePeriod, @UnsalablePeriod, @CommissionGroupId, @AllowNegativeStock, @SSTTariffCode, @TariffCodeId, 
                @DiscountApplyToQty, @FloorPrice, @DisplayInModules, @PointMgtQty, @PointMgtPointsPerQty
            )";
        var updateQuery = @"
                UPDATE Stocks SET
                    StockCode = @StockCode, ItemTypeId = @ItemTypeId, StockName = @StockName, StockName2 = @StockName2, Description = @Description, CategoryId = @CategoryId, ClassId = @ClassId,
                    GroupId = @GroupId, BaseUOM = @BaseUOM, MinQty = @MinQty, MaxQty = @MaxQty, ReorderLevel = @ReorderLevel, ReorderQty = @ReorderQty, SalesTax = @SalesTax, SalesTaxGLAccountId = @SalesTaxGLAccountId,
                    OpeningQty = @OpeningQty, OpeningCost = @OpeningCost, ListPrice = @ListPrice, MinPrice = @MinPrice, LastSellingPrice = @LastSellingPrice, SalesDiscount = @SalesDiscount, PurchasePrice = @PurchasePrice,
                    PurchaseDiscount = @PurchaseDiscount, LastPurchasePrice = @LastPurchasePrice, BarCode = @BarCode, Weight = @Weight, WeightUOM = @WeightUOM, Volumn = @Volumn, VolumnUOM = @VolumnUOM, DefaultLocationId = @DefaultLocationId,
                    Notes = @Notes, CurrentBalance = @CurrentBalance, LastSellingDate = @LastSellingDate, LastPurchaseDate = @LastPurchaseDate, IsActive = @IsActive, IsBundled = @IsBundled, LastPurchasePriceWithLandingCost = @LastPurchasePriceWithLandingCost,
                    StockAccountPresetId = @StockAccountPresetId, CreateDate = @CreateDate, ItemTypeCode = @ItemTypeCode, FurtherDescription = @FurtherDescription, DefaultInputTaxCodeId = @DefaultInputTaxCodeId, DefaultOutputTaxCodeId = @DefaultOutputTaxCodeId,
                    UseSerialNo = @UseSerialNo, SerialNoPrefix = @SerialNoPrefix, SerialNoSuffix = @SerialNoSuffix, OptimisticLockField = OptimisticLockField + 1, ObjectType = @ObjectType, Remark1 = @Remark1, Remark2 = @Remark2, Remark3 = @Remark3, Remark4 = @Remark4, Remark5 = @Remark5, HSN = @HSN, 
                    UseBatchNo = @UseBatchNo, UseDefaultUnsalablePeriod = @UseDefaultUnsalablePeriod, UnsalablePeriod = @UnsalablePeriod, CommissionGroupId = @CommissionGroupId, AllowNegativeStock = @AllowNegativeStock, SSTTariffCode = @SSTTariffCode, TariffCodeId = @TariffCodeId, 
                    DiscountApplyToQty = @DiscountApplyToQty, FloorPrice = @FloorPrice, DisplayInModules = @DisplayInModules, PointMgtQty = @PointMgtQty, PointMgtPointsPerQty = @PointMgtPointsPerQty
                    WHERE Id = @Id";
        // check if the record already exists
        int affectedRows;
        var exist = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Stocks WHERE Id = @Id", new { stocks.Id }, transaction) > 0;
        try
        {
            if (!exist)
            {
                // Insert new record
                affectedRows = await connection.ExecuteAsync(insertQuery, stocks, transaction);
            }
            else
            {
                // Update existing record
                affectedRows = await connection.ExecuteAsync(updateQuery, stocks, transaction);
            }
            transaction.Commit();
            return affectedRows.ToString();


        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }


    public async Task<string> SaveMultiUOMs(List<UOMs> uoms)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var UOMsInsert = @"INSERT INTO UOMs
        (
            Id,StockId,UOMCode,Description,SalesPrice,Rate,IsBaseUOM,PurchasePrice,
            MinSalesPrice,MaxSalesPrice,MinPurchasePrice,MaxPurchasePrice,SalesDiscount,
            PurchaseDiscount,BarCode,Pos,IsActive,OptimisticLockField,IsBatchBaseUOM,IsECProduct
        )
        VALUES
        (
            @Id,@StockId,@UOMCode,@Description,@SalesPrice,@Rate,@IsBaseUOM,@PurchasePrice,
            @MinSalesPrice,@MaxSalesPrice,@MinPurchasePrice,@MaxPurchasePrice,@SalesDiscount,
            @PurchaseDiscount,@BarCode,@Pos,@IsActive,@OptimisticLockField,@IsBatchBaseUOM,@IsECProduct
        )";
        var UOMsUpdate = @"UPDATE UOMs SET
            UOMCode = @UOMCode,
            Description = @Description,
            SalesPrice = @SalesPrice,
            Rate = @Rate,
            IsBaseUOM = @IsBaseUOM,
            PurchasePrice = @PurchasePrice,
            MinSalesPrice = @MinSalesPrice,
            MaxSalesPrice = @MaxSalesPrice,
            MinPurchasePrice = @MinPurchasePrice,
            MaxPurchasePrice = @MaxPurchasePrice,
            SalesDiscount = @SalesDiscount,
            PurchaseDiscount = @PurchaseDiscount,
            BarCode = @BarCode,
            Pos = @Pos,
            IsActive = @IsActive,
            OptimisticLockField = OptimisticLockField + 1,
            IsBatchBaseUOM = @IsBatchBaseUOM,
            IsECProduct = @IsECProduct
        WHERE Id = @Id";
        object affectedrows = string.Empty;
        try
        {
            foreach (var uomslist in uoms)
            {
                var exist = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM UOMs WHERE Id = @Id", new { uomslist.Id }, transaction) > 0;
                if (!exist)
                {
                    affectedrows = await connection.ExecuteAsync(UOMsInsert, uomslist, transaction);

                }
                else
                {
                    affectedrows = await connection.ExecuteAsync(UOMsUpdate, uomslist, transaction);

                }
            }
            transaction.Commit();
            return $"Success! {affectedrows} UOms";

        }

        catch (SqlException ex)
        {
            transaction.Rollback();
            return ex.Message;
            throw;
        }


    }

    public async Task<string> SaveStockUOMLinks(StockUOMLinks stockUOMLinks)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var UOMsInsert = @"INSERT INTO StockUOMLinks
        (
            StockId,
            SalesUOMId,
            PurchaseUOMId,
            OptimisticLockField,
            SalesPackingUOMId,
            PurchasePackingUOMId
        )
        VALUES
        (
            @StockId,
            @SalesUOMId,
            @PurchaseUOMId,
            @OptimisticLockField,
            @SalesPackingUOMId,
            @PurchasePackingUOMId
        )";
        string affectedrowsMessage = string.Empty;

        try
        {
            if (stockUOMLinks.StockId is not null)
            {
                var exist = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM StockUOMLinks WHERE StockId = @StockId", new { StockId = stockUOMLinks.StockId }, transaction) > 0;
                if (!exist)
                {
                    var affectedrows = await connection.ExecuteAsync(UOMsInsert, stockUOMLinks, transaction);
                    affectedrowsMessage = affectedrows.ToString();
                }
            }

            transaction.Commit();

            return $"Success! {affectedrowsMessage} StockUOMLinks";
        }

        catch (SqlException ex)
        {
            return ex.Message;
            throw;
        }


    }


    public async Task<string> SaveStockBundleList(List<StockBundlesModel> stockBundlesModel)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        object affectedrows = string.Empty;

        var insertquery = @"
        INSERT INTO StockBundles (
            Id, StockId, SubItemId, Pos, Description,
            ProjectId, StockLocationId, Qty, UOMId,
            SalesPrice, PurchasesCost, OptimisticLockField,
            UnitPrice, UnitCost
        )
        VALUES (
            @Id, @StockId, @SubItemId, @Pos, @Description,
            @ProjectId, @StockLocationId, @Qty, @UOMId,
            @SalesPrice, @PurchasesCost, @OptimisticLockField,
            @UnitPrice, @UnitCost
        );";
        var updatequery = @"
        UPDATE StockBundles SET
            StockId = @StockId,
            SubItemId = @SubItemId,
            Pos = @Pos,
            Description = @Description,
            ProjectId = @ProjectId,
            StockLocationId = @StockLocationId,
            Qty = @Qty,
            UOMId = @UOMId,
            SalesPrice = @SalesPrice,
            PurchasesCost = @PurchasesCost,
            OptimisticLockField = OptimisticLockField + 1,
            UnitPrice = @UnitPrice,
            UnitCost = @UnitCost
        WHERE Id = @Id;";

        try
        {
            foreach (var stockBundleList in stockBundlesModel)
            {
                var exist = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM UOMs WHERE Id = @Id", new { stockBundleList.Id }, transaction) > 0;
                if (!exist)
                {
                    affectedrows = await connection.ExecuteAsync(insertquery, stockBundleList, transaction);

                }
                else
                {
                    affectedrows = await connection.ExecuteAsync(updatequery, stockBundleList, transaction);

                }
            }
            transaction.Commit();
            return $"Success! {affectedrows} UOms";

        }

        catch (SqlException ex)
        {
            transaction.Rollback();
            return ex.Message;
            throw;
        }
    }



    //GET LIST OF LOCATIOLIST 
    public async Task<IEnumerable<StockLocations>> GetStockLocationList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockLocations WHERE IsActive != 0 ORDER BY LocationCode ASC";
        var model = await connection.QueryAsync<StockLocations>(query, commandType: CommandType.Text);
        return model;
    }

    public async Task<IEnumerable<UOMs>> GetUOMsList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM UOMs WHERE IsActive != 0";
        var model = await connection.QueryAsync<UOMs>(query, commandType: CommandType.Text);
        return model;

    }

    // GET STOCK CATEGORIES LIST
    public async Task<IEnumerable<VW_StockCategoriesListing>> GetStockCategories()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockCategories ORDER BY CategoryCode ASC";
        var model = await connection.QueryAsync<VW_StockCategoriesListing>(query, commandType: CommandType.Text);
        return model;

    }

    // GET STOCK CATEGORIES BY ID (HEADER AUTOFILL DATA)
    public async Task<StockCategoriesModel> GetStockCategoriesById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockCategories WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<StockCategoriesModel>(query, new { Id });
        return model;
    }

    // SAVE OR UPDATE STOCK CATEGORY HEADER
    public async Task<Guid> SaveStockCategory(StockCategoriesModel header)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        {
            // check if the record already exists
            var existing = await connection.QueryFirstOrDefaultAsync<StockCategoriesModel>(
                "SELECT Id FROM StockCategories WHERE Id = @Id", new
                {
                    header.Id
                });

            if (existing == null)
            {
                // Insert new record
                var insertQuery = @"
            INSERT INTO StockCategories (
                Id, CategoryCode, Description, IsActive, OptimisticLockField    
            ) VALUES (
                @Id, @CategoryCode, @Description, @IsActive, @OptimisticLockField   
            )";
                await connection.ExecuteAsync(insertQuery, header);
            }
            else
            {
                // Update existing record
                var updateQuery = @"
            UPDATE StockCategories SET
            CategoryCode = @CategoryCode,
            Description = @Description,
            IsActive = @IsActive,
            OptimisticLockField = @OptimisticLockField    
            WHERE Id = @Id";
                await connection.ExecuteAsync(updateQuery, header);
            }
        }

        return header.Id;
    }

    // GET STOCK GROUP LIST 
    public async Task<IEnumerable<VW_StockGroupsListing>> GetStockGroups()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockGroups ORDER BY GroupCode ASC";
        var model = await connection.QueryAsync<VW_StockGroupsListing>(query, commandType: CommandType.Text);
        return model;
    }

    // GET STOCK GROUPS BY ID (HEADER AUTOFILL DATA)
    public async Task<StockGroupsModel> GetStockGroupsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockGroups WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<StockGroupsModel>(query, new { Id });
        return model;
    }

    // SAVE OR UPDATE STOCK GROUPS HEADER
    public async Task<Guid> SaveStockGroup(StockGroupsModel header)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        {
            // check if the record already exists
            var existing = await connection.QueryFirstOrDefaultAsync<StockGroupsModel>(
                "SELECT Id FROM StockGroups WHERE Id = @Id", new
                {
                    header.Id
                });

            if (existing == null)
            {
                // Insert new record
                var insertQuery = @"
            INSERT INTO StockGroups (
                Id, GroupCode, Description, IsActive, OptimisticLockField    
            ) VALUES (
                @Id, @GroupCode, @Description, @IsActive, @OptimisticLockField   
            )";
                await connection.ExecuteAsync(insertQuery, header);
            }
            else
            {
                // Update existing record
                var updateQuery = @"
            UPDATE StockGroups SET
            GroupCode = @GroupCode,
            Description = @Description,
            IsActive = @IsActive,
            OptimisticLockField = @OptimisticLockField    
            WHERE Id = @Id";
                await connection.ExecuteAsync(updateQuery, header);
            }
        }

        return header.Id;
    }


    // GET STOCK CLASSES LIST
    public async Task<IEnumerable<VW_StockClassesListing>> GetStockClasses()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockClasses ORDER BY ClassCode ASC";
        var model = await connection.QueryAsync<VW_StockClassesListing>(query, commandType: CommandType.Text);
        return model;
    }


    // GET STOCK CLASSES BY ID (HEADER AUTOFILL DATA)
    public async Task<StockClassesModel> GetStockClassesById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockClasses WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<StockClassesModel>(query, new { Id });
        return model;
    }


    // SAVE OR UPDATE STOCK CLASSES HEADER
    public async Task<Guid> SaveStockClasses(StockClassesModel header)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        {
            // check if the record already exists
            var existing = await connection.QueryFirstOrDefaultAsync<StockClassesModel>(
                "SELECT Id FROM StockClasses WHERE Id = @Id", new
                {
                    header.Id
                });

            if (existing == null)
            {
                // Insert new record
                var insertQuery = @"
            INSERT INTO StockClasses (
                Id, ClassCode, Description, IsActive, OptimisticLockField    
            ) VALUES (
                @Id, @ClassCode, @Description, @IsActive, @OptimisticLockField   
            )";
                await connection.ExecuteAsync(insertQuery, header);
            }
            else
            {
                // Update existing record
                var updateQuery = @"
            UPDATE StockClasses SET
            ClassCode = @ClassCode,
            Description = @Description,
            IsActive = @IsActive,
            OptimisticLockField = @OptimisticLockField    
            WHERE Id = @Id";
                await connection.ExecuteAsync(updateQuery, header);
            }
        }
        return header.Id;
    }

    //GET LIST OF LOCATIOLIST 
    public async Task<IEnumerable<VW_StockLocationsListing>> GetStocksLocationList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM VW_StockLocations ORDER BY LocationCode ASC";
        var model = await connection.QueryAsync<VW_StockLocationsListing>(query, commandType: CommandType.Text);
        return model;
    }

    // GET STOCK LOCATION BY ID (HEADER AUTOFILL DATA)
    public async Task<StockLocationsModel> GetStockLocationsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockLocations WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<StockLocationsModel>(query, new { Id });
        return model;
    }



    // SAVE OR UPDATE STOCK LOCATION HEADER
    public async Task<Guid> SaveStockLocation(StockLocationsModel header)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        {
            // check if the record already exists
            var existing = await connection.QueryFirstOrDefaultAsync<StockLocationsModel>(
                "SELECT Id FROM StockLocations WHERE Id = @Id", new
                {
                    header.Id
                });

            if (existing == null)
            {
                // Insert new record
                var insertQuery = @"
            INSERT INTO StockLocations (
                Id, LocationCode, Description, Address1, Address2, Address3, Address4, Contact, PhoneNo, FaxNo, IsActive, OptimisticLockField    
            ) VALUES (
                @Id, @LocationCode, @Description, @Address1, @Address2, @Address3, @Address4, @Contact, @PhoneNo, @FaxNo, @IsActive, @OptimisticLockField   
            )";
                await connection.ExecuteAsync(insertQuery, header);
            }
            else
            {
                // Update existing record
                var updateQuery = @"
            UPDATE StockLocations SET
            LocationCode = @LocationCode,
            Description = @Description,
            Address1 = @Address1,
            Address2 = @Address2,
            Address3 = @Address3,
            Address4 = @Address4,
            Contact = @Contact,
            PhoneNo = @PhoneNo,
            FaxNo = @FaxNo,
            IsActive = @IsActive,
            OptimisticLockField = @OptimisticLockField    
            WHERE Id = @Id";
                await connection.ExecuteAsync(updateQuery, header);
            }
        }
        return header.Id;
    }


    // GET STOCK BATCH NUMBER BY ID (HEADER AUTOFILL DATA)
    public async Task<StockBatchNumbersModel> GetStockBatchNumbersById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockBatchNumbers WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<StockBatchNumbersModel>(query, new { Id });
        return model;
    }

    // SAVE OR UPDATE STOCK BATCH NUMBER HEADER
    public async Task<Guid> SaveStockBatchNo(StockBatchNumbersModel header)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        {
            // check if the record already exists
            var existing = await connection.QueryFirstOrDefaultAsync<StockBatchNumbersModel>(
                "SELECT Id FROM StockBatchNumbers WHERE Id = @Id", new
                {
                    header.Id
                });

            if (existing == null)
            {
                // Insert new record
                var insertQuery = @"
            INSERT INTO StockBatchNumbers (
                Id, StockId, BatchNo, Description, Pos, BatchNoManufacturingDate, BatchNoExpiryDate, BalanceQty, OptimisticLockField, BatchNoRef1, BatchNoRef2   
            ) VALUES (
                @Id, @StockId, @BatchNo, @Description, @Pos, @BatchNoManufacturingDate, @BatchNoExpiryDate, @BalanceQty, @OptimisticLockField, @BatchNoRef1, @BatchNoRef2   
            )";
                await connection.ExecuteAsync(insertQuery, header);
            }
            else
            {
                // Update existing record
                var updateQuery = @"
            UPDATE StockBatchNumbers SET
            StockId = @StockId,
            BatchNo = @BatchNo,
            Description = @Description,
            Pos = Pos,
            BatchNoManufacturingDate = @BatchNoManufacturingDate,
            BatchNoExpiryDate = @BatchNoExpiryDate,
            OptimisticLockField = @OptimisticLockField,
            BatchNoRef1 = @BatchNoRef1, 
            BatchNoRef2 = @BatchNoRef2   
            WHERE Id = @Id";
                await connection.ExecuteAsync(updateQuery, header);
            }
        }
        return header.Id;
    }

    // GET STOCK BATCH NUMBERS LIST
    public async Task<IEnumerable<StockBatchNumbersModel>> GetStockBatchNumberList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"
            SELECT 
                StockBatchNumbers.Id,
                StockBatchNumbers.StockId,
                Stocks.StockCode,
                StockBatchNumbers.BatchNo,
                StockBatchNumbers.Description,
                StockBatchNumbers.Pos,
                StockBatchNumbers.BatchNoManufacturingDate,
                StockBatchNumbers.BatchNoExpiryDate,
                StockBatchNumbers.BalanceQty,
                StockBatchNumbers.OptimisticLockField,
                StockBatchNumbers.BatchNoRef1,
                StockBatchNumbers.BatchNoRef2
            FROM StockBatchNumbers
            LEFT OUTER JOIN Stocks ON Stocks.Id = StockBatchNumbers.StockId";
        var model = await connection.QueryAsync<StockBatchNumbersModel>(query, commandType: CommandType.Text);
        return model;
    }

    public async Task<IEnumerable<StockBatchNumbersModel>>
    GetAvailableStockBatchNumbers(Guid stockId)
    {
        var connectionString =
            await qneConnection.GetConnectionStringAsync();

        using var connection =
            new SqlConnection(connectionString);

        var query = @"
            SELECT
                sbn.Id,
                sbn.StockId,
                sbn.BatchNo,
                sbn.Description,
                sbn.Pos,
                sbn.BatchNoManufacturingDate,
                sbn.BatchNoExpiryDate,
                sbn.BalanceQty,
                sbn.BatchNoRef1,
                sbn.BatchNoRef2
            FROM StockBatchNumbers sbn
            WHERE sbn.StockId = @StockId
            AND ISNULL(sbn.BalanceQty,0) > 0
            ORDER BY sbn.BatchNo";

        return await connection.QueryAsync<StockBatchNumbersModel>(
            query,
            new { StockId = stockId });
    }

    public async Task<string> GetLocationCode(Guid locationId)
    {
        var connectionString =
                await qneConnection.GetConnectionStringAsync();

        using var connection =
            new SqlConnection(connectionString);

        return await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT LocationCode FROM StockLocations WHERE Id = @Id",
            new { Id = locationId });
    }

    public async Task<string> GetUOMCode(Guid uomId)
    {
        var connectionString =
                await qneConnection.GetConnectionStringAsync();

        using var connection =
            new SqlConnection(connectionString);

        return await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT UOMCode FROM UOMs WHERE Id = @Id",
            new { Id = uomId });
    }

    // GET STOCK ACCOUNT PRESET LIST
    public async Task<IEnumerable<VW_StockAccountPresetListing>> GetStockAccountPresets()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockAccountPresets ORDER BY PresetCode ASC";
        var model = await connection.QueryAsync<VW_StockAccountPresetListing>(query, commandType: CommandType.Text);
        return model;
    }


    // GET STOCK ACCOUNT PRESET BY ID (HEADER AUTOFILL DATA)
    public async Task<VW_StockAccountPresetListing> GetStocksAccountPresetById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM VW_StockAccountPresetListing WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<VW_StockAccountPresetListing>(query, new { Id });
        return model;
    }


    // GET STOCKS ACCOUNT PRESET BY ID
    public async Task<IEnumerable<Stocks>> GetStocksByPresetId(Guid? presetId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"
           SELECT 
               s.StockCode,
               s.StockName,
               ISNULL(s.CurrentBalance, 0) AS CurrentBalance
           FROM Stocks s
           WHERE s.StockAccountPresetId = @presetId
           ORDER BY s.StockCode DESC";
        var result = await connection.QueryAsync<Stocks>(query, new { presetId });
        return result;

    }


    // SAVE OR UPDATE STOCK ACCOUNT PRESET HEADER
    public async Task<Guid> SaveStockAccountPreset(StockAccountPresetsModel header)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        {
            // check if the record already exists
            var existing = await connection.QueryFirstOrDefaultAsync<StockAccountPresetsModel>(
                "SELECT Id FROM StockAccountPresets WHERE Id = @Id", new
                {
                    header.Id
                });

            if (existing == null)
            {
                // Insert new record
                var insertQuery = @"
            INSERT INTO StockAccountPresets (
                Id, PresetCode, Description, SalesAccountId, SalesReturnAccountId, 
                CashSalesAccountId, PurchaseAccountId, PurchaseReturnAccountId, 
                SalesDiscountAccountId, PurchaseDiscountAccountId, SalesDiscountReturnAccountId, 
                PurchaseDiscountReturnAccountId, CostOfSalesAccountId, IsDefault, 
                OptimisticLockField, AccruedPurchaseAccountId
            ) 
            VALUES (
                @Id, @PresetCode, @Description, @SalesAccountId, @SalesReturnAccountId, 
                @CashSalesAccountId, @PurchaseAccountId, @PurchaseReturnAccountId, 
                @SalesDiscountAccountId, @PurchaseDiscountAccountId, @SalesDiscountReturnAccountId, 
                @PurchaseDiscountReturnAccountId, @CostOfSalesAccountId, @IsDefault, 
                @OptimisticLockField, @AccruedPurchaseAccountId
            )";
                await connection.ExecuteAsync(insertQuery, header);
            }
            else
            {
                // Update existing record
                var updateQuery = @"
            UPDATE StockAccountPresets SET
            PresetCode = @PresetCode,
            Description = @Description,
            SalesAccountId = @SalesAccountId,
            SalesReturnAccountId = @SalesReturnAccountId, 
            CashSalesAccountId = @CashSalesAccountId,
            PurchaseAccountId = @PurchaseAccountId,
            PurchaseReturnAccountId = @PurchaseReturnAccountId,
            SalesDiscountAccountId = @SalesDiscountAccountId,
            PurchaseDiscountAccountId = @PurchaseDiscountAccountId, 
            SalesDiscountReturnAccountId = @SalesDiscountReturnAccountId,
            PurchaseDiscountReturnAccountId = @PurchaseDiscountReturnAccountId,
            CostOfSalesAccountId = @CostOfSalesAccountId,
            IsDefault = @IsDefault,
            OptimisticLockField = @OptimisticLockField,
            AccruedPurchaseAccountId = @AccruedPurchaseAccountId
            WHERE Id = @Id";
                await connection.ExecuteAsync(updateQuery, header);
            }
        }

        return header.Id;
    }

    public async Task<IEnumerable<UOMs>> GetMultiUOMsById(string StockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM UOMs WHERE StockId = @StockId ORDER BY Pos ASC";
        var model = await connection.QueryAsync<UOMs>(query, new { StockId });
        return model;
    }
    public async Task<IEnumerable<StockBundlesModel>> GetStockBundlesById(string StockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockBundles WHERE StockId = @StockId ORDER BY Pos ASC";
        var model = await connection.QueryAsync<StockBundlesModel>(query, new { StockId });
        return model;
    }

    public async Task<IEnumerable<StockBundlesModel>> GetStockBundlesList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockBundles ORDER BY Pos ASC";
        var model = await connection.QueryAsync<StockBundlesModel>(query);
        return model;
    }
    public async Task<StockPurchasePriceHistoryModel> GetLastPurchasePriceById(string StockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockPurchasePriceHistory WHERE Stock = @StockId ORDER BY DocDate ASC";
        var model = await connection.QueryFirstOrDefaultAsync<StockPurchasePriceHistoryModel>(query, new { StockId });
        return model;
    }

    public async Task<StockSalesPriceHistoryModel> GetLastSellingPriceById(string StockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockSalesPriceHistory WHERE Stock = @StockId ORDER BY DocDate DESC";
        var model = await connection.QueryFirstOrDefaultAsync<StockSalesPriceHistoryModel>(query, new { StockId });
        return model;

    }


    // GET STOCK OPENING BALANCE 
    public async Task<IEnumerable<StockOpeningBalanceModel>> GetStockOpeningBalance()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockOB ORDER BY StockOBCode ASC";
        var model = await connection.QueryAsync<StockOpeningBalanceModel>(query, commandType: CommandType.Text);
        return model;

    }

    //GET STOCK OPENING BALANCE DATA BY ID
    public async Task<StockOpeningBalanceModel> GetStockOpeningBalanceById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockOB WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<StockOpeningBalanceModel>(query, new { Id });
        return model;

    }


    // GET STOCK OPENING BALANCE DETAILS DATA BY ID
    public async Task<IEnumerable<StockOpeningBalanceDetails>> GetStockOpeningBalanceDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockOBDetails WHERE StockOBId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<StockOpeningBalanceDetails>(query, new { Id });
        return model;
    }

    // GET STOCK ADJUSTMENT LIST
    public async Task<IEnumerable<StockAdjustmentsModel>> GetStockAdjustmentList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var query = @"
        SELECT

            SA.*,

            FCU.UserName AS FirstCreatedUser,
            DI.FirstCreatedDate,

            LMU.UserName AS LastModifiedUser,
            DI.LastModifiedDate,

            DI.PrintCount

        FROM StockAdjustments SA

        LEFT JOIN DocumentInfo DI
            ON SA.Id = DI.DocumentId
            AND DI.DocType = 'StockAdjustments'

        LEFT JOIN SecuritySystemUser FCU
            ON DI.FirstCreatedUserId = FCU.Oid

        LEFT JOIN SecuritySystemUser LMU
            ON DI.LastModifiedUserId = LMU.Oid

        ORDER BY
            SA.StockAdjustmentDate DESC
        ";

        var model = await connection.QueryAsync<StockAdjustmentsModel>(
            query,
            commandType: CommandType.Text
        );

        return model;
    }

    //GET STOCK ADJUSTMENT DATA BY ID
    public async Task<StockAdjustmentsModel> GetStockAdjustmentById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockAdjustments WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<StockAdjustmentsModel>(query, new { Id });
        return model;

    }

    // GET STOCK ADJUSTMENT DETAILS DATA BY ID
    public async Task<IEnumerable<StockAdjustmentDetails>> GetStockAdjustmentDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockAdjustmentDetails WHERE StockAdjustmentId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<StockAdjustmentDetails>(query, new { Id });
        return model;
    }


    // SAVE OR UPDATE STOCK ADJUSTMENT HEADER 
    public async Task<string> SaveStockAdjustmentHeader(
        StockAdjustmentsModel stockadjustmentModel,
        Guid currentUserId)   // add current user id parameter
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var insertheaderquery = @"INSERT INTO StockAdjustments (
        Id, StockAdjustmentCode, StockAdjustmentDate, Description, ReferenceNo, Title,
        Ref1, Ref2, Remark1, Remark2, ProjectId, TotalAmount, IsCancelled, StockLocationId,
        CostCentreId, Notes, OptimisticLockField, Title2, DimensionId    
    ) VALUES (
        @Id, @StockAdjustmentCode, @StockAdjustmentDate, @Description, @ReferenceNo, @Title,
        @Ref1, @Ref2, @Remark1, @Remark2, @ProjectId, @TotalAmount, @IsCancelled, @StockLocationId,
        @CostCentreId, @Notes, @OptimisticLockField, @Title2, @DimensionId)";

        var updateheadquery = @"UPDATE StockAdjustments SET

        StockAdjustmentCode=@StockAdjustmentCode,
        StockAdjustmentDate=@StockAdjustmentDate,
        Description=@Description, 
        ReferenceNo=@ReferenceNo,
        Title=@Title,
        Ref1=@Ref1,
        Ref2=@Ref2,
        Remark1=@Remark1,
        Remark2=@Remark2,
        ProjectId=@ProjectId, 
        TotalAmount=@TotalAmount,
        IsCancelled=@IsCancelled,
        StockLocationId=@StockLocationId,
        CostCentreId=@CostCentreId,
        Notes=@Notes,
        OptimisticLockField=@OptimisticLockField,
        Title2=@Title2,
        DimensionId=@DimensionId

    WHERE Id=@Id";

        object affectedRows = string.Empty;

        // DOCUMENT INFO INSERT
        var insertDocumentInfo = @"

        INSERT INTO DocumentInfo
        (
            Id,
            DocumentId,
            DocType,
            FirstCreatedUserId,
            FirstCreatedDate,
            LastModifiedUserId,
            LastModifiedDate,
            PrintCount
        )

        VALUES
        (
            NEWID(),
            @DocumentId,
            'StockAdjustments',
            @UserId,
            GETDATE(),
            @UserId,
            GETDATE(),
            0
        )
    ";

        // DOCUMENT INFO UPDATE
        var updateDocumentInfo = @"

        UPDATE DocumentInfo

        SET

            LastModifiedUserId=@UserId,
            LastModifiedDate=GETDATE()

        WHERE

            DocumentId=@DocumentId

        AND

            DocType='StockAdjustments'

    ";

        // GENERATE ID FOR NEW HEADER
        if (stockadjustmentModel.Id == Guid.Empty)
        {
            stockadjustmentModel.Id = Guid.NewGuid();
        }

        var existheader =
            await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM StockAdjustments WHERE Id=@Id",
                new { stockadjustmentModel.Id },
                transaction) > 0;

        try
        {
            if (!existheader)
            {
                // SAVE HEADER
                affectedRows =
                    await connection.ExecuteAsync(
                        insertheaderquery,
                        stockadjustmentModel,
                        transaction);

                // INSERT DOCUMENT INFO
                await connection.ExecuteAsync(
                    insertDocumentInfo,
                    new
                    {
                        DocumentId = stockadjustmentModel.Id,
                        UserId = currentUserId
                    },
                    transaction);
            }
            else
            {
                // UPDATE HEADER
                affectedRows =
                    await connection.ExecuteAsync(
                        updateheadquery,
                        stockadjustmentModel,
                        transaction);

                // UPDATE DOCUMENT INFO
                await connection.ExecuteAsync(
                    updateDocumentInfo,
                    new
                    {
                        DocumentId = stockadjustmentModel.Id,
                        UserId = currentUserId
                    },
                    transaction);
            }

            transaction.Commit();

            return $"Success {affectedRows} affected";
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            return $"Error: {ex.Message}";
        }
    }

    // SAVE OR UPDATE STOCK ADJUSTMENT DETAILS
    public async Task<string> SaveStockAdjustmentDetails(List<StockAdjustmentDetails> stockadjustmentsDetailsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertdetailsquery = @"INSERT INTO StockAdjustmentDetails (
            Id, StockAdjustmentId, Pos, Numbering, StockId, Description, UOMId, Price,
            Qty, Amount, ProjectId, StockLocationId, CostCentreId, Note, ItemTypeCode,
            ReferenceNo, StockType, OptimisticLockField, StockBatchNumberId, IsBundled,
            IsSubItem, BundledTransactionId, SerialNumber, SerialNumberRef1, SerialNumberRef2,
            PackingUOMId, Packing, PackingQty, Ref1, Ref2, Ref3, Ref4, Ref5, PackingUOM1,
            PackingUOM2, HighestUOM, CostA, CostB, CostC, CostD, StockAgingDate, DimensionId
        ) VALUES (
            @Id, @StockAdjustmentId, @Pos, @Numbering, @StockId, @Description, @UOMId, @Price,
            @Qty, @Amount, @ProjectId, @StockLocationId, @CostCentreId, @Note, @ItemTypeCode,
            @ReferenceNo, @StockType, @OptimisticLockField, @StockBatchNumberId, @IsBundled,
            @IsSubItem, @BundledTransactionId, @SerialNumber, @SerialNumberRef1, @SerialNumberRef2,
            @PackingUOMId, @Packing, @PackingQty, @Ref1, @Ref2, @Ref3, @Ref4, @Ref5, @PackingUOM1,
            @PackingUOM2, @HighestUOM, @CostA, @CostB, @CostC, @CostD, @StockAgingDate, @DimensionId
        )";
        var updatedetailsquery = @"UPDATE StockAdjustmentDetails SET
            StockAdjustmentId = @StockAdjustmentId,Pos = @Pos,Numbering = @Numbering,StockId = @StockId,
            Description = @Description,UOMId = @UOMId,Price = @Price,Qty = @Qty,Amount = @Amount,ProjectId = @ProjectId,
            StockLocationId = @StockLocationId,CostCentreId = @CostCentreId,Note = @Note,ItemTypeCode = @ItemTypeCode,
            ReferenceNo = @ReferenceNo,StockType = @StockType,OptimisticLockField = @OptimisticLockField,
            StockBatchNumberId = @StockBatchNumberId,IsBundled = @IsBundled,IsSubItem = @IsSubItem,BundledTransactionId = @BundledTransactionId,
            SerialNumber = @SerialNumber,SerialNumberRef1 = @SerialNumberRef1,SerialNumberRef2 = @SerialNumberRef2,
            PackingUOMId = @PackingUOMId,Packing = @Packing,PackingQty = @PackingQty,Ref1 = @Ref1,Ref2 = @Ref2,Ref3 = @Ref3,
            Ref4 = @Ref4,Ref5 = @Ref5,PackingUOM1 = @PackingUOM1,PackingUOM2 = @PackingUOM2,HighestUOM = @HighestUOM,
            CostA = @CostA,CostB = @CostB,CostC = @CostC,CostD = @CostD,StockAgingDate = @StockAgingDate,DimensionId = @DimensionId
        WHERE Id = @Id";
        object detailsMessage = string.Empty;
        try
        {
            foreach (var stockadjustmentsDetails in stockadjustmentsDetailsList)
            {
                var existheader = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM StockAdjustmentDetails WHERE Id = @Id",
                    new { stockadjustmentsDetails.Id },
                    transaction) > 0;

                if (!existheader)
                {
                    detailsMessage = await connection.ExecuteAsync(insertdetailsquery, stockadjustmentsDetails, transaction);
                }
                else
                {
                    detailsMessage = await connection.ExecuteAsync(updatedetailsquery, stockadjustmentsDetails, transaction);
                }
            }
            transaction.Commit();
            return $"Success {detailsMessage}";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return ex.Message;
            throw;
        }
    }


    // STOCK ADJUSTMENT TRANSACTION
    public async Task<string> SaveStockAdjustmentTransactions(
    StockAdjustmentsModel header,
    List<StockAdjustmentDetails> details,
    Guid currentUserId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertQuery = @"
        INSERT INTO StockTransactions (
            Id, StockId, TransactionDate, TransactionTime, DocumentCode, DocType,
            Description, ItemDescription, Qty, Amount, NetAmount, AmountLocal,
            NetAmountLocal, TaxExclusiveAmountLocal, UOMQty, UOMPrice, DiscountAmount,
            UOMPriceExcLandingCost, LocationId, Projectid, CostCentreId, IsCancelled,
            DocumentId, UOMId, IsBundled
        ) VALUES (
            @Id, @StockId, @TransactionDate, @TransactionTime, @DocumentCode, @DocType,
            @Description, @ItemDescription, @Qty, @Amount, @NetAmount, @AmountLocal,
            @NetAmountLocal, @TaxExclusiveAmountLocal, @UOMQty, @UOMPrice, @DiscountAmount,
            @UOMPriceExcLandingCost, @LocationId, @Projectid, @CostCentreId, @IsCancelled,
            @DocumentId, @UOMId, @IsBundled
        )";

        try
        {
            foreach (var d in details)
            {
                if (d.StockId is null || d.Qty is null || d.Qty == 0)
                    continue; // skip blank rows

                var qty = d.Qty.Value;
                var price = d.Price ?? 0m;
                var amount = qty * price;

                await connection.ExecuteAsync(insertQuery, new
                {
                    Id = Guid.NewGuid(),
                    StockId = d.StockId,
                    TransactionDate = header.StockAdjustmentDate,
                    TransactionTime = DateTime.Now.TimeOfDay,
                    DocumentCode = header.StockAdjustmentCode,
                    DocType = "StockAdjustments",
                    Description = header.Description,
                    ItemDescription = d.Description,
                    Qty = qty,
                    Amount = amount,
                    NetAmount = amount,
                    AmountLocal = amount,
                    NetAmountLocal = amount,
                    TaxExclusiveAmountLocal = amount,
                    UOMQty = qty,
                    UOMPrice = price,
                    DiscountAmount = 0m,
                    UOMPriceExcLandingCost = price,
                    LocationId = d.StockLocationId ?? header.StockLocationId,
                    Projectid = d.ProjectId ?? header.ProjectId,
                    CostCentreId = d.CostCentreId,
                    IsCancelled = false,
                    DocumentId = header.Id,
                    UOMId = d.UOMId,
                    IsBundled = d.IsBundled ?? false,   // fall back to false if not set on the detail row
                }, transaction);
            }

            transaction.Commit();
            return "Success: StockTransactions synced";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }

    public async Task<Guid> GetSecurityUserOid(string username)
    {
        using var connection =
            new SqlConnection(await qneConnection.GetConnectionStringAsync());

        return await connection.ExecuteScalarAsync<Guid>(
        @"SELECT Oid
          FROM SecuritySystemUser
          WHERE UserName=@username",
          new { username });
    }

    // Display automatic Unit Price table of (GoodsReceivedNoteDetails ) 
    // USED ON STOCK ADJUSTMENT MODULE
    // AUG 6, 2026
    // Stock Adjustment Module 
    public async Task<decimal> GetLatestPurchaseUnitPrice(Guid stockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);

        var query = @"
            SELECT TOP 1 
             d.UnitPrice
         FROM GoodsReceivedNoteDetails D
         INNER JOIN GoodsReceivedNotes H ON D.GoodsReceivedNoteId = H.Id
         WHERE d.StockId = @StockId
         ORDER BY H.GoodsReceivedNoteDate DESC";

        return await connection.ExecuteScalarAsync<decimal?>(
            query,
            new { stockId }
        ) ?? 0;

    }

    // GET ID, NAMES AND FORMAT TRANSACTION CODES (STOCK INS)
    public async Task<IEnumerable<TransactionsCode>> GetStockTransactionCodeStockIn(int TableId)
    {
        // assign proper parameter for query of specific stock code
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"SELECT *
             FROM TransactionCodes
             WHERE TableId = @TableId
             ORDER BY Name ASC;";

        var model = await connection.QueryAsync<TransactionsCode>(query, new { TableId = TableId });
        return model;
    }

    // GET STOCK INS BY ID
    public async Task<StockInsModel> GetStockInsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockIns WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<StockInsModel>(query, new { Id });
        return model;
    }

    // GET STOCK INS DETAILS DATA BY ID
    public async Task<IEnumerable<StockInDetailsModel>> GetStockInsDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"
                    SELECT * FROM StockInDetails 
                    WHERE StockInId LIKE @Id 
                    ORDER BY Pos ASC";
        var model = await connection.QueryAsync<StockInDetailsModel>(query, new { Id });
        return model;
    }

    // GET STOCK INS LIST
    public async Task<IEnumerable<StockInsModel>> GetStockInList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"
                    SELECT
                        SI.*,

                        FCU.UserName AS FirstCreatedUser,
                        DI.FirstCreatedDate,

                        LMU.UserName AS LastModifiedUser,
                        DI.LastModifiedDate,

                        DI.PrintCount

                    FROM StockIns SI

                    LEFT JOIN DocumentInfo DI
                        ON SI.Id = DI.DocumentId
                        AND DI.DocType = 'StockIns'

                    LEFT JOIN SecuritySystemUser FCU
                        ON DI.FirstCreatedUserId = FCU.Oid

                    LEFT JOIN SecuritySystemUser LMU
                        ON DI.LastModifiedUserId = LMU.Oid

                    ORDER BY 
                    
                        SI.StockInDate DESC,
                        SI.StockInCode ASC,
                        SI.Description DESC
                    ";

        var model = await connection.QueryAsync<StockInsModel>(query, commandType: CommandType.Text);
        return model;
    }

    // SAVE OR UPDATE STOCK INS HEADER
    public async Task<string> SaveStockInsHeader(StockInsModel stockinModel)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var insertheader = @"INSERT INTO StockIns (
            Id, StockInCode, StockInDate, Description, ReferenceNo, Title, Ref1,
            Ref2, Remark1, Remark2, ProjectId, TotalAmount, IsCancelled, StockLocationId,
            CostCentreId, Notes, OptimisticLockField, IsProduction, TransferFromInfo,
            Title2, DimensionId
        ) VALUES (
            @Id, @StockInCode, @StockInDate, @Description, @ReferenceNo, @Title, @Ref1,
            @Ref2, @Remark1, @Remark2, @ProjectId, @TotalAmount, @IsCancelled, @StockLocationId,
            @CostCentreId, @Notes, @OptimisticLockField, @IsProduction, @TransferFromInfo,
            @Title2, @DimensionId)";

        var updateheader = @"UPDATE StockIns SET
            StockInCode=@StockInCode, StockInDate=@StockInDate, Description=@Description, ReferenceNo=@ReferenceNo, Title=@Title, Ref1=@Ref1,
            Ref2=@Ref2, Remark1=@Remark1, Remark2=@Remark2, ProjectId=@ProjectId, TotalAmount=@TotalAmount, IsCancelled=@IsCancelled, StockLocationId=@StockLocationId,
            CostCentreId=@CostCentreId, Notes=@Notes, OptimisticLockField=@OptimisticLockField, IsProduction=@IsProduction, TransferFromInfo=@TransferFromInfo,
            Title2=@Title2, DimensionId=@DimensionId

        WHERE Id=@Id";

        object affectedRows = string.Empty;

        // GENERATE ID FOR NEW HEADER
        if (stockinModel.Id == Guid.Empty)
        {
            stockinModel.Id = Guid.NewGuid();
        }

        var existheader =
            await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM StockIns WHERE Id=@Id",
                new { stockinModel.Id },
                transaction) > 0;
        try
        {
            if (!existheader)
            {
                // SAVE HEADER
                affectedRows = await connection.ExecuteAsync(insertheader, stockinModel, transaction);
            }
            else
            {
                // UPDATE HEADER
                affectedRows = await connection.ExecuteAsync(updateheader, stockinModel, transaction);
            }

            transaction.Commit();

            return $"Success {affectedRows} affected";
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            return $"Error: {ex.Message}";
        }
    }

    // SAVE OR UPDATE STOCK INS DETAILS 
    public async Task<string> SaveStockInDetails(List<StockInDetailsModel> stockinDetailsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertdetails = @"INSERT INTO StockInDetails (
            Id, StockInId, Pos, Numbering, StockId, Description, UOMId, Price,
            Qty, Amount, ProjectId, StockLocationId, CostCentreId, Note, ItemTypeCode,
            StockType, OptimisticLockField, StockBatchNumberId, TransferLink, IsBundled,
            IsSubItem, BundledTransactionId, SerialNumber, SerialNumberRef1, SerialNumberRef2,
            PackingUOMId, Packing, PackingQty, Ref1, Ref2, Ref3, Ref4, Ref5, PackingUOM1,
            PackingUOM2, HighestUOM, CostA, CostB, CostC, CostD, RefTransferFrom, StockAgingDate,
            DimensionId
        ) VALUES (
            @Id, @StockInId, @Pos, @Numbering, @StockId, @Description, @UOMId, @Price,
            @Qty, @Amount, @ProjectId, @StockLocationId, @CostCentreId, @Note, @ItemTypeCode,
            @StockType, @OptimisticLockField, @StockBatchNumberId, @TransferLink, @IsBundled,
            @IsSubItem, @BundledTransactionId, @SerialNumber, @SerialNumberRef1, @SerialNumberRef2,
            @PackingUOMId, @Packing, @PackingQty, @Ref1, @Ref2, @Ref3, @Ref4, @Ref5, @PackingUOM1,
            @PackingUOM2, @HighestUOM, @CostA, @CostB, @CostC, @CostD, @RefTransferFrom, @StockAgingDate,
            @DimensionId
        )";
        var updatedetails = @"UPDATE StockInDetails SET
            StockInId=@StockInId, Pos=@Pos, Numbering=@Numbering, StockId=@StockId, Description=@Description, UOMId=@UOMId, Price=@Price,
            Qty=@Qty, Amount=@Amount, ProjectId=@ProjectId, StockLocationId=@StockLocationId, CostCentreId=@CostCentreId, Note=@Note, ItemTypeCode=@ItemTypeCode,
            StockType=@StockType, OptimisticLockField=@OptimisticLockField, StockBatchNumberId=@StockBatchNumberId, TransferLink=@TransferLink, IsBundled=@IsBundled,
            IsSubItem=@IsSubItem, BundledTransactionId=@BundledTransactionId, SerialNumber=@SerialNumber, SerialNumberRef1=@SerialNumberRef1, SerialNumberRef2=@SerialNumberRef2,
            PackingUOMId=@PackingUOMId, Packing=@Packing, PackingQty=@PackingQty, Ref1=@Ref1, Ref2=@Ref2, Ref3=@Ref3, Ref4=@Ref4, Ref5=@Ref5, PackingUOM1=@PackingUOM1,
            PackingUOM2=@PackingUOM2, HighestUOM=@HighestUOM, CostA=@CostA, CostB=@CostB, CostC=@CostC, CostD=@CostD, RefTransferFrom=@RefTransferFrom, StockAgingDate=@StockAgingDate,
            DimensionId=@DimensionId
        WHERE Id = @Id";
        object detailsMessage = string.Empty;
        try
        {
            foreach (var stockinsDetails in stockinDetailsList)
            {
                var existheader = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM StockInDetails WHERE Id = @Id",
                    new { stockinsDetails.Id },
                    transaction) > 0;

                if (!existheader)
                {
                    detailsMessage = await connection.ExecuteAsync(insertdetails, stockinsDetails, transaction);
                }
                else
                {
                    detailsMessage = await connection.ExecuteAsync(updatedetails, stockinsDetails, transaction);
                }
            }
            transaction.Commit();
            return $"Success {detailsMessage}";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }

    // STOCK IN -> StockTransactions (QTY stays POSTIVE, since it's an IN movement)
    // Normal Stock In: Qty stays POSITIVE (increases balance)
    // Transfer-based Stock In (TransferFromInfo populated): QTY goes NEGATIVE
    // to match observed QNE behavior (balance decreases, show under "OUT")
    // SEPT 2, 2026
    public async Task<string> SaveStockInTransactions(
    StockInsModel header,
    List<StockInDetailsModel> details)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var insertQuery = @"
        INSERT INTO StockTransactions (
            Id, StockId, TransactionDate, TransactionTime, DocumentCode, DocType,
            Description, ItemDescription, Qty, Amount, NetAmount, AmountLocal,
            NetAmountLocal, TaxExclusiveAmountLocal, UOMQty, UOMPrice, DiscountAmount,
            UOMPriceExcLandingCost, LocationId, Projectid, CostCentreId, IsCancelled,
            DocumentId, UOMId, IsBundled
        ) VALUES (
            @Id, @StockId, @TransactionDate, @TransactionTime, @DocumentCode, @DocType,
            @Description, @ItemDescription, @Qty, @Amount, @NetAmount, @AmountLocal,
            @NetAmountLocal, @TaxExclusiveAmountLocal, @UOMQty, @UOMPrice, @DiscountAmount,
            @UOMPriceExcLandingCost, @LocationId, @Projectid, @CostCentreId, @IsCancelled,
            @DocumentId, @UOMId, @IsBundled
        )";

        // Detect Transfer-based Stock In
        var isTransfer = !string.IsNullOrWhiteSpace(header.TransferFromInfo);

        try
        {
            foreach (var d in details)
            {
                if (d.StockId is null || d.Qty is null || d.Qty == 0)
                    continue; // skip blank rows

                // Stock In is ALWAYS positive (IN movement), trqansfer-based or not
                var qty = Math.Abs(d.Qty.Value);

                var price = d.Price ?? 0m;
                var amount = qty * price;

                await connection.ExecuteAsync(insertQuery, new
                {
                    Id = Guid.NewGuid(),
                    StockId = d.StockId,
                    TransactionDate = header.StockInDate,
                    TransactionTime = DateTime.Now.TimeOfDay,
                    DocumentCode = header.StockInCode,
                    DocType = "StockIns",
                    Description = header.Description,
                    ItemDescription = d.Description,
                    Qty = qty,
                    Amount = amount,
                    NetAmount = amount,
                    AmountLocal = amount,
                    NetAmountLocal = amount,
                    TaxExclusiveAmountLocal = amount,
                    UOMQty = qty,
                    UOMPrice = price,
                    DiscountAmount = 0m,
                    UOMPriceExcLandingCost = price,
                    LocationId = d.StockLocationId ?? header.StockLocationId,
                    Projectid = d.ProjectId ?? header.ProjectId,
                    CostCentreId = d.CostCentreId,
                    IsCancelled = false,
                    DocumentId = header.Id,
                    UOMId = d.UOMId,
                    IsBundled = d.IsBundled ?? false
                }, transaction);
            }

            transaction.Commit();
            return "Success: StockTransactions synced";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }

    // GET LATEST UNIT PRICE
    public async Task<decimal?> GetLatestUnitPrice(Guid stockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"
        SELECT TOP 1 D.Price
        FROM StockOutDetails D
        INNER JOIN StockOuts H ON H.Id = D.StockOutId
        WHERE D.StockId = @StockId
        AND D.Price > 0.0000
        ORDER BY H.StockOutDate DESC";

        return await connection.QueryFirstOrDefaultAsync<decimal?>(
            query,
            new { StockId = stockId }
        );
    }

    // GET LATEST UNIT PRICE FROM STOCK OUT DETAILS TABLE
    // USED IN STOCK OUT MODULE
    public async Task<decimal?> GetLatestUnitPriceStockOut(Guid stockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"
        SELECT TOP 1 D.Price
        FROM StockOutDetails D
        INNER JOIN StockOuts H ON H.Id = D.StockOutId
        WHERE D.StockId = @StockId
        AND D.Price > 0.0000
        ORDER BY H.StockOutDate DESC";

        return await connection.QueryFirstOrDefaultAsync<decimal?>(
            query,
            new { StockId = stockId }
        );
    }

    // GET ID, NAMES AND FORMAT TRANSACTION CODES (STOCK OUT)
    public async Task<IEnumerable<TransactionsCode>> GetStockTransactionCodeStockOut(int TableId)
    {

        //assign proper parameter for query of specific stockcode
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT *
            FROM TransactionCodes
            WHERE TableId = @TableId
            ORDER BY Name ASC;";
        var model = await connection.QueryAsync<TransactionsCode>(query, new { TableId = TableId });
        return model;
    }

    // GET STOCK OUTS BY ID
    public async Task<StockOutsModel> GetStockOutsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockOuts WHERE Id = @Id";
        var model = await connection.QueryFirstOrDefaultAsync<StockOutsModel>(query, new { Id });
        return model;
    }

    // GET STOCK OUTS DETAILS DATA BY ID
    public async Task<IEnumerable<StockOutDetails>> GetStockOutsDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"SELECT * FROM StockOutDetails WHERE StockOutId LIKE @Id ORDER BY Pos ASC";
        var model = await connection.QueryAsync<StockOutDetails>(query, new { Id });
        return model;
    }

    // GET STOCK OUTS LIST
    public async Task<IEnumerable<StockOutsModel>> GetStockOutsList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @" SELECT
             SO.*,

             FCU.UserName AS FirstCreatedUser,
             DI.FirstCreatedDate,

             LMU.UserName AS LastModifiedUser,
             DI.LastModifiedDate,

             DI.PrintCount

         FROM StockOuts SO

         LEFT JOIN DocumentInfo DI
             ON SO.Id = DI.DocumentId
             AND DI.DocType = 'StockOuts'

         LEFT JOIN SecuritySystemUser FCU
             ON DI.FirstCreatedUserId = FCU.Oid

         LEFT JOIN SecuritySystemUser LMU
             ON DI.LastModifiedUserId = LMU.Oid

         ORDER BY 
 
             SO.StockOutDate DESC,
             SO.StockOutCode ASC,
             SO.Description DESC;
         ";
        var model = await connection.QueryAsync<StockOutsModel>(query, commandType: CommandType.Text);
        return model;
    }

    // GET STOCK OUT TRANSFER LIST
    public async Task<IEnumerable<StockOutTransferList>> GetStockOutTransferList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"
        SELECT DISTINCT

            H.Id,

            SSUFC.FullName,

            H.StockOutCode,

            DI.FirstCreatedDate,

            H.StockOutDate,

            H.Description,

            SSULM.FullName AS LastModifiedUser,

            DI.LastModifiedDate,

            H.ReferenceNo,

            H.TotalAmount,

            DI.PrintCount

        FROM StockOuts H

        INNER JOIN StockOutDetails D
            ON H.Id = D.StockOutId

        LEFT JOIN DocumentInfo DI
            ON DI.DocumentId = H.Id

        LEFT JOIN UOMs U
            ON D.UOMId = U.Id

        LEFT JOIN SecuritySystemUser SSUFC
            ON SSUFC.Oid = DI.FirstCreatedUserId

        LEFT JOIN SecuritySystemUser SSULM
            ON SSULM.Oid = DI.LastModifiedUserId

        WHERE ISNULL(H.IsCancelled,0) = 0

        AND (
            D.Qty - ISNULL(D.Transferred / NULLIF(U.Rate,0),0)
        ) > 0

        ORDER BY H.StockOutDate DESC";

        return await connection.QueryAsync<StockOutTransferList>(query);
    }

    // SAVE OR UPDATE STOCK OUT HEADER
    public async Task<string> SaveStockOutsHeader(StockOutsModel stockoutModel)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var insertheader = @"INSERT INTO StockOuts (
            Id, StockOutCode, StockOutDate, Description, ReferenceNo, Title,
            Ref1, Ref2, Remark1, Remark2, ProjectId, TotalAmount, IsCancelled,
            StockLocationId, CostCentreId, Notes, OptimisticLockField,
            IsProduction, TransferToInfo, Title2, IsPast, DimensionId
        ) VALUES (
            @Id, @StockOutCode, @StockOutDate, @Description, @ReferenceNo, @Title,
            @Ref1, @Ref2, @Remark1, @Remark2, @ProjectId, @TotalAmount, @IsCancelled,
            @StockLocationId, @CostCentreId, @Notes, @OptimisticLockField,
            @IsProduction, @TransferToInfo, @Title2, @IsPast, @DimensionId
        )";

        var updateheader = @"UPDATE StockOuts SET
            StockOutCode=@StockOutCode, StockOutDate=@StockOutDate, Description=@Description, ReferenceNo=@ReferenceNo, Title=@Title,
            Ref1=@Ref1, Ref2=@Ref2, Remark1=@Remark1, Remark2=@Remark2, ProjectId=@ProjectId, TotalAmount=@TotalAmount, IsCancelled=@IsCancelled,
            StockLocationId=@StockLocationId, CostCentreId=@CostCentreId, Notes=@Notes, OptimisticLockField=@OptimisticLockField,
            IsProduction=@IsProduction, TransferToInfo=@TransferToInfo, Title2=@Title2, IsPast@IsPast, DimensionId=@DimensionId
        WHERE Id=@Id";

        object affectedRows = string.Empty;

        // GENERATE ID FOR NEW HEADER
        if (stockoutModel.Id == Guid.Empty)
        {
            stockoutModel.Id = Guid.NewGuid();
        }

        var existheader =
        await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM StockOuts WHERE Id=@Id",
            new { stockoutModel.Id },
            transaction) > 0;
        try
        {
            if (!existheader)
            {
                // SAVE HEADER
                affectedRows = await connection.ExecuteAsync(insertheader, stockoutModel, transaction);
            }
            else
            {
                // UPDATE HEADER
                affectedRows = await connection.ExecuteAsync(updateheader, stockoutModel, transaction);
            }

            transaction.Commit();

            return $"Success {affectedRows} affected";
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            return $"Error: {ex.Message}";
        }
    }

    // SAVE OR UPDATE STOCK OUT DETAILS
    public async Task<string> SaveStockOutDetails(List<StockOutDetails> stockOutDetailsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var insertdetails = @"INSERT INTO StockOutDetails (
            Id, StockOutId, Pos, Numbering, StockId, Description, UOMId,
            Price, Qty, Amount, ProjectId, StockLocationId, CostCentreId,
            Note, ItemTypeCode, StockType, OptimisticLockField, StockBatchNumberId,
            Transferred, IsBundled, IsSubItem, BundledTransactionId, SerialNumber,
            SerialNumberRef1, SerialNumberRef2, PackingUOMId, Packing, PackingQty,
            Ref1, Ref2, Ref3, Ref4, Ref5, PackingUOM1, PackingUOM2, HighestUOM,
            DimensionId
        ) VALUES (
            @Id, @StockOutId, @Pos, @Numbering, @StockId, @Description, @UOMId,
            @Price, @Qty, @Amount, @ProjectId, @StockLocationId, @CostCentreId,
            @Note, @ItemTypeCode, @StockType, @OptimisticLockField, @StockBatchNumberId,
            @Transferred, @IsBundled, @IsSubItem, @BundledTransactionId, @SerialNumber,
            @SerialNumberRef1, @SerialNumberRef2, @PackingUOMId, @Packing, @PackingQty,
            @Ref1, @Ref2, @Ref3, @Ref4, @Ref5, @PackingUOM1, @PackingUOM2, @HighestUOM,
            @DimensionId
        )";
        var updatedetails = @"UPDATE StockOutDetails SET
            StockOutId=@StockOutId, Pos=@Pos, Numbering=@Numbering, StockId=@StockId, Description=@Description, UOMId=@UOMId,
            Price=@Price, Qty=@Qty, Amount=@Amount, ProjectId=@ProjectId, StockLocationId=@StockLocationId, CostCentreId=@CostCentreId,
            Note=@Note, ItemTypeCode=@ItemTypeCode, StockType=@StockType, OptimisticLockField=@OptimisticLockField, StockBatchNumberId=@StockBatchNumberId,
            Transferred=@Transferred, IsBundled=@IsBundled, IsSubItem=@IsSubItem, BundledTransactionId=@BundledTransactionId, =@SerialNumber,
            =@SerialNumberRef1, =@SerialNumberRef2, =@PackingUOMId, =@Packing, =@PackingQty,
            Ref1=@Ref1, Ref2=@Ref2, Ref3=@Ref3, Ref4=@Ref4, Ref5=@Ref5, PackingUOM1=@PackingUOM1, =@PackingUOM2, =@HighestUOM,
            DimensionId=@DimensionId
        WHERE Id = @Id";
        object detailsMessage = string.Empty;
        try
        {
            foreach (var stockoutsDetails in stockOutDetailsList)
            {
                var existheader = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM StockOutDetails WHERE Id = @Id",
                    new { stockoutsDetails.Id },
                    transaction) > 0;

                if (!existheader)
                {
                    detailsMessage = await connection.ExecuteAsync(insertdetails, stockoutsDetails, transaction);
                }
                else
                {
                    detailsMessage = await connection.ExecuteAsync(updatedetails, stockoutsDetails, transaction);
                }
            }
            transaction.Commit();
            return $"Success {detailsMessage}";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }

    // STOCK OUT -> StockTransactions (Qty stored NEGATIVE, since it's an OUT movement,
    // even though the grid/detail table stores Qty as a positive number)
    public async Task<string> SaveStockOutTransactions(
        StockOutsModel header,
        List<StockOutDetails> details)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertQuery = @"
        INSERT INTO StockTransactions (
            Id, StockId, TransactionDate, TransactionTime, DocumentCode, DocType,
            Description, ItemDescription, Qty, Amount, NetAmount, AmountLocal,
            NetAmountLocal, TaxExclusiveAmountLocal, UOMQty, UOMPrice, DiscountAmount,
            UOMPriceExcLandingCost, LocationId, Projectid, CostCentreId, IsCancelled,
            DocumentId, UOMId, IsBundled
        ) VALUES (
            @Id, @StockId, @TransactionDate, @TransactionTime, @DocumentCode, @DocType,
            @Description, @ItemDescription, @Qty, @Amount, @NetAmount, @AmountLocal,
            @NetAmountLocal, @TaxExclusiveAmountLocal, @UOMQty, @UOMPrice, @DiscountAmount,
            @UOMPriceExcLandingCost, @LocationId, @Projectid, @CostCentreId, @IsCancelled,
            @DocumentId, @UOMId, @IsBundled
        )";

        try
        {
            foreach (var d in details)
            {
                if (d.StockId is null || d.Qty is null || d.Qty == 0)
                    continue; // skip blank rows

                var qty = -Math.Abs(d.Qty.Value);   // force NEGATIVE for Stock Out
                var price = d.Price ?? 0m;
                var amount = qty * price;           // negative amount, matches ledger convention

                await connection.ExecuteAsync(insertQuery, new
                {
                    Id = Guid.NewGuid(),
                    StockId = d.StockId,
                    TransactionDate = header.StockOutDate,
                    TransactionTime = DateTime.Now.TimeOfDay,
                    DocumentCode = header.StockOutCode,
                    DocType = "StockOuts",
                    Description = header.Description,
                    ItemDescription = d.Description,
                    Qty = qty,
                    Amount = amount,
                    NetAmount = amount,
                    AmountLocal = amount,
                    NetAmountLocal = amount,
                    TaxExclusiveAmountLocal = amount,
                    UOMQty = qty,
                    UOMPrice = price,
                    DiscountAmount = 0m,
                    UOMPriceExcLandingCost = price,
                    LocationId = d.StockLocationId ?? header.StockLocationId,
                    Projectid = d.ProjectId ?? header.ProjectId,
                    CostCentreId = d.CostCentreId,
                    IsCancelled = false,
                    DocumentId = header.Id,
                    UOMId = d.UOMId,
                    IsBundled = d.IsBundled ?? false
                }, transaction);
            }

            transaction.Commit();
            return "Success: StockTransactions synced";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }


    // GET STOCK STOCK BILL OF MATERIAL LIST 
    public async Task<IEnumerable<StockBOMModel>> GetStockBOMList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"
        SELECT 
            sb.*,
            s.StockCode
        FROM StockBOM sb
        INNER JOIN Stocks s
            ON sb.StockId = s.Id
        ORDER BY BOMCode DESC";


        var model = await connection.QueryAsync<StockBOMModel>(query, commandType: CommandType.Text);

        return model;

    }

    // GET STOCK BILL OF MATERIAL BY ID
    public async Task<StockBOMModel> GetStockBOMById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM StockBOM WHERE Id = @Id";

        var model = await connection.QueryFirstOrDefaultAsync<StockBOMModel>(query, new { Id });

        return model;

    }

    // GET STOCK BILL OF MATERIAL DETAILS DATA BY ID
    public async Task<IEnumerable<StockBOMDetailModel>> GetStockBOMDetailsById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM StockBOMDetail WHERE BOMId LIKE @Id ORDER BY Pos ASC";

        var model = await connection.QueryAsync<StockBOMDetailModel>(query, new { Id });

        return model;
    }

    public async Task<IEnumerable<StockBOMDetailModel>> GetStockBOMDetailsByIdSAO(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"
            SELECT * FROM StockBOMDetail 
            WHERE BOMId LIKE @Id 
            ORDER BY Description ASC";

        var model = await connection.QueryAsync<StockBOMDetailModel>(query, new { Id });

        return model;
    }

    // SAVE OR UPDATE STOCK BILL OF MATERIAL HEADER
    public async Task<string> SaveStockBOMHeader(StockBOMModel stockbomModel)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var insertheader = @"INSERT INTO StockBOM (
           Id, StockId, BOMCode, Description,
           IsDefault, AssemblyCost, StockLocationId,
           UnitCost, OptimisticLockField, Notes
        ) VALUES (
           @Id, @StockId, @BOMCode, @Description,
           @IsDefault, @AssemblyCost, @StockLocationId,
           @UnitCost, @OptimisticLockField, @Notes
        )";

        var updateheader = @"UPDATE StockBOM SET
           StockId = @StockId , BOMCode = @BOMCode, Description = @Description,
           IsDefault = @IsDefault , AssemblyCost = @AssemblyCost , StockLocationId = @StockLocationId,
           UnitCost = @UnitCost , OptimisticLockField = @OptimisticLockField, Notes = @Notes
        WHERE Id=@Id";

        object affectedRows = string.Empty;

        // GENERATE ID FOR NEW HEADER
        if (stockbomModel.Id == Guid.Empty)
        {
            stockbomModel.Id = Guid.NewGuid();
        }

        var existheader = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM StockBOM WHERE Id = @Id",
            new { stockbomModel.Id },
            transaction) > 0;

        try
        {
            if (!existheader)
            {
                // SAVE HEADER 
                affectedRows = await connection.ExecuteAsync(insertheader, stockbomModel, transaction);
            }
            else
            {
                // UPDATE HEADER
                affectedRows = await connection.ExecuteAsync(updateheader, stockbomModel, transaction);
            }

            transaction.Commit();

            return $"Success {affectedRows} affected";
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            return $"Error: {ex.Message}";
        }
    }

    // SAVE OR UPDATE STOCK BILL OF MATERIAL DETAILS 
    public async Task<string> SaveStockBOMDetails(List<StockBOMDetailModel> stockbomDetailsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var insertdetails = @"INSERT INTO StockBOMDetail (
            Id, BOMId, StockId, Description, UOMId,
            Qty, UnitCost, LabourCost, OverheadCost,
            StockLocationId, ItemTypeCode, Pos,
            OptimisticLockField, BOMCode
        ) VALUES (
            @Id, @BOMId, @StockId, @Description, @UOMId,
            @Qty, @UnitCost, @LabourCost, @OverheadCost,
            @StockLocationId, @ItemTypeCode, @Pos,
            @OptimisticLockField, @BOMCode
        )";
        var updatedetails = @"UPDATE StockBOMDetail SET
            BOMId = @BOMId, StockId = @StockId, Description = @Description, UOMId = @UOMId,
            Qty = @Qty, UnitCost = @UnitCost, LabourCost = @LabourCost, OverheadCost = @OverheadCost,
            StockLocationId = @StockLocationId, ItemTypeCode = @ItemTypeCode, Pos = @Pos,y
            OptimisticLockField = @OptimisticLockField, BOMCode = @BOMCode
        WHERE Id = @Id";
        object detailsMessage = string.Empty;
        try
        {
            foreach (var stockbomDetails in stockbomDetailsList)
            {
                var existheader = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM StockBOMDetail WHERE Id = @Id",
                    new { stockbomDetails.Id }, transaction) > 0;

                if (!existheader)
                {
                    detailsMessage = await connection.ExecuteAsync(insertdetails, stockbomDetails, transaction);
                }
                else
                {
                    detailsMessage = await connection.ExecuteAsync(updatedetails, stockbomDetails, transaction);
                }
            }
            transaction.Commit();
            return $"Success {detailsMessage}";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }


    // GET NEXT BOM CODE FOR A GIVEN STOCK (e.g. IM-07-0075-001, -002, ...)
    public async Task<string> GetNextBOMCode(Guid stockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var stockCode = await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT StockCode FROM Stocks WHERE Id = @StockId",
            new { StockId = stockId });

        if (string.IsNullOrEmpty(stockCode))
            return string.Empty;

        // Pull ALL existing BOM codes for this stock — string ORDER BY is
        // unreliable once a code gets manually renamed/appended (e.g. "SAMPLE-002 (RM) SAMP..."),
        // so we scan every row instead of trusting TOP 1.
        var allCodes = await connection.QueryAsync<string>(
            "SELECT BOMCode FROM StockBOM WHERE StockId = @StockId",
            new { StockId = stockId });

        int maxSeq = 0;

        // Only match codes that START with "{stockCode}-" followed by digits.
        // Anything appended after the number (like " (RM) SAMPCODE") is ignored,
        // so a manually-edited code still counts toward the sequence correctly.
        var pattern = $@"^{System.Text.RegularExpressions.Regex.Escape(stockCode)}-(\d+)";

        foreach (var code in allCodes)
        {
            if (string.IsNullOrEmpty(code)) continue;

            var match = System.Text.RegularExpressions.Regex.Match(code, pattern);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int seq))
            {
                if (seq > maxSeq)
                    maxSeq = seq;
            }
        }

        int nextSeq = maxSeq + 1;

        return $"{stockCode}-{nextSeq:D3}";
    }



    // GET STOCK ASSEMBLY ORDER LIST 
    public async Task<IEnumerable<StockAssemblyOrderModel>> GetStockAssemblyOrderList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"
        SELECT
            sao.*,
            sb.BatchNo
        FROM StockAssemblyOrder AS sao
        INNER JOIN StockBatchNumbers AS sb
            ON sao.StockBatchNumberId = sb.Id
        ORDER BY 
            sao.AssemblyOrderDate DESC, 
            sao.AssemblyOrderCode ASC, 
            sao.Description DESC;";

        var model = await connection.QueryAsync<StockAssemblyOrderModel>(query, commandType: CommandType.Text);

        return model;
    }

    // GET STOCK ASSEMBLY ORDER BY ID
    public async Task<StockAssemblyOrderModel> GetStockAssemblyOrderById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM StockAssemblyOrder WHERE Id = @Id";

        var model = await connection.QueryFirstOrDefaultAsync<StockAssemblyOrderModel>(query, new { Id });

        return model;
    }

    // GET STOCK ASSEMBLY ORDER DETAILS DATA BY ID
    public async Task<IEnumerable<StockAssemblyOrderDetail>> GetStockAssemblyOrderDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"
            SELECT 
                sad.*,
                sbn.BatchNoRef1,
                sbn.BatchNoRef2,
                sbn.BatchNoExpiryDate
            FROM StockAssemblyOrderDetail sad
            LEFT JOIN StockBatchNumbers sbn
                ON sad.StockBatchNumberId = sbn.Id
            WHERE sad.StockAssemblyOrderId LIKE @Id
            ORDER BY sad.Pos ASC";

        var model = await connection.QueryAsync<StockAssemblyOrderDetail>(query, new { Id });

        return model;
    }



    // SAVE OR UPDATE STOCK ASSEMBLY ORDER
    public async Task<string> SaveStockAssemblyOrderHeader(StockAssemblyOrderModel stockassemblyorderModel)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var insertheader = @"INSERT INTO StockAssemblyOrder (
            Id, StockId, BOMId, UOMId, Description, Qty, ReferenceNo, ProjectId,
            StockLocationId, IsCancelled, Cost, TotalCost, AssemblyOrderCode, AssemblyOrderDate,
            IsClosed, TransferToInfo, StockBatchNumberId, StockType, SerialNumber, SerialNumberRef1,
            SerialNumberRef2, OptimisticLockField, HasBOM, TransferToMasterInfo, TransferQty
        ) VALUES (
            @Id, @StockId, @BOMId, @UOMId, @Description, @Qty, @ReferenceNo, @ProjectId,
            @StockLocationId, @IsCancelled, @Cost, @TotalCost, @AssemblyOrderCode, @AssemblyOrderDate,
            @IsClosed, @TransferToInfo, @StockBatchNumberId, @StockType, @SerialNumber, @SerialNumberRef1,
            @SerialNumberRef2, @OptimisticLockField, @HasBOM, @TransferToMasterInfo, @TransferQty)";

        var updateheader = @"UPDATE StockAssemblyOrder SET
            StockId=@StockId, BOMId=@BOMId, UOMId=@UOMId, Description=@Description, Qty=@Qty, ReferenceNo=@ReferenceNo, ProjectId=@ProjectId,
            StockLocationId=@StockLocationId, IsCancelled=@IsCancelled, Cost=@Cost, TotalCost=@TotalCost, AssemblyOrderCode=@AssemblyOrderCode, AssemblyOrderDate=@AssemblyOrderDate,
            IsClosed=@IsClosed, TransferToInfo=@TransferToInfo, StockBatchNumberId=@StockBatchNumberId, StockType=@StockType, SerialNumber=@SerialNumber, SerialNumberRef1=@SerialNumberRef1,
            SerialNumberRef2=@SerialNumberRef2, OptimisticLockField=@OptimisticLockField, HasBOM=@HasBOM, TransferToMasterInfo=@TransferToMasterInfo, TransferQty=@TransferQty
        WHERE Id=@Id";

        object affectedRows = string.Empty;

        // GENERATE ID FOR NEW HEADER
        if (stockassemblyorderModel.Id == Guid.Empty)
        {
            stockassemblyorderModel.Id = Guid.NewGuid();
        }

        // DEFAULT VALUES - Para mag display sa Stock Assembly Order Transfer List
        // "H.IsClosed = 0" and "H.TransferQty = 0" (no ISNULL wrapper), so a NULL
        stockassemblyorderModel.IsCancelled ??= false;
        stockassemblyorderModel.IsClosed ??= false;
        stockassemblyorderModel.TransferQty ??= 0m;
        stockassemblyorderModel.OptimisticLockField ??= 1;

        var existheader =
            await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM StockAssemblyOrder WHERE Id=@Id",
                new { stockassemblyorderModel.Id },
                transaction) > 0;
        try
        {
            if (!existheader)
            {
                // SAVE HEADER
                affectedRows = await connection.ExecuteAsync(insertheader, stockassemblyorderModel, transaction);
            }
            else
            {
                // UPDATE HEADER
                affectedRows = await connection.ExecuteAsync(updateheader, stockassemblyorderModel, transaction);
            }

            transaction.Commit();

            return $"Success {affectedRows} affected";
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            return $"Error: {ex.Message}";
        }
    }

    // SAVE OR UPDATE STOCK ASSEMBLY ORDER DETAILS
    public async Task<string> SaveStockAssemblyOrderDetails(List<StockAssemblyOrderDetail> stockassemblyorderDetailsList)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var insertdetails = @"INSERT INTO StockAssemblyOrderDetail (
            Id, Numbering, StockId, Description, UOMId, Qty, DefaultQty,
            UnitCost, LabourCost, OverheadCost, StockLocationId, ProjectId,
            StockAssemblyOrderId, Pos, Ref1, Ref2, Ref3, Ref4, Ref5,
            Transferred, StockType, SerialNumber, SerialNumberRef1,
            SerialNumberRef2, StockBatchNumberId, OptimisticLockField,
            TransferLink
        ) VALUES (
            @Id, @Numbering, @StockId, @Description, @UOMId, @Qty, @DefaultQty,
            @UnitCost, @LabourCost, @OverheadCost, @StockLocationId, @ProjectId,
            @StockAssemblyOrderId, @Pos, @Ref1, @Ref2, @Ref3, @Ref4, @Ref5,
            @Transferred, @StockType, @SerialNumber, @SerialNumberRef1,
            @SerialNumberRef2, @StockBatchNumberId, @OptimisticLockField,
            @TransferLink
        )";
        var updatedetails = @"UPDATE StockAssemblyOrderDetail SET
            Numbering=@Numbering, StockId=@StockId, Description=@Description, UOMId=@UOMId, Qty=@Qty, DefaultQty=@DefaultQty,
            UnitCost=@UnitCost, LabourCost=@LabourCost, OverheadCost=@OverheadCost, StockLocationId=@StockLocationId, ProjectId=@ProjectId,
            StockAssemblyOrderId=@StockAssemblyOrderId, Pos=@Pos, Ref1=@Ref1, Ref2=@Ref2, Ref3=@Ref3, Ref4=@Ref4, Ref5=@Ref5,
            Transferred=@Transferred, StockType=@StockType, SerialNumber=@SerialNumber, SerialNumberRef1=@SerialNumberRef1,
            SerialNumberRef2=@SerialNumberRef2, StockBatchNumberId=@StockBatchNumberId, OptimisticLockField=@OptimisticLockField,
            TransferLink=@TransferLink
        WHERE Id = @Id";
        object detailsMessage = string.Empty;
        try
        {
            foreach (var stockassemblyorderDetails in stockassemblyorderDetailsList)
            {
                var existheader = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM StockAssemblyOrderDetail WHERE Id = @Id",
                    new { stockassemblyorderDetails.Id },
                    transaction) > 0;

                if (!existheader)
                {
                    detailsMessage = await connection.ExecuteAsync(insertdetails, stockassemblyorderDetails, transaction);
                }
                else
                {
                    detailsMessage = await connection.ExecuteAsync(updatedetails, stockassemblyorderDetails, transaction);
                }
            }
            transaction.Commit();
            return $"Success {detailsMessage}";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }

    // LATEST UNIT COST FROM STOCK IN DETAILS TABLE (USED BY STOCK ASSEMBLY ORDER MODULE)
    // AUG 17, 2026
    public async Task<decimal> GetLatestUnitCostStockInDetails(Guid stockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"
            SELECT TOP 1 
                D.Price
            FROM StockInDetails D
            INNER JOIN StockIns H ON H.Id = D.StockInId
            WHERE D.StockId = @StockId
            ORDER BY H.StockInDate DESC";

        return await connection.ExecuteScalarAsync<decimal?>(query, new { StockId = stockId }) ?? 0;
    }

    // GET BOM LIST BY STOCK
    // STOCK ASSEMBLY ORDER MODULE
    // JULY 1, 2026
    public async Task<IEnumerable<StockBOMModel>> GetStockBOMByStock(Guid stockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"
        SELECT
            sb.*,
            s.StockCode
        FROM StockBOM sb
        INNER JOIN Stocks s
            ON sb.StockId = s.Id
        WHERE sb.StockId = @StockId
        ORDER BY sb.IsDefault DESC, sb.BOMCode ASC";

        var model = await connection.QueryAsync<StockBOMModel>(
            query,
            new { StockId = stockId });

        return model;
    }

    // GET BATCH NO LIST BY STOCK
    // STOCK ASSEMBLY ORDER MODULE
    // JULY 1, 2026
    public async Task<IEnumerable<StockBatchNumbersModel>> GetBatchNumbersByStock(Guid stockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"
            SELECT
            Id,
            StockId,
            BatchNo,
            Description,
            Pos,
            BatchNoManufacturingDate,
            BatchNoExpiryDate,
            BalanceQty
        FROM StockBatchNumbers
        WHERE StockId = @StockId
        ORDER BY BatchNo ASC";

        return await connection.QueryAsync<StockBatchNumbersModel>(
            query, new { StockId = stockId });
    }

    // GET UOM CODE LIST BY STOCK
    // STOCK ASSEMBLY ORDER MODULE
    // JULY 1, 2026
    public async Task<IEnumerable<UOMs>> GetUOMCodeByStock(Guid stockId)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"
            SELECT
            Id,
            StockId,
            UOMCode
        FROM UOMs
        WHERE StockId = @StockId
        ORDER BY UOMCode ASC";

        return await connection.QueryAsync<UOMs>(
            query, new { StockId = stockId });
    }

    // STOCK ASSEMBLY ORDER TRANSFER LIST 
    // AUG 10, 2026
    public async Task<IEnumerable<StockAssemblyOrderTransferListModel>> GetStockAssemblyOrderTransferList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        var query = @"
        SELECT DISTINCT 
                H.Id,
		        H.AssemblyOrderDate,
		        H.AssemblyOrderCode,
                H.Description,
                H.ReferenceNo,
	             SUM(ISNULL(D.Qty,0) * ISNULL(D.UnitCost,0)) / NULLIF(ISNULL(H.Qty,0), 0) AS UnitCost, -- unitcost computation galing details table sum of QTY * UNITCOST = AMOUNT / H.QTY
		         H.Qty AS SAOQty,
                (ISNULL(H.Qty,0) - ISNULL(SA.Qty,0)) AS RemainingQty,
                H.TransferQty,
                H.TransferToInfo,
                H.TransferToMasterInfo
            FROM StockAssemblyOrder H
	        INNER JOIN StockAssemblyOrderDetail D ON H.Id = D.StockAssemblyOrderId
	        LEFT JOIN StockAssembly SA ON H.TransferToMasterInfo = SA.AssemblyCode
	        LEFT JOIN UOMs U ON H.UOMId = U.Id
            WHERE ISNULL(H.IsCancelled, 0) = 0
            AND H.IsClosed = 0
            AND H.TransferQty = 0
            AND H.Qty != 0

 
        GROUP BY
            H.Id,
            H.AssemblyOrderDate,
            H.AssemblyOrderCode,
            H.Description,
            H.ReferenceNo,
            H.Qty,
            SA.Qty,
           H.TransferQty,
             H.TransferToInfo,
                H.TransferToMasterInfo
           ORDER BY H.AssemblyOrderCode DESC";
        return await connection.QueryAsync<StockAssemblyOrderTransferListModel>(query);
    }


    // GET STOCK ASSEMBLY 
    // JULY 2, 2026
    public async Task<IEnumerable<StockAssemblyModel>> GetStockAssemblyList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"
            SELECT
                 SA.*,

                 sb.BatchNo,

                 s.StockCode,

                 FCU.UserName AS FirstCreatedUser,
                 DI.FirstCreatedDate,

                 LMU.UserName AS LastModifiedUser,
                 DI.LastModifiedDate,

                 DI.PrintCount

             FROM StockAssembly SA

             LEFT JOIN DocumentInfo DI
                 ON SA.Id = DI.DocumentId
                 AND DI.DocType = 'StockAssembly'

             LEFT JOIN SecuritySystemUser FCU
                 ON DI.FirstCreatedUserId = FCU.Oid

             LEFT JOIN SecuritySystemUser LMU
                 ON DI.LastModifiedUserId = LMU.Oid

             LEFT JOIN StockBatchNumbers sb
                ON sb.Id = SA.StockBatchNumberId

            INNER JOIN Stocks s
                ON s.Id = SA.StockId

             ORDER BY 
 
                 SA.AssemblyDate DESC,
                 SA.AssemblyCode ASC,
                 SA.Description DESC;
                     ";
        var model = await connection.QueryAsync<StockAssemblyModel>(query, commandType: CommandType.Text);
        return model;
    }

    // GET STOCK ASSEMBLY BY ID
    public async Task<StockAssemblyModel> GetStockAssemblyById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM StockAssembly WHERE Id = @Id";

        var model = await connection.QueryFirstOrDefaultAsync<StockAssemblyModel>(query, new { Id });

        return model;
    }

    // GET STOCK ASSEMBLY DETAILS DATA BY ID
    public async Task<IEnumerable<StockAssemblyDetail>> GetStockAssemblyDetailById(string Id)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);

        var query = @"SELECT * FROM StockAssemblyDetail WHERE StockAssemblyId LIKE @Id ORDER BY Pos ASC";

        var model = await connection.QueryAsync<StockAssemblyDetail>(query, new { Id });

        return model;
    }

    protected CheckExistModel checkExistModel = new CheckExistModel();

    public async Task<string> SaveStockAssemblyWithTransfer(StockAssemblyModel stockAssemblyHeader, List<StockAssemblyDetail> stockAssemblyDetails, StockAssemblyOrderForTransferHeader stockAssemblyOrderForTransferHeader, List<StockAssemblyOrderForTransferDetails> stockAssemblyOrderForTransferDetails)
    {
        checkExistModel.Id = stockAssemblyHeader.Id;
        checkExistModel.TableName = "StockAssembly";

        var connectionString = await qneConnection.GetConnectionStringAsync();

        using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        // QUERY FOR HEADER INSERT & UPDATE
        var insertHeaderQuery = @"INSERT INTO StockAssembly(
            Id, StockId, BOMId, UOMId, Description, Qty, ReferenceNo,ProjectId,
            StockLocationId, IsCancelled, Cost, TotalCost, AssemblyCode, AssemblyDate,
            TransferFromInfo, UseStandardUnitCost, StandardUnitCost, StockBatchNumberId,
            StockType, SerialNumber, SerialNumberRef1, SerialNumberRef2, OptimisticLockField,
            HasBOM, TransferFromMasterInfo 
        ) VALUES (
            @Id, @StockId, @BOMId, @UOMId, @Description, @Qty, @ReferenceNo, @ProjectId,
            @StockLocationId, @IsCancelled, @Cost, @TotalCost, @AssemblyCode, @AssemblyDate,
            @TransferFromInfo, @UseStandardUnitCost, @StandardUnitCost, @StockBatchNumberId,
            @StockType, @SerialNumber, @SerialNumberRef1, @SerialNumberRef2, @OptimisticLockField,
            @HasBOM, @TransferFromMasterInfo
        );";
        var updateHeaderQuery = @"
        UPDATE StockAssembly SET
            StockId=@StockId, BOMId=@BOMId, UOMId=@UOMId, Description=@Description, Qty=@Qty, ReferenceNo=@ReferenceNo, ProjectId=@ProjectId,
            StockLocationId=@StockLocationId, IsCancelled=@IsCancelled, Cost=@Cost, TotalCost=@TotalCost, AssemblyCode=@AssemblyCode, AssemblyDate=@AssemblyDate,
            TransferFromInfo=@TransferFromInfo, UseStandardUnitCost=@UseStandardUnitCost, StandardUnitCost=@StandardUnitCost, StockBatchNumberId=@StockBatchNumberId,
            StockType=@StockType, SerialNumber=@SerialNumber, SerialNumberRef1=@SerialNumberRef1, SerialNumberRef2=@SerialNumberRef2, OptimisticLockField=@OptimisticLockField,
            HasBOM=@HasBOM, TransferFromMasterInfo=@TransferFromMasterInfo
        WHERE Id=@Id";

        // QUERY FOR DETAILS INSERT & UPDATE
        var insertDetailQuery = @"
        INSERT INTO StockAssemblyDetail (
            Id, Numbering, StockId, Description, UOMId, Qty, DefaultQty, UnitCost,
            LabourCost, OverheadCost, StockLocationId, ProjectId, StockAssemblyId,
            Pos, Ref1, Ref2, Ref3, Ref4, Ref5, TransferLink, StockType, SerialNumber,
            SerialNumberRef1, SerialNumberRef2, StockBatchNumberId, OptimisticLockField,
            IsWastage, RefTransferFrom
        ) VALUES (
            @Id, @Numbering, @StockId, @Description, @UOMId, @Qty, @DefaultQty, @UnitCost,
            @LabourCost, @OverheadCost, @StockLocationId, @ProjectId, @StockAssemblyId,
            @Pos, @Ref1, @Ref2, @Ref3, @Ref4, @Ref5, @TransferLink, @StockType, @SerialNumber,
            @SerialNumberRef1, @SerialNumberRef2, @StockBatchNumberId, @OptimisticLockField,
            @IsWastage, @RefTransferFrom)";
        var updateDetailQuery = @"
        UPDATE StockAssemblyDetail SET
            Numbering=@Numbering, StockId=@StockId, Description=@Description, UOMId=@UOMId, Qty=@Qty, DefaultQty=@DefaultQty, UnitCost=@UnitCost,
            LabourCost=@LabourCost, OverheadCost=@OverheadCost, StockLocationId=@StockLocationId, ProjectId=@ProjectId, StockAssemblyId=@StockAssemblyId,
            Pos=@Pos, Ref1=@Ref1, Ref2=@Ref2, Ref3=@Ref3, Ref4=@Ref4, Ref5=@Ref5, TransferLink=@TransferLink, StockType=@StockType, SerialNumber=@SerialNumber,
            SerialNumberRef1=@SerialNumberRef1, SerialNumberRef2=@SerialNumberRef2, StockBatchNumberId=@StockBatchNumberId, OptimisticLockField=@OptimisticLockField,
            IsWastage=@IsWastage, RefTransferFrom=@RefTransferFrom
        WHERE Id=@Id";

        var detailCheckModel = new CheckExistModel { TableName = "StockAssemblyDetail" };

        try
        {
            var existheader = await checkExistServices.RecordExistsByIdAsync(checkExistModel);

            if (!existheader)
            {
                await connection.ExecuteAsync(insertHeaderQuery, stockAssemblyHeader, transaction);

                var isTransfer = stockAssemblyOrderForTransferHeader?.StockAssemblyOrderHeaderId != null;


                if (isTransfer)
                {
                    var updateStockAssemblyOrderHeaderQuery = @"UPDATE StockAssemblyOrder
                    SET TransferToInfo =
                        CASE
                            WHEN TransferToInfo IS NULL OR TransferToInfo = '' THEN @TransferToInfo
                            ELSE TransferToInfo + ', ' + @TransferToInfo
                        END,
                        TransferToMasterInfo = @TransferToMasterInfo,
                        TransferQty = @TransferQty
                    WHERE Id = @StockAssemblyOrderHeaderId";

                    await connection.ExecuteAsync(updateStockAssemblyOrderHeaderQuery, stockAssemblyOrderForTransferHeader, transaction);

                    // link table rows
                    await stocksTransferLinkServices.InsertStocksTransferLink(stockAssemblyOrderForTransferDetails);

                    // bump Transferred on each source order detail row
                    var updateSAODetailQuery = @"
                        UPDATE StockAssemblyOrderDetail
                        SET Transferred = ISNULL(Transferred,0) + @Transferred,
                            OptimisticLockField = OptimisticLockField + 1
                        WHERE Id = @StockAssemblyOrderDetailId";

                    foreach (var saodetail in stockAssemblyOrderForTransferDetails)
                    {
                        await connection.ExecuteAsync(updateSAODetailQuery,
                            new
                            {
                                StockAssemblyOrderDetailId = saodetail.StockAssemblyOrderDetailId,
                                Transferred = saodetail.Transferred
                            },
                            transaction);
                    }
                }
            }
            else
            {
                await connection.ExecuteAsync(updateHeaderQuery, stockAssemblyHeader, transaction);
            }

            foreach (var detail in stockAssemblyDetails)
            {
                detailCheckModel.Id = detail.Id;
                var detailExists = await checkExistServices.RecordExistsByIdAsync(detailCheckModel);

                if (!detailExists)
                    await connection.ExecuteAsync(insertDetailQuery, detail, transaction);
                else
                    await connection.ExecuteAsync(updateDetailQuery, detail, transaction);
            }

            transaction.Commit();
            return "Success: Stock Assembly saved successfully.";
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }

    }

    // Stock Assembly creates 2 types of stock Transanctions
    // 1. Finished item = IN (Add to Stock)
    // 2. Components = OUT (Remove from stock)
    public async Task<string> SaveStockAssemblyTransactions(
        StockAssemblyModel header,
        List<StockAssemblyDetail> details)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertQuery = @"
        INSERT INTO StockTransactions (
            Id, StockId, TransactionDate, TransactionTime, DocumentCode, DocType,
            Description, ItemDescription, Qty, Amount, NetAmount, AmountLocal,
            NetAmountLocal, TaxExclusiveAmountLocal, UOMQty, UOMPrice, DiscountAmount,
            UOMPriceExcLandingCost, LocationId, Projectid, CostCentreId, IsCancelled,
            DocumentId, UOMId, IsBundled, StockBatchNumberId
        ) VALUES (
            @Id, @StockId, @TransactionDate, @TransactionTime, @DocumentCode, @DocType,
            @Description, @ItemDescription, @Qty, @Amount, @NetAmount, @AmountLocal,
            @NetAmountLocal, @TaxExclusiveAmountLocal, @UOMQty, @UOMPrice, @DiscountAmount,
            @UOMPriceExcLandingCost, @LocationId, @Projectid, @CostCentreId, @IsCancelled,
            @DocumentId, @UOMId, @IsBundled,  @StockBatchNumberId
        )";

        try
        {
            // 1. Add the finished item to stock
            if (header.StockId != null && header.Qty is decimal hq && hq != 0)
            {
                var qty = Math.Abs(hq);
                var price = header.Cost ?? 0m;
                var amount = qty * price;

                await connection.ExecuteAsync(insertQuery, new
                {
                    Id = Guid.NewGuid(),
                    StockId = header.StockId,
                    TransactionDate = header.AssemblyDate,
                    TransactionTime = DateTime.Now.TimeOfDay,
                    DocumentCode = header.AssemblyCode,
                    DocType = "StockAssembly",
                    Description = header.Description,
                    ItemDescription = header.Description,
                    Qty = qty,
                    Amount = amount,
                    NetAmount = amount,
                    AmountLocal = amount,
                    NetAmountLocal = amount,
                    TaxExclusiveAmountLocal = amount,
                    UOMQty = qty,
                    UOMPrice = price,
                    DiscountAmount = 0m,
                    UOMPriceExcLandingCost = price,
                    LocationId = header.StockLocationId,
                    Projectid = header.ProjectId,
                    CostCentreId = (int?)null,
                    IsCancelled = false,
                    DocumentId = header.Id,
                    UOMId = header.UOMId,
                    IsBundled = false,
                    StockBatchNumberId = header.StockBatchNumberId // ADDED 10/05/2026
                }, transaction);
            }

            // 2. Process the components items
            // Positive Qty = use the component, so remove it from stock
            // Negative Qty = return the component, so add it back to stock
            foreach (var d in details)
            {
                // Skip rows without a stock item or quantity
                if (d.StockId is null || d.Qty is null || d.Qty == 0)
                    continue;

                // Change the sign:
                // Positive Qty becomes negative (OUT)
                // Negative Qty becomes positive (IN)
                var qty = -d.Qty.Value;

                // Get the component cost
                var price = d.UnitCost ?? 0m;

                // Calculate the total amount
                var amount = qty * price;

                await connection.ExecuteAsync(insertQuery, new
                {
                    Id = Guid.NewGuid(),
                    StockId = d.StockId,
                    TransactionDate = header.AssemblyDate,
                    TransactionTime = DateTime.Now.TimeOfDay,
                    DocumentCode = header.AssemblyCode,
                    DocType = "StockAssembly",
                    Description = header.Description,
                    ItemDescription = d.Description,
                    Qty = qty,
                    Amount = amount,
                    NetAmount = amount,
                    AmountLocal = amount,
                    NetAmountLocal = amount,
                    TaxExclusiveAmountLocal = amount,
                    UOMQty = qty,
                    UOMPrice = price,
                    DiscountAmount = 0m,
                    UOMPriceExcLandingCost = price,
                    LocationId = d.StockLocationId ?? header.StockLocationId,
                    Projectid = header.ProjectId,
                    CostCentreId = (int?)null,
                    IsCancelled = false,
                    DocumentId = header.Id,
                    UOMId = d.UOMId ?? header.UOMId,
                    IsBundled = false,
                    StockBatchNumberId = d.StockBatchNumberId // ADDED 10/05/2026

                }, transaction);
            }

            // Save all changes
            transaction.Commit();
            return "Success: StockTransactions synced";
        }
        catch (SqlException ex)
        {
            // Undo all changes if something goes wrong
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }

    // Check if Stock will go below zero
    // Only check items that will be removed from stock (Qty > 0)
    // Negative Qty means stock is being added, so don't check it
    public async Task<List<NegativeInventoryLine>> CheckNegativeInventory(
    StockAssemblyModel header,
    List<StockAssemblyDetail> details)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);

        var result = new List<NegativeInventoryLine>();


        var grouped = details
    .Where(d => d.StockId != null && d.Qty is decimal q && q > 0)
    .GroupBy(d => new
    {
        StockId = d.StockId!.Value,
        LocationId = d.StockLocationId ?? header.StockLocationId,
        BatchId = d.StockBatchNumberId          // <-- add batch to the grouping
    })
    .Select(g => new { g.Key.StockId, g.Key.LocationId, g.Key.BatchId, Qty = g.Sum(x => x.Qty ?? 0) });

        foreach (var line in grouped)
        {
            var stock = await connection.QueryFirstOrDefaultAsync<Stocks>(
                "SELECT * FROM Stocks WHERE Id = @Id", new { Id = line.StockId });
            if (stock == null || stock.AllowNegativeStock == true) continue;

            var onHand = await connection.ExecuteScalarAsync<decimal?>(
                @"SELECT SUM(Qty) FROM StockTransactions
                  WHERE StockId = @StockId
                    AND IsCancelled = 0
                    AND DocumentId <> @DocumentId
                    AND (@LocationId IS NULL OR LocationId = @LocationId)
                    AND (@BatchId IS NULL OR StockBatchNumberId = @BatchId)",
                new { line.StockId, line.LocationId, line.BatchId, DocumentId = header.Id }) ?? 0m;

            // finished good produced by this same document (same stock, location and batch)
            var headerInQty =
                header.StockId == line.StockId
                && header.Qty is decimal hq && hq > 0
                && (line.LocationId == null || line.LocationId == header.StockLocationId)
                && (line.BatchId == null || line.BatchId == header.StockBatchNumberId)
                    ? hq : 0m;

            var projectedBalance = onHand + headerInQty - line.Qty;   // <-- use headerInQty

            if (projectedBalance < 0)
            {
                var locationCode = line.LocationId != null ? await GetLocationCode(line.LocationId.Value) : string.Empty;
                result.Add(new NegativeInventoryLine
                {
                    StockName = stock.StockName,
                    StockCode = stock.StockCode,
                    LocationCode = locationCode,
                    OnHand = onHand,
                    OutQty = -(line.Qty - headerInQty),   // -9, same as QNE
                    StockBalance = projectedBalance
                });
            }
        }
        return result;
    }


    public async Task<List<NegativeBatchNoLine>> CheckNegativeBatchNo(
        StockAssemblyModel header,
        List<StockAssemblyDetail> details)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);

        var result = new List<NegativeBatchNoLine>();

        // Only rows that consume stock (positive Qty) AND have a batch selected
        var grouped = details
            .Where(d => d.StockId != null
                     && d.StockBatchNumberId != null
                     && d.Qty is decimal q && q > 0)
            .GroupBy(d => new
            {
                StockId = d.StockId!.Value,
                BatchId = d.StockBatchNumberId!.Value,
                LocationId = d.StockLocationId ?? header.StockLocationId
            })
            .Select(g => new
            {
                g.Key.StockId,
                g.Key.BatchId,
                g.Key.LocationId,
                Qty = g.Sum(x => x.Qty ?? 0)
            });

        foreach (var line in grouped)
        {
            var stock = await connection.QueryFirstOrDefaultAsync<Stocks>(
                "SELECT * FROM Stocks WHERE Id = @Id", new { Id = line.StockId });

            if (stock == null || stock.AllowNegativeStock == true)
                continue;

            var batch = await connection.QueryFirstOrDefaultAsync<StockBatchNumbersModel>(
                "SELECT * FROM StockBatchNumbers WHERE Id = @Id", new { Id = line.BatchId });

            if (batch == null)
                continue;

            // LIVE balance from StockTransactions — matches FN_StockLedgerWithBatch,
            // NOT the static StockBatchNumbers.BalanceQty column.
            // ADDED 10/05/2026
            var available = await connection.ExecuteScalarAsync<decimal?>(
                @"SELECT SUM(Qty) FROM StockTransactions
                  WHERE StockId = @StockId
                  AND StockBatchNumberId = @BatchId
                  AND IsCancelled = 0
                  AND DocumentId <> @DocumentId          -- ignore this document's own earlier postings (edit case)
                  AND (@LocationId IS NULL OR LocationId = @LocationId)",
                new { StockId = line.StockId, BatchId = line.BatchId, LocationId = line.LocationId, DocumentId = header.Id }) ?? 0m;

            // The finished good created by this same document is an IN movement
            var headerInQty = 0m;
            if (header.StockId == line.StockId
                && header.StockBatchNumberId == line.BatchId
                && header.Qty is decimal hq && hq > 0
                && (line.LocationId == null || line.LocationId == header.StockLocationId))
            {
                headerInQty = hq;
            }

            var projectedBatchBalance = available - line.Qty;

            if (projectedBatchBalance < 0)
            {
                var locationCode = line.LocationId != null
                    ? await GetLocationCode(line.LocationId.Value)
                    : string.Empty;

                result.Add(new NegativeBatchNoLine
                {
                    StockCode = stock.StockCode,
                    BatchNo = batch.BatchNo,
                    Available = available,
                    OutQty = -line.Qty,
                    BatchNoBalance = projectedBatchBalance,
                    LocationCode = locationCode
                });
            }
        }
        return result;
    }


    // GET STOCK CODE, AND STOCK NAME USED IN (DISCOUNT ITEMS MODULE)

    public async Task<IEnumerable<Stocks>> GetStockDiscountItemsList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"
            SELECT Id,
	               StockCode,
	               StockName,
	               SalesDiscount,
	               PurchaseDiscount
            FROM Stocks WHERE ItemTypeCode = 'D' 
            ORDER BY StockCode DESC";
        var model = await connection.QueryAsync<Stocks>(query, commandType: CommandType.Text);
        return model;

    }

    // SAVE OR UPDATE STOCK (USED BY DISCOUNT ITEMS MODULE)
    public async Task<Guid> SaveStockDiscountItems(Stocks header)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);
        {
            // check if the record already exists
            var existing = await connection.QueryFirstOrDefaultAsync<Stocks>(
                "SELECT Id FROM Stocks WHERE Id = @Id", new
                {
                    header.Id
                });

            if (existing == null)
            {
                // Insert new record
                var insertQuery = @"
                INSERT INTO Stocks (
                    Id, StockCode, ItemTypeId, StockName, StockName2, Description,
                    CategoryId, ClassId, GroupId, BaseUOM, MinQty, MaxQty, ReorderLevel,
                    ReorderQty, SalesTax, SalesTaxGLAccountId, OpeningQty, OpeningCost,
                    ListPrice, MinPrice, LastSellingPrice, SalesDiscount, PurchasePrice,
                    PurchaseDiscount, LastPurchasePrice, BarCode, Weight, WeightUOM, Volumn,
                    VolumnUOM, DefaultLocationId, Notes, CurrentBalance, LastSellingDate,
                    LastPurchaseDate, IsActive, IsBundled, LastPurchasePriceWithLandingCost, StockAccountPresetId,
                    CreateDate, StockControl, ItemTypeCode, FurtherDescription, DefaultInputTaxCodeId, DefaultOutputTaxCodeId,
                    UseSerialNo, SerialNoPrefix, SerialNoSuffix, OptimisticLockField, Remark1, Remark2, Remark3,
                    Remark4, Remark5, HSN, UseBatchNo, UseDefaultUnsalablePeriod, UnsalablePeriod, CommissionGroupId,
                    AllowNegativeStock, SSTTariffCode, TariffCodeId, DiscountApplyToQty, FloorPrice, DisplayInModules, PointMgtQty,
                    PointMgtPointsPerQty, ClassificationId, MarkupDiscountPrice, EInvoiceValidation    
                ) VALUES (
                    @Id, @StockCode, @ItemTypeId, @StockName, @StockName2, @Description,
                    @CategoryId, @ClassId, @GroupId, @BaseUOM, @MinQty, @MaxQty, @ReorderLevel,
                    @ReorderQty, @SalesTax, @SalesTaxGLAccountId, @OpeningQty, @OpeningCost,
                    @ListPrice, @MinPrice, @LastSellingPrice, @SalesDiscount, @PurchasePrice,
                    @PurchaseDiscount, @LastPurchasePrice, @BarCode, @Weight, @WeightUOM, @Volumn,
                    @VolumnUOM, @DefaultLocationId, @Notes, @CurrentBalance, @LastSellingDate,
                    @LastPurchaseDate, @IsActive, @IsBundled, @LastPurchasePriceWithLandingCost, @StockAccountPresetId,
                    @CreateDate, @StockControl, @ItemTypeCode, @FurtherDescription, @DefaultInputTaxCodeId, @DefaultOutputTaxCodeId,
                    @UseSerialNo, @SerialNoPrefix, @SerialNoSuffix, @OptimisticLockField, @Remark1, @Remark2, @Remark3,
                    @Remark4, @Remark5, @HSN, @UseBatchNo, @UseDefaultUnsalablePeriod, @UnsalablePeriod, @CommissionGroupId,
                    @AllowNegativeStock, @SSTTariffCode, @TariffCodeId, @DiscountApplyToQty, @FloorPrice, @DisplayInModules, @PointMgtQty,
                    @PointMgtPointsPerQty, @ClassificationId, @MarkupDiscountPrice, @EInvoiceValidation 
                )";
                await connection.ExecuteAsync(insertQuery, header);
            }
            else
            {
                // Update existing record
                var updateQuery = @"
                    UPDATE Stocks SET
                    StockCode = @StockCode, ItemTypeId = @ItemTypeId, StockName = @StockName, StockName2 = @StockName2, Description = @Description,
                    CategoryId = @CategoryId, ClassId = @ClassId, GroupId = @GroupId, BaseUOM = @BaseUOM, MinQty = @MinQty, MaxQty = @MaxQty, ReorderLevel = @ReorderLevel,
                    ReorderQty = @ReorderQty, SalesTax = @SalesTax, SalesTaxGLAccountId = @SalesTaxGLAccountId, OpeningQty = @OpeningQty, OpeningCost = @OpeningCost,
                    ListPrice = @ListPrice, MinPrice = @MinPrice, LastSellingPrice = @LastSellingPrice, SalesDiscount = @SalesDiscount, PurchasePrice = @PurchasePrice,
                    PurchaseDiscount = @PurchaseDiscount, LastPurchasePrice = @LastPurchasePrice, BarCode = @BarCode, Weight = @Weight, WeightUOM = @WeightUOM, Volumn = @Volumn,
                    VolumnUOM = @VolumnUOM, DefaultLocationId = @DefaultLocationId, Notes = @Notes, CurrentBalance = @CurrentBalance, LastSellingDate = @LastSellingDate,
                    LastPurchaseDate = @LastPurchaseDate, IsActive = @IsActive, IsBundled = @IsBundled, LastPurchasePriceWithLandingCost = @LastPurchasePriceWithLandingCost, StockAccountPresetId = @StockAccountPresetId,
                    CreateDate = @CreateDate, StockControl = @StockControl, ItemTypeCode = @ItemTypeCode, FurtherDescription = @FurtherDescription, DefaultInputTaxCodeId = @DefaultInputTaxCodeId, DefaultOutputTaxCodeId = @DefaultOutputTaxCodeId,
                    UseSerialNo = @UseSerialNo, SerialNoPrefix = @SerialNoPrefix, SerialNoSuffix = @SerialNoSuffix, OptimisticLockField = @OptimisticLockField, Remark1 = @Remark1, Remark2 = @Remark2, Remark3 = @Remark3,
                    Remark4 = @Remark4, Remark5 = @Remark5, HSN = @HSN, UseBatchNo = @UseBatchNo, UseDefaultUnsalablePeriod = @UseDefaultUnsalablePeriod, UnsalablePeriod = @UnsalablePeriod, RowIndex = @RowIndex, CommissionGroupId = @CommissionGroupId,
                    AllowNegativeStock = @AllowNegativeStock, SSTTariffCode = @SSTTariffCode, TariffCodeId = @TariffCodeId, DiscountApplyToQty = @DiscountApplyToQty, FloorPrice = @FloorPrice, DisplayInModules = @DisplayInModules, PointMgtQty = @PointMgtQty,
                    PointMgtPointsPerQty = @PointMgtPointsPerQty, ClassificationId = @ClassificationId, MarkupDiscountPrice = @MarkupDiscountPrice, EInvoiceValidation = @EInvoiceValidation
                    WHERE Id = @Id";
                await connection.ExecuteAsync(updateQuery, header);
            }
        }
        return header.Id;
    }

    // GET STOCK CODE, AND STOCK NAME USED IN (DISCOUNT ITEMS MODULE)
    public async Task<IEnumerable<Stocks>> GetStocksSubTotalItemsList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);
        var query = @"
            SELECT Id,
	               StockCode,
	               StockName
            FROM Stocks WHERE ItemTypeCode = 'T' 
            ORDER BY StockCode DESC";
        var model = await connection.QueryAsync<Stocks>(query, commandType: CommandType.Text);
        return model;

    }

    // SAVE / UPDATE MULTIPLE STOCK BATCH NUMBERS (used by Stock Ins on Save)
    public async Task<string> SaveStockBatchNumbers(List<StockBatchNumbersModel> batches)
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var insertQuery = @"
        INSERT INTO StockBatchNumbers (
            Id, StockId, BatchNo, Description, Pos, BatchNoManufacturingDate,
            BatchNoExpiryDate, BalanceQty, OptimisticLockField, BatchNoRef1, BatchNoRef2
        ) VALUES (
            @Id, @StockId, @BatchNo, @Description, @Pos, @BatchNoManufacturingDate,
            @BatchNoExpiryDate, @BalanceQty, @OptimisticLockField, @BatchNoRef1, @BatchNoRef2
        )";

        var updateQuery = @"
        UPDATE StockBatchNumbers SET
            BatchNo = @BatchNo,
            Description = @Description,
            BatchNoManufacturingDate = @BatchNoManufacturingDate,
            BatchNoExpiryDate = @BatchNoExpiryDate,
            BatchNoRef1 = @BatchNoRef1,
            BatchNoRef2 = @BatchNoRef2,
            OptimisticLockField = ISNULL(OptimisticLockField, 0) + 1
        WHERE Id = @Id";

        try
        {
            foreach (var b in batches)
            {
                var exists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM StockBatchNumbers WHERE Id = @Id",
                    new { b.Id }, transaction) > 0;

                if (!exists)
                {
                    var nextPos = await connection.ExecuteScalarAsync<int>(
                        "SELECT ISNULL(MAX(Pos), 0) + 1 FROM StockBatchNumbers WHERE StockId = @StockId",
                        new { b.StockId }, transaction);

                    await connection.ExecuteAsync(insertQuery, new
                    {
                        b.Id,
                        b.StockId,
                        b.BatchNo,
                        b.Description,
                        Pos = nextPos,
                        b.BatchNoManufacturingDate,
                        b.BatchNoExpiryDate,
                        BalanceQty = b.BalanceQty ?? 0m,
                        OptimisticLockField = 1,
                        b.BatchNoRef1,
                        b.BatchNoRef2
                    }, transaction);
                }
                else
                {
                    await connection.ExecuteAsync(updateQuery, b, transaction);
                }
            }

            transaction.Commit();
            return "Success: Batch numbers saved";
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return $"Error: {ex.Message}";
        }
    }

    /// OCTOBER 15, 2026
    public async Task<IEnumerable<StockGLAccounts>> GetStockSalesAccountList()
    {
        var connectionString = await qneConnection.GetConnectionStringAsync();

        var connection = new SqlConnection(connectionString);
        var query = @"SELECT ST.Id AS StockId, 
        StockCode, 
        StockName, 
        ST.StockAccountPresetId
        ,STAP.SalesAccountId
        ,STAP.SalesReturnAccountId
        ,STAP.CashSalesAccountId
        ,STAP.PurchaseAccountId
        ,STAP.PurchaseReturnAccountId
        FROM Stocks ST
        LEFT JOIN StockAccountPresets STAP ON STAP.Id = ST.StockAccountPresetId";
        var model = await connection.QueryAsync<StockGLAccounts>(query, commandType: CommandType.Text);
        return model;
    }



}

