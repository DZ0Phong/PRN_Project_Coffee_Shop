using PRN_Project_Coffee_Shop.Models;
using System;
using System.Linq;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class FinancialsPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();

        public FinancialsPage()
        {
            InitializeComponent();
            ReportDatePicker.SelectedDate = DateTime.Today;
        }

        private void ReportDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportDatePicker.SelectedDate.HasValue)
            {
                LoadReportForDate(ReportDatePicker.SelectedDate.Value);
            }
        }

        private void LoadReportForDate(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var ordersForDay = _context.Orders
                                       .Include(o => o.User)
                                       .Where(o => o.Status == "Completed" && o.OrderDate >= startDate && o.OrderDate < endDate)
                                       .ToList();

            RevenueDataGrid.ItemsSource = ordersForDay;

            decimal totalRevenue = ordersForDay.Sum(o => o.TotalAmount);
            TotalRevenueTextBlock.Text = $"Total Revenue for {date:d}: {totalRevenue:N0} VND";
        }
    }
}
