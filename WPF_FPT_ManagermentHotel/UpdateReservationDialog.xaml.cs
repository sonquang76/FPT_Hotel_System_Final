using System.Windows;
using System.Windows.Controls;
using BussinessObjects;
using Services;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for UpdateReservationDialog.xaml
    /// </summary>
    public partial class UpdateReservationDialog : Window
    {
        private readonly Account account;
        private readonly Booking booking;
        public UpdateReservationDialog(Account acc, Booking book)
        {
            InitializeComponent();
            this.account = acc;
            this.booking = book;
            LoadBooking();
        }

        private void LoadBooking()
        {
            txtCustomerName.Text = booking.CreatedByNavigation.FullName;
            txtEmail.Text = booking.CreatedByNavigation.Email;
            txtPhone.Text = booking.CreatedByNavigation.Phone;
            IRoomService roomService = new RoomService();
            cbRoom.ItemsSource = roomService.GetRoomAvailiable();
            cbRoom.DisplayMemberPath = "RoomNumber";
            cbRoom.SelectedValuePath = "RoomId";
            cbRoom.SelectedValue = booking.RoomId;
            cbStatus.Text = booking.BookingStatus;
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string name = txtCustomerName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Please fill in all information!",
                                "Warning",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (cbRoom.SelectedItem == null)
            {
                MessageBox.Show("Please select a room!",
                                "Warning",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select booking status!",
                                "Warning",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }


            Room selectedRoom = cbRoom.SelectedItem as Room;


            ComboBoxItem statusItem = cbStatus.SelectedItem as ComboBoxItem;
            string status = statusItem.Content.ToString();


            booking.CreatedByNavigation.FullName = name;
            booking.CreatedByNavigation.Email = email;
            booking.CreatedByNavigation.Phone = phone;

            booking.RoomId = selectedRoom.RoomId;
            booking.Room = selectedRoom;

            booking.BookingStatus = status;

            IBookingService bookingService = new BookingService();

            Booking result = bookingService.UpdateReservation(booking);

            if (result != null)
            {
                MessageBox.Show("Update reservation successfully!",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                LoadBooking();
            }
            else
            {
                MessageBox.Show("Update reservation failed!",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
           
            Close();
        }
    }
}
