using Auction.WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Diagnostics;

namespace Auction.WebSite.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }

            int pageNumber = page ?? 1;

            if (!String.IsNullOrEmpty(searchString))
                return View("Index", _auctionService.Objects.Where(ao => ao.Name.Contains(searchString)).OrderByDescending(ao => ao.StartDate).ToPagedList(pageNumber, 20));
            else
                return View("Index", _auctionService.Objects.OrderByDescending(ao => ao.StartDate).ToPagedList(pageNumber, 20));
        }
        
        public ActionResult List(Int32? categoryId, string searchString, int? page)
        {
            IPagedList<AuctionObject> objects;

            if (searchString != null)
            {
                page = 1;
            }

            int pageNumber = page ?? 1;

            if (!String.IsNullOrEmpty(searchString))
            {
                objects = _auctionService.GetObjects(categoryId).Where(ao => ao.Name.Contains(searchString) && ao.EndDate >= DateTime.Now).OrderByDescending(ao => ao.StartDate).ToPagedList(pageNumber, 20);
            }
            else
            {
                objects = _auctionService.GetObjects(categoryId).Where(ao => ao.EndDate >= DateTime.Now).OrderByDescending(ao => ao.StartDate).ToPagedList(pageNumber, 20);
            }

                if (objects == null) 
                return RedirectToAction("Index");

            return View("Index", objects);
        }

        public ActionResult Details(Int32? objectId)
        {
            AuctionObject auctionObject = _auctionService.GetObject(objectId);

            if (auctionObject == null)
                return RedirectToAction("Index");

            ViewBag.Title = "Tárgy részletei: " + auctionObject.Name + " (" + auctionObject.AuctionCategory.Name + ")";

            return View("Details", auctionObject);
        }

        public FileResult ImageForObject(Int32? objectId)
        {
            String imagePath = _auctionService.GetObjectImageId(objectId);

            if (imagePath == null)
                return File("~/Content/NoImage.png", "image/png");

            return File("~/Images/" + imagePath, "image/png");
        }

        public ActionResult MyBiddings()
        {
            IEnumerable<Bidding> biddings = _auctionService.Biddings;

            return View("MyBiddings", biddings);
        }
    }
}