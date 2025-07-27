using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class MenuManagementPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();
        private Product? _selectedProduct;
        private ObservableCollection<ProductIngredient> _productIngredients = new ObservableCollection<ProductIngredient>();

        public MenuManagementPage()
        {
            InitializeComponent();
            LoadProducts();
            LoadCategories();
            IngredientsListView.ItemsSource = _productIngredients;
        }

        private void LoadProducts()
        {
            ProductsDataGrid.ItemsSource = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
                .ToList();
        }

        private void LoadCategories()
        {
            CategoryComboBox.ItemsSource = _context.Categories.ToList();
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ProductsDataGrid.SelectedItem as Product;
            ProductForm.IsEnabled = _selectedProduct != null;

            if (_selectedProduct != null)
            {
                // Load product details into the form
                ProductNameTextBox.Text = _selectedProduct.ProductName;
                CategoryComboBox.SelectedValue = _selectedProduct.CategoryId;
                PriceTextBox.Text = _selectedProduct.Price.ToString("N0");
                IsOutOfStockCheckBox.IsChecked = _selectedProduct.IsOutOfStock;

                // Load ingredients for the selected product
                _productIngredients.Clear();
                if (_selectedProduct.ProductIngredients != null)
                {
                    foreach (var ingredient in _selectedProduct.ProductIngredients)
                    {
                        _productIngredients.Add(ingredient);
                    }
                }
            }
            else
            {
                ClearForm();
            }
        }

        private void ClearForm()
        {
            _selectedProduct = null;
            ProductNameTextBox.Clear();
            CategoryComboBox.SelectedIndex = -1;
            PriceTextBox.Clear();
            IsOutOfStockCheckBox.IsChecked = false;
            _productIngredients.Clear();
            ProductsDataGrid.SelectedItem = null;
            ProductForm.IsEnabled = false;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            _selectedProduct = new Product();
            ProductForm.IsEnabled = true;
            ProductNameTextBox.Focus();
            _productIngredients.Clear();
            // Add two default empty slots for ingredients
            _productIngredients.Add(new ProductIngredient());
            _productIngredients.Add(new ProductIngredient());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("No product selected or created.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- Basic Validation ---
            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
            {
                MessageBox.Show("Product name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CategoryComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid, non-negative price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // --- Ingredient Validation ---
            var validIngredients = _productIngredients
                .Where(pi => pi.IngredientId > 0)
                .ToList();

            if (!validIngredients.Any())
            {
                MessageBox.Show("A product must have at least one ingredient.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var pi in validIngredients)
            {
                if (pi.QuantityRequired <= 0)
                {
                    MessageBox.Show($"Quantity for ingredient '{pi.Ingredient.IngredientName}' must be greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }


            _selectedProduct.ProductName = ProductNameTextBox.Text;
            _selectedProduct.CategoryId = (int)CategoryComboBox.SelectedValue;
            _selectedProduct.Price = price;
            _selectedProduct.IsOutOfStock = IsOutOfStockCheckBox.IsChecked ?? false;

            try
            {
                // --- Handle Ingredients within a single transaction ---
                if (_selectedProduct.ProductId != 0)
                {
                    // Load existing ingredients to be removed
                    var existingIngredients = _context.ProductIngredients
                                                    .Where(pi => pi.ProductId == _selectedProduct.ProductId)
                                                    .ToList();
                    _context.ProductIngredients.RemoveRange(existingIngredients);
                }

                // Clear the navigation property and add the new valid ingredients
                _selectedProduct.ProductIngredients.Clear();
                foreach (var pi in validIngredients)
                {
                    _selectedProduct.ProductIngredients.Add(new ProductIngredient
                    {
                        IngredientId = pi.IngredientId,
                        QuantityRequired = pi.QuantityRequired
                    });
                }

                // --- Add or Update Product ---
                if (_selectedProduct.ProductId == 0) // New product
                {
                    _context.Products.Add(_selectedProduct);
                }
                else // Existing product
                {
                    _context.Products.Update(_selectedProduct);
                }

                _context.SaveChanges(); // All changes are saved here in one transaction

                LoadProducts();
                MessageBox.Show("Product saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
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
            if (_selectedProduct == null || _selectedProduct.ProductId == 0)
            {
                MessageBox.Show("Please select a product to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if the product is used in any ACTIVE orders (Pending or Preparing)
            bool isActiveOrder = _context.OrderDetails
                .Any(od => (od.Order.Status == "Pending" || od.Order.Status == "Preparing") &&
                           (od.ProductId == _selectedProduct.ProductId || od.Toppings.Any(t => t.ProductId == _selectedProduct.ProductId)));

            if (isActiveOrder)
            {
                MessageBox.Show("This product cannot be deleted because it is part of an existing order. Consider marking it as 'Out of Stock' instead.", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // Remove related product ingredients first
                    var ingredientsToRemove = _context.ProductIngredients.Where(pi => pi.ProductId == _selectedProduct.ProductId);
                    if (ingredientsToRemove.Any())
                    {
                        _context.ProductIngredients.RemoveRange(ingredientsToRemove);
                    }

                    // Remove the product itself
                    _context.Products.Remove(_selectedProduct);

                    _context.SaveChanges(); // Save all changes in one transaction

                    LoadProducts();
                    ClearForm();
                    MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (DbUpdateException ex)
                {
                    // Check for a specific foreign key constraint violation (error code 547 for SQL Server)
                    if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                    {
                        MessageBox.Show(
                            "Không thể xóa sản phẩm này vì nó là một phần của các đơn hàng đã tồn tại trong lịch sử.\n\n" +
                            "Để ngừng bán mặt hàng này, vui lòng đánh dấu là 'Hết hàng' (Out of Stock).",
                            "Xóa không thành công",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Đã xảy ra lỗi cơ sở dữ liệu khi xóa: {ex.InnerException?.Message ?? ex.Message}", "Lỗi cơ sở dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null) return;

            var ingredientSelectionWindow = new IngredientSelectionWindow();
            if (ingredientSelectionWindow.ShowDialog() == true)
            {
                var selectedIngredient = ingredientSelectionWindow.SelectedIngredient;
                if (selectedIngredient != null)
                {
                    if (!_productIngredients.Any(pi => pi.IngredientId == selectedIngredient.IngredientId))
                    {
                        _productIngredients.Add(new ProductIngredient
                        {
                            IngredientId = selectedIngredient.IngredientId,
                            Ingredient = selectedIngredient,
                            QuantityRequired = 1 // Default quantity
                        });
                    }
                    else
                    {
                        MessageBox.Show("This ingredient is already in the list.", "Duplicate Ingredient", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void RemoveIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIngredient = IngredientsListView.SelectedItem as ProductIngredient;
            if (selectedIngredient != null)
            {
                _productIngredients.Remove(selectedIngredient);
            }
            else
            {
                MessageBox.Show("Please select an ingredient to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
