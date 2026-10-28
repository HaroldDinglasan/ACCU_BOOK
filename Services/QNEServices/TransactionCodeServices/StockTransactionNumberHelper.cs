using Accounting_V4.Model.QNEModels.TransactionCodesModels;

namespace Accounting_V4.Services.QNEServices.TransactionCodeServices
{
    public class StockTransactionNumberHelper
    {
        public static void Compute(StockTransDetail row)
        {
            row.Amount =
                (row.Qty ?? 0)
                *
                (row.Price ?? 0);
        }
    }
}
