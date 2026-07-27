using BussinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class AccountDao
    {
        public AccountDao() { }
        public static List<Account> GetAccounts()
        {
            using (ManagementHotelNewContext context = new ManagementHotelNewContext())
            {
                return context.Accounts.ToList();
            }
        }
        public static List<Account> GetAccount(string role)
        {
            using (ManagementHotelNewContext context = new ManagementHotelNewContext())
            {
                return context.Accounts
                    .Include(r => r.Roles)
                    .Where(a => a.Roles.Any(r => r.RoleId == role))
                    .ToList();
            }
        }
        // CRUD
        //C-
        public static Account SignUpAccount(Account addAcc)
        {
            using (var context = new ManagementHotelNewContext())
            {
                // Kiểm tra email đã tồn tại
                bool existsEmail = context.Accounts.Any(a => a.Email == addAcc.Email);

                if (existsEmail)
                    return null;

                // Tạo Account
                Account account = new Account
                {
                    AccountId = Guid.NewGuid().ToString(),
                    Password = addAcc.Password,
                    Gender = addAcc.Gender,
                    FullName = addAcc.FullName,
                    Dob = addAcc.Dob,
                    Email = addAcc.Email,
                    Phone = addAcc.Phone,
                    AccountStatus = addAcc.AccountStatus,
                    IdentityCard = addAcc.IdentityCard
                };

                // Gán Role
                foreach (var role in addAcc.Roles)
                {
                    var dbRole = context.Roles.Find(role.RoleId);
                    if (dbRole != null)
                    {
                        account.Roles.Add(dbRole);
                    }
                }

                // Thêm Account trước
                context.Accounts.Add(account);

                // Tạo Customer tương ứng
                Customer customer = new Customer
                {
                    FullName = account.FullName,
                    Email = account.Email,
                    PhoneNumber = account.Phone,
                    IdentityCard = account.IdentityCard
                };

                context.Customers.Add(customer);

                context.SaveChanges();

                return account;
            }
        }
        //R-

        public static List<Account> SearchByName(string Name)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Accounts.Where(
                    a => a.FullName != null && a.FullName.Contains(Name)
                    ).ToList();
            }
        }

        //U-

        public static Account UpdateAccount(Account acc)
        {
            using (var context = new ManagementHotelNewContext())
            {
                Account exitsAcc = context.Accounts.Find(acc.AccountId);

                if (exitsAcc == null) return null;

                exitsAcc.Gender = acc.Gender;
                exitsAcc.FullName = acc.FullName;
                exitsAcc.Dob = acc.Dob;
                exitsAcc.Email = acc.Email;
                exitsAcc.Phone = acc.Phone;
                exitsAcc.IdentityCard = acc.IdentityCard;
                exitsAcc.AccountStatus = acc.AccountStatus;

                context.SaveChanges();
                return exitsAcc;

            }
        }

        public static bool DeleteAccount(string accountId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var exit = context.Accounts.Find(accountId);
                if (exit == null) return false;

                exit.AccountStatus = "UNDEACTIVE";
                return context.SaveChanges() > 0;
            }
        }

        public static Account GetAccountById(string id)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var account = context.Accounts.Find(id);
                return account;
            }
        }

        public static Account GetAccountByCitizenId(string CitizenId)
        {
            using (var context = new ManagementHotelNewContext())
            {
                return context.Accounts
                    .FirstOrDefault(a => a.IdentityCard == CitizenId);
            }
        }

        public static Account Login(string email, string password)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var account = context.Accounts
                    .Include(a => a.Roles)
                    .FirstOrDefault(
                    a => a.Email == email && a.Password == password
                    );
                return account;
            }
        }
        public static bool ChangePassword(ChangePasswordModel model)
        {
            using (var context = new ManagementHotelNewContext())
            {
                var account = context.Accounts.Find(model.AccountId);

                if (account == null) return false;

                if (account.Password != model.OldPassword)
                    throw new Exception("Old password is incorrect.");

                if (model.NewPassword != model.ConfirmPassword)
                    throw new Exception("Confirm password does not match.");

                account.Password = model.NewPassword;

                return context.SaveChanges() > 0;
            }
        }

    }
}

