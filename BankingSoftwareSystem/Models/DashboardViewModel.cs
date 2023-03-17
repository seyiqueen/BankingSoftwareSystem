using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Models
{
    public class DashboardViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectListItem> LoggedInUserAccounts { get; set; } = new List<SelectListItem>();
    }
}