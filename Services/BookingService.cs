using BussinessObjects;
using Repositories;

namespace Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository repository;
        public BookingService()
        {
            this.repository = new BookingRepository();
        }
        public decimal CalculateDepositAmount(Booking booking)
        {
            return this.repository.CalculateDepositAmount(booking);
        }

        public decimal CalculateTotalCharges(int bookingId)
        {
            return this.repository.CalculateTotalCharges(bookingId);
        }

        public bool CancelBooking(int bookingId)
        {
            return this.repository.CancelBooking(bookingId);
        }

        public bool ConfirmBooking(int bookingId, out string message)
        {
            return this.repository.ConfirmBooking(bookingId, out message);
        }

        public Invoice CreateInvoice(int bookingId)
        {
            return this.repository.CreateInvoice(bookingId);
        }

        public Booking CreateReservation(Booking booking)
        {
            return this.repository.CreateReservation(booking);
        }

        public bool ExtendCheckout(int bookingId, DateTime newCheckout)
        {
            return this.repository.ExtendCheckout(bookingId, newCheckout);
        }

        public List<Booking> FilterDateToChoose(DateTime startDate, DateTime endDate)
        {
            return this.repository.FilterDateToChoose(startDate, endDate);
        }

        public List<Booking> FilterRoomsbyFloor(int? floor)
        {
            return this.repository.FilterRoomsbyFloor((int)floor);
        }

        public List<Booking> FilterRoomsbyStatus(string status)
        {
            return this.repository.GetBookingByStatus(status);
        }

        public List<Booking> GetBooking()
        {
            return this.repository.GetBooking();
        }

        public Booking GetBookingById(int bookingId)
        {
            return this.repository.GetBookingById(bookingId);
        }

        public List<Booking> GetBookingByStatus(string status)
        {
            return this.repository.GetBookingByStatus(status);
        }

        public List<Booking> GetGuestHistory(string accountId)
        {
            return this.repository.GetGuestHistory(accountId);
        }

        public List<Booking> GetReservationReport()
        {
            return this.repository.GetReservationReport();
        }


        public void GuestCheckin(int bookingId)
        {
            this.repository.GuestCheckin(bookingId);
        }

        public bool GuestCheckout(int bookingId)
        {
            return this.repository.GuestCheckout(bookingId);
        }

        public List<Booking> LoadBookingCardsData()
        {
            return this.repository.LoadBookingCardsData();
        }

        public void RoomAssignment(int bookingId, int roomId)
        {
            this.repository.RoomAssignment(bookingId, roomId);
        }

        public List<Booking> SearchBookingByName(string name)
        {
            return this.repository.SearchBookingByName(name);
        }

        public List<Booking> SearchByRoomNumber(string roomNumber)
        {
            return this.repository.SearchByRoomNumber(roomNumber);
        }

        public Booking UpdateReservation(Booking booking)
        {
            return this.repository.UpdateReservation(booking);
        }
    }
}
