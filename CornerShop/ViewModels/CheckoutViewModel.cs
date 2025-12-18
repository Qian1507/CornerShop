using CornerShop.Core.Data;
using CornerShop.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CornerShop.ViewModels
{
    public class CheckoutViewModel:INotifyPropertyChanged
    {
        private readonly ShopRepository repo;
        private readonly User user;
        public ObservableCollection<CartItem> CartItems { get; set; }
        public IEnumerable<Currency> Currencies { get; }=Enum.GetValues(typeof(Currency)).Cast<Currency>();

        private Currency selectedCurrency = Currency.SEK;
      
        public Currency SelectedCurrency
        {
            get => selectedCurrency;
            set
            {
                selectedCurrency = value;
                OnPropertyChanged(nameof(SelectedCurrency));
                UpdateTotalDisplay();
            }
        }
        private string displayTotal;
        public string DisplayTotal
        {
            get=>displayTotal;
            set { displayTotal=value; OnPropertyChanged(nameof(DisplayTotal));}
        }
        private string message;
        public string Message
        {
            get => message;
            set {message=value; OnPropertyChanged(nameof(Message));}
        }
        public event Action OnPaymentSuccess;

        private string _originalPriceDisplay;
        public string OriginalPriceDisplay
        {
            get => _originalPriceDisplay;
            set { _originalPriceDisplay = value; OnPropertyChanged(nameof(OriginalPriceDisplay)); }
        }
        private string _discountDisplay;
        public string DiscountDisplay
        {
            get => _discountDisplay;
            set { _discountDisplay = value; OnPropertyChanged(nameof(DiscountDisplay)); }
        }
        private bool _hasDiscount;
        public bool HasDiscount
        {
            get => _hasDiscount;
            set { _hasDiscount = value; OnPropertyChanged(nameof(HasDiscount)); }
        }





        public CheckoutViewModel(User user, ShopRepository repo)
        {
            this.user = user;
            this.repo = repo;
            CartItems = new ObservableCollection<CartItem>();
            LoadCart();
        }
        public async void LoadCart()
        {
            CartItems.Clear();
            if(user.Cart!=null)
            {
                foreach(var item in user.Cart)
                {
                    CartItems.Add(item);
                }
                UpdateTotalDisplay();
            }
        }
        private void UpdateTotalDisplay()
        {
            decimal totalSEK = CartItems.Sum(x => x.UnitPrice * x.Quantity);
            decimal rate = 1.0m;          
            string symbol = "Kr";

            switch(SelectedCurrency)
            {
                case Currency.USD:
                    rate = 0.095m;
                    symbol = "$";
                    break;
                case Currency.EUR:
                    rate= 0.088m;
                    symbol = "€";
                    break;
                default:
                    rate=1.0m;
                    symbol = "Kr";
                    break;
            }
            decimal discountRate = 0m;
            switch (user.Level)
            {
                case UserLevel.Gold: discountRate=0.15m; break;
                case UserLevel.Silver:discountRate=0.10m; break;
                case UserLevel.Bronze:discountRate = 0.05m; break;
                default :              discountRate = 0.00m; break;
            }
            decimal originalPrice=totalSEK*rate;
            decimal discountAmount=originalPrice*discountRate;
            decimal finalPrice=originalPrice- discountAmount;

            DisplayTotal = $"{symbol}{finalPrice:N2}";
            if (discountRate > 0)
            {
                HasDiscount = true;
                OriginalPriceDisplay = $"{symbol} {originalPrice:N2}";
                DiscountDisplay = $"{user.Level} (-{discountRate:P0}): -{symbol}{discountAmount:N2}";
            }
            else
            {
                HasDiscount = false;
                OriginalPriceDisplay = "";
                DiscountDisplay = "";
            }



        }
        public async Task PayAsync()
        {
            try
            {
                if (CartItems.Count == 0)
                {
                    Message = "Cart is empty!";
                    return;
                }
                var oldLevel=user.Level;
                decimal finalPaidSEK = await repo.CheckoutAsync(user.Id);
                var updatedUser=await repo.GetUserByIdAsync(user.Id);
                if(updatedUser != null)
                {
                    user.Level = updatedUser.Level;
                    user.Cart=updatedUser.Cart;
                }
                CartItems.Clear();
                UpdateTotalDisplay();

                string msg = $"Payment Successful!\nPaid: {finalPaidSEK:N2} SEK";

                if (updatedUser.Level > oldLevel)
                {
                    msg += $"\n\n🎉 CONGRATULATIONS! 🎉\nYou have been upgraded to {updatedUser.Level} Member!";
                }
                else
                {
                    msg += "\nHope to see you again soon!";
                }

                System.Windows.MessageBox.Show(msg, "Receipt", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                OnPaymentSuccess?. Invoke();
            }catch (Exception ex)
            {
                Message="Payment failed: "+ ex.Message;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


    }
}
