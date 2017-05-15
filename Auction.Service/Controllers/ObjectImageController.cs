using Auction.Data;
using Auction.Service.Models;
using NLog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Auction.Service.Controllers
{
    public class ObjectImageController : ApiController
    {
        private Logger _log;
        private IAuctionEntities _entities;
        
        public ObjectImageController()
            : this(new AuctionEntities())
        {
        }
        
        public ObjectImageController(IAuctionEntities entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            _log = LogManager.GetLogger("service");
            _entities = entities;
        }
        
        ~ObjectImageController()
        {
            Dispose(false);
        }

        [HttpGet]
        public IHttpActionResult GetImage()
        {
            return Ok(200);
        }


        [HttpPost]
        public IHttpActionResult PostImage([FromBody] ImageDTO image)
        {
            try
            {
                if (image == null)
                    return NotFound();

                byte[] bitmap = image.Image;

                if(_entities.AuctionObject.FirstOrDefault(ao => ao.Id == image.ObjectId) == null)
                {
                    return NotFound();
                }

                AuctionObject auctionObject = _entities.AuctionObject.FirstOrDefault(ao => ao.Id == image.ObjectId);
                
                using (Image im = Image.FromStream(new MemoryStream(bitmap)))
                {
                    im.Save("C:/Users/Eszti/Documents/Documents/ELTE/WAF/ANMS27/bead2/Auction/Auction.WebSite/Images/" + auctionObject.Id.ToString() + ".png", ImageFormat.Png); 
                }

                _entities.SaveChanges();
                return Created(Request.RequestUri + image.Id.ToString(), image.Id);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "BUILDING PUT query failed from address {0} with imageId: {1}, buildingId: {2}",
                           HttpContext.Current.Request.UserHostAddress,
                           image.Id, image.ObjectId);

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
