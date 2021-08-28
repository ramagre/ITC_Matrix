using ITC_Matrix.Common;
using ITC_Matrix.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace ITC_Matrix.Controllers
{

    public class LoginController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmm=new New_ITC_WMMEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Login
        public ActionResult CheckCountValue(int i)
        {
            if (i < 20)
            {
                //business logic goes here  
            }
            else
            {
                throw (new Exception("Out of the Range"));
            }

            return View();
        }
        public ActionResult Index()
        {
            int result = CheckWebConfigForInstall();

            if (result == -1)
            {
                return View("Error");
            }
            else if (result == 1)
            {
               return RedirectToAction("Install");
            }
            else if (result == 2)
            {
                return RedirectToAction("Setup");
            }
            else if (result == 3)
            {
                return RedirectToAction("Index");
            }
            else if (result == 4)
            {
                TempData["Message"] = "Invalid connection detail.";
                TempData["MessageClass"] = "error-msg";

                return RedirectToAction("Install");
            }
            
            List<Language> lstInstalledLanguages = GetInstalledLanguage();
            ViewBag.Lang = new MultiSelectList(lstInstalledLanguages, "Code", "DSCR");

            // Set default theme
            //string defaultTheme = Common.Functions.GetDefaultTheme();

            Common.Functions.SetDefaultTheme();

            if (Session["UserName"] != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        private int CheckWebConfigForInstall()
        {
            try
            {
                int result = 0;

                string ConnectionSetting = Common.Functions.GetConnectionSettings();

                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

                // if file not found and connection string has placeholders
                if (ConnectionSetting.Equals("1"))
                {
                    result = 1;
                }
                else
                {
                    bool isModify = true;

                    for (int i = 0; i < config.ConnectionStrings.ConnectionStrings.Count; i++)
                    {
                        if (config.ConnectionStrings.ConnectionStrings[i].Name == "New_ITC_WMMEntities" || config.ConnectionStrings.ConnectionStrings[i].Name == "New_ITC_MultiplanEntities")
                        {
                            string strConn = config.ConnectionStrings.ConnectionStrings[i].ConnectionString.ToString();

                            if (strConn.Contains("#HOSTNAME#") || strConn.Contains("#DBMULTIPLAN#") || strConn.Contains("#DBWMM#") ||
                               strConn.Contains("#USERID#") || strConn.Contains("#PASSWORD#"))
                            {
                                isModify = false;
                                break;
                            }
                        }
                    }

                    if (isModify == false)
                    {
                        string[] separators = { "*/*" };
                        string[] Settings = ConnectionSetting.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        // decrypt data                        
                        string HostName = Common.Functions.DecryptInURL(Settings[0],false);
                        string DBMultiplan = Common.Functions.DecryptInURL(Settings[1], false);
                        string DBWMM = Common.Functions.DecryptInURL(Settings[2], false);
                        string UserID = Common.Functions.DecryptInURL(Settings[3], false);
                        string Password = Common.Functions.DecryptInURL(Settings[4], false);

                        if (Connection(HostName, DBMultiplan, DBWMM, UserID, Password))
                        {
                            ModifyConnectionString(HostName, DBMultiplan, DBWMM, UserID, Password);

                            result = 2;
                        }
                        else
                        {
                            result = 4;
                        }
                    }
                    else
                    {
                        result = 0;     // all ok, redirect to login
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();

                logger.Error("Error occurred while processing install :", ex);

                return -1;
            }
        }

        /// <summary>
        /// Get Installed Language
        /// </summary>
        /// <returns></returns>
        private List<Language> GetInstalledLanguage()
        {
            return db.Languages.ToList();
        }

        #region
        /// <summary> Login
        /// 
        /// </summary>
        /// <param name="LOGIN"></param>
        /// <param name="PASSWORD"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string LOGIN, string PASSWORD, string Lang)
        {
            LOGIN = LOGIN.Trim();
            PASSWORD = PASSWORD.Trim();

            if (string.IsNullOrEmpty(LOGIN) && string.IsNullOrEmpty(PASSWORD))
            {
                ViewBag.lblError = "Please enter the username.";
                ViewBag.lblError1 = "Please enter the password.";
                ViewBag.Lang = new MultiSelectList(db.Languages.ToList(), "Code", "DSCR");
                return View();
            }

            if (string.IsNullOrEmpty(LOGIN))
            {
                ViewBag.lblError = "Please enter the username.";
                ViewBag.Lang = new MultiSelectList(db.Languages.ToList(), "Code", "DSCR");
                return View();
            }

            if (string.IsNullOrEmpty(PASSWORD))
            {
                ViewBag.lblError1 = "Please enter the password.";
                ViewBag.Lang = new MultiSelectList(db.Languages.ToList(), "Code", "DSCR");
                return View();
            }

            if (ModelState.IsValid) //This is Check Validity
            {
                var strdecpwd = db.Operators.Where(x => x.LOGIN.Equals(LOGIN)).SingleOrDefault();

                string decpwd = Common.Functions.SvcEncrypt(PASSWORD);
                var login = db.Operators.Where(x => x.LOGIN.Equals(LOGIN) && x.PASSWORD.Equals(decpwd)).FirstOrDefault();

                if (login != null)
                {
                    Session["UserName"] = LOGIN.ToString();
                    Session["Language"] = Lang;

                    ViewBag.message = "Loading...";
                    logger.Info("Login succeeded for " + LOGIN.ToString());

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {

                    ModelState.AddModelError("LogOnError", "The username or password provided is incorrect.");
                    ViewBag.Message = "Please enter the correct credentials.";
                    ViewBag.MessageClass = "error-msg";

                    List<Language> lstInstalledLanguages = GetInstalledLanguage();
                    ViewBag.Lang = new MultiSelectList(lstInstalledLanguages, "Code", "DSCR");
                    return View();
                }
            }

            return RedirectToAction("Index");
        }
        #endregion

        /// <summary>
        /// Log out from system
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            Session["UserName"] = null;
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Buffer = true;
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);
            Response.Expires = -1000;
            Response.CacheControl = "no-cache";
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.RemoveAll();
            System.Web.HttpContext.Current.Session.Abandon();
            System.Web.HttpContext.Current.Request.Cookies.Clear();
            Session["user"] = null;

            ViewBag.loggedOut = true;
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            TempData["message"] = null;

            if (string.IsNullOrEmpty(Email))
            {
                TempData["message"] = "Please enter email address.";
                TempData["MessageClass"] = "error-msg";
            }
            else
            {
                List<Operator> lstOperator = db.Operators.ToList().FindAll(x => x.Email.ToLower().Equals(Email.ToLower()));

                if (lstOperator.Count() == 0)
                {
                    TempData["message"] = "If your account exists in the system, you will receive an email shortly.";
                    TempData["MessageClass"] = "success-msg";
                }
                else
                {
                    string messagePath = Server.MapPath("~\\Mails\\");
                    messagePath = String.Concat(messagePath, "ForgotPassword.html");
                    string mailMessage = Common.Functions.ReadFile(messagePath);

                    int emailSent = SendUserEmail(mailMessage, Email.Trim(), lstOperator[0]);

                    if (emailSent == 0) //if mail send failed
                    {
                        TempData["message"] = string.Concat("Error sending email to ", Email);
                        TempData["MessageClass"] = "error-msg";
                    }
                    else if (emailSent == 1) // mail send successfully
                    {
                        TempData["message"] = "If your account exists in the system, you will receive an email shortly.";
                        TempData["MessageClass"] = "success-msg";
                    }
                    else
                    {
                        TempData["message"] = "Unable to recover your password information. Please try again later.";
                        TempData["MessageClass"] = "error-msg";
                    }
                }
            }

            ViewBag.lblErrorlogin = TempData["message"];
            ViewBag.MessageClass = TempData["MessageClass"];

            return View();
        }

        private int SendUserEmail(string mailMessage, string email, Operator operators)
        {
            string strFromAddress = ConfigurationManager.AppSettings["fromMailId"].ToString();
            string strToAddress = email;
            string strMessageSubject = "Forgot Password";
            string emailHeader = "Below is your password for Log in to the ITC Matrix:";
            string emailFooter = " Matrix Manager&#8482; ITC Systems. All rights reserved.";

            StringBuilder strBuilderMessageBody = new StringBuilder();

            //mailMessage = mailMessage.Replace("#ITCLogo#", String.Concat(ConfigurationManager.AppSettings["DomainURL"].ToString(), "/assets/images/logo.png"));
            mailMessage = mailMessage.Replace("#EmailBodyHeader#", emailHeader);
            mailMessage = mailMessage.Replace("#Name#", string.Concat(operators.FAMILY, " ", operators.GIVEN));
            mailMessage = mailMessage.Replace("#LoginLink#", ConfigurationManager.AppSettings["DomainURL"].ToString());
            mailMessage = mailMessage.Replace("#UserID#", email);
            mailMessage = mailMessage.Replace("#EmailBodyFooter#", emailFooter);

            mailMessage = mailMessage.Replace("#PasswordResetTime#", string.Concat(ConfigurationManager.AppSettings["PasswordResetTime"].ToString(), " minutes"));
            string resetPasswordPath = String.Concat(ConfigurationManager.AppSettings["DomainURL"].ToString(), "Login/ResetPassword/", Common.Functions.EncryptInURL(operators.LOGIN.ToString(), false), "-", Common.Functions.EncryptInURL(DateTime.Now.ToFileTime().ToString(), false));
            string resetPasswordURL = string.Concat("<a href='", resetPasswordPath, "' target ='_blank'>", resetPasswordPath, "</a>");
            mailMessage = mailMessage.Replace("#ResetPasswordURL#", resetPasswordURL);

            strBuilderMessageBody.Append(mailMessage);

            bool blnIsMailSent = false;

            blnIsMailSent = Common.Functions.SendMail(strFromAddress, strToAddress, strMessageSubject, strBuilderMessageBody.ToString());

            if (blnIsMailSent)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            // Set default theme
            Common.Functions.SetDefaultTheme();
            
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string id, string password, string ConfirmPassword)
        {
            if (password.Trim().Equals(string.Empty))
            {
                TempData["message"] = "Please enter password.";
                TempData["MessageClass"] = "error-msg";
            }
            else
            {
                ViewBag.Timeout = null;
                string login = string.Empty;
                string[] arrParameter;
                long timeStamp = 0;

                if (!string.IsNullOrEmpty(id))
                {
                    //reset URL contains userid and timestamp
                    arrParameter = id.Split('-');

                    if (arrParameter.Length == 2)
                    {
                        login = Common.Functions.DecryptInURL(arrParameter[0], false);

                        if (!string.IsNullOrEmpty(login))
                        {
                            if (!Int64.TryParse(Common.Functions.DecryptInURL(arrParameter[1], false), out timeStamp))
                            {
                                TempData["message"] = "Error in reset password.";
                                TempData["MessageClass"] = "error-msg";
                                return View();
                            }

                            DateTime resetTime = DateTime.FromFileTime(Convert.ToInt64(Common.Functions.DecryptInURL(arrParameter[1], false)));
                            TimeSpan timeDifference = DateTime.Now - resetTime;

                            int PasswordResetTime = 0;

                            if (ConfigurationManager.AppSettings["PasswordResetTime"] != null)
                            {
                                PasswordResetTime = Convert.ToInt32(ConfigurationManager.AppSettings["PasswordResetTime"].ToString());
                            }

                            if (timeDifference.TotalMinutes <= PasswordResetTime)      // The link is active for PasswordResetTime settings 
                            {
                                // Check password policy and display message accordingly ----------
                                if (CheckPaswordPolicy(password) == true)
                                {
                                    SavePassword(login, password, ConfirmPassword);
                                }
                            }
                            else
                            {
                                TimeSpan span = TimeSpan.FromMinutes(PasswordResetTime);

                                string message = "Sorry, the link to reset password has expired. Please retry to reset the password within #PasswordResetTime# minutes.";
                                message = message.Replace("#PasswordResetTime#", span.TotalMinutes.ToString());

                                TempData["message"] = message;
                                TempData["MessageClass"] = "error-msg";

                                ViewBag.ForgotLink = "ForgotPassword";
                                ViewBag.Timeout = "Timeout";
                            }
                        }
                    }
                    else
                    {
                        TempData["message"] = "Error in reset password.";
                        TempData["MessageClass"] = "error-msg";
                    }
                }
                else
                {
                    TempData["message"] = "Error in reset password.";
                    TempData["MessageClass"] = "error-msg";
                }
            }

            ViewBag.lblErrorlogin = TempData["message"];
            ViewBag.MessageClass = TempData["MessageClass"];

            return View();
        }

        /// <summary>
        /// Check Pasword Policy
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool CheckPaswordPolicy(string password)
        {
            string errorMessage = Common.Functions.CheckPasswordPolicy(password);

            if (!string.IsNullOrEmpty(errorMessage.Trim()))
            {
                TempData["message"] = errorMessage;
                TempData["MessageClass"] = "error-msg";

                return false;
            }

            return true;
        }

        /// <summary>
        /// saves new password
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        private void SavePassword(string login, string password, string ConfirmPassword)
        {
            if (!string.IsNullOrEmpty(login))
            {
                Operator operators = new Operator();
                operators = db.Operators.Find(login);

                operators.PASSWORD = Common.Functions.SvcEncrypt(password);
                operators.ConfirmPassword = Common.Functions.SvcEncrypt(ConfirmPassword);
                operators.OldPassword = operators.PASSWORD;

                if (ModelState.IsValid)
                {
                    db.Entry(operators).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    TempData["message"] = "Your password has been reset successfully.";
                    ViewBag.MessageClass = "success-msg";
                }
            }
        }

        public ActionResult Install(string HostName, string DBMultiplan, string DBWMM, string UserID, string Password, string Command)
        {
            if (!string.IsNullOrEmpty(Command))
            {
                if (Command.ToLower().Equals("save"))
                {
                    try
                    {
                        if (HostName != null && DBMultiplan != null && DBWMM != null && UserID != null && Password != null)
                        {
                            if (Connection(HostName, DBMultiplan, DBWMM, UserID, Password))
                            {
                                ModifyConnectionString(HostName, DBMultiplan, DBWMM, UserID, Password);  

                                WriteFileForUpgrade(HostName, DBMultiplan, DBWMM, UserID, Password);

                                return RedirectToAction("Setup");
                            }
                            else
                            {
                                TempData["Message"] = "Invalid connection detail.";
                                TempData["MessageClass"] = "error-msg";

                                ViewBag.Message = TempData["Message"];
                                ViewBag.MessageClass = TempData["MessageClass"];
                                return View();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = ex.ToString();

                        logger.Error("Error occurred while processing install :", ex);
                        return View("Error");
                    }
                }
                else if(Command.ToLower().Equals("test connection"))
                {
                    if (Connection(HostName, DBMultiplan, DBWMM, UserID, Password))
                    {
                        ViewBag.Message = "Connection successful.";
                        ViewBag.MessageClass = "success-msg";
                    }
                    else
                    {
                        ViewBag.Message = "Connection failed.";
                        ViewBag.MessageClass = "error-msg";                      
                    }

                    return View();
                }
            }

            return View();
        }

        private void ModifyConnectionString(string HostName, string DBMultiplan, string DBWMM, string UserID, string Password)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            for (int i = 0; i < config.ConnectionStrings.ConnectionStrings.Count; i++)
            {
                if (config.ConnectionStrings.ConnectionStrings[i].Name == Common.CommonEnum.DatabaseEntities.New_ITC_WMMEntities.ToString() || config.ConnectionStrings.ConnectionStrings[i].Name == Common.CommonEnum.DatabaseEntities.New_ITC_MultiplanEntities.ToString())
                {
                    string strConn = config.ConnectionStrings.ConnectionStrings[i].ConnectionString.ToString();
                    string strNewConn = string.Empty;
                    strNewConn = strConn.Replace("#HOSTNAME#",HostName);
                    strNewConn = strNewConn.Replace("#DBMULTIPLAN#", DBMultiplan);
                    strNewConn = strNewConn.Replace("#DBWMM#", DBWMM);
                    strNewConn = strNewConn.Replace("#USERID#", UserID);
                    strNewConn = strNewConn.Replace("#PASSWORD#", Password);
                    config.ConnectionStrings.ConnectionStrings[i].ConnectionString = strNewConn;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                }
            }
        }

        private void WriteFileForUpgrade(string HostName, string DBMultiplan, string DBWMM, string UserID, string Password)
        {
            string fileName = GlobalVariables.ConnectionSetting;
            string path = Server.MapPath("~/Assets/");

            if (!System.IO.Directory.Exists(System.IO.Path.Combine(path, "Install")))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(path, "Install"));
            }

            if (!System.IO.File.Exists(Server.MapPath(fileName)))
            {
                System.IO.File.WriteAllText(Server.MapPath(fileName), string.Concat(Common.Functions.EncryptInURL(HostName,false), "*/*", Common.Functions.EncryptInURL(DBMultiplan, false), "*/*", Common.Functions.EncryptInURL(DBWMM, false), "*/*", Common.Functions.EncryptInURL(UserID, false), "*/*", Common.Functions.EncryptInURL(Password, false)));
            }
        }

        private bool TestConnection(string connectionString)
        {
            bool result = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    if (connection.State == ConnectionState.Open)
                    {
                        result = true;
                    }

                    connection.Close();
                }
                catch (SqlException ex)
                {
                    result = false;

                    logger.Error("Error occurred while processing install :", ex);                   
                }
            }

            return result;
        }

        private bool Connection(string HostName, string DBMultiplan, string DBWMM, string UserID, string Password)
        {
            string connectionStringMultiplan = string.Concat("Data Source='" + HostName + "';Initial Catalog='" + DBMultiplan + "';User ID='" + UserID + "';Password='" + Password + "'");
            string connectionStringWMM = string.Concat("Data Source='" + HostName + "';Initial Catalog='" + DBWMM + "';User ID='" + UserID + "';Password='" + Password + "'");

            if (TestConnection(connectionStringMultiplan) && TestConnection(connectionStringWMM))
            {
                return true;
            }

            return false;
        }

        public ActionResult Setup()
        {
            int result = CheckWebConfigForInstall();

            if (result == -1)
            {
                return View("Error");
            }
            else if (result == 1)
            {
                return RedirectToAction("Install");
            }
            else if (result == 2)
            {
                return RedirectToAction("Setup");
            }
            else if (result == 3)
            {
                return RedirectToAction("Index");
            }
            else if (result == 4)
            {
                TempData["Message"] = "Invalid connection detail.";
                TempData["MessageClass"] = "error-msg";

                return RedirectToAction("Install");
            }

            ViewBag.SetupMessage = TempData["SetupMessage"];
            ViewBag.SetupMessageClass = TempData["SetupMessageClass"];

            ViewBag.masterRole= new MultiSelectList(db.RightGroups, "Code", "Dscr");
            ViewBag.masterUser = new MultiSelectList(db.Operators, "login", "family");

            return View();
        }

        [HttpPost]
        public ActionResult Setup(string matrixKey, string masterRole, string masterUser, string reportsFrom, string htmldocbaseurl, string htmldocpath, string htmldocpdfstore,
            string htmldocsiterootstore, string sessionName, string sessionIdle)
        {
            try
            {
                SaveSetting("matrixKey", matrixKey, matrixKey);

                SaveSetting("masterRole", masterRole, masterRole);

                SaveSetting("masterUser", masterUser, masterUser);

                SaveSetting("reportsFrom", reportsFrom, reportsFrom);

                SaveSetting("htmldocbaseurl", htmldocbaseurl, "http://localhost:82/");

                SaveSetting("htmldocpath", htmldocpath, "/reportsStorage/");

                SaveSetting("htmldocpdfstore", htmldocpdfstore, "/reportsStorage/");

                SaveSetting("htmldocsiterootstore", htmldocsiterootstore, "C://inetpub/MatrixManager/");

                SaveSetting("sessionName", sessionName, "matrix");

                SaveSetting("sessionIdle", sessionIdle, "180");

                SaveSetting("reportsPath", GlobalVariables.reportsPath, GlobalVariables.reportsPath);

                return RedirectToAction("Index");       // redirect to login page                
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing setup :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }           
        }
        
        private void SaveSetting(string name, string value, string defaultValue)
        {
            pref objpref = new pref();

            objpref = dbwmm.prefs.ToList().FindAll(x => x.name.ToLower().Equals(name.ToLower())).FirstOrDefault();

            if(string.IsNullOrEmpty(value))
            {
                value = defaultValue;            
            }

            if (objpref == null)
            {
                objpref = new pref();

                objpref.name = name;
                objpref.value = value;

                dbwmm.prefs.Add(objpref);
                dbwmm.SaveChanges();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    objpref.value = value;

                    dbwmm.Entry(objpref).State = EntityState.Modified;
                    dbwmm.SaveChanges();
                }
            }
        }
    }
}