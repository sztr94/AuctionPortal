using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.WebSite.Models
{
    public class GuestViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(60, ErrorMessage = "A foglaló neve maximum 60 karakter lehet.")]
        public String Name { get; set; }

        [Required(ErrorMessage = "A telefonszám megadása kötelező.")]
        [Phone(ErrorMessage = "A telefonszám formátuma nem megfelelő.")]
        [DataType(DataType.PhoneNumber)]
        public String PhoneNumber { get; set; }

        [Required(ErrorMessage = "Az e-mail cím megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Az e-mail cím nem megfelelő formátumú.")]
        [DataType(DataType.EmailAddress)] 
        public String Email { get; set; }
    }
}

