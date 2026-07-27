using BussinessObjects;
using DataAccessLayer; // Thêm đúng namespace chứa lớp ServiceOrderDao của bạn
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF_FPT_ManagementHotel
{
    public partial class ServiceWindow : Window
    {
        private readonly Booking booking;
        private List<Serviceorder> _allServiceOrders = new List<Serviceorder>();
        private readonly Account account;
        public ServiceWindow(Account acc, Booking book)
        {
            InitializeComponent();
            this.booking = book;
            this.account = acc;
            this.DataContext = booking;
            dpFilterDate.SelectedDate = DateTime.Now;

            LoadServiceData();
        }

        private void LoadServiceData()
        {
            try
            {
                _allServiceOrders = ServiceOrderDao.GetServiceUsageReport();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            var filteredList = _allServiceOrders.AsEnumerable();

            string keyword = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(keyword))
            {
                filteredList = filteredList.Where(so =>
                    (so.Booking?.Room?.RoomNumber != null && so.Booking.Room.RoomNumber.ToLower().Contains(keyword)) ||
                    (so.Service?.ServiceName != null && so.Service.ServiceName.ToLower().Contains(keyword))
                );
            }

            if (chkFilterByDate.IsChecked == true && dpFilterDate.SelectedDate.HasValue)
            {
                DateTime selectedDate = dpFilterDate.SelectedDate.Value.Date;
                filteredList = filteredList.Where(so => so.OrderTime.HasValue && so.OrderTime.Value.Date == selectedDate);
            }

            dgServiceHistory.ItemsSource = filteredList.ToList();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int orderId)
            {
                try
                {
                    ServiceOrderDao.ConfirmServiceOrder(orderId);
                    MessageBox.Show("Service request confirmed successfully!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadServiceData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Execution error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int orderId)
            {
                if (MessageBox.Show("Are you sure you want to cancel this service order?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        ServiceOrderDao.CancelServiceOrder(orderId);
                        MessageBox.Show("Service order cancelled!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadServiceData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Execution error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int orderId)
            {
                try
                {
                    ServiceOrderDao.CompleteServiceOrder(orderId);
                    MessageBox.Show("Service request completed!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadServiceData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Execution error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }
        private void ChkFilterByDate_Changed(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }
        private void DpFilterDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadServiceData();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e) { }

        // SỬA ĐỔI: Tìm và hiện lại cửa sổ ReceptionistWindow cũ đang ẩn
        private void BtnReservations_Click(object sender, RoutedEventArgs e)
        {
            var receptionistWindow = Application.Current.Windows.OfType<ReceptionistWindow>().FirstOrDefault();
            if (receptionistWindow != null)
            {
                receptionistWindow.Show();
            }
            else
            {
                receptionistWindow = new ReceptionistWindow(account, booking);
                receptionistWindow.Show();
            }
            Close();
        }

        private void BtnRoomManagement_Click(object sender, RoutedEventArgs e)
        {
            RoomReservationWindow roomReservationWindow = new RoomReservationWindow(account, booking);
            roomReservationWindow.Show();
            Close();
        }

        // BỔ SUNG: Hiện lại cửa sổ ReceptionistWindow khi người dùng đóng bằng dấu X góc phải
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            var receptionistWindow = Application.Current.Windows.OfType<ReceptionistWindow>().FirstOrDefault();
            if (receptionistWindow != null)
            {
                receptionistWindow.Show();
            }
        }
    }
}