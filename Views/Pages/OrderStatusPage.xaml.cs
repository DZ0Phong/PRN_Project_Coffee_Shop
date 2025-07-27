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

            // Deduct inventory only when moving from Pending to Preparing
            if (orderToUpdate.Status == "Pending" && newStatus == "Preparing")
            {
                DeductInventory(orderToUpdate);
            }

            orderToUpdate.Status = newStatus;
            _context.SaveChanges();

            LoadOrders(OrderDatePicker.SelectedDate);
            MessageBox.Show($"Order {orderToUpdate.OrderId} status updated to {newStatus}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeductInventory(Order order)
        {
            foreach (var detail in order.OrderDetails)
            {
                // Deduct ingredients for the main product
                foreach (var productIngredient in detail.Product.ProductIngredients)
                {
                    var ingredient = _context.Ingredients.Find(productIngredient.IngredientId);
                    if (ingredient != null)
                    {
                        ingredient.QuantityInStock -= (decimal)(productIngredient.QuantityRequired * detail.Quantity);
                    }
                }

                // Deduct ingredients for toppings
                foreach (var topping in detail.Toppings)
                {
                    var ingredient = _context.Ingredients.Find(topping.ProductId); // Assuming topping is a product with inventory
                    if (ingredient != null)
                    {
                        ingredient.QuantityInStock -= detail.Quantity; // Assuming 1 topping unit per drink
                    }
                }
            }
        }
    }
}
