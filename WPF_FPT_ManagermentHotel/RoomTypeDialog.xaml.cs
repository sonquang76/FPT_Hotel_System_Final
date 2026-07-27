using BussinessObjects;
using Microsoft.Win32;
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
    /// Interaction logic for RoomTypeDialog.xaml
    /// </summary>
    public partial class RoomTypeDialog : Window
    {
        private Roomtype _roomtype;
        private readonly Account _account;
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypeDialog(Roomtype roomtype, Account account)
        {
            InitializeComponent();
            _roomtype = roomtype;
            _account = account;
            _roomTypeService = new RoomTypeService();

            // Set DataContext để Image tự động binding với trường Url
            this.DataContext = _roomtype;

            LoadData();
        }

        private void LoadData()
        {
            if (_roomtype.RoomTypeId != 0) // Trường hợp Update
            {
                txtDialogTitle.Text = "📝 UPDATE ROOM TYPE";
                txtTypeName.Text = _roomtype.TypeName;
                txtBasePrice.Text = _roomtype.BasePrice.ToString();
                txtCapacity.Text = _roomtype.Capacity.ToString();
                txtUrl.Text = _roomtype.Url;
            }
        }

        // --- SỰ KIỆN NÚT CHỌN ẢNH BROWSE ---
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Room Type Image";
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                txtUrl.Text = selectedFilePath;

                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(selectedFilePath));
                    imgPreview.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot load image preview: " + ex.Message);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gán dữ liệu từ form vào object
                _roomtype.TypeName = txtTypeName.Text;
                _roomtype.BasePrice = decimal.Parse(txtBasePrice.Text);
                _roomtype.Capacity = int.Parse(txtCapacity.Text);

                // Lấy URL từ ô nhập hoặc ảnh vừa chọn
                _roomtype.Url = txtUrl.Text;

                if (_roomtype.RoomTypeId == 0)
                {
                    // Logic Add
                    _roomTypeService.AddRoomType(_roomtype);
                    MessageBox.Show("Room Type added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Logic Update
                    _roomTypeService.UpdateRoomType(_roomtype);
                    MessageBox.Show("Room Type updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Quay lại màn hình quản lý
                RoomTypeManagementWindow window = new RoomTypeManagementWindow(_roomtype, _account);
                window.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving Room Type: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            RoomTypeManagementWindow window = new RoomTypeManagementWindow(_roomtype, _account);
            window.Show();
            this.Close();
        }
    }
}
