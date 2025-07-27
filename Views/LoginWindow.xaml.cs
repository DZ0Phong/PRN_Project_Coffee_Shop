using Microsoft.EntityFrameworkCore;
using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PRN_Project_Coffee_Shop.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            // Use the password from the visible control
            string password = (PasswordBox.Visibility == Visibility.Visible) ? PasswordBox.Password : PasswordTextBox.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new PrnProjectCoffeeShopContext())
            {
                // IMPORTANT: In a real-world application, you MUST hash passwords.
                // This is for demonstration purposes only.
                var user = context.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    // This is a placeholder for password verification.
                    // You should replace this with a proper password hashing and verification mechanism.
                    // For now, we assume the scaffolded password hash is the plain text password for testing.
                    if (user.PasswordHash == password) // Replace with password hash check
                    {
                        MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Open the main window and close the login window
                        MainWindow mainWindow = new MainWindow(user); // Pass the logged-in user
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void ShowHideButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                // Show password
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordTextBox.Text = PasswordBox.Password;
                ShowHideButton.Content = "Ẩn";
                PasswordTextBox.Focus();
            }
            else
            {
                // Hide password
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Password = PasswordTextBox.Text;
                ShowHideButton.Content = "Hiện";
                PasswordBox.Focus();
            }
        }
    }
}
