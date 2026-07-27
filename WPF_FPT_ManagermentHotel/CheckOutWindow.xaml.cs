using BussinessObjects;
using Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for CheckOutWindow.xaml
    /// </summary>
    public partial class CheckOutWindow : Window
    {
        private readonly Account account;
        private readonly Booking booking;
        public CheckOutWindow(Account acc, Booking book)
        {
            InitializeComponent();
            this.account = acc;
            this.booking = book;
            LoadBooking();
        }

        private void LoadBooking()
        {
            if (booking == null)
            {
                return;
            }
            txtCustomer.Text = booking.CreatedByNavigation.FullName ?? "[Unknown]";
            txtRoom.Text = booking.Room.RoomNumber ?? "[Unknown]";
            txtExpectedCheckout.Text = booking.ExpectedCheckOut.ToString();
            txtCheckIn.Text = booking.CheckInDate.ToString();
            txtRoomPrice.Text = booking.Room?.RoomType?.BasePrice.ToString("N0") + "VND" ?? "0";
            txtServicePrice.Text = booking.Serviceorders.Count().ToString() ?? "0";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            IBookingService bookingService = new BookingService();
            var result = bookingService.GuestCheckout(booking.BookingId);

            if (result)
            {
                InvoiceDialog invoiceDialog = new InvoiceDialog(account, booking);
                invoiceDialog.Show();
                Close();

            }
        }
    }
}
