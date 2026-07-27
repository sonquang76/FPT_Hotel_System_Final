using BussinessObjects;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace WPF_FPT_ManagementHotel
{
    public partial class AdminWindow : Window
    {
        private readonly Account account;
        private readonly IAccountService service;

        public AdminWindow(Account acc)
        {
            InitializeComponent();
            account = acc;
            service = new AccountService();

            txtWelcome.Text = $"Welcome, {account.FullName}";

            // Tải toàn bộ dữ liệu lên Dashboard và các DataGrid
            RefreshData();
        }

        private void RefreshData()
        {
            // 1. Cập nhật số lượng đếm trên Dashboard
            var managersList = service.GetAccount("MANAGER");
            var usersList = service.GetAccount("USER");

            txtManagerCount.Text = managersList.Count.ToString();
            txtUserCount.Text = usersList.Count.ToString();

            // 2. Ép DataGrid hủy nhận nguồn cũ rồi gán lại để giao diện bắt buộc phải vẽ lại trạng thái mới
            dgManagers.ItemsSource = null;
            dgManagers.ItemsSource = managersList;

            dgUsers.ItemsSource = null;
            dgUsers.ItemsSource = usersList;
        }

        // Click nút "Manage Managers" trên Sidebar -> Chuyển sang Tab Managers (Index 0)
        private void BtnManagerTab_Click(object sender, RoutedEventArgs e)
        {
            tcAdminManagement.SelectedIndex = 0;
        }

        // Click nút "Manage Users" trên Sidebar -> Chuyển sang Tab Users (Index 1)
        private void BtnUserTab_Click(object sender, RoutedEventArgs e)
        {
            tcAdminManagement.SelectedIndex = 1;
        }

        // Hành động thay đổi trạng thái Active / Inactive của tài khoản
        private void BtnToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Account selectedAccount)
            {
                if (selectedAccount.AccountStatus == "ACTIVE")
                {
                    selectedAccount.AccountStatus = "UNDEACTIVE";
                }
                else
                {
                    selectedAccount.AccountStatus = "ACTIVE";
                }

                service.UpdateAccount(selectedAccount);

                MessageBox.Show($"Updated status for {selectedAccount.FullName} successfully!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                RefreshData();
            }
        }

        // Hành động xóa Manager
        private void BtnDeleteManager_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Account selectedAccount)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete Manager: {selectedAccount.FullName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    service.DeleteAccount(selectedAccount.AccountId);
                    RefreshData();
                }
            }
        }

        // Hành động xóa User
        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Account selectedAccount)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete User: {selectedAccount.FullName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    service.DeleteAccount(selectedAccount.AccountId);
                    RefreshData();
                }
            }
        }

        // Đăng xuất hệ thống
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}