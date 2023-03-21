using BankingSoftwareSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Controllers
{
    public class WithdrawController : Controller
    {
        // GET: Deposit
        public ActionResult Index()

        {
            var userAccId = Session[ApplicationConstants.Session_User_Account_Id];
            if (userAccId == null)
                return RedirectToAction("Login", "Portal");

            var model = new WithdrawViewModel();

            var userId = Session[ApplicationConstants.Session_User_Id];

            MySqlConnection conn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
            conn.Open();

            var query = $"USE {ApplicationConstants.DB};";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.ExecuteNonQuery();

            command = new MySqlCommand($"SELECT u.Id, u.AccountNumber, u.Balance, a.Type, u.FirstName, u.LastName FROM UserAccounts u LEFT JOIN AccountTypes a ON a.Id = u.AccountTypeId WHERE u.UserId = '{userId}'", conn);
            MySqlDataReader dataReader = command.ExecuteReader();

            List<SelectListItem> LoggedInUserAccounts = new List<SelectListItem>();

            while (dataReader.Read())
            {
                LoggedInUserAccounts.Add(new SelectListItem
                {
                    Value = dataReader.GetString("Id"),
                    Text = dataReader.GetString("FirstName") + " " + dataReader.GetString("LastName") + " - " + dataReader.GetString("AccountNumber"),
                    Selected = dataReader.GetString("Id") == (string)userAccId,
                });

            }
            ViewBag.LoggedInUserAccounts = LoggedInUserAccounts;
            model.LoggedInUserAccounts = LoggedInUserAccounts;
            dataReader.Close();

            conn.Close();

            return View("Index", model);
        }
        public JsonResult GetUserData()
        {
            var userId = Session[ApplicationConstants.Session_User_Id];
            var userAccId = Session[ApplicationConstants.Session_User_Account_Id];

            MySqlConnection conn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
            conn.Open();

            var query = $"USE {ApplicationConstants.DB};";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.ExecuteNonQuery();

            command = new MySqlCommand($"SELECT * FROM UserAccounts WHERE UserId = '{userId}'", conn);
            MySqlDataReader dataReader = command.ExecuteReader();

            List<SelectListItem> LoggedInUserAccounts = new List<SelectListItem>();

            string fullName = string.Empty;
            float balance = 0;

            while (dataReader.Read())
            {
                LoggedInUserAccounts.Add(new SelectListItem
                {
                    Value = dataReader.GetString("AccountNumber"),
                    Text = dataReader.GetString("AccountNumber"),
                    Selected = dataReader.GetString("Id") == (string)userAccId,
                });

                if (dataReader.GetString("Id") == (string)userAccId)
                {
                    fullName = dataReader.GetString("FirstName") + " " + dataReader.GetString("LastName");
                    balance = dataReader.GetFloat("Balance");
                }
            }

            ViewBag.LoggedInUserAccounts = LoggedInUserAccounts;
            dataReader.Close();

            conn.Close();

            return Json(new
            {
                fullname = fullName,
                balance = balance
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SwitchAccount(string uid)
        {
            Session[ApplicationConstants.Session_User_Account_Id] = uid;
            return RedirectToAction("Index");
        }
        public ActionResult InitiateWithdraw(decimal amount)
        {
            MySqlConnection conn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
            conn.Open();

            var query = $"USE {ApplicationConstants.DB};";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.ExecuteNonQuery();

            var userAccId = Session[ApplicationConstants.Session_User_Account_Id];

            command = new MySqlCommand($"SELECT * FROM UserAccounts WHERE Id = '{userAccId}'", conn);
            var dataReader = command.ExecuteReader();

            decimal balance = 0m;

            while (dataReader.Read())
            {
                balance = dataReader.GetDecimal("Balance");
            }
            var newBalance = balance - amount;

            dataReader.Close();

            var command2 = new MySqlCommand($"UPDATE UserAccounts SET Balance = {newBalance} WHERE Id = '{userAccId}'", conn);
            command2.ExecuteNonQuery();
            var command3 = new MySqlCommand($"INSERT INTO AccountStatements (TransferToUserAccountId, TransferFromUserAccountId, Amount, IsOwnTransfer, TransferDate, Narration) " +
                                            $"VALUES ('{userAccId}', '{userAccId}', {amount}, 1, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', 'Withdraw')", conn);

            command3.ExecuteNonQuery();

            conn.Close();
            return RedirectToAction("Index");
        }
    }
}