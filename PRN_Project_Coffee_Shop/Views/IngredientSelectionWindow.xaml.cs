using PRN_Project_Coffee_Shop.Models;
using System.Linq;
using System.Windows;

namespace PRN_Project_Coffee_Shop.Views
{
    public partial class IngredientSelectionWindow : Window
    {
        private readonly PrnProjectCoffeeShopContext _context = new PrnProjectCoffeeShopContext();
        public Ingredient? SelectedIngredient { get; private set; }

        public IngredientSelectionWindow()
        {
            InitializeComponent();
            LoadIngredients();
        }

        private void LoadIngredients()
        {
            IngredientsListView.ItemsSource = _context.Ingredients.ToList();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (IngredientsListView.SelectedItem != null)
            {
                SelectedIngredient = IngredientsListView.SelectedItem as Ingredient;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select an ingredient.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
