using Accounting_V4.Model.QNEModels.TransactionCodesModels;

namespace Accounting_V4.Services.QNEServices.AllBasicServices;

public class TermsDayMonthServices
{
    public async Task<DateTime> ComputeDaysMonth(DateTime daysMonth, int termDays)
    {
        if (termDays == 0)
        {
            return await Task.FromResult(daysMonth);
        }

        var DueDate = daysMonth.AddDays(termDays);
        return await Task.FromResult(DueDate);
    }
}