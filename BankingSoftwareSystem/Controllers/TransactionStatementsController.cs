using BankingSoftwareSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Controllers
{
    public class TransactionStatementsController : Controller
    {
        // GET: TransactionStatements
        public ActionResult Index()
        {
            return RenderPage(1);
        }

        public ActionResult Range(int id, string startDate, string endDate)
        {
            return RenderPage(id, startDate, endDate);
        }

        private ActionResult RenderPage(int rangeId, string _startDate = null, string _endDate = null)
        {
            var userAccId = Session[ApplicationConstants.Session_User_Account_Id];
            if (userAccId == null)
                return RedirectToAction("Login", "Portal");

            var model = new TransactionStatementsViewModel();

            var userId = Session[ApplicationConstants.Session_User_Id];

            MySqlConnection conn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
            conn.Open();

            var query = $"USE {ApplicationConstants.DB};";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.ExecuteNonQuery();

            command = new MySqlCommand($"SELECT * FROM UserAccounts WHERE UserId = '{userId}'", conn);
            MySqlDataReader dataReader = command.ExecuteReader();

            List<SelectListItem> LoggedInUserAccounts = new List<SelectListItem>();

            string fullName = string.Empty;
            string accountNumber = string.Empty;
            while (dataReader.Read())
            {
                LoggedInUserAccounts.Add(new SelectListItem
                {
                    Value = dataReader.GetString("Id"),
                    Text = dataReader.GetString("FirstName") + " " + dataReader.GetString("LastName") + " - " + dataReader.GetString("AccountNumber"),
                    Selected = dataReader.GetString("Id") == (string)userAccId,
                });

                if (dataReader.GetString("Id") == (string)userAccId)
                {
                    accountNumber = dataReader.GetString("AccountNumber");
                    fullName = dataReader.GetString("FirstName") + " " + dataReader.GetString("FirstName");
                }
            }

            ViewBag.LoggedInUserAccounts = LoggedInUserAccounts;
            model.LoggedInUserAccounts = LoggedInUserAccounts;

            dataReader.Close();

            model.Dates.Add(new SelectListItem
            {
                Text = "One Week ago",
                Value = "1",
                Selected = rangeId == 1
            });

            model.Dates.Add(new SelectListItem
            {
                Text = "Two Weeks ago",
                Value = "2",
                Selected = rangeId == 2
            });

            model.Dates.Add(new SelectListItem
            {
                Text = "A month ago",
                Value = "3",
                Selected = rangeId == 3
            });

            model.Dates.Add(new SelectListItem
            {
                Text = "Custom Range",
                Value = "-1",
                Selected = rangeId == -1
            });

            ViewBag.Dates = model.Dates;

            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            if (rangeId == 1)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            else if (rangeId == 2)
            {
                startDate = DateTime.Now.AddDays(-14);
                endDate = DateTime.Now;
            }
            else if (rangeId == 3)
            {
                startDate = DateTime.Now.AddMonths(-1);
                endDate = DateTime.Now;
            }
            else
            {
                startDate = DateTime.Parse(_startDate);
                endDate = DateTime.Parse(_endDate).AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            model.StartDate = startDate.ToString("yyyy-MM-dd");
            model.EndDate = endDate.ToString("yyyy-MM-dd");
            model.TodaysDate = DateTime.Now.ToString("yyyy-MM-dd");

            var startDateUTC = startDate.ToUniversalTime().Ticks;
            var endDateUTC = endDate.ToUniversalTime().Ticks;

            MySqlCommand command4 = new MySqlCommand($"SELECT s.Amount, u.AccountNumber, s.IsOwnTransfer, s.Narration, s.TransferDate, s.TransferFromUserAccountId, s.TransferToUserAccountId, u.FirstName, u.LastName, u.AccountNumber FROM AccountStatements s LEFT JOIN UserAccounts u ON s.TransferFromUserAccountId = u.Id OR s.TransferToUserAccountId = u.Id WHERE (s.TransferFromUserAccountId = '{userAccId}' OR s.TransferToUserAccountId = '{userAccId}') AND u.AccountNumber = '{accountNumber}'", conn);
            MySqlDataReader dataReader4 = command4.ExecuteReader();

            var rate = 1;

            while (dataReader4.Read())
            {
                var transferDateUTC = dataReader4.GetDateTime("TransferDate").ToUniversalTime().Ticks;
                if (transferDateUTC >= startDateUTC && transferDateUTC <= endDateUTC)
                {
                    MySqlConnection newconn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
                    newconn.Open();

                    var newquery = $"USE {ApplicationConstants.DB};";
                    MySqlCommand newcommand = new MySqlCommand(newquery, newconn);
                    newcommand.ExecuteNonQuery();

                    var id = dataReader4.GetString("TransferFromUserAccountId");

                    newcommand = new MySqlCommand($"SELECT u.FirstName, u.LastName, u.AccountNumber FROM AccountStatements s LEFT JOIN UserAccounts u ON s.TransferFromUserAccountId = u.Id WHERE s.TransferFromUserAccountId = " + id, newconn);
                    MySqlDataReader newdataReader = newcommand.ExecuteReader();

                    var record = newdataReader.Read();
                    string _accNumber = newdataReader.GetString("AccountNumber");
                    string _firstName = newdataReader.GetString("FirstName");
                    string _lastName = newdataReader.GetString("LastName");

                    newdataReader.Close();

                    MySqlCommand newcommand2 = new MySqlCommand($"SELECT u.FirstName, u.LastName, u.AccountNumber FROM AccountStatements s LEFT JOIN UserAccounts u ON s.TransferToUserAccountId = u.Id WHERE s.TransferToUserAccountId = " + dataReader4.GetString("TransferToUserAccountId"), newconn);
                    MySqlDataReader newdataReader2 = newcommand2.ExecuteReader();

                    var record2 = newdataReader2.Read();
                    string _accNumber2 = newdataReader2.GetString("AccountNumber");
                    string _firstName2 = newdataReader2.GetString("FirstName");
                    string _lastName2 = newdataReader2.GetString("LastName");


                    var tmodel = new TransactionStatementModel
                    {
                        Amount = decimal.Round((decimal)(dataReader4.GetFloat("Amount") * rate), 2, MidpointRounding.AwayFromZero),
                        IsOwnTransfer = dataReader4.GetBoolean("IsOwnTransfer"),
                        Narration = dataReader4.GetString("Narration"),
                        TransferDate = dataReader4.GetDateTime("TransferDate").ToLocalTime(),
                        TransferFromUserAccountId = dataReader4.GetString("TransferFromUserAccountId"),
                        TransferToUserAccountId = dataReader4.GetString("TransferToUserAccountId"),
                        Sender = _firstName + " " + _lastName,
                        Receiver = _firstName2 + " " + _lastName2,
                        SenderAccountNumber = _accNumber,
                        ReceiverAccountNumber = _accNumber2,
                        TransactionType = dataReader4.GetString("TransferFromUserAccountId") == (string)Session[ApplicationConstants.Session_User_Account_Id] ? "Debit" : "Credit"
                    };

                    model.TransactionStatements.Add(tmodel);
                    newdataReader2.Close();
                    newconn.Close();
                }
            }

            dataReader4.Close();

            model.TransactionStatements = model.TransactionStatements.OrderByDescending(x => x.TransferDate.ToUniversalTime().Ticks).ToList();

            conn.Close();

            return View("TransactionStatements", model);
        }

        public ActionResult SwitchAccount(string uid)
        {
            Session[ApplicationConstants.Session_User_Account_Id] = uid;
            return RedirectToAction("Index");
        }

    }
}