using BussinessObjects;
using Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for AccountManagementWindow.xaml
    /// </summary>
    public partial class AccountManagementWindow : Window
    {
        private readonly Account account;

        // Biến lưu trữ ID của tài khoản đang được chọn để Update
        private string _selectedAccountId = null;

        public AccountManagementWindow(Account acc)
        {
            InitializeComponent();
            this.account = acc;
            this.DataContext = account;
            LoadReceptionist();
        }

        private void ClearForm()
        {
            // Reset trạng thái về Thêm Mới
            _selectedAccountId = null;
            dgReceptionists.SelectedItem = null;

            txtPassword.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtIdentityCard.Clear();

            cboGender.SelectedIndex = -1;
            cboAccountStatus.SelectedIndex = 0;
            dpDob.SelectedDate = null;

            // Đổi lại giao diện nút bấm thành ADD
            btnSaveAccount.Content = "ADD ACCOUNT";
            btnSaveAccount.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"));
        }

        private void LoadReceptionist()
        {
            IAccountService accountService = new AccountService();
            dgReceptionists.ItemsSource = accountService.GetAccount("RECEPTIONIST");
        }

        // Sự kiện tự động fill dữ liệu lên form khi Click vào một dòng trong bảng
        private void dgReceptionists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgReceptionists.SelectedItem is Account selectedAcc)
            {
                // Lưu lại AccountId để dùng cho Update
                _selectedAccountId = selectedAcc.AccountId;

                txtFullName.Text = selectedAcc.FullName;
                txtPassword.Password = selectedAcc.Password;
                txtEmail.Text = selectedAcc.Email;
                txtPhone.Text = selectedAcc.Phone;
                txtIdentityCard.Text = selectedAcc.IdentityCard;
                dpDob.SelectedDate = selectedAcc.Dob;

                // Bind lại ComboBox Gender
                foreach (ComboBoxItem item in cboGender.Items)
                {
                    if (item.Content.ToString() == selectedAcc.Gender)
                    {
                        cboGender.SelectedItem = item;
                        break;
                    }
                }

                // Bind lại ComboBox Status
                foreach (ComboBoxItem item in cboAccountStatus.Items)
                {
                    if (item.Content.ToString() == selectedAcc.AccountStatus)
                    {
                        cboAccountStatus.SelectedItem = item;
                        break;
                    }
                }

                // Chuyển giao diện nút bấm thành màu Cam (Báo hiệu đang ở chế độ UPDATE)
                btnSaveAccount.Content = "UPDATE ACCOUNT";
                btnSaveAccount.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F39C12"));
            }
        }

        // Nút Clear Form
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BtnSaveAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate trống
                if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Password) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPhone.Text) ||
                    string.IsNullOrWhiteSpace(txtIdentityCard.Text) ||
                    dpDob.SelectedDate == null)
                {
                    MessageBox.Show("Please fill in all required fields!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string gender = ((ComboBoxItem)cboGender.SelectedItem).Content.ToString();
                string status = ((ComboBoxItem)cboAccountStatus.SelectedItem).Content.ToString();

                IAccountService accountService = new AccountService();

                // NẾU _selectedAccountId LÀ NULL => CHẾ ĐỘ THÊM MỚI (ADD)
                if (string.IsNullOrEmpty(_selectedAccountId))
                {
                    Account newAccount = new Account()
                    {
                        AccountId = Guid.NewGuid().ToString(),
                        Password = txtPassword.Password,
                        FullName = txtFullName.Text.Trim(),
                        Gender = gender,
                        Dob = dpDob.SelectedDate.Value,
                        Email = txtEmail.Text.Trim(),
                        Phone = txtPhone.Text.Trim(),
                        IdentityCard = txtIdentityCard.Text.Trim(),
                        AccountStatus = status
                    };

                    newAccount.Roles.Add(new Role { RoleId = "RECEPTIONIST" });

                    var success = accountService.SignUpAccount(newAccount);

                    if (success != null)
                    {
                        MessageBox.Show("Create account successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadReceptionist();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Create account failed! Identity or Email might exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                // NẾU CÓ _selectedAccountId => CHẾ ĐỘ CẬP NHẬT (UPDATE)
                else
                {
                    Account updateAccount = new Account()
                    {
                        AccountId = _selectedAccountId,
                        Password = txtPassword.Password,
                        FullName = txtFullName.Text.Trim(),
                        Gender = gender,
                        Dob = dpDob.SelectedDate.Value,
                        Email = txtEmail.Text.Trim(),
                        Phone = txtPhone.Text.Trim(),
                        IdentityCard = txtIdentityCard.Text.Trim(),
                        AccountStatus = status
                    };

                    var success = accountService.UpdateAccount(updateAccount);

                    if (success != null)
                    {
                        MessageBox.Show("Update account successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadReceptionist();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Update account failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnDeleteStatus_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            Account selected = btn.DataContext as Account;
            if (selected == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to deactivate account '{selected.FullName}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            IAccountService accountService = new AccountService();
            bool confirmed = accountService.DeleteAccount(selected.AccountId);

            if (confirmed)
            {
                MessageBox.Show($"Account '{selected.FullName}' has been deactivated successfully!");
                LoadReceptionist();
                ClearForm(); // Chống lỗi bấm chọn rồi xóa luôn nhưng chữ trên form vẫn còn
            }
            else
            {
                MessageBox.Show($"Failed to deactivate account '{selected.FullName}'.");
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                LoadReceptionist();
                return;
            }

            IAccountService accountService = new AccountService();
            var allReceptionists = accountService.GetAccount("RECEPTIONIST");

            var filteredResult = allReceptionists
                .Where(a => a.FullName != null && a.FullName.ToLower().Contains(search))
                .ToList();

            if (filteredResult.Count > 0)
            {
                dgReceptionists.ItemsSource = filteredResult;
            }
            else
            {
                dgReceptionists.ItemsSource = null;
                MessageBox.Show("Not found any receptionist with that name!!!");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow mw = new ManagerWindow(account);
            mw.Show();
            this.Close();
        }
    }
}
