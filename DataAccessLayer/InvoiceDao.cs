using BussinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class InvoiceDao
    {
        public InvoiceDao() { }

        public static Invoice CalculateTotalCharges(int bookingId)
        {
            using (ManagementHotelNewContext context = new ManagementHotelNewContext())
            {
                var booking = context.Bookings
                             .Include(b => b.Room)
                             .Include(b => b.Serviceorders)
                             .Include(b => b.Payments)
                             .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                    throw new Exception("Booking not found.");

                if (booking.BookingStatus != "CheckedOut")
                    throw new Exception("Invoice can only be created after check-out.");

                //Tinh so ngay
                DateTime checkin = booking.CheckInDate ?? booking.ExpectedCheckIn;
                DateTime checkout = booking.CheckOutDate ?? DateTime.Now;

                int days = (int)Math.Ceiling((checkout.Date - checkin.Date).TotalDays);

                if (days <= 0) days = 1;

                //tinh tien
                decimal roomCharge = booking.Room.RoomType.BasePrice * days;

                decimal serviceCharge = booking.Serviceorders.Sum(r => r.Price * r.Quantity) * days;

                decimal totalExpense = roomCharge + serviceCharge;

                decimal reamaining;

                decimal paymentOnline = booking.Payments.Where(
                    p => p.PaymentMethod == "Online")
                    .Sum(p => p.Amount);

                if (paymentOnline > 0)
                {
                    reamaining = totalExpense - paymentOnline;
                }
                else
                {
                    reamaining = totalExpense;
                }
                var payment = booking.Payments
                             .OrderByDescending(p => p.PaymentDate)
                             .FirstOrDefault();

                var invoice = new Invoice
                {
                    BookingId = bookingId,
                    RoomCharge = roomCharge,
                    ServiceCharge = serviceCharge,
                    Discount = 0,
                    TotalAmount = totalExpense,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = payment.PaymentMethod
                };

                context.Invoices.Add(invoice);
                context.SaveChanges();
                return invoice;
            }
        }
        public static decimal GetRevenueReport(int? month, int year)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var query = context.Invoices
                    .Where(i => i.PaymentDate.HasValue &&
                                i.PaymentDate.Value.Year == year);

                if (month.HasValue)
                {
                    query = query.Where(i => i.PaymentDate.Value.Month == month.Value);
                }

                return query.Sum(i => i.TotalAmount ?? 0);
            }
        }
    }
}
