using BussinessObjects;
using Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WPF_FPT_ManagementHotel
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private readonly Invoice invoice;
        private readonly Account acc;

        public ReportWindow(Invoice inv, Account acc)
        {
            InitializeComponent();
            invoice = inv;
            this.acc = acc;
            Loadreport();
            HideAllGrid();
           
        }

        private void HideAllGrid()
        {
            dgRevenue.Visibility = Visibility.Collapsed;
            dgReservation.Visibility = Visibility.Collapsed;
            dgService.Visibility = Visibility.Collapsed;

            txtRevenue.Visibility = Visibility.Collapsed;
            txtOccupancyStats.Visibility = Visibility.Collapsed;
        }

        private void Loadreport()
        {
            // Month
            cboMonth.ItemsSource = Enumerable.Range(1, 12).ToList();

            // Year
            cboYear.ItemsSource = Enumerable.Range(2020, DateTime.Now.Year - 2020 + 1).ToList();

            cboMonth.SelectedItem = DateTime.Now.Month;
            cboYear.SelectedItem = DateTime.Now.Year;

            cboReport.SelectedIndex = 0;

            txtRevenue.Visibility = Visibility.Collapsed;
            txtOccupancyStats.Visibility = Visibility.Collapsed;
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            HideAllGrid();

            int month = (int)cboMonth.SelectedItem;
            int year = (int)cboYear.SelectedItem;

            string reportType =
                ((ComboBoxItem)cboReport.SelectedItem).Content.ToString();

            if (reportType == "Revenue Report")
            {
                lblKpiTitle.Text = "TOTAL REVENUE";
                InvoiceService invoiceService = new InvoiceService();

                decimal revenue = invoiceService.GetRevenueReport(month, year);

                txtRevenue.Visibility = Visibility.Visible;
                txtRevenue.Text = $"{revenue:N0} VND";

                // BỔ SUNG: Nạp danh sách hóa đơn chi tiết vào bảng bên phải
                using (var context = new ManagementHotelNewContext())
                {
                    var invoicesList = context.Invoices
                        .Include(i => i.Booking)
                            .ThenInclude(b => b.CreatedByNavigation)
                        .Include(i => i.Booking)
                            .ThenInclude(b => b.Room)
                        .Where(i => i.PaymentDate.HasValue &&
                                    i.PaymentDate.Value.Month == month &&
                                    i.PaymentDate.Value.Year == year)
                        .Select(i => new
                        {
                            InvoiceId = i.InvoiceId,
                            BookingId = i.BookingId,
                            RoomNumber = i.Booking.Room.RoomNumber,
                            CustomerName = i.Booking.CreatedByNavigation.FullName,
                            RoomCharge = i.RoomCharge,
                            ServiceCharge = i.ServiceCharge,
                            Discount = i.Discount,
                            TotalAmount = i.TotalAmount,
                            PaymentDate = i.PaymentDate,
                            PaymentMethod = i.PaymentMethod
                        })
                        .ToList();

                    dgRevenue.ItemsSource = null;
                    dgRevenue.ItemsSource = invoicesList;
                }

                dgRevenue.Visibility = Visibility.Visible;
            }
            else if (reportType == "Occupancy Report")
            {
                lblKpiTitle.Text = "OCCUPANCY METRICS";
                IRoomService roomService = new RoomService();
                var rooms = roomService.GetRooOoccupancyReport();

                // BỔ SUNG: Tính toán và hiển thị thống kê phòng trống/bận bên trái
                int totalRooms = rooms.Count;
                int occupiedRooms = rooms.Count(r => r.Status == "Occupied");
                int reservedRooms = rooms.Count(r => r.Status == "Reserved");
                int maintenanceRooms = rooms.Count(r => r.Status == "Maintenance");
                int availableRooms = rooms.Count(r => r.Status == "Available");
                double occupancyRate = totalRooms > 0 ? ((double)(occupiedRooms + reservedRooms) / totalRooms) * 100 : 0;

                txtOccupancyStats.Visibility = Visibility.Visible;
                txtOccupancyStats.Text = $"Total Rooms: {totalRooms}\n" +
                                         $"• Available (Trống): {availableRooms}\n" +
                                         $"• Occupied (Bận): {occupiedRooms}\n" +
                                         $"• Reserved (Đặt trước): {reservedRooms}\n" +
                                         $"• Maintenance: {maintenanceRooms}\n" +
                                         $"• Occupancy Rate: {occupancyRate:F1}%";

                dgReservation.ItemsSource = null;
                dgReservation.ItemsSource = rooms;

                dgReservation.Visibility = Visibility.Visible;
            }
            else if (reportType == "Service Usage Report")
            {
                lblKpiTitle.Text = "SERVICE REVENUE";
                IServiceOrderService serviceOrderService = new ServiceOrderService();
                var serviceUsage = serviceOrderService.GetServiceUsageReport();

                // BỔ SUNG: Hiển thị doanh thu dịch vụ bên trái
                decimal totalServiceRevenue = serviceUsage
                    .Where(s => s.OrderStatus == "Completed")
                    .Sum(s => s.Price);

                txtRevenue.Visibility = Visibility.Visible;
                txtRevenue.Text = $"{totalServiceRevenue:N0} VND";

                dgService.ItemsSource = null;
                dgService.ItemsSource = serviceUsage;

                dgService.Visibility = Visibility.Visible;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow mw = new ManagerWindow(acc);
            mw.Show();
            this.Close();
        }
    }
}