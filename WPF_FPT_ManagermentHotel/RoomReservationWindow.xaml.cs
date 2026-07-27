using BussinessObjects;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for RoomReservationWindow.xaml
    /// </summary>
    public partial class RoomReservationWindow : Window
    {
        private Booking selectedBooking;
        private readonly Booking booking;
        private readonly Account account;
        public RoomReservationWindow(Account acc, Booking book)
        {
            InitializeComponent();

            this.account = acc;
            this.booking = book;
            this.DataContext = booking;

            // Đăng ký sự kiện: Khi nào màn hình vẽ xong hoàn toàn thì mới chạy code nội bộ
            this.Loaded += RoomReservationWindow_Loaded;
        }

        private void RoomReservationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Code ở đây chỉ chạy khi mọi Button, ListBox trong XAML đã khởi tạo xong 100%
            LoadBoking();
            ClearForm();

            // Gán ngày ở đây sẽ kích hoạt sự kiện lọc một cách an toàn, không bao giờ bị crash nữa
            dpFilterStartDate.SelectedDate = DateTime.Now.Date;
            dpFilterEndDate.SelectedDate = DateTime.Now.Date.AddDays(1);
        }
        private void ClearForm()
        {
            txtRoomNumber.Clear();
            txtRoomType.Clear();
            txtCapacity.Clear();
            txtPrice.Clear();
            txtStatus.Clear();

            txtCustomerName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtCitizenID.Clear();

            txtNote.Clear();

            dpBookingCheckIn.SelectedDate = null;
            dpBookingCheckOut.SelectedDate = null;

            cbBookingGuest.SelectedIndex = -1;

            selectedBooking = null;

            if (lbRooms != null)
            {
                lbRooms.SelectedIndex = -1;
            }
        }

        private void LoadBoking()
        {
            if (lbRooms == null) return;

            try
            {
                IBookingService bookingService = new BookingService();
                IRoomService roomService = new RoomService();

                // 1. Lấy ngày lọc (nếu DatePicker trên UI chưa chọn thì mặc định là hôm nay)
                DateTime startDate = dpFilterStartDate?.SelectedDate ?? DateTime.Today;
                DateTime endDate = dpFilterEndDate?.SelectedDate ?? DateTime.Today.AddDays(1);

                // 2. Lấy TẤT CẢ các phòng từ Database để đảm bảo hiển thị đủ 27 phòng
                var allRooms = roomService.GetRooOoccupancyReport() ?? new List<Room>();

                // 3. Lấy toàn bộ danh sách Booking để so sánh lịch bận/trống
                var allBookings = bookingService.GetBooking() ?? new List<Booking>();

                // 4. Tìm danh sách RoomId đang có lịch trùng trong khoảng thời gian được chọn
                var conflictedRoomIds = allBookings
                    .Where(b => b.BookingStatus != "Cancelled" &&
                                b.BookingStatus != "CheckedOut" &&
                                b.ExpectedCheckIn < endDate &&
                                b.ExpectedCheckOut > startDate)
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToList();

                // 5. Duyệt qua từng phòng gốc và đóng gói thành List<Booking> tương thích với XAML
                var boardItems = allRooms.Select(room =>
                {
                    // Kiểm tra và gán trạng thái trực tiếp vào thực thể Room
                    bool isOccupied = conflictedRoomIds.Contains(room.RoomId);
                    room.Status = isOccupied ? "Occupied" : "Available";

                    // Tìm Booking thực tế đang diễn ra tại phòng này (nếu có)
                    var activeBooking = allBookings.FirstOrDefault(b => b.RoomId == room.RoomId &&
                                        (b.BookingStatus == "Booked" || b.BookingStatus == "CheckedIn"));

                    // Trả về Object Booking để không làm lỗi Binding giao diện của bạn
                    return new Booking
                    {
                        RoomId = room.RoomId,
                        Room = room, // Khác null, hoàn toàn hợp lệ

                        // Tránh lỗi kiểu dữ liệu DateTime (không được null)
                        ExpectedCheckIn = activeBooking != null ? activeBooking.ExpectedCheckIn : DateTime.Today,
                        ExpectedCheckOut = activeBooking != null ? activeBooking.ExpectedCheckOut : DateTime.Today.AddDays(1),

                        // Đồng bộ trạng thái hiển thị của Row/Card
                        BookingStatus = isOccupied ? "CheckedIn" : "CheckedOut",

                        // SỬA LỖI: Tạo instance rỗng để thỏa mãn ràng buộc "= null!;" trong BusinessObjects
                        CreatedByNavigation = activeBooking?.CreatedByNavigation ?? new Account { FullName = "N/A" },
                        Customer = activeBooking?.Customer ?? new Customer { FullName = "N/A" },
                        CreatedBy = activeBooking?.CreatedBy ?? string.Empty
                    };
                }).ToList();

                // 6. Đổ dữ liệu đã xử lý mượt mà lên giao diện
                lbRooms.ItemsSource = boardItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading room list: {ex.Message}", "System error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBookRoom_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra chọn phòng
            if (selectedBooking == null)
            {
                MessageBox.Show("Please select a room.");
                return;
            }

            // Lấy thông tin khách
            string name = txtCustomerName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();
            string citizenID = txtCitizenID.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter customer name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(citizenID))
            {
                MessageBox.Show("Please fill all required information.");
                return;
            }

            // Kiểm tra ngày
            if (dpBookingCheckIn.SelectedDate == null ||
                dpBookingCheckOut.SelectedDate == null)
            {
                MessageBox.Show("Please select Check In and Check Out date.");
                return;
            }

            if (dpBookingCheckOut.SelectedDate <= dpBookingCheckIn.SelectedDate)
            {
                MessageBox.Show("Check Out must be after Check In.");
                return;
            }

            DateTime checkIn = dpBookingCheckIn.SelectedDate.Value.Date.AddHours(12);
            DateTime checkOut = dpBookingCheckOut.SelectedDate.Value.Date.AddHours(12);

            try
            {
                IAccountService accountService = new AccountService();
                ICustomerService customerService = new CustomerService();
                IBookingService bookingService = new BookingService();

                // Tìm Account theo CCCD
                Account customerAccount = accountService.GetAccountByCitizenId(citizenID);

                // Nếu chưa có Account thì tạo mới
                if (customerAccount == null)
                {
                    customerAccount = new Account()
                    {
                        FullName = name,
                        Email = email,
                        Phone = phone,
                        IdentityCard = citizenID,
                        Password = "12345",
                        AccountStatus = "Active",
                        Gender = "Male",       // hoặc giá trị mặc định của bạn
                        Dob = DateTime.Now.AddYears(-20),
                        Roles = new List<Role>()

                    };

                    accountService.SignUpAccount(customerAccount);

                    // Lấy lại Account vừa tạo
                    customerAccount = accountService.GetAccountByCitizenId(citizenID);

                    if (customerAccount == null)
                    {
                        MessageBox.Show("Cannot create customer account.");
                        return;
                    }
                }

                Customer customer = customerService.GetCustomerByCitizenId(citizenID);

                if (customer == null)
                {
                    MessageBox.Show("Customer not found.");
                    return;
                }

                // Tạo Booking
                Booking newBooking = new Booking()
                {
                    CustomerId = customer.CustomerId,
                    RoomId = selectedBooking.RoomId,

                    ExpectedCheckIn = checkIn,

                    ExpectedCheckOut = checkOut,

                    BookingStatus = "Booked",

                    CreatedBy = customerAccount.AccountId
                };

                Booking result = bookingService.CreateReservation(newBooking);

                if (result != null)
                {
                    MessageBox.Show(
                        $"Room {selectedBooking.Room.RoomNumber} booked successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    LoadBoking();
                }
                else
                {
                    MessageBox.Show(
                        "Room is unavailable or already booked during this period.",
                        "Booking Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            dpFilterStartDate.SelectedDate = DateTime.Now.Date;
            dpFilterEndDate.SelectedDate = DateTime.Now.Date.AddDays(1);

            LoadBoking();
            ClearForm();
        }

        private void btnClearForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
           LoginWindow lw = new LoginWindow();  
           lw.Show();
            this.Close();
        }

        private void lbRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (lbRooms.SelectedItem == null) return;

            var selectedItem = lbRooms.SelectedItem as Booking;
            selectedBooking = selectedItem;

            // DÒNG QUAN TRỌNG NHẤT: Gán DataContext để kích hoạt Trigger hiển thị nút bấm trong XAML
            this.DataContext = selectedItem;

            if (selectedItem != null && selectedItem.Room != null)
            {
                var currentRoom = selectedItem.Room;
                var currentCustomer = selectedItem.CreatedByNavigation;
                txtRoomNumber.Text = currentRoom.RoomNumber;

                txtRoomType.Text = currentRoom.RoomType?.TypeName ?? "N/A";

                txtCapacity.Text = currentRoom.RoomType?.Capacity.ToString() ?? "0";
                txtPrice.Text = currentRoom.RoomType?.BasePrice.ToString("N0") ?? "0";
                txtStatus.Text = currentRoom.Status;
                cbBookingGuest.Text = currentRoom.RoomType?.Capacity.ToString() ?? "1";

                if (selectedItem.CreatedByNavigation != null)
                {
                    txtCustomerName.Text = currentCustomer.FullName;
                    txtPhone.Text = currentCustomer.Phone;
                    txtEmail.Text = currentCustomer.Email;
                    txtCitizenID.Text = currentCustomer.IdentityCard;
                }

                if (selectedItem.Room.Status == "Available")
                {
                    txtCustomerName.Clear();
                    txtPhone.Clear();
                    txtEmail.Clear();
                    txtCitizenID.Clear();
                    dpBookingCheckIn.SelectedDate = null;
                    dpBookingCheckOut.SelectedDate = null;
                }
                else
                {
                    dpBookingCheckIn.SelectedDate = selectedItem.ExpectedCheckIn;
                    dpBookingCheckOut.SelectedDate = selectedItem.ExpectedCheckOut;
                }
            }
        }

        private void dpFilterDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpFilterStartDate == null || dpFilterEndDate == null || lbRooms == null)
                return;
            if (!dpFilterStartDate.SelectedDate.HasValue || !dpFilterEndDate.SelectedDate.HasValue) return;

            DateTime startDate = dpFilterStartDate.SelectedDate.Value.Date;
            DateTime endDate = dpFilterEndDate.SelectedDate.Value.Date;

            if (startDate < DateTime.Today)
            {
                MessageBox.Show("Start date cannot be in the past.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (endDate <= startDate)
            {
                MessageBox.Show("End date must be after start date.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IBookingService bookingService = new BookingService();
                var allBookings = bookingService.GetBooking();

                if (allBookings != null && allBookings.Any())
                {
                    // Lọc ngay từ đầu, chỉ giữ lại các booking có dữ liệu Phòng (Room) hợp lệ
                    var validBookings = allBookings.Where(b => b.Room != null).ToList();

                    // 1. Tìm các RoomId dính lịch đặt (giao thoa thời gian) trong khoảng khách chọn
                    var conflictedRoomIds = validBookings
                        .Where(b => b.BookingStatus != "Cancelled" &&
                                    b.BookingStatus != "CheckedOut" &&
                                    b.ExpectedCheckIn < endDate &&
                                    b.ExpectedCheckOut > startDate)
                        .Select(b => b.RoomId)
                        .Distinct()
                        .ToList();

                    // 2. Gom nhóm theo từng phòng duy nhất
                    var uniqueRoomBoard = validBookings
                        .GroupBy(b => b.RoomId)
                        .Select(group =>
                        {
                            // Ưu tiên lấy đơn đang hoạt động (Booked/CheckedIn)
                            var activeBooking = group.FirstOrDefault(b => b.BookingStatus == "Booked" || b.BookingStatus == "CheckedIn")
                                                ?? group.First();

                            // Chắc chắn không null vì đã lọc ở validBookings
                            if (conflictedRoomIds.Contains(activeBooking.RoomId))
                            {
                                activeBooking.Room.Status = "Occupied";
                            }
                            else
                            {
                                activeBooking.Room.Status = "Available";
                            }

                            return activeBooking;
                        })
                        .ToList();

                    // 3. Đổ dữ liệu an toàn lên ListBox
                    lbRooms.ItemsSource = uniqueRoomBoard;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering dates: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void cbFilterFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 1. Kiểm tra điều kiện an toàn cho UI
            if (cbFilterFloor == null || lbRooms == null || cbFilterFloor.SelectedItem == null) return;
            if (dpFilterStartDate == null || dpFilterEndDate == null) return;

            var items = cbFilterFloor.SelectedItem as ComboBoxItem;
            if (items == null) return;

            var text = items.Content.ToString();

            try
            {
                // Nếu chọn tất cả các tầng, gọi hàm LoadBoking tiêu chuẩn
                if (text == "-- All Floors --")
                {
                    LoadBoking();
                    return;
                }

                // Lấy số tầng (Ví dụ: "1st Floor" -> 1)
                string floorDigit = new string(text.TakeWhile(char.IsDigit).ToArray());
                if (int.TryParse(floorDigit, out int floor))
                {
                    IBookingService bookingService = new BookingService();
                    IRoomService roomService = new RoomService(); // Sử dụng RoomService để lấy đủ phòng gốc

                    DateTime startDate = dpFilterStartDate.SelectedDate ?? DateTime.Today;
                    DateTime endDate = dpFilterEndDate.SelectedDate ?? DateTime.Today.AddDays(1);

                    // LẤY PHÒNG LÀM GỐC: Chỉ lấy các phòng thuộc tầng được chọn (Đảm bảo hiện đủ số phòng của tầng đó)
                    var allRoomsOnFloor = roomService.GetRooOoccupancyReport()?
                                                     .Where(r => r.Floor == floor)
                                                     .ToList() ?? new List<Room>();

                    if (!allRoomsOnFloor.Any())
                    {
                        MessageBox.Show($"No rooms found on the floor {floor}!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                        lbRooms.ItemsSource = null;
                        return;
                    }

                    // Lấy toàn bộ danh sách Booking của hệ thống để quét lịch trùng
                    var allBookings = bookingService.GetBooking() ?? new List<Booking>();

                    // Tìm danh sách RoomId đang bị trùng lịch trong khoảng thời gian lọc
                    var conflictedRoomIds = allBookings
                        .Where(b => b.BookingStatus != "Cancelled" &&
                                    b.BookingStatus != "CheckedOut" &&
                                    b.ExpectedCheckIn < endDate &&
                                    b.ExpectedCheckOut > startDate)
                        .Select(b => b.RoomId)
                        .Distinct()
                        .ToList();

                    // Duyệt qua từng phòng của tầng và đóng gói dữ liệu hiển thị động
                    var floorBoardItems = allRoomsOnFloor.Select(room =>
                    {
                        bool isOccupied = conflictedRoomIds.Contains(room.RoomId);
                        room.Status = isOccupied ? "Occupied" : "Available";

                        var activeBooking = allBookings.FirstOrDefault(b => b.RoomId == room.RoomId &&
                                            (b.BookingStatus == "Booked" || b.BookingStatus == "CheckedIn"));

                        return new Booking
                        {
                            RoomId = room.RoomId,
                            Room = room,
                            ExpectedCheckIn = activeBooking != null ? activeBooking.ExpectedCheckIn : DateTime.Today,
                            ExpectedCheckOut = activeBooking != null ? activeBooking.ExpectedCheckOut : DateTime.Today.AddDays(1),
                            BookingStatus = isOccupied ? "CheckedIn" : "CheckedOut",

                            // Sửa lỗi tránh Crash do ràng buộc không null (= null!;)
                            CreatedByNavigation = activeBooking?.CreatedByNavigation ?? new Account { FullName = "N/A" },
                            Customer = activeBooking?.Customer ?? new Customer { FullName = "N/A" },
                            CreatedBy = activeBooking?.CreatedBy ?? string.Empty
                        };
                    }).ToList();

                    // Đổ danh sách phòng đã lọc theo tầng lên giao diện
                    lbRooms.ItemsSource = floorBoardItems;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stage filter error: {ex.Message}", "System error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            dpFilterStartDate.SelectedDate = DateTime.Today;
            dpFilterEndDate.SelectedDate = DateTime.Today.AddDays(1);

            if (cbFilterFloor != null) cbFilterFloor.SelectedIndex = 0;
            if (txtSearchRoomNumber != null) txtSearchRoomNumber.Clear();

            LoadBoking();
        }

        private void btnFilterStatus_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.Tag == null || lbRooms == null) return;
            if (dpFilterStartDate == null || dpFilterEndDate == null) return;

            string statusTag = btn.Tag.ToString(); // Lấy trạng thái từ Tag ("All", "Available", "Occupied", v.v.)

            try
            {
                IBookingService bookingService = new BookingService();
                IRoomService roomService = new RoomService();

                // 1. Nếu bấm nút "All", chỉ cần gọi lại hàm LoadBoking() là xong
                if (statusTag == "All")
                {
                    LoadBoking();
                    return;
                }

                DateTime startDate = dpFilterStartDate.SelectedDate ?? DateTime.Today;
                DateTime endDate = dpFilterEndDate.SelectedDate ?? DateTime.Today.AddDays(1);

                // 2. Lấy TẤT CẢ các phòng gốc từ hệ thống
                var allRooms = roomService.GetRooOoccupancyReport() ?? new List<Room>();
                var allBookings = bookingService.GetBooking() ?? new List<Booking>();

                // 3. Tìm các phòng đang dính lịch bận trong khoảng ngày lọc
                var conflictedRoomIds = allBookings
                    .Where(b => b.BookingStatus != "Cancelled" &&
                                b.BookingStatus != "CheckedOut" &&
                                b.ExpectedCheckIn < endDate &&
                                b.ExpectedCheckOut > startDate)
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToList();

                // 4. Tính toán trạng thái động cho từng phòng và lọc theo Tag của nút bấm
                var filteredBoardItems = new List<Booking>();

                foreach (var room in allRooms)
                {
                    // Xác định trạng thái thực tế của phòng tại thời điểm đang lọc ngày
                    bool isOccupied = conflictedRoomIds.Contains(room.RoomId);

                    // Ép trạng thái hiển thị động
                    room.Status = isOccupied ? "Occupied" : "Available";

                    // NẾU TRẠNG THÁI ĐỘNG CỦA PHÒNG KHÔNG TRÙNG VỚI NÚT BẤM FILTER -> BỎ QUA KHÔNG ADD VÀO BOARD
                    // (Ví dụ: Nút bấm chọn "Available" nhưng phòng này đang "Occupied" thì bỏ qua)
                    if (room.Status != statusTag)
                    {
                        continue;
                    }

                    // Nếu phòng thỏa mãn bộ lọc, đóng gói đối tượng Booking để đẩy lên UI không lỗi Binding
                    var activeBooking = allBookings.FirstOrDefault(b => b.RoomId == room.RoomId &&
                                        (b.BookingStatus == "Booked" || b.BookingStatus == "CheckedIn"));

                    filteredBoardItems.Add(new Booking
                    {
                        RoomId = room.RoomId,
                        Room = room,
                        ExpectedCheckIn = activeBooking != null ? activeBooking.ExpectedCheckIn : DateTime.Today,
                        ExpectedCheckOut = activeBooking != null ? activeBooking.ExpectedCheckOut : DateTime.Today.AddDays(1),
                        BookingStatus = isOccupied ? "CheckedIn" : "CheckedOut",

                        // Tránh lỗi gán null cho thuộc tính không được phép null (= null!;)
                        CreatedByNavigation = activeBooking?.CreatedByNavigation ?? new Account { FullName = "N/A" },
                        Customer = activeBooking?.Customer ?? new Customer { FullName = "N/A" },
                        CreatedBy = activeBooking?.CreatedBy ?? string.Empty
                    });
                }

                // 5. Cập nhật lại giao diện hiển thị danh sách phòng đã lọc trạng thái
                lbRooms.ItemsSource = filteredBoardItems;

                // Thông báo nếu không tìm thấy phòng nào có trạng thái tương ứng
                if (!filteredBoardItems.Any())
                {
                    MessageBox.Show($"No rooms found with status '{statusTag}' in this period!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering room status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchRoomNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 1. Kiểm tra an toàn UI
            if (txtSearchRoomNumber == null || lbRooms == null) return;
            if (dpFilterStartDate == null || dpFilterEndDate == null) return;

            string searchKeyword = txtSearchRoomNumber.Text.Trim();

            // Nếu ô tìm kiếm trống, chỉ cần gọi lại LoadBoking() để hiển thị toàn bộ phòng và dừng hàm
            if (string.IsNullOrWhiteSpace(searchKeyword))
            {
                LoadBoking();
                return;
            }

            try
            {
                IBookingService bookingService = new BookingService();
                IRoomService roomService = new RoomService(); // Lấy dữ liệu từ Room gốc

                DateTime startDate = dpFilterStartDate.SelectedDate ?? DateTime.Today;
                DateTime endDate = dpFilterEndDate.SelectedDate ?? DateTime.Today.AddDays(1);

                // 2. Tìm kiếm phòng gốc từ hệ thống có Số phòng chứa từ khóa (Không phân biệt chữ hoa, chữ thường)
                var matchedRooms = roomService.GetRooOoccupancyReport()?
                                              .Where(r => r.RoomNumber.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase))
                                              .ToList() ?? new List<Room>();

                // Nếu không tìm thấy bất kỳ phòng nào khớp số phòng vừa gõ
                if (!matchedRooms.Any())
                {
                    lbRooms.ItemsSource = null;
                    return;
                }

                // 3. Lấy toàn bộ danh sách Booking của hệ thống để quét lịch trùng
                var allBookings = bookingService.GetBooking() ?? new List<Booking>();

                // Tìm danh sách RoomId đang bận trong khoảng thời gian lọc ngày
                var conflictedRoomIds = allBookings
                    .Where(b => b.BookingStatus != "Cancelled" &&
                                b.BookingStatus != "CheckedOut" &&
                                b.ExpectedCheckIn < endDate &&
                                b.ExpectedCheckOut > startDate)
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToList();

                // 4. Duyệt qua danh sách phòng tìm được và gán trạng thái màu sắc động
                var searchResults = matchedRooms.Select(room =>
                {
                    bool isOccupied = conflictedRoomIds.Contains(room.RoomId);
                    room.Status = isOccupied ? "Occupied" : "Available";

                    var activeBooking = allBookings.FirstOrDefault(b => b.RoomId == room.RoomId &&
                                        (b.BookingStatus == "Booked" || b.BookingStatus == "CheckedIn"));

                    return new Booking
                    {
                        RoomId = room.RoomId,
                        Room = room,
                        ExpectedCheckIn = activeBooking != null ? activeBooking.ExpectedCheckIn : DateTime.Today,
                        ExpectedCheckOut = activeBooking != null ? activeBooking.ExpectedCheckOut : DateTime.Today.AddDays(1),
                        BookingStatus = isOccupied ? "CheckedIn" : "CheckedOut",

                        // Sửa lỗi gán null cho các thuộc tính bắt buộc (= null!;)
                        CreatedByNavigation = activeBooking?.CreatedByNavigation ?? new Account { FullName = "N/A" },
                        Customer = activeBooking?.Customer ?? new Customer { FullName = "N/A" },
                        CreatedBy = activeBooking?.CreatedBy ?? string.Empty
                    };
                }).ToList();

                // 5. Đổ kết quả tìm kiếm kèm trạng thái chuẩn lên giao diện
                lbRooms.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while searching: {ex.Message}", "System error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelRoom_Click(object sender, RoutedEventArgs e)
        {
            // Sử dụng ngay biến toàn cục selectedBooking (hoặc lấy từ DataContext của Window)
            if (selectedBooking == null)
            {
                MessageBox.Show("No booking selected!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Hỏi xác nhận trước khi thực hiện hủy
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to cancel this booking?",
                "Confirm Cancellation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            // Gọi Service xử lý hủy
            IBookingService bookingService = new BookingService();
            bool isCancelled = bookingService.CancelBooking(selectedBooking.BookingId);

            if (isCancelled)
            {
                MessageBox.Show("Booking cancelled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // CẬP NHẬT GIAO DIỆN: Gọi hàm load lại danh sách phòng của bạn tại đây để cập nhật màu sắc
                // Ví dụ: LoadRoomList(); hoặc RefreshData();
            }
            else
            {
                // Thông báo chuẩn xác nguyên nhân không hủy được
                MessageBox.Show("Cannot cancel this booking because it has already been Confirmed / Checked-In or the room is currently Occupied!",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // BỔ SUNG CÁC SỰ KIỆN CLICK CỦA SIDEBAR
        private void BtnDashboard_Click(object sender, RoutedEventArgs e) { }

        private void BtnReservations_Click(object sender, RoutedEventArgs e)
        {
            var receptionistWindow = Application.Current.Windows.OfType<ReceptionistWindow>().FirstOrDefault();
            if (receptionistWindow != null)
            {
                receptionistWindow.Show();
            }
            else
            {
                receptionistWindow = new ReceptionistWindow(account, booking);
                receptionistWindow.Show();
            }
            Close();
        }

        private void BtnService_Click(object sender, RoutedEventArgs e)
        {
            ServiceWindow serviceWindow = new ServiceWindow(account, booking);
            serviceWindow.Show();
            Close();
        }

        // Hiện lại cửa sổ chính khi người dùng nhấn nút dấu X đóng cửa sổ này
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            var receptionistWindow = Application.Current.Windows.OfType<ReceptionistWindow>().FirstOrDefault();
            if (receptionistWindow != null)
            {
                receptionistWindow.Show();
            }
        }
    }
}