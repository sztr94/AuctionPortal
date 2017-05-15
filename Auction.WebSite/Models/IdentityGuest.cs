using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace Auction.WebSite.Models
{
    public class IdentityGuest : IdentityUser
    {
        public String Name { get; set; }
    }
}