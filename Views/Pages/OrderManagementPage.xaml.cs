using Microsoft.EntityFrameworkCore;
using PRN_Project_Coffee_Shop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        private Promotion _appliedPromotion = null;

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
                // Correct real-time check: count existing items in the cart + the new one.
                int quantityInCart = _currentOrderItems.Count(item => item.ProductId == productToAdd.ProductId);
                if (!CheckIngredientAvailability(productToAdd, quantityInCart + 1))
                {
                    MessageBox.Show($"Sorry, you cannot add more '{productToAdd.ProductName}'. Not enough ingredients in stock.", "Out of Stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Revert to original logic: always add a new item for individual customization.
                var newOrderItem = new OrderDetail
                {
                    ProductId = productToAdd.ProductId,
                    Product = productToAdd,
                    Quantity = 1, // Always 1 for a new line item
                    Price = productToAdd.Price,
                    SugarPercent = 100,
                    IcePercent = 100
                };
                _currentOrderItems.Add(newOrderItem);
                UpdateOrderTotal();
                OrderDataGrid.SelectedItem = newOrderItem;
            }
        }

        private bool CheckIngredientAvailability(Product product, int quantity)
        {
            var reservedStock = GetReservedStock();
            var productIngredients = _context.ProductIngredients
                                             .Include(pi => pi.Ingredient)
                                             .Where(pi => pi.ProductId == product.ProductId)
                                             .ToList();

            foreach (var pi in productIngredients)
            {
                decimal reserved = reservedStock.GetValueOrDefault(pi.IngredientId, 0);
                decimal effectiveStock = pi.Ingredient.QuantityInStock - reserved;

                if (effectiveStock < (pi.QuantityRequired * quantity))
                {
                    return false;
                }
            }
            return true;
        }

        private Dictionary<int, decimal> GetReservedStock()
        {
            var reservedStock = new Dictionary<int, decimal>();
            var pendingOrders = _context.Orders
                                        .Include(o => o.OrderDetails)
                                            .ThenInclude(od => od.Product)
                                                .ThenInclude(p => p.ProductIngredients)
                                        .Include(o => o.OrderDetails)
                                            .ThenInclude(od => od.Toppings)
                                                .ThenInclude(t => t.ProductIngredients)
                                        .Where(o => o.Status == "Pending")
                                        .ToList();

            foreach (var order in pendingOrders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    // Main product ingredients
                    foreach (var pi in detail.Product.ProductIngredients)
                    {
                        if (reservedStock.ContainsKey(pi.IngredientId))
                            reservedStock[pi.IngredientId] += pi.QuantityRequired * detail.Quantity;
                        else
                            reservedStock[pi.IngredientId] = pi.QuantityRequired * detail.Quantity;
                    }

                    // Toppings ingredients
                    foreach (var topping in detail.Toppings)
                    {
                        foreach (var pi in topping.ProductIngredients)
                        {
                            if (reservedStock.ContainsKey(pi.IngredientId))
                                reservedStock[pi.IngredientId] += pi.QuantityRequired * detail.Quantity;
                            else
                                reservedStock[pi.IngredientId] = pi.QuantityRequired * detail.Quantity;
                        }
                    }
                }
            }
            return reservedStock;
        }

        private void OrderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderDataGrid.SelectedItem is OrderDetail selectedItem)
            {
                OptionsPanel.Visibility = Visibility.Visible;

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
                selectedItem.RefreshTotalPrice();
                UpdateOrderTotal();
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

            if (_appliedPromotion != null)
            {
                decimal discountAmount = total * (_appliedPromotion.DiscountPercentage / 100);
                decimal discountedTotal = total - discountAmount;
                TotalAmountTextBlock.Text = $"Total: {discountedTotal:N0} VND (after {_appliedPromotion.DiscountPercentage}% discount)";
            }
            else
            {
                TotalAmountTextBlock.Text = $"Total: {total:N0} VND";
            }
        }

        private void ApplyPromotionButton_Click(object sender, RoutedEventArgs e)
        {
            string code = PromotionCodeTextBox.Text;
            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Please enter a promotion code.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var promotion = _context.Promotions.FirstOrDefault(p => p.PromotionCode == code);

            if (promotion == null)
            {
                MessageBox.Show("Invalid promotion code.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!promotion.IsActive || promotion.IsUsed)
            {
                MessageBox.Show("This promotion is no longer active or has already been used.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DateOnly.FromDateTime(DateTime.Now) < promotion.StartDate || DateOnly.FromDateTime(DateTime.Now) > promotion.EndDate)
            {
                MessageBox.Show("This promotion is not valid at this time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _appliedPromotion = promotion;
            PromotionCodeTextBox.IsEnabled = false;
            ApplyPromotionButton.IsEnabled = false;
            MessageBox.Show($"Promotion '{promotion.Description}' applied successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateOrderTotal();
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

            // Final availability check before confirming
            foreach (var item in _currentOrderItems)
            {
                if (!CheckIngredientAvailability(item.Product, item.Quantity))
                {
                    MessageBox.Show($"Sorry, '{item.Product.ProductName}' has become unavailable in the quantity you requested.", "Stock Changed", MessageBoxButton.OK, MessageBoxImage.Error);
                    item.Product.IsOutOfStock = true;
                    _context.Products.Update(item.Product);
                    _context.SaveChanges();
                    LoadInitialData(); // Refresh menu
                    return;
                }
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    decimal originalTotal = _currentOrderItems.Sum(item => (item.Price + item.Toppings.Sum(t => t.Price)) * item.Quantity);
                    decimal finalTotal = originalTotal;

                    if (_appliedPromotion != null)
                    {
                        finalTotal = originalTotal - (originalTotal * (_appliedPromotion.DiscountPercentage / 100));
                    }

                    var newOrder = new Order
                    {
                        UserId = _currentUser.UserId,
                        OrderDate = DateTime.Now,
                        TotalAmount = finalTotal,
                        Status = "Pending",
                        TableNumber = TableComboBox.SelectedItem.ToString(),
                        IsDelivery = TableComboBox.SelectedItem.ToString() == "Ship",
                        DeliveryNotes = DeliveryNotesTextBox.Text,
                        PromotionCode = _appliedPromotion?.PromotionCode,
                        OrderDetails = new List<OrderDetail>() // Initialize empty
                    };

                    // Create new OrderDetail entities from the view model items
                    foreach (var itemVM in _currentOrderItems)
                    {
                        var newDetail = new OrderDetail
                        {
                            ProductId = itemVM.ProductId,
                            Quantity = itemVM.Quantity,
                            Price = itemVM.Price,
                            SugarPercent = itemVM.SugarPercent,
                            IcePercent = itemVM.IcePercent,
                            Toppings = new List<Product>() // Initialize empty
                        };

                        // Re-fetch topping entities to ensure correct tracking
                        var toppingIds = itemVM.Toppings.Select(t => t.ProductId).ToList();
                        var trackedToppings = _context.Products.Where(p => toppingIds.Contains(p.ProductId)).ToList();
                        foreach (var topping in trackedToppings)
                        {
                            newDetail.Toppings.Add(topping);
                        }
                        newOrder.OrderDetails.Add(newDetail);
                    }


                    if (!string.IsNullOrWhiteSpace(CustomerEmailTextBox.Text))
                    {
                        var customer = _context.Customers.FirstOrDefault(c => c.Email == CustomerEmailTextBox.Text);
                        bool isNewCustomer = customer == null;

                        if (isNewCustomer)
                        {
                            customer = new Customer { Email = CustomerEmailTextBox.Text, CustomerName = "New Customer", Points = 0 };
                            _context.Customers.Add(customer);
                        }
                        
                        newOrder.Customer = customer;

                        int pointsToAdd = 0;
                        foreach (var item in newOrder.OrderDetails)
                        {
                            var product = _context.Products.Include(p => p.Category).First(p => p.ProductId == item.ProductId);
                            if (product.Category.CategoryName == "Cà Phê" || product.Category.CategoryName == "Trà")
                            {
                                pointsToAdd += 5 * item.Quantity; // Reduced from 14
                            }
                            else if (product.Category.CategoryName == "Bánh")
                            {
                                pointsToAdd += 8 * item.Quantity; // Reduced from 23
                            }
                        }
                        customer.Points = (customer.Points ?? 0) + pointsToAdd;
                        _context.Customers.Update(customer);

                        // Save here to get CustomerID if new
                        _context.SaveChanges(); 

                        if (customer.Points >= 100)
                        {
                            int promotionsToCreate = customer.Points.Value / 100;
                            for (int i = 0; i < promotionsToCreate; i++)
                            {
                                customer.Points -= 100;
                                CreateAndSendPromotion(customer);
                            }
                            _context.Customers.Update(customer);
                        }
                    }

                    if (_appliedPromotion != null)
                    {
                        var promotionToUpdate = _context.Promotions.First(p => p.PromotionId == _appliedPromotion.PromotionId);
                        promotionToUpdate.IsUsed = true;
                        promotionToUpdate.IsActive = false;
                        _context.Promotions.Update(promotionToUpdate);
                    }

                    // DO NOT deduct ingredients here. This will be done in OrderStatusPage.
                    _context.Orders.Add(newOrder);
                    _context.SaveChanges();

                    transaction.Commit();

                    // Post-order availability check
                    CheckAndUpdateAllProductAvailability();
                    LoadInitialData(); // Refresh menu

                    MessageBox.Show("Order successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    CancelOrderButton_Click(sender, e);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"An error occurred: {ex.InnerException?.Message ?? ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CreateAndSendPromotion(Customer customer)
        {
            // This check is crucial for new customers
            if (customer.CustomerId == 0)
            {
                 MessageBox.Show("Cannot create promotion for a customer that hasn't been saved to the database yet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                 return;
            }

            var newPromotion = new Promotion
            {
                PromotionCode = $"DISCOUNT20_{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                Description = $"Discount 20% for customer {customer.Email}",
                DiscountPercentage = 20,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                IsActive = true,
                IsUsed = false,
                CustomerId = customer.CustomerId
            };
            _context.Promotions.Add(newPromotion);

            // Send Email
            try
            {
                string fromMail = "assasinhp619@gmail.com";
                string fromPassword = "slos bctt epxv osla";

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail, "The Coffee Shop"); // Set sender display name
                message.Subject = "Your 20% Discount Code from The Coffee Shop!";
                message.To.Add(new MailAddress(customer.Email));

                string customerGreetingName = string.IsNullOrWhiteSpace(customer.CustomerName) ? "Valued Customer" : customer.CustomerName;
                message.Body = $"<html><body><h3>Hello {customerGreetingName},</h3><p>Congratulations! You've earned a 20% discount on your next order. Use the code below:</p><h2>{newPromotion.PromotionCode}</h2><p>This code is valid for 6 months and can be used once.</p><p>Thank you for being a loyal customer!</p><p>Sincerely,<br/>The Coffee Shop Team</p></body></html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.Send(message);
                 MessageBox.Show($"Promotion code sent to {customer.Email}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Log or show a less intrusive error. Maybe the order can still be created.
                MessageBox.Show($"Failed to send promotion email: {ex.Message}", "Email Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            _currentOrderItems.Clear();
            _appliedPromotion = null;
            PromotionCodeTextBox.IsEnabled = true;
            ApplyPromotionButton.IsEnabled = true;
            UpdateOrderTotal();
            TableComboBox.SelectedIndex = -1;
            CustomerEmailTextBox.Clear();
            PromotionCodeTextBox.Clear();
            OptionsPanel.Visibility = Visibility.Collapsed;
        }
        
        private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrderDataGrid.SelectedItem is OrderDetail selectedItem)
            {
                _currentOrderItems.Remove(selectedItem);
                UpdateOrderTotal();
                // Hide options panel if no item is selected anymore
                if (OrderDataGrid.SelectedItem == null)
                {
                    OptionsPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MessageBox.Show("Please select an item to remove.", "No Item Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

        private void CheckAndUpdateAllProductAvailability()
        {
            var allProducts = _context.Products.Include(p => p.ProductIngredients).ThenInclude(pi => pi.Ingredient).ToList();
            foreach (var product in allProducts)
            {
                product.IsOutOfStock = !CheckIngredientAvailability(product, 1);
                _context.Products.Update(product);
            }
            _context.SaveChanges();
        }
    }

    public class MenuGroup
    {
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }
    }
}
