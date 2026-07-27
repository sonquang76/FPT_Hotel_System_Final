using BussinessObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public interface IServiceRepository
    {
        List<Service> GetServices();
    }
}
