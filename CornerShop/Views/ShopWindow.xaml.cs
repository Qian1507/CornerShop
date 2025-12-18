using CornerShop.Core.Data;
using CornerShop.Core.Models;
using CornerShop.ViewModels;
using CornerShop.Views;
using System.Windows;
using System.Windows.Controls;

namespace CornerShop
{
    /// <summary>
    /// Interaction logic for ShopWindow.xaml
    /// </summary>
    public partial class ShopWindow : Window
    {
        private ShopViewModel _viewModel;
        private ShopRepository _repo;
        public ShopWindow(User user, ShopRepository repo)
        {
            InitializeComponent();
            _viewModel = new ShopViewModel(user);
            DataContext = _viewModel;
            _viewModel.OnLogoutRequest += () =>
            {
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            };
            _repo = repo;
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();

        }
        private void Checkout_Click(object sender,RoutedEventArgs e)
        {
            var user = _viewModel.CurrentUser;
            if (user == null) return;
            CheckoutWindow checkout=new CheckoutWindow(user,_repo);
            bool? result=checkout.ShowDialog();
            if(result==true)
            {
                MainWindow login=new MainWindow();
                login.Show();
                this.Close();
            }
            else
            {

            }
            
        }
        private async void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            string originalText=button.Content.ToString();

            button.IsEnabled= false;
            button.Content = "Adding...";
            try
            {
                var product = button.Tag as Product;
                if (product != null)
                {
                    await _viewModel.AddToCartAsync(product);
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                button.IsEnabled= true;
                button.Content = originalText;
            }

            
        }
    }
}
