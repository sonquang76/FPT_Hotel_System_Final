using BussinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class PaymentDao
    {
        public PaymentDao() { }
        public static bool ProcessOnlineDeposit(int bookingId)
        {
            using (ManagementHotelNewContext context = new ManagementHotelNewContext())
            {
                var exiting = context.Bookings
                             .Include(b => b.Room)
                                 .ThenInclude(r => r.RoomType)
                             .FirstOrDefault(b => b.BookingId == bookingId);
                if (exiting == null) return false;

                int NumberOfDay = (exiting.ExpectedCheckOut - exiting.ExpectedCheckIn).Days;

                decimal OnlDeposit = NumberOfDay * exiting.Room.RoomType.BasePrice * 0.3m;

                bool payAlready = context.Payments.Any(
                    p => p.BookingId == bookingId && p.PaymentStatus == "Paid"
                    );

                if (payAlready) return false;

                Payment payment = new Payment()
                {
                    BookingId = bookingId,
                    Amount = OnlDeposit,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Online",
                    PaymentStatus = "Paid"
                };

                context.Payments.Add(payment);

                exiting.BookingStatus = "Booked";

                return context.SaveChanges() > 0;

            }
        }
        public static bool Refund(int bookingId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var payments = context.Payments.FirstOrDefault(
                    p => p.BookingId == bookingId && p.PaymentStatus == "Paid"
                    );

                if (payments == null) return false;

                payments.PaymentStatus = "Refunded";
                return context.SaveChanges() > 0;
            }
        }

    }
}
