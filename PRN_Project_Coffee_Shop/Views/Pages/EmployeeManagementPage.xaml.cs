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
                PasswordBox.Password = "";
                PasswordBox.IsEnabled = true; // Enable for password reset
            }
            else
            {
                // Keep the form clear if no employee is selected
                _selectedEmployee = null;
                EmployeeForm.DataContext = null;
                PasswordBox.Password = "";
                PasswordBox.IsEnabled = false;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedEmployee = new Employee
            {
                User = new User() // Initialize the nested User object for data binding
            };
            EmployeeForm.DataContext = _selectedEmployee;
            PasswordBox.Password = "";
            EmployeesDataGrid.SelectedItem = null;
            PasswordBox.IsEnabled = true;
            FullNameTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null || EmployeeForm.DataContext == null)
            {
                MessageBox.Show("No employee data to save.", "Information");
                return;
            }

            // Validate the data context directly
            if (string.IsNullOrWhiteSpace(_selectedEmployee.FullName) ||
                string.IsNullOrWhiteSpace(_selectedEmployee.Position) ||
                _selectedEmployee.Salary <= 0 ||
                _selectedEmployee.User == null || string.IsNullOrWhiteSpace(_selectedEmployee.User.Email))
            {
                MessageBox.Show("Please fill all fields correctly.", "Warning");
                return;
            }

            if (_selectedEmployee.EmployeeId == 0) // New Employee
            {
                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Password is required for new employee.", "Warning");
                    return;
                }

                // The User object is already part of _selectedEmployee due to binding
                _selectedEmployee.User.PasswordHash = HashPassword(PasswordBox.Password);
                _selectedEmployee.User.RoleId = 2; // Employee Role
                _selectedEmployee.User.IsLocked = false;
                
                _context.Employees.Add(_selectedEmployee);
            }
            else // Existing Employee
            {
                // The context is already tracking the _selectedEmployee and its related User.
                // Changes made in the UI are already updated in the tracked entities.
                _context.Employees.Update(_selectedEmployee);
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
