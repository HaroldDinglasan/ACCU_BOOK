using Accounting_V4.Model.QNEModels.GeneralLedgerModels;
using Accounting_V4.Model.QNEModels.SupplierModels;

namespace Accounting_V4.Services.QNEServices.TransactionCodeServices;

public class JournalTransactionHelper
{
    public static void Compute(JournalDetails row)
    {
        // =========================================================
        // DEBIT
        // =========================================================

        row.TaxDebit = 0;
        row.TaxDebitLocal = 0;

        row.NetDebit = 0;
        row.NetDebitLocal = 0;

        row.WTaxDebit = 0;

        row.TaxExclusiveDebit = row.Debit;
        row.TaxExclusiveDebitLocal = row.DebitLocal;

        if (row.Debit != 0.00M && row.Debit > 0)
        {
            decimal debit = row.Debit;

            if (row.IsTaxInclusive &&
                row.TaxRate.HasValue &&
                row.TaxRate.Value > 0)
            {
                decimal taxRate = row.TaxRate.Value;

                // Tax Inclusive
                row.TaxExclusiveDebit =
                    debit / (1 + taxRate);

                row.TaxExclusiveDebitLocal =
                    (row.DebitLocal) / (1 + taxRate);

                row.TaxDebit =
                    debit - row.TaxExclusiveDebit;

                row.TaxDebitLocal =
                    (row.DebitLocal) - row.TaxExclusiveDebitLocal;
            }
            else
            {
                // Tax Exclusive
                row.TaxExclusiveDebit = debit;
                row.TaxExclusiveDebitLocal = row.DebitLocal;

                if (row.TaxRate.HasValue && row.TaxRate.Value > 0)
                {
                    decimal taxRate = row.TaxRate.Value;

                    row.TaxDebit =
                        Math.Max(
                            (row.TaxExclusiveDebit) * taxRate,
                            0
                        );

                    row.TaxDebitLocal =
                        Math.Max(
                            (row.TaxExclusiveDebitLocal) * taxRate,
                            0
                        );
                }
            }



          // Withholding Tax
          
            if (row.WTaxRate.HasValue && row.WTaxRate.Value > 0)
            {
                row.WTaxDebit = Math.Max((row.TaxExclusiveDebit) * (row.WTaxRate.Value), 0);
            }


            // Net Debit
            row.NetDebit =Math.Round((row.TaxExclusiveDebit) + row.TaxDebit,2);

            row.NetDebitLocal =Math.Round((row.TaxExclusiveDebitLocal) + row.TaxDebitLocal,2);
        }


        // =========================================================
        // CREDIT
        // =========================================================

        row.TaxCredit = 0;
        row.TaxCreditLocal = 0;

        row.NetCredit = 0;
        row.NetCreditLocal = 0;

        row.WTaxCredit = 0;

        row.TaxExclusiveCredit = row.Credit;
        row.TaxExclusiveCreditLocal = row.CreditLocal;

        if (row.Credit != 0.00M && row.Credit > 0)
        {
            decimal credit = row.Credit;

            if (row.IsTaxInclusive &&
                row.TaxRate.HasValue &&
                row.TaxRate.Value > 0)
            {
                decimal taxRate = row.TaxRate.Value;

                // Tax Inclusive
                row.TaxExclusiveCredit = credit / (1 + taxRate);

                row.TaxExclusiveCreditLocal = (row.CreditLocal) / (1 + taxRate);

                row.TaxCredit = credit - row.TaxExclusiveCredit;

                row.TaxCreditLocal = (row.CreditLocal) - row.TaxExclusiveCreditLocal;
            }
            else
            {
                // Tax Exclusive
                row.TaxExclusiveCredit = credit;
                row.TaxExclusiveCreditLocal = row.CreditLocal;

                if (row.TaxRate.HasValue && row.TaxRate.Value > 0)
                {
                    decimal taxRate = row.TaxRate.Value;

                    row.TaxCredit =
                        Math.Max(
                            (row.TaxExclusiveCredit) * taxRate,
                            0
                        );

                    row.TaxCreditLocal =
                        Math.Max(
                            (row.TaxExclusiveCreditLocal) * taxRate,
                            0
                        );
                }
            }

            // Withholding Tax
         
            if (row.WTaxRate.HasValue && row.WTaxRate.Value > 0)
            {
                row.WTaxCredit = Math.Max((row.TaxExclusiveCredit) * (row.WTaxRate.Value), 0);
            }


            // Net Credit
            row.NetCredit = Math.Round((row.TaxExclusiveCredit) + row.TaxCredit,2);

            row.NetCreditLocal = Math.Round((row.TaxExclusiveCreditLocal) + row.TaxCreditLocal,2);
        }
    }
}
