using BussinessObjects;
using Microsoft.Extensions.Configuration;
using Services;
using System.IO;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string Email = txtEmail.Text;
            string Pass = txtPassword.Password;
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrEmpty(Pass))
            {
                MessageBox.Show("Please fill all empty!!!");
            }
            IAccountService accountService = new AccountService();


            var account = accountService.Login(Email, Pass);

            if (account == null)
            {
                MessageBox.Show("Your email or password invalid!!! , Please check again!!!");
            }
            if (account != null)
            {
                try
                {
                    IConfiguration configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
                    string adminEmail = configuration["AccountAdmin:EmailAdmin"];
                    string adminPassword = configuration["AccountAdmin:PassAdmin"];

                    if (account.Email == adminEmail && account.Password == adminPassword)
                    {
                        AdminWindow adminWindow = new AdminWindow(account);
                        adminWindow.Show();
                        Close();
                        return;
                    }

                    else if (account.Roles.Any(r => r.RoleId == "MANAGER"))
                    {
                        ManagerWindow managerWindow = new ManagerWindow(account);
                        managerWindow.Show();
                        Close();
                        return;
                    }
                    else if (account.Roles.Any(r => r.RoleId == "RECEPTIONIST"))
                    {
                        Booking booking = new Booking();
                        ReceptionistWindow receptionistWindow = new ReceptionistWindow(account, booking);
                        receptionistWindow.Show();
                        Close();
                        return;
                    }
                    Booking book = new Booking();
                    MainWindow mainWindow = new MainWindow(account, book);
                    mainWindow.Show();
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while logging in: " + ex.Message);
                }
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUpDialog signUpDialog = new SignUpDialog();
            signUpDialog.ShowDialog();
        }
    }
}
