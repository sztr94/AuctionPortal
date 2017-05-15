using Auction.WebSite.Models;
using System;
using System.Web.Mvc;
using System.Diagnostics;

namespace Auction.WebSite.Controllers
{
    public class BiddingController : BaseController
    {
        [HttpGet]
        public ActionResult Index(Int32? objectId)
        {
            BiddingViewModel bidding = _auctionService.NewBidding(objectId);

            if (bidding == null)
                return RedirectToAction("Index", "Home");
            
            if (_accountService.CurrentUserName != null)
            {
                AuctionGuest guest = _accountService.GetGuest(_accountService.CurrentUserName);
                
                if (guest != null)
                {
                    bidding.Email = guest.Email;
                    bidding.Name = guest.Name;
                    bidding.PhoneNumber = guest.PhoneNumber;
                }
            }

            return View("Index", bidding);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Int32? objectId, BiddingViewModel bidding)
        {
            if (objectId == null || bidding == null)
                return RedirectToAction("Index", "Home");

            bidding.AuctionObject = _auctionService.GetObject(objectId);

            if (bidding.AuctionObject == null)
                return RedirectToAction("Index", "Home");

            switch (BiddingValidator.Validate(bidding.BiddingAmount, bidding.AuctionObject.Id))
            {
                case BiddingError.PriceError:
                    ModelState.AddModelError("", "A megadott összegnek nagyobbnak kell lennie jelenlegi licitnél.");
                    break;
                case BiddingError.FirstBiddingError:
                    ModelState.AddModelError("", "Erre a tárgyra még nem licitáltak. Ilyenkor a kezdő összeget kell megadnia.");
                    break;
            }

            if (!ModelState.IsValid)
            {
                return View("Index", bidding);
            }
            
            String userName;
            
            if (_accountService.CurrentUserName != null)
            {
                userName = _accountService.CurrentUserName;
            }
            else
            {
                if (!_accountService.Create(bidding, out userName))
                {
                    ModelState.AddModelError("", "A licitálás rögzítése sikertelen, kérem próbálja újra!");
                    return View("Index", bidding);
                }
            }

            if (!_auctionService.SaveBidding(objectId, userName, bidding))
            {
                ModelState.AddModelError("", "A licitálás rögzítése sikertelen, kérem próbálja újra!");
                return View("Index", bidding);
            }

            ViewBag.Message = "A licitálást sikeresen rögzítettük!";
            return View("Result", bidding);
        }

        public FileResult ImageForObject(Int32? objectId)
        {
            String imagePath = _auctionService.GetObjectImageId(objectId);

            if (imagePath == null)
                return File("~/Content/NoImage.png", "image/png");

            return File("C:/Users/Eszti/Documents/Documents/ELTE/WAF/Auction.WebSite/Images/" + imagePath, "image/png");
        }
    }
}
