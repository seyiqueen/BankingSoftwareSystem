using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankingSoftwareSystem.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountNumber { get; set; }
        public string PIN { get; set; }
        public bool IsPinMode { get; set; }
    }
}