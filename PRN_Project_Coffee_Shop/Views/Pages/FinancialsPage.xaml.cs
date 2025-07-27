using PRN_Project_Coffee_Shop.Models;
using System;
using System.Linq;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class FinancialsPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();

        public FinancialsPage()
        {
            InitializeComponent();
            this.Loaded += FinancialsPage_Loaded;
        }

        private void FinancialsPage_Loaded(object sender, RoutedEventArgs e)
        {
            ReportDatePicker.SelectedDate = DateTime.Today;
            LoadDashboardData(DateTime.Today);
        }

        private void ReportDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportDatePicker.SelectedDate.HasValue)
            {
                LoadDashboardData(ReportDatePicker.SelectedDate.Value);
            }
        }

        private void LoadDashboardData(DateTime selectedDate)
        {
            // Date ranges
            var todayStart = selectedDate.Date;
            var todayEnd = todayStart.AddDays(1);
            var monthStart = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            // --- Daily Stats for Selected Date ---
            var ordersForSelectedDay = _context.Orders
                                               .Where(o => o.OrderDate >= todayStart && o.OrderDate < todayEnd)
                                               .ToList();
            
            decimal todaysRevenue = ordersForSelectedDay
                                    .Where(o => o.Status == "Completed")
                                    .Sum(o => o.TotalAmount);

            int dineInOrders = ordersForSelectedDay.Count(o => !o.IsDelivery);
            int takeAwayOrders = ordersForSelectedDay.Count(o => o.IsDelivery);
            int completedOrders = ordersForSelectedDay.Count(o => o.Status == "Completed");
            int cancelledOrders = ordersForSelectedDay.Count(o => o.Status == "Cancelled");

            // --- Monthly Stats ---
            var monthlyCompletedOrders = _context.Orders
                                                 .Where(o => o.OrderDate >= monthStart && o.OrderDate < monthEnd && o.Status == "Completed")
                                                 .ToList();
            decimal totalMonthlyRevenue = monthlyCompletedOrders.Sum(o => o.TotalAmount);
            
            // Calculate salaries only for active (not locked) employees
            decimal totalEmployeeSalaries = _context.Employees
                                                    .Include(e => e.User)
                                                    .Where(e => e.User != null && !e.User.IsLocked)
                                                    .Sum(emp => emp.Salary);
                                                    
            decimal monthlyNetRevenue = totalMonthlyRevenue - totalEmployeeSalaries;

            // --- Most Popular Item for Selected Date ---
            var mostPopularItem = _context.OrderDetails
                                          .Where(od => od.Order.OrderDate >= todayStart && od.Order.OrderDate < todayEnd)
                                          .GroupBy(od => od.Product.ProductName)
                                          .Select(g => new { ProductName = g.Key, Quantity = g.Sum(od => od.Quantity) })
                                          .OrderByDescending(g => g.Quantity)
                                          .FirstOrDefault();

            // --- Update UI ---
            TodaysRevenueTextBlock.Text = $"{todaysRevenue:N0} VND";
            MonthlyNetRevenueTextBlock.Text = $"{monthlyNetRevenue:N0} VND";
            TotalSalariesTextBlock.Text = $"{totalEmployeeSalaries:N0} VND";
            MostPopularItemTextBlock.Text = mostPopularItem?.ProductName ?? "N/A";

            DineInOrdersTextBlock.Text = dineInOrders.ToString();
            TakeAwayOrdersTextBlock.Text = takeAwayOrders.ToString();
            CompletedOrdersTextBlock.Text = completedOrders.ToString();
            CancelledOrdersTextBlock.Text = cancelledOrders.ToString();
        }
    }
}
