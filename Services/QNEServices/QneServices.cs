using Accounting_V4.ConnectionServices;
using Accounting_V4.Model;
using Accounting_V4.Model.QNEModels.SalesModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices
{
    public class QneServices
    {
        private readonly QneConnectionString _qneConnection;

        public QneServices(QneConnectionString qneConnection)
        {
            _qneConnection = qneConnection;
        }


        public async Task<IEnumerable<OutStandingInvoice>> GetOutstandingInvoiceAsync(OutStandingInvoiceFilter filter)
        {
            var connectionString = await _qneConnection.GetConnectionStringAsync();
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<OutStandingInvoice>(
                @"SELECT * FROM FN_OutstandingInvoicesWithDetails(
            @date,
            @withPDCheques,
            @includeFutureKnockOff
        );",
                new
                {
                    date = filter.Date,
                    withPDCheques = filter.withPDCheques,
                    includeFutureKnockOff = filter.includeFutureKnockOff
                }, commandTimeout: 1200);
        }



        public async Task<IEnumerable<VW_Quotations>> GetQuotationAsync()
        {
            var connectionString = await _qneConnection.GetConnectionStringAsync();
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<VW_Quotations>(
                @"SELECT
                VWQ.Id,
                CRNCY.CurrencyCode,
                QuotationCode,
                QuotationDate,
                DebtorId,
                DebtorName,
                Address1,
                Address2,
                Address3,
                Address4,
                ReferenceNo,
                TermId,
                SalesPersonId,
                DeliveryTerm,
                Ref1,
                Ref2,
                Ref3,
                Ref4,
                Ref5,
                Remark1,
                Remark2,
                Remark3,
                Remark4,
                Remark5,
                ProjectId,
                DepartmentId,
                StockLocationId,
                CurrencyId,
                CurrencyRate,
                TotalAmount,
                NetTotalAmountLocal,
                IsCancelled,
                DOAddress1,
                DOAddress2,
                DOAddress3,
                DOAddress4,
                CostCentreId,
                Discount,
                DiscountAmount,
                NetTotalAmount,
                DiscountAmountLocal,
                TotalAmountLocal,
                TaxTotalAmount,
                TaxTotalAmountLocal,
                SubtotalAmount,
                SubtotalAmountLocal,
                ExportCountry,
                Notes,
                IsTaxInclusive,
                TaxDate,
                TaxExclusiveTotalAmount,
                TaxExclusiveTotalAmountLocal,
                IsTaxInclusiveOnly,
                DeliveryArea,
                SgstTotalAmount,
                SgstTotalAmountLocal,
                CgstTotalAmount,
                CgstTotalAmountLocal,
                IgstTotalAmount,
                IgstTotalAmountLocal,
                TransferToInfo,
                IsApproved,
                VWQ.ReverseRate,
                SIRef1,
                SIRef2,
                TotalServiceAmount,
                TotalServiceAmountLocal,
                CustomerCode,
                CustomerName,
                CustomerName2,
                Term,
                StaffCode,
                AreaCode,
                CategoryCode,
                ControlAccountCode,
                VWQ.OptimisticLockField
                 FROM VW_Quotations AS VWQ
    LEFT OUTER JOIN Currencies AS CRNCY
    ON VWQ.CurrencyId = CRNCY.Id
    ORDER BY QuotationDate DESC
        ");
        }
    }
}
