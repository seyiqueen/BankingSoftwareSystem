using BankingSoftwareSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingSoftwareSystem.Controllers
{
    public class PortalController : Controller
    {
        // GET: Portal
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            Session.Clear();
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
                conn.Open();

                var query = $"USE {ApplicationConstants.DB};";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();

                string db_user_Id = string.Empty;
                string db_user_Account_Id = string.Empty;
                string db_userNameOrAccountNumber = string.Empty;
                string db_Password = string.Empty;

                if (model.IsPinMode)
                {
                    command = new MySqlCommand($"SELECT Id, UserId, AccountNumber, AccountPin FROM UserAccounts WHERE AccountNumber = '{model.AccountNumber}' AND AccountPin = '{model.PIN}'", conn);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        db_user_Account_Id = dataReader.GetString("Id");
                        db_user_Id = dataReader.GetString("UserId");
                        db_userNameOrAccountNumber = dataReader.GetString("AccountNumber");
                        db_Password = dataReader.GetString("AccountPin");
                    }

                    conn.Close();

                    if (model.Username != db_userNameOrAccountNumber && model.PIN != db_Password)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Credentials");
                        return View(model);
                    }
                    else
                    {
                        Session[ApplicationConstants.Session_User_Id] = db_user_Id;
                        Session[ApplicationConstants.Session_User_Account_Id] = db_user_Account_Id;
                        return RedirectToAction("Index", "App");
                    }
                }
                else
                {
                    command = new MySqlCommand($"SELECT * FROM Users WHERE Username = '{model.Username}' AND Password = '{model.Password}'", conn);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        db_user_Id = dataReader.GetString("Id");
                        db_userNameOrAccountNumber = dataReader.GetString("Username");
                        db_Password = dataReader.GetString("Password");
                    }

                    if (model.Username != db_userNameOrAccountNumber && model.Password != db_Password)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Username or Password");
                        return View(model);
                    }

                    dataReader.Close();

                    // lets sign the first account we can find for that user

                    MySqlCommand command2 = new MySqlCommand($"SELECT Id FROM UserAccounts WHERE UserId = '{db_user_Id}' LIMIT 1", conn);
                    MySqlDataReader dataReader2 = command2.ExecuteReader();

                    while (dataReader2.Read())
                    {
                        Session[ApplicationConstants.Session_User_Id] = db_user_Id;
                        Session[ApplicationConstants.Session_User_Account_Id] = dataReader2.GetString("Id");
                    }

                    conn.Close();

                    return RedirectToAction("Index", "App");
                }
            }
            catch (Exception e)
            {
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            MySqlConnection conn = new MySqlConnection(ApplicationConstants.DatabaseConnectionString);
            conn.Open();

            var query = $"USE {ApplicationConstants.DB};";
            MySqlCommand command = new MySqlCommand(query, conn);
            command.ExecuteNonQuery();

            // validate if account number is legit first

            command = new MySqlCommand($"SELECT * FROM UserAccounts WHERE AccountNumber = '{model.AccountNumber}'", conn);
            var dataReader = command.ExecuteReader();

            string user_id = string.Empty;

            while (dataReader.Read())
            {
                user_id = dataReader.GetString("Id");
            }

            if (string.IsNullOrWhiteSpace(user_id))
            {
                conn.Close();

                ModelState.AddModelError(string.Empty, "* Oops!, We cannot find this Account number with our Bank.");
                return View(model);
            }

            dataReader.Close();

            // go get a matching record with the input parameters

            MySqlCommand command4 = new MySqlCommand($"SELECT * FROM UserAccounts a " +
                                                     $"LEFT JOIN Users u ON u.Id = a.UserId " +
                                                     $"WHERE u.Username = '{model.Username}' AND " +
                                                     $"u.Password = '{model.Password}' AND " +
                                                     $"a.FirstName = '{model.FirstName}' AND " +
                                                     $"a.LastName = '{model.LastName}'", conn);

            MySqlDataReader dataReader4 = command4.ExecuteReader();

            string _acctNumber = string.Empty;
            string _username = string.Empty;
            if (dataReader4.HasRows)
            {
                // there is a user account
                while (dataReader4.Read())
                {
                    user_id = dataReader4.GetString("UserId");
                    _acctNumber = dataReader4.GetString("AccountNumber");
                    // check if input account number is mapped to registerd account 
                    if (_acctNumber == model.AccountNumber)
                    {
                        conn.Close();
                        ModelState.AddModelError(string.Empty, "* Oops!, This account number is already registered to this User Account.");
                        return View(model);
                    }
                }

                dataReader4.Close();

                // its a new account number to existing credentials, add a second account
                var command8 = new MySqlCommand($"UPDATE UserAccounts " +
                                                $"SET UserId = {user_id}, " +
                                                $"FirstName = '{model.FirstName}', " +
                                                $"LastName = '{model.LastName}', " +
                                                $"AccountPin = '{model.PIN}' " +
                                                $"WHERE AccountNumber = '{model.AccountNumber}'", conn);

                command8.ExecuteNonQuery();

                conn.Close();

                return RedirectToAction("Index", "App");
            }
            else
            {
                dataReader4.Close();
                conn.Close();
                ModelState.AddModelError(string.Empty, "* Oops!, This account number is already registered to this User Account.");
                return View(model);
                
            }
        }
    }
}