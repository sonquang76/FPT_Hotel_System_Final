using BussinessObjects;
using Services;
using System.Windows;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for UpdateRoomDialog.xaml
    /// </summary>
    public partial class UpdateRoomDialog : Window
    {
        private readonly Room room;
        public UpdateRoomDialog(Room Udroom)
        {
            InitializeComponent();
            this.room = Udroom;
            this.DataContext = room;
            LoadRoom();
        }

        private void LoadRoom()
        {
            IRoomTypeService roomTypeService = new RoomTypeService();
            txtRoomNumber.Text = room.RoomNumber;
            txtFloor.Text = room.Floor.ToString();
            cbRoomType.ItemsSource = roomTypeService.GetRoomtypes();
            cbRoomType.SelectedValue = room.RoomTypeId;
            cbStatus.Text = room.Status;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string roomNumber = txtRoomNumber.Text;
            if (string.IsNullOrEmpty(roomNumber))
            {
                MessageBox.Show("Please fill all empty, check again!!!", "Room Number", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (cbRoomType.SelectedItem == null)
            {
                MessageBox.Show("Please select a room type, check again!!!", "Room Type", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Roomtype roomtype = cbRoomType.SelectedItem as Roomtype;
            int roomTypeId = roomtype.RoomTypeId;

            string status = cbStatus.Text;
            if (string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Please select a status, check again!!!", "Status", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int floor;
            if (!int.TryParse(txtFloor.Text, out floor))
            {
                MessageBox.Show("Please enter a valid floor number, check again!!!", "Floor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            room.RoomNumber = roomNumber;
            room.RoomTypeId = roomTypeId;
            room.Status = status;
            room.Floor = floor;

            IRoomService roomService = new RoomService();
            var result = roomService.UpdateRoom(room);

            if (result != null)
            {
                MessageBox.Show("Update room successfully!", "Update Room", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();

            }
            else
            {
                MessageBox.Show("Update room failed!", "Update Room", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
        }
    }
}
