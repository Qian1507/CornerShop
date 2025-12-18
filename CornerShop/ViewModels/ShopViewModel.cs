using CornerShop.Core.Data;
using CornerShop.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CornerShop.ViewModels
{
    public class ShopViewModel:INotifyPropertyChanged
    {
        private readonly ShopRepository repo;
        private User currentUser;
            
        public ObservableCollection<CartItem> CartItems {  get; set; }=new ObservableCollection<CartItem>();
        private decimal cartTotal;
        public decimal CartTotal
        {
            get=>cartTotal;
            set
            {
                cartTotal = value; OnPropertyChanged(nameof(CartItem));
            }

        }

        public event Action OnLogoutRequest;
        public event Action<User> OnOpenCartRequest;

        public User CurrentUser
        {
            get => currentUser;
            set { currentUser = value; OnPropertyChanged(nameof(CurrentUser)); }
        }

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            { 
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
        public ShopViewModel(User user)
        {
            CurrentUser = user;
            repo=new ShopRepository(DBConfig.ConnectionString);
            LoadProducts();
        }
        private async void LoadProducts()
        {
            try
            {
                var list = await repo.GetAllProductsAsync();
                Products.Clear();
                foreach (var p in list)
                {
                    Products.Add(p);
                }
                StatusMessage=$"Loaded{Products.Count} products";
            }catch (Exception ex)
            {
                StatusMessage = "Error: " +ex.Message;
            }
        }

        public async Task AddToCartAsync(Product product)
        {
            if(product == null) return;
            try
            {
                await repo.AddToCartAsync(CurrentUser.Id, product, 1);

                var existing = CartItems.FirstOrDefault(c => c.ProductId == product.Id);
                if (existing != null)
                {
                    existing.Quantity += 1;
                }
                else
                {
                    CartItems.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = 1,
                        Category = product.Category,
                    });
                    CartTotal=CartItems.Sum(c=>c.UnitPrice*c.Quantity);
                    StatusMessage = $"Added '{product.Name}' to cart!";
                }
            }
            catch(Exception ex) 
            {
                StatusMessage = "Error: " + ex.Message;
            }
        }
        public void Logout()
        {
            OnLogoutRequest?.Invoke();
        }





        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name ="")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
