using Accounting_V4.Model.QNEModels.StocksModels;

namespace Accounting_V4.Services.QNEServices.UOMServices;

public class UomConversionService : IUomConversionService
{

    // public static (decimal uom1, decimal uom2) ComputePacking(decimal qty, decimal selectedUomRate, List<UOMs> uoms)
    public (decimal uom1, decimal uom2) ComputePackingSalesUOM(decimal qty, decimal selectedUomRate, List<UOMs> uoms)
    {
        if (uoms == null || !uoms.Any()) return (0, 0);

        var activeUoms = uoms.Where(x => x.IsActive).ToList();
        if (!activeUoms.Any()) return (0, 0);

        var highest = activeUoms
            .OrderByDescending(x => x.Rate)
            .FirstOrDefault();

        var baseUom = activeUoms
            .OrderBy(x => x.Rate)
            .FirstOrDefault();

        if (highest == null || baseUom == null) return (0, 0);

        decimal pack1 = 0;
        decimal pack2 = 0;

        // If the selected UOM is the base UOM, the entire quantity goes to pack2
        if (selectedUomRate == baseUom.Rate)
        {
            pack1 = 0;
            pack2 = qty;
        }
        else
        {
            // Convert input quantity to the absolute Base Unit
            decimal totalBaseQty = qty * selectedUomRate;

            // Calculate Packing UOM 1
            pack1 = Math.Floor(totalBaseQty / highest.Rate);

            // Calculate the Remainder in Base Units
            decimal remainderBase = totalBaseQty - (pack1 * highest.Rate);

            // Packing UOM 2 is the raw remainder in the Base Unit
            pack2 = remainderBase / baseUom.Rate;
        }

        return (pack1, pack2);
    }


    ///THERES AND ISSUE HERE ON CONVERTING UOMPACKING FOR PURCHASEREQUISITION
    public (decimal uom1, decimal uom2) ComputePackingPurchasingUOM(decimal qty, decimal selectedUomRate, List<UOMs> uoms)
    {
        if (uoms == null || !uoms.Any()) return (0, 0);

        var activeUoms = uoms.Where(x => x.IsActive).ToList();
        if (!activeUoms.Any()) return (0, 0);

        var highest = activeUoms
            .OrderByDescending(x => x.Rate)
            .FirstOrDefault();

        var baseUom = activeUoms
            .OrderBy(x => x.Rate)
            .FirstOrDefault();

        if (highest == null || baseUom == null) return (0, 0);

        decimal pack1 = 0;
        decimal pack2 = 0;

        // If the selected UOM is the base UOM, the entire quantity goes to pack2
        if (selectedUomRate == baseUom.Rate)
        {
            pack1 = 0;
            pack2 = qty;
        }
        else
        {
            // Convert input quantity to the absolute Base Unit
            decimal totalBaseQty = qty * selectedUomRate;

            // Calculate Packing UOM 1
            pack1 = Math.Floor(totalBaseQty / highest.Rate);

            // Calculate the Remainder in Base Units
            decimal remainderBase = totalBaseQty - (pack1 * highest.Rate);

            // Packing UOM 2 is the raw remainder in the Base Unit
            pack2 = remainderBase / baseUom.Rate;
        }

        return (pack1, pack2);
    }
}


