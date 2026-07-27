using DataAccessLayer;

namespace Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public bool ProcessOnlineDeposit(int bookingId)
        {
            return PaymentDao.ProcessOnlineDeposit(bookingId);
        }

        public bool Refund(int bookingId)
        {
            return PaymentDao.Refund(bookingId);
        }
    }
}
