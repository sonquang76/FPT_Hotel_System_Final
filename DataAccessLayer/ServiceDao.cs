using BussinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer
{
    public class ServiceDao
    {
        public ServiceDao() { }
        public static List<Service> GetServices()
        {
            using(var context = new ManagementHotelNewContext())
            {
                var services = context.Services.Include(s => s.Serviceorders).ToList();
                return services;
            }
        }
    }
}
