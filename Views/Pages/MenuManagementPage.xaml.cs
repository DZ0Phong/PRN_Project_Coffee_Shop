using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

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

            // Validation
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
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _selectedProduct.ProductName = ProductNameTextBox.Text;
            _selectedProduct.CategoryId = (int)CategoryComboBox.SelectedValue;
            _selectedProduct.Price = price;
            _selectedProduct.IsOutOfStock = IsOutOfStockCheckBox.IsChecked ?? false;

            try
            {
                // Handle Ingredients
                // First, remove existing ingredients that are no longer in the list
                if (_selectedProduct.ProductId != 0)
                {
                    var existingIngredients = _context.ProductIngredients
                                                    .Where(pi => pi.ProductId == _selectedProduct.ProductId)
                                                    .ToList();
                    _context.ProductIngredients.RemoveRange(existingIngredients);
                    _context.SaveChanges();
                }


                // Then, add the current list of ingredients
                _selectedProduct.ProductIngredients.Clear();
                foreach (var pi in _productIngredients)
                {
                    if (pi.IngredientId > 0 && pi.QuantityRequired > 0)
                    {
                        _selectedProduct.ProductIngredients.Add(new ProductIngredient
                        {
                            IngredientId = pi.IngredientId,
                            QuantityRequired = pi.QuantityRequired
                        });
                    }
                }


                if (_selectedProduct.ProductId == 0) // It's a new product
                {
                    _context.Products.Add(_selectedProduct);
                }
                else // It's an existing product
                {
                    _context.Products.Update(_selectedProduct);
                }

                _context.SaveChanges();
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
            if (_selectedProduct != null && _selectedProduct.ProductId != 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Also remove related product ingredients
                    var ingredientsToRemove = _context.ProductIngredients.Where(pi => pi.ProductId == _selectedProduct.ProductId);
                    _context.ProductIngredients.RemoveRange(ingredientsToRemove);

                    _context.Products.Remove(_selectedProduct);
                    _context.SaveChanges();
                    LoadProducts();
                    ClearForm();
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
