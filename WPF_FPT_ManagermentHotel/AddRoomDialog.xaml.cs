using BussinessObjects;
using Services;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for AddRoomDialog.xaml
    /// </summary>
    public partial class AddRoomDialog : Window
    {
        public AddRoomDialog()
        {
            InitializeComponent();
            LoadRoomType();
        }

        private void LoadRoomType()
        {
            IRoomTypeService roomTypeService = new RoomTypeService();
            cbRoomType.ItemsSource = roomTypeService.GetRoomtypes();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string roomNumber = txtRoomNumber.Text;
            if (string.IsNullOrWhiteSpace(roomNumber))
            {
                MessageBox.Show("Please enter a room number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cbRoomType.SelectedItem == null)
            {
                MessageBox.Show("Please select a room type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Roomtype roomtype = (Roomtype)cbRoomType.SelectedItem;
            int roomTypeId = roomtype.RoomTypeId;

            int floor;
            if (!int.TryParse(txtFloor.Text, out floor))
            {
                MessageBox.Show("Please enter a valid floor number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Room room = new Room()
            {
                RoomNumber = roomNumber,
                RoomTypeId = roomTypeId,
                Status = "Available",
                Floor = floor
            };
            IRoomService roomService = new RoomService();
            var result = roomService.CreateRoom(room);

            if (result != null)
            {
                MessageBox.Show("Room added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Failed to add room.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
