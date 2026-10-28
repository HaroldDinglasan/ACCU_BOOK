using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.TransactionCodesModels;

namespace Accounting_V4.Services.QNEServices.TransactionCodeServices;

public class TransactionNumberHelper
{
    /// <summary>
    /// Computes all amounts: Amount, Discounts, Tax, WTax, NetAmount.
    /// </summary>
    /// <summary>
    /// Computes with Tax Inclusive (amount / 1.12)  If IsTaxInclusive is true
    /// </summary>
    public static void Compute(TransDetail row)
    {

        // 1. Base Amount
        row.Amount = row.Qty * row.UnitPrice;
        row.AmountLocal = row.Qty * row.UnitPrice;

        decimal discountValue = GetDiscountValue(row.Amount, row.Discount);

        row.SubAmount = discountValue > 0 ? discountValue : row.Amount;
        row.SubAmountLocal = row.SubAmount;

        // 2. Discounts
        //row.DiscountAmount = GetDiscountValue(row.Amount, row.Discount);
        //row.DiscountAmountLocal = row.DiscountAmount;
        //row.DiscountAmount = (row.Amount - (row.SubAmount ?? 0.00m))??0.00M;
        row.DiscountAmount = row.SubAmount > 0 ? row.Amount - row.SubAmount : 0.00m;
        row.DiscountAmountLocal = row.DiscountAmount;


        //3.2 Sub Amount
        //row.SubAmount = row.Amount - row.DiscountAmount;
        //row.SubAmountLocal = row.SubAmount;



        // 2.1 Tax Exclusive Amount
        //row.TaxExclusiveAmount = row.Amount - row.DiscountAmount;
        //row.TaxExclusiveAmountLocal = row.TaxExclusiveAmount;
        row.TaxExclusiveAmount = row.SubAmount;
        //  row.TaxExclusiveAmount = row.SubAmount == 0.00M ? row.Amount;
        row.TaxExclusiveAmountLocal = row.TaxExclusiveAmount;

        // 4. Tax
        row.TaxAmount = 0;
        row.TaxAmountLocal = 0;
        if (row.TaxRate.HasValue && row.TaxRate.Value > 0)
        {
            row.TaxAmount = Math.Max((row.TaxExclusiveAmount ?? 0) * (row.TaxRate.Value), 0);
            row.TaxAmountLocal = row.TaxAmount;

            // Tax Inclusive (amount / 1.12)
            if (row.IsTaxInclusive)
            {   // tanggalin ang VAT kung inclusive
                row.TaxExclusiveAmount = row.TaxExclusiveAmount / 1.12m;
                row.TaxExclusiveAmountLocal = row.TaxExclusiveAmount;

                // Tax Inclusive (amount / 1.12)
                row.TaxAmount = row.TaxAmount / 1.12m;
                row.TaxAmountLocal = row.TaxAmount;
            }

        }

        // 5. Withholding Tax
        row.WTaxAmount = 0;
        row.WTaxAmountLocal = 0;
        if (row.WTaxRate.HasValue && row.WTaxRate.Value > 0)
        {

            row.WTaxAmount = Math.Max((row.TaxExclusiveAmount ?? 0) * (row.WTaxRate.Value), 0);
            row.WTaxAmountLocal = row.WTaxAmount;
        }

        // 6. Net Amount (rounded 2 decimals)
        row.NetAmount = Math.Round((row.TaxExclusiveAmount ?? 0) + row.TaxAmount, 2);
        row.NetAmountLocal = row.NetAmount;
    }


    /// <summary>
    /// Computes total discount amount from string (e.g. "20%", "10%/5%", "100").
    /// </summary>
    /// //DEPRICATED METHOD, use GetDiscountValue instead
    public static decimal GetDiscountValueOld(decimal amount, string discountInput)
    {
        if (string.IsNullOrWhiteSpace(discountInput))
            return 0;

        var discounts = discountInput.Split('/', StringSplitOptions.RemoveEmptyEntries);

        decimal totalDiscount = 0.0000m;
        decimal currentBase = amount;

        foreach (var d in discounts)
        {
            var disc = d.Trim();

            if (disc.EndsWith("%"))
            {
                if (decimal.TryParse(disc.TrimEnd('%'), out var percent))
                {
                    var thisDiscount = currentBase * (percent / 100m);
                    totalDiscount += thisDiscount;
                    currentBase -= thisDiscount;
                }
            }
            else
            {
                if (decimal.TryParse(disc, out var number))
                {
                    var thisDiscount = number;
                    totalDiscount += thisDiscount;
                    currentBase -= thisDiscount;
                }
            }
        }

        return totalDiscount < 0 ? 0 : totalDiscount;
    }


    private static string NormalizeDiscountInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return new string(input
            .Where(c =>
                char.IsDigit(c) ||
                c == '%' ||
                c == '+' ||
                c == '/')
            .ToArray());
    }
    public static decimal GetDiscountValue(decimal amount, string discountInput)
    {
        discountInput = NormalizeDiscountInput(discountInput);
        if (string.IsNullOrWhiteSpace(discountInput))
            return 0;

        decimal current = amount;

        var parts = discountInput.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (var raw in parts)
        {
            var part = raw.Trim();

            // -------------------------
            // PERCENT (20%)
            // -------------------------
            if (part.EndsWith("%"))
            {
                if (decimal.TryParse(part.TrimEnd('%'), out var percent))
                {
                    current -= current * (percent / 100m);
                }
            }

            // -------------------------
            // TRADE (10+1 / 10+2)
            // -------------------------
            else if (part.Contains('+'))
            {
                var t = part.Split('+');

                if (t.Length == 2 &&
                    decimal.TryParse(t[0], out var buy) &&
                    decimal.TryParse(t[1], out var free))
                {
                    current = (current * buy) / (buy + free);
                }
            }

            // -------------------------
            // FIXED (100)
            // -------------------------
            else
            {
                if (decimal.TryParse(part, out var fixedValue))
                {
                    current -= fixedValue;
                }
            }
        }

        //var discount = amount - current;
        var discount = current;

        return discount < 0 ? 0 : discount;
    }
}
