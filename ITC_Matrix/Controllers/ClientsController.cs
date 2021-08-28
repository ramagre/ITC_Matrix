/*  
    File Name : ClientController.cs
    File For :Manage the Client Model
    Created Date : 18-11-2015
    created by : Sandip Katore
    Modified Date : 30-12-2015
*/

using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ITC_Matrix.Controllers
{
    public class ClientsController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = CommonEnum.SubMenus.clients_node.ToString();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region GetClientList User Define Function for Getting Client List Data From Two Tables

        public List<Client> GetClientList()
        {

            var Profile_Tbl = (from Client_List in db.Clients
                               join Profile_List in db.Profiles on new { dealCap = (int)Client_List.PROFILE } equals new { dealCap = (int)Profile_List.CODE }
                               select new
                               {
                                   Client_List.ID_NO,
                                   Client_List.FAMILY,
                                   Client_List.GIVEN,
                                   Client_List.NetID,
                                   Client_List.PROFILE,
                                   Client_List.TAG_NO,
                                   Client_List.ACC_BALANCE1,
                                   Profile_List.DSCR,
                                   Profile_List.CODE
                               }).AsEnumerable();

            List<Client> Client_Profile = new List<Client>();

            foreach (var m in Profile_Tbl)
            {
                Client Profile_code = new Client();
                Profile_code.ID_NO = m.ID_NO;
                Profile_code.FAMILY = m.FAMILY;
                Profile_code.GIVEN = m.GIVEN;
                Profile_code.NetID = m.NetID;
                Profile_code.PROFILE = m.PROFILE;
                Profile_code.ACC_BALANCE1 = m.ACC_BALANCE1;
                Profile_code.Profile_DSCR = m.DSCR;
                Profile_code.TAG_NO = m.TAG_NO;
                //Profile_code.Transactions = m.Transactions;

                Client_Profile.Add(Profile_code);
            }
            return Client_Profile;

        }

        #endregion

        #region Index GET method for filtering the client details
        /// <summary>
        /// POST method for Filetering the client details
        /// </summary>
        /// <param name="Profile"></param>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        // POST : Client
        [HttpGet]
        public ActionResult Index(int? Profile, string txtSearch, string txtCard, int? page, string sortBy)
        {
            ViewBag.Prefs = dbwmn.prefs.ToList();
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_client.ToString()) == false)
            {
                return View("NoAccess");
            }

            // Check delete permission and hide button
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.delete_clients.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            ViewBag.Message = TempData["message"];
            ViewBag.Class = TempData["class"];

            //Getting page size from pref table
            int pageSize = Common.Functions.GetPageSize();

            if (page < 0)
            {
                page = 1;
            }

            if (!string.IsNullOrEmpty(txtSearch))
            {
                txtSearch = txtSearch.ToLower().Trim();
            }

            //for sorting purpose
            ViewBag.ClientIDSort = String.IsNullOrEmpty(sortBy) ? "ClientID desc" : string.Empty;
            ViewBag.FamilySort = sortBy == "Family" ? "Family desc" : "Family";
            ViewBag.NetWorkIDSort = sortBy == "NetWorkID" ? "NetWorkID desc" : "NetWorkID";
            ViewBag.ProfileSort = sortBy == "Profile" ? "Profile desc" : "Profile";
            ViewBag.CardNumberSort = sortBy == "CardNumber" ? "CardNumber desc" : "CardNumber";
            ViewBag.FlexAccountBalanceSort = sortBy == "FlexAccountBalance" ? "FlexAccountBalance desc" : "FlexAccountBalance";

            //List<Profile> lstProfile = Common.Functions.GetPermittedProfiles();
            var profiles = new SelectList(Common.Functions.GetPermittedProfiles(), "CODE", "DSCR");

            if (profiles != null && profiles.Count() > 0)
            {
                ViewBag.Profile = profiles;
            }

            List<Client> objClientList = new List<Client>();

            objClientList = GetClientList();

            // filter client list based on profile right assigned to operator role
            List<Profile> lstProfile = Common.Functions.GetPermittedProfiles();
             objClientList = objClientList.Where(x => lstProfile.Any(y => y.CODE == x.PROFILE)).ToList();

            // if search textbox has value 
            if (Profile == null && txtSearch != null && txtSearch.Contains(',') && string.IsNullOrEmpty(txtCard))
            {
                string[] searchStrings = txtSearch.Split(',');
                string firstName = searchStrings[0];
                string lastName = searchStrings[1];

                if (Profile == null && txtSearch != null && string.IsNullOrEmpty(txtCard))
                {
                    objClientList = objClientList.Where(x => x.FAMILY.ToLower().Contains(firstName) && x.GIVEN.ToLower().Contains(lastName)).ToList();
                }

                if (objClientList != null && objClientList.Count > 0)
                {
                    return View(objClientList.ToPagedList(page ?? 1, pageSize));
                }

                if (Profile == null && txtSearch != null && string.IsNullOrEmpty(txtCard))
                {
                    objClientList = objClientList.Where(x => x.GIVEN.ToLower().Contains(lastName) && x.FAMILY.ToLower().Contains(firstName) || x.NetID.Contains(txtSearch) || x.ID_NO.Contains(txtSearch)).ToList();
                }

            }

            //if profile select and search textbox has values with ','
            if (Profile.HasValue && txtSearch != null && txtSearch.Contains(',') && string.IsNullOrEmpty(txtCard))
            {
                if (txtSearch.Contains(','))
                {
                    string[] searchStrings = txtSearch.Split(',');
                    string firstName = searchStrings[0];
                    string lastName = searchStrings[1];

                    List<Client> clients = objClientList.Where(x => x.PROFILE == Profile).ToList();

                    objClientList = clients.Where(x => x.GIVEN.ToLower().Contains(lastName) || x.FAMILY.ToLower().Contains(firstName) || x.NetID.Contains(txtSearch) || x.ID_NO.Contains(txtSearch)).ToList();
                }
                else
                {
                    List<Client> clients = objClientList.Where(x => x.PROFILE == Profile).ToList();

                    objClientList = clients.Where(x => x.GIVEN.ToLower().Contains(txtSearch) || x.FAMILY.ToLower().Contains(txtSearch) || x.NetID.Contains(txtSearch) || x.ID_NO.Contains(txtSearch)).ToList();
                }
            }

            //if search textbox and card textbox only both has values
            if (Profile == null && txtSearch != null && !string.IsNullOrEmpty(txtCard))
            {
                if (txtSearch.Contains(','))
                {
                    string[] searchStrings = txtSearch.Split(',');
                    string firstName = searchStrings[0];
                    string lastName = searchStrings[1];

                    objClientList = objClientList.Where(x => x.GIVEN.ToLower().Contains(lastName) || x.FAMILY.ToLower().Contains(firstName) || x.NetID.Contains(txtSearch) || x.ID_NO.Contains(txtSearch) || x.TAG_NO.Contains(txtCard)).ToList();
                }
                else
                {
                    objClientList = objClientList.Where(x => x.GIVEN.ToLower().Contains(txtSearch) || x.FAMILY.ToLower().Contains(txtSearch) || x.NetID.Contains(txtSearch) || x.ID_NO.Contains(txtSearch)).ToList();

                    objClientList = objClientList.Where(x => x.TAG_NO.Contains(txtCard)).ToList();
                }
            }
            //if all filter null default
            if (Profile == null && string.IsNullOrEmpty(txtSearch) && string.IsNullOrEmpty(txtCard))
            {
                objClientList = objClientList.ToList();
            }

            // if only search textbox has values
            if (Profile == null && txtSearch != null && string.IsNullOrEmpty(txtCard))
            {
                objClientList = objClientList.Where(x => x.GIVEN.ToLower().Contains(txtSearch) || x.FAMILY.ToLower().Contains(txtSearch) || x.NetID.Contains(txtSearch) || x.ID_NO.Contains(txtSearch)).ToList();
            }

            // if only textbox card has value 
            if (Profile == null && string.IsNullOrEmpty(txtSearch) && !string.IsNullOrEmpty(txtCard))
            {
                objClientList = objClientList.Where(x => x.TAG_NO.ToLower().ToString().Contains(txtCard)).ToList();
            }

            //if profile selected & textbox search and Card has values
            if (Profile.HasValue && txtSearch != null && !string.IsNullOrEmpty(txtCard))
            {
                if (txtSearch.Contains(','))
                {
                    string[] searchStrings = txtSearch.Split(',');
                    string firstName = searchStrings[0];
                    string lastName = searchStrings[1];

                    List<Client> filterClients = objClientList.Where(x => x.PROFILE == Profile).ToList();

                    objClientList = filterClients.Where(x => x.GIVEN.ToLower().Contains(lastName) || x.FAMILY.ToLower().Contains(firstName) || x.NetID.Contains(txtSearch) || x.ID_NO.Contains(txtSearch) || x.TAG_NO.Contains(txtCard)).ToList();

                }
                else
                {
                    List<Client> clients = objClientList.Where(x => x.PROFILE == Profile).ToList();

                    List<Client> resulttxtSearch = clients.Where(x => x.GIVEN.ToLower().Contains(txtSearch) || x.FAMILY.ToLower().StartsWith(txtSearch) || x.NetID.Contains(txtSearch) || x.ID_NO.Contains(txtSearch)).ToList();

                    objClientList = resulttxtSearch.Where(x => x.TAG_NO.Contains(txtCard)).ToList();
                }
            }

            if (Profile.HasValue && string.IsNullOrEmpty(txtSearch) && string.IsNullOrEmpty(txtCard))
            {
                objClientList = objClientList.Where(x => x.PROFILE == Profile).ToList();
            }

            if (Profile.HasValue && txtSearch == null && !string.IsNullOrEmpty(txtCard))
            {
                List<Client> clients = objClientList.Where(x => x.PROFILE == Profile).ToList();
                objClientList = clients.Where(x => x.TAG_NO.Contains(txtCard)).ToList();
            }
            else
            {
                objClientList = objClientList.ToList();
            }

            switch (sortBy)
            {
                case "ClientID desc":
                    objClientList = objClientList.OrderByDescending(x => x.ID_NO).ToList();
                    break;
                case "ClientID":
                    objClientList = objClientList.OrderBy(x => x.ID_NO).ToList();
                    break;

                case "Family desc":
                    objClientList = objClientList.OrderByDescending(x => x.FAMILY).ToList();
                    break;
                case "Family":
                    objClientList = objClientList.OrderBy(x => x.FAMILY).ToList();
                    break;
                case "NetWorkID desc":
                    objClientList = objClientList.OrderByDescending(x => x.NetID).ToList();
                    break;
                case "NetWorkID":
                    objClientList = objClientList.OrderBy(x => x.NetID).ToList();
                    break;
                case "Profile desc":
                    objClientList = objClientList.OrderByDescending(x => x.PROFILE).ToList();
                    break;
                case "Profile":
                    objClientList = objClientList.OrderBy(x => x.PROFILE).ToList();
                    break;
                case "CardNumber desc":
                    objClientList = objClientList.OrderByDescending(x => x.TAG_NO).ToList();
                    break;
                case "CardNumber":
                    objClientList = objClientList.OrderBy(x => x.TAG_NO).ToList();
                    break;

                case "FlexAccountBalance desc":
                    objClientList = objClientList.OrderByDescending(x => x.ACC_BALANCE1).ToList();
                    break;
                case "FlexAccountBalance":
                    objClientList = objClientList.OrderBy(x => x.ACC_BALANCE1).ToList();
                    break;
                default:
                    objClientList = objClientList.OrderBy(x => x.ID_NO).ToList();
                    break;
            }

            return View(objClientList.ToPagedList(page ?? 1, pageSize));
        }

        #endregion

        #region   Actionmethod for updating the credit transactions.
        /// <summary>
        ///  updatecreadit transaction actionmethod for updating the credit transactions   
        /// </summary>
        /// <param name="ID_NO"></param>
        /// <param name="stracc_code"></param>
        /// <param name="ActCredit"></param>
        /// <returns>json data</returns>

        public ActionResult UpdateCreditTransaction(string ID_NO, string stracc_code, string ActCredit)
        {
            string[] arr = stracc_code.Split('_');

            stracc_code = arr[1];
            string straccCode = string.Concat("acc", stracc_code, "_addMoney");

            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, straccCode) == false)
            {
                return View("NoAccess");
            }

            Int16 itemp = Convert.ToInt16(arr[1]);
            Client client = new Client();
            client = db.Clients.Find(ID_NO);

            var tempaccodes = from act in db.Accounts
                              orderby act.CODE
                              select act.CODE;
            int count = 1;
            foreach (var item in tempaccodes)
            {
                if (itemp == item)
                {
                    stracc_code = count.ToString();
                    break;
                }
                count = count + 1;
            }

            if (ActCredit == "Unlimited")
            {
                if (stracc_code == "1")
                {
                    client.ACC_UNLIMITED1 = 1;
                }
                else if (stracc_code == "2")
                {
                    client.ACC_UNLIMITED2 = 1;
                }
                else if (stracc_code == "3")
                {
                    client.ACC_UNLIMITED3 = 1;
                }
                else if (stracc_code == "4")
                {
                    client.ACC_UNLIMITED4 = 1;
                }
                else if (stracc_code == "5")
                {
                    client.ACC_UNLIMITED5 = 1;
                }
            }
            else
            {
                if (stracc_code == "1")
                {
                    client.ACC_UNLIMITED1 = 0;
                }
                else if (stracc_code == "2")
                {
                    client.ACC_UNLIMITED2 = 0;
                }
                else if (stracc_code == "3")
                {
                    client.ACC_UNLIMITED3 = 0;
                }
                else if (stracc_code == "4")
                {
                    client.ACC_UNLIMITED4 = 0;
                }
                else if (stracc_code == "5")
                {
                    client.ACC_UNLIMITED5 = 0;
                }
            }

            db.Entry(client).State = EntityState.Modified;

            ClientAccount CliAct = (db.ClientAccounts.Where(x => x.ClientID == ID_NO && x.AccountId == itemp)).SingleOrDefault();

            if (CliAct != null)
            {
                CliAct.UNLIMITED = 1;
                db.Entry(client).State = EntityState.Modified;
            }
            else
            {
                CliAct = new ClientAccount();
                CliAct.ClientID = client.ID_NO;
                CliAct.CREDITLIMIT = 0;
                CliAct.AccountId = itemp;
                CliAct.UNLIMITED = 1;
                CliAct.BALANCE = 0;

                db.ClientAccounts.Add(CliAct);
            }
            try
            {
                db.SaveChanges();
                TempData["message"] = "Credit updated successfully.";
                TempData["class"] = "success-msg";
                return new JsonResult() { Data = new { name = "Success" } };
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region Actionmethod for Insert Transaction 
        /// <summary>
        /// save transaction 
        /// </summary>
        /// <param name="ID_NO"></param>
        /// <param name="COMMENT"></param>
        /// <param name="PaymentMethods"></param>
        /// <param name="chequeno"></param>
        /// <param name="TextAccBalance"></param>
        /// <param name="strdate"></param>
        /// <param name="bank"></param>
        /// <param name="stracc_code"></param>
        /// <returns></returns>

        public ActionResult InsertTransaction(string ID_NO, string COMMENT, string PaymentMethods, string chequeno, string TextAccBalance, string strdate, string bank, string stracc_code)
        {
            // check module permission
            short accCode = Convert.ToInt16(stracc_code.Remove(0, 2));

            string straccCode = string.Concat("acc", accCode, "_addMoney");

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, straccCode) == false)
            {                
                return new JsonResult() { Data = new { name = "NoAccess" } };               
            }

            if (string.IsNullOrEmpty(ID_NO))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string[] arr = stracc_code.Split('_');

            Int16 itemp = Convert.ToInt16(arr[1]);

            stracc_code = arr[1];
            int PrevAccBal = 0;
            try
            {
                var BALANCEVal = (db.ClientAccounts.Where(x => x.ClientID == ID_NO && x.AccountId == itemp)).SingleOrDefault();
                if (BALANCEVal == null)
                {
                    PrevAccBal = 0;
                }
                else
                {
                    PrevAccBal = BALANCEVal.BALANCE;
                }
            }
            catch (Exception)
            {
                PrevAccBal = 0;
            }

            Client client = new Client();
            client = db.Clients.Find(ID_NO);
            Int16 PayMethodId = Convert.ToInt16(PaymentMethods);
            Int16 trantype = 0;
            Int16 paytype = 0;
            TrnDep trndep = new TrnDep();
            var source = from src in db.PayMethods
                         where src.CODE == PayMethodId
                         select new { src.TRANSTYPE, src.TYPE };

            foreach (var item in source)
            {
                trantype = Convert.ToInt16(item.TRANSTYPE);
                paytype = Convert.ToInt16(item.TYPE);
            }
            trndep.SOURCE = PayMethodId;
            trndep.AMOUNT = Convert.ToInt32(TextAccBalance) * 100;
            trndep.ID_NO = ID_NO;
            trndep.ACC_CODE = itemp;
            trndep.LOGIN = Convert.ToString(Session["UserName"]);
            trndep.TRN_DATE = DateTime.Now;

            if (paytype == 0)
            {
                trndep.COMMENT = string.Empty;
            }
            else if (paytype == 1)
            {
                trndep.COMMENT = COMMENT;
            }
            else if (paytype == 2)
            {
                trndep.COMMENT = "Cheque#:" + chequeno + " " + "Date:" + Convert.ToString(strdate) + " " + "Bank:" + bank;
            }
            else if (paytype == 3)
            {
                trndep.COMMENT = string.Empty;
            }


            var tempaccodes = from act in db.Accounts
                              orderby act.CODE
                              select act.CODE;
            int count = 1;
            foreach (var item in tempaccodes)
            {
                if (itemp == item)
                {
                    stracc_code = count.ToString();
                    break;
                }
                count = count + 1;
            }

            if (trantype == 1)
            {
                if (PrevAccBal != 0)
                {
                    trndep.BALANCE = (Convert.ToInt32(TextAccBalance) + (PrevAccBal / 100)) * 100;
                    if (stracc_code == "1")
                    {
                        client.ACC_BALANCE1 = (Convert.ToInt32(TextAccBalance) + (PrevAccBal / 100)) * 100;
                    }
                    else if (stracc_code == "2")
                    {
                        client.ACC_BALANCE2 = (Convert.ToInt32(TextAccBalance) + (PrevAccBal / 100)) * 100;
                    }
                    else if (stracc_code == "3")
                    {
                        client.ACC_BALANCE3 = (Convert.ToInt32(TextAccBalance) + (PrevAccBal / 100)) * 100;
                    }
                    else if (stracc_code == "4")
                    {
                        client.ACC_BALANCE4 = (Convert.ToInt32(TextAccBalance) + (PrevAccBal / 100)) * 100;
                    }
                    else if (stracc_code == "5")
                    {
                        client.ACC_BALANCE5 = (Convert.ToInt32(TextAccBalance) + (PrevAccBal / 100)) * 100;
                    }
                }
                else
                {
                    trndep.BALANCE = Convert.ToInt32(TextAccBalance) * 100;
                    if (stracc_code == "1")
                    {
                        client.ACC_BALANCE1 = (Convert.ToInt32(TextAccBalance) * 100);
                    }
                    else if (stracc_code == "2")
                    {
                        client.ACC_BALANCE2 = Convert.ToInt32(TextAccBalance) * 100;
                    }
                    else if (stracc_code == "3")
                    {
                        client.ACC_BALANCE3 = Convert.ToInt32(TextAccBalance) * 100;
                    }
                    else if (stracc_code == "4")
                    {
                        client.ACC_BALANCE4 = Convert.ToInt32(TextAccBalance) * 100;
                    }
                    else if (stracc_code == "5")
                    {
                        client.ACC_BALANCE5 = Convert.ToInt32(TextAccBalance) * 100;
                    }
                }
            }
            else if (trantype == 2)
            {
                 straccCode = string.Concat("acc", accCode, "_deductMoney");

                if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, straccCode) == false)
                {
                    return new JsonResult() { Data = new { name = "NoAccess" } };
                }

                if (PrevAccBal != 0)
                {
                    trndep.BALANCE = ((PrevAccBal / 100) - (Convert.ToInt32(TextAccBalance))) * 100;
                    if (stracc_code == "1")
                    {
                        client.ACC_BALANCE1 = ((PrevAccBal / 100) - Convert.ToInt32(TextAccBalance)) * 100;
                    }
                    else if (stracc_code == "2")
                    {
                        client.ACC_BALANCE2 = ((PrevAccBal / 100) - Convert.ToInt32(TextAccBalance)) * 100;
                    }
                    else if (stracc_code == "3")
                    {
                        client.ACC_BALANCE3 = ((PrevAccBal / 100) - Convert.ToInt32(TextAccBalance)) * 100;
                    }
                    else if (stracc_code == "4")
                    {
                        client.ACC_BALANCE4 = ((PrevAccBal / 100) - Convert.ToInt32(TextAccBalance)) * 100;
                    }
                    else if (stracc_code == "5")
                    {
                        client.ACC_BALANCE5 = ((PrevAccBal / 100) - Convert.ToInt32(TextAccBalance)) * 100;
                    }
                }
                else
                {
                    trndep.BALANCE = ((PrevAccBal / 100) - Convert.ToInt32(TextAccBalance)) * 100;
                    if (stracc_code == "1")
                    {
                        client.ACC_BALANCE1 = -(Convert.ToInt32(TextAccBalance) * 100);
                    }
                    else if (stracc_code == "2")
                    {
                        client.ACC_BALANCE2 = -(Convert.ToInt32(TextAccBalance) * 100);
                    }
                    else if (stracc_code == "3")
                    {
                        client.ACC_BALANCE3 = -(Convert.ToInt32(TextAccBalance) * 100);
                    }
                    else if (stracc_code == "4")
                    {
                        client.ACC_BALANCE4 = -(Convert.ToInt32(TextAccBalance) * 100);
                    }
                    else if (stracc_code == "5")
                    {
                        client.ACC_BALANCE5 = -(Convert.ToInt32(TextAccBalance) * 100);
                    }
                }
            }

            trndep.PLANCODE = 0;
            trndep.MEALCODE = 0;
            trndep.MEALS = 0;
            trndep.CreditCardAuthNumber = "0";
            trndep.CreditCardType = 0;
            db.TrnDeps.Add(trndep);

            client.COMMENT = COMMENT;

            ClientAccount CliAct = (db.ClientAccounts.Where(x => x.ClientID == ID_NO && x.AccountId == itemp)).SingleOrDefault();

            if (CliAct != null)
            {
                if (trantype == 1)
                {
                    if (PrevAccBal != 0)
                    {
                        CliAct.BALANCE = (Convert.ToInt32(TextAccBalance) + (PrevAccBal / 100)) * 100;
                    }
                    else
                    {
                        CliAct.BALANCE = Convert.ToInt32(TextAccBalance) * 100;
                    }
                    db.Entry(client).State = EntityState.Modified;

                }
                else if (trantype == 2)
                {
                    if (PrevAccBal != 0)
                    {
                        CliAct.BALANCE = ((PrevAccBal / 100) - Convert.ToInt32(TextAccBalance)) * 100;
                    }
                    else
                    {
                        CliAct.BALANCE = -(Convert.ToInt32(TextAccBalance) * 100);
                    }
                    db.Entry(client).State = EntityState.Modified;
                }
            }
            else
            {
                CliAct = new ClientAccount();
                CliAct.ClientID = client.ID_NO;
                CliAct.CREDITLIMIT = 0;
                CliAct.AccountId = itemp;
                CliAct.UNLIMITED = 0;
                if (trantype == 1)
                {
                    if (PrevAccBal != 0)
                    {
                        CliAct.BALANCE = ((PrevAccBal / 100) + Convert.ToInt32(TextAccBalance)) * 100;
                    }
                    else
                    {
                        CliAct.BALANCE = Convert.ToInt32(TextAccBalance) * 100;
                    }
                }
                else if (trantype == 2)
                {
                    if (PrevAccBal != 0)
                    {
                        CliAct.BALANCE = ((PrevAccBal / 100) - Convert.ToInt32(TextAccBalance)) * 100;
                    }
                    else
                    {
                        CliAct.BALANCE = -(Convert.ToInt32(TextAccBalance) * 100);
                    }
                }

                db.ClientAccounts.Add(CliAct);
            }
            try
            {
                db.SaveChanges();
                TempData["message"] = "Balance deposited successfully.";
                TempData["class"] = "success-msg";
                return new JsonResult() { Data = new { name = "Success" } };
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error"); 
            }
        }

        #endregion

        #region Function used to get the Limit Value  thrugh ajax call

        public ActionResult GetLimit(string id, string stracc_code)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName) == false)
            {
                return View("NoAccess");
            }

            string[] arr = stracc_code.Split('_');

            Int16 itemp = Convert.ToInt16(arr[1]);
            var LimitVal = (db.ClientAccounts.Where(x => x.ClientID == id && x.AccountId == itemp)).SingleOrDefault();

            if (LimitVal != null)
            {
                return new JsonResult() { Data = new { name = LimitVal.UNLIMITED } };
            }
            return new JsonResult() { Data = new { name = 1 } };
        }

        #endregion

        #region Function used to get the payment type thrugh ajax call
        public ActionResult GetPayType(int id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName) == false)
            {
                return View("NoAccess");
            }

            var paytypes = db.PayMethods.Where(c => c.CODE == id);
            foreach (var item in paytypes)
            {
                return new JsonResult() { Data = new { name = item.TYPE } };
            }

            return new JsonResult();
        }
        #endregion

        #region actionmethod for updating the Deposit

        [HttpPost]
        public ActionResult Deposit([Bind(Include = "ID_NO,FAMILY,GIVEN,ACC_BALANCE1,ACC_BALANCE2,ACC_BALANCE3,ACC_BALANCE4,ACC_BALANCE5,COMMENT,txtdeposit")] Client client, ViewModel vd, string ID_NO, string Command)
        {
            //we need to pass taskref

            //string taskRef =  Common.Functions.GetTaskRef();
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName) == false)
            {
                return View();
            }

            try
            {
                if (Command == "Submit")
                {
                    if (ModelState.IsValid)
                    {
                        if (ID_NO == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        List<TrnDep> lsttrndep = new List<TrnDep>();
                        TrnDep trndep = new TrnDep();

                        Int16 trantype = Convert.ToInt16(Request.Form["PaymentMethods"]);

                        trndep.ID_NO = ID_NO;
                        trndep.ACC_CODE = 1;
                        if (trantype == 2)
                        {
                            trndep.TRN_DATE = Convert.ToDateTime(Request.Form["txtdate"]);
                            trndep.COMMENT = "Cheque#:" + Request.Form["txtchequeno"] + " " + "Date:" + Convert.ToString(Request.Form["txtdate"]) + " " + "Bank:" + Convert.ToString(Request.Form["txtbank"]);
                        }
                        else
                        {
                            trndep.TRN_DATE = DateTime.Now;
                            trndep.COMMENT = Request.Form["COMMENT"];
                        }
                        trndep.AMOUNT = Convert.ToInt32(Request.Form["AccountBalance"]) * 100;
                        trndep.BALANCE = (Convert.ToInt32(Request.Form["AccountBalance"]) + (client.ACC_BALANCE1 / 100)) * 100;
                        trndep.LOGIN = Session["UserName"].ToString();


                        var source = from src in db.PayMethods
                                     where src.CODE == trantype
                                     select src.TRANSTYPE;
                        foreach (var item in source)
                        {
                            trndep.SOURCE = Convert.ToInt16(item);
                        }

                        trndep.PLANCODE = 0;
                        trndep.MEALCODE = 0;
                        trndep.MEALS = 0;
                        trndep.CreditCardAuthNumber = "0";
                        trndep.CreditCardType = 0;
                        db.TrnDeps.Add(trndep);

                        client = db.Clients.Find(ID_NO);

                        if (Request.Form["ACC_BALANCE1"] != string.Empty)
                        {
                            client.ACC_BALANCE1 = (client.ACC_BALANCE1 + Convert.ToInt32(Request.Form["ACC_BALANCE1"])) * 100;
                        }
                        if (Request.Form["COMMENT"] != string.Empty)
                        {
                            client.COMMENT = Convert.ToString(Request.Form["COMMENT"]);
                        }

                        db.Entry(client).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("AccountInfo");
                    }

                }
                else if (Command == "Cancel")
                {
                    RedirectToAction("Deposit");
                }
                return RedirectToAction("AccountInfo");
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error occurred in updating a department.";
                TempData["class"] = "error-msg";
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
        #endregion

        #region GET actionmethod for Account Information

        [HttpGet]
        public ActionResult AccountInfo(string id)
        {
           
            // check module permission
            ViewBag.Prefs = dbwmn.prefs.ToList();
            ViewBag.moduleName = moduleName;
            ViewBag.message = TempData["message"];
            ViewBag.Class = TempData["class"];
            Session["ClientAccountID"] = id;         

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName) == false)
            {
                return View("NoAccess");
            }

            try
            {
                Client client = db.Clients.Find(id);
                //var PayMethodsQuery = (from s in db.PayMethods select s).ToList();
                //var PermissionsGrantsQuery = (from s in dbwmn.permissionsGrants select s).ToList().Where(x => x.ptype == "payMethod");
                //var result = (from t1 in PayMethodsQuery
                //              join t2 in PermissionsGrantsQuery
                //              on new { commcol = (int)t1.CODE } equals new { commcol = t2.pid }
                //              select new { t1.DSCR, t1.TYPE, t1.CODE, t1.TRANSTYPE, t2.pid, t2.ptype }).AsEnumerable();


                //ViewBag.PaymentMethods = new MultiSelectList(result, "CODE", "DSCR");

                List<PayMethod> lstpayMethod = Common.Functions.GetPermittedPayMethods();

                if (lstpayMethod != null)
                {
                    ViewBag.PaymentMethods = new SelectList(lstpayMethod.ToList(), "CODE", "DSCR");
                }               
                var CurSign = db.Params.Where(x => x.Name.ToUpper() == "CURRENCYSIGN").SingleOrDefault();
                var signdirection = db.Params.Where(x => x.Name.ToUpper() == "CURRENCYSIGNALIGN").SingleOrDefault();
                string strcurSign = string.Empty;
                string strsigndirection = string.Empty;
                if (!(string.IsNullOrEmpty(signdirection.Value)))
                {
                    strsigndirection = signdirection.Value;
                }
                else
                {
                    strsigndirection = "L";
                }

                if (CurSign.Value != string.Empty)
                {
                    strcurSign = CurSign.Value;
                }

                ViewBag.strcurSign = strcurSign;

                int DB_Structure = Common.Functions.GetDBStructure();
               
                string tempitem = "0.00";

                var allacounts = db.Accounts.Select(x => new { CODE = x.CODE, DSCR = x.DSCR }).ToList();

                // if unlimited structure
                if (DB_Structure >=(int) CommonEnum.DBStructure.Unlimited)
                {
                    var lstAccount = db.Accounts.ToList().Select(x => x.DSCR);
                    
                    ViewBag.allDescription = from act in db.Accounts
                                             orderby act.CODE
                                             select (act.DSCR);

                    var accountdesclst = from act in db.Accounts
                                         orderby act.CODE
                                         select act.DSCR;
                
                    int k = accountdesclst.ToList().Count;

                    var Actlst = db.ClientAccounts.Where(x => x.ClientID == id).OrderBy(x => x.AccountId).ToList();
                    var tempacclist = db.Accounts.OrderBy(x => x.CODE).ToList();
                    ViewBag.tempacclist = tempacclist;


                    client.ActBalList = new List<string>();
                    string[] temparr = new string[k];
                    for (int i = 0; i < k; i++)
                    {
                        temparr[i] = tempacclist.ToList()[i].CODE.ToString();
                    }

                    if (strsigndirection == "L")
                    {
                        for (int i = 0; i < k; i++)
                        {
                            tempitem = "0.00";

                            try
                            {
                                short itemp = Convert.ToInt16(temparr[i]);
                                var chkaccbal = db.ClientAccounts.Where(x => x.ClientID == id && x.AccountId == itemp).ToList();
                                if (chkaccbal != null)
                                {
                                    if (chkaccbal.Count() > 0)
                                    {
                                        tempitem = chkaccbal.ToList()[0].BALANCE.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                tempitem = "0.00";
                            }



                            if (Convert.ToDouble(tempitem) != 0)
                            {
                                tempitem = (Convert.ToDouble(tempitem) / 100).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            client.ActBalList.Add(strcurSign + tempitem);
                        }
                    }
                    else if (strsigndirection == "R")
                    {
                        for (int i = 0; i < k; i++)
                        {
                            tempitem = "0.00";
                            try
                            {
                                short itemp = Convert.ToInt16(temparr[i]);
                                var chkaccbal = db.ClientAccounts.Where(x => x.ClientID == id && x.AccountId == itemp).ToList();
                                if (chkaccbal != null)
                                {
                                    if (chkaccbal.Count() > 0)
                                    {
                                        tempitem = chkaccbal.ToList()[0].BALANCE.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                }

                            }
                            catch (Exception)
                            {
                                tempitem = "0.00";
                            }
                            if (Convert.ToDouble(tempitem) != 0)
                            {
                                tempitem = (Convert.ToDouble(tempitem) / 100).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            client.ActBalList.Add(tempitem + strcurSign);
                        }
                    }
                    else if (strsigndirection == "N")
                    {
                        for (int i = 0; i < k; i++)
                        {
                            tempitem = "0.00";

                            try
                            {
                                short itemp = Convert.ToInt16(temparr[i]);
                                var chkaccbal = db.ClientAccounts.Where(x => x.ClientID == id && x.AccountId == itemp).ToList();
                                if (chkaccbal != null)
                                {
                                    if (chkaccbal.Count() > 0)
                                    {
                                        tempitem = chkaccbal.ToList()[0].BALANCE.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                tempitem = "0.00";
                            }
                            if (Convert.ToDouble(tempitem) != 0)
                            {
                                tempitem = (Convert.ToDouble(tempitem) / 100).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            client.ActBalList.Add(tempitem);
                        }
                    }
                }
                else
                {                    
                    allacounts = db.Accounts.Select(x => new {CODE= x.CODE, DSCR= x.DSCR }).Take(5).ToList();

                    var Actlst = (from Client_List in db.Clients.Where(c => c.ID_NO == id)
                                  select new
                                  {
                                      Client_List.ACC_BALANCE1,
                                      Client_List.ACC_BALANCE2,
                                      Client_List.ACC_BALANCE3,
                                      Client_List.ACC_BALANCE4,
                                      Client_List.ACC_BALANCE5,
                                  }).AsEnumerable();
                    client.ActBalList = new List<string>();

                    if (strsigndirection == "L")
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            tempitem = "0.00";

                            try
                            {
                                if (i == 0)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE1.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 1)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE2.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 2)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE3.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 3)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE4.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 4)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE5.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }
                            catch (Exception)
                            {
                                tempitem = "0.00";
                            }

                            if (Convert.ToDouble(tempitem) != 0)
                            {
                                tempitem = (Convert.ToDouble(tempitem) / 100).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            client.ActBalList.Add(strcurSign + tempitem);
                        }
                    }
                    else if (strsigndirection == "R")
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            tempitem = "0.00";

                            try
                            {
                                if (i == 0)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE1.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 1)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE2.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 2)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE3.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 3)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE4.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 4)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE5.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }
                            catch (Exception)
                            {
                                tempitem = "0.00";
                            }

                            if (Convert.ToDouble(tempitem) != 0)
                            {
                                tempitem = (Convert.ToDouble(tempitem) / 100).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            client.ActBalList.Add(tempitem + strcurSign);
                        }
                    }
                    else if (strsigndirection == "N")
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            tempitem = "0.00";

                            try
                            {
                                if (i == 0)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE1.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 1)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE2.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 2)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE3.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 3)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE4.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else if (i == 4)
                                {
                                    tempitem = Actlst.ToList()[0].ACC_BALANCE5.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }
                            catch (Exception)
                            {
                                tempitem = "0.00";
                            }
                            if (Convert.ToDouble(tempitem) != 0)
                            {
                                tempitem = (Convert.ToDouble(tempitem) / 100).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            client.ActBalList.Add(tempitem);
                        }
                    }
                }
                
                List<Account> lstAccountList = new List<Account>();

                foreach (var item in allacounts)
                {
                    Account objAccount = new Account();
                    objAccount.CODE = item.CODE;
                    objAccount.DSCR = item.DSCR;
                    lstAccountList.Add(objAccount);
                }

                ViewBag.allacounts = lstAccountList;


                return View(client);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
        #endregion

        #region Create GET Method for creating the new Client details
        /// <summary>
        /// GET method for creating the new client details
        /// </summary>
        /// <returns></returns>

        // GET: Clients/Create
        public ActionResult Create()
        {
            // check module permission
            if (Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client.ToString()) == false)
            {
                return View("NoAccess");
            }
            ViewBag.Message = TempData["message"];
            ViewBag.Class = TempData["class"];
            //binding data to the Profile Dropdown

            // Get permitted profile only.
            List<Profile> lstProfile = Common.Functions.GetPermittedProfiles();

            if (lstProfile != null)
            {
                ViewBag.PROFILE = new SelectList(lstProfile.ToList(), "CODE", "DSCR");
            }

            //Binding sample data to the language Dropdown

            IEnumerable<SelectListItem> languages =
                    (from language in db.Languages
                     select new SelectListItem
                     {
                         Text = language.DSCR,
                         Value = language.Code.ToString()
                     }).AsEnumerable();

            ViewBag.LanguagePreference = languages;

            ViewBag.AccountType = (db.AccountCodes.ToList()).AsEnumerable();
            return View();
        }

        #endregion

        #region Create POST method for creating  new clients
        /// <summary>
        /// POST method for creating the new client details
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_NO,PROFILE,FAMILY,GIVEN,PHONE,ADDRESS,CITY,STATE,ZIP,COUNTRY,COMMENT,ACC_BALANCE1,ACC_LIMIT1,ACC_UNLIMITED1,ACC_BALANCE2,ACC_LIMIT2,ACC_UNLIMITED2,ACC_BALANCE3,ACC_LIMIT3,ACC_UNLIMITED3,PLAN_CODE1,PLAN_CODE2,PLAN_CODE3,PLAN_START1,PLAN_START2,PLAN_START3,TAG_CODE,TAG_NO,TAG_ISSUE,TAG_EXPIRY,TAG_STATUS,LastMealTime,WeekMealCount,LastPlanCode,HoldExpiry,HoldRegisterID,Faculty,PrOfStudy,Department,Title,EMail,NetID,Allow_Undefined,RefundLastMealTime,RefundLastPlanCode,ACC_BALANCE4,ACC_LIMIT4,ACC_UNLIMITED4,ACC_BALANCE5,ACC_LIMIT5,ACC_UNLIMITED5,Allow_FreePrint,WeekDayCount,EasyConvert_Admin,WeekMealCount2,WeekMealCount3,DOB,LanguagePreference,TermCondAccept,TermCondAcceptDate,LastMealTime2,LastMealTime3,GradeLevel,ResidencyStatus,EmailFlag,PinMustChange,LastWebPurchaseLogin,LockoutAttempts,PinChangeDate,Lockout,LastUpdate,ParkMember,OperatorID,AppName,AppID,IPAddress,AllowMailingBool,EasyConvert_AdminBool,Allow_UndefinedBool,Allow_FreePrintBool")] Client client, HttpPostedFileBase uploadedImage, string selectedAccCode)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client.ToString()) == false)
            {
                return View("NoAccess");
            }

            //selected AccCode Values
            if (!string.IsNullOrEmpty(selectedAccCode))
            {
                client.Department = selectedAccCode;
            }

            corClientPicture objcorClientPicture = new corClientPicture();

            //list for showing the Departments Code
            List<string> selectedAccountCode = @ViewBag.selected;

            if (uploadedImage != null)
            {
                if (uploadedImage.ContentLength < 2097152)
                {
                    if (uploadedImage.ContentType.ToLower() == "image/jpg" ||
                        uploadedImage.ContentType.ToLower() == "image/png" ||
                        uploadedImage.ContentType.ToLower() == "image/jpeg" ||
                        uploadedImage.ContentType.ToLower() == "image/gif" ||
                        uploadedImage.ContentType.ToLower() == "image/tif")
                    {
                        objcorClientPicture.Picture = ConvertToBytes(uploadedImage);
                    }
                    else
                    {
                        TempData["message"] = "Please select only images of type jpg/png/jpeg/gif/tif ";
                        TempData["class"] = "error-msg";
                        return RedirectToAction("create");
                    }
                }
                else
                {
                    TempData["message"] = "Please select image less then 2MB";
                    TempData["class"] = "error-msg";
                    return RedirectToAction("create");
                }
            }

            SetDefaulValues(client);

            if (ModelState.IsValid)
            {
                try
                {
                    db.Clients.Add(client);
                    db.SaveChanges();
                    TempData["message"] = "Client added successfully.";
                    TempData["class"] = "success-msg";
                    //saving image into corClientPicture model
                    if (uploadedImage != null)
                    {
                        objcorClientPicture.ClientID = client.ID_NO;
                        objcorClientPicture.LastUpdated = DateTime.Now;
                        db.corClientPictures.Add(objcorClientPicture);

                        db.SaveChanges();
                        TempData["message"] = "Client added successfully.";
                        TempData["class"] = "success-msg";
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while processing :", ex);
                    TempData["message"] = "Error occurred in adding a client";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }

            }

            return View(client);
        }

        #endregion

        #region  User Define Function for saving Uploaded Image

        public byte[] ConvertToBytes(HttpPostedFileBase uploadedImage)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(uploadedImage.InputStream);
            imageBytes = reader.ReadBytes((int)uploadedImage.ContentLength);
            return imageBytes;
        }

        #endregion

        #region  function for assigning default values
        /// <summary>
        /// function for assigning default values
        /// </summary>
        /// <param name="client"></param>

        public void SetDefaulValues(Client client)
        {
            if (string.IsNullOrEmpty(client.ADDRESS))
            {
                client.ADDRESS = string.Empty;
            }

            if (string.IsNullOrEmpty(client.GIVEN))
            {
                client.GIVEN = string.Empty;
            }

            if (string.IsNullOrEmpty(client.PHONE))
            {
                client.PHONE = string.Empty;
            }

            if (string.IsNullOrEmpty(client.STATE))
            {
                client.STATE = string.Empty;
            }

            if (string.IsNullOrEmpty(client.CITY))
            {
                client.CITY = string.Empty;
            }

            if (string.IsNullOrEmpty(client.ZIP))
            {
                client.ZIP = string.Empty;
            }

            if (string.IsNullOrEmpty(client.COUNTRY))
            {
                client.COUNTRY = string.Empty;
            }

            if (string.IsNullOrEmpty(client.COMMENT))
            {
                client.COMMENT = string.Empty;
            }

            if (client.ACC_BALANCE1.Equals(string.Empty))
            {
                client.ACC_BALANCE1 = 0;
            }

            if (client.ACC_LIMIT1.Equals(string.Empty))
            {
                client.ACC_LIMIT1 = 0;
            }

            if (client.ACC_UNLIMITED1.Equals(string.Empty))
            {
                client.ACC_UNLIMITED1 = 0;
            }
            if (client.ACC_BALANCE2.Equals(string.Empty))
            {
                client.ACC_BALANCE2 = 0;
            }

            if (client.ACC_UNLIMITED2.Equals(string.Empty))
            {
                client.ACC_UNLIMITED2 = 0;
            }
            if (client.ACC_BALANCE3.Equals(string.Empty))
            {
                client.ACC_BALANCE3 = 0;
            }

            if (client.ACC_LIMIT3.Equals(string.Empty))
            {
                client.ACC_LIMIT3 = 0;
            }

            if (client.ACC_UNLIMITED3.Equals(string.Empty))
            {
                client.ACC_UNLIMITED3 = 0;
            }

            if (client.PLAN_START1 == null)
            {
                client.PLAN_START1 = System.DateTime.Now;
            }
            if (client.PLAN_START2 == null)
            {
                client.PLAN_START2 = System.DateTime.Now;
            }

            if (client.PLAN_START3 == null)
            {
                client.PLAN_START3 = System.DateTime.Now;
            }
            if (string.IsNullOrEmpty(client.TAG_NO))
            {
                client.TAG_NO = string.Empty;
            }

            if (string.IsNullOrEmpty(client.TAG_CODE))
            {
                client.TAG_CODE = string.Empty;
            }

            if (client.TAG_ISSUE == null)
            {
                client.TAG_ISSUE = System.DateTime.Now;
            }

            if (client.TAG_EXPIRY == null)
            {
                client.TAG_EXPIRY = System.DateTime.Now;
            }

            if (client.LastMealTime == null)
            {
                client.LastMealTime = System.DateTime.Now;
            }

            if (client.WeekMealCount.Equals(string.Empty))
            {
                client.WeekMealCount = 0;
            }

            if (client.LastPlanCode.Equals(string.Empty))
            {
                client.LastPlanCode = 0;
            }

            if (client.HoldExpiry == null)
            {
                client.HoldExpiry = System.DateTime.Now;
            }

            if (client.HoldRegisterID.Equals(string.Empty))
            {
                client.HoldRegisterID = 0;
            }

            if (string.IsNullOrEmpty(client.Faculty))
            {
                client.Faculty = string.Empty;
            }

            if (string.IsNullOrEmpty(client.PrOfStudy))
            {
                client.PrOfStudy = string.Empty;
            }

            if (string.IsNullOrEmpty(client.Department))
            {
                client.Department = string.Empty;
            }

            if (string.IsNullOrEmpty(client.Title))
            {
                client.Title = string.Empty;
            }

            if (string.IsNullOrEmpty(client.EMail))
            {
                client.EMail = string.Empty;
            }

            if (string.IsNullOrEmpty(client.NetID))
            {
                client.NetID = string.Empty;
            }

            if (client.Allow_Undefined.Equals(string.Empty))
            {
                client.Allow_Undefined = 0;
            }

            if (client.RefundLastMealTime == null)
            {
                client.RefundLastMealTime = System.DateTime.Now;
            }

            if (client.RefundLastPlanCode.Equals(string.Empty))
            {
                client.RefundLastPlanCode = 0;
            }

            if (client.ACC_BALANCE4.Equals(string.Empty))
            {
                client.ACC_BALANCE4 = 0;
            }

            if (client.ACC_LIMIT4.Equals(string.Empty))
            {
                client.ACC_LIMIT4 = 0;
            }

            if (client.ACC_UNLIMITED4.Equals(string.Empty))
            {
                client.ACC_UNLIMITED4 = 0;
            }
            if (client.ACC_BALANCE5.Equals(string.Empty))
            {
                client.ACC_BALANCE5 = 0;
            }

            if (client.ACC_LIMIT5.Equals(string.Empty))
            {
                client.ACC_LIMIT5 = 0;
            }
            if (client.ACC_UNLIMITED5.Equals(string.Empty))
            {
                client.ACC_UNLIMITED5 = 0;
            }

            if (client.Allow_FreePrint.Equals(string.Empty))
            {
                client.Allow_FreePrint = 0;
            }

            if (client.WeekDayCount.Equals(string.Empty))
            {
                client.WeekDayCount = 0;
            }

            if (client.EasyConvert_Admin.Equals(string.Empty))
            {
                client.EasyConvert_Admin = 0;
            }

            if (client.WeekMealCount2.Equals(string.Empty))
            {
                client.WeekMealCount2 = 0;
            }

            if (client.WeekMealCount3.Equals(string.Empty))
            {
                client.WeekMealCount3 = 0;
            }

            if (client.DOB == null)
            {
                client.DOB = System.DateTime.Now;
            }

            if (client.TermCondAccept.Equals(string.Empty))
            {
                client.TermCondAccept = 0;
            }

            if (client.TermCondAcceptDate == null)
            {
                client.TermCondAcceptDate = System.DateTime.Now;
            }
            if (client.LastMealTime2 == null)
            {
                client.LastMealTime2 = System.DateTime.Now;
            }

            if (client.LastMealTime3 == null)
            {
                client.LastMealTime3 = System.DateTime.Now;
            }

            if (string.IsNullOrEmpty(client.GradeLevel))
            {
                client.GradeLevel = string.Empty;
            }
            if (string.IsNullOrEmpty(client.ResidencyStatus))
            {
                client.ResidencyStatus = string.Empty;
            }

            if (client.EmailFlag.Equals(string.Empty))
            {
                client.EmailFlag = 0;
            }

            if (client.PinMustChange.Equals(string.Empty))
            {
                client.PinMustChange = 0;
            }

            if (client.LockoutAttempts.Equals(string.Empty))
            {
                client.LockoutAttempts = 0;
            }

            if (client.Lockout.Equals(string.Empty))
            {
                client.Lockout = 0;
            }
        }

        #endregion

        #region Edit GET Method for Updating the client details
        /// <summary>
        /// GET method for Updating the client Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Clients/Edit/5
        public ActionResult Edit(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Client client = db.Clients.Find(id);

            ViewBag.ProfileImage = db.corClientPictures.Where(x => x.ClientID == id).Select(y => y.Picture).SingleOrDefault();

            //Binding data to the language Dropdown(selected value)
            ViewBag.LanguagePreference = new SelectList(db.Languages.ToList(), "CODE", "DSCR", client.LanguagePreference);

            // Get permitted profile only.
            List<Profile> lstProfile = Common.Functions.GetPermittedProfiles();

            if (lstProfile != null)
            {
                ViewBag.PROFILE = new SelectList(lstProfile.ToList(), "CODE", "DSCR", client.PROFILE);
            }

            ////Binding data to the Profile Dropdown(selected value)
            //ViewBag.Profile = new SelectList(db.Profiles.ToList(), "CODE", "DSCR", client.PROFILE);

            //For Showing the selected AccountCodes

            string ClientAccountType = db.Clients.Where(x => x.ID_NO == id).Select(y => y.Department).SingleOrDefault();


            List<AccountCode> lstAccountCode = db.AccountCodes.ToList();

            if (ClientAccountType != null)
            {
                string[] listClientAccountType = ClientAccountType.Split(',');

                for (int i = 0; i < listClientAccountType.Count(); i++)
                {
                    for (int j = 0; j < lstAccountCode.Count(); j++)
                    {
                        if (listClientAccountType[i].Equals(lstAccountCode[j].AccCode))
                        {
                            lstAccountCode[j].isSelected = true;
                        }
                    }
                }
            }

            ViewBag.AccountType = lstAccountCode;

            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        #endregion

        #region Edit POST method for Updating client Details 
        /// <summary>
        /// POST method for Updating the Client Details
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_NO,PROFILE,FAMILY,GIVEN,PHONE,ADDRESS,CITY,STATE,ZIP,COUNTRY,COMMENT,ACC_BALANCE1,ACC_LIMIT1,ACC_UNLIMITED1,ACC_BALANCE2,ACC_LIMIT2,ACC_UNLIMITED2,ACC_BALANCE3,ACC_LIMIT3,ACC_UNLIMITED3,PLAN_CODE1,PLAN_CODE2,PLAN_CODE3,PLAN_START1,PLAN_START2,PLAN_START3,TAG_CODE,TAG_NO,TAG_ISSUE,TAG_EXPIRY,TAG_STATUS,LastMealTime,WeekMealCount,LastPlanCode,HoldExpiry,HoldRegisterID,Faculty,PrOfStudy,Department,Title,EMail,NetID,Allow_Undefined,RefundLastMealTime,RefundLastPlanCode,ACC_BALANCE4,ACC_LIMIT4,ACC_UNLIMITED4,ACC_BALANCE5,ACC_LIMIT5,ACC_UNLIMITED5,Allow_FreePrint,WeekDayCount,EasyConvert_Admin,WeekMealCount2,WeekMealCount3,DOB,LanguagePreference,TermCondAccept,TermCondAcceptDate,LastMealTime2,LastMealTime3,GradeLevel,ResidencyStatus,EmailFlag,PinMustChange,LastWebPurchaseLogin,LockoutAttempts,PinChangeDate,Lockout,LastUpdate,ParkMember,OperatorID,AppName,AppID,IPAddress,AllowMailingBool,Allow_UndefinedBool,Allow_FreePrintBool,EasyConvert_AdminBool")] Client client, HttpPostedFileBase uploadedImage, string selectedAccCode)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client.ToString()) == false)
            {
                return View();
            }

            SetDefaulValues(client);

            //selected AccCode Values
            if (!string.IsNullOrEmpty(selectedAccCode))
            {
                client.Department = selectedAccCode;
            }

            corClientPicture objcorClientPicture = new corClientPicture();

            if (uploadedImage != null)
            {
                if (uploadedImage.ContentLength < 2097152)
                {
                    if (uploadedImage.ContentType.ToLower() == "image/jpg" ||
                        uploadedImage.ContentType.ToLower() == "image/png" ||
                        uploadedImage.ContentType.ToLower() == "image/jpeg" ||
                        uploadedImage.ContentType.ToLower() == "image/gif" ||
                        uploadedImage.ContentType.ToLower() == "image/tif")
                    {
                        objcorClientPicture.Picture = ConvertToBytes(uploadedImage);

                    }
                    else
                        ViewBag.Message = "Please Select Only Images of type jpg/png/jpeg/gif/tif ";
                }
                else
                    ViewBag.Message = "File cannot be more than 2MB";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(client).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = "Client updated successfully.";
                    TempData["class"] = "success-msg";
                }
                catch (Exception ex)
                {

                    TempData["message"] = "Error occurred in updating a client";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }

                //saving image into corClientPicture model

                if (objcorClientPicture.Picture != null)
                {
                    objcorClientPicture.ClientID = client.ID_NO;
                    objcorClientPicture.LastUpdated = DateTime.Now;

                    db.Entry(objcorClientPicture).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(client);
        }

        #endregion

        #region User Define function for Checking the ClientId exist or not?
        /// <summary>
        /// User Define function for Checking the ClientId exist or not?
        /// </summary>
        /// <param name="ID_NO"></param>
        /// <returns></returns>


        public JsonResult IsClientIDAvailble(string ID_NO)
        {
            var result = true;
            var clientID = db.Clients.Where(x => x.ID_NO.ToString().Trim() == ID_NO.ToString().Trim()).FirstOrDefault();
            var deletedClientID = db.DelClients.Where(x => x.ID_NO.ToString().Trim() == ID_NO.ToString().Trim()).FirstOrDefault();

            if (clientID != null || deletedClientID != null)
                result = false;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region User Define function for Checking the NetworkId exist or not?

        public JsonResult IsNetIDAvailble(string NetID)
        {
            var result = true;
            var ClientID = db.Clients.Where(x => x.NetID.ToString().Trim().Equals(NetID.ToString().Trim())).FirstOrDefault();

            if (ClientID != null)
                result = false;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region POST method for Confirmation on CLient Delete and Deleted Client from Clients and Inserted In To DelClients
        /// <summary>
        /// POST method for Confirmation on Client Delete
        /// Create by: Nilesh Jadhav
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            //// check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.delete_clients.ToString()) == false)
            {
                return View("NoAccess");
            }

            var delClient = db.DelClients.Where(x => x.ID_NO == id);

            if (delClient != null)
            {
                if (delClient.Count() > 0)
                {
                    TempData["message"] = "Please delete the client from deleted client.";
                    TempData["class"] = "error-msg";
                    return Json(new { Success = false });
                }
            }

            var cli = db.ClientAccounts.Where(x => x.ClientID == id);

            if (cli != null)
            {
                if (cli.Count() > 0)
                {
                    TempData["message"] = "Client can not be deleted as it has reference in client account.";
                    TempData["class"] = "error-msg";
                    return Json(new { Success = false });
                }
            }

            var clientPlansUnlimited = db.ClientPlansUnlimiteds.Where(x => x.ClientID == id);

            if (clientPlansUnlimited != null)
            {
                if (clientPlansUnlimited.Count() > 0)
                {
                    TempData["message"] = "Client can not be deleted as it has reference in clientPlansUnlimited.";
                    TempData["class"] = "error-msg";
                    return Json(new { Success = false });
                }
            }

            Client client = db.Clients.Find(id.Trim());
            Client obj = new Client();
            DelClient objDel = new DelClient();
            List<DelClient> list2 = new List<DelClient>();
            obj = db.Clients.Where(x => x.ID_NO == id.Trim()).First();
            objDel.ID_NO = obj.ID_NO;
            objDel.PROFILE = obj.PROFILE;
            objDel.FAMILY = obj.FAMILY;
            objDel.ADDRESS = obj.ADDRESS;
            objDel.GIVEN = obj.GIVEN;
            objDel.PHONE = obj.PHONE;
            objDel.CITY = obj.CITY;
            objDel.STATE = obj.STATE;
            objDel.ZIP = obj.ZIP;
            objDel.COUNTRY = obj.COUNTRY;
            objDel.COMMENT = obj.COMMENT; ;
            objDel.ACC_BALANCE1 = 0;
            objDel.ACC_LIMIT1 = 0;
            objDel.ACC_UNLIMITED1 = 0;
            objDel.ACC_BALANCE2 = 0;
            objDel.ACC_LIMIT2 = 0;
            objDel.ACC_UNLIMITED2 = 0;
            objDel.ACC_BALANCE3 = 0;
            objDel.ACC_LIMIT3 = 0;
            objDel.ACC_UNLIMITED3 = 0;
            objDel.PLAN_START1 = System.DateTime.Now;
            objDel.PLAN_START2 = System.DateTime.Now;
            objDel.PLAN_START3 = System.DateTime.Now;
            objDel.TAG_CODE = obj.FAMILY;
            objDel.TAG_NO = obj.FAMILY;
            objDel.TAG_ISSUE = System.DateTime.Now;
            objDel.TAG_EXPIRY = System.DateTime.Now;
            objDel.TAG_STATUS = 1;
            objDel.LastMealTime = System.DateTime.Now;
            objDel.WeekMealCount = 0;
            objDel.LastPlanCode = 0;
            objDel.HoldExpiry = System.DateTime.Now;
            objDel.HoldRegisterID = 0;
            objDel.Faculty = obj.Faculty;
            objDel.PrOfStudy = obj.PrOfStudy;
            objDel.Department = obj.Department;
            objDel.Title = obj.Title;
            objDel.EMail = obj.EMail;
            objDel.NetID = obj.NetID;
            objDel.Allow_Undefined = 0;
            objDel.RefundLastMealTime = System.DateTime.Now;
            objDel.RefundLastPlanCode = 0;
            objDel.ACC_BALANCE4 = 0;
            objDel.ACC_LIMIT4 = 0;
            objDel.ACC_UNLIMITED4 = 0;
            objDel.ACC_BALANCE5 = 0;
            objDel.ACC_LIMIT5 = 0;
            objDel.ACC_UNLIMITED5 = 0;
            objDel.Allow_FreePrint = 0;
            objDel.WeekDayCount = 0;
            objDel.EasyConvert_Admin = 0;
            objDel.WeekMealCount2 = 0;
            objDel.WeekMealCount3 = 0;
            objDel.DOB = System.DateTime.Now;
            objDel.LanguagePreference = 0;
            objDel.TermCondAccept = 0;
            objDel.TermCondAcceptDate = System.DateTime.Now;
            objDel.LastMealTime2 = System.DateTime.Now;
            objDel.LastMealTime3 = System.DateTime.Now;
            objDel.GradeLevel = obj.GradeLevel;
            objDel.ResidencyStatus = obj.ResidencyStatus;
            objDel.EmailFlag = 0;
            objDel.PinMustChange = 0;
            objDel.LockoutAttempts = 0;
            objDel.Lockout = 0;

            db.DelClients.Add(objDel);
            try
            {
                db.Clients.Remove(obj);
                db.SaveChanges();
                TempData["message"] = "Client deleted successfully.";
                TempData["class"] = "success-msg";

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error occurred in deleting a client.";
                TempData["class"] = "error-msg";
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
