using BankingSoftwareSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Controllers
{
    public class AccountDetailsController : Controller
    {
        // GET: AccountDetails
        public ActionResult Index()
        {
            var userAccId = Session[ApplicationConstants.Session_User_Account_Id];
            if (userAccId == null)
                return RedirectToAction("Login", "Portal");

            var model = new AccountDetailsViewModel();

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
                    model.FirstName = dataReader.GetString("FirstName");
                    model.LastName = dataReader.GetString("LastName");
                }
            }

            ViewBag.LoggedInUserAccounts = LoggedInUserAccounts;
            model.LoggedInUserAccounts = LoggedInUserAccounts;
            dataReader.Close();

            conn.Close();

            return View("AccountDetails", model);
        }
    }

}
