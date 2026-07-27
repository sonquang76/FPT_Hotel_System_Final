using BussinessObjects;
using Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for ServiceBillWindow.xaml
    /// </summary>
    public partial class ServiceBillWindow : Window
    {
        private readonly Booking booking;
        public ServiceBillWindow(Booking book)
        {
            InitializeComponent();
            this.booking = book;
            LoadService();
        }

        private void LoadService()
        {
            try
            {
                IServiceToService serviceOrderService = new ServiceToService();
                dgServices.ItemsSource = serviceOrderService.GetServices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading services: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddService_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
            {
                return;
            }
            var selected = btn.DataContext as Service;

            // 1. Đọc số lượng từ TextBox txtQuantity trên giao diện
            int quantity = 1;
            DependencyObject parent = VisualTreeHelper.GetParent(btn);
            while (parent != null && !(parent is DataGridRow))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            DataGridRow row = parent as DataGridRow;
            if (row != null)
            {
                TextBox txtQty = FindVisualChild<TextBox>(row, "txtQuantity");
                if (txtQty != null && int.TryParse(txtQty.Text, out int qty))
                {
                    if (qty <= 0)
                    {
                        MessageBox.Show("Quantity must be greater than 0!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    quantity = qty;
                }
            }

            try
            {
                Serviceorder newOrder = new Serviceorder
                {
                    BookingId = this.booking.BookingId,
                    ServiceId = selected.ServiceId,
                    Quantity = quantity, // Sử dụng số lượng đã nhập thực tế
                };

                IServiceOrderService serviceOrderManager = new ServiceOrderService();
                var result = serviceOrderManager.CreateRestaurantOrders(newOrder);

                if (result != null)
                {
                    MessageBox.Show($"Successfully ordered {quantity} '{selected.ServiceName}'!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Order Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Hàm phụ trợ tìm phần tử con trên giao diện WPF để đọc giá trị TextBox
        private T FindVisualChild<T>(DependencyObject obj, string name = null) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && (name == null || ((FrameworkElement)child).Name == name))
                {
                    return (T)child;
                }
                T childOfChild = FindVisualChild<T>(child, name);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }
}