using Auction.WebSite.Models;
using System.Web.Mvc;

namespace Auction.WebSite.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        
        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserViewModel user)
        {
            if (!ModelState.IsValid)
                return View("Login", user);
            
            if (!_accountService.Login(user))
            { 
                ModelState.AddModelError("", "Hibás felhasználónév, vagy jelszó.");
                return View("Login", user);
            }

            return RedirectToAction("Index", "Home"); 
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(GuestRegistrationViewModel guest)
        {
            if (!ModelState.IsValid)
                return View("Register", guest);

            if (!_accountService.Register(guest))
            {
                ModelState.AddModelError("UserName", "A megadott felhasználónév már létezik.");
                return View("Register", guest);
            }

            string userName = guest.UserName;
            _accountService.Create(guest, out userName);

            _accountService.Logout();

            ViewBag.Information = "A regisztráció sikeres volt. Kérjük, jelentkezzen be.";

            return RedirectToAction("Login");
        }
        
        public ActionResult Logout()
        {
            _accountService.Logout();

            return RedirectToAction("Index", "Home");
        }
    }
}