using BussinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class BookingDao
    {
        public BookingDao() { }
        public static List<Booking> GetBooking()
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Bookings
                    .Include(b => b.CreatedByNavigation)
                    .Include(b => b.Room)
                            .ThenInclude(r => r.RoomType)
                    .Include(b => b.Serviceorders)
                            .ThenInclude(s => s.Service)
                    .Include(b => b.Invoice)
                    .ToList();
            }
        }
        public static Booking GetBookingById(int bookingId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Bookings
                    .Include(b => b.CreatedByNavigation)
                    .Include(b => b.Room)
                        .ThenInclude(r => r.RoomType)
                    .Include(b => b.Serviceorders)
                        .ThenInclude(s => s.Service)
                    .Include(b => b.Payments)
                    .Include(b => b.Invoice)
                    .FirstOrDefault(b => b.BookingId == bookingId);
            }
        }
        public static List<Booking> LoadBookingCardsData()
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Bookings
                    .Include(b => b.Room)
                    .Include(b => b.Room.RoomType)
                    .ToList();
            }
        }
        public static Booking CreateReservation(Booking booking)
        {
            using (ManagementHotelNewContext context = new ManagementHotelNewContext())
            {
                var room = context.Rooms.FirstOrDefault(r =>
                                   r.RoomId == booking.RoomId);

                if (room == null)
                    return null;

                if (room.Status != "Available")
                    return null;

                if (booking.ExpectedCheckOut <= booking.ExpectedCheckIn)
                    return null;

                // Kiểm tra phòng đã có người đặt trong khoảng thời gian này chưa
                bool isBooked = context.Bookings.Any(b =>
                   b.RoomId == booking.RoomId &&
                   b.BookingStatus != "Cancelled" &&
                   b.BookingStatus != "CheckedOut" &&
                   booking.ExpectedCheckIn < b.ExpectedCheckOut &&
                   booking.ExpectedCheckOut > b.ExpectedCheckIn);

                if (isBooked)
                    return null;

                // Thiết lập giá trị mặc định
                booking.CheckInDate = null;
                booking.CheckOutDate = null;
                booking.BookingStatus = "Booked";
                // Lưu Booking
                context.Bookings.Add(booking);
                context.SaveChanges();

                return booking;
            }
        }
        public static Booking UpdateReservation(Booking UdBooking)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var existing = context.Bookings
                                      .Include(b => b.Room)
                                      .Include(b => b.CreatedByNavigation)
                                      .FirstOrDefault(b => b.BookingId == UdBooking.BookingId);

                if (existing == null)
                    return null;


                existing.BookingStatus = UdBooking.BookingStatus;

                existing.RoomId = UdBooking.RoomId;

                var room = context.Rooms.FirstOrDefault(r => r.RoomId == UdBooking.RoomId && r.Status != "Occupied");

                if (room != null && UdBooking.Room != null)
                {
                    room.Status = UdBooking.Room.Status;
                }


                var account = context.Accounts.FirstOrDefault(a => a.AccountId == existing.CreatedBy);

                if (account != null && UdBooking.CreatedByNavigation != null)
                {
                    account.FullName = UdBooking.CreatedByNavigation.FullName;
                    account.Email = UdBooking.CreatedByNavigation.Email;
                    account.Phone = UdBooking.CreatedByNavigation.Phone;
                }

                context.SaveChanges();

                return existing;
            }
        }
        public static bool CancelBooking(int bookingId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var existing = context.Bookings.Find(bookingId);
                if (existing == null) return false;

                // CHỈ CHẶN khi đơn đặt phòng thực sự đã được Confirm hoặc CheckedIn từ lễ tân
                if (existing.BookingStatus == "Confirmed" || existing.BookingStatus == "CheckedIn")
                {
                    return false;
                }

                // Tải thông tin Room liên quan nếu chưa có
                if (existing.Room == null)
                {
                    context.Entry(existing).Reference(b => b.Room).Load();
                }

                // --- HỢP LỆ -> TIẾN HÀNH HỦY ---

                // 1. Cập nhật trạng thái đơn đặt thành Cancelled
                existing.BookingStatus = "Cancelled";

                // 2. Trả trạng thái phòng về trống (Available) để người khác đặt
                if (existing.Room != null)
                {
                    existing.Room.Status = "Available";
                }

                return context.SaveChanges() > 0;
            }
        }
        public static decimal CalculateDepositAmount(Booking booking)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var room = context.Rooms.Include(r => r.RoomType)
                    .FirstOrDefault(r => r.RoomId == booking.RoomId
                    );
                if (room == null) return 0;

                int NumberOfDay = (booking.ExpectedCheckOut - booking.ExpectedCheckIn).Days;

                return NumberOfDay * room.RoomType.BasePrice * 0.3m;
            }
        }

        public static bool ExtendCheckout(int bookingId, DateTime newCheckout)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var booking = context.Bookings.Find(bookingId);

                if (booking == null) return false;

                // chỉ cho extend khi đang ở
                if (booking.BookingStatus != "Confirmed") return false;

                // ngày mới phải lớn hơn ngày cũ
                if (newCheckout <= booking.ExpectedCheckOut) return false;

                // kiểm tra trùng lịch
                bool overlap = context.Bookings.Any(b =>
                    b.BookingId != booking.BookingId &&
                    b.RoomId == booking.RoomId &&
                    b.BookingStatus != "Cancelled" &&
                    b.BookingStatus != "CheckedOut" &&
                    booking.ExpectedCheckIn < b.ExpectedCheckOut &&
                    newCheckout > b.ExpectedCheckIn);

                if (overlap) return false;

                booking.ExpectedCheckOut = newCheckout;
                return context.SaveChanges() > 0;
            }
        }

        public static bool ConfirmBooking(int bookingId, out string message)
        {
            message = "";

            using (var context = new ManagementHotelNewContext())
            {

                var booking = context.Bookings
                                     .Include(b => b.Room)
                                     .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    message = "Booking not found.";
                    return false;
                }

                // 1. Kiểm tra trạng thái đơn đặt phòng
                if (booking.BookingStatus != "Booked")
                {
                    message = $"This booking cannot be checked in. Current status is '{booking.BookingStatus}' (Expected: 'Booked').";
                    return false;
                }

                // 2. Kiểm tra xem phòng có tồn tại trong đơn này không (Tránh lỗi NullReferenceException)
                if (booking.Room == null)
                {
                    message = "This booking does not have a valid room assigned.";
                    return false;
                }

                // 3. Kiểm tra trạng thái phòng đi kèm
                if (booking.Room.Status != "Reserved" && booking.Room.Status != "Available")
                {
                    message = $"The assigned room {booking.Room.RoomNumber} is currently '{booking.Room.Status}' (Expected: 'Reserved').";
                    return false;
                }


                DateTime now = DateTime.Now;

                // Chưa đến ngày check-in
                if (now.Date < booking.ExpectedCheckIn.Date)
                {
                    message = $"Guest can only check in on or after {booking.ExpectedCheckIn:dd/MM/yyyy} at 12:00.";
                    return false;
                }

                // Đúng ngày nhưng chưa đến 12:00
                if (now.Date == booking.ExpectedCheckIn.Date && now.TimeOfDay < new TimeSpan(12, 0, 0))
                {
                    message = "Check-in is only allowed after 12:00 PM.";
                    return false;
                }


                booking.BookingStatus = "Confirmed";
                booking.CheckInDate = DateTime.Now;

                booking.Room.Status = "Occupied";

                context.SaveChanges();

                message = "Guest checked in successfully.";

                return true;
            }
        }

        public static void GuestCheckin(int booking)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var checkin = context.Bookings
                    .Include(c => c.Room)
                    .FirstOrDefault(c => c.BookingId == booking);

                if (checkin == null)
                    throw new Exception("Booking Not Found");

                if (checkin.BookingStatus != "Confirmed")
                    throw new Exception("Only confirmed bookings can be checked in.");

                if (DateTime.Today < checkin.ExpectedCheckIn.Date)
                {
                    throw new Exception("Too early to check in.");
                }

                if (DateTime.Today >= checkin.ExpectedCheckOut.Date)
                {
                    throw new Exception("Your booking expired already");
                }
                if (checkin.Room.Status != "Available")
                {
                    throw new Exception("Room is not available.");
                }

                checkin.BookingStatus = "CheckedIn";
                checkin.CheckInDate = DateTime.Now;
                checkin.Room.Status = "Occupied";
                context.SaveChanges();
            }
        }
        public static void RoomAssignment(int bookingid, int roomid)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var booking = context.Bookings.FirstOrDefault(b => b.BookingId == bookingid);

                if (booking == null) throw new Exception("Not found Booking");

                var room = context.Rooms.FirstOrDefault(
                    r => r.RoomId == roomid
                    );
                if (room == null) throw new Exception("Room not found");

                if (room.Status != "Available")
                    throw new Exception("Room is not available.");
                booking.RoomId = roomid;
                context.SaveChanges();
            }
        }
        public static bool GuestCheckout(int bookingid)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var booking = context.Bookings
                              .Include(b => b.CreatedByNavigation)
                              .Include(b => b.Customer)
                              .Include(b => b.Room)
                                   .ThenInclude(r => r.RoomType)
                              .Include(b => b.Serviceorders)
                              .Include(b => b.Invoice)
                              .FirstOrDefault(b => b.BookingId == bookingid);
                if (booking == null)
                    throw new Exception("Booking Not Found");

                if (booking.BookingStatus != "Confirmed")
                    throw new Exception("Guest has not checked in");

                booking.BookingStatus = "CheckedOut";
                booking.CheckOutDate = DateTime.Now;
                booking.Room.Status = "Available";

                return context.SaveChanges() > 0;
            }
        }
        public static decimal CalculateTotalCharges(int bookingid)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var booking = context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Room)
                        .ThenInclude(r => r.RoomType)
                    .Include(b => b.Payments)
                    .Include(b => b.Serviceorders)
                        .ThenInclude(s => s.Service)
                    .Include(b => b.Invoice)
                    .FirstOrDefault(b => b.BookingId == bookingid);
                if (booking == null)
                    throw new Exception("Booking not found.");


                DateTime checkin = booking.CheckInDate ?? booking.ExpectedCheckIn;
                DateTime checkout = booking.CheckOutDate ?? booking.ExpectedCheckOut;

                int days = (int)Math.Ceiling((checkout.Date - checkin.Date).TotalDays);

                if (days <= 0)
                    days = 1;

                decimal roomCharge = booking.Room.RoomType.BasePrice * days;
                decimal serviceCharge = booking.Serviceorders.Sum(s => (s.Service?.Price ?? 0m) * s.Quantity);

                // Tính tổng tiền cọc 30% đã thanh toán trực tuyến
                decimal deposit = booking.Payments
                    .Where(p => p.PaymentStatus == "Paid" && p.PaymentMethod == "Online")
                    .Sum(p => p.Amount);

                // Số tiền còn lại = (Tiền phòng + Tiền dịch vụ - Tiền cọc)
                decimal remainingBeforeTax = roomCharge + serviceCharge - deposit;
                if (remainingBeforeTax < 0) remainingBeforeTax = 0;

                // Tổng tiền thanh toán cuối cùng bao gồm 10% thuế VAT
                decimal totalCharge = remainingBeforeTax * 1.10m;


                context.SaveChanges();

                return totalCharge;

            }
        }

        //Báo cáo đặt phòng
        public static List<Booking> GetReservationReport()
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Room)
                    .OrderByDescending(b => b.ExpectedCheckIn)
                    .ToList();
            }
        }
        //Xem lich su luu tru

        public static List<Booking> GetGuestHistory(string accountId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Room)
                           .ThenInclude(r => r.RoomType)
                    .Include(b => b.Payments)
                    .Where(b => b.CreatedBy == accountId)
                    .ToList();
            }
        }
        public static List<Booking> GetBookingByStatus(string status)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Bookings
                    .Include(b => b.CreatedByNavigation)
                    .Include(b => b.Customer)
                    .Where(b => b.BookingStatus == status).ToList();
            }
        }
        public static List<Booking> SearchBookingByName(string name)
        {
            using (var context = new ManagementHotelNewContext())
            {
                string searchKeyword = name.Trim().ToLower();

                return context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.CreatedByNavigation)
                    .Include(b => b.Room)
                    .Where(b =>
                        (b.CreatedByNavigation != null && b.CreatedByNavigation.FullName != null && b.CreatedByNavigation.FullName.ToLower().Contains(searchKeyword))
                        ||
                        (b.Customer != null && b.Customer.FullName != null && b.Customer.FullName.ToLower().Contains(searchKeyword))
                    )
                    .ToList();
            }
        }
        public static Invoice CreateInvoice(int bookingId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var booking = context.Bookings
                    .Include(b => b.Room)
                        .ThenInclude(r => r.RoomType)
                    .Include(b => b.Serviceorders)
                        .ThenInclude(s => s.Service)
                    .Include(b => b.Payments)
                    .Include(b => b.Invoice)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                    throw new Exception("Booking not found.");

                // Nếu đã có Invoice thì trả về luôn
                if (booking.Invoice != null)
                    return booking.Invoice;

                DateTime checkIn = booking.CheckInDate ?? booking.ExpectedCheckIn;
                DateTime checkOut = booking.CheckOutDate ?? booking.ExpectedCheckOut;

                int days = (checkOut.Date - checkIn.Date).Days;
                if (days <= 0) days = 1;

                decimal roomCharge = booking.Room.RoomType.BasePrice * days;

                decimal serviceCharge = booking.Serviceorders.Sum(s =>
                    s.Service.Price * s.Quantity);

                // Tính tổng tiền cọc 30% đã thanh toán trực tuyến
                decimal deposit = booking.Payments
                    .Where(p => p.PaymentStatus == "Paid" && p.PaymentMethod == "Online")
                    .Sum(p => p.Amount);

                // Số tiền còn lại cần thu = (Tiền phòng + Tiền dịch vụ - Tiền cọc)
                decimal remainingBeforeTax = roomCharge + serviceCharge - deposit;
                if (remainingBeforeTax < 0) remainingBeforeTax = 0;

                // Tổng tiền thanh toán cuối cùng bao gồm 10% thuế VAT
                decimal totalAmount = remainingBeforeTax * 1.10m;

                var payment = booking.Payments
                    .OrderByDescending(p => p.PaymentDate)
                    .FirstOrDefault();

                Invoice invoice = new Invoice
                {
                    BookingId = booking.BookingId,
                    RoomCharge = roomCharge,
                    ServiceCharge = serviceCharge,
                    Discount = deposit, // Lưu số tiền cọc đã thanh toán vào cột Discount
                    TotalAmount = totalAmount, // Lưu tổng tiền còn lại đã tính 10% VAT
                    PaymentDate = DateTime.Now,
                    PaymentMethod = payment?.PaymentMethod ?? "Cash"
                };
                context.Invoices.Add(invoice);
                context.SaveChanges();

                return invoice;
            }
        }
        public static List<Booking> FilterDateToChoose(DateTime startDate, DateTime endDate)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var allRooms = context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => r.Status != "Maintenance")
                    .ToList();

                var activeBookings = context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.CreatedByNavigation)
                    .Include(b => b.Room)
                            .ThenInclude(r => r.RoomType)
                    .Where(b => b.ExpectedCheckIn < endDate
                             && b.ExpectedCheckOut > startDate
                             && b.BookingStatus != "Cancelled"
                             && b.BookingStatus != "CheckedOut")
                    .ToList();

                List<Booking> result = new List<Booking>();

                foreach (var room in allRooms)
                {
                    var currentBooking = activeBookings.FirstOrDefault(b => b.RoomId == room.RoomId);

                    if (currentBooking != null)
                    {
                        currentBooking.Room = room;
                        result.Add(currentBooking);
                    }
                    else
                    {
                        result.Add(new Booking
                        {
                            RoomId = room.RoomId,
                            Room = room,
                            BookingStatus = null,
                            BookingId = 0
                        });
                    }
                }

                return result;
            }
        }

        public static List<Booking> SearchByRoomNumber(string roomNumber)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Bookings
                    .Include(b => b.CreatedByNavigation)
                    .Include(b => b.Room)
                            .ThenInclude(r => r.RoomType)
                    .Include(b => b.Serviceorders)
                            .ThenInclude(s => s.Service)
                    .Include(b => b.Invoice)
                    .Where(b => b.Room.RoomNumber == roomNumber)
                    .ToList();
            }
        }
        public static List<Booking> FilterRoomsbyFloor(int? floor)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Bookings
                    .Include(b => b.Room)
                          .ThenInclude(b => b.RoomType)
                    .Where(b => (!floor.HasValue || b.Room.Floor == floor))
                    .ToList();
            }
        }

        public static List<Booking> FilterRoomsbyStatus(string status)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Bookings
                    .Include(r => r.Room)
                         .ThenInclude(r => r.RoomType)
                    .Where(r => (string.IsNullOrEmpty(status) || r.Room.Status == status))
                    .ToList();
            }
        }
    }
}

