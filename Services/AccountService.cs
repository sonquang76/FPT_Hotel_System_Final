using BussinessObjects;
using Repositories;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        public AccountService()
        {
            this._repository = new AccountRepository();
        }
        public bool ChangePassword(ChangePasswordModel model)
        {
            return this._repository.ChangePassword(model);
        }

        public bool DeleteAccount(string accountId)
        {
            return this._repository.DeleteAccount(accountId);
        }

        public List<Account> GetAccount(string role)
        {
            return this._repository.GetAccount(role);
        }

        public Account GetAccountByCitizenId(string CitizenId)
        {
            return this._repository.GetAccountByCitizenId(CitizenId);
        }

        public Account GetAccountById(string id)
        {
            return this._repository.GetAccountById(id);
        }

        public List<Account> GetAccounts()
        {
            return this._repository.GetAccounts();
        }

        public Account Login(string email, string password)
        {
            return this._repository.Login(email, password);
        }

        public List<Account> SearchByName(string name)
        {
            return this._repository.SearchByName(name);
        }

        public Account SignUpAccount(Account addAcc)
        {
            return this._repository.SignUpAccount(addAcc);
        }

        public Account UpdateAccount(Account acc)
        {
            return this._repository.UpdateAccount(acc);
        }
    }
}
