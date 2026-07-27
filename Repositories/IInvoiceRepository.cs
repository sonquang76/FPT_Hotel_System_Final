using BussinessObjects;

namespace Repositories
{
    public interface IInvoiceRepository
    {
        Invoice CalculateTotalCharges(int bookingId);

        decimal GetRevenueReport(int? month, int year);
    }
}
