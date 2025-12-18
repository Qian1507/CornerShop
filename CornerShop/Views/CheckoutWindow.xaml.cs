using CornerShop.Core.Data;
using CornerShop.Core.Models;
using CornerShop.ViewModels;
using System.Windows;

namespace CornerShop.Views
{
    /// <summary>
    /// Interaction logic for CheckoutWindow.xaml
    /// </summary>
    public partial class CheckoutWindow : Window
    {
        private CheckoutViewModel viewModel;
        public CheckoutWindow(User user, ShopRepository repo)
        {
            InitializeComponent();
            viewModel=new CheckoutViewModel(user,repo);
            viewModel.OnPaymentSuccess += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
            this.DataContext = viewModel;
        }
        private async void Pay_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.PayAsync();
        }
    }
}
