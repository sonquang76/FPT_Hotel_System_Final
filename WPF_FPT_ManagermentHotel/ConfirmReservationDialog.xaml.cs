using BussinessObjects;
using Services;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for ConfirmReservationDialog.xaml
    /// </summary>

    public partial class ConfirmReservationDialog : Window
    {
        private readonly Booking booking;
        private readonly Account account;
        public ConfirmReservationDialog(Account acc, Booking book)
        {
            InitializeComponent();
            this.account = acc;
            this.booking = book;
            this.DataContext = booking;
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReceptionistWindow receptionistWindow = new ReceptionistWindow(account, booking);
            receptionistWindow.Show();
            Close();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            IBookingService bookingService = new BookingService();

            string message;
            var confirm = bookingService.ConfirmBooking(booking.BookingId, out message);
            if (confirm)
            {   
                MessageBox.Show(message,
                                "Notification",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                this.Close();
            }
            else
            {
                MessageBox.Show(message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
