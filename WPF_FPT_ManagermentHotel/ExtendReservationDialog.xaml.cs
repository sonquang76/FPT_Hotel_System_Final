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
    /// Interaction logic for ExtendReservationDialog.xaml
    /// </summary>
    public partial class ExtendReservationDialog : Window
    {
        private readonly Account account;
        private readonly Booking booking;
        public ExtendReservationDialog(Account acc, Booking book)
        {
            InitializeComponent();
            this.account = acc;
            this.booking = book;
            this.DataContext = booking;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!dpNewCheckOut.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a new check-out date!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime newCheck = dpNewCheckOut.SelectedDate.Value;

            if (newCheck <= booking.ExpectedCheckIn)
            {
                MessageBox.Show("The new check-out date must be after the check-in date!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IBookingService bookingService = new BookingService();
            var result = bookingService.ExtendCheckout(booking.BookingId, newCheck);
            if (result)
            {
                booking.ExpectedCheckOut = newCheck;
                MessageBox.Show("Extend sucessfully!!!", "Extend", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();

            }
            else
            {
                MessageBox.Show("Extend fail, Because it overlap!!! , Check your input again!!!!", "Extend", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
