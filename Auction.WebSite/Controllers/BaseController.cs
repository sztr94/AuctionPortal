using System.Web.Mvc;
using System.Linq;
using Auction.WebSite.Models;

namespace Auction.WebSite.Controllers
{
    public class BaseController : Controller
    {
        protected IAccountService _accountService;
        protected IAuctionService _auctionService;

        public BaseController()
        {
            _accountService = new IdentityAccountService();
            _auctionService = new AuctionService();
            ViewBag.AuctionCategories = _auctionService.Categories.ToList();
            ViewBag.AuctionObjects = _auctionService.Objects.ToList();
            ViewBag.GetActualPrices = _auctionService.GetActualPrices.ToList();
            ViewBag.NoBidding = _auctionService.NoBidding.ToList();

            if (_accountService.CurrentUserName != null)
            {
                ViewBag.CurrentGuestName = _accountService.GetGuest(_accountService.CurrentUserName).Name;

                ViewBag.ActiveWon = _auctionService.GetActiveWonBiddings(_accountService.CurrentUserName).ToList();
                ViewBag.ActiveLost = _auctionService.GetActiveLostBiddings(_accountService.CurrentUserName).ToList();
                ViewBag.OldWon = _auctionService.GetOldWonBiddings(_accountService.CurrentUserName).ToList();
                ViewBag.OldLost = _auctionService.GetOldLostBiddings(_accountService.CurrentUserName).ToList();
            }

            
        }
    }
}
