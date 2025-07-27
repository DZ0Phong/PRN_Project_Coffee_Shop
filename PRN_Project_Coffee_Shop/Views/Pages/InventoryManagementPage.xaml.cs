using Microsoft.EntityFrameworkCore;
using PRN_Project_Coffee_Shop.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class InventoryManagementPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();

        public InventoryManagementPage()
        {
            InitializeComponent();
            LoadIngredients();
        }

        private void LoadIngredients()
        {
            IngredientsDataGrid.ItemsSource = _context.Ingredients.ToList();
        }

        private void IngredientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IngredientsDataGrid.SelectedItem is Ingredient selectedIngredient)
            {
                IngredientNameTextBox.Text = selectedIngredient.IngredientName;
                QuantityTextBox.Text = selectedIngredient.QuantityInStock.ToString();
                UnitTextBox.Text = selectedIngredient.Unit;
                ThresholdTextBox.Text = selectedIngredient.WarningThreshold.ToString();
                ExpiryDatePicker.SelectedDate = selectedIngredient.ExpiryDate.HasValue ? new DateTime(selectedIngredient.ExpiryDate.Value.Year, selectedIngredient.ExpiryDate.Value.Month, selectedIngredient.ExpiryDate.Value.Day) : (DateTime?)null;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            IngredientNameTextBox.Clear();
            QuantityTextBox.Clear();
            UnitTextBox.Clear();
            ThresholdTextBox.Clear();
            ExpiryDatePicker.SelectedDate = null;
            IngredientsDataGrid.SelectedItem = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(QuantityTextBox.Text, out decimal quantity) || !decimal.TryParse(ThresholdTextBox.Text, out decimal threshold))
            {
                MessageBox.Show("Please enter valid numbers for quantity and threshold.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (quantity < 0)
            {
                MessageBox.Show("Stock quantity cannot be negative.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IngredientsDataGrid.SelectedItem is Ingredient selectedIngredient)
            {
                // Update existing ingredient
                selectedIngredient.IngredientName = IngredientNameTextBox.Text;
                selectedIngredient.QuantityInStock = quantity;
                selectedIngredient.Unit = UnitTextBox.Text;
                selectedIngredient.WarningThreshold = threshold;
                selectedIngredient.ExpiryDate = ExpiryDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(ExpiryDatePicker.SelectedDate.Value) : (DateOnly?)null;
                _context.Ingredients.Update(selectedIngredient);
            }
            else
            {
                // Add new ingredient
                var newIngredient = new Ingredient
                {
                    IngredientName = IngredientNameTextBox.Text,
                    QuantityInStock = quantity,
                    Unit = UnitTextBox.Text,
                    WarningThreshold = threshold,
                    ExpiryDate = ExpiryDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(ExpiryDatePicker.SelectedDate.Value) : (DateOnly?)null
                };
                _context.Ingredients.Add(newIngredient);
            }
            _context.SaveChanges();
            LoadIngredients();
            CheckAndUpdateProductAvailability();
        }

        private void CheckAndUpdateProductAvailability()
        {
            var products = _context.Products.Include(p => p.ProductIngredients).ThenInclude(pi => pi.Ingredient).ToList();
            foreach (var product in products)
            {
                bool isAvailable = true;
                foreach (var pi in product.ProductIngredients)
                {
                    if (pi.Ingredient.QuantityInStock < pi.QuantityRequired)
                    {
                        isAvailable = false;
                        break;
                    }
                }
                product.IsOutOfStock = !isAvailable;
                _context.Products.Update(product);
            }
            _context.SaveChanges();
        }
    }
}
