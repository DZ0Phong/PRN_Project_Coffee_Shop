using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class EmployeeManagementPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();
        private Employee? _selectedEmployee;

        public EmployeeManagementPage()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            EmployeesDataGrid.ItemsSource = _context.Employees.Include(e => e.User).ToList();
        }

        private void EmployeesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is Employee employee)
            {
                _selectedEmployee = employee;
                EmployeeForm.DataContext = _selectedEmployee;
                EmailTextBox.Text = _selectedEmployee.User?.Email;
                PasswordBox.Password = "";
                PasswordBox.IsEnabled = true; // Enable for password reset
            }
            else
            {
                _selectedEmployee = null;
                EmployeeForm.DataContext = null;
                EmailTextBox.Text = "";
                PasswordBox.Password = "";
                PasswordBox.IsEnabled = false;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedEmployee = new Employee();
            EmployeeForm.DataContext = _selectedEmployee;
            EmailTextBox.Text = "";
            PasswordBox.Password = "";
            EmployeesDataGrid.SelectedItem = null;
            PasswordBox.IsEnabled = true;
            FullNameTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PositionTextBox.Text) ||
                string.IsNullOrWhiteSpace(SalaryTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Please fill all fields.", "Warning");
                return;
            }

            if (_selectedEmployee == null)
            {
                _selectedEmployee = new Employee();
            }

            _selectedEmployee.FullName = FullNameTextBox.Text;
            _selectedEmployee.Position = PositionTextBox.Text;
            if (decimal.TryParse(SalaryTextBox.Text, out decimal salary))
            {
                _selectedEmployee.Salary = salary;
            }
            else
            {
                MessageBox.Show("Invalid salary format.", "Error");
                return;
            }

            if (_selectedEmployee.EmployeeId == 0) // New Employee
            {
                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Password is required for new employee.", "Warning");
                    return;
                }

                var newUser = new User
                {
                    Email = EmailTextBox.Text,
                    PasswordHash = HashPassword(PasswordBox.Password),
                    RoleId = 2, // Assuming 2 is the RoleId for Employee
                    IsLocked = false
                };
                _context.Users.Add(newUser);
                _selectedEmployee.User = newUser;
                _context.Employees.Add(_selectedEmployee);
            }
            else // Existing Employee
            {
                var user = _context.Users.Find(_selectedEmployee.UserId);
                if (user != null)
                {
                    user.Email = EmailTextBox.Text;
                }
            }

            _context.SaveChanges();
            LoadEmployees();
            MessageBox.Show("Employee saved successfully.", "Success");
        }

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.CommandParameter is Employee employee)
            {
                var user = _context.Users.Find(employee.UserId);
                if (user != null)
                {
                    user.IsLocked = !user.IsLocked;
                    _context.SaveChanges();
                    LoadEmployees();
                    MessageBox.Show($"User {user.Email} has been {(user.IsLocked ? "locked" : "unlocked")}.", "Success");
                }
            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null || _selectedEmployee.UserId == 0)
            {
                MessageBox.Show("Please select an employee to reset password.", "Warning");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Please enter a new password in the password box.", "Warning");
                return;
            }

            var user = _context.Users.Find(_selectedEmployee.UserId);
            if (user != null)
            {
                user.PasswordHash = HashPassword(PasswordBox.Password);
                _context.SaveChanges();
                PasswordBox.Password = "";
                MessageBox.Show("Password has been reset successfully.", "Success");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
