using BussinessObjects;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for RoomManagementWindow.xaml
    /// </summary>
    public partial class RoomManagementWindow : Window
    {
        private readonly Room room;
        private readonly Account acc;
        public RoomManagementWindow(Room mgRoom, Account acc)
        {
            InitializeComponent();
            this.room = mgRoom;
            this.acc = acc;
            this.DataContext = room;
            LoadRoom();
        }

        private void LoadRoom()
        {
            IRoomService roomService = new RoomService();
            
           var Rooms = roomService.FilterRooms(null, "");
            foreach(var room in Rooms)
            {
                if(room.Status != "Maintenance")
                {
                    room.Status = "Available";
                }
            }
            dgRoom.ItemsSource = Rooms;

            cboFloor.ItemsSource = roomService.FilterRooms(null, "").Select(r => r.Floor).Distinct().ToList();

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddRoomDialog addRoomDialog = new AddRoomDialog();
            if (addRoomDialog.ShowDialog() == true)
            {
                LoadRoom();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            IRoomService roomService = new RoomService();
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Not found room number", "Search Room", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var result = roomService.SearchByRoomNumber(searchText);
            if (result != null)
            {
                dgRoom.ItemsSource = result;
            }
            else
            {
                LoadRoom();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            var selectedRoom = btn.DataContext as Room;
            if (selectedRoom != null)
            {
                UpdateRoomDialog updateRoomDialog = new UpdateRoomDialog(selectedRoom);
                if (updateRoomDialog.ShowDialog() == true)
                {
                    LoadRoom();
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            var selectedRoom = btn.DataContext as Room;
            if (selectedRoom != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete room {selectedRoom.RoomNumber}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                    );
                if (result == MessageBoxResult.Yes)
                {
                    IRoomService roomService = new RoomService();
                    var deleteResult = roomService.DeleteRoom(selectedRoom.RoomId);
                    if (deleteResult)
                    {
                        MessageBox.Show($"Room {selectedRoom.RoomNumber} deleted successfully.", "Delete Room", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadRoom();
                    }
                    else
                    {
                        MessageBox.Show($"Failed to delete room {selectedRoom.RoomNumber}.", "Delete Room", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {

            //ktr status có được chọn ch
            if (cboStatus.Text == null)
            {
                MessageBox.Show("Please select a status to filter.", "Filter Rooms", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // kiểm tra floor được chọn chưa
            if (cboFloor.SelectedItem == null || string.IsNullOrWhiteSpace(cboFloor.Text))
            {
                MessageBox.Show("Please select a floor to filter.", "Filter Rooms", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            string selectedStatus = cboStatus.Text;

            int selectedFloorText;
            if (!int.TryParse(cboFloor.SelectedItem?.ToString(), out selectedFloorText))
            {
                MessageBox.Show("Please select a valid floor number to filter.", "Filter Rooms", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IRoomService roomService = new RoomService();
            var result = roomService.FilterRooms(selectedFloorText, selectedStatus);
            if (result != null)
            {
                
                foreach (var room in result)
                {
                    if (room.Status != "Maintenance")
                    {
                        room.Status = "Available";
                    }
                }
                
                dgRoom.ItemsSource = result;

            }
            else
            {
                LoadRoom();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cboStatus.SelectedItem = null;
            cboFloor.SelectedItem = null;
            LoadRoom();
            
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow mw = new ManagerWindow(acc);
            mw.Show();
            this.Close();
        }
    }
}
