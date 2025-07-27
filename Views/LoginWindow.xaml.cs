using Microsoft.EntityFrameworkCore;
using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = (PasswordBox.Visibility == Visibility.Visible) ? PasswordBox.Password : PasswordTextBox.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new PrnProjectCoffeeShopContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    if (user.IsLocked)
                    {
                        MessageBox.Show("This account has been locked.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string hashedPassword = HashPassword(password);
                    if (user.PasswordHash == hashedPassword)
                    {
                        MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        MainWindow mainWindow = new MainWindow(user);
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
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordTextBox.Text = PasswordBox.Password;
                ShowHideButton.Content = "Ẩn";
                PasswordTextBox.Focus();
            }
            else
            {
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Password = PasswordTextBox.Text;
                ShowHideButton.Content = "Hiện";
                PasswordBox.Focus();
            }
        }
    }
}
