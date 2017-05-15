using Auction.Desktop.Model;
using Auction.Desktop.Persistence;
using System;
using System.Windows.Controls;

namespace Auction.Desktop.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private IAuctionModel _model;
        
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand LoginCommand { get; private set; }
        
        public String UserName { get; set; }
        
        public event EventHandler ExitApplication;
        public event EventHandler LoginSuccess;
        public event EventHandler LoginFailed;
        
        public LoginViewModel(IAuctionModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            _model = model;
            UserName = String.Empty;

            ExitCommand = new DelegateCommand(param => OnExitApplication());

            LoginCommand = new DelegateCommand(param => LoginAsync(param as PasswordBox));
        }
        
        private async void LoginAsync(PasswordBox passwordBox)
        {
            if (passwordBox == null)
                return;

            try
            {
                Boolean result = await _model.LoginAsync(UserName, passwordBox.Password);

                if (result)
                    OnLoginSuccess();
                else
                    OnLoginFailed();
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("Nincs kapcsolat a kiszolgálóval.");
            }
        }
        
        private void OnLoginSuccess()
        {
            if (LoginSuccess != null)
                LoginSuccess(this, EventArgs.Empty);
        }
        
        private void OnExitApplication()
        {
            if (ExitApplication != null)
                ExitApplication(this, EventArgs.Empty);
        }

        private void OnLoginFailed()
        {
            if (LoginFailed != null)
                LoginFailed(this, EventArgs.Empty);
        }

    }
}
