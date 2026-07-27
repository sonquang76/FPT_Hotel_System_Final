using BussinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        public List<Service> GetServices()
        {
            return ServiceDao.GetServices();
        }
    }
}
