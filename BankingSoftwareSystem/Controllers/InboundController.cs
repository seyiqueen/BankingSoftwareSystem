using BankingSoftwareSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Controllers
{
    public class InboundController : Controller
    {
        // GET: Inbound
        public ActionResult Index()
        {
            var userAccId = Session[ApplicationConstants.Session_User_Account_Id];
            if (userAccId == null)
                return RedirectToAction("Login", "Portal");

            var model = new InboundTransferViewModel();

            var userId = Session[ApplicationConstants.Session_User_Id];

            MySqlConnection conn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
            conn.Open();

            var query = $"USE {ApplicationConstants.DB};";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.ExecuteNonQuery();

            command = new MySqlCommand($"SELECT u.Id, u.AccountNumber, u.Balance, a.Type, u.FirstName, u.LastName FROM UserAccounts u LEFT JOIN AccountTypes a ON a.Id = u.AccountTypeId WHERE u.UserId = '{userId}'", conn);
            MySqlDataReader dataReader = command.ExecuteReader();

            List<SelectListItem> LoggedInUserAccounts = new List<SelectListItem>();

            model.TransferAbleToAccounts.Add(new SelectListItem
            {
                Text = "-- Please Select --",
                Value = "-1",
            });

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
                    model.CurrentBalance = dataReader.GetFloat("Balance");
                    model.AccountNumber = dataReader.GetString("AccountNumber");
                    model.AccountType = dataReader.GetString("Type");
                }
                else
                {
                    model.TransferAbleToAccounts.Add(new SelectListItem
                    {
                        Text = dataReader.GetString("FirstName") + " " + dataReader.GetString("LastName") + " - " + dataReader.GetString("AccountNumber"),
                        Value = dataReader.GetString("AccountNumber"),
                    });
                }
            }

            ViewBag.TransferAbleToAccounts = model.TransferAbleToAccounts;
            ViewBag.LoggedInUserAccounts = LoggedInUserAccounts;
            model.LoggedInUserAccounts = LoggedInUserAccounts;
            dataReader.Close();

            conn.Close();

            return View("InboundTransfer", model);
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
                    message = "Insufficient Funds to Transfer this Amount.",
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