using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction.Data
{
    public class BiddingDTO
    {
        public Int32 Id { get; set; }
        public ObjectDTO ObjectId { get; set; }
        public GuestDTO UserName { get; set; }
        public Int32 BiddingAmount { get; set; }
        public DateTime BiddingDate { get; set; }
    }
}
