using BussinessObjects;

namespace DataAccessLayer
{
    public class CustomerDao
    {
        public CustomerDao() { }
        public static List<Customer> GetCustomers()
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Customers.ToList();
            }
        }
        public static Customer CreateCustomer(Customer AddCust)
        {
            using (var context = new ManagementHotelNewContext())
            {
                Customer customer = new Customer()
                {
                    FullName = AddCust.FullName,
                    IdentityCard = AddCust.IdentityCard,
                    PhoneNumber = AddCust.PhoneNumber,
                    Email = AddCust.Email,
                };

                context.Customers.Add(customer);
                context.SaveChanges();

                return customer;
            }
        }

        public static List<Customer> SearchByName(string Name)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Customers.Where(
                    c => c.FullName != null && c.FullName.Contains(Name)
                    ).ToList();
            }
        }

        public static Customer UpdateCustomer(Customer UdCust)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var customer = context.Customers.Find(UdCust.CustomerId);

                if (customer == null) return null;

                customer.FullName = UdCust.FullName;
                customer.IdentityCard = UdCust.IdentityCard;
                customer.PhoneNumber = UdCust.PhoneNumber;
                customer.Email = UdCust.Email;

                context.SaveChanges();
                return customer;
            }
        }

        public static bool DeleteCustomer(int custId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var customer = context.Customers.Find(custId);

                if (customer == null) return false;

                context.Customers.Remove(customer);

                return context.SaveChanges() > 0;
            }
        }
        public static Customer GetCustomerByEmail(string email)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Customers.FirstOrDefault(r => r.Email == email);
            }
        }
        public static Customer GetCustomerByCitizenId(string indentityCard)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Customers.FirstOrDefault(r => r.IdentityCard == indentityCard);
            }
        }
    }
}
