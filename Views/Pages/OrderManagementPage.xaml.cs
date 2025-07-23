using Microsoft.EntityFrameworkCore;
using PRN_Project_Coffee_Shop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class OrderManagementPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();
        private readonly ObservableCollection<OrderDetail> _currentOrderItems = new ObservableCollection<OrderDetail>();
        private readonly User _currentUser;
        private List<Product> _toppings;

        public OrderManagementPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadInitialData();
            OrderDataGrid.ItemsSource = _currentOrderItems;
        }

        private void LoadInitialData()
        {
            // Load Toppings
            var toppingsCategory = _context.Categories.FirstOrDefault(c => c.CategoryName == "Topping");
            if (toppingsCategory != null)
            {
                _toppings = _context.Products.Where(p => p.CategoryId == toppingsCategory.CategoryId).ToList();
                ToppingsItemsControl.ItemsSource = _toppings;
            }

            // Load Menu
            var menuProducts = _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.CategoryName != "Topping" && !p.IsOutOfStock)
                .ToList();

            var groupedMenu = menuProducts.GroupBy(p => p.Category.CategoryName)
                                          .Select(g => new MenuGroup { CategoryName = g.Key, Products = g.ToList() })
                                          .ToList();
            MenuCategoriesItemsControl.ItemsSource = groupedMenu;

            // Populate Table ComboBox
            for (int i = 1; i <= 11; i++)
            {
                TableComboBox.Items.Add($"Table {i}");
            }
            TableComboBox.Items.Add("Ship");
        }

        private void AddToOrder_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Product productToAdd)
            {
                var newOrderItem = new OrderDetail
                {
                    ProductId = productToAdd.ProductId,
                    Product = productToAdd,
                    Quantity = 1,
                    Price = productToAdd.Price,
                    SugarPercent = 100,
                    IcePercent = 100
                };
                _currentOrderItems.Add(newOrderItem);
                UpdateOrderTotal();
                OrderDataGrid.SelectedItem = newOrderItem;
            }
        }

        private void OrderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderDataGrid.SelectedItem is OrderDetail selectedItem)
            {
                OptionsPanel.Visibility = Visibility.Visible;
                NotesTextBox.Text = selectedItem.Notes;

                bool isDrink = selectedItem.Product?.Category?.CategoryName == "Cà Phê" || selectedItem.Product?.Category?.CategoryName == "Trà";

                if (isDrink)
                {
                    DrinkOptionsPanel.Visibility = Visibility.Visible;
                    ToppingsPanel.Visibility = Visibility.Visible;
                    SugarSlider.Value = selectedItem.SugarPercent;
                    IceSlider.Value = selectedItem.IcePercent;

                    // Defer the checkbox update until the UI is ready
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (var item in ToppingsItemsControl.Items)
                        {
                            var container = ToppingsItemsControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                            if (container != null)
                            {
                                var checkBox = FindVisualChild<CheckBox>(container);
                                if (checkBox != null)
                                {
                                    int toppingId = (int)checkBox.Tag;
                                    checkBox.IsChecked = selectedItem.Toppings.Any(t => t.ProductId == toppingId);
                                }
                            }
                        }
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                }
                else
                {
                    DrinkOptionsPanel.Visibility = Visibility.Collapsed;
                    ToppingsPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                OptionsPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void Option_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OrderDataGrid.SelectedItem is OrderDetail selectedItem)
            {
                selectedItem.SugarPercent = (int)SugarSlider.Value;
                selectedItem.IcePercent = (int)IceSlider.Value;
            }
        }

        private void Topping_Checked(object sender, RoutedEventArgs e)
        {
            if (OrderDataGrid.SelectedItem is OrderDetail selectedItem && sender is CheckBox checkBox)
            {
                int toppingId = (int)checkBox.Tag;
                var topping = _toppings.FirstOrDefault(t => t.ProductId == toppingId);

                if (topping == null) return;

                if (checkBox.IsChecked == true)
                {
                    if (!selectedItem.Toppings.Contains(topping))
                    {
                        if (selectedItem.Toppings.Count < 5)
                        {
                            selectedItem.Toppings.Add(topping);
                        }
                        else
                        {
                            MessageBox.Show("Maximum 5 toppings allowed.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            checkBox.IsChecked = false;
                        }
                    }
                }
                else
                {
                    if (selectedItem.Toppings.Contains(topping))
                    {
                        selectedItem.Toppings.Remove(topping);
                    }
                }
                UpdateOrderTotal();
            }
        }

        private void NotesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (OrderDataGrid.SelectedItem is OrderDetail selectedItem)
            {
                selectedItem.Notes = NotesTextBox.Text;
            }
        }

        private void UpdateOrderTotal()
        {
            decimal total = 0;
            foreach (var item in _currentOrderItems)
            {
                decimal itemTotal = item.Price * item.Quantity;
                decimal toppingsTotal = item.Toppings.Sum(t => t.Price);
                total += itemTotal + (toppingsTotal * item.Quantity);
            }
            TotalAmountTextBlock.Text = $"Total: {total:N0} VND";
        }

        private void ConfirmOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_currentOrderItems.Any())
            {
                MessageBox.Show("Cannot confirm an empty order.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (TableComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a table or 'Ship'.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                decimal totalAmount = _currentOrderItems.Sum(item => (item.Price + item.Toppings.Sum(t => t.Price)) * item.Quantity);

                var newOrder = new Order
                {
                    UserId = _currentUser.UserId,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    Status = "Pending",
                    TableNumber = TableComboBox.SelectedItem.ToString(),
                    IsDelivery = TableComboBox.SelectedItem.ToString() == "Ship",
                    PromotionCode = PromotionCodeTextBox.Text,
                    OrderDetails = new List<OrderDetail>(_currentOrderItems.Select(od => new OrderDetail
                    {
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        SugarPercent = od.SugarPercent,
                        IcePercent = od.IcePercent,
                        Notes = od.Notes,
                        Toppings = new List<Product>(od.Toppings)
                    }))
                };

                if (!string.IsNullOrWhiteSpace(CustomerEmailTextBox.Text))
                {
                    var customer = _context.Customers.FirstOrDefault(c => c.Email == CustomerEmailTextBox.Text);
                    if (customer != null)
                    {
                        newOrder.CustomerId = customer.CustomerId;
                    }
                    else
                    {
                        var newCustomer = new Customer { Email = CustomerEmailTextBox.Text, Points = 0 };
                        _context.Customers.Add(newCustomer);
                        newOrder.Customer = newCustomer;
                    }
                }

                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                MessageBox.Show("Order successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CancelOrderButton_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while confirming the order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            _currentOrderItems.Clear();
            UpdateOrderTotal();
            TableComboBox.SelectedIndex = -1;
            CustomerEmailTextBox.Clear();
            PromotionCodeTextBox.Clear();
            OptionsPanel.Visibility = Visibility.Collapsed;
        }
        
        // Helper to find a child of a specific type in the visual tree.
        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }

    public class MenuGroup
    {
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }
    }
}
