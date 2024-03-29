﻿using BankingSoftwareSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Controllers
{
    public class AppController : Controller
    {
        // GET: App
        public ActionResult Index()
        {
            var userAccId = Session[ApplicationConstants.Session_User_Account_Id];
            if (userAccId == null)
                return RedirectToAction("Login", "Portal");

            var model = new DashboardViewModel();

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
                    model.FirstName = dataReader.GetString("FirstName");
                    model.LastName = dataReader.GetString("LastName");
                    fullName = dataReader.GetString("FirstName") + " " + dataReader.GetString("LastName");
                }
            }

            ViewBag.LoggedInUserAccounts = LoggedInUserAccounts;
            model.LoggedInUserAccounts = LoggedInUserAccounts;
            dataReader.Close();

            MySqlCommand command2 = new MySqlCommand($"SELECT * FROM AccountStatements WHERE TransferFromUserAccountId = '{userAccId}' OR TransferToUserAccountId = '{userAccId}'", conn);
            MySqlDataReader dataReader2 = command2.ExecuteReader();

            List<TransactionStatementModel> TransactionStatements = new List<TransactionStatementModel>();

            while (dataReader2.Read())
            {
                TransactionStatements.Add(new TransactionStatementModel
                {
                    TransferToUserAccountId = dataReader2.GetString("TransferToUserAccountId"),
                    Amount = (decimal)dataReader2.GetFloat("Amount"),
                    IsOwnTransfer = dataReader2.GetBoolean("IsOwnTransfer"),
                    Narration = dataReader2.GetString("Narration"),
                    TransferDate = dataReader2.GetDateTime("TransferDate"),
                    TransferFromUserAccountId = dataReader2.GetString("TransferFromUserAccountId")
                });
            }

            dataReader2.Close();

            List<int> records = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                var data = TransactionStatements.Where(x => x.TransferDate.Month == i).ToList();
                records.Add(data.Count);
            }

            conn.Close();

            ViewBag.GraphData = string.Join(",", records);

            return View("Dashboard", model);
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
    }
}