using BussinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public Customer CreateCustomer(Customer addCust)
        {
            return CustomerDao.CreateCustomer(addCust);
        }

        public bool DeleteCustomer(int custId)
        {
            return CustomerDao.DeleteCustomer(custId);
        }

        public Customer GetCustomerByCitizenId(string indentityCard)
        {
            return CustomerDao.GetCustomerByCitizenId(indentityCard);
        }

        public Customer GetCustomerByEmail(string email)
        {
            return CustomerDao.GetCustomerByEmail(email);
        }

        public List<Customer> GetCustomers()
        {
            return CustomerDao.GetCustomers();
        }

        public List<Customer> SearchByName(string name)
        {
            return CustomerDao.SearchByName(name);
        }

        public Customer UpdateCustomer(Customer customer)
        {
            return CustomerDao.UpdateCustomer(customer);
        }
    }
}
