using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class MenuManagementPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();
        private Product? _selectedProduct;

        public MenuManagementPage()
        {
            InitializeComponent();
            LoadProducts();
            LoadCategories();
        }

        private void LoadProducts()
        {
            ProductsDataGrid.ItemsSource = _context.Products.Include(p => p.Category).ToList();
        }

        private void LoadCategories()
        {
            CategoryComboBox.ItemsSource = _context.Categories.ToList();
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ProductsDataGrid.SelectedItem as Product;
            // The DataContext is now bound in XAML, but we can disable the form if nothing is selected
            ProductForm.IsEnabled = _selectedProduct != null;
            if (_selectedProduct == null)
            {
                // Clear the form by setting DataContext to null if selection is cleared
                ProductForm.DataContext = null;
            }
            else
            {
                ProductForm.DataContext = _selectedProduct;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedProduct = new Product { Price = 0 }; // Create a new product instance
            ProductForm.DataContext = _selectedProduct;
            ProductsDataGrid.SelectedItem = null; // Deselect the grid
            ProductForm.IsEnabled = true;
            ProductNameTextBox.Focus(); // Set focus to the first field
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("No product selected or created.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation
            if (string.IsNullOrWhiteSpace(_selectedProduct.ProductName))
            {
                MessageBox.Show("Product name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_selectedProduct.CategoryId == 0)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Price is already bound, no need for manual parsing. The binding will fail for non-decimal values.

            try
            {
                if (_selectedProduct.ProductId == 0) // It's a new product
                {
                    _context.Products.Add(_selectedProduct);
                }
                else // It's an existing product
                {
                    // The context is already tracking the selected product, so changes will be saved.
                    _context.Products.Update(_selectedProduct);
                }

                _context.SaveChanges();
                LoadProducts();
                MessageBox.Show("Product saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ProductForm.IsEnabled = false;
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"An error occurred while saving: {ex.InnerException?.Message ?? ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct != null && _selectedProduct.ProductId != 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Products.Remove(_selectedProduct);
                    _context.SaveChanges();
                    LoadProducts();
                    NewButton_Click(sender, e); // Clear the form
                }
            }
        }
    }
}
