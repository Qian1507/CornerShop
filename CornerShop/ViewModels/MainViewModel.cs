using CornerShop.Core.Data;
using CornerShop.Core.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace CornerShop.ViewModels
{
    public class MainViewModel:INotifyPropertyChanged
    {
        private readonly ShopRepository _repo;
      
        private string username { get; set; } ="";
        public string Username
        {
            get { return username; }
            set 
            { 
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public event Action<User> OnLoginSuccess;

        private string _message;
        public string Message
        {
            get { return _message; }
            set 
            { 
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        public MainViewModel()
        {
            _repo = new ShopRepository(DBConfig.ConnectionString);
        }
        public async Task<User> LoginAsync(string username,string password)
        {
            username = username?.Trim();
            password = password?.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password.");
                return null;
            }

            try
            {
                var user = await _repo.LoginAsync(username, password);

                if (user == null)
                {
                    bool userexists= await _repo.UserExistsAsync(username);
                    if (userexists)
                    {
                        MessageBox.Show ( "Incorrect Password. Please try again.");
                    }
                    else
                    {
                        MessageBox.Show("Account does not exist. Please Register first.");
                    }
                    return null;
                }
                Message = "";

                if (user.Role == UserRole.Admin)
                {
                    MessageBox.Show($"Welcome, Admin!\n\nOpening Admin API...",
                            "Login Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    OpenAdminApi();
                    System.Windows.Application.Current.Shutdown();
                }
                
                else
                {
                   
                    OnLoginSuccess?.Invoke(user);
                }
                return user;
            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
            }
            return null;
        }
        
        public async Task<User> RegisterAsync(string username,string password)
        {
            username = username?.Trim();
            password = password?.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and Password cannot be empty.");
                return null;
            }
            try
            {
                Message = "";
                return await _repo.RegisterAsync(username, password);

            }catch(Exception ex)
            {
                Message = ex.Message;
                return null;

            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name="")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        private void OpenAdminApi()
        {
            string apiUrl= DBConfig.AdminApiUrl;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = apiUrl,
                    UseShellExecute = true,
                });

            }catch(Exception ex)
            {
                MessageBox.Show($"Could not open Admin API : + ex.Message");
            }
        }
        }
    
}
