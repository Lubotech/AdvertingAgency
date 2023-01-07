﻿using AdvertisingAgency.Models.Date;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AdvertisingAgency.Models.ViewModels.Account
{
    public class UserVM
    {
        public UserVM()
        {
        }

        public UserVM(UserDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAdress = row.EmailAdress;
            Username = row.Username;
            ConfirmPassword = row.Password;
        }

        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Surname")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email Address Please")]
        public string EmailAdress { get; set; }
        [Required]
        [DisplayName("Login")]
        public string Username { get; set; }
        [Required]
        [DisplayName("Password")]
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}