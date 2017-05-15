using System;
using System.Linq;

namespace Auction.WebSite.Models
{
    public static class BiddingValidator
    {
        public static BiddingError Validate(Int32 price, Int32? objectId)
        {
            using (AuctionEntities entities = new AuctionEntities())
            {
                if (entities.Bidding.OrderByDescending(b => b.BiddingDate).FirstOrDefault(Bidding => Bidding.AuctionObject.Id == objectId) != null)
                {
                    if (price <= entities.Bidding.OrderByDescending(b => b.BiddingDate).FirstOrDefault(Bidding => Bidding.AuctionObject.Id == objectId).BiddingAmount)
                        return BiddingError.PriceError;
                }
                else
                {
                    if (price != entities.AuctionObject.FirstOrDefault(o => o.Id == objectId).StartBiddingAmount)
                        return BiddingError.FirstBiddingError;
                }
            }

            return BiddingError.None;
        }
    }
}