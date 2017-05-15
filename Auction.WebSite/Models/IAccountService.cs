using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction.WebSite.Models
{
    public interface IAccountService
    {
        String CurrentUserName { get; }
        AuctionGuest GetGuest(String userName);
        Boolean Login(UserViewModel user);
        Boolean Logout();
        Boolean Register(GuestRegistrationViewModel guest);
        Boolean Create(GuestViewModel guest, out String userName);
    }
}