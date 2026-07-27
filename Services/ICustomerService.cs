using BussinessObjects;

namespace Services
{
    public interface ICustomerService
    {
        List<Customer> GetCustomers();

        Customer CreateCustomer(Customer addCust);

        List<Customer> SearchByName(string name);

        Customer UpdateCustomer(Customer customer);

        bool DeleteCustomer(int custId);
        Customer GetCustomerByEmail(string email);
        Customer GetCustomerByCitizenId(string indentityCard);
    }
}
