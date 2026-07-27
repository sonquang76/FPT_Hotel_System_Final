using BussinessObjects;
using Microsoft.EntityFrameworkCore;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Account account;
        private readonly Booking booking;
        private readonly ManagementHotelNewContext context;
        public MainWindow(Account acc, Booking book)
        {
            InitializeComponent();
            this.account = acc;
            this.booking = book;

            dpCheckIn.SelectedDate = DateTime.Today;
            dpCheckOut.SelectedDate = DateTime.Today.AddDays(1);
            context = new ManagementHotelNewContext();
            var rooms = context.Rooms.Include(r => r.RoomType).ToList();
            icRooms.ItemsSource = rooms;
            LoadRoom();

        }


        private void LoadRoom()
        {

            try
            {
                IBookingService bookingService = new BookingService();

                // Lấy ngày hôm nay từ DatePicker đã set ở trên
                DateTime todayStart = dpCheckIn.SelectedDate.Value;
                DateTime todayEnd = dpCheckOut.SelectedDate.Value;

                // Sử dụng hàm filter theo ngày mà bạn đã viết ở dưới nút "Lọc phòng"
                var result = bookingService.FilterDateToChoose(todayStart, todayEnd);

                if (result != null)
                {
                    icRooms.ItemsSource = result;
                    rdAll.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during automatic room loading today {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void btnInforAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountInformationDialog accountInformationDialog = new AccountInformationDialog(account);
            accountInformationDialog.ShowDialog();
        }

        private void btnChagePassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordDialog changePasswordDialog = new ChangePasswordDialog(account);
            changePasswordDialog.ShowDialog();
        }

        private void Room_Click(object sender, RoutedEventArgs e)
        {

            Button btn = sender as Button;
            if (btn == null) return;

            var selected = btn.DataContext as Booking;
            if (selected != null)
            {
                RoomDetailDialog roomDetailDialog = new RoomDetailDialog(account, selected);
                roomDetailDialog.Show();
                Close();
            }

        }


        private void btnHistoryBooking_Click(object sender, RoutedEventArgs e)
        {
            BookingHistoryWindow booking = new BookingHistoryWindow(account);
            booking.Show();

        }
        private void btnFilterDate_Click(object sender, RoutedEventArgs e)
        {
            if (!dpCheckIn.SelectedDate.HasValue || dpCheckIn.SelectedDate.Value < DateTime.Now.Date)
            {
                MessageBox.Show("Invalid Check-in date! Please select today or a future date.", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dpCheckOut.SelectedDate.HasValue || dpCheckOut.SelectedDate.Value < DateTime.Now.Date)
            {
                MessageBox.Show("Invalid Check-out date! Please select today or a future date.", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime startDate = dpCheckIn.SelectedDate.Value;

            DateTime endDate = dpCheckOut.SelectedDate.Value;

            if (startDate >= endDate)
            {
                MessageBox.Show("Check-out date must be after the Check-in date!", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

                IBookingService bookingService = new BookingService();
                var result = bookingService.FilterDateToChoose(startDate, endDate);

                if (result != null)
                {
                    icRooms.ItemsSource = result;

                    if (result.Count == 0)
                    {
                        MessageBox.Show("No rooms available for the selected dates!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while filtering data: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void rdStatus_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;

            IBookingService bookingService = new BookingService();

            // 1. Lấy mốc thời gian hiện tại trên DatePicker để đảm bảo cấu trúc 1 phòng = 1 thẻ
            DateTime startDate = dpCheckIn.SelectedDate ?? DateTime.Today;
            DateTime endDate = dpCheckOut.SelectedDate ?? DateTime.Today.AddDays(1);

            // Lấy danh sách gốc đã được chuẩn hóa
            List<Booking> result = bookingService.FilterDateToChoose(startDate, endDate);

            // 2. Lọc theo trạng thái
            if (rdAvailable.IsChecked == true)
            {
                // Phòng Available là phòng chưa có dữ liệu Booking (BookingStatus là null do hàm FilterDateToChoose tạo ra)
                result = result.Where(r => r.BookingStatus == null).ToList();
            }
            else if (rdBooked.IsChecked == true)
            {
                // Phòng Occupied là phòng đã có dữ liệu Booking đi kèm
                result = result.Where(r => r.BookingStatus != null).ToList();
            }
            else if (rdMaintenance.IsChecked == true)
            {
                // Vì FilterDateToChoose loại bỏ phòng Maintenance, ta cần lấy trực tiếp từ RoomService
                IRoomService roomService = new RoomService();
                var maintenanceRooms = roomService.FilterRoomsbyStatus("Maintenance");

                // Đóng gói lại thành object Booking giả để tương thích với XAML Binding
                result = maintenanceRooms.Select(room => new Booking
                {
                    RoomId = room.RoomId,
                    Room = room,
                    BookingStatus = "Maintenance",
                    BookingId = 0
                }).ToList();
            }

            // 3. Cập nhật giao diện
            icRooms.ItemsSource = result;
        }
    }
}
