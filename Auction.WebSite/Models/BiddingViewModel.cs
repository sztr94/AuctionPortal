using System;
using System.ComponentModel.DataAnnotations;

namespace Auction.WebSite.Models
{
    public class BiddingViewModel : GuestViewModel
    {
        public AuctionObject AuctionObject { get; set; }
        
        [Required(ErrorMessage = "A licitösszeg megadása kötelező.")]
        public Int32 BiddingAmount { get; set; }
    }
}