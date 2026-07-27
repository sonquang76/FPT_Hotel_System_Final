using BussinessObjects;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private readonly Account account;
        public ManagerWindow(Account acc)
        {
            InitializeComponent();
            this.account = acc;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void ManageProfile_Click(object sender, RoutedEventArgs e)
        {
            AccountInformationDialog accountInformationDialog = new AccountInformationDialog(account);
            accountInformationDialog.ShowDialog();
        }

        private void ManageRooms_Click(object sender, RoutedEventArgs e)
        {
            Room room = new Room();
            RoomManagementWindow roomManagementWindow = new RoomManagementWindow(room, account);
            roomManagementWindow.Show();
            Close();
        }

        private void ManageCustomers_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow(account);
            accountManagementWindow.Show();
            Close();
        }

        //private void ManageBookings_Click(object sender, RoutedEventArgs e)
        //{
        //    BookingManagementWindow bookingManagementWindow = new BookingManagementWindow();
        //    bookingManagementWindow.Show();
        //    Close();
        //}

        private void ManageRoomType_Click(object sender, RoutedEventArgs e)
        {
            Roomtype roomtype = new Roomtype();
            RoomTypeManagementWindow roomTypeManagementWindow = new RoomTypeManagementWindow(roomtype, account);
            roomTypeManagementWindow.Show();
            Close();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            Invoice invoice = new Invoice();
            ReportWindow reportWindow = new ReportWindow(invoice, account);
            reportWindow.Show();
            Close();
        }
    }
}
