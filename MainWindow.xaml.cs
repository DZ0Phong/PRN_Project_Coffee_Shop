using PRN_Project_Coffee_Shop.Models;
using PRN_Project_Coffee_Shop.Views;
using PRN_Project_Coffee_Shop.Views.Pages;
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
    }
}
