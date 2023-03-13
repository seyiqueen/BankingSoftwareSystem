using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankingSoftwareSystem.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter an Account Number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Please Enter your preferred PIN")]
        public string PIN { get; set; }

        [Compare(nameof(PIN), ErrorMessage = "Account PINs do not match")]
        public string ConfirmPIN { get; set; }

    }
}