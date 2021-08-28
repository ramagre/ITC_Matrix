
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ITC_Matrix.Models;
using System.Text;
using System.Collections;
using ITC_Matrix.Common;
using PagedList;

namespace ITC_Matrix.Controllers
{
    public class AccountCodesController : Controller
    {
        #region ---------- Class Variables ----------
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.clients_departments.ToString();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region ---------- Action Methods ----------
        #region Index Listing Method with searching,sorting and pagination(GET INDEX)       
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="search"></param>
        /// <param name="sortBy"></param>
        /// <param name="page"></param>
        /// <returns>search result</returns>
        [HttpGet]
        public ActionResult Index(int? searchBy, string search, string sortBy, int? page)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_department.ToString()) == false)
            {
                return View("NoAccess");
            }

            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_department.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            ViewBag.message = TempData["message"];
            ViewBag.Class = TempData["class"];
            //getting the page size
            int pageSize = Common.Functions.GetPageSize();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
            }

            var CostCentreName = (from t1 in db.AccountCodes
                                  join t2 in db.CostCentres on t1.CostCentreID equals t2.CostCentreID
                                  select new { t1.AccCode, t1.CostCentreID, t2.CostCentreName, t1.Description, t1.Accounts });

            List<AccountCode> partsList = new List<AccountCode>();

            foreach (var item in CostCentreName)
            {
                AccountCode lstact = new AccountCode();
                lstact.AccCode = item.AccCode;
                lstact.CostCentreID = item.CostCentreID;
                lstact.CostCentreName = item.CostCentreName;
                lstact.Description = item.Description;
                lstact.Accounts = item.Accounts;
                partsList.Add(lstact);
            }

            // data for dropdown list
            ViewBag.ActCodelist = new MultiSelectList(db.Accounts.ToList(), "Code", "DSCR");

            //for sorting purpose
            ViewBag.CodeSort = String.IsNullOrEmpty(sortBy) ? "Code desc" : "";
            ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";

            if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(search))
            {
                partsList = partsList.OrderBy(x => x.AccCode).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(search))
            {
                partsList = partsList.OrderBy(x => x.Description).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Code && search != string.Empty)
            {
                partsList = partsList.Where(x => x.AccCode.ToString().Contains(search)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Description && search != string.Empty)
            {
                partsList = partsList.Where(x => x.Description.ToLower().Contains(search)).ToList();
            }
            else
            {
                partsList = partsList.ToList();
            }
            switch (sortBy)
            {
                case "Code desc":
                    partsList = partsList.OrderByDescending(x => x.AccCode).ToList();
                    break;
                case "Code":
                    partsList = partsList.OrderBy(x => x.AccCode).ToList();
                    break;
                case "Description desc":
                    partsList = partsList.OrderByDescending(x => x.Description).ToList();
                    break;
                case "Description":
                    partsList = partsList.OrderBy(x => x.Description).ToList();
                    break;
                default:
                    partsList = partsList.OrderBy(x => x.AccCode).ToList();
                    break;
            }
            return View(partsList.ToPagedList(page ?? 1, pageSize));
        }

        #endregion

        #region create get actionmethod

        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_department.ToString()) == false)
            {
                return View("NoAccess");
            }
            ViewBag.ActCodelist = new MultiSelectList(db.Accounts.ToList(), "Code", "DSCR");
            ViewBag.CostCentreList = new SelectList(db.CostCentres.ToList(), "CostCentreID", "CostCentreName");
            return View();
        }
        #endregion

        #region create post actionmethod for creating the new account type
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="Accountslist"></param>
        /// <param name="CostCentreList"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccCode,CostCentreID,Description,Accounts,CostCentreList")] AccountCode accountCode, IEnumerable<string> Accountslist, int CostCentreList)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_department.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (ModelState.IsValid)
            {
                accountCode.CostCentreID = CostCentreList;
                StringBuilder sb = new StringBuilder();

                if (Accountslist != null)
                {
                    sb.Append(string.Join(",", Accountslist));
                    accountCode.Accounts = sb.ToString() + ",";
                }
                else
                {
                    accountCode.Accounts = string.Empty;
                }

                if (accountCode.Description == null)
                {
                    accountCode.Description = string.Empty;
                }

                try
                {
                    db.AccountCodes.Add(accountCode);
                    db.SaveChanges();
                    TempData["message"] = "Department added successfully.";
                    TempData["class"] = "success-msg";
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Error occurred in adding a department.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }

                return RedirectToAction("Index");
            }

            return View(accountCode);
        }

        #endregion

        #region Edit get action method for updating the accoun type.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_department.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            id = id.Trim();
            AccountCode accountCode = db.AccountCodes.Find(id);
            if (accountCode == null)
            {
                return HttpNotFound();
            }
            var QueryAccounts = from act in db.AccountCodes
                                where act.AccCode == id
                                select act.Accounts;

            var AllAccounts = db.Accounts.ToList();

            ArrayList al = new ArrayList();

            foreach (var item in QueryAccounts)
            {
                string[] split = item.Split(',');
                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].ToString().Trim() != string.Empty)
                        al.Add(split[i].ToString());
                }
            }

            var SelectedList = new List<SelectListItem>();
            var NotSelectedList = new List<SelectListItem>();

            foreach (var item in AllAccounts)
            {
                if (al.Contains(item.CODE.ToString().Trim()))
                {
                    SelectedList.Add(new SelectListItem()
                    {
                        Value = Convert.ToString(item.CODE),
                        Text = item.DSCR,
                    });
                }
                else
                {
                    NotSelectedList.Add(new SelectListItem()
                    {
                        Value = Convert.ToString(item.CODE),
                        Text = item.DSCR,
                    });
                }
            }

            TempData["SelActCodelist"] = new MultiSelectList(SelectedList.ToList().AsEnumerable(), "Value", "Text");
            ViewBag.NotSelActCodelist = new MultiSelectList(NotSelectedList.ToList().AsEnumerable(), "Value", "Text");
            ViewBag.CostCentreList = new SelectList(db.CostCentres.ToList(), "CostCentreID", "CostCentreName", accountCode.CostCentreID);
            return View(accountCode);
        }

        #endregion

        #region Edit post actiomethod for updating the account type
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="selectedProductString"></param>
        /// <param name="Accountslist"></param>
        /// <param name="PrevSelectaccountslist"></param>
        /// <param name="CostCentreList"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccCode,CostCentreID,Description,Accounts")] AccountCode accountCode, string selectedProductString, IEnumerable<string> Accountslist, IEnumerable<string> PrevSelectaccountslist, string CostCentreList)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_department.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (ModelState.IsValid)
            {
                var lst = Request.Form["Account"];

                if (selectedProductString != string.Empty)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Join(",", selectedProductString));
                    accountCode.Accounts = string.Concat(sb.ToString(), ",");
                }
                else
                {
                    accountCode.Accounts = string.Empty;
                }

                if (accountCode.Description == null)
                {
                    accountCode.Description = string.Empty;
                }

                accountCode.CostCentreID = Convert.ToInt32(CostCentreList);
                try
                {
                    db.Entry(accountCode).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = "Department updated successfully.";
                    TempData["class"] = "success-msg";
                    return RedirectToAction("Index");
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
            return View(accountCode);
        }
        #endregion

        #region Delete actionmethod for deleting the account type. 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_department.ToString()) == false)
            {
                return View("NoAccess");
            }
            try
            {
                AccountCode accountCode = db.AccountCodes.Find(id);
                db.AccountCodes.Remove(accountCode);
                db.SaveChanges();
                TempData["message"] = "Department deleted successfully.";
                TempData["class"] = "success-msg";
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error occurred in deleting a Department.";
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
        #endregion

        #region ---------- User Defined Functions ----------

        #region user define function for Checking  If Account Code Already Exist
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccCode"></param>
        /// <returns></returns>
        public JsonResult ISAccountCodeExist(string AccCode)
        {
            var result = true;
            var code_id = db.AccountCodes.Where(x => x.AccCode.Trim() == AccCode.ToString().Trim()).FirstOrDefault();

            if (code_id != null)
                result = false;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
      
    }
}
