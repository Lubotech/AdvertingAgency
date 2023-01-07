using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AdvertisingAgency.Models.ViewModels.Account
{
    public class LoginUserVM
    {
        [Required]
        [DisplayName("Login")]
        public string Username { get; set; }
        [Required]
        [DisplayName("Password")]
        public string Password { get; set; }
        [DisplayName("Remember")]
        public bool RememberMe { get; set; }

        [DisplayName("Ur age")]
        public short Age { get; set; }   
    }
}