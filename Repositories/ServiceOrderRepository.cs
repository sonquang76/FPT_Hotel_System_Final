using BussinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class ServiceOrderRepository : IServiceOrderRepository
    {
        public ServiceOrderRepository() { }
        public decimal AssignServiceCharges(int roomId)
        {
            return ServiceOrderDao.AssignServiceCharges(roomId);
        }

        public void CancelServiceOrder(int serviceorderId)
        {
            ServiceOrderDao.CancelServiceOrder(serviceorderId);
        }

        public void CompleteServiceOrder(int serviceorderId)
        {
            ServiceOrderDao.CompleteServiceOrder(serviceorderId);
        }

        public void ConfirmServiceOrder(int serviceorderId)
        {
            ServiceOrderDao.ConfirmServiceOrder(serviceorderId);
        }

        public Serviceorder CreateRestaurantOrders(Serviceorder serviceorder)
        {
            return ServiceOrderDao.CreateRestaurantOrders(serviceorder);
        }

        public List<Serviceorder> GetServiceHistory(int bookingId)
        {
            return ServiceOrderDao.GetServiceHistory(bookingId);
        }

        public List<Serviceorder> GetServiceorders()
        {
            return ServiceOrderDao.GetServiceorders();
        }

        public List<Serviceorder> GetServiceUsageReport()
        {
            return ServiceOrderDao.GetServiceUsageReport();
        }
    }
}
