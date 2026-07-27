using Repositories;
using BussinessObjects;
namespace Services
{
    public class ServiceToService : IServiceToService
    {
        private readonly ServiceRepository repository;
        public ServiceToService()
        {
            this.repository = new ServiceRepository();
        }
        public List<Service> GetServices()
        {
            return this.repository.GetServices();
        }
    }
}
