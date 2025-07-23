using PRN_Project_Coffee_Shop.Models;
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
            if (IngredientsDataGrid.SelectedItem is Ingredient selectedIngredient)
            {
                // Update existing ingredient
                selectedIngredient.IngredientName = IngredientNameTextBox.Text;
                selectedIngredient.QuantityInStock = decimal.Parse(QuantityTextBox.Text);
                selectedIngredient.Unit = UnitTextBox.Text;
                selectedIngredient.WarningThreshold = decimal.Parse(ThresholdTextBox.Text);
                selectedIngredient.ExpiryDate = ExpiryDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(ExpiryDatePicker.SelectedDate.Value) : (DateOnly?)null;
                _context.Ingredients.Update(selectedIngredient);
            }
            else
            {
                // Add new ingredient
                var newIngredient = new Ingredient
                {
                    IngredientName = IngredientNameTextBox.Text,
                    QuantityInStock = decimal.Parse(QuantityTextBox.Text),
                    Unit = UnitTextBox.Text,
                    WarningThreshold = decimal.Parse(ThresholdTextBox.Text),
                    ExpiryDate = ExpiryDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(ExpiryDatePicker.SelectedDate.Value) : (DateOnly?)null
                };
                _context.Ingredients.Add(newIngredient);
            }
            _context.SaveChanges();
            LoadIngredients();
        }
    }
}
