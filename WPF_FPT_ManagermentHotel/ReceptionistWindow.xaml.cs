using BussinessObjects;
using DataAccessLayer;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for ReceptionistWindow.xaml
    /// </summary>
    public partial class ReceptionistWindow : Window
    {
        private readonly Booking booking;
        private readonly Account account;
        public ReceptionistWindow(Account acc, Booking book)
        {
            InitializeComponent();
            this.account = acc;
            this.booking = book;
            this.DataContext = booking;
            LoadBooking();
        }

        private void LoadBooking()
        {
            IBookingService bookingService = new BookingService();
            dgBookings.ItemsSource = bookingService.GetBooking();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            var selectedBooking = btn.DataContext as Booking;
            var currentAccount = this.account;

            if (selectedBooking != null && currentAccount != null)
            {
                ConfirmReservationDialog confirmReservationDialog = new ConfirmReservationDialog(currentAccount, selectedBooking);
                confirmReservationDialog.Owner = this;
                confirmReservationDialog.ShowDialog();

                // TỰ ĐỘNG TẢI LẠI: Sau khi đóng cửa sổ check-in
                LoadBooking();
            }
            else
            {
                MessageBox.Show("No booking information or valid account found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBooking();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = txtSearch.Text;
            if (string.IsNullOrWhiteSpace(search))
            {
                LoadBooking();
                return;
            }

            try
            {
                IBookingService bookingService = new BookingService();
                var result = bookingService.SearchBookingByName(search);

                if (result != null)
                {
                    dgBookings.ItemsSource = result;
                }
                else
                {
                    dgBookings.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BtnExtend_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            var selected = btn.DataContext as Booking;
            var CurrentAccount = this.account;
            if (selected != null)
            {
                ExtendReservationDialog extendReservationDialog = new ExtendReservationDialog(CurrentAccount, selected);
                if (extendReservationDialog.ShowDialog() == true)
                {
                    LoadBooking();
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn == null) return;

            var selected = btn.DataContext as Booking;
            var CurrentAccount = this.account;
            if (selected != null)
            {
                UpdateReservationDialog updateReservation = new UpdateReservationDialog(CurrentAccount, selected);
                updateReservation.ShowDialog();

                // TỰ ĐỘNG TẢI LẠI: Sau khi sửa thông tin đặt phòng
                LoadBooking();
            }
        }

        private void BtnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            var selected = btn.DataContext as Booking;
            var CurrentAccount = this.account;
            if (selected != null)
            {
                CheckOutWindow checkOutWindow = new CheckOutWindow(CurrentAccount, selected);
                checkOutWindow.ShowDialog();

                // TỰ ĐỘNG TẢI LẠI: Sau khi làm thủ tục trả phòng (Check-Out)
                LoadBooking();
            }
        }

        private void FilterStatus_Changed(object sender, RoutedEventArgs e)
        {
            if (dgBookings == null || chkConfirmed == null || chkCheckedOut == null || chkCancelled == null)
            {
                return;
            }

            List<string> selectStatus = new List<string>();
            if (chkConfirmed.IsChecked == true) selectStatus.Add("Confirmed");
            if (chkCheckedOut.IsChecked == true) selectStatus.Add("CheckedOut");
            if (chkCancelled.IsChecked == true) selectStatus.Add("Cancelled");

            IBookingService service = new BookingService();
            var filterStatus = new List<BussinessObjects.Booking>();

            foreach (string status in selectStatus)
            {
                var result = service.GetBookingByStatus(status);
                if (result != null)
                {
                    filterStatus.AddRange(result);
                }
            }
            dgBookings.ItemsSource = filterStatus;
        }

        private void BtnRowService_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            var selected = btn.DataContext as Booking;

            if (selected != null)
            {
                ServiceBillWindow service = new ServiceBillWindow(selected);
                service.ShowDialog();

                // TỰ ĐỘNG TẢI LẠI: Cập nhật hóa đơn dịch vụ
                LoadBooking();
            }
        }

        private void btnService_Click(object sender, RoutedEventArgs e)
        {
            ServiceWindow serviceWindow = new ServiceWindow(account, booking);
            serviceWindow.Show();
            this.Close(); // Giữ ẩn cửa sổ này để bảo lưu trạng thái
        }

        private void btnRoomReservation_Click(object sender, RoutedEventArgs e)
        {
            RoomReservationWindow roomReservationWindow = new RoomReservationWindow(account, booking);
            roomReservationWindow.Show();
            this.Close(); // Giữ ẩn cửa sổ này để bảo lưu trạng thái
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you want to cancel this booking?",
                                                "Confirm cancel this booking",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Warning
                                                 );

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Button btn = sender as Button;
                if (btn == null) return;

                var selected = btn.DataContext as Booking;

                try
                {
                    if (selected != null)
                    {
                        IBookingService service = new BookingService();
                        var result = service.CancelBooking(selected.BookingId);
                        if (result != null)
                        {
                            MessageBox.Show("Cancel Successfully !!!", "Cancel", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadBooking();
                        }
                        else
                        {
                            MessageBox.Show("Cancel Fail !!!", "Cancel", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

     
        
        
    }
}