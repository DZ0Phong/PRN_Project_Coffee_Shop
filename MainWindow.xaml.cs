using PRN_Project_Coffee_Shop.Models;
using PRN_Project_Coffee_Shop.Views;
using PRN_Project_Coffee_Shop.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PRN_Project_Coffee_Shop
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        // We will pass the logged-in user to the main window
        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            SetupUIBasedOnRole();
            // Perform inventory check after the window is loaded
            this.Loaded += (s, e) => CheckInventoryWarnings();
        }

        private void SetupUIBasedOnRole()
        {
            // Display user info in the status bar
            UserInfoTextBlock.Text = $"User: {_currentUser.Email} | Role: {_currentUser.Role.RoleName}";

            // Using RoleID for checking. 1 for Admin, 2 for Staff.
            // It's better to check by RoleName for readability, but this requires an Include() in the login query.
            if (_currentUser.Role.RoleName == "Admin")
            {
                MenuManagementButton.Visibility = Visibility.Visible;
                FinancialsButton.Visibility = Visibility.Visible;
                EmployeeManagementButton.Visibility = Visibility.Visible;
            }
        }

        private void OrderManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderManagementPage(_currentUser));
        }

        private void OrderStatusButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderStatusPage());
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CustomerManagementPage());
        }

        private void InventoryManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new InventoryManagementPage());
        }

        private void MenuManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MenuManagementPage());
        }

        private void FinancialsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new FinancialsPage());
        }

        private void EmployeeManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeeManagementPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void CheckInventoryWarnings()
        {
            using (var context = new PrnProjectCoffeeShopContext())
            {
                var lowStockIngredients = context.Ingredients
                    .Where(i => i.QuantityInStock <= i.WarningThreshold)
                    .ToList();

                var expiringIngredients = context.Ingredients
                    .Where(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value <= DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
                    .ToList();

                var warningMessage = new StringBuilder();

                if (lowStockIngredients.Any())
                {
                    warningMessage.AppendLine("Cảnh báo tồn kho thấp:");
                    foreach (var item in lowStockIngredients)
                    {
                        warningMessage.AppendLine($"- {item.IngredientName} (Còn lại: {item.QuantityInStock} {item.Unit})");
                    }
                    warningMessage.AppendLine("Vui lòng bổ sung thêm.");
                    warningMessage.AppendLine();
                }

                if (expiringIngredients.Any())
                {
                    warningMessage.AppendLine("Cảnh báo hạn sử dụng:");
                    foreach (var item in expiringIngredients)
                    {
                        if (item.ExpiryDate < DateOnly.FromDateTime(DateTime.Today))
                        {
                            warningMessage.AppendLine($"- {item.IngredientName} (Đã hết hạn vào ngày {item.ExpiryDate})");
                        }
                        else
                        {
                            warningMessage.AppendLine($"- {item.IngredientName} (Sắp hết hạn vào ngày {item.ExpiryDate})");
                        }
                    }
                    warningMessage.AppendLine("Vui lòng kiểm tra và nhập hàng mới.");
                }

                if (warningMessage.Length > 0)
                {
                    MessageBox.Show(warningMessage.ToString(), "Cảnh báo Kho hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
