using Auction.Desktop.Persistence;
using Auction.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Desktop.Model
{
    public class AuctionModel : IAuctionModel
    {
        private enum DataFlag
        {
            Create,
            Close
        }

        private IAuctionPersistence _persistence;
        private List<ObjectDTO> _objects;
        private List<BiddingDTO> _biddings;
        private Dictionary<ObjectDTO, DataFlag> _objectFlags;
        private Dictionary<ImageDTO, DataFlag> _imageFlags;
        private List<CategoryDTO> _categories;
        
        public AuctionModel(IAuctionPersistence persistence)
        {
            if (persistence == null)
                throw new ArgumentNullException("persistence");

            IsUserLoggedIn = false;
            _persistence = persistence;
        }
        
        public IReadOnlyList<CategoryDTO> AuctionCategories
        {
            get { return _categories; }
        }
        public IReadOnlyList<ObjectDTO> AuctionObjects
        {
            get { return _objects; }
        }
        public IReadOnlyList<BiddingDTO> Biddings
        {
            get { return _biddings; }
        }

        public Boolean IsUserLoggedIn { get; private set; }

        public event EventHandler<ObjectEventArgs> ObjectClosed;
        
        public void CreateObject(ObjectDTO auctionObject)
        {
            if (auctionObject == null)
                throw new ArgumentNullException("auctionObject");
            if (_objects.Contains(auctionObject))
                throw new ArgumentException("The object is already in the collection.", "auctionObject");

            auctionObject.Id = (_objects.Count > 0 ? _objects.Max(ao => ao.Id) : 0) + 1;
            auctionObject.ActualPrice = auctionObject.StartBiddingAmount;
            _objectFlags.Add(auctionObject, DataFlag.Create);
            _objects.Add(auctionObject);
        }
        
        public async Task CloseObject(ObjectDTO auctionObject)
        {
            if (auctionObject == null)
                throw new ArgumentNullException("auctionObject");
            
            ObjectDTO objectToClose = _objects.FirstOrDefault(ao => ao.Id == auctionObject.Id);

            if (objectToClose == null)
                throw new ArgumentException("The object does not exist.", "object");

            await _persistence.CloseObjectAsync(objectToClose);

            OnObjectChanged(auctionObject.Id);
        }
        
        public async Task LoadAsync()
        {
            _objects = (await _persistence.ReadObjectsAsync()).ToList();
            _categories = (await _persistence.ReadCategoriesAsync()).ToList();
            
            _objectFlags = new Dictionary<ObjectDTO, DataFlag>();
            _imageFlags = new Dictionary<ImageDTO, DataFlag>();
        }
        
        public async Task SaveAsync()
        {
            List<ObjectDTO> objectsToSave = _objectFlags.Keys.ToList();

            foreach (ObjectDTO auctionObject in objectsToSave)
            {
                Boolean result = true;
                               
                switch (_objectFlags[auctionObject])
                {
                    case DataFlag.Create:
                        result = await _persistence.CreateObjectAsync(auctionObject);
                        break;
                }

                if (!result)
                    throw new InvalidOperationException("Operation " + _objectFlags[auctionObject] + " failed on object " + auctionObject.Id);
                
                _objectFlags.Remove(auctionObject);
            }

            List<ImageDTO> imagesToSave = _imageFlags.Keys.ToList();

            foreach (ImageDTO image in imagesToSave)
            {
                Boolean result = true;
                switch (_imageFlags[image])
                {
                    case DataFlag.Create:
                        result = await _persistence.CreateImageAsync(image);
                        break;
                }

                if (!result)
                    throw new InvalidOperationException("Operation " + _imageFlags[image] + " failed on image " + image.Id);
            }
        }

        public async Task LoadAsyncBiddings(ObjectDTO auctionObject)
        {
            _biddings = (await _persistence.ReadBiddingsAsync(auctionObject)).ToList();
        }

        public async Task<Boolean> LoginAsync(String userName, String userPassword)
        {
            IsUserLoggedIn = await _persistence.LoginAsync(userName, userPassword);
            return IsUserLoggedIn;
        }
        
        public async Task<Boolean> LogoutAsync()
        {
            if (!IsUserLoggedIn)
                return true;

            IsUserLoggedIn = !(await _persistence.LogoutAsync());

            return IsUserLoggedIn;
        }

        private void OnObjectChanged(Int32 objectId)
        {
            if (ObjectClosed != null)
                ObjectClosed(this, new ObjectEventArgs { Object = objectId });
        }

        public void CreateImage(Int32 objectId, Byte[] imageSmall)
        {                            
            ImageDTO image = new ImageDTO
            {
                Id = objectId,
                ObjectId = objectId,
                Image = imageSmall
            };
            
            _imageFlags.Add(image, DataFlag.Create);
        }
    }
}

