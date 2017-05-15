using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Diagnostics;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace Auction.WebSite.Models
{
    public class AuctionService : IAuctionService
    {
        private AuctionEntities _entities;

        public AuctionService()
        {
            _entities = new AuctionEntities();
        }

        public IEnumerable<AuctionCategory> Categories { get { return _entities.AuctionCategory; } }

        public IEnumerable<AuctionObject> Objects { get { return _entities.AuctionObject; } }

        public AuctionObject GetObject(Int32? objectId)
        {
            if (objectId == null)
                return null;

            return _entities.AuctionObject.FirstOrDefault(AuctionObject => AuctionObject.Id == objectId);
        }

        public IEnumerable<AuctionObject> GetObjects(Int32? categoryId)
        {
            if (categoryId == null || !_entities.AuctionObject.Any(o => o.CategoryId == categoryId))
                return null;

            return _entities.AuctionObject.Where(o => o.CategoryId == categoryId);
        }

        public String GetObjectImageId(Int32? objectId)
        {
            if (objectId == null)
                return null;

            return _entities.AuctionObject.Where(AuctionObject => AuctionObject.Id == objectId).Select(AuctionObject => AuctionObject.ObjectImage).FirstOrDefault();
        }

        public IEnumerable<Bidding> Biddings
        {
            get { return _entities.Bidding.ToList(); }
        }

        public IEnumerable<Bidding> GetActualPrices
        {
            get
            {
                List<Bidding> result = new List<Bidding>();

                foreach(AuctionObject obj in _entities.AuctionObject)
                {
                    if(_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(Bidding => Bidding.ObjectId == obj.Id) != null)
                    {
                        result.Add(_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(Bidding => Bidding.ObjectId == obj.Id));
                    }
                }

                return result;
            }
        }

        public IEnumerable<AuctionObject> NoBidding
        {
            get
            {
                List<AuctionObject> result = new List<AuctionObject>();

                foreach (AuctionObject obj in _entities.AuctionObject)
                {
                    if (_entities.Bidding.OrderByDescending(b => b.BiddingDate).FirstOrDefault(Bidding => Bidding.ObjectId == obj.Id) == null)
                    {
                        result.Add(_entities.AuctionObject.FirstOrDefault(o => o.Id == obj.Id));
                    }
                }

                IEnumerable<AuctionObject> finalResult = result;

                return result;
            }
        }

        public BiddingViewModel NewBidding(Int32? objectId)
        {
            if (objectId == null)
                return null;

            AuctionObject AuctionObject = _entities.AuctionObject.FirstOrDefault(ao => ao.Id == objectId);

            BiddingViewModel bidding = new BiddingViewModel { AuctionObject = AuctionObject };

            return bidding;
        }

        public Int32 GetPrice(Int32? objectId, BiddingViewModel bidding)
        {
            if (objectId == null || bidding == null)
                return 0;

            return bidding.BiddingAmount;
        }

        public Boolean SaveBidding(Int32? objectId, String userName, BiddingViewModel bidding)
        {
            if (objectId == null || bidding == null || bidding.AuctionObject == null || userName == null)
            {
                return false;
            }
            
            if (!Validator.TryValidateObject(bidding, new ValidationContext(bidding, null, null), null))
            { 
                return false;
            }

            if (BiddingValidator.Validate(bidding.BiddingAmount, objectId) != BiddingError.None)
            {
                return false;
            }

            AuctionGuest guest = _entities.AuctionGuest.Where(u => u.UserName == userName).FirstOrDefault();
            
            if (guest == null)
            {
                return false;
            }

            _entities.Bidding.Add(new Bidding
            {
                ObjectId = bidding.AuctionObject.Id,
                UserName = userName,
                BiddingAmount = bidding.BiddingAmount,
                BiddingDate = DateTime.Now
            });

            try
            {
                _entities.SaveChanges();
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        public IEnumerable<Bidding> GetActiveWonBiddings(string userName)
        {
            IEnumerable<Bidding> actual = GetActualPrices;
            List<Bidding> ownActive = new List<Bidding>();

            foreach (AuctionObject ao in _entities.AuctionObject)
            {
                if (_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(b => b.UserName == userName && b.ObjectId == ao.Id && DateTime.Now <= b.AuctionObject.EndDate) != null)
                    ownActive.Add(_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(b => b.UserName == userName && b.ObjectId == ao.Id && DateTime.Now <= b.AuctionObject.EndDate));
            }

            List<Bidding> result = new List<Bidding>();

            foreach(Bidding b in ownActive)
            {
                if (actual.Contains(b)) { result.Add(b); }
            }

            return result.ToList();
        }

        public IEnumerable<Bidding> GetActiveLostBiddings(string userName)
        {
            IEnumerable<Bidding> actual = GetActualPrices;
            List<Bidding> ownActive = new List<Bidding>();

            foreach(AuctionObject ao in _entities.AuctionObject)
            {
                if(_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(b => b.UserName == userName && b.ObjectId == ao.Id && DateTime.Now <= b.AuctionObject.EndDate) != null)
                    ownActive.Add(_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(b => b.UserName == userName && b.ObjectId == ao.Id && DateTime.Now <= b.AuctionObject.EndDate));
            }           

            List<Bidding> result = new List<Bidding>();

            foreach (Bidding b in ownActive)
            {
                if (!actual.Contains(b)) { result.Add(b); }
            }
            
            return result.ToList();
        }

        public IEnumerable<Bidding> GetOldWonBiddings(string userName)
        {
            IEnumerable<Bidding> actual = GetActualPrices;
            List<Bidding> ownOld = new List<Bidding>();

            foreach (AuctionObject ao in _entities.AuctionObject)
            {
                if (_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(b => b.UserName == userName && b.ObjectId == ao.Id && DateTime.Now > b.AuctionObject.EndDate) != null)
                    ownOld.Add(_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(b => b.UserName == userName && b.ObjectId == ao.Id && DateTime.Now > b.AuctionObject.EndDate));
            }

            List<Bidding> result = new List<Bidding>();

            foreach (Bidding b in ownOld)
            {
                if (actual.Contains(b)) { result.Add(b); }
            }

            return result.ToList();
        }

        public IEnumerable<Bidding> GetOldLostBiddings(string userName)
        {
            IEnumerable<Bidding> actual = GetActualPrices;
            List<Bidding> ownOld = new List<Bidding>();

            foreach (AuctionObject ao in _entities.AuctionObject)
            {
                if (_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(b => b.UserName == userName && b.ObjectId == ao.Id && DateTime.Now > b.AuctionObject.EndDate) != null)
                    ownOld.Add(_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(b => b.UserName == userName && b.ObjectId == ao.Id && DateTime.Now > b.AuctionObject.EndDate));
            }

            List<Bidding> result = new List<Bidding>();

            foreach (Bidding b in ownOld)
            {
                if (!actual.Contains(b)) { result.Add(b); }
            }

            return result.ToList();
        }
    }
}