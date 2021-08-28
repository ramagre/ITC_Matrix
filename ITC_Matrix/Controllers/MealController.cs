/*
   FileFor : Managing the Client Profile Model
   FileName : MealController.cs
   Created Date : 17-12-2015
   Created By : Sandip Katore
   Modified Date :18-12-2015
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
    public class MealController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.plans_types.ToString();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region Index get actionmethod for listing the MealTypes with searching ,sorting and pagination
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="search"></param>
        /// <param name="sortBy"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(int? searchBy, string search, string sortBy, int? page)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_meal_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            // check module permission for hiding buttons
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission. manage_meal_plans.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            //getting the page size
            int pageSize = Common.Functions.GetPageSize();

            //converting the search string into lower case
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
            }

            //status messages for Insert/Upadate/Delete 
            ViewBag.Mealmessage = TempData["Mealmessage"];
            ViewBag.Class = TempData["class"];
            //TempData.Keep("message");
            //TempData.Keep("class");
            var mealsList = db.Meals.ToList();
            var mealCount = mealsList.Count();

            if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(search))
            {
                mealsList = mealsList.OrderBy(x => x.CODE).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(search))
            {
                mealsList = mealsList.OrderBy(x => x.DSCR).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Code && search != string.Empty)
            {
                mealsList = mealsList.Where(x => x.CODE.ToString().Contains(search)).ToList();
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Description && search != string.Empty)
            {
                mealsList = mealsList.Where(x => x.DSCR.ToLower().Contains(search)).ToList();
            }
            else
            {
                mealsList = mealsList.ToList();
            }

            // for sorting

            ViewBag.CODESort = String.IsNullOrEmpty(sortBy) ? "CODE desc" : "";
            ViewBag.DSCRSort = sortBy == "DSCR" ? "DSCR desc" : "DSCR";
            ViewBag.FromSort = sortBy == "From" ? "From desc" : "From";
            ViewBag.ToSort = sortBy == "To" ? "To desc" : "To";

            switch (sortBy)
            {
                case "CODE desc":
                    mealsList = mealsList.OrderByDescending(x => x.CODE).ToList();
                    break;

                case "CODE":
                    mealsList = mealsList.OrderBy(x => x.CODE).ToList();
                    break;

                case "DSCR desc":
                    mealsList = mealsList.OrderByDescending(x => x.DSCR).ToList();
                    break;

                case "DSCR":
                    mealsList = mealsList.OrderBy(x => x.DSCR).ToList();
                    break;

                case "From desc":
                    mealsList = mealsList.OrderByDescending(x => x.FromTime).ToList();
                    break;

                case "From":
                    mealsList = mealsList.OrderBy(x => x.FromTime).ToList();
                    break;

                case "To desc":
                    mealsList = mealsList.OrderByDescending(x => x.ToTime).ToList();
                    break;

                case "To":
                    mealsList = mealsList.OrderBy(x => x.ToTime).ToList();
                    break;

                default:
                    mealsList.OrderByDescending(x => x.CODE).ToList();
                    break;
            }

            return View(mealsList.ToPagedList(page ?? 1, pageSize));
        }
        #endregion

        #region Create Get method 
        // GET: Meal/Create
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            ViewBag.Mealmessage = TempData["Mealmessage"];
            ViewBag.Class = TempData["class"];

            //TempData.Keep("message");
            //TempData.Keep("class");

            List<int> lstMealType = new List<int>();
            List<Meal> lstMeal = new List<Meal>();

            lstMeal = db.Meals.ToList();

            ViewBag.lstMealCount = lstMeal.Count();

            for (int i = 1; i <= 5; i++)
            {
                if (lstMeal.FindAll(x => x.CODE == i).Count() == 0)
                {
                    lstMealType.Add(i);
                }
            }

            ViewBag.lstMealType = new SelectList(lstMealType);

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

        #region Create Post Method for creating the Meal Type

        // POST: Meal/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="meal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODE,DSCR,FromTime,ToTime,FromHH,FromMM,ToMM,ToHH")] Meal meal)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            CalculateFromToTime(meal);
            List<Meal> lstMeal = db.Meals.ToList();

            if (ModelState.IsValid)
            {
                if (meal.FromTime == meal.ToTime)
                {
                    TempData["Mealmessage"] = "Start and End time not equal.";
                    TempData["class"] = "error-msg";
                    return RedirectToAction("Create");
                }

                if (meal.ToTime < meal.FromTime)
                {
                    TempData["Mealmessage"] = "Meal Time Overlapped.";
                    TempData["class"] = "error-msg";
                    return RedirectToAction("Create");
                }

                var FromTime = lstMeal.FindAll(x => x.FromTime == meal.FromTime).ToList();
                var ToTime = lstMeal.FindAll(x => x.ToTime == meal.ToTime).ToList();

                if (FromTime.Count == 0)
                {
                    db.Meals.Add(meal);
                    db.SaveChanges();
                    TempData["Mealmessage"] = "Meal type added successfully.";
                    TempData["class"] = "success-msg";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mealmessage"] = "Meal time overlapped.";
                    TempData["class"] = "error-msg";
                    return RedirectToAction("Create");
                }
            }

            return View(meal);
        }

        #endregion

        #region user Define Function for Calculting the FromTime and ToTime
        void CalculateFromToTime(Meal meal)
        {
            if (meal.FromHH != 0 || meal.FromMM != 0)
            {
                meal.FromTime = Convert.ToInt16((60 * meal.FromHH) + meal.FromMM);
            }
            else
            {
                meal.FromTime = 0;
            }
            if (meal.ToHH != 0 || meal.ToMM != 0)
            {
                meal.ToTime = Convert.ToInt16((60 * meal.ToHH) + meal.ToMM);
            }
            else
            {
                meal.ToTime = 0;
            }
        }
        #endregion

        #region  Edit Get Method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Meal/Edit/5
        public ActionResult Edit(short? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            ViewBag.Mealmessage = TempData["Mealmessage"];
            ViewBag.Classs = TempData["class"];

            //TempData.Keep("message");
            //TempData.Keep("class");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meal meal = db.Meals.Find(id);

            // Converting the existing resetBalanceFrom value into hours and minutes
            var FromTime = (from p in db.Meals where p.CODE == id select p.FromTime).Single();

            if (FromTime > 0)
            {
                short totalTimeFrom = Convert.ToInt16(FromTime);

                meal.FromHH = Convert.ToInt16(totalTimeFrom / 60);
                meal.FromMM = Convert.ToInt16(totalTimeFrom % 60);
            }

            // Converting the existing resetBalanceTo value into hours and minutes

            var ToTime = (from p in db.Meals where p.CODE == id select p.ToTime).Single();
            if (ToTime > 0)
            {
                short totalTimeTo = Convert.ToInt16(ToTime);

                meal.ToHH = Convert.ToInt16(totalTimeTo / 60);
                meal.ToMM = Convert.ToInt16(totalTimeTo % 60);
            }

            // Dropdown for Form Time hours

            var fromHH = new List<string>();

            for (short i = 0; i <= 23; i++)
            {
                fromHH.Add(i.ToString("00"));
            }

            ViewBag.FromHH = new SelectList(fromHH.ToList(), meal.FromHH.ToString("00"));//for resetBalanceFrom
            ViewBag.ToHH = new SelectList(fromHH.ToList(), meal.ToHH.ToString("00"));//for resetBalanceTo


            // Dropdown for Form Time Minute

            var fromMM = new List<string>();

            for (short i = 0; i <= 59; i++)
            {
                fromMM.Add(i.ToString("00"));
            }

            ViewBag.FromMM = new SelectList(fromMM, meal.FromMM.ToString("00"));//for resetBalanceFrom
            ViewBag.ToMM = new SelectList(fromMM, meal.ToMM.ToString("00"));//for resetBalanceTo

            if (meal == null)
            {
                return HttpNotFound();
            }
            return View(meal);
        }

        #endregion

        #region Edit Post Method for updating the Meal Type
        // POST: Meal/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="meal"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODE,DSCR,FromTime,ToTime,FromHH,FromMM,ToMM,ToHH")] Meal meal, FormCollection fc)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_types.ToString()) == false)
            {
                return View("NoAccess");
            }

            CalculateFromToTime(meal);

            Meal objMeal = new Meal();
            objMeal.CODE = meal.CODE;
            objMeal.DSCR = meal.DSCR;

            objMeal.FromTime = meal.FromTime;
            objMeal.ToTime = meal.ToTime;

            if (ModelState.IsValid)
            {
                try
                {
                    if (meal.FromTime == meal.ToTime)
                    {
                        TempData["Mealmessage"] = "Start and end time not equal.";
                        TempData["class"] = "error-msg";
                        return RedirectToAction("Edit");
                    }

                    if (meal.ToTime < meal.FromTime)
                    {
                        TempData["Mealmessage"] = "Meal time overlapped.";
                        TempData["class"] = "error-msg";
                        return RedirectToAction("Edit");
                    }

                    List<Meal> lstMeal = db.Meals.ToList();

                    var overlapTime = lstMeal.FindAll(x => x.FromTime == meal.FromTime && x.ToTime == meal.ToTime && x.CODE != meal.CODE).ToList();

                    int id = meal.CODE;
                    if (overlapTime.Count == 0)     // does not exist in database
                    {
                        meal = db.Meals.Find(id);
                        db.Meals.Remove(meal);
                        db.SaveChanges();

                        db.Meals.Add(objMeal);
                        db.SaveChanges();

                        TempData["Mealmessage"] = "Meal type updaed successfully.";
                        TempData["class"] = "success-msg";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Mealmessage"] = "Meal time overlapped.";
                        TempData["class"] = "error-msg";
                        return RedirectToAction("Edit");
                    }

                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
            }
            return View(meal);
        }

        #endregion

        #region Delete Get Method for deletin the meal type

        // POST: Meal/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(short id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_types.ToString()) == false)
            {
                return View("NoAccess");
            }
            try
            {
                Meal meal = db.Meals.Find(id);
                db.Meals.Remove(meal);
                db.SaveChanges();
                TempData["Mealmessage"] = "Meal type deleted successfully.";
                TempData["class"] = "success-msg";
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
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
