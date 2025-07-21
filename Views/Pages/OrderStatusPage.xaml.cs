using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class OrderStatusPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();

        public OrderStatusPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            // Eagerly load related User and Customer data to avoid lazy loading issues.
            OrdersDataGrid.ItemsSource = _context.Orders
                                                 .Include(o => o.User)
                                                 .Include(o => o.Customer)
                                                 .OrderByDescending(o => o.OrderDate)
                                                 .ToList();
        }

        private void UpdateStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                if (StatusComboBox.SelectedItem is ComboBoxItem selectedStatusItem && selectedStatusItem.Content is string newStatus)
                {
                    var orderToUpdate = _context.Orders.Find(selectedOrder.OrderId);
                    if (orderToUpdate != null)
                    {
                        orderToUpdate.Status = newStatus;
                        _context.SaveChanges();
                        
                        // Refresh the DataGrid to show the updated status
                        LoadOrders();
                        MessageBox.Show($"Order {orderToUpdate.OrderId} status updated to {newStatus}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a new status.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select an order to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
