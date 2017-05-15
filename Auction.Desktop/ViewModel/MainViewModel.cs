using Auction.Desktop.Model;
using Auction.Desktop.Persistence;
using Auction.Data;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Auction.Desktop.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IAuctionModel _model;
        private ObservableCollection<ObjectDTO> _objects;
        private ObservableCollection<CategoryDTO> _categories;
        private ObservableCollection<BiddingDTO> _biddings;
        private ObjectDTO _selectedObject;
        private Boolean _isLoaded;
        private Boolean _hasPicture;
        private Boolean _hasBiddings;
        private String _userName;

        public ObservableCollection<ObjectDTO> Objects
        {
            get { return _objects; }
            private set
            {
                if (_objects != value)
                {
                    _objects = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<CategoryDTO> Categories
        {
            get { return _categories; }
            private set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<BiddingDTO> Biddings
        {
            get { return _biddings; }
            private set
            {
                if (_biddings != value)
                {
                    _biddings = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsLoaded
        {
            get { return _isLoaded; }
            private set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    OnPropertyChanged();
                }
            }
        }
        public Boolean HasPicture
        {
            get { return _hasPicture; }
            set
            {
                if (_hasPicture != value)
                {
                    _hasPicture = value;
                    OnPropertyChanged();
                }
            }
        }
        public Boolean NoPicture
        {
            get { return !_hasPicture; }
            set
            {
                if (_hasPicture != value)
                {
                    _hasPicture = value;
                    OnPropertyChanged();
                }
            }
        }
        public Boolean HasBiddings
        {
            get { return _hasBiddings; }
            set
            {
                if (_hasBiddings != value)
                {
                    _hasBiddings = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObjectDTO SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObjectDTO EditedObject { get; private set; }

        public String SelectedObjectImage
        {
            get
            {
                if(SelectedObject.ObjectImage == null)
                {
                    return Path.GetFullPath("~/../../../../Auction.WebSite/Content/NoImage.png");
                }
                else
                {
                    return Path.GetFullPath("~/../../../../Auction.WebSite/Images/" + SelectedObject.ObjectImage);
                }
            }
        }
      
        public DelegateCommand CreateObjectCommand { get; private set; }
        public DelegateCommand CreateImageCommand { get; private set; }
        public DelegateCommand ViewObjectCommand { get; private set; }
        public DelegateCommand CloseObjectCommand { get; private set; }
        public DelegateCommand SaveChangesCommand { get; private set; }
        public DelegateCommand CancelChangesCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand LogOutCommand { get; private set; }

        public event EventHandler<ObjectEventArgs> ImageEditingStarted;
        public event EventHandler<ObjectEventArgs> OnViewObject;
        public event EventHandler<ObjectEventArgs> ObjectClosed;
        public event EventHandler ExitApplication;
        public event EventHandler CreateObjectStarted;
        public event EventHandler CreateObjectFinished;
        public event EventHandler LogOut;

        public MainViewModel(IAuctionModel model, String UserName)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            _model = model;
            _model.ObjectClosed += Model_ObjectChanged;

            _isLoaded = false;
            _hasPicture = false;

            _userName = UserName;

            CreateObjectCommand = new DelegateCommand(param =>
            {
                EditedObject = new ObjectDTO();
                EditedObject.Id = (_objects.Count > 0 ? _objects.Max(b => b.Id) : 0) + 1;
                EditedObject.Advertiser = _userName;
                EditedObject.StartDate = DateTime.Today;
                EditedObject.EndDate = DateTime.Today;
                OnCreateObjectStarted();
            });
            CreateImageCommand = new DelegateCommand(param => OnImageEditingStarted(param as ObjectDTO));
            ViewObjectCommand = new DelegateCommand(param => ViewObject(param as ObjectDTO));
            CloseObjectCommand = new DelegateCommand(param => CloseObject(param as ObjectDTO));
            SaveChangesCommand = new DelegateCommand(param => SaveChanges());
            CancelChangesCommand = new DelegateCommand(param => CancelChanges());
            LoadCommand = new DelegateCommand(param => LoadAsync());
            SaveCommand = new DelegateCommand(param => SaveAsync());
            ExitCommand = new DelegateCommand(param => OnExitApplication());
            LogOutCommand = new DelegateCommand(param => OnLogOut());
        }

        public async void LoadBiddings()
        {
            try
            {
                await _model.LoadAsyncBiddings(_selectedObject);
                Biddings = new ObservableCollection<BiddingDTO>(_model.Biddings);
                if(_biddings.Count == 0 || SelectedObject.EndDate < DateTime.Now)
                {
                    HasBiddings = false;
                }
                else
                {
                    HasBiddings = true;
                }
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("A betöltés sikertelen! Nincs kapcsolat a kiszolgálóval.");
            }
        }
        
        private async void CloseObject(ObjectDTO auctionObject)
        {
            if (auctionObject == null)
                return;

            try
            {
                await _model.CloseObject(_selectedObject);
                OnMessageApplication("A tárgy lezárásra került!");
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("A tárgy lezárása sikertelen! Nincs kapcsolat a kiszolgálóval.");
            }
        }

        private void SaveChanges()
        {
            if (EditedObject == null)
                return;

            if (EditedObject.Name == null)
            {
                OnMessageApplication("Az tárgy neve nincs megadva!");
                return;
            }
            if (EditedObject.Category == null)
            {
                OnMessageApplication("A tárgy kategóriája nincs megadva!");
                return;
            }
            if (EditedObject.Description == null)
            {
                OnMessageApplication("A tárgy leírása nincs megadva!");
                return;
            }
            if (EditedObject.StartBiddingAmount == 0)
            {
                OnMessageApplication("A kezdő licitösszeg nincs megadva!");
                return;
            }
            if (EditedObject.EndDate <= DateTime.Today)
            {
                OnMessageApplication("A lezárás dátuma helytelen!");
                return;
            }

            _model.CreateObject(EditedObject);
            Objects.Add(EditedObject);
            SelectedObject = EditedObject;

            EditedObject = null;

            OnCreateObjectFinished();
        }

        private void CancelChanges()
        {
            EditedObject = null;
            OnCreateObjectFinished();
        }

        private async void LoadAsync()
        {
            try
            {
                await _model.LoadAsync();
                Objects = new ObservableCollection<ObjectDTO>(_model.AuctionObjects); 
                Categories = new ObservableCollection<CategoryDTO>(_model.AuctionCategories);
                IsLoaded = true;
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("A betöltés sikertelen! Nincs kapcsolat a kiszolgálóval.");
            }
        }
        
        private async void SaveAsync()
        {
            try
            {
                await _model.SaveAsync();
                OnMessageApplication("A mentés sikeres!");
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("A mentés sikertelen! Nincs kapcsolat a kiszolgálóval.");
            }
        }
        
        private void OnExitApplication()
        {
            if (ExitApplication != null)
                ExitApplication(this, EventArgs.Empty);
        }
        
        private void OnCreateObjectStarted()
        {
            if (CreateObjectStarted != null)
                CreateObjectStarted(this, EventArgs.Empty);
        }
        
        private void OnCreateObjectFinished()
        {
            if (CreateObjectFinished != null)
                CreateObjectFinished(this, EventArgs.Empty);
        }

        private void OnImageEditingStarted(ObjectDTO AuctionObject)
        {
            if (ImageEditingStarted != null)
                ImageEditingStarted(this, new ObjectEventArgs { Object = AuctionObject.Id });

            EditedObject.ObjectImage = EditedObject.Id.ToString() + ".png";
        }

        private void ViewObject(ObjectDTO AuctionObject)
        {
            if (OnViewObject != null)
                OnViewObject(this, new ObjectEventArgs { Object = AuctionObject.Id });
        }

        private void Model_ObjectChanged(object sender, ObjectEventArgs e)
        {
            LoadAsync();

            SelectedObject = Objects.FirstOrDefault(ao => ao.Id == e.Object);

            if (ObjectClosed != null)
                ObjectClosed(this, new ObjectEventArgs { Object = e.Object });
        }

        private void OnLogOut()
        {
            if (LogOut != null)
                LogOut(this, new EventArgs());
        }
    }
}

