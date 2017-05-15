using Auction.Service.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using NLog;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Auction.Service.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private Logger _log;
        private AdministratorDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        
        public AccountController()
        {
            _log = LogManager.GetLogger("account");
            _context = new AdministratorDbContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_context));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
        }
        
        ~AccountController()
        {
            Dispose(false);
        }
        
        [Route("login/{userName}/{userPassword}")]
        [HttpGet]
        public IHttpActionResult Login(String userName, String userPassword)
        {
            try
            {
                IdentityUser user = _userManager.Find(userName, userPassword);

                if (user == null)
                {
                    _log.Info("LOGIN FAILED for user {0} from address {1}", userName, HttpContext.Current.Request.UserHostAddress);

                    return NotFound();
                }
                
                HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                
                ClaimsIdentity claimsIdentity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.Current.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, claimsIdentity);

                _log.Info("LOGIN SUCCESS for user {0} from address {1}", userName, HttpContext.Current.Request.UserHostAddress);

                return Ok();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "LOGIN aborted with exception.");

                return NotFound();
            }
        }
        
        [Route("logout")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult Logout()
        {
            try
            {
                HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                return Ok();
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
                _userManager.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
