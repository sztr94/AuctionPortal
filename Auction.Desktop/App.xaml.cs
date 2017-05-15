using Auction.Desktop.Model;
using Auction.Desktop.Persistence;
using Auction.Desktop.View;
using Auction.Desktop.ViewModel;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Windows;

namespace Auction.Desktop
{
    public partial class App : Application
    {
        private IAuctionModel _model;
        private LoginViewModel _loginViewModel;
        private LoginWindow _loginView;
        private MainViewModel _mainViewModel;
        private MainWindow _mainView;
        private ObjectEditorWindow _editorView;
        private ObjectInfoWindow _objectView;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
            Exit += new ExitEventHandler(App_Exit);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            String serviceAddress = ConfigurationManager.AppSettings["ServiceAddress"];

            if (String.IsNullOrEmpty(serviceAddress))
            {
                MessageBox.Show("Konfigurációs hiba:" + Environment.NewLine +
                                "A szolgáltatás címe nem elérhető.", "Aukciós portál",
                                MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            _model = new AuctionModel(new AuctionServicePersistence(serviceAddress));

            _loginViewModel = new LoginViewModel(_model);
            _loginViewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);
            _loginViewModel.MessageApplication += new EventHandler<MessageEventArgs>(ViewModel_MessageApplication);
            _loginViewModel.LoginSuccess += new EventHandler(ViewModel_LoginSuccess);
            _loginViewModel.LoginFailed += new EventHandler(ViewModel_LoginFailed);

            _loginView = new LoginWindow();
            _loginView.DataContext = _loginViewModel;
            _loginView.Show();
        }

        public async void App_Exit(object sender, ExitEventArgs e)
        {
            if (_model.IsUserLoggedIn) 
            {
                await _model.LogoutAsync();
            }
        }

        private void ViewModel_LoginSuccess(object sender, EventArgs e)
        {
            String userName = _loginViewModel.UserName;

            _mainViewModel = new MainViewModel(_model, userName);
            _mainViewModel.MessageApplication += new EventHandler<MessageEventArgs>(ViewModel_MessageApplication);
            _mainViewModel.CreateObjectStarted += new EventHandler(MainViewModel_CreateObjectStarted);
            _mainViewModel.CreateObjectFinished += new EventHandler(MainViewModel_CreateObjectFinished);
            _mainViewModel.ImageEditingStarted += new EventHandler<ObjectEventArgs>(MainViewModel_ImageEditingStarted);
            _mainViewModel.OnViewObject += new EventHandler<ObjectEventArgs>(MainViewModel_ViewObject);
            _mainViewModel.ObjectClosed += new EventHandler<ObjectEventArgs>(MainViewModel_ObjectClosed);
            _mainViewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);
            _mainViewModel.LogOut += new EventHandler(ViewModel_LogOut);

            _mainView = new MainWindow();
            _mainView.DataContext = _mainViewModel;
            _mainView.Show();

            _loginView.Close();
        }

        private void ViewModel_LoginFailed(object sender, EventArgs e)
        {
            MessageBox.Show("A bejelentkezés sikertelen!", "Aukciós portál", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ViewModel_MessageApplication(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, "Aukciós portál", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void MainViewModel_CreateObjectStarted(object sender, EventArgs e)
        {
            _editorView = new ObjectEditorWindow();
            _editorView.DataContext = _mainViewModel;
            _editorView.Show();
        }

        private void MainViewModel_CreateObjectFinished(object sender, EventArgs e)
        {
            _editorView.Close();
        }

        private void MainViewModel_ImageEditingStarted(object sender, ObjectEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.CheckFileExists = true;
                dialog.Filter = "Képfájlok|*.jpg;*.jpeg;*.bmp;*.tif;*.gif;*.png;";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                Boolean? result = dialog.ShowDialog();

                if (result == true)
                {
                    Int32 smallImageSize;

                    String smallImageSizeText = ConfigurationManager.AppSettings["SmallImageSize"];

                    if (String.IsNullOrEmpty(smallImageSizeText) || !Int32.TryParse(smallImageSizeText, out smallImageSize) || smallImageSize < 1)
                        smallImageSize = 100;

                    _model.CreateImage(e.Object, ImageHandler.OpenAndResize(dialog.FileName, smallImageSize));

                    _mainViewModel.NoPicture = false;
                }
            }
            catch { }
        }

        private void MainViewModel_ViewObject(object sender, ObjectEventArgs e)
        {
            _mainViewModel.LoadBiddings();

            _objectView = new ObjectInfoWindow();
            _objectView.DataContext = _mainViewModel;
            _objectView.Show();
        }

        private void MainViewModel_ObjectClosed(object sender, ObjectEventArgs e)
        {
            _objectView.Close();
        }

        private void ViewModel_ExitApplication(object sender, System.EventArgs e)
        {
            Shutdown();
        }

        private async void ViewModel_LogOut(object sender, System.EventArgs e)
        {
            if (_model.IsUserLoggedIn)
            {
                await _model.LogoutAsync();
            }

            _loginView = new LoginWindow();
            _loginView.DataContext = _loginViewModel;
            _loginView.Show();

            _mainView.Close();
        }
    }
}
