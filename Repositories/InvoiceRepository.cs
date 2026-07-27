using BussinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        public Invoice CalculateTotalCharges(int bookingId)
        {
            return InvoiceDao.CalculateTotalCharges(bookingId);
        }

        public decimal GetRevenueReport(int? month, int year)
        {
            return InvoiceDao.GetRevenueReport(month, year);
        }
    }
}
