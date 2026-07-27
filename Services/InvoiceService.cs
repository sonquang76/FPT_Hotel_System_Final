using BussinessObjects;
using Repositories;

namespace Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository repository;
        public InvoiceService()
        {
            this.repository = new InvoiceRepository();
        }
        public Invoice CalculateTotalCharges(int bookingId)
        {
            return this.repository.CalculateTotalCharges(bookingId);
        }

        public decimal GetRevenueReport(int? month, int year)
        {
            return this.repository.GetRevenueReport(month, year);
        }
    }
}
