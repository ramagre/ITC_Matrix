/*
   FileFor : Managing the Client Profile Model
   FileName : PayMethodController.cs
   Created Date : 14-12-2015
   Created By : Sandip Katore
   Modified Date :
*/

using System;
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
    public class PayMethodController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.accounts_paymentTypes.ToString();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Index get actionmethod for listing the paymethods with searching ,sorting and pagination
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="search"></param>
        /// <param name="sortBy"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(int?searchBy,string search,string sortBy,int? page)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_pmt_methods.ToString()) == false)
            {
                return View("NoAccess");
            }

            ViewBag.CurrentPage = page;
            Session["page"] = page;
            // check module permission for hiding the buttons.
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_pmt_methods.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            int pageSize = Common.Functions.GetPageSize();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
            }

            ViewBag.PayMethodMessage = TempData["PayMethodmessage"];
            ViewBag.Class = TempData["class"];
            var paymethodList = db.PayMethods.ToList();

            // filter client list based on profile right assigned to operator role
            // var lstpermittedpayMethods = Common.Functions.GetPermittedPayMethods();
            //paymethodList = paymethodList.Where(x => lstpermittedpayMethods.Any(y => y.CODE == x.CODE)).ToList();


            if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(search))
            {              
                paymethodList = paymethodList.OrderBy(x => x.CODE).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(search))
            {
                paymethodList = paymethodList.OrderBy(x => x.DSCR).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Code && search != string.Empty)
            {
                paymethodList = paymethodList.Where(x => x.CODE.ToString().Contains(search)).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Description && search != string.Empty)
            {   
                paymethodList = paymethodList.Where(x => x.DSCR.ToLower().Contains(search)).ToList();
            }
            else
            {
                paymethodList = paymethodList.ToList();
            }
            // for sorting
            ViewBag.CODESort = String.IsNullOrEmpty(sortBy) ? "CODE desc" : "";
            ViewBag.DSCRSort = sortBy == "DSCR" ? "DSCR desc" : "DSCR";          

            switch (sortBy)
            {
                case "CODE desc":
                    paymethodList = paymethodList.OrderByDescending(x => x.CODE).ToList();
                    break;

                case "CODE":
                    paymethodList = paymethodList.OrderBy(x => x.CODE).ToList();
                    break;

                case "DSCR desc":
                    paymethodList = paymethodList.OrderByDescending(x => x.DSCR).ToList();
                    break;

                case "DSCR":
                    paymethodList = paymethodList.OrderBy(x => x.DSCR).ToList();
                    break;

                default:
                    paymethodList.OrderByDescending(x => x.CODE).ToList();
                    break;
            }
            return View(paymethodList.ToPagedList(page ?? 1, pageSize));
        }
        #endregion

        #region PayMethod/Create Get Method
        // GET: 
        [HttpGet]
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_pmt_methods.ToString()) == false)
            {
                return View("NoAccess");
            }

            return View();
        }
        #endregion

        #region PayMethod/Create Post Method
        // POST: 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="payMethod"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODE,DSCR,TYPE,TRANSTYPE")] PayMethod payMethod)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_pmt_methods.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.PayMethods.Add(payMethod);
                    db.SaveChanges();
                    TempData["PayMethodmessage"] = "PayMethod added successfully.";
                    TempData["class"] = "success-msg";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while processing :",ex);
                    ViewBag.Error = ex.ToString();

                    return View("Error");
                }
             
            }

            return View(payMethod);
        }

        #endregion

        #region PayMethod/Edit Get Method
        
        // GET: PayMethod/Edit/5
        public ActionResult Edit(short? id)
        {           
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_pmt_methods.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayMethod payMethod = db.PayMethods.Find(id);
            if (payMethod == null)
            {
                return HttpNotFound();
            }
            return View(payMethod);
        }

        #endregion

        #region PayMethod/Edit Post Method
                
        // POST: PayMethod/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODE,DSCR,TYPE,TRANSTYPE")] PayMethod payMethod)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_pmt_methods.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(payMethod).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["PayMethodmessage"] = "PayMethod Updated Successfully.";
                    TempData["class"] = "success-msg";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while processing :",ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }               
            }
            return View(payMethod);
        }

        #endregion

        #region PayMethod/Delete/ Get Method
        
        // GET: PayMethod/Delete/5
        [HttpPost, ActionName("Delete")]    
        public ActionResult DeleteConfirmed(short id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_pmt_methods.ToString()) == false)
            {
                return View("NoAccess");
            }
            try
            {
                PayMethod payMethod = db.PayMethods.Find(id);
                db.PayMethods.Remove(payMethod);
                db.SaveChanges();
                TempData["PayMethodmessage"] = "PayMethod Deleted Successfully.";
                TempData["class"] = "success-msg";
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :",ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }         
        }

        #endregion

        #region User Define Function for checking code is exist or not in database?
        public JsonResult IsCodeAvailble(short? CODE)
        {
            int code = Convert.ToInt32(CODE);
            var result = true;
            var PayMethodCode = db.PayMethods.Where(x => x.CODE.ToString().Trim() == code.ToString().Trim()).FirstOrDefault();

            if (PayMethodCode != null)
                result = false;
            return Json(result, JsonRequestBehavior.AllowGet);
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
