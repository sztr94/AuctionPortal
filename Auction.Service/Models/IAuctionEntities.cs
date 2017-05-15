using System;
using System.Data.Entity;

namespace Auction.Service.Models
{
    public interface IAuctionEntities : IDisposable
    {
        DbSet<AuctionAdvertiser> AuctionAdvertiser { get; set; }
        DbSet<AuctionGuest> AuctionGuest { get; set; }
        DbSet<AuctionCategory> AuctionCategory { get; set; }
        DbSet<AuctionObject> AuctionObject { get; set; }
        DbSet<Bidding> Bidding { get; set; }

        Int32 SaveChanges();
    }
}