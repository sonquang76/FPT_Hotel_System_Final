using BussinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class BookingRepository : IBookingRepository
    {
        public decimal CalculateDepositAmount(Booking booking)
        {
            return BookingDao.CalculateDepositAmount(booking);
        }

        public decimal CalculateTotalCharges(int bookingId)
        {
            return BookingDao.CalculateTotalCharges(bookingId);
        }

        public bool CancelBooking(int bookingId)
        {
            return BookingDao.CancelBooking(bookingId);
        }

        public bool ConfirmBooking(int bookingId, out string message)
        {
            return BookingDao.ConfirmBooking(bookingId, out message);
        }

        public Invoice CreateInvoice(int bookingId)
        {
            return BookingDao.CreateInvoice(bookingId);
        }

        public Booking CreateReservation(Booking booking)
        {
            return BookingDao.CreateReservation(booking);
        }

        public bool ExtendCheckout(int bookingId, DateTime newCheckout)
        {
            return BookingDao.ExtendCheckout(bookingId, newCheckout);
        }

        public List<Booking> FilterDateToChoose(DateTime startDate, DateTime endDate)
        {
            return BookingDao.FilterDateToChoose(startDate, endDate);
        }

        public List<Booking> FilterRoomsbyFloor(int? floor)
        {
            return BookingDao.FilterRoomsbyFloor((int?) floor);
        }

        public List<Booking> FilterRoomsbyStatus(string status)
        {
            return BookingDao.FilterRoomsbyStatus(status);
        }

        public List<Booking> GetBooking()
        {
            return BookingDao.GetBooking();
        }

        public Booking GetBookingById(int bookingId)
        {
            return BookingDao.GetBookingById(bookingId);
        }

        public List<Booking> GetBookingByStatus(string status)
        {
            return BookingDao.GetBookingByStatus(status);
        }

        public List<Booking> GetGuestHistory(string accountId)
        {
            return BookingDao.GetGuestHistory(accountId);
        }

        public List<Booking> GetReservationReport()
        {
            return BookingDao.GetReservationReport();
        }


        public void GuestCheckin(int bookingId)
        {
            BookingDao.GuestCheckin(bookingId);
        }

        public bool GuestCheckout(int bookingId)
        {
           return BookingDao.GuestCheckout(bookingId);
        }

        public List<Booking> LoadBookingCardsData()
        {
            return BookingDao.LoadBookingCardsData();
        }

        public void RoomAssignment(int bookingId, int roomId)
        {
            BookingDao.RoomAssignment(bookingId, roomId);
        }

        public List<Booking> SearchBookingByName(string name)
        {
            return BookingDao.SearchBookingByName(name);
        }

        public List<Booking> SearchByRoomNumber(string roomNumber)
        {
            return BookingDao.SearchByRoomNumber(roomNumber);
        }

        public Booking UpdateReservation(Booking booking)
        {
            return BookingDao.UpdateReservation(booking);
        }
    }
}
