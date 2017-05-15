using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction.Data
{
    public class GuestDTO
    {
        public String Name { get; set; }
        public String UserName { get; set; }
        public Byte[] UserPassword { get; set; }
        public String PhoneNumber { get; set; }
        public String Email { get; set; }
    }
}
