namespace Services
{
    public interface IPaymentService
    {
        bool ProcessOnlineDeposit(int bookingId);

        bool Refund(int bookingId);
    }
}
