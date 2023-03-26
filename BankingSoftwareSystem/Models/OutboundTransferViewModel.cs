using System.Collections.Generic;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Models
{
    public class OutboundTransferViewModel
    {
        public string AccountNumber { get; set; }
        public float CurrentBalance { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountType { get; set; }
        public List<SelectListItem> LoggedInUserAccounts { get; set; } = new List<SelectListItem>();
    }
}
