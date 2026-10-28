using Accounting_V4.Model.QNEModels.SupplierModels;

namespace Accounting_V4.Services.QNEServices.TransactionCodeServices;

public class BillsTransactionHelper
{
    public static void Compute(BillDetails row)
    {
        decimal amount = row.Amount;

        // Reset
        row.TaxExclusiveAmount = amount;
        row.TaxExclusiveAmountLocal = amount;

        row.SubAmount = amount;
        row.SubAmountLocal = amount;

        row.TaxAmount = 0m;
        row.TaxAmountLocal = 0m;

        row.WTaxAmount = 0m;
        row.WTaxAmountLocal = 0m;

        // ==========================
        // VAT COMPUTATION
        // ==========================
        // VAT
        if ((row.TaxRate ?? 0m) > 0)
        {
            decimal taxRate = row.TaxRate.Value;

            if (row.IsTaxInclusive == true)
            {
                decimal divisor = 1 + taxRate;

                row.TaxExclusiveAmount = amount / divisor;
                row.TaxExclusiveAmountLocal = row.TaxExclusiveAmount;

                row.TaxAmount = amount - row.TaxExclusiveAmount;
                row.TaxAmountLocal = row.TaxAmount;
            }
            else
            {
                row.TaxExclusiveAmount = amount;
                row.TaxExclusiveAmountLocal = amount;

                row.TaxAmount = amount * taxRate;
                row.TaxAmountLocal = row.TaxAmount;
            }
        }

        // WTax
        if ((row.WTaxRate ?? 0m) > 0)
        {
            row.WTaxAmount = (row.TaxExclusiveAmount ?? 0m) * row.WTaxRate.Value;
            row.WTaxAmountLocal = row.WTaxAmount;
        }

        // Same as original
        row.NetAmount = Math.Round((row.TaxExclusiveAmount ?? 0m) +(row.TaxAmount ?? 0m), 2);

        row.NetAmountLocal = row.NetAmount;
    }
}
