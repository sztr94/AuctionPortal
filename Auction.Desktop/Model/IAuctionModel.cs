using Auction.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auction.Desktop.Model
{
    public interface IAuctionModel
    {
        IReadOnlyList<ObjectDTO> AuctionObjects { get; }
        IReadOnlyList<CategoryDTO> AuctionCategories { get; }
        IReadOnlyList<BiddingDTO> Biddings { get; }
        Boolean IsUserLoggedIn { get; }
        event EventHandler<ObjectEventArgs> ObjectClosed;
        void CreateObject(ObjectDTO auctionObject);
        Task CloseObject(ObjectDTO auctionObject);
        void CreateImage(Int32 objectId, byte[] image);
        Task LoadAsync();
        Task SaveAsync();
        Task LoadAsyncBiddings(ObjectDTO auctionObject);
        Task<Boolean> LoginAsync(String userName, String userPassword);
        Task<Boolean> LogoutAsync();
    }
}
