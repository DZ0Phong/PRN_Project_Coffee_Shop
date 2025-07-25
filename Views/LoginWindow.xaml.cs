using Microsoft.EntityFrameworkCore;
using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;

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
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
