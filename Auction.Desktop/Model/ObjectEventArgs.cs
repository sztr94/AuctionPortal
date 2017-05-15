using System;
using Auction.Data;

namespace Auction.Desktop.Model
{
    public class ObjectEventArgs : EventArgs
    {
        public Int32 Object { get; set; }
        public ObjectDTO AuctionObject { get; set; }
    }
}
