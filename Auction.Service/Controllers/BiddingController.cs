using Auction.Data;
using Auction.Service.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;

namespace Auction.Service.Controllers
{
    [RoutePrefix("api/bidding")]
    public class BiddingController : ApiController
    {
        private Logger _log;
        private IAuctionEntities _entities;

        public BiddingController()
            : this(new AuctionEntities())
        {
        }

        public BiddingController(IAuctionEntities entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");
            
            _entities = entities;
        }

        ~BiddingController()
        {
            Dispose(false);
        }

        [Route("getbiddings/{auctionObjectId}")]
        [HttpGet]
        public IHttpActionResult GetBiddings(Int32 auctionObjectId)
        {
            try
            {
                List<BiddingDTO> result = new List<BiddingDTO>();
                result = _entities.Bidding.OrderByDescending(b => b.Id).Where(b => b.ObjectId == auctionObjectId).ToList().Select(b => new BiddingDTO
                {
                    Id = b.Id,
                    ObjectId =  new ObjectDTO {
                                Id = b.AuctionObject.Id,
                                Name = b.AuctionObject.Name,
                                Category = new CategoryDTO { Id = b.AuctionObject.AuctionCategory.Id, Name = b.AuctionObject.AuctionCategory.Name },
                                Advertiser = b.AuctionObject.AuctionAdvertiser.UserName,
                                StartBiddingAmount = b.AuctionObject.StartBiddingAmount,
                                ActualPrice = b.AuctionObject.StartBiddingAmount,
                                StartDate = b.AuctionObject.StartDate,
                                EndDate = b.AuctionObject.EndDate,
                                ObjectImage = b.AuctionObject.ObjectImage},
                    UserName = new GuestDTO { UserName = b.AuctionGuest.UserName, UserPassword = b.AuctionGuest.UserPassword, Name = b.AuctionGuest.Name, Email = b.AuctionGuest.Email, PhoneNumber = b.AuctionGuest.PhoneNumber},
                    BiddingAmount = b.BiddingAmount,
                    BiddingDate = b.BiddingDate
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "AUCTIONOBJECT GET query failed from address {0} with argument: {1}",
                           HttpContext.Current.Request.UserHostAddress);

                return InternalServerError();
            }
        }
    }
}