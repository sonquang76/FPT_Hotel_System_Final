using BussinessObjects;
using Repositories;

namespace Services
{
    public class ServiceOrderService : IServiceOrderService
    {
        private readonly IServiceOrderRepository repository;
        public ServiceOrderService() { this.repository = new ServiceOrderRepository(); }
        public decimal AssignServiceCharges(int roomId)
        {
            return this.repository.AssignServiceCharges(roomId);
        }

        public void CancelServiceOrder(int serviceorderId)
        {
            this.repository.CancelServiceOrder(serviceorderId);
        }

        public void CompleteServiceOrder(int serviceorderId)
        {
            this.repository.CompleteServiceOrder(serviceorderId);
        }

        public void ConfirmServiceOrder(int serviceorderId)
        {
            this.repository.ConfirmServiceOrder(serviceorderId);
        }

        public Serviceorder CreateRestaurantOrders(Serviceorder serviceorder)
        {
            return this.repository.CreateRestaurantOrders(serviceorder);
        }

        public List<Serviceorder> GetServiceHistory(int bookingId)
        {
            return this.repository.GetServiceHistory(bookingId);
        }

        public List<Serviceorder> GetServiceorders()
        {
            return this.repository.GetServiceorders();
        }

        public List<Serviceorder> GetServiceUsageReport()
        {
            return this.repository.GetServiceUsageReport();
        }
    }
}
