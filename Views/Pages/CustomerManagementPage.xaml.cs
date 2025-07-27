using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class CustomerManagementPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();

        public CustomerManagementPage()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            CustomersDataGrid.ItemsSource = _context.Customers.ToList();
        }

        private void SearchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim();
            var searchTerms = searchText.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            string normalizedSearchText = string.Join(" ", searchTerms);

            if (string.IsNullOrWhiteSpace(normalizedSearchText))
            {
                LoadCustomers();
            }
            else
            {
                CustomersDataGrid.ItemsSource = _context.Customers
                                                        .Where(c => c.Email.Contains(normalizedSearchText) || c.CustomerName.Contains(normalizedSearchText))
                                                        .ToList();
            }
        }

        private void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            LoadCustomers();
        }

        private void CustomersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customer selectedCustomer)
            {
                PurchaseHistoryDataGrid.ItemsSource = _context.Orders
                                                              .Where(o => o.CustomerId == selectedCustomer.CustomerId)
                                                              .OrderByDescending(o => o.OrderDate)
                                                              .ToList();
            }
            else
            {
                PurchaseHistoryDataGrid.ItemsSource = null;
            }
        }

        private void CustomersDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Column is DataGridBoundColumn column && column.Binding is System.Windows.Data.Binding binding && binding.Path.Path == "CustomerName")
                {
                    var customer = e.Row.Item as Customer;
                    if (customer != null)
                    {
                        var textBox = e.EditingElement as TextBox;
                        customer.CustomerName = textBox.Text;
                        _context.Update(customer);
                        _context.SaveChanges();
                    }
                }
            }
        }
    }
}
