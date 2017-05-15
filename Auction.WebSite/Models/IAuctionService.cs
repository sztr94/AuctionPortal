using System;
using System.Collections.Generic;

namespace Auction.WebSite.Models
{
    public interface IAuctionService
    {
        IEnumerable<AuctionCategory> Categories { get; }

        IEnumerable<AuctionObject> Objects { get; }
        AuctionObject GetObject(Int32? objectId);
        IEnumerable<AuctionObject> GetObjects(Int32? categoryId);
        String GetObjectImageId(Int32? objectId);

        IEnumerable<Bidding> Biddings { get; }
        IEnumerable<Bidding> GetActiveWonBiddings(string userName);
        IEnumerable<Bidding> GetOldWonBiddings(string userName);
        IEnumerable<Bidding> GetActiveLostBiddings(string userName);
        IEnumerable<Bidding> GetOldLostBiddings(string userName);
        IEnumerable<Bidding> GetActualPrices { get; }
        IEnumerable<AuctionObject> NoBidding { get; }
        BiddingViewModel NewBidding(Int32? objectId);
        Int32 GetPrice(Int32? objectId, BiddingViewModel bidding);
        Boolean SaveBidding(Int32? objectId, String userName, BiddingViewModel bidding);

    }
}
