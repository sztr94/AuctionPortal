using Auction.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auction.Desktop.Persistence
{
    public interface IAuctionPersistence
    {
        Task<IEnumerable<ObjectDTO>> ReadObjectsAsync();
        Task<IEnumerable<CategoryDTO>> ReadCategoriesAsync();
        Task<IEnumerable<BiddingDTO>> ReadBiddingsAsync(ObjectDTO auctionObject);

        Task<Boolean> CreateObjectAsync(ObjectDTO auctionObject);
        Task<Boolean> CloseObjectAsync(ObjectDTO auctionObject);
        Task<Boolean> CreateImageAsync(ImageDTO image);

        Task<Boolean> LoginAsync(String userName, String userPassword);
        Task<Boolean> LogoutAsync();
    }
}
