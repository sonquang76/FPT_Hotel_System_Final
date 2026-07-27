using BussinessObjects;
using Services;
using System.Windows;
using System.Windows.Media;

namespace WPF_FPT_ManagementHotel
{
    public partial class RoomAfterBookingWindow : Window
    {
        private readonly Account account;
        private readonly Booking booking;

        public RoomAfterBookingWindow(Account acc, Booking book)
        {
            InitializeComponent();

            account = acc;
            booking = book;

            // Room Information
            txtRoomNumber.Text = booking.Room.RoomNumber;
            txtRoomType.Text = booking.Room.RoomType.TypeName;
            txtFloor.Text = booking.Room.Floor.ToString();

            // Booking Information
            txtCheckIn.Text = booking.ExpectedCheckIn.ToString("dd/MM/yyyy");
            txtCheckOut.Text = booking.ExpectedCheckOut.ToString("dd/MM/yyyy");

            int nights = (booking.ExpectedCheckOut - booking.ExpectedCheckIn).Days;
            txtNight.Text = nights.ToString();

            // Deposit
            var payment = booking.Payments.FirstOrDefault();

            if (payment != null)
            {
                txtDeposit.Text = payment.Amount.ToString("N0") + " VND";
            }
            else
            {
                txtDeposit.Text = "Not Paid";
            }

            // Description
            txtDescription.Text = booking.Room.Description;

            // Status
            txtStatus.Text = booking.BookingStatus;

            switch (booking.BookingStatus)
            {
                case "Booked":
                    bdStatus.Background = Brushes.LightGreen;
                    break;

                case "Confirmed":
                    bdStatus.Background = Brushes.Gold;
                    break;

                case "CheckedIn":
                    bdStatus.Background = Brushes.LightSkyBlue;
                    break;

                case "CheckedOut":
                    bdStatus.Background = Brushes.LightGray;
                    btnCancelBooking.IsEnabled = false;
                    break;

                case "Cancelled":
                    bdStatus.Background = Brushes.IndianRed;
                    btnCancelBooking.IsEnabled = false;
                    break;

                default:
                    bdStatus.Background = Brushes.Gray;
                    break;
            }
        }

        private void btnCancelBooking_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to cancel this booking?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            IBookingService bookingService = new BookingService();
            bool cancel = bookingService.CancelBooking(booking.BookingId);
            if (cancel)
            {
                MessageBox.Show("Booking cancelled successfully.");
                Close();
            }

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(account , booking);
            mainWindow.Show();
            Close();
        }
    }
}