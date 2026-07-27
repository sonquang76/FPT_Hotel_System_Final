using BussinessObjects;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for BookingHistoryWindow.xaml
    /// </summary>
    public partial class BookingHistoryWindow : Window
    {
        private readonly Account account;
        public BookingHistoryWindow(Account acc)
        {
            InitializeComponent();
            this.account = acc;
            LoadBookingHistory();
        }

        private void LoadBookingHistory()
        {
            IBookingService bookingService = new BookingService();
            icBookings.ItemsSource = bookingService.GetGuestHistory(account.AccountId);
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
            {
                MessageBox.Show("No booking selected!");
                return;
            }

            var booking = btn.DataContext as Booking;

            if (booking != null)
            {
                RoomAfterBookingWindow roomAfterBookingWindow = new RoomAfterBookingWindow(account, booking);
                roomAfterBookingWindow.Show();
                Close();
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
               "Are you sure you want to cancel this booking?",
               "Confirm",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            Button btn = sender as Button;
            if (btn == null)
            {
                MessageBox.Show("No booking selected!");
                return;
            }

            var booking = btn.DataContext as Booking;
            if (booking != null)
            {
                if (result != MessageBoxResult.Yes)
                    return;

                IBookingService bookingService = new BookingService();
                bool cancel = bookingService.CancelBooking(booking.BookingId);
                if (cancel)
                {
                    MessageBox.Show("Booking cancelled successfully.");
                    return;
                }
                else
                {
                    MessageBox.Show("Booking cancelled already!!!.");
                    return;
                }
            }
        }
    }
}
