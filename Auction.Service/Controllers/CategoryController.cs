using Auction.Data;
using Auction.Service.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace Auction.Service.Controllers
{
    public class CategoryController : ApiController
    {
        private IAuctionEntities _entities;
        
        public CategoryController()
            : this(new AuctionEntities())
        {
        }
        
        public CategoryController(IAuctionEntities entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            _entities = entities;
        }

        ~CategoryController()
        {
            Dispose(false);
        }
        
        public IHttpActionResult GetCities()
        {
            try
            {
                return Ok(_entities.AuctionCategory.ToList().Select(cat => new CategoryDTO
                {
                    Id = cat.Id,
                    Name = cat.Name
                }));
            }
            catch
            {
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
