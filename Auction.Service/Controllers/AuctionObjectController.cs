using Auction.Data;
using Auction.Service.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using System.Diagnostics;

namespace Auction.Service.Controllers
{
    public class AuctionObjectController : ApiController
    {
        private Logger _log;
        private IAuctionEntities _entities;

        public AuctionObjectController()
            : this(new AuctionEntities())
        {
        }

        public AuctionObjectController(IAuctionEntities entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            _log = LogManager.GetLogger("service");
            _entities = entities;
        }

        ~AuctionObjectController()
        {
            Dispose(false);
        }
        
        [HttpGet]
        public IHttpActionResult GetObjects()
        {
            try
            {
                List<ObjectDTO> result = new List<ObjectDTO>();
                result = _entities.AuctionObject.Include("AuctionCategory").Include("AuctionAdvertiser").OrderByDescending(ao => ao.EndDate).ToList().Select(ao => new ObjectDTO
                {
                    Id = ao.Id,
                    Name = ao.Name,
                    Category = new CategoryDTO { Id = ao.AuctionCategory.Id, Name = ao.AuctionCategory.Name },
                    Description = ao.Description,
                    Advertiser =  ao.AuctionAdvertiser.UserName,
                    StartBiddingAmount = ao.StartBiddingAmount,
                    StartDate = ao.StartDate,
                    EndDate = ao.EndDate,
                    ObjectImage = ao.ObjectImage
                }).ToList();

                foreach(ObjectDTO o in result)
                {
                    if (_entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(Bidding => Bidding.ObjectId == o.Id) != null)
                    {
                        o.ActualPrice = _entities.Bidding.OrderByDescending(b => b.Id).FirstOrDefault(Bidding => Bidding.ObjectId == o.Id).BiddingAmount;
                    }
                    else
                    {
                        o.ActualPrice = o.StartBiddingAmount;
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "AUCTIONOBJECT GET query failed from address {0} with argument: {1}",
                           HttpContext.Current.Request.UserHostAddress);

                return InternalServerError();
            }
        }
        
        [HttpPost]
        public IHttpActionResult PostAuctionObject([FromBody] ObjectDTO objectDTO)
        {
            try
            {
                AuctionObject addedObject = _entities.AuctionObject.Add(new AuctionObject
                {
                    Name = objectDTO.Name,
                    CategoryId = objectDTO.Category.Id,
                    Description = objectDTO.Description,
                    AdvertiserId = objectDTO.Advertiser,
                    StartBiddingAmount = objectDTO.StartBiddingAmount,
                    StartDate = objectDTO.StartDate,
                    EndDate = objectDTO.EndDate,
                    ObjectImage = objectDTO.ObjectImage
                });

                _entities.SaveChanges();

                objectDTO.Id = objectDTO.Id;
                
                return Created(Request.RequestUri + addedObject.Id.ToString(), objectDTO);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "AUCTIONOBJECT POST query failed from address {0} with content: {1}",
                           HttpContext.Current.Request.UserHostAddress,
                           JsonConvert.SerializeObject(objectDTO, Formatting.None));

                return InternalServerError();
            }
        }
        
        [HttpPut]
        public IHttpActionResult CloseObject([FromBody] ObjectDTO objectToClose)
        {
            try
            {
                AuctionObject auctionObject = _entities.AuctionObject.FirstOrDefault(ao => ao.Id == objectToClose.Id);

                if (auctionObject == null)
                    return NotFound();

                auctionObject.EndDate = DateTime.Now;

                _entities.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "AUCTIONOBJECT PUT query failed from address {0} with content: {1}",
                           HttpContext.Current.Request.UserHostAddress,
                           JsonConvert.SerializeObject(objectToClose, Formatting.None));

                return InternalServerError();
            }
        }
        
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                _entities.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
