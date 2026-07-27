namespace Repositories
{
    public interface IPaymentRepository
    {
        bool ProcessOnlineDeposit(int bookingId);

        bool Refund(int bookingId);
    }
}
