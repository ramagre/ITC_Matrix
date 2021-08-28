/*  
    File Name : DeletedClientsController.cs
    File For :Manage the Deleted Clients Model
    Created Date : 23-11-2015
    created by : Nilesh Jadhav
    Modified Date : 
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;


namespace ITC_Matrix.Controllers
{
    public class DeletedClientsController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string moduleName = Common.CommonEnum.SubMenus.clients_deleted.ToString();

        #region POST method for Sorting the Deleted Clients details
        /// <summary>
        /// POST method for Sorting the client details
        /// </summary>
        /// <param name="ID_NO"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        // POST : Deleted Clients

        [HttpGet]
        public ActionResult Index(int? searchBy, string search, int? page, string sortBy)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_client.ToString()) == false)
            {
                return View("NoAccess");
            }

            // Check delete permission and hide button
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_deleted_clients.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            //get the page size
            int pageSize = Common.Functions.GetPageSize();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
            }

            ViewBag.DeletedClientsmessage = TempData["DeletedClientsmessage"];
            ViewBag.Class = TempData["class"];

            //for sorting purpose
            ViewBag.ClientIDSort = String.IsNullOrEmpty(sortBy) ? "ClientID desc" : "";
            ViewBag.FamilySort = sortBy == "Family" ? "Family desc" : "Family";
            ViewBag.ACC_BALANCE1Sort = sortBy == "ACC_BALANCE1" ? "ACC_BALANCE1 desc" : "ACC_BALANCE1";


            List<DelClient> lstDelClient = CheckTransactions(db.DelClients.ToList());

            var Profile_Tbl = (from Client_List in lstDelClient
                               join Profile_List in db.Profiles on new { dealCap = (int)Client_List.PROFILE } equals new { dealCap = (int)Profile_List.CODE }
                               select new
                               {
                                   Client_List.ID_NO,
                                   Client_List.FAMILY,
                                   Client_List.GIVEN,
                                   Client_List.PROFILE,
                                   Client_List.ACC_BALANCE1,
                                   Profile_List.DSCR,
                                   Profile_List.CODE,
                                   Client_List.Transactions
                               }).AsEnumerable();

            List<DelClient> Client_Profile = new List<DelClient>();

            foreach (var m in Profile_Tbl)
            {
                DelClient Profile_code = new DelClient();
                Profile_code.ID_NO = m.ID_NO;
                Profile_code.FAMILY = m.FAMILY;
                Profile_code.GIVEN = m.GIVEN;
                Profile_code.PROFILE = m.PROFILE;
                Profile_code.ACC_BALANCE1 = m.ACC_BALANCE1;
                Profile_code.Profile_DSCR = m.DSCR;
                Profile_code.Transactions = m.Transactions;

                Client_Profile.Add(Profile_code);
            }


            if (search != null && search.Contains(','))
            {
                string[] searchStrings = search.Split(',');
                string firstName = searchStrings[0];
                string lastName = searchStrings[1];

                if (Profile == null && !string.IsNullOrEmpty(search))
                {
                    Client_Profile = Client_Profile.Where(x => x.FAMILY.ToLower().Contains(firstName) && x.GIVEN.ToLower().Contains(lastName)).ToList();
                }

                if (Client_Profile != null && Client_Profile.Count > 0)
                {
                    Client_Profile = Client_Profile.ToList();
                }

                if (Profile == null && !string.IsNullOrEmpty(search))
                {
                    Client_Profile = Client_Profile.Where(x => x.GIVEN.ToLower().Contains(lastName) && x.FAMILY.ToLower().Contains(firstName) || x.NetID.Contains(search) || x.ID_NO.Contains(search)).ToList();
                }

            }

            if (searchBy == Convert.ToInt32(CommonEnum.SearchDeletedClient.Code) && string.IsNullOrEmpty(search))
            {
                Client_Profile = Client_Profile.Where(x => x.ID_NO.ToString().Contains(search)).ToList();
            }

            else if (searchBy == Convert.ToInt32(CommonEnum.SearchDeletedClient.Family_Name) && string.IsNullOrEmpty(search))
            {
                Client_Profile = Client_Profile.Where(x => x.FAMILY.ToLower().Contains(search)).ToList();
            }

            else if (searchBy == Convert.ToInt32(CommonEnum.SearchDeletedClient.Given_Name) && string.IsNullOrEmpty(search))
            {
                Client_Profile = Client_Profile.Where(x => x.GIVEN.ToLower().Contains(search)).ToList();
            }

            else if (searchBy == Convert.ToInt32(CommonEnum.SearchDeletedClient.Family_Given_Name) && string.IsNullOrEmpty(search))
            {
                Client_Profile = Client_Profile.Where(x => x.FAMILY.ToLower().Contains(search) && x.GIVEN.ToLower().Contains(search)).ToList();
            }

            else if (searchBy == Convert.ToInt32(CommonEnum.SearchDeletedClient.Given_Name_Family_Name) && string.IsNullOrEmpty(search))
            {
                Client_Profile = Client_Profile.Where(x => x.GIVEN.ToLower().Contains(search) || x.FAMILY.ToLower().Contains(search)).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchDeletedClient.Code && !string.IsNullOrEmpty(search))
            {
                Client_Profile = Client_Profile.Where(x => x.ID_NO.ToString().Contains(search)).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchDeletedClient.Given_Name && !string.IsNullOrEmpty(search))
            {
                if (search.Contains(','))
                {
                    string[] searchStrings = search.Split(',');
                    string firstName = searchStrings[0];
                    Client_Profile = Client_Profile.Where(x => x.GIVEN.ToLower().Contains(firstName)).ToList();
                }
                else
                {
                    Client_Profile = Client_Profile.Where(x => x.GIVEN.ToLower().Contains(search)).ToList();
                }

            }

            else if (searchBy == (int)CommonEnum.SearchDeletedClient.Family_Name && !string.IsNullOrEmpty(search))
            {
                if (search.Contains(','))
                {
                    string[] searchStrings = search.Split(',');
                    string lastName = searchStrings[1];
                    Client_Profile = Client_Profile.Where(x => x.FAMILY.ToLower().Contains(lastName)).ToList();
                }
                else
                {
                    Client_Profile = Client_Profile.Where(x => x.FAMILY.ToLower().Contains(search)).ToList();
                }
            }

            else if (searchBy == (int)CommonEnum.SearchDeletedClient.Family_Given_Name && !string.IsNullOrEmpty(search))
            {
                if (search.Contains(','))
                {
                    string[] searchStrings = search.Split(',');
                    string firstName = searchStrings[0];
                    string lastName = searchStrings[1];
                    Client_Profile = Client_Profile.Where(x => x.FAMILY.ToLower().Contains(firstName) || x.GIVEN.ToLower().Contains(lastName)).ToList();
                }
                else
                {
                    Client_Profile = Client_Profile.Where(x => x.FAMILY.ToLower().Contains(search) || x.GIVEN.ToLower().Contains(search)).ToList();
                }
            }

            else if (searchBy == (int)CommonEnum.SearchDeletedClient.Given_Name_Family_Name && !string.IsNullOrEmpty(search))
            {
                if (search.Contains(','))
                {
                    string[] searchStrings = search.Split(',');
                    string firstName = searchStrings[0];
                    string lastName = searchStrings[1];

                    Client_Profile = Client_Profile.Where(x => x.FAMILY.ToLower().Contains(firstName) || x.GIVEN.ToLower().Contains(lastName)).ToList();
                }
                else
                {
                    Client_Profile = Client_Profile.Where(x => x.GIVEN.ToLower().Contains(search) || x.FAMILY.ToLower().Contains(search)).ToList();
                }
            }

            else if (searchBy == (int)CommonEnum.SearchDeletedClient.Family_Name && !string.IsNullOrEmpty(search))
            {
                Client_Profile = Client_Profile.Where(x => x.GIVEN.ToLower().Contains(search)).ToList();
            }

            else
            {
                Client_Profile = Client_Profile.ToList().ToList();
            }

            switch (sortBy)
            {
                case "ClientID desc":
                    Client_Profile = Client_Profile.OrderByDescending(x => x.ID_NO).ToList();
                    break;
                case "ClientID":
                    Client_Profile = Client_Profile.OrderBy(x => x.ID_NO).ToList();
                    break;

                case "Family desc":
                    Client_Profile = Client_Profile.OrderByDescending(x => x.FAMILY).ToList();
                    break;
                case "Family":
                    Client_Profile = Client_Profile.OrderBy(x => x.FAMILY).ToList();
                    break;

                case "ACC_BALANCE1 desc":
                    Client_Profile = Client_Profile.OrderByDescending(x => x.ACC_BALANCE1).ToList();
                    break;

                case "ACC_BALANCE1":
                    Client_Profile = Client_Profile.OrderBy(x => x.ACC_BALANCE1).ToList();
                    break;

                default:
                    Client_Profile = Client_Profile.OrderBy(x => x.ID_NO).ToList();
                    break;
            }

            return View(Client_Profile.ToPagedList(page ?? 1, pageSize));
        }

        #endregion


        // GET: DeletedClients/Details/5
        public ActionResult Details(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName) == false)
            {
                return View();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DelClient delClient = db.DelClients.Find(id);
            if (delClient == null)
            {
                return HttpNotFound();
            }
            return View(delClient);
        }

        // GET: DeletedClients/Create
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_deleted_clients.ToString()) == false)
            {
                return View("NoAccess");
            }

            return View();
        }

        // POST: DeletedClients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_NO,PROFILE,FAMILY,GIVEN,PHONE,ADDRESS,CITY,STATE,ZIP,COUNTRY,COMMENT,ACC_BALANCE1,ACC_LIMIT1,ACC_UNLIMITED1,ACC_BALANCE2,ACC_LIMIT2,ACC_UNLIMITED2,ACC_BALANCE3,ACC_LIMIT3,ACC_UNLIMITED3,PLAN_CODE1,PLAN_CODE2,PLAN_CODE3,PLAN_START1,PLAN_START2,PLAN_START3,TAG_CODE,TAG_NO,TAG_ISSUE,TAG_EXPIRY,TAG_STATUS,LastMealTime,WeekMealCount,LastPlanCode,HoldExpiry,HoldRegisterID,Faculty,PrOfStudy,Department,Title,EMail,NetID,Allow_Undefined,RefundLastMealTime,RefundLastPlanCode,ACC_BALANCE4,ACC_LIMIT4,ACC_UNLIMITED4,ACC_BALANCE5,ACC_LIMIT5,ACC_UNLIMITED5,Allow_FreePrint,WeekDayCount,EasyConvert_Admin,WeekMealCount2,WeekMealCount3,DOB,LanguagePreference,TermCondAccept,TermCondAcceptDate,LastMealTime2,LastMealTime3,GradeLevel,ResidencyStatus,EmailFlag,PinMustChange,LastWebPurchaseLogin,LockoutAttempts,PinChangeDate,Lockout,LastUpdate,ParkMember,OperatorID,AppName,AppID,IPAddress")] DelClient delClient)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_deleted_clients.ToString()) == false)
            {
                return View("NoAccess");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.DelClients.Add(delClient);
                    db.SaveChanges();
                    TempData["DeletedClientsmessage"] = "Record Added Successfully";
                    return RedirectToAction("Index");
                }

                return View(delClient);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
                throw;
            }

        }

        // GET: DeletedClients/Edit/5
        public ActionResult Edit(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_deleted_clients.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DelClient delClient = db.DelClients.Find(id);
            if (delClient == null)
            {
                return HttpNotFound();
            }
            return View(delClient);
        }

        #region DeletedClients/Edit/5
        // POST: DeletedClients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_NO,PROFILE,FAMILY,GIVEN,PHONE,ADDRESS,CITY,STATE,ZIP,COUNTRY,COMMENT,ACC_BALANCE1,ACC_LIMIT1,ACC_UNLIMITED1,ACC_BALANCE2,ACC_LIMIT2,ACC_UNLIMITED2,ACC_BALANCE3,ACC_LIMIT3,ACC_UNLIMITED3,PLAN_CODE1,PLAN_CODE2,PLAN_CODE3,PLAN_START1,PLAN_START2,PLAN_START3,TAG_CODE,TAG_NO,TAG_ISSUE,TAG_EXPIRY,TAG_STATUS,LastMealTime,WeekMealCount,LastPlanCode,HoldExpiry,HoldRegisterID,Faculty,PrOfStudy,Department,Title,EMail,NetID,Allow_Undefined,RefundLastMealTime,RefundLastPlanCode,ACC_BALANCE4,ACC_LIMIT4,ACC_UNLIMITED4,ACC_BALANCE5,ACC_LIMIT5,ACC_UNLIMITED5,Allow_FreePrint,WeekDayCount,EasyConvert_Admin,WeekMealCount2,WeekMealCount3,DOB,LanguagePreference,TermCondAccept,TermCondAcceptDate,LastMealTime2,LastMealTime3,GradeLevel,ResidencyStatus,EmailFlag,PinMustChange,LastWebPurchaseLogin,LockoutAttempts,PinChangeDate,Lockout,LastUpdate,ParkMember,OperatorID,AppName,AppID,IPAddress")] DelClient delClient)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_deleted_clients.ToString()) == false)
            {
                return View("NoAccess");
            }
            try
            {


                if (ModelState.IsValid)
                {
                    db.Entry(delClient).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["DeletedClientsmessage"] = "Record Updated Successfully";
                    return RedirectToAction("Index");
                }
                return View(delClient);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region CheckTransactions method for if Transactionn has value then Locked client 
        /// <summary>
        /// POST method for Deleted clients Forever but Checked on Transaction
        /// if Transaction has value then locked Otherwise Delete Client Forever
        /// </summary>
        ///<param name="ID_NO"></param>
        /// <returns></returns>



        public List<DelClient> CheckTransactions(List<DelClient> lstDelClient)
        {
            int transactions = 0;
            List<DelClient> lstTemp = new List<DelClient>();
            string ClientID = string.Empty;

            for (int i = 0; i < lstDelClient.Count; i++)
            {
                ClientID = lstDelClient[i].ID_NO;

                var no = Convert.ToInt32((from tb1 in db.TrnDeps where tb1.ID_NO.Trim().Equals(ClientID) select tb1.ID_NO).ToList().Count()) +
                       Convert.ToInt32((from tb2 in db.TrnDeps where tb2.ID_NO.Trim().Equals(ClientID) select tb2.ID_NO).ToList().Count()) +
                       Convert.ToInt32((from tb3 in db.TrnPayrols where tb3.ID_NO.Trim().Equals(ClientID) select tb3.ID_NO).ToList().Count()) +
                       Convert.ToInt32((from tb4 in db.TrnDeps where tb4.ID_NO.Trim().Equals(ClientID) select tb4.ID_NO).ToList().Count());

                transactions = Convert.ToInt32(no);
                lstDelClient[i].Transactions = transactions;
            }

            return lstDelClient;

        }
        #endregion

        #region POST method for Deleted clients Basis on Transaction Conditions  
        /// <summary>
        /// POST method for Deleted clients Forever but Checked on Transaction
        /// if Transaction has value then locked Otherwise Delete Client Forever
        /// </summary>
        ///
        /// <returns></returns>
        // POST : Deleted Clients

        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_deleted_clients.ToString()) == false)
            {
                return View("NoAccess");
            }
            try
            {
                DelClient delClient = db.DelClients.Find(id);
                corClientPicture objcorClientPicture = db.corClientPictures.Where(X => X.ClientID.Equals(id)).SingleOrDefault();

                if (objcorClientPicture != null)
                {
                    db.corClientPictures.Remove(objcorClientPicture);
                }

                db.DelClients.Remove(delClient);
                db.SaveChanges();
                TempData["DeletedClientsmessage"] = "Client deleted successfully.";
                TempData["class"] = "success-msg";
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region Post Method  for Reactive Client from Deleted Clients and Inserted In To Clients
        /// <summary>
        /// User Define function for Reactive (Deleted) Client from DelClients and Inserted In To Clients
        /// </summary>
        /// <param name="ID_NO"></param>
        /// <returns></returns>
        // POST : Reactive Clients

        [HttpPost, ActionName("Reactive")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_deleted_clients.ToString()) == false)
            {
                return View("NoAccess");
            }

            DelClient delClient = db.DelClients.Find(id.Trim());
            Client obj = new Client();
            DelClient objDel = new DelClient();
            List<Client> list2 = new List<Client>();
            objDel = db.DelClients.Where(x => x.ID_NO == id.Trim()).First();
            obj.ID_NO = objDel.ID_NO;
            obj.PROFILE = objDel.PROFILE;
            obj.FAMILY = objDel.FAMILY;
            obj.ADDRESS = objDel.ADDRESS;
            obj.GIVEN = objDel.GIVEN;
            obj.PHONE = objDel.PHONE;
            obj.CITY = objDel.CITY;
            obj.STATE = objDel.STATE;
            obj.ZIP = objDel.ZIP;
            obj.COUNTRY = objDel.COUNTRY;
            obj.COMMENT = objDel.COMMENT; ;
            obj.ACC_BALANCE1 = 0;
            obj.ACC_LIMIT1 = 0;
            obj.ACC_UNLIMITED1 = 0;
            obj.ACC_BALANCE2 = 0;
            obj.ACC_LIMIT2 = 0;
            obj.ACC_UNLIMITED2 = 0;
            obj.ACC_BALANCE3 = 0;
            obj.ACC_LIMIT3 = 0;
            obj.ACC_UNLIMITED3 = 0;
            obj.PLAN_START1 = System.DateTime.Now;
            obj.PLAN_START2 = System.DateTime.Now;
            obj.PLAN_START3 = System.DateTime.Now;
            obj.TAG_CODE = objDel.FAMILY;
            obj.TAG_NO = objDel.FAMILY;
            obj.TAG_ISSUE = System.DateTime.Now;
            obj.TAG_EXPIRY = System.DateTime.Now;
            obj.TAG_STATUS = 1;
            obj.LastMealTime = System.DateTime.Now;
            obj.WeekMealCount = 0;
            obj.LastPlanCode = 0;
            obj.HoldExpiry = System.DateTime.Now;
            obj.HoldRegisterID = 0;
            obj.Faculty = objDel.Faculty;
            obj.PrOfStudy = objDel.PrOfStudy;
            obj.Department = objDel.Department;
            obj.Title = objDel.Title;
            obj.EMail = objDel.EMail;
            obj.NetID = objDel.NetID;
            obj.Allow_Undefined = 0;
            obj.RefundLastMealTime = System.DateTime.Now;
            obj.RefundLastPlanCode = 0;
            obj.ACC_BALANCE4 = 0;
            obj.ACC_LIMIT4 = 0;
            obj.ACC_UNLIMITED4 = 0;
            obj.ACC_BALANCE5 = 0;
            obj.ACC_LIMIT5 = 0;
            obj.ACC_UNLIMITED5 = 0;
            obj.Allow_FreePrint = 0;
            obj.WeekDayCount = 0;
            obj.EasyConvert_Admin = 0;
            obj.WeekMealCount2 = 0;
            obj.WeekMealCount3 = 0;
            obj.DOB = System.DateTime.Now;
            obj.LanguagePreference = 0;
            obj.TermCondAccept = 0;
            obj.TermCondAcceptDate = System.DateTime.Now;
            obj.LastMealTime2 = System.DateTime.Now;
            obj.LastMealTime3 = System.DateTime.Now;
            obj.GradeLevel = objDel.GradeLevel;
            obj.ResidencyStatus = objDel.ResidencyStatus;
            obj.EmailFlag = 0;
            obj.PinMustChange = 0;
            obj.LockoutAttempts = 0;
            obj.Lockout = 0;
            try
            {
                db.Clients.Add(obj);
                db.DelClients.Remove(objDel);
                db.SaveChanges();
                TempData["DeletedClientsmessage"] = "Client reactivated successfully.";
                TempData["class"] = "success-msg";
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                TempData["DeletedClientsmessage"] = "Error ocured in deleting a client.";
                TempData["class"] = "error-msg";
                ViewBag.Error = ex.ToString();
                logger.Error("Error occurred while processing :", ex);
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
