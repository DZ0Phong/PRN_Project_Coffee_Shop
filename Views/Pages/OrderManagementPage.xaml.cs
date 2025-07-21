using PRN_Project_Coffee_Shop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class OrderManagementPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();
        private readonly ObservableCollection<OrderDetail> _currentOrderItems = new ObservableCollection<OrderDetail>();
        private readonly User _currentUser;

        public OrderManagementPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadMenu();
            OrderDataGrid.ItemsSource = _currentOrderItems;
        }

        private void LoadMenu()
        {
            MenuListView.ItemsSource = _context.Products.Where(p => !p.IsOutOfStock).ToList();
        }

        // This method will be called from a button inside the ListView's ItemTemplate
        private void AddToOrder_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Product productToAdd)
            {
                var existingItem = _currentOrderItems.FirstOrDefault(item => item.ProductId == productToAdd.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    _currentOrderItems.Add(new OrderDetail
                    {
                        ProductId = productToAdd.ProductId,
                        Product = productToAdd,
                        Quantity = 1,
                        Price = productToAdd.Price
                    });
                }
                UpdateOrderTotal();
                OrderDataGrid.Items.Refresh();
            }
        }

        private void UpdateOrderTotal()
        {
            decimal total = _currentOrderItems.Sum(item => item.TotalPrice);
            TotalAmountTextBlock.Text = $"Total: {total:N0} VND";
        }

        private void ConfirmOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_currentOrderItems.Any())
            {
                MessageBox.Show("Cannot confirm an empty order.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newOrder = new Order
            {
                UserId = _currentUser.UserId,
                OrderDate = DateTime.Now,
                TotalAmount = _currentOrderItems.Sum(item => item.TotalPrice),
                Status = "Pending", // Default status
                TableNumber = TableNumberTextBox.Text,
                IsDelivery = false, // Assuming not a delivery for now
                PromotionCode = PromotionCodeTextBox.Text,
                OrderDetails = new List<OrderDetail>(_currentOrderItems)
            };

            // Handle customer lookup
            if (!string.IsNullOrWhiteSpace(CustomerPhoneTextBox.Text))
            {
                var customer = _context.Customers.FirstOrDefault(c => c.PhoneNumber == CustomerPhoneTextBox.Text);
                if (customer != null)
                {
                    newOrder.CustomerId = customer.CustomerId;
                }
                else
                {
                    // Optional: Create a new customer if they don't exist
                    var newCustomer = new Customer { PhoneNumber = CustomerPhoneTextBox.Text };
                    _context.Customers.Add(newCustomer);
                    // The newOrder will be associated with this new customer upon saving.
                    newOrder.Customer = newCustomer;
                }
            }

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            MessageBox.Show("Order successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Clear the form for the next order
            CancelOrderButton_Click(sender, e);
        }

        private void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            _currentOrderItems.Clear();
            UpdateOrderTotal();
            TableNumberTextBox.Clear();
            CustomerPhoneTextBox.Clear();
            PromotionCodeTextBox.Clear();
        }
    }
}
