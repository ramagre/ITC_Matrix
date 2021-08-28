using ITC_Matrix.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ITC_Matrix.Common
{
    public static class Functions
    {
        private static TripleDESCryptoServiceProvider cryptDES3 = new TripleDESCryptoServiceProvider();

        private static MD5CryptoServiceProvider cryptMD5Hash = new MD5CryptoServiceProvider();

        /// <summary>
        /// Get page size from prefs table
        /// </summary>
        /// <returns></returns>
        public static int GetPageSize()
        {
            New_ITC_WMMEntities dbMatrix = new New_ITC_WMMEntities();
            int pageSize = 10;

            List<pref> lstpref = dbMatrix.prefs.ToList().FindAll(x => x.name.ToLower().Equals("maxrows"));

            if (lstpref.Count > 0)
            {
                if (!string.IsNullOrEmpty(lstpref[0].value))
                {
                    bool result = Int32.TryParse(lstpref[0].value, out pageSize);

                    if (pageSize == 0)
                    {
                        pageSize = 10;
                    }
                }
            }

            return pageSize;
        }

        internal static int GetSessionTimeout()
        {
            New_ITC_WMMEntities dbwmm = new New_ITC_WMMEntities();
            var sessionTimeout = dbwmm.prefs.Where(x => x.name == "sessionIdle").Select(y => y.value).SingleOrDefault();
            int timeOut = 180;

            if (sessionTimeout != null)
            {
                timeOut = Convert.ToInt32(sessionTimeout);
            }

            return timeOut;
        }

        /// <summary>
        /// Convert To Hexadecimal number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToHex(string input)
        {
            StringBuilder output = new StringBuilder();
            char[] values = input.ToCharArray();

            foreach (char letter in values)
            {
                int value = Convert.ToInt32(letter);
                string hexOutput = String.Format("{0:X}", value);
                output.Append(hexOutput);
            }

            return output.ToString();
        }

        /// <summary>
        /// decrypts data
        /// </summary>
        /// <param name="Secret"></param>
        /// <param name="decKey"></param>
        /// <returns></returns>
        public static string svcDecrypt(string Secret)
        {
            string decKey = "PIN:";

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PasswordKey"]))
            {
                decKey = ConfigurationManager.AppSettings["PasswordKey"].ToString();
            }

            int iChar = 0;
            int iVal = 0;
            string decPass = string.Empty;
            int i = 0;
            int a = 0;
            i = 0;
            a = 0;

            for (i = 1; i <= Secret.Length; i++)
            {
                if ((a == decKey.Length))
                {
                    a = 0;
                }

                if (((i - 1) % 2 == 0))
                {
                    iChar = Convert.ToInt32(Secret.Substring(i - 1, 2), 16);
                    iVal = Convert.ToInt32(decKey.Substring(a + 1 - 1, 2), 16);
                    decPass = decPass + (char)(iChar ^ iVal);       // XOR
                }

                a = a + 1;
            }

            return decPass;
        }

        /// <summary>
        /// Check menu permission
        /// </summary>
        /// <param name="lstprefs"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static bool CheckMenuPermission(List<pref> lstprefs, string moduleName, string modulePermission = "")
        {
            New_ITC_WMMEntities dbwmm = new New_ITC_WMMEntities();

            bool isPermission = true;
            lstprefs = lstprefs.FindAll(x => x.name.ToLower().Equals("globalmenus"));

            if (lstprefs.FindAll(x => x.value.ToLower().Contains(moduleName.ToLower())).Count == 0)
            {
                isPermission = false;
                return isPermission;
            }
            else
            {
                if (!string.IsNullOrEmpty(modulePermission))
                {
                    // check module permission
                    List<int> roleList = new List<int>();
                    string userid = string.Empty;

                    if (Common.Functions.CheckSession("UserName"))
                    {
                        userid = HttpContext.Current.Session["UserName"].ToString();
                    }

                    if (!string.IsNullOrEmpty(userid))
                    {
                        roleList = dbwmm.permissionsAssigneds.ToList().FindAll(x => x.userid.ToLower().Equals(userid)).Select(y => y.roleid).ToList();
                    }

                    if (roleList != null)
                    {
                        var permittedList = dbwmm.permissionsRolesTasks
                                           .Where(m => roleList.Contains(m.roleid))
                                           .Select(x => x.taskid)
                                           .ToList();
                        if (permittedList != null)
                        {
                            var lstTaskPermission = dbwmm.permissionsTasks
                                                  .Where(p => permittedList
                                                  .Contains(p.taskid) && p.taskref.ToLower().Equals(modulePermission.ToLower()))
                                                  .ToList();
                            if (lstTaskPermission != null)
                            {
                                if (lstTaskPermission.Count == 0)
                                {
                                    isPermission = false;
                                    return isPermission;
                                }
                            }

                            //CheckMenuAccessPermission(moduleName, modulePermission);
                        }
                    }
                }
            }

            return isPermission;
        }

        //private static void CheckMenuAccessPermission(string moduleName, string modulePermission)
        //{
        //    throw new NotImplementedException();
        //}

        //internal static bool CheckDeletePermission(List<pref> lstprefs, string moduleName, string modulePermission = "")
        //{
        //    New_ITC_WMMEntities dbwmm = new New_ITC_WMMEntities();

        //    bool isPermission = true;
        //    lstprefs = lstprefs.FindAll(x => x.name.ToLower().Equals("globalmenus"));

        //    if (lstprefs.FindAll(x => x.value.ToLower().Contains(moduleName.ToLower())).Count > 0)
        //    {                
        //        if (!string.IsNullOrEmpty(modulePermission))
        //        {
        //            // check module permission

        //            List<int> roleList = new List<int>();
        //            string userid = string.Empty;

        //            if (Common.Functions.CheckSession("UserName"))
        //            {
        //                userid = HttpContext.Current.Session["UserName"].ToString();
        //            }

        //            if (!string.IsNullOrEmpty(userid))
        //            {
        //                roleList = dbwmm.permissionsAssigneds.ToList().FindAll(x => x.userid.ToLower().Equals(userid)).Select(y => y.roleid).ToList();
        //            }

        //            if (roleList != null)
        //            {
        //                var permittedList = dbwmm.permissionsRolesTasks
        //                                   .Where(m => roleList.Contains(m.roleid))
        //                                   .Select(x => x.taskid)
        //                                   .ToList();
        //                if (permittedList != null)
        //                {
        //                    var lstTaskPermission = dbwmm.permissionsTasks
        //                                          .Where(p => permittedList
        //                                          .Contains(p.taskid) && p.taskref.ToLower().Equals(modulePermission.ToLower()))
        //                                          .ToList();
        //                    if (lstTaskPermission != null)
        //                    {
        //                        if (lstTaskPermission.Count == 0)
        //                        {
        //                            isPermission = false;                                   
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return isPermission;
        //}

        /// <summary>
        /// Encrypts data
        /// </summary>
        /// <param name="Secret"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string SvcEncrypt(string Secret)
        {
            string password = "PIN:";

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PasswordKey"]))
            {
                password = ConfigurationManager.AppSettings["PasswordKey"].ToString();
            }

            string sResponse = string.Empty;
            StringBuilder sResponsesb = new StringBuilder();
            int lSecretLen = 0;
            int lPwdLen = 0;
            byte ucTemp = 0;
            byte ucTemp123 = 0;
            int intuctemp = 0;
            int lTemp = 0;
            int i = 0;
            lSecretLen = Secret.Length;
            lPwdLen = password.Length;
            string str = string.Empty;
            char pad = '0';
            byte[] temp1;
            byte[] temp2;
            sResponse = str.PadLeft(lSecretLen * 2, pad);
            StringBuilder sb = new StringBuilder();
            sb.Capacity = sResponse.Length;

            for (i = 1; i <= lSecretLen; i++)
            {
                lTemp = i % lPwdLen;
                temp1 = Encoding.ASCII.GetBytes(password.Substring((lTemp + (lPwdLen * (lTemp == 0 ? 1 : 0))) - 1, 1));
                temp2 = Encoding.ASCII.GetBytes(Secret.Substring(i - 1, 1));
                ucTemp = temp1[0];
                ucTemp123 = temp2[0];
                intuctemp = (int)ucTemp ^ (int)ucTemp123;   // XOR
                string appstring = string.Empty;

                if (intuctemp.ToString("X").Length == 1)
                {
                    appstring = "0" + intuctemp.ToString("X");
                }
                else
                {
                    appstring = intuctemp.ToString("X");
                }

                sb.Append(appstring);
            }

            string theString = sb.ToString();
            return theString;
        }

        /// <summary>
        /// Check Password Policy
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string CheckPasswordPolicy(string password, string pin = "")
        {
            string passwordOrPin = "Password";

            if (!pin.Equals(string.Empty))
            {
                passwordOrPin = "Pin";
            }

            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
            string errorMessage = string.Empty;
            string lengthError = string.Empty;

            // Check Password Policy
            int minPasswordLength = 4;
            int maxPasswordLength = 20;
            int isNumbersOnly = 0;
            bool result = false;
            int isUpperLowerCaseReq = 0;
            int numericPassword = 0;
            int isSpecialCharacters = 0;
            bool isFoundSpecialCharacters = true;

            List<Param> lstParam = db.Params.ToList().FindAll(x => x.Section.ToLower().Equals("web"));

            #region ---------- Check password minimum/maximum length ----------

            lstParam = db.Params.ToList().FindAll(x => x.Name.ToLower().Equals("pinmin"));

            if (lstParam.Count > 0)
            {
                if (!string.IsNullOrEmpty(lstParam[0].Value))
                {
                    Int32.TryParse(lstParam[0].Value, out minPasswordLength);

                    if (minPasswordLength == 0)
                    {
                        minPasswordLength = 4;
                    }
                }
            }

            lstParam = db.Params.ToList().FindAll(x => x.Name.ToLower().Equals("pinmax"));

            if (lstParam.Count > 0)
            {
                if (!string.IsNullOrEmpty(lstParam[0].Value))
                {
                    Int32.TryParse(lstParam[0].Value, out maxPasswordLength);

                    if (maxPasswordLength == 0 || maxPasswordLength > 20)
                    {
                        maxPasswordLength = 20;
                    }
                }
            }

            if (password.Length < minPasswordLength || password.Length > maxPasswordLength)
            {
                lengthError = string.Concat(passwordOrPin, " must be between ", minPasswordLength, " - ", maxPasswordLength, " characters");
            }

            #endregion

            #region ---------- Check Numbers only ----------

            lstParam = db.Params.ToList().FindAll(x => x.Name.ToLower().Equals("numberpin"));
            if (lstParam.Count > 0)
            {
                if (!string.IsNullOrEmpty(lstParam[0].Value))
                {
                    Int32.TryParse(lstParam[0].Value, out isNumbersOnly);
                }
            }

            result = Int32.TryParse(password, out numericPassword);

            if (isNumbersOnly == 1 && result == false)
            {
                if (lengthError.Equals(string.Empty))
                {
                    lengthError = string.Concat(passwordOrPin, " must be between ", minPasswordLength, " - ", maxPasswordLength, " characters");
                }

                errorMessage = string.Concat(lengthError, ", and should contain only numbers.");

                return errorMessage;        // return message because, if numbers only then other settings would not be applied.
            }

            #endregion

            #region ---------- Upper Lower Case ----------

            lstParam = db.Params.ToList().FindAll(x => x.Name.ToLower().Equals("upperlowercasereq"));

            if (lstParam.Count > 0)
            {
                if (!string.IsNullOrEmpty(lstParam[0].Value))
                {
                    Int32.TryParse(lstParam[0].Value, out isUpperLowerCaseReq);
                }
            }

            if (isUpperLowerCaseReq == 1 && (password.Any(l => char.IsLower(l)) == false || password.Any(u => char.IsUpper(u)) == false))
            {
                if (lengthError.Equals(string.Empty))
                {
                    lengthError = string.Concat(passwordOrPin, " must be between ", minPasswordLength, " - ", maxPasswordLength, " characters");
                }

                errorMessage = string.Concat(lengthError, ", should contain at least one upper and lower case letter");

                //return errorMessage;
            }

            #endregion

            #region ---------- Alpha Numeric Required ----------

            lstParam = db.Params.ToList().FindAll(x => x.Name.ToLower().Equals("numericreqpin"));

            if (lstParam.Count > 0)
            {
                if (!string.IsNullOrEmpty(lstParam[0].Value))
                {
                    Int32.TryParse(lstParam[0].Value, out numericPassword);
                }
            }

            if (numericPassword == 1 && password.Any(n => char.IsNumber(n)) == false)
            {
                if (errorMessage.Equals(string.Empty))
                {
                    if (lengthError.Equals(string.Empty))
                    {
                        lengthError = string.Concat(passwordOrPin, " must be between ", minPasswordLength, " - ", maxPasswordLength, " characters");
                    }

                    errorMessage = string.Concat(lengthError, ", should contain at least one number");
                }
                else
                {
                    errorMessage = string.Concat(errorMessage, ", should contain at least one number");
                }

                //return errorMessage;
            }

            #endregion

            #region ---------- Special Character ----------
            Match match = Regex.Match(password, "[^a-z0-9]", RegexOptions.IgnoreCase);

            if (!match.Success)     // if special characters not found
            {
                isFoundSpecialCharacters = false;
            }

            lstParam = db.Params.ToList().FindAll(x => x.Name.ToLower().Equals("specialcharreq"));

            if (lstParam.Count > 0)
            {
                if (!string.IsNullOrEmpty(lstParam[0].Value))
                {
                    Int32.TryParse(lstParam[0].Value, out isSpecialCharacters);
                }
            }

            if (isSpecialCharacters == 1 && isFoundSpecialCharacters == false)
            {
                if (errorMessage.Equals(string.Empty))
                {
                    if (lengthError.Equals(string.Empty))
                    {
                        lengthError = string.Concat(passwordOrPin, " must be between ", minPasswordLength, " - ", maxPasswordLength, " characters");
                    }

                    errorMessage = string.Concat(lengthError, ", should contain at least one special character");
                }
                else
                {
                    errorMessage = string.Concat(errorMessage, ", should contain at least one special character.");
                }

                //return errorMessage;
            }

            #endregion

            if (errorMessage.Equals(string.Empty))
            {
                errorMessage = lengthError;
            }

            return errorMessage;
        }

        /// <summary>
        /// Checks session
        /// </summary>
        /// <param name="sessionName"></param>
        /// <returns></returns>
        internal static bool CheckSession(string sessionName)
        {
            if (!string.IsNullOrEmpty((string)HttpContext.Current.Session[sessionName]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// reads file contents
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>file contents</returns>
        public static string ReadFile(string path)
        {
            //Read the File Contents.

            StreamReader streamreader = new StreamReader(path);
            string text, stream;
            text = string.Empty;
            stream = streamreader.ReadToEnd();

            if (stream != null)
            {
                text = stream;
            }

            streamreader.Close();
            return text;
        }

        /// <summary>
        /// Parses the integer.
        /// </summary>
        /// <param name="intValue">The int value.</param>
        /// <returns></returns>
        public static int ParseInteger(string intValue)
        {
            int intVar = 0;
            int result = 0;

            if (!string.IsNullOrEmpty(intValue))
            {
                if (int.TryParse(intValue, out intVar))
                {
                    result = Convert.ToInt32(intValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Encrypts the specified to encrypt.s
        /// </summary>
        /// <param name="toEncrypt">To encrypt.</param>
        /// <param name="useHashing">if set to <c>true</c> [use hashing].</param>
        /// <returns></returns>
        public static string EncryptInURL(string toEncrypt, bool useHashing)
        {
            cryptDES3.Key = cryptMD5Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["SecurityKey"].ToString()));
            cryptDES3.Mode = CipherMode.ECB;
            ICryptoTransform desdencrypt = cryptDES3.CreateEncryptor();
            byte[] buff = ASCIIEncoding.ASCII.GetBytes(toEncrypt);
            string Encrypt = Convert.ToBase64String(desdencrypt.TransformFinalBlock(buff, 0, buff.Length));
            Encrypt = Encrypt.Replace("+", "!");
            Encrypt = Encrypt.Replace("/", "_");

            return Encrypt;
        }

        /// <summary>
        /// Decrypts the specified cipher string.
        /// </summary>
        /// <param name="cipherString">The cipher string.</param>
        /// <param name="useHashing">if set to <c>true</c> [use hashing].</param>
        /// <returns></returns>
        public static string DecryptInURL(string cipherString, bool useHashing)
        {
            try
            {
                cipherString = cipherString.Replace("!", "+");
                cipherString = cipherString.Replace("_", "/");
                byte[] buf = new byte[cipherString.Length];
                cryptDES3.Key = cryptMD5Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["SecurityKey"].ToString()));
                cryptDES3.Mode = CipherMode.ECB;
                ICryptoTransform desdencrypt = cryptDES3.CreateDecryptor();
                buf = Convert.FromBase64String(cipherString);
                string Decrypt = ASCIIEncoding.ASCII.GetString(desdencrypt.TransformFinalBlock(buf, 0, buf.Length));
                return Decrypt;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// sends mail 
        /// </summary>
        /// <param name="emailFrom"></param>
        /// <param name="emailTo"></param>
        /// <param name="subject"></param>
        /// <param name="messageBody"></param>
        /// <returns></returns>
        public static bool SendMail(string emailFrom, string emailTo, string subject, string messageBody)
        {
            try
            {
                MailMessage messsage;
                SmtpClient smtpClient;

                messsage = new MailMessage();
                messsage.Subject = subject;
                messsage.From = new MailAddress(emailFrom);
                smtpClient = new SmtpClient(ConfigurationManager.AppSettings["smtpClient"]);
                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                MailAddress mailAddress = new MailAddress(emailTo);
                messsage.To.Clear();
                messsage.To.Add(mailAddress);

                // Create the mailing List 

                messsage.Body = messageBody;
                System.IO.Stream stream = new System.IO.MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(messsage.Body));
                AlternateView alternate = new AlternateView(stream, new System.Net.Mime.ContentType("text/html"));
                messsage.AlternateViews.Add(alternate);

                smtpClient.Send(messsage);
                return true;
            }
            catch (SmtpException ex)
            {
                string message = string.Concat(ex.Message, "\r\n", ex.StackTrace);
                return false;
            }
        }

        public static int GetDBStructure()
        {
            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
            int db_structure = 15;

            List<Param> lstParam = db.Params.ToList().FindAll(x => x.Name.ToLower().Equals("db_structure"));

            if (lstParam.Count > 0)
            {
                if (!string.IsNullOrEmpty(lstParam[0].Value))
                {
                    bool result = Int32.TryParse(lstParam[0].Value, out db_structure);

                    if (db_structure == 0)
                    {
                        db_structure = 10;
                    }
                }
            }

            return db_structure;
        }

        internal static void SetDefaultTheme()
        {
            New_ITC_WMMEntities dbwmm = new New_ITC_WMMEntities();
            pref objpref = dbwmm.prefs.ToList().FindAll(x => x.name.ToLower().Equals("skincss")).FirstOrDefault();

            string defaultTheme = string.Empty;

            if (objpref != null)
            {
                if (Convert.ToInt32(objpref.value) > 0)
                {
                    theme objtheme = dbwmm.themes.ToList().FindAll(x => x.id == Convert.ToInt32(objpref.value)).FirstOrDefault();

                    if (objtheme != null)
                    {
                        string defaultFilePath = string.Concat(HttpContext.Current.Server.MapPath("~/_themes/"), objtheme.themename, "/default-theme.css");

                        if (System.IO.File.Exists(defaultFilePath))
                        {
                            defaultTheme = string.Concat("/_themes/", objtheme.themename, "/default-theme.css");
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(defaultTheme))
            {
              HttpContext.Current.Session["DefaultTheme"] = defaultTheme;
            }
            else
            {
                HttpContext.Current.Session["DefaultTheme"] = null;
            }            
        }

        // user define function for gettin the profiles from the operator roles
        internal static List<Profile> GetPermittedProfiles()
        {
            New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();

            List<Profile> lstProfile = new List<Profile>();

            string userid = string.Empty;
            List<int> roleList = new List<int>();

            if (Common.Functions.CheckSession("UserName"))
            {
                userid = HttpContext.Current.Session["UserName"].ToString();
            }

            if (!string.IsNullOrEmpty(userid))
            {
                roleList = dbwmn.permissionsAssigneds.ToList().FindAll(x => x.userid.ToLower().Equals(userid)).Select(y => y.roleid).ToList();
            }

            if (roleList != null)
            {
                var permittedList = dbwmn.permissionsGrants
                .Where(m => m.ptype.ToLower().Equals("profile") && roleList.Contains(m.roleid))
                .Select(x => x.pid)
                .ToList();

                lstProfile = db.Profiles
                .Where(p => permittedList
                           .Contains(p.CODE))
                .ToList();
            }

            return lstProfile;
        }

        // User define function for getting the pay method type from operator roles.
        internal static List<PayMethod> GetPermittedPayMethods()
        {
            New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();

            List<PayMethod> lstPayMethod = new List<PayMethod>();

            string userid = string.Empty;
            List<int> roleList = new List<int>();

            if (Common.Functions.CheckSession("UserName"))
            {
                userid = HttpContext.Current.Session["UserName"].ToString();
            }

            if (!string.IsNullOrEmpty(userid))
            {
                roleList = dbwmn.permissionsAssigneds.ToList().FindAll(x => x.userid.ToLower().Equals(userid)).Select(y => y.roleid).ToList();
            }

            if (roleList != null)
            {
                var permittedList = dbwmn.permissionsGrants
                .Where(m => m.ptype.ToLower().Equals("paymethod") && roleList.Contains(m.roleid))
                .Select(x => x.pid)
                .ToList();

                lstPayMethod = db.PayMethods
                .Where(p => permittedList
                           .Contains(p.CODE))
                .ToList();
            }

            return lstPayMethod;
        }

        // User define function for getting the permitted device type from operator roles.
        internal static List<RegisterGroup> GetPermittedDevice_Group()
        {

            New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();

            List<RegisterGroup> lstRegister = new List<RegisterGroup>();

            string userid = string.Empty;
            List<int> roleList = new List<int>();

            if (Common.Functions.CheckSession("UserName"))
            {
                userid = HttpContext.Current.Session["UserName"].ToString();
            }

            if (!string.IsNullOrEmpty(userid))
            {
                roleList = dbwmn.permissionsAssigneds.ToList().FindAll(x => x.userid.ToLower().Equals(userid)).Select(y => y.roleid).ToList();
            }

            if (roleList != null)
            {
                var permittedList = dbwmn.permissionsGrants
                .Where(m => m.ptype.ToLower().Equals("devgroup") && roleList.Contains(m.roleid))
                .Select(x => x.pid)
                .ToList();

                lstRegister = db.RegisterGroups.Where(x => permittedList.Contains(x.CODE)).ToList();
            }
            return lstRegister;
           }

        public static string GetTranslation(int MessageID)
        {
            int languageID = 0;

            if (CheckSession("Language"))
            {
                languageID = Convert.ToInt32(HttpContext.Current.Session["Language"]);
            }

            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();

            List<CommMessage> lstCommMessage = new List<CommMessage>();

            lstCommMessage = db.CommMessages.ToList().FindAll(x => x.MsgGroup.ToLower().Equals(CommonEnum.MessageGroup.WebLabel.ToString().ToLower()));

            string translation = string.Empty;
            CommMessage objCommMessage = new CommMessage();

            if (lstCommMessage.Count > 0)
            {
                objCommMessage = lstCommMessage.FindAll(x => x.MessageID == MessageID && x.Language == languageID).FirstOrDefault();

                if (objCommMessage != null)
                {
                    if (objCommMessage.UseCustom == 1)
                    {
                        translation = objCommMessage.CustomMessage;
                    }
                    else
                    {
                        translation = objCommMessage.DefaultMessage;
                    }
                }
            }

            // // if not found display english labels
            if (translation.Equals(string.Empty))
            {
                objCommMessage = lstCommMessage.FindAll(x => x.MessageID == MessageID && x.Language == 0).FirstOrDefault();

                if (objCommMessage != null)
                {
                    translation = objCommMessage.DefaultMessage;
                }
                else
                {
                    CommonEnum.Translation enumDisplayStatus = (CommonEnum.Translation)MessageID;
                    translation = enumDisplayStatus.ToString();
                }
            }

            return translation;
        }

        internal static string GetConnectionSettings()
        {
            string fileName = GlobalVariables.ConnectionSetting;
            string result = string.Empty;

            if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(fileName)))
            {
                result = "1";

                return result;
            }

            string connectionSetting = ReadFile(HttpContext.Current.Server.MapPath(fileName));
           
            if (!string.IsNullOrEmpty(connectionSetting))
            {
                result = connectionSetting;
            }

            return result;
        }
    }
}