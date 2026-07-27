using BussinessObjects;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace WPF_FPT_ManagementHotel
{
    public partial class PaymentOnlineWindow : Window
    {
        private readonly Booking booking;
        private readonly Account account;

        private readonly IBookingService bookingService;
        private readonly IPaymentService paymentService;

        public PaymentOnlineWindow(Account account, Booking book)
        {
            InitializeComponent();

            this.account = account;
            this.booking = book;

            bookingService = new BookingService();
            paymentService = new PaymentService();

            LoadPayment();
        }

        private void LoadPayment()
        {
            txtRoomNumber.Text = booking.Room.RoomNumber;
            txtRoomType.Text = booking.Room.RoomType.TypeName;
            txtFloor.Text = booking.Room.Floor.ToString();

            decimal price = booking.Room.RoomType.BasePrice;
            txtPrice.Text = price.ToString("N0") + " VND";

            txtMethod.Text = "Online";
            txtStatus.Text = "Pending";

            dpCheckIn.DisplayDateStart = DateTime.Today;
            dpCheckOut.DisplayDateStart = DateTime.Today;
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            if (!dpCheckIn.SelectedDate.HasValue || !dpCheckOut.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select Check-in and Check-out dates.");
                return;
            }

            DateTime checkIn = dpCheckIn.SelectedDate.Value;
            DateTime checkOut = dpCheckOut.SelectedDate.Value;

            if (checkOut <= checkIn)
            {
                MessageBox.Show("Check-out must be after Check-in.");
                return;
            }

            try
            {
                // 1. KHỞI TẠO ĐỐI TƯỢNG BOOKING MỚI (Đồng bộ với room và account)
                Booking newBookingRequest = new Booking
                {
                    RoomId = this.booking.Room.RoomId,
                    ExpectedCheckIn = checkIn,
                    ExpectedCheckOut = checkOut,
                    BookingStatus = "Booked",
                    CreatedBy = account.AccountId, // Gán ID tài khoản đang đăng nhập
                    CustomerId = 1
                };

                // 2. Gọi hàm TẠO MỚI (CreateReservation) xuống Database
                var newBooking = bookingService.CreateReservation(newBookingRequest);

                if (newBooking == null)
                {
                    MessageBox.Show("Failed to create booking. Room may be unavailable or conflict timing.");
                    return;
                }

                // 3. Tiến hành xử lý thanh toán dựa trên BookingId vừa được Database sinh tự động
                bool paid = paymentService.ProcessOnlineDeposit(newBooking.BookingId);

                if (!paid)
                {
                    MessageBox.Show("Payment failed.");
                    return;
                }

                MessageBox.Show("Payment successful!");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                // In ra lỗi chi tiết nếu có phát sinh trong quá trình lưu dữ liệu
                MessageBox.Show($"An error occurred: {ex.Message}\nInner Exception: {ex.InnerException?.Message}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!dpCheckIn.SelectedDate.HasValue || !dpCheckOut.SelectedDate.HasValue)
                return;

            if (dpCheckOut.SelectedDate <= dpCheckIn.SelectedDate)
                return;

            int nights = (dpCheckOut.SelectedDate.Value - dpCheckIn.SelectedDate.Value).Days;

            decimal pricePerNight = booking.Room.RoomType.BasePrice;
            decimal total = pricePerNight * nights;
            decimal deposit = total * 0.3m;

            txtNight.Text = nights.ToString();
            txtTotal.Text = deposit.ToString("N0") + " VND";
            txtDeposit.Text = deposit.ToString("N0") + " VND";
        }
    }
}