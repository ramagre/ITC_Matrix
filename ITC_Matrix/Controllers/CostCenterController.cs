/*  
    File Name :CostCenterController.cs
    File For :Manage the Cost Center Model
    Created Date : 30-11-2015
    created by : Sandip Katore
    Modified Date : 11-12-2015
*/
using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ITC_Matrix.Controllers
{
    public class CostCenterController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.clients_costcenters.ToString();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region  GET method for Sorting the Cost Center 
        /// <summary>
        /// POST method for Sorting the Cost Center 
        /// </summary>
        /// <param name="CostCentreID"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        // POST : Cost Center 

        [HttpGet]
        public ActionResult Index(int? searchBy, string search,string sortBy,int? page) //for Search Functionality
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_cost_centre.ToString()) == false)
            {
                return View("NoAccess");
            }

            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_cost_centre.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().ToString().Trim();
            }

            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();

            int pageSize = Functions.GetPageSize();

            // message showing insert/update/delete status
            ViewBag.CostCenterMessage = TempData["CostCentermessage"];
            ViewBag.Class = TempData["class"];

            //TempData.Keep("message");
            //TempData.Keep("class");
          

            //for sorting
            ViewBag.CodeSort = String.IsNullOrEmpty(sortBy) ? "Code desc" : "";
            ViewBag.NameSort = sortBy == "Name" ? "Name desc" : "Name";

            List<CostCentre> lstCostCentre = db.CostCentres.ToList();

            //search conditions
            if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(search))
            {
                lstCostCentre = lstCostCentre.Where(x => x.CostCentreID.ToString().Contains(search)).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Code && search != string.Empty)
            {
                lstCostCentre = lstCostCentre.Where(x => x.CostCentreID.ToString().Contains(search)).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(search))
            {
                lstCostCentre = lstCostCentre.Where(x => x.CostCentreName.ToLower().StartsWith(search)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Description && search != string.Empty)
            {
                lstCostCentre = lstCostCentre.Where(x => x.CostCentreName.ToLower().StartsWith(search)).ToList();
            }
            else
            {
                lstCostCentre = lstCostCentre.ToList();
            }

            //sorting
            switch (sortBy)
            {
                case "Code desc":
                    lstCostCentre = lstCostCentre.OrderByDescending(x => x.CostCentreID).ToList();
                    break;

                case "Code":
                    lstCostCentre = lstCostCentre.OrderBy(x => x.CostCentreID).ToList();
                    break;

                case "Name desc":
                    lstCostCentre = lstCostCentre.OrderByDescending(x => x.CostCentreName).ToList();
                    break;

                case "Name":
                    lstCostCentre = lstCostCentre.OrderBy(x => x.CostCentreName).ToList();
                    break;

                default:
                    lstCostCentre = lstCostCentre.OrderBy(x => x.CostCentreID).ToList();
                    break;
            }            
            return View(lstCostCentre.ToPagedList(page ?? 1, pageSize));
        }

        #endregion

        #region   GET create action method
        // GET: CostCenter/Create
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_cost_centre.ToString()) == false)
            {
                return View();
            }

            return View();
        }
        #endregion

        #region Create Post Method for creating the new Cost Center
        /// <summary>
        /// GET method for creating the new Cost Center
        /// </summary>
        /// <returns></returns>
        // POST: CostCenter/Create


        [HttpPost]        
        public ActionResult Create([Bind(Include = "CostCentreID,CostCentreName")] CostCentre costCentre)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_cost_centre.ToString()) == false)
            {
                return View();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.CostCentres.Add(costCentre);
                    db.SaveChanges();
                    TempData["CostCentermessage"] = "Cost Center added successfully.";
                    TempData["class"] = "success-msg";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["CostCentermessage"] = "Error occurred in adding a Cost Center.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing :",ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");

                }

            }

            return View(costCentre);
        }

        #endregion
        
        public ActionResult Edit(int? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_cost_centre.ToString()) == false)
            {
                return View();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CostCentre costCentre = db.CostCentres.Find(id);

            if (costCentre == null)
            {
                return HttpNotFound();
            }

            return View(costCentre);
        }

        #region Edit POST method for Updating Cost Center
        /// <summary>
        /// Post method for creating the new Cost Center
        /// </summary>
        /// <returns></returns>
        // POST: CostCenter/Edit/5  

        [HttpPost]       
        public ActionResult Edit([Bind(Include = "CostCentreID,CostCentreName")] CostCentre costCentre)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_cost_centre.ToString()) == false)
            {
                return View();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(costCentre).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["CostCentermessage"] = "Cost Center updated successfully.";
                    TempData["class"] = "success-msg";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["CostCentermessage"] = "Error occurred in updating a cost center.";
                    ViewBag.MessageClass = "error-msg";
                    logger.Error("Error occurred while processing :",ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }               
            }

            return View(costCentre);
        }
        #endregion

        #region
        // GET: CostCenter/Delete/5
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_cost_centre.ToString()) == false)
            {
                return View();
            }

            // check if the cost centre is assigned to any department
            bool isDelete = true;
            List<AccountCode> lstAccountCode = new List<AccountCode>();

            lstAccountCode = db.AccountCodes.ToList().FindAll(x => x.CostCentreID == id);

            if (lstAccountCode.Count > 0)
            {
                isDelete = false;
            }

            if (isDelete)
            {
                CostCentre costCentre = db.CostCentres.Find(id);
                db.CostCentres.Remove(costCentre);
                db.SaveChanges();

                TempData["CostCentermessage"] = "Cost Center deleted successfully.";
                TempData["class"] = "success-msg";
                return Json(new { Success = true });
            }
            else
            {
                TempData["CostCentermessage"] = "Cost Center can not be deleted as it is assigned to department(s).";
                TempData["class"] = "error-msg";
                return Json(new { Success = true });
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

        #region User Define function for Checking the CostCentreID exist or not?
        /// <summary>
        /// User Define function for Checking the CostCentreID exist or not?
        /// </summary>
        /// <param name="CostCentreID"></param>
        /// <returns></returns>

        public JsonResult IsCostCentreIDExist(string CostCentreID)
        {

            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
            var result = true;
            int code = Convert.ToInt32(CostCentreID);
            var code_id = db.CostCentres.Where(x => x.CostCentreID.ToString()== code.ToString()).FirstOrDefault();

            if (code_id != null)
                result = false;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
