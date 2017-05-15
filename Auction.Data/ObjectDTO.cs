using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction.Data
{
    public class ObjectDTO
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public CategoryDTO Category { get; set; }
        public String Description { get; set; }
        public String Advertiser { get; set; }
        public Int32 StartBiddingAmount { get; set; }
        public Int32 ActualPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String ObjectImage { get; set; }
    }
}
