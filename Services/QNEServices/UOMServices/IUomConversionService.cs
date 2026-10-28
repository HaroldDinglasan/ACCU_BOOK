using Accounting_V4.Model.QNEModels.StocksModels;

namespace Accounting_V4.Services.QNEServices.UOMServices;

public interface IUomConversionService
{
    (decimal uom1, decimal uom2)
      ComputePackingSalesUOM(
          decimal qty,
          decimal selectedUomRate,
          List<UOMs> uoms
      );

    (decimal uom1, decimal uom2)
     ComputePackingPurchasingUOM(
         decimal qty,
         decimal selectedUomRate,
         List<UOMs> uoms
     );
}
