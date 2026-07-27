using BussinessObjects;
using Services;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for ChangePasswordDialog.xaml
    /// </summary>
    public partial class ChangePasswordDialog : Window
    {
        private readonly Account account;
        public ChangePasswordDialog(Account acc)
        {
            InitializeComponent();
            this.account = acc;
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            string OldPass = txtOldPassword.Password;
            string NewPass = txtNewPassword.Password;
            string Confirm = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(OldPass) || string.IsNullOrWhiteSpace(NewPass) || string.IsNullOrWhiteSpace(Confirm))
            {
                MessageBox.Show("Please fill all field");
                return;
            }
            ChangePasswordModel model = new ChangePasswordModel()
            {
                AccountId = account.AccountId,
                OldPassword = OldPass,
                NewPassword = NewPass,
                ConfirmPassword = Confirm
            };

            IAccountService accountService = new AccountService();
            try
            {
                bool result = accountService.ChangePassword(model);

                if (result)
                {
                    MessageBox.Show("Change password success!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Change password failed!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
