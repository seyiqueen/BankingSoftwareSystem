using MySql.Data.MySqlClient;
using BankingSoftwareSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Controllers
{
    public class OutboundController : Controller
    {
        // GET: Outbound
        public ActionResult Index()
        {
            var userAccId = Session[ApplicationConstants.Session_User_Account_Id];
            if (userAccId == null)
                return RedirectToAction("Login", "Portal");

            var model = new OutboundTransferViewModel();

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
                    Text = dataReader.GetString("FirstName") + " " + dataReader.GetString("LastName") + " - " + dataReader.GetString("AccountNumber"),
                    Value = dataReader.GetString("Id"),
                    Selected = dataReader.GetString("Id") == (string)userAccId,
                });

                if (dataReader.GetString("Id") == (string)userAccId)
                {
                    model.CurrentBalance = dataReader.GetFloat("Balance");
                    model.AccountNumber = dataReader.GetString("AccountNumber");
                    model.AccountType = dataReader.GetString("Type");
                }
            }

            ViewBag.LoggedInUserAccounts = LoggedInUserAccounts;
            model.LoggedInUserAccounts = LoggedInUserAccounts;
            dataReader.Close();

            conn.Close();

            return View("OutboundTransfer", model);
        }

        public JsonResult VerifyAccountNumber(string accountNumber)
        {
            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                MySqlConnection conn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
                conn.Open();

                var query = $"USE {ApplicationConstants.DB};";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();

                command = new MySqlCommand($"SELECT u.Id, u.AccountNumber, u.FirstName, u.LastName, a.Type FROM UserAccounts u LEFT JOIN AccountTypes a ON u.AccountTypeId = a.Id WHERE AccountNumber = '{accountNumber}'", conn);
                MySqlDataReader dataReader = command.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    conn.Close();
                    return Json(new { error = "notfound" }, JsonRequestBehavior.AllowGet);
                }

                while (dataReader.Read())
                {
                    if (dataReader.GetString("Id") != (string)Session[ApplicationConstants.Session_User_Account_Id])
                    {

                        string _accountNumber = dataReader.GetString("AccountNumber");
                        string _lastName = dataReader.GetString("LastName");
                        string _firstName = dataReader.GetString("FirstName");
                        string _type = dataReader.GetString("Type");

                        conn.Close();
                        return Json(new
                        {
                            error = "",
                            receiverAccountNumber = _accountNumber,
                            receiverFullName = _firstName + " " + _lastName,
                            receiverAccountType = _type
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        conn.Close();

                        return Json(new { error = "same" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { error = "invalid" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TransferMoney(float amount, string accountNumber, string narration)
        {
            MySqlConnection conn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
            conn.Open();

            var query = $"USE {ApplicationConstants.DB};";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.ExecuteNonQuery();

            MySqlCommand command4 = new MySqlCommand($"SELECT AccountNumber, Balance FROM UserAccounts WHERE Id = '{Session[ApplicationConstants.Session_User_Account_Id]}'", conn);
            MySqlDataReader dataReader2 = command4.ExecuteReader();

            float senderBalance = 0;

            while (dataReader2.Read())
            {
                senderBalance = dataReader2.GetFloat("Balance");
            }

            dataReader2.Close();

            if (senderBalance < amount)
            {
                conn.Close();

                return Json(new
                {
                    success = false,
                    message = "Insufficient Funds to process this Transaction",
                }, JsonRequestBehavior.AllowGet);
            }

            MySqlCommand command2 = new MySqlCommand($"SELECT Id, AccountNumber, Balance FROM UserAccounts WHERE AccountNumber = '{accountNumber}'", conn);
            MySqlDataReader dataReader1 = command2.ExecuteReader();

            string receiverId = string.Empty;
            float receiverBalance = 0;

            while (dataReader1.Read())
            {
                receiverId = dataReader1.GetString("Id");
                receiverBalance = dataReader1.GetFloat("Balance");
            }

            dataReader1.Close();

            receiverBalance += amount;

            MySqlCommand command3 = new MySqlCommand($"UPDATE UserAccounts SET Balance = {receiverBalance} WHERE AccountNumber = '{accountNumber}'", conn);
            int rowCount = command3.ExecuteNonQuery();

            if (rowCount > 0)
            {
                senderBalance -= amount;

                MySqlCommand command5 = new MySqlCommand($"UPDATE UserAccounts SET Balance = {senderBalance} WHERE Id = '{Session[ApplicationConstants.Session_User_Account_Id]}'", conn);
                int rowCount3 = command5.ExecuteNonQuery();

                if (rowCount3 > 0)
                {
                    var todaysDate = DateTime.Now.ToUniversalTime();

                    MySqlCommand command6 = new MySqlCommand($"INSERT INTO AccountStatements (TransferToUserAccountId, TransferFromUserAccountId, Amount, IsOwnTransfer, TransferDate, Narration) VALUES ('{receiverId}', '{Session[ApplicationConstants.Session_User_Account_Id]}', {amount}, true, '{todaysDate.ToString("yyyy-MM-dd HH:mm:ss")}', '{narration}') ", conn);
                    int rowCount4 = command6.ExecuteNonQuery();

                    if (rowCount4 > 0)
                    {
                        conn.Close();

                        return Json(new
                        {
                            success = true,
                            message = "Transfer Successful",
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

            }

            conn.Close();

            return Json(new
            {
                success = false,
                message = "An error occured.",
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SwitchAccount(string uid)
        {
            Session[ApplicationConstants.Session_User_Account_Id] = uid;
            return RedirectToAction("Index");
        }
    }
}
