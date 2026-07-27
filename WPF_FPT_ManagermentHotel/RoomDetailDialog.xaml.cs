using BussinessObjects;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for RoomDetailDialog.xaml
    /// </summary>
    public partial class RoomDetailDialog : Window
    {
        private readonly Booking booking;
        private readonly Account account;
        public RoomDetailDialog(Account acc, Booking book)
        {
            InitializeComponent();
            this.account = acc;
            this.booking = book;
            this.DataContext = booking; 

            txtRoomNumber.Text = booking.Room.RoomNumber;
            txtFloor.Text = booking.Room.Floor.ToString();
            txtRoomType.Text = book.Room.RoomType.TypeName;
            txtStatus.Text = booking.Room.Status;
            switch (booking.Room.Status.ToString())
            {
                case "Available":
                    bdStatus.Background = Brushes.LightGreen;
                    break;

                case "Booked":
                    bdStatus.Background = Brushes.OrangeRed;
                    break;

                case "Maintenance":
                    bdStatus.Background = Brushes.Gray;
                    break;

                default:
                    bdStatus.Background = Brushes.LightGray;
                    break;
            }
            txtDescription.Text = booking.Room.Description;
            txtPrice.Text = booking.Room.RoomType.BasePrice.ToString();

            // --- ĐOẠN CODE LOAD ẢNH LÊN imgRoom ---
            try
            {
                // Lấy đường dẫn URL từ RoomType trong DB
                string imgUrl = booking.Room?.RoomType?.Url;

                if (!string.IsNullOrEmpty(imgUrl))
                {
                    // Nạp ảnh từ đường dẫn Pack URI hoặc Relative Path
                    imgRoom.Source = new BitmapImage(new Uri(imgUrl, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    // Ảnh mặc định nếu DB bị null/trống
                    imgRoom.Source = new BitmapImage(new Uri("/Image/RoomTypeStandard.jpg", UriKind.Relative));
                }
            }
            catch
            {
                // Nếu đường dẫn bị sai hoặc hỏng file, tự động load ảnh mặc định tránh crash app
                imgRoom.Source = new BitmapImage(new Uri("/Image/RoomTypeStandard.jpg", UriKind.Relative));
            }
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            PaymentOnlineWindow paymentOnlineWindow = new PaymentOnlineWindow(account, booking);

            paymentOnlineWindow.ShowDialog();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(account, booking);
            mainWindow.Show();
            Close();
        }

    }
}
