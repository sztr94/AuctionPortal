using System;

namespace Auction.Data
{
    public class CategoryDTO
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        
        public override String ToString() { return Name; }
    }
}
