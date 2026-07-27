using BussinessObjects;

namespace Services
{
    public interface IAccountService
    {
        List<Account> GetAccounts();

        Account SignUpAccount(Account addAcc);

        List<Account> SearchByName(string name);

        Account UpdateAccount(Account acc);

        bool DeleteAccount(string accountId);

        Account GetAccountById(string id);

        Account Login(string email, string password);

        bool ChangePassword(ChangePasswordModel model);
        Account GetAccountByCitizenId(string CitizenId);
        List<Account> GetAccount(string role);
    }
}
