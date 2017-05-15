using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

using System.Diagnostics;
using System.Security.Cryptography;

namespace Auction.WebSite.Models
{
    public class IdentityAccountService : IAccountService
    {
        private UserManager<IdentityGuest> _guestManager;
        private AuctionEntities _entities;

        public IdentityAccountService()
        {
            _guestManager = new UserManager<IdentityGuest>(new UserStore<IdentityGuest>(new IdentityDbContext<IdentityGuest>()));
            _entities = new AuctionEntities();
        }

        public String CurrentUserName
        {
            get
            {
                String name = HttpContext.Current.User.Identity.Name;

                return String.IsNullOrEmpty(name) ? null : name;
            }
        }

        public AuctionGuest GetGuest(String userName)
        {
            if (userName == null)
                return null;
            
            IdentityGuest guest = _guestManager.FindByName(userName);
            if (guest != null)
            {
                return new AuctionGuest
                {
                    Name = guest.Name,
                    Email = guest.Email,
                    PhoneNumber = guest.PhoneNumber
                };
            }
            
            return _entities.AuctionGuest.FirstOrDefault(c => c.UserName == userName);
        }

        public Boolean Login(UserViewModel user)
        {
            if (user == null)
                return false;
            
            if (!Validator.TryValidateObject(user, new ValidationContext(user, null, null), null))
                return false;
            
            IdentityGuest identityGuest = _guestManager.Find(user.UserName, user.UserPassword);

            if (identityGuest == null) 
                return false;
            
            HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            ClaimsIdentity claimsIdentity = _guestManager.CreateIdentity(identityGuest, DefaultAuthenticationTypes.ApplicationCookie);
            HttpContext.Current.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = user.RememberLogin }, claimsIdentity);

            return true;
        }

        public Boolean Logout()
        {
            HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return true;
        }

        public Boolean Register(GuestRegistrationViewModel guest)
        {
            if (guest == null)
                return false;

            if (!Validator.TryValidateObject(guest, new ValidationContext(guest, null, null), null))
                return false;

            if (_guestManager.FindByName(guest.UserName) != null)
                return false;

            IdentityResult result = _guestManager.Create(new IdentityGuest
            {
                UserName = guest.UserName,
                Name = guest.Name,
                Email = guest.Email,
                PhoneNumber = guest.PhoneNumber,
            }, guest.UserPassword);

            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            Byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(guest.UserPassword));

            _entities.AuctionGuest.Add(new AuctionGuest
            {
                Name = guest.Name,
                UserName = guest.UserName,
                UserPassword = hashedBytes,
                Email = guest.Email,
                PhoneNumber = guest.PhoneNumber,
                
            });

            try
            {
                _entities.SaveChanges();
            }
            catch
            {
                return false;
            }

            return result.Succeeded;         
        }
        
        public Boolean Create(GuestViewModel guest, out String userName)
        {
            userName = "user" + Guid.NewGuid();

            if (guest == null)
                return false;
            
            if (!Validator.TryValidateObject(guest, new ValidationContext(guest, null, null), null))
                return false;
            
            _entities.AuctionGuest.Add(new AuctionGuest
            {
                Name = guest.Name,
                Email = guest.Email,
                PhoneNumber = guest.PhoneNumber,
                UserName = userName
            });

            try
            {
                _entities.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}