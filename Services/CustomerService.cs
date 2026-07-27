using BussinessObjects;
using Repositories;

namespace Services
{
    public class CustomerService : ICustomerService
    {
        public readonly ICustomerRepository repository;
        public CustomerService()
        {
            this.repository = new CustomerRepository();
        }

        public Customer CreateCustomer(Customer addCust)
        {
            return this.repository.CreateCustomer(addCust);
        }

        public bool DeleteCustomer(int custId)
        {
            return this.repository.DeleteCustomer(custId);
        }

        public Customer GetCustomerByCitizenId(string indentityCard)
        {
            return this.repository.GetCustomerByCitizenId(indentityCard);
        }

        public Customer GetCustomerByEmail(string email)
        {
            return this.repository.GetCustomerByEmail(email);
        }

        public List<Customer> GetCustomers()
        {
            return this.repository.GetCustomers();
        }

        public List<Customer> SearchByName(string name)
        {
            return this.repository.SearchByName(name);
        }

        public Customer UpdateCustomer(Customer customer)
        {
            return this.repository.UpdateCustomer(customer);
        }
    }
}
