using BussinessObjects;

namespace Services
{
    public interface IInvoiceService
    {
        Invoice CalculateTotalCharges(int bookingId);

        decimal GetRevenueReport(int? month, int year);
    }
}
