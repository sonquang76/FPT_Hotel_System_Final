using BussinessObjects;
using Services;
using System;
using System.Linq;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for InvoiceDialog.xaml
    /// </summary>
    public partial class InvoiceDialog : Window
    {
        private readonly Account account;
        private readonly Booking booking;
        private bool _isSaved = false; // Biến kiểm tra xem đã bấm Save chưa
        public InvoiceDialog(Account acc, Booking book)
        {
            InitializeComponent();
            this.account = acc;
            this.booking = book;
            this.Closing += Window_Closing;
            LoadInvoice();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isSaved)
            {
                MessageBox.Show("Please click 'Save' before closing this invoice!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Cancel = true; // Hủy lệnh đóng cửa sổ
            }
        }

        private void LoadInvoice()
        {
            IBookingService bookingService = new BookingService();
            var b = bookingService.GetBookingById(booking.BookingId) ?? this.booking;

            txtInvoiceID.Text = b?.Invoice?.InvoiceId.ToString() ?? "N/A";
            txtBookingID.Text = b?.BookingId.ToString() ?? "N/A";
            txtCreatedBy.Text = b?.CreatedBy ?? "Unknown";

            txtCustomerName.Text = b?.CreatedByNavigation?.FullName ?? "Unknown";
            txtEmail.Text = b?.CreatedByNavigation?.Email ?? "Unknown";
            txtPhone.Text = b?.CreatedByNavigation?.Phone ?? "Unknown";

            txtRoomNumber.Text = b?.Room?.RoomNumber ?? "Unknown";
            txtRoomType.Text = b?.Room?.RoomType?.TypeName ?? "Unknown";
            txtPricePerNight.Text = b?.Room?.RoomType != null ? b.Room.RoomType.BasePrice.ToString("N0") : "Unknown";
            txtRoomDescription.Text = b?.Room?.Description ?? "Unknown";

            txtExpectedCheckIn.Text = b?.ExpectedCheckIn.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
            txtExpectedCheckOut.Text = b?.ExpectedCheckOut.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
            txtActualCheckIn.Text = b?.CheckInDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
            txtActualCheckOut.Text = b?.CheckOutDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

            DateTime checkin = b?.CheckInDate ?? b?.ExpectedCheckIn ?? DateTime.Today;
            DateTime checkout = b?.CheckOutDate ?? b?.ExpectedCheckOut ?? checkin.AddDays(1);
            int night = (checkout - checkin).Days;
            if (night <= 0) night = 1;
            txtNight.Text = night.ToString();

            // 1. Phí phòng ban đầu (Room Charge)
            decimal totalRoomCharge = night * (b?.Room?.RoomType?.BasePrice ?? 0m);
            txtRoomCharge.Text = totalRoomCharge.ToString("N0") + " VND";

            // 2. Phí dịch vụ (Service Charge)
            decimal totalService = b?.Serviceorders?.Sum(s => (s.Service?.Price ?? 0m) * s.Quantity) ?? 0m;
            txtServiceCharge.Text = totalService.ToString("N0") + " VND";

            // 3. Tổng phụ (Sub Total) = Tiền phòng + Tiền dịch vụ
            decimal subTotal = totalRoomCharge + totalService;
            txtSubTotal.Text = subTotal.ToString("N0") + " VND";

            // 4. Khấu trừ tiền cọc đã trả trực tuyến (Hiển thị ở ô Discount)
            decimal deposit = 0m;
            if (b?.Payments != null)
            {
                deposit = b.Payments
                    .Where(p => p.PaymentStatus == "Paid" && p.PaymentMethod == "Online")
                    .Sum(p => p.Amount);
            }
            txtDiscount.Text = deposit.ToString("N0") + " VND";

            // 5. Thuế VAT 10%
            txtVAT.Text = "10";

            // 6. Số tiền còn lại phải thanh toán cuối cùng (đã trừ cọc và cộng 10% VAT)
            decimal remainingBeforeTax = subTotal - deposit;
            if (remainingBeforeTax < 0) remainingBeforeTax = 0;

            decimal vatAmount = remainingBeforeTax * 0.10m;
            decimal grandTotal = remainingBeforeTax + vatAmount;

            txtTotalAmount.Text = grandTotal.ToString("N0") + " VND";
            lblGrandTotal.Text = grandTotal.ToString("N0") + " VND";

            txtReceptionist.Text = this.account?.FullName ?? "Unknown";

            dgServices.ItemsSource = b.Serviceorders?.ToList() ?? new System.Collections.Generic.List<Serviceorder>();

            dpPaymentDate.SelectedDate = DateTime.Today;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Print invoice successfully!!!, please check your bill again before leave it!!!",
                "Print Invoice",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            IBookingService bookingService = new BookingService();
            var result = bookingService.CreateInvoice(booking.BookingId);
            if (result != null)
            {
                _isSaved = true; // Đánh dấu là đã lưu thành công
                MessageBox.Show("Save invoice successfully!!!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (!_isSaved)
            {
                MessageBox.Show("Please click 'Save' before closing this invoice!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Chặn không cho thoát nếu chưa lưu
            }

           
            Close();
        }
    }
}