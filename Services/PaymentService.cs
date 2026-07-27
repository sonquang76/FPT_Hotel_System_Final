using Repositories;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository repository;
        public PaymentService() { this.repository = new PaymentRepository(); }
        public bool ProcessOnlineDeposit(int bookingId)
        {
            return this.repository.ProcessOnlineDeposit(bookingId);
        }

        public bool Refund(int bookingId)
        {
            return this.repository.Refund(bookingId);
        }
    }
}
