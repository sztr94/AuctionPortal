using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.WebSite.Models
{
    public class GuestRegistrationViewModel : GuestViewModel
    {
        [Required(ErrorMessage = "A felhasználónév megadása kötelező.")]
        [RegularExpression("^[A-Za-z0-9_-]{5,40}$", ErrorMessage = "A felhasználónév formátuma, vagy hossza nem megfelelő.")]
        public String UserName { get; set; }

        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        [RegularExpression("^[A-Za-z0-9_-]{5,40}$", ErrorMessage = "A jelszó formátuma, vagy hossza nem megfelelő.")]
        [DataType(DataType.Password)]
        public String UserPassword { get; set; }

        [Required(ErrorMessage = "A jelszó ismételt megadása kötelező.")]
        [Compare("UserPassword", ErrorMessage = "A két jelszó nem egyezik.")]
        [DataType(DataType.Password)]
        public String UserConfirmPassword { get; set; }
    }
}