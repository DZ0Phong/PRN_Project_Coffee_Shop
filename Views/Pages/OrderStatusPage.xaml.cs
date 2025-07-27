using PRN_Project_Coffee_Shop.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class OrderStatusPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();

        public OrderStatusPage()
        {
            InitializeComponent();
            OrderDatePicker.SelectedDate = DateTime.Today;
            OrderDatePicker.DisplayDateEnd = DateTime.Today;
        }

        private void LoadOrders(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue) return;

            var orders = _context.Orders
                                 .Include(o => o.User)
                                 .Include(o => o.Customer)
                                 .Include(o => o.OrderDetails)
                                     .ThenInclude(od => od.Product)
                                         .ThenInclude(p => p.Category)
                                 .Include(o => o.OrderDetails)
                                     .ThenInclude(od => od.Toppings)
                                 .Where(o => o.OrderDate.Date == selectedDate.Value.Date)
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToList();
            OrdersDataGrid.ItemsSource = orders;
        }

        private void OrderDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadOrders(OrderDatePicker.SelectedDate);
            OrderDetailsDataGrid.ItemsSource = null;
            CustomerNoteTextBlock.Text = "N/A";
            OrderDetailsNote.Text = "Select an order to see details";
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                OrderDetailsDataGrid.ItemsSource = selectedOrder.OrderDetails;
                CustomerNoteTextBlock.Text = string.IsNullOrWhiteSpace(selectedOrder.DeliveryNotes) ? "N/A" : selectedOrder.DeliveryNotes;
                OrderDetailsNote.Text = $"Details for Order #{selectedOrder.OrderId}";
                UpdateStatusControls(selectedOrder);
            }
            else
            {
                UpdateStatusControls(null);
            }
        }

        private void UpdateStatusControls(Order? order)
        {
            // Default state: all disabled
            PreparingRadioButton.IsEnabled = false;
            CompletedRadioButton.IsEnabled = false;
            CancelledRadioButton.IsEnabled = false;
            UpdateStatusButton.IsEnabled = false;
            CurrentStatusTextBlock.Text = "N/A";

            // Uncheck all radio buttons
            PreparingRadioButton.IsChecked = false;
            CompletedRadioButton.IsChecked = false;
            CancelledRadioButton.IsChecked = false;

            if (order == null) return;

            CurrentStatusTextBlock.Text = order.Status;

            switch (order.Status)
            {
                case "Pending":
                    PreparingRadioButton.IsEnabled = true;
                    // Ship orders can be cancelled while pending
                    if (order.IsDelivery)
                    {
                        CancelledRadioButton.IsEnabled = true;
                    }
                    UpdateStatusButton.IsEnabled = true;
                    break;

                case "Preparing":
                    CompletedRadioButton.IsEnabled = true;
                    UpdateStatusButton.IsEnabled = true;
                    break;

                // If Completed or Cancelled, all controls remain disabled
                case "Completed":
                case "Cancelled":
                default:
                    break;
            }
        }


        private void UpdateStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is not Order selectedOrder)
            {
                MessageBox.Show("Please select an order to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string? newStatus = null;
            if (PreparingRadioButton.IsChecked == true) newStatus = "Preparing";
            else if (CompletedRadioButton.IsChecked == true) newStatus = "Completed";
            else if (CancelledRadioButton.IsChecked == true) newStatus = "Cancelled";

            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Please select a new status.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var orderToUpdate = _context.Orders
                                        .Include(o => o.OrderDetails).ThenInclude(od => od.Product).ThenInclude(p => p.ProductIngredients).ThenInclude(pi => pi.Ingredient)
                                        .Include(o => o.OrderDetails).ThenInclude(od => od.Toppings)
                                        .FirstOrDefault(o => o.OrderId == selectedOrder.OrderId);

            if (orderToUpdate == null) return;

            // Handle inventory check and deduction when moving from Pending to Preparing
            if (orderToUpdate.Status == "Pending" && newStatus == "Preparing")
            {
                string? inventoryCheckError = CheckInventory(orderToUpdate);
                if (inventoryCheckError != null)
                {
                    MessageBox.Show(inventoryCheckError, "Inventory Check Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Stop the update process
                }

                DeductInventory(orderToUpdate);
            }
            // Handle inventory restoration if an order is cancelled
            else if (newStatus == "Cancelled" && orderToUpdate.Status != "Cancelled")
            {
                // Optional: Restore inventory if an order is cancelled.
                // This might need more complex logic depending on business rules
                // (e.g., only restore if it was in "Preparing" state).
                // For now, we'll assume cancellation from "Pending" doesn't need restoration
                // as inventory wasn't deducted yet.
            }


            orderToUpdate.Status = newStatus;
            _context.SaveChanges();

            // Explicitly update the controls to reflect the new state immediately
            UpdateStatusControls(orderToUpdate);
            
            LoadOrders(OrderDatePicker.SelectedDate);
            
            // Reselect the item in the grid to maintain context
            var gridItemsSource = OrdersDataGrid.ItemsSource as List<Order>;
            if (gridItemsSource != null)
            {
                OrdersDataGrid.SelectedItem = gridItemsSource.FirstOrDefault(o => o.OrderId == orderToUpdate.OrderId);
            }

            MessageBox.Show($"Order {orderToUpdate.OrderId} status updated to {newStatus}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string? CheckInventory(Order order)
        {
            var requiredIngredients = GetRequiredIngredientsForOrder(order);
            var errorMessages = new List<string>();
            var today = DateOnly.FromDateTime(DateTime.Today);

            foreach (var required in requiredIngredients)
            {
                var ingredientInStock = _context.Ingredients.Find(required.Key);
                string ingredientName = ingredientInStock?.IngredientName ?? $"ID {required.Key}";

                // Check for sufficient quantity
                if (ingredientInStock == null || ingredientInStock.QuantityInStock < required.Value)
                {
                    errorMessages.Add($"- Không đủ '{ingredientName}'. Yêu cầu: {required.Value}, Tồn kho: {ingredientInStock?.QuantityInStock ?? 0}.");
                }

                // Check for expiry date
                if (ingredientInStock?.ExpiryDate.HasValue == true && ingredientInStock.ExpiryDate.Value < today)
                {
                    errorMessages.Add($"- Nguyên liệu '{ingredientName}' đã hết hạn vào ngày {ingredientInStock.ExpiryDate.Value}.");
                }
            }

            return errorMessages.Any() ? "Không thể cập nhật trạng thái. Vấn đề kho hàng:\n" + string.Join("\n", errorMessages) : null;
        }

        private void DeductInventory(Order order)
        {
            var itemsToDeduct = GetRequiredIngredientsForOrder(order);

            // Perform the deduction
            foreach (var item in itemsToDeduct)
            {
                var ingredient = _context.Ingredients.Find(item.Key);
                if (ingredient != null)
                {
                    ingredient.QuantityInStock -= item.Value;
                }
            }
        }

        private Dictionary<int, decimal> GetRequiredIngredientsForOrder(Order order)
        {
            var requiredItems = new Dictionary<int, decimal>();

            // Consolidate all required ingredients from the order
            foreach (var detail in order.OrderDetails)
            {
                // Main product ingredients
                foreach (var pi in detail.Product.ProductIngredients)
                {
                    if (requiredItems.ContainsKey(pi.IngredientId))
                        requiredItems[pi.IngredientId] += pi.QuantityRequired * detail.Quantity;
                    else
                        requiredItems[pi.IngredientId] = pi.QuantityRequired * detail.Quantity;
                }

                // Toppings ingredients
                foreach (var topping in detail.Toppings)
                {
                    // The product model for toppings should already be loaded with its ingredients
                    foreach (var pi in topping.ProductIngredients)
                    {
                        if (requiredItems.ContainsKey(pi.IngredientId))
                            requiredItems[pi.IngredientId] += pi.QuantityRequired * detail.Quantity; // Assuming topping quantity is tied to main item quantity
                        else
                            requiredItems[pi.IngredientId] = pi.QuantityRequired * detail.Quantity;
                    }
                }
            }
            return requiredItems;
        }
    }
}
