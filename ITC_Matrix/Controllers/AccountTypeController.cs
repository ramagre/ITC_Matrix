/*  
    File Name : ClientController.cs
    File For :Manage the Client Model
    Created Date : 30-11-2015
    created by : Sandip Katore
    Modified Date : 4-12-2015
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
    public class AccountTypeController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.accounts_types.ToString();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Index action method with listing,searching and pagination functionality
        [HttpGet]
        public ActionResult Index(int? searchBy, string txtSearch, int? page, string sortBy)//for Search Functionality parameter
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_acct_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            // check module delete permission for hiding the delete button
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_acct_types.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            if (!string.IsNullOrEmpty(txtSearch))
            {
                txtSearch = txtSearch.ToLower().Trim();
            }


            int pageSize = Common.Functions.GetPageSize();
            ViewBag.CODE = new SelectList(db.Accounts, "CODE", "DSCR"); // preselect item in selectlist by Code param
            ////binding data to the TotalUse Dropdown
            ViewBag.TotalUse = new SelectList(db.Accounts.ToList(), "CODE", "DSCR");


            // message showing insert/update/delete status
            ViewBag.AccountTypeMessage = TempData["AccountTypemessage"];
            ViewBag.Class = TempData["class"];

            //TempData.Keep("message");
            //TempData.Keep("class");

            //for sorting
            ViewBag.CodeSort = String.IsNullOrEmpty(sortBy) ? "Code desc" : "";
            ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";

            List<Account> accountList = db.Accounts.ToList();

            //search conditions
            if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(txtSearch))
            {
                accountList = accountList.OrderBy(x => x.CODE).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(txtSearch))
            {
                accountList = accountList.OrderBy(x => x.DSCR).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Code && txtSearch != string.Empty)
            {
                accountList = accountList.Where(x => x.CODE.ToString().Contains(txtSearch)).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Description && txtSearch != string.Empty)
            {
                accountList = accountList.Where(x => x.DSCR.ToLower().Contains(txtSearch)).ToList();
            }
            else
            {
                accountList = accountList.ToList();
            }

            //sorting
            switch (sortBy)
            {
                case "Code desc":
                    accountList = accountList.OrderByDescending(x => x.CODE).ToList();
                    break;

                case "Code":
                    accountList = accountList.OrderBy(x => x.CODE).ToList();
                    break;

                case "Description desc":
                    accountList = accountList.OrderByDescending(x => x.DSCR).ToList();
                    break;

                case "Description":
                    accountList = accountList.OrderBy(x => x.DSCR).ToList();
                    break;

                default:
                    accountList = accountList.OrderBy(x => x.CODE).ToList();
                    break;
            }
            return View(accountList.ToPagedList(page ?? 1, pageSize));
        }
        #endregion

        #region  create GET method
        /// <summary>
        /// create GET method 
        /// </summary>
        /// <returns></returns>
        // GET: AccountType/Create
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.su_acct_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            var profiles= Common.Functions.GetPermittedProfiles();

            if (profiles != null) {
                ViewBag.profiles = profiles;
            }

            return View();
        }

        #endregion

        #region Create Post method for adding the new account type
        /// <summary>
        /// for adding the new record 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="selectedProfiles"></param>
        /// <returns></returns>
        // POST: AccountType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODE,DSCR,TotalUse,FreePrint,SecondDSCR,TaxExempt,TransferLimitFrom,AllowCreditTransfer,ExcludeFromBalance,MinTransactionAmount,AllowFree_Print,AllowFreeBool")] Account account, string selectedProfiles)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.su_acct_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (string.IsNullOrEmpty(account.TotalUse))
            {
                account.TotalUse = string.Empty;
            }

            string UnlimitedAccount = string.Empty;
            string acc = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    db.Accounts.Add(account);

                    // code for adding entry in assignpermissiontask module.           
                    List<string> accountref = new List<string> { "Add Money", "Deduct Money", "View Balance", "Statement", "Set Credit Limit" };

                    foreach (var item in accountref)
                    {
                        permissionsTask objpermissionsTask = new permissionsTask();
                        objpermissionsTask.taskgroup = string.Concat("UnlimitedAccount-", account.DSCR);

                        objpermissionsTask.taskname = string.Concat(account.DSCR, " ", item);

                        if (item.ToLower().Contains("add money"))
                        {
                            objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.addMoney);
                            objpermissionsTask.taskdescription = string.Concat("Operator can add money to ", account.DSCR);
                        }

                       else if (item.ToLower().Contains("deduct money"))
                        {
                            objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.deductMoney);
                            objpermissionsTask.taskdescription = string.Concat("Operator can deduct money from ", account.DSCR);
                        }

                        else if (item.ToLower().Contains("view balance"))
                        {
                            objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.balance);
                            objpermissionsTask.taskdescription = string.Concat("Operator can view the account balance of ", account.DSCR);
                        }

                        else if (item.ToLower().Contains("statement"))
                        {
                            objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.statement);
                            objpermissionsTask.taskdescription = string.Concat("Operator can view the statement for ", account.DSCR);
                        }

                        else if (item.ToLower().Contains("set credit limit"))
                        {
                            objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.setCredit);
                            objpermissionsTask.taskdescription = string.Concat("Operator can set the credit limit for ", account.DSCR);
                        }

                        objpermissionsTask.status = 1;

                        dbwmn.permissionsTasks.Add(objpermissionsTask);
                        dbwmn.SaveChanges();
                    }
                    db.SaveChanges();
                    TempData["AccountTypemessage"] = "Account type added successfully.";
                    TempData["class"] = "success-msg";
                }

                catch (Exception ex)
                {
                    TempData["AccountTypemessage"] = "Error occurred in adding a account type.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
            }

            AccProfileWebDeposit objAccProfileWebDeposit = new AccProfileWebDeposit();

            if (selectedProfiles != string.Empty)
            {
                try
                {
                    string[] arr = selectedProfiles.Split(',');
                    if (arr != null)
                    {
                        foreach (var item in arr)
                        {
                            objAccProfileWebDeposit = new AccProfileWebDeposit();
                            objAccProfileWebDeposit.ACC_CODE = account.CODE;
                            objAccProfileWebDeposit.PROFILE = Convert.ToInt16(item);

                            db.AccProfileWebDeposits.Add(objAccProfileWebDeposit);
                            db.SaveChanges();
                            TempData["AccountTypemessage"] = "Account type added successfully.";
                            TempData["class"] = "success-msg";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["AccountTypemessage"] = "Error occurred in updating a department.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }

            }

            return RedirectToAction("Index");
        }

        #endregion

        #region User Define function for Checking the Accound_Code exist or not?
        /// <summary>
        /// User Define function for Checking the Accound_Code exist or not?
        /// </summary>
        /// <param name="CODE"></param>
        /// <returns></returns>

        public JsonResult doesCodeIDExist(string CODE)  //For Existing Code Check
        {
            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
            var result = true;
            int code = Convert.ToInt32(CODE);
            var code_id = db.Accounts.Where(x => x.CODE.ToString() == code.ToString().Trim()).FirstOrDefault();

            if (code_id != null)
                result = false;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Edit Get method for editing the account type details  
        /// <summary>
        /// Edit Get method for editing the account type details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        // GET: AccountType/Edit/5
        public ActionResult Edit(short? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_acct_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);

            List<Profile> lstProfile = Common.Functions.GetPermittedProfiles();
            List<AccProfileWebDeposit> lstAccProfileWebDeposit = db.AccProfileWebDeposits.ToList();

            List<AccProfileWebDeposit> lstTemp = new List<AccProfileWebDeposit>();

            //showing the selected chechbox checked in edit mode
            lstTemp = lstAccProfileWebDeposit.FindAll(x => x.ACC_CODE == id);

            for (int i = 0; i < lstTemp.Count; i++)
            {
                for (int j = 0; j < lstProfile.Count(); j++)
                {
                    if (lstProfile[j].CODE == lstTemp[i].PROFILE)
                    {
                        lstProfile[j].isSelected = true;
                    }
                }
            }
          
            ViewBag.Profiles = lstProfile;

            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        #endregion

        #region POST Edit actiomethod for updating the account type

        // POST: AccountType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODE,DSCR,TotalUse,FreePrint,SecondDSCR,TaxExempt,TransferLimitFrom,AllowCreditTransfer,ExcludeFromBalance,MinTransactionAmount,AllowFree_Print,AllowFreeBool")] Account account, string selectedProfiles)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_acct_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            //code for updating the permission task module                        
            bool isExist = false;
            short accountCode = account.CODE;

            List<permissionsTask> lstpermissionsTask = dbwmn.permissionsTasks.Where(x => x.taskref.StartsWith(string.Concat("acc",accountCode.ToString(),"_"))).ToList();

            if (lstpermissionsTask.Count() > 5) {
                TempData["AccountTypemessage"] = "Error occurred while updating the account type";              
                TempData["class"] = "error-msg";
            }
            if (lstpermissionsTask.Count() > 0) {
                isExist = true;
            }

            if (isExist == true)
            {
                //need to update the permissiontask module.                     
                List<string> accountref = new List<string> { "Add Money", "Deduct Money", "View Balance", "Statement", "Set Credit Limit" };

                foreach (var item in accountref)
                {
                    permissionsTask objpermissionsTask = new permissionsTask();

                    objpermissionsTask.taskgroup = string.Concat("UnlimitedAccount-", account.DSCR);

                    objpermissionsTask.taskname = string.Concat(account.DSCR, " ", item);

                    if (item.ToLower().Contains("add money"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.addMoney);

                        int taskid = lstpermissionsTask.Where(x => x.taskref.EndsWith("_addMoney")).Select(y => y.taskid).SingleOrDefault();
                        if (taskid > 0)
                        {
                            objpermissionsTask.taskid = taskid;
                        }

                        objpermissionsTask.taskdescription = string.Concat("Operator can add money to ", account.DSCR);
                    }
                    else if (item.ToLower().Contains("deduct money"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.deductMoney);

                        int taskid = lstpermissionsTask.Where(x => x.taskref.EndsWith("_deductMoney")).Select(y => y.taskid).SingleOrDefault();

                        if (taskid != 0)
                        {
                            objpermissionsTask.taskid = taskid;
                        }

                        objpermissionsTask.taskdescription = string.Concat("Operator can deduct money from ", account.DSCR);
                    }
                    else if (item.ToLower().Contains("view balance"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.balance);

                        int taskid = lstpermissionsTask.Where(x => x.taskref.EndsWith("_balance")).Select(y => y.taskid).SingleOrDefault();

                        if (taskid != 0)
                        {
                            objpermissionsTask.taskid = taskid;
                        }

                        objpermissionsTask.taskdescription = string.Concat("Operator can view the account balance of ", account.DSCR);
                    }
                    else if (item.ToLower().Contains("statement"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.statement);

                        int taskid = lstpermissionsTask.Where(x => x.taskref.EndsWith("_statement")).Select(y => y.taskid).SingleOrDefault();

                        if (taskid != 0)
                        {
                            objpermissionsTask.taskid = taskid;
                        }

                        objpermissionsTask.taskdescription = string.Concat("Operator can view the statement for ", account.DSCR);
                    }
                    else if (item.ToLower().Contains("set credit limit"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.setCredit);

                        int taskid = lstpermissionsTask.Where(x => x.taskref.EndsWith("_setCredit")).Select(y => y.taskid).SingleOrDefault();

                        if (taskid != 0)
                        {
                            objpermissionsTask.taskid = taskid;
                        }

                        objpermissionsTask.taskdescription = string.Concat("Operator can set the credit limit for ", account.DSCR);
                    }

                    objpermissionsTask.status = 1;                
                    permissionsTask modify = new permissionsTask();

                    modify = dbwmn.permissionsTasks.Find(objpermissionsTask.taskid);

                    if (modify != null)
                    {
                        modify.taskdescription = objpermissionsTask.taskdescription;
                        modify.status = objpermissionsTask.status;
                        modify.taskname = objpermissionsTask.taskname;
                        modify.taskgroup = objpermissionsTask.taskgroup;
                        modify.taskref = objpermissionsTask.taskref;
                        dbwmn.Entry(modify).State = EntityState.Modified;
                        dbwmn.SaveChanges();
                    }
                }
            }
            else
            {
                //need to add the permissiontask module.               
                List<string> accountref = new List<string> { "Add Money", "Deduct Money", "View Balance", "Statement", "Set Credit Limit" };

                foreach (var item in accountref)
                {
                    permissionsTask objpermissionsTask = new permissionsTask();
                    objpermissionsTask.taskgroup = string.Concat("UnlimitedAccount-", account.DSCR);

                    objpermissionsTask.taskname = string.Concat(account.DSCR, " ", item);

                    if (item.ToLower().Contains("add money"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.addMoney);
                        objpermissionsTask.taskdescription = string.Concat("Operator can add money to ", account.DSCR);
                    }

                    else if (item.ToLower().Contains("deduct money"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.deductMoney);
                        objpermissionsTask.taskdescription = string.Concat("Operator can deduct money from ", account.DSCR);
                    }

                    else if (item.ToLower().Contains("view balance"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.balance);
                        objpermissionsTask.taskdescription = string.Concat("Operator can view the account balance of ", account.DSCR);
                    }

                    else if (item.ToLower().Contains("statement"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.statement);
                        objpermissionsTask.taskdescription = string.Concat("Operator can view the statement for ", account.DSCR);
                    }

                    else if (item.ToLower().Contains("set credit limit"))
                    {
                        objpermissionsTask.taskref = string.Concat("acc", account.CODE, "_", CommonEnum.Unlimitedaccounts.setCredit);
                        objpermissionsTask.taskdescription = string.Concat("Operator can set the credit limit for ", account.DSCR);
                    }

                    objpermissionsTask.status = 1;

                    dbwmn.permissionsTasks.Add(objpermissionsTask);
                    dbwmn.SaveChanges();
                }
            }

            //-----------------------------------
            short code = account.CODE;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(account).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AccountTypemessage"] = "Account type updated successfully.";
                    TempData["class"] = "success-msg";
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
            }

            AccProfileWebDeposit objAccProfileWebDeposit = new AccProfileWebDeposit();

            string[] arr = selectedProfiles.Split(',');

            if (arr != null)
            {
                List<AccProfileWebDeposit> objAccProfileWebDeposite = db.AccProfileWebDeposits.Where(x => x.ACC_CODE == code).ToList();

                if (objAccProfileWebDeposite != null && objAccProfileWebDeposite.Count() > 0)
                {
                    foreach (var item in objAccProfileWebDeposite)
                    {
                        db.AccProfileWebDeposits.Remove(item);
                        db.SaveChanges();
                    }
                }

                foreach (var item in arr)
                {
                    if (!item.Equals(string.Empty))
                    {
                        objAccProfileWebDeposit = new AccProfileWebDeposit();
                        objAccProfileWebDeposit.ACC_CODE = account.CODE;
                        objAccProfileWebDeposit.PROFILE = Convert.ToInt16(item);

                        db.AccProfileWebDeposits.Add(objAccProfileWebDeposit);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(account);
        }

        #endregion

        #region Delete actiomethod for deleting the account type
        /// <summary>
        /// Actiomethod for deleting the account type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Delete(short? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.su_acct_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            List<ClientAccount> lstClientAccount = new List<ClientAccount>();
            lstClientAccount = db.ClientAccounts.ToList().FindAll(x => x.AccountId == id);

            if (lstClientAccount.Count > 0 && lstClientAccount != null)
            {
                TempData["AccountTypemessage"] = "Account can not be deleted as it is assigned to client.";
                TempData["class"] = "error-msg";
                return Json(new { Success = false });
            }

            else
            {
                try
                {
                    Account account = db.Accounts.Find(id);
                    string accountdrsc = account.DSCR;
                    db.Accounts.Remove(account);
                    db.SaveChanges();

                    string ID = id.ToString();
                    var lstpermissionsTasks = dbwmn.permissionsTasks.Where(x => x.taskgroup.EndsWith(accountdrsc)).ToList();

                    var lstTaskIDs = lstpermissionsTasks.Select(x => x.taskid).ToList();

                    List<permissionsRolesTask> lstpermissionsRolesTask = new List<permissionsRolesTask>();

                    if (lstTaskIDs != null && lstTaskIDs.Count() > 0)
                    {
                        foreach (var item in lstTaskIDs)
                        {
                            var permissionsRolesTask = dbwmn.permissionsRolesTasks.Where(x => x.taskid.Equals(item)).SingleOrDefault();
                            if (permissionsRolesTask != null)
                            {
                                dbwmn.permissionsRolesTasks.Remove(permissionsRolesTask);
                                dbwmn.SaveChanges();
                            }
                        }
                    }

                    if (lstpermissionsTasks != null && lstpermissionsTasks.Count() > 0)
                    {
                        foreach (var item in lstpermissionsTasks)
                        {
                            if (item != null)
                            {
                                dbwmn.permissionsTasks.Remove(item);
                                dbwmn.SaveChanges();
                            }
                        }
                    }
                    TempData["AccountTypemessage"] = "Account type deleted successfully.";
                    TempData["class"] = "success-msg";
                    return Json(new { Success = true });
                }
                catch (Exception ex)
                {
                    TempData["AccountTypemessage"] = "Error occurred in deleting a Account type.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
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
