using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.WebSite.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "A felhasználónév megadása kötelező.")]
        public String UserName { get; set; }


        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        [DataType(DataType.Password)]
        public String UserPassword { get; set; }

        public Boolean RememberLogin { get; set; }
    }
}