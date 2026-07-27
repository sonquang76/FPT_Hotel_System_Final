using BussinessObjects;
using Services;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for SignUpDialog.xaml
    /// </summary>
    public partial class SignUpDialog : Window
    {
        public SignUpDialog()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string gender = cbGender.Text;
            string email = txtEmail.Text;
            string pass = txtPassword.Password;
            string phone = txtPhone.Text;
            string Card = txtIdentity.Text;

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(gender) ||
                  string.IsNullOrWhiteSpace(email) ||
                  string.IsNullOrWhiteSpace(phone) ||
                  string.IsNullOrWhiteSpace(Card))
            {
                MessageBox.Show("Please fill all empty!!!");
                return;
            }

            if (!dpDOB.SelectedDate.HasValue || dpDOB.SelectedDate.Value >= DateTime.Now)
            {
                MessageBox.Show("Please check your birthday");
                return;
            }

            var dob = dpDOB.SelectedDate;

            IAccountService accountService = new AccountService();
            IRoleService roleService = new RoleService();
            HashSet<Role> roles = new HashSet<Role>();

            Role UserRole = roleService.FindRoleById("USER");
            if (UserRole == null)
            {
                MessageBox.Show("Role not exits!!!");
                return;
            }
            roles.Add(UserRole);

            var account = new Account()
            {
                FullName = fullName,
                Gender = gender,
                Dob = (DateTime)dob,
                Email = email,
                Password = pass,
                Phone = phone,
                AccountStatus = "ACTIVE",
                IdentityCard = Card,
            };

            account.Roles.Add(UserRole);

            var acc = accountService.SignUpAccount(account);

            if (acc != null)
            {
                MessageBox.Show("Sign up successfully!!!");
                Close();
            }
            else
            {
                MessageBox.Show("Sign up fail , please check your input again!!!");
            }

        }
    }
}
