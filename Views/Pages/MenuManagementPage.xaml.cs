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
            if (ProductsDataGrid.SelectedItem is Product product)
            {
                _selectedProduct = product;
                ProductForm.DataContext = _selectedProduct;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedProduct = new Product();
            ProductForm.DataContext = _selectedProduct;
            ProductsDataGrid.SelectedItem = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null) return;

            // Manually update properties from TextBoxes because DataContext binding might not be enough
            _selectedProduct.ProductName = ProductNameTextBox.Text;
            _selectedProduct.Price = decimal.Parse(PriceTextBox.Text); // Add validation
            _selectedProduct.CategoryId = (int)CategoryComboBox.SelectedValue;
            _selectedProduct.ImagePath = ImagePathTextBox.Text;
            _selectedProduct.IsOutOfStock = IsOutOfStockCheckBox.IsChecked ?? false;
            _selectedProduct.Description = DescriptionTextBox.Text;

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
            MessageBox.Show("Product saved successfully.", "Success");
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
