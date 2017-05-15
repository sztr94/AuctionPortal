using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction.Data
{
    public class ImageDTO
    {
        public Int32 Id { get; set; }
        public Int32 ObjectId { get; set; }
        public Byte[] Image { get; set; }
    }
}
