using System.Text.RegularExpressions;
using Accounting_V4.Model.QNEModels.SalesModels;
using Accounting_V4.Model.QNEModels.TransactionCodesModels;

namespace Accounting_V4.Services.QNEServices.TransactionCodeServices;

public class DiscountAndSubtotalHelper
{

    public static void Compute(TransDetail row)
    {
        // 1. Base Amount
        // 2. Discounts
        //row.DiscountAmount = GetDiscountValue(row.Amount, row.Discount);
        //row.DiscountAmountLocal = row.DiscountAmount;
        row.Amount = GetDiscountValue(row.SubAmount, row.Discount);
        //row.DiscountAmountLocal = row.DiscountAmount;

        row.AmountLocal = row.Amount;

        //3.2 Sub Amount
        row.SubAmount = row.Amount;
        row.SubAmountLocal = row.SubAmount;

        row.DiscountAmount = 0.00M;
        row.DiscountAmountLocal = row.DiscountAmount;

        // 2.1 Tax Exclusive Amount
        row.TaxExclusiveAmount = row.Amount;
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
        MakeAllNegative(row);
    }


    /// <summary>
    /// Computes total discount amount from string (e.g. "20%", "10%/5%", "100").
    /// </summary>
    /// 


    public static decimal GetDiscountValue(decimal amount, string discountInput)
    {
        if (amount <= 0 || string.IsNullOrWhiteSpace(discountInput))
            return 0m;

        // ✅ remove any words (net, less, etc.)
        discountInput = Regex.Replace(discountInput, @"[^0-9%/+\-\.]", "");

        // treat + same as /
        discountInput = discountInput.Replace("+", "/");

        var parts = discountInput.Split('/', StringSplitOptions.RemoveEmptyEntries);

        decimal currentBase = amount;
        decimal totalDiscount = 0m;

        foreach (var raw in parts)
        {
            var part = raw.Trim();
            decimal thisDiscount = 0m;

            // % discount (sequential)
            if (part.EndsWith("%"))
            {
                if (decimal.TryParse(part.TrimEnd('%'), out var percent))
                    thisDiscount = currentBase * (percent / 100m);
            }
            // fixed amount
            else if (decimal.TryParse(part, out var fixedAmt))
            {
                thisDiscount = fixedAmt;
            }

            if (thisDiscount <= 0)
                continue;

            totalDiscount += thisDiscount;   // ✅ SUM of thisDiscount only
            currentBase -= thisDiscount;     // ✅ next step base
        }

        return totalDiscount < 0 ? 0 : totalDiscount;
    }

    private static void MakeAllNegative(TransDetail row)
    {
        row.Amount = ToNegative(row.Amount);
        row.AmountLocal = ToNegative(row.AmountLocal);

        row.DiscountAmount = ToNegative(row.DiscountAmount);
        row.DiscountAmountLocal = ToNegative(row.DiscountAmountLocal);

        row.SubAmount = ToNegative(row.SubAmount);
        row.SubAmountLocal = ToNegative(row.SubAmountLocal);

        row.TaxExclusiveAmount = ToNegative(row.TaxExclusiveAmount ?? 0);
        row.TaxExclusiveAmountLocal = ToNegative(row.TaxExclusiveAmountLocal ?? 0);

        row.TaxAmount = ToNegative(row.TaxAmount);
        row.TaxAmountLocal = ToNegative(row.TaxAmountLocal ?? 0);

        row.WTaxAmount = ToNegative(row.WTaxAmount);
        row.WTaxAmountLocal = ToNegative(row.WTaxAmountLocal);

        row.NetAmount = ToNegative(row.NetAmount);
        row.NetAmountLocal = ToNegative(row.NetAmountLocal);
    }

    private static decimal ToNegative(decimal value)
    {
        return value > 0 ? -value : value;
    }





}


// public static void Old2Compute(QuotationDetails row)
//{
//    // 1. Base Amount (original)
//    decimal baseAmount = row.Amount;

//    row.AmountLocal = baseAmount;

//    // 2. Discount
//    row.DiscountAmount = GetDiscountValue(baseAmount, row.Discount);
//    row.DiscountAmountLocal = row.DiscountAmount;

//    // 3. Amount AFTER discount (SINGLE SOURCE OF TRUTH)
//    decimal netBaseAmount = baseAmount - row.DiscountAmount;

//    row.Amount = netBaseAmount;
//    row.AmountLocal = netBaseAmount;

//    // 4. Sub Amount
//    row.SubAmount = netBaseAmount;
//    row.SubAmountLocal = netBaseAmount;

//    // 5. Tax Exclusive Amount
//    row.TaxExclusiveAmount = netBaseAmount;
//    row.TaxExclusiveAmountLocal = netBaseAmount;

//    // 6. Tax
//    row.TaxAmount = 0;
//    row.TaxAmountLocal = 0;

//    if (row.TaxRate.HasValue && row.TaxRate.Value > 0)
//    {
//        row.TaxAmount = Math.Max(netBaseAmount * row.TaxRate.Value, 0);
//        row.TaxAmountLocal = row.TaxAmount;

//        if (row.IsTaxInclusive)
//        {
//            row.TaxExclusiveAmount = netBaseAmount / 1.12m;
//            row.TaxExclusiveAmountLocal = row.TaxExclusiveAmount;

//            row.TaxAmount = ((decimal)row.TaxExclusiveAmount) * row.TaxRate.Value;
//            row.TaxAmountLocal = row.TaxAmount;
//        }

//    }

//    // 7. Withholding Tax
//    row.WTaxAmount = 0;
//    row.WTaxAmountLocal = 0;

//    if (row.WTaxRate.HasValue && row.WTaxRate.Value > 0)
//    {
//        row.WTaxAmount = Math.Max(row.TaxExclusiveAmount.Value * row.WTaxRate.Value, 0);
//        row.WTaxAmountLocal = row.WTaxAmount;
//    }

//    // 8. Net Amount
//    row.NetAmount = Math.Round(
//        row.TaxExclusiveAmount.Value + row.TaxAmount - row.WTaxAmount,
//        2
//    );
//    row.NetAmountLocal = row.NetAmount;

//    // 9. FORCE ALL RESULTS TO NEGATIVE
//    MakeAllNegative(row);
//}

