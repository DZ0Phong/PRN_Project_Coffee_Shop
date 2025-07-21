using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

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
            LoadUsers();
        }

        private void LoadEmployees()
        {
            EmployeesDataGrid.ItemsSource = _context.Employees.Include(e => e.User).ToList();
        }

        private void LoadUsers()
        {
            // Load users that are not yet assigned to an employee
            var assignedUserIds = _context.Employees.Select(e => e.UserId).ToList();
            UserComboBox.ItemsSource = _context.Users.Where(u => !assignedUserIds.Contains(u.UserId)).ToList();
        }

        private void EmployeesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is Employee employee)
            {
                _selectedEmployee = employee;
                EmployeeForm.DataContext = _selectedEmployee;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedEmployee = new Employee();
            EmployeeForm.DataContext = _selectedEmployee;
            EmployeesDataGrid.SelectedItem = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null) return;

            _selectedEmployee.FullName = FullNameTextBox.Text;
            _selectedEmployee.Position = PositionTextBox.Text;
            _selectedEmployee.Salary = decimal.Parse(SalaryTextBox.Text); // Add validation
            _selectedEmployee.UserId = (int)UserComboBox.SelectedValue;

            if (_selectedEmployee.EmployeeId == 0)
            {
                _context.Employees.Add(_selectedEmployee);
            }
            else
            {
                _context.Employees.Update(_selectedEmployee);
            }

            _context.SaveChanges();
            LoadEmployees();
            LoadUsers(); // Refresh user list
            MessageBox.Show("Employee saved successfully.", "Success");
        }
    }
}
