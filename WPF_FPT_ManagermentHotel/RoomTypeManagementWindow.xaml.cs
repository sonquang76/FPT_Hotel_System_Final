using BussinessObjects;
using Services;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for RoomTypeManagementWindow.xaml
    /// </summary>
    public partial class RoomTypeManagementWindow : Window
    {
        private readonly Roomtype roomtype;
        IRoomTypeService roomTypeService = new RoomTypeService();
        private readonly Account account;
        public RoomTypeManagementWindow(Roomtype type, Account acc)
        {
            InitializeComponent();
            this.roomtype = type;
            this.account = acc;
            this.DataContext = roomtype;
            loadRoomType();
        }

        private void loadRoomType()
        {
           
            dgRoomType.ItemsSource = roomTypeService.GetRoomtypes();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoomType.SelectedItem is Roomtype selectedRoomType)
            {
                RoomTypeDialog updateDialog = new RoomTypeDialog(selectedRoomType, account);
                if (updateDialog.ShowDialog() == true)
                {
                    MessageBox.Show("Room type updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadRoomType(); // Tải lại Grid
                }
            }
            else
            {
                MessageBox.Show("Please select a room type from the list to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Sửa lỗi ở đây: Tạo đối tượng rỗng và truyền kèm account
            Roomtype newRoomType = new Roomtype();
            RoomTypeDialog addDialog = new RoomTypeDialog(newRoomType, this.account);
            addDialog.Show();
            this.Close(); // Đóng form quản lý
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string key = txtType.Text.Trim().ToLower();
            // Lấy từ khóa tìm kiếm từ TextBox (Đang dùng tạm txtFloor theo XAML của bạn)
            string keyword = txtType.Text.Trim().ToLower();

            // Lấy toàn bộ danh sách
            var allRoomTypes = roomTypeService.GetRoomtypes();

            // Lọc dữ liệu bằng LINQ
            if (string.IsNullOrEmpty(keyword))
            {
                dgRoomType.ItemsSource = allRoomTypes;
            }
            else
            {
                // Lọc theo TypeName có chứa từ khóa
                dgRoomType.ItemsSource = allRoomTypes
                    .Where(rt => rt.TypeName.ToLower().Contains(keyword))
                    .ToList();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoomType.SelectedItem is Roomtype selectedRoomType)
            {
                // 2. Hiển thị hộp thoại xác nhận
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete Room Type: {selectedRoomType.TypeName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        roomTypeService.DeleteRoomType(selectedRoomType);

                        MessageBox.Show("Room type deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // 4. Tải lại danh sách
                        loadRoomType();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a room type to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow mw = new ManagerWindow(account);
            mw.Show();
            this.Close();
        }
    }
}
