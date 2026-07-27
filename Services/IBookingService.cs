using BussinessObjects;

namespace Services
{
    public interface IBookingService
    {
        List<Booking> GetBooking();

        Booking CreateReservation(Booking booking);

        Booking UpdateReservation(Booking booking);

        bool CancelBooking(int bookingId);

        decimal CalculateDepositAmount(Booking booking);

        bool ExtendCheckout(int bookingId, DateTime newCheckout);

        bool ConfirmBooking(int bookingId, out string message);

        void GuestCheckin(int bookingId);

        void RoomAssignment(int bookingId, int roomId);

        bool GuestCheckout(int bookingId);

        decimal CalculateTotalCharges(int bookingId);

        List<Booking> GetReservationReport();

        List<Booking> GetGuestHistory(string accountId);
        List<Booking> LoadBookingCardsData();
        List<Booking> GetBookingByStatus(string status);
        Booking GetBookingById(int bookingId);
        Invoice CreateInvoice(int bookingId);
        List<Booking> FilterDateToChoose(DateTime startDate, DateTime endDate);
        List<Booking> SearchByRoomNumber(string roomNumber);
        List<Booking> FilterRoomsbyStatus(string status);
        List<Booking> FilterRoomsbyFloor(int? floor);
        List<Booking> SearchBookingByName(string name);

    }
}
