using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PRN_Project_Coffee_Shop.Views.Pages
{
    public partial class InventoryManagementPage : Page
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();
        private Ingredient? _selectedIngredient;

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
            if (IngredientsDataGrid.SelectedItem is Ingredient ingredient)
            {
                _selectedIngredient = ingredient;
                IngredientForm.DataContext = _selectedIngredient;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedIngredient = new Ingredient();
            IngredientForm.DataContext = _selectedIngredient;
            IngredientsDataGrid.SelectedItem = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedIngredient == null) return;

            // Add validation for parsing
            _selectedIngredient.IngredientName = IngredientNameTextBox.Text;
            _selectedIngredient.QuantityInStock = decimal.Parse(QuantityTextBox.Text);
            _selectedIngredient.Unit = UnitTextBox.Text;
            _selectedIngredient.WarningThreshold = decimal.Parse(ThresholdTextBox.Text);
            _selectedIngredient.ExpiryDate = ExpiryDatePicker.SelectedDate;

            if (_selectedIngredient.IngredientId == 0)
            {
                _context.Ingredients.Add(_selectedIngredient);
            }
            else
            {
                _context.Ingredients.Update(_selectedIngredient);
            }

            _context.SaveChanges();
            LoadIngredients();
            MessageBox.Show("Ingredient saved successfully.", "Success");
        }
    }
}
