using BussinessObjects;
using Services;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for AccountInformationDialog.xaml
    /// </summary>
    public partial class AccountInformationDialog : Window
    {
        private readonly Account account;
        public AccountInformationDialog(Account acc)
        {
            InitializeComponent();
            this.account = acc;
            this.DataContext = account;
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string gender = txtGender.Text;
            string email = txtEmail.Text;
            string phone = txtPhone.Text;
            string Card = txtCard.Text;

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(gender) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(Card))
            {
                MessageBox.Show("Not allow any infor empty!!!");
                return;
            }

            if (!dpDate.SelectedDate.HasValue || dpDate.SelectedDate.Value >= DateTime.Now)
            {
                MessageBox.Show("Your birthday invalid!!!");
                return;
            }
            DateTime date = dpDate.SelectedDate.Value;

            account.FullName = fullName;
            account.Gender = gender;
            account.Email = email;
            account.Dob = date;
            account.Phone = phone;
            account.IdentityCard = Card;

            IAccountService accountService = new AccountService();
            var acc = accountService.UpdateAccount(account);

            if (acc != null)
            {
                MessageBox.Show("Update successfully!!!", "Notificaiton", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Update fail, please check your input which you just type it");
            }
        }
    }
}
