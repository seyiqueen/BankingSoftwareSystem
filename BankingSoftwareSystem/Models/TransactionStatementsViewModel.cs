using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Models
{
    public class TransactionStatementsViewModel
    {
        public string AccountNumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TodaysDate { get; set; }
        public List<SelectListItem> LoggedInUserAccounts { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Dates { get; set; } = new List<SelectListItem>();
        public List<TransactionStatementModel> TransactionStatements { get; set; } = new List<TransactionStatementModel>();
    }

    public class TransactionStatementModel
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string TransactionType { get; set; }
        public string SenderAccountNumber { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public string TransferToUserAccountId { get; set; }
        public string TransferFromUserAccountId { get; set; }
        public decimal Amount { get; set; }
        public bool IsOwnTransfer { get; set; }
        public DateTime TransferDate { get; set; }
        public string Narration { get; set; }
    }
}