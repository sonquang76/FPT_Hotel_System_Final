using BussinessObjects;

namespace Services
{
    public interface IServiceOrderService
    {
        List<Serviceorder> GetServiceorders();

        Serviceorder CreateRestaurantOrders(Serviceorder serviceorder);

        void ConfirmServiceOrder(int serviceorderId);

        void CompleteServiceOrder(int serviceorderId);

        void CancelServiceOrder(int serviceorderId);

        List<Serviceorder> GetServiceHistory(int bookingId);

        List<Serviceorder> GetServiceUsageReport();

        decimal AssignServiceCharges(int roomId);
    }
}
