/*
   FileFor : Managing the Client Profile Model
   FileName : ProfileController.cs
   Created Date : 22-11-2015
   Created By : Sandip Katore
   Modified Date :12-8-2015
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;

namespace ITC_Matrix.Controllers
{
    public class ProfileController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.clients_profiles.ToString();

        #region GET Index method for filtering records based on given input
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        // GET
        [HttpGet]
        public ActionResult Index(int? searchBy, string txtSearch,int? page,string sortBy)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_profiles.ToString()) == false)
            {
                return View("NoAccess");
            }

            // check module permission for hiding the buttons.
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_profiles.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            //Converting the search text into lower case
            if (!string.IsNullOrEmpty(txtSearch)) {
                txtSearch = txtSearch.ToLower().Trim();
            }

           int pageSize =Common.Functions.GetPageSize();

            ViewBag.CodeSort = String.IsNullOrEmpty(sortBy) ? "Code desc" : "";
            ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";

            var profiles = db.Profiles.AsQueryable();

            // message showing insert/update/delete status
            ViewBag.Profilemessage = TempData["Profilemessage"];
            ViewBag.Class = TempData["class"];

            //TempData.Keep("message");
            //TempData.Keep("class");


            if (searchBy == (int)CommonEnum.SearchMethod.Code  && string.IsNullOrEmpty(txtSearch))
            {
                profiles = db.Profiles.OrderBy(x => x.CODE);              
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(txtSearch))
            {
                 profiles = db.Profiles.OrderBy(x => x.DSCR);            
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Code && txtSearch != string.Empty)
            {
               profiles = db.Profiles.Where(x => x.CODE.ToString().Contains(txtSearch));
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Description && txtSearch != string.Empty)
            {
                 profiles = db.Profiles.Where(x => x.DSCR.ToLower().Contains(txtSearch));
            }
            else
            {
                 profiles = db.Profiles;
            }
            switch (sortBy)
            {
                case "Code desc":
                    profiles = profiles.OrderByDescending(x => x.CODE);
                    break;

                case "Code":
                    profiles = profiles.OrderBy(x => x.CODE);
                    break;

                case "Description desc":
                    profiles = profiles.OrderByDescending(x => x.DSCR);
                    break;

                case "Description":
                    profiles = profiles.OrderBy(x => x.DSCR);
                    break;

                default:
                    profiles = profiles.OrderBy(x => x.CODE);
                    break;
            }
            return View(profiles.ToPagedList(page ?? 1, pageSize));

        }

        #endregion

        #region Get Method for Creating new client Profile
        /// <summary>
        /// GET method for creating new client
        /// </summary>
        /// <returns></returns>
        // GET: Profile/Create
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_profiles.ToString()) == false)
            {
                return View("NoAccess");
            }
            
            List<DayOfWeek> objDayOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();

            ViewBag.DayOfWeek = new SelectList(objDayOfWeek);
            //for binding dropdown list to the days of month
            var list = new List<string>();

            for (int i = 1; i <= 31; i++)
            {
                list.Add(i.ToString("00"));
            } 
            ViewBag.ResetDayOfMonth = new SelectList(list);           

            // for binding the discount percentage dropdown
            var discountpercentage = new List<string>();

            for (int i = 0 ; i <=100; i++)
            {
                discountpercentage.Add(string.Concat(i.ToString() , "%"));               
            }

            SelectList discountdropdown = new SelectList(discountpercentage);
            ViewBag.ddlDiscountPercent = discountdropdown;
            
            ViewBag.AccCode = new SelectList(db.Accounts.ToList(), "CODE", "DSCR");

            // Dropdown for Form Time hours

            var fromHH = new List<string>();

            for (int i = 0; i <= 23; i++)
            {
                fromHH.Add(i.ToString("00"));               
            }

            ViewBag.FromHH = new SelectList(fromHH);

            // Dropdown for Form Time Minute

            var fromMM = new List<string>();

            for (int i = 0; i <= 59; i++)
            {
                fromMM.Add(i.ToString("00"));               
            }

            ViewBag.FromMM = new SelectList(fromMM);

            return View();
        }

        #endregion

        #region POST method for Creating client Profile record
        /// <summary>
        /// POST method for saving client records
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        // POST: Profile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODE,DSCR,DiscountPercent,ResetBalanceEnabled,ResetFrequency,ResetBalanceValue,ResetDayOfWeek,ResetDayOfMonth,ResetBalanceFrom,ResetBalanceTo,ResetBalanceLast,IsCashOnly,RemoveTax1,RemoveTax2,RemoveTax3,RemoveTax4,RemoveTax5,ProCompleteNonTaxableAccount,AccCode,SecondDSCR,ResetBalanceEnabledBool,RemoveTax4Bool,RemoveTax1Bool,RemoveTax2Bool,RemoveTax3Bool,RemoveTax5Bool,ProCompleteNonTaxableAccountBool,ResetFrequencyBool,FromHH,FromMM,ToHH,ToMM,WeekDays")] Profile profile,string txtResetBalanceValue,string ddlDiscountPercent)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_profiles.ToString()) == false)
            {
                return View("NoAccess");
            }
            
            //removing the % sign 
            profile.DiscountPercent = Convert.ToInt32(ddlDiscountPercent.Replace("%", ""));

            if (string.IsNullOrEmpty(txtResetBalanceValue))
            {
                profile.ResetBalanceValue = 0;
            }
            else
            {
                profile.ResetBalanceValue = Convert.ToInt32(txtResetBalanceValue);
            }

            SetDefaultValues(profile);

            if (ModelState.IsValid)
            {
                try
                {
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                    TempData["Profilemessage"] = "Profile added successfully.";
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
            return View(profile);
        }

        #endregion

        #region  user define function for setting default values
        /// <summary>
        /// user define function for setting default values
        /// </summary>
        /// <param name="profile"></param>
        public void SetDefaultValues(Profile profile)
        {
            
            if (profile.ResetBalanceValue > 0) {
                profile.ResetBalanceValue = profile.ResetBalanceValue;
            }
            else
            {
                profile.ResetBalanceValue = 0;
            }

            if (profile.AccCode != 1) {
                profile.AccCode = profile.AccCode;
            }

            if (profile.FromHH != 0 || profile.FromMM != 0)
            {
                profile.ResetBalanceFrom = Convert.ToInt16((60 * profile.FromHH) + profile.FromMM);
            }

            if (profile.ToHH != 0 || profile.ToMM != 0)
            {
                profile.ResetBalanceTo = Convert.ToInt16((60 * profile.ToHH) + profile.ToMM);
            }

            if (string.IsNullOrEmpty(profile.DSCR))
            {
                profile.DSCR = string.Empty;
            }

            if (profile.DiscountPercent == 0 )
            {
                profile.DiscountPercent = 0;
            }
           

            if (profile.ResetBalanceEnabled == 0)
            {
                profile.ResetBalanceEnabled = 0;
            }

            if (profile.ResetBalanceValue == 0)
            {
                profile.ResetBalanceValue = 0;
            }
            else
            {
                profile.ResetBalanceValue = 100 * profile.ResetBalanceValue;
            }

            
            if (profile.ResetDayOfWeek ==1)
            {
                profile.ResetDayOfWeek = profile.WeekDays;
            }

            if (profile.ResetDayOfMonth == 1)
            {
                profile.ResetDayOfMonth = profile.ResetDayOfMonth;
            }

            if (profile.ResetBalanceFrom.Equals(string.Empty))
            {
                profile.ResetBalanceFrom = 0;
            }

            if (profile.ResetBalanceTo.Equals(string.Empty))
            {
                profile.ResetBalanceTo = 0;
            }

            if (profile.ResetBalanceLast == null)
            {
                profile.ResetBalanceLast = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy/MM/dd"));
            }

            if (profile.IsCashOnly.Equals(string.Empty))
            {
                profile.IsCashOnly = 0;
            }

            if (profile.RemoveTax1.Equals(string.Empty))
            {
                profile.RemoveTax1 = 0;
            }

            if (profile.RemoveTax2.Equals(string.Empty))
            {
                profile.RemoveTax2 = 0;
            }

            if (profile.RemoveTax3.Equals(string.Empty))
            {
                profile.RemoveTax3 = 0;
            }

            if (profile.RemoveTax4.Equals(string.Empty))
            {
                profile.RemoveTax4 = 0;
            }

            if (profile.RemoveTax5.Equals(string.Empty))
            {
                profile.RemoveTax5 = 0;
            }

            if (profile.ProCompleteNonTaxableAccount.Equals(string.Empty))
            {
                profile.ProCompleteNonTaxableAccount = 0;
            }
            
            if (string.IsNullOrEmpty(profile.SecondDSCR))
            {
                profile.SecondDSCR = string.Empty;
            }
        }

        #endregion

        #region GET method for Updating the client details Profile
        /// <summary>
        /// GET method for updating the client details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Profile/Edit/5
        public ActionResult Edit(short? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_profiles.ToString()) == false)
            {
                return View("NoAccess");
            }
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }

            // Converting the existing resetBalanceFrom value into hours and minutes
            var resetBalanceFrom = (from p in db.Profiles where p.CODE ==id select p.ResetBalanceFrom).Single() ;

            if (resetBalanceFrom > 0) {
                short totalTimeFrom = Convert.ToInt16(resetBalanceFrom);

                profile.FromHH = Convert.ToInt16(totalTimeFrom / 60);
                profile.FromMM = Convert.ToInt16(totalTimeFrom % 60);
            }

            // Converting the existing resetBalanceTo value into hours and minutes

            var resetBalanceTo = (from p in db.Profiles where p.CODE == id select p.ResetBalanceTo).Single();
            if (resetBalanceTo > 0)
            {
                short totalTimeTo = Convert.ToInt16(resetBalanceTo);

                profile.ToHH = Convert.ToInt16(totalTimeTo / 60);
                profile.ToMM = Convert.ToInt16(totalTimeTo % 60);
            }
            //for binding dropdown list to the days of month
            var ResetDayOfMonth = new List<string>();
                    
            for (short i = 1; i <= 31; i++)
            {
                ResetDayOfMonth.Add(i.ToString("00"));             
            }      
                  
            ViewBag.ResetDayOfMonth = new SelectList(ResetDayOfMonth, profile.ResetDayOfMonth.ToString("00")).ToList().AsEnumerable();
           

            // for binding the discount percentage dropdown
            var discountpercentage = new List<string>();

            for (int i = 0; i <= 100; i++)
            {
                discountpercentage.Add(string.Concat(i,"%"));
            }

            ViewBag.ddlDiscountPercent = new SelectList(discountpercentage,string.Concat(profile.DiscountPercent, "%")).ToList().AsEnumerable();         

            //dropdown for weekdays

            var listItems = new List<ListItem>
              {
                    new ListItem { Text = "Sunday", Value="1" },
                    new ListItem { Text = "Monday", Value="2" },
                    new ListItem { Text = "Tuesday", Value="3" },
                    new ListItem { Text = "Wednesday", Value="4" },
                    new ListItem { Text = "Thursday", Value="5" },
                    new ListItem { Text = "Friday", Value="6" },
                     new ListItem { Text = "Saturday", Value="7" }
              };

            ViewBag.ResetDayOfWeek = new SelectList(listItems, "Value", "Text",profile.ResetDayOfWeek).AsEnumerable();

            // Dropdown for Form Time hours

            var fromHH = new List<string>();

            for (short i = 0; i <= 23; i++)
            {
                fromHH.Add(i.ToString("00"));
            }
          
            ViewBag.FromHH = new SelectList(fromHH.ToList(), profile.FromHH.ToString("00"));//for resetBalanceFrom
            ViewBag.ToHH = new SelectList(fromHH.ToList(), profile.ToHH.ToString("00"));//for resetBalanceTo


            // Dropdown for Form Time Minute

            var fromMM = new List<string>();

            for (short i = 0; i <= 59; i++)
            {
                fromMM.Add(i.ToString("00"));               
            }

            ViewBag.fromMM = new SelectList(fromMM, profile.FromMM.ToString("00"));//for resetBalanceFrom
            ViewBag.ToMM = new SelectList(fromMM, profile.ToMM.ToString("00"));//for resetBalanceTo

            //selected value for dropdown box in edit mode
            ViewBag.AccCode = new SelectList(db.Accounts.ToList(), "CODE", "DSCR", profile.AccCode);

            return View(profile);
        }

        #endregion

        #region POST method for Updating the client Profile Details
        /// <summary>
        /// POST method for Updating the client Details
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODE,DSCR,DiscountPercent,ResetBalanceEnabled,ResetBalanceValue,ResetFrequency,ResetDayOfWeek,ResetDayOfMonth,ResetBalanceFrom,ResetBalanceTo,ResetBalanceLast,IsCashOnly,RemoveTax1,RemoveTax2,RemoveTax3,RemoveTax4,RemoveTax5,ResetFrequencyBool,ProCompleteNonTaxableAccount,AccCode,SecondDSCR,ResetBalanceEnabledBool,RemoveTax4Bool,RemoveTax1Bool,RemoveTax2Bool,RemoveTax3Bool,RemoveTax5Bool,ProCompleteNonTaxableAccountBool,ResetFrequencyBool,FromHH,FromMM,ToHH,ToMM")] Profile profile,string ddlDiscountPercent)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_profiles.ToString()) == false)
            {
                return View();
            }

            int value = profile.ResetBalanceValue;
            profile.DiscountPercent = Convert.ToInt32(ddlDiscountPercent.Replace("%", ""));            
            SetDefaultValues(profile);
                        
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(profile).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Profilemessage"] = "Profile updated successfully.";
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
            return View(profile);
        }

        #endregion

        #region GET method for deleting the client Profile details
        /// <summary>
        /// GET method for the deleting the client details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Profile/Delete/5
        [HttpPost]
        public ActionResult Delete(short? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_profiles.ToString()) == false)
            {
                return View("NoAccess");
            }
            try
            {
                Profile profile = db.Profiles.Find(id);
                db.Profiles.Remove(profile);
                db.SaveChanges();
                TempData["Profilemessage"] = "Profile deleted successfully.";
                TempData["class"] = "success-msg";
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :",ex);
                ViewBag.Error = ex.ToString();
                return View("Error");

            }
            //return RedirectToAction("Index");
        }

        #endregion
        
        #region User Define function for Checking the ClientId exist or not?
        /// <summary>
        /// User Define function for Checking the ClientId exist or not?
        /// </summary>
        /// <param name="ID_NO"></param>
        /// <returns></returns>

        public JsonResult IsClientProfileIDAvailble(short? CODE)
        {
            int code = Convert.ToInt32(CODE);
            var result = true;
            var code_id = db.Profiles.Where(x => x.CODE.ToString().Trim() == code.ToString().Trim()).FirstOrDefault();

            if (code_id != null)
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
