using BussinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public AccountRepository() { }
        public bool ChangePassword(ChangePasswordModel model)
        {
            return AccountDao.ChangePassword(model);
        }

        public bool DeleteAccount(string accountId)
        {
            return AccountDao.DeleteAccount(accountId);
        }

        public List<Account> GetAccount(string role)
        {
            return AccountDao.GetAccount(role);
        }

        public Account GetAccountByCitizenId(string CitizenId)
        {
            return AccountDao.GetAccountByCitizenId(CitizenId);
        }

        public Account GetAccountById(string id)
        {
            return AccountDao.GetAccountById(id);
        }

        public List<Account> GetAccounts()
        {
            return AccountDao.GetAccounts();
        }

        public Account Login(string email, string password)
        {
            return AccountDao.Login(email, password);
        }

        public List<Account> SearchByName(string name)
        {
            return AccountDao.SearchByName(name);
        }

        public Account SignUpAccount(Account addAcc)
        {
            return AccountDao.SignUpAccount(addAcc);
        }

        public Account UpdateAccount(Account acc)
        {
            return AccountDao.UpdateAccount(acc);
        }
    }
}
