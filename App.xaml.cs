using PRN_Project_Coffee_Shop.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PRN_Project_Coffee_Shop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }

}
