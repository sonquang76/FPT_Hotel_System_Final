using BussinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class ServiceOrderDao
    {
        public ServiceOrderDao() { }
        public static List<Serviceorder> GetServiceorders()
        {
            using (ManagementHotelNewContext context = new ManagementHotelNewContext())
            {
                return context.Serviceorders
                    .Include(s => s.Service)
                    .ToList();
            }
        }

        public static Serviceorder CreateRestaurantOrders(Serviceorder serviceorder)
        {
            using (ManagementHotelNewContext context = new ManagementHotelNewContext())
            {
                var booking = context.Bookings
                    .FirstOrDefault(
                    b => b.BookingId == serviceorder.BookingId
                    );

                if (booking == null) return null;

                if (booking.BookingStatus != "Confirmed")
                    throw new Exception("Service can only be ordered after check-in.");

                var service = context.Services
                    .FirstOrDefault(s => s.ServiceId == serviceorder.ServiceId);

                if (service == null)
                    throw new Exception("Service not found.");

                serviceorder.Price = service.Price * serviceorder.Quantity;
                serviceorder.OrderTime = DateTime.Now;
                serviceorder.OrderStatus = "Pending";

                context.Serviceorders.Add(serviceorder);
                context.SaveChanges();
                return serviceorder;

            }
        }
        public static void ConfirmServiceOrder(int serviceorderid)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var order = context.Serviceorders.FirstOrDefault(
                    o => o.ServiceOrderId == serviceorderid
                    );
                if (order == null)
                    throw new Exception("Order not found.");
                if (order.OrderStatus != "Pending")
                    throw new Exception("Only pending orders can be confirmed.");

                order.OrderStatus = "Confirmed";
                context.SaveChanges();
            }
        }

        public static void CompleteServiceOrder(int serviceorderid)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var order = context.Serviceorders.FirstOrDefault(c => c.ServiceOrderId == serviceorderid);
                if (order == null) throw new Exception("Order not found.");

                if (order.OrderStatus != "Confirmed")
                    throw new Exception("Order has not been confirmed");

                order.OrderStatus = "Completed";
                context.SaveChanges();
            }
        }

        public static void CancelServiceOrder(int serviceorderid)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var order = context.Serviceorders.FirstOrDefault(c => c.ServiceOrderId == serviceorderid);
                if (order == null) throw new Exception("Order not found.");

                if (order.OrderStatus != "Pending")
                    throw new Exception("Only pending orders can be cancelled.");

                order.OrderStatus = "Cancelled";
                context.SaveChanges();
            }
        }

        public static List<Serviceorder> GetServiceHistory(int bookingId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Serviceorders
                    .Include(s => s.Booking)
                    .Where(s => s.BookingId == bookingId)
                    .OrderByDescending(s => s.OrderTime)
                    .ToList();
            }
        }

        public static List<Serviceorder> GetServiceUsageReport()
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Serviceorders
                    .Include(s => s.Booking)
                            .ThenInclude(b => b.Room)
                    .Include(s => s.Booking)
                            .ThenInclude(b => b.CreatedByNavigation)
                    .Include(s => s.Service)
                    .OrderByDescending(s => s.OrderTime)
                    .ToList();
            }
        }
        //Phân công chi phí dịch vụ cho phòng
        public static decimal AssignServiceCharges(int roomId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var booking = context.Bookings.FirstOrDefault(
                    b => b.RoomId == roomId
                      && b.BookingStatus == "CheckedIn");

                if (booking == null) return 0;

                var totalCharge =  context.Serviceorders
                    .Where(o => o.BookingId == booking.BookingId && o.OrderStatus == "Completed")
                    .Sum(o => o.Price);

                return totalCharge;
            }
        }
    }
}
