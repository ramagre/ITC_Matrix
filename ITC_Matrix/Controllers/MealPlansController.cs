using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace ITC_Matrix.Controllers
{
    public class MealPlansController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.plans_mealplans.ToString();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Index Get actionmethod for listing page,searching and sorting
        
        public ActionResult Index(int? searchBy, string txtSearch, int? page, string sortBy)
        {
            try
            {
                // check module permission
                if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_meal_plans.ToString()) == false)
                {
                    return View("NoAccess");
                }

                // check module delete permission for hiding the buttons.
                if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_plans.ToString()) == false)
                {
                    ViewBag.isPermission = false;
                }

                ViewBag.MealPlansMessage = TempData["MealPlansmessage"];
                ViewBag.Class = TempData["class"];
               
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
                ViewBag.CODESort = String.IsNullOrEmpty(sortBy) ? "CODE desc" : "";
                ViewBag.DSCRSort = sortBy == "DSCR" ? "DSCR desc" : "DSCR";
                ViewBag.DAYSSort = sortBy == "DAYS" ? "DAYS desc" : "DAYS";
                ViewBag.EXPIRY_DATESort = sortBy == "EXPIRY_DATE" ? "EXPIRY_DATE desc" : "EXPIRY_DATE";
                ViewBag.PRICESort = sortBy == "PRICE" ? "PRICE desc" : "PRICE";

                var lstPlans = db.Plans.ToList();

                if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(txtSearch))
                {
                    lstPlans = lstPlans.OrderBy(x => x.CODE).ToList();
                }
                else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(txtSearch))
                {
                    lstPlans = lstPlans.OrderBy(x => x.DSCR).ToList();
                }
                else if (searchBy == (int)CommonEnum.SearchMethod.Code && txtSearch != string.Empty)
                {
                    lstPlans = lstPlans.Where(x => x.CODE.ToString().Contains(txtSearch)).ToList();
                }
                else if (searchBy == (int)CommonEnum.SearchMethod.Description && txtSearch != string.Empty)
                {
                    lstPlans = lstPlans.Where(x => x.DSCR.ToLower().Contains(txtSearch)).ToList();
                }
                else
                {
                    lstPlans = lstPlans.ToList();
                }

                switch (sortBy)
                {
                    case "CODE desc":
                        lstPlans = lstPlans.OrderByDescending(x => x.CODE).ToList();
                        break;

                    case "CODE":
                        lstPlans = lstPlans.OrderBy(x => x.CODE).ToList();
                        break;

                    case "DSCR":
                        lstPlans = lstPlans.OrderBy(x => x.DSCR).ToList();
                        break;

                    case "DSCR desc":
                        lstPlans = lstPlans.OrderByDescending(x => x.DSCR).ToList();
                        break;

                    case "DAYS":
                        lstPlans = lstPlans.OrderBy(x => x.DAYS).ToList();
                        break;

                    case "DAYS desc":
                        lstPlans = lstPlans.OrderByDescending(x => x.DAYS).ToList();
                        break;

                    case "EXPIRY_DATE":
                        lstPlans = lstPlans.OrderBy(x => x.EXPIRY_DATE).ToList();
                        break;

                    case "EXPIRY_DATE desc":
                        lstPlans = lstPlans.OrderByDescending(x => x.EXPIRY_DATE).ToList();
                        break;

                    case "PRICE":
                        lstPlans = lstPlans.OrderBy(x => x.PRICE).ToList();
                        break;

                    case "PRICE desc":
                        lstPlans = lstPlans.OrderByDescending(x => x.PRICE).ToList();
                        break;

                    default:
                        lstPlans = lstPlans.OrderBy(x => x.CODE).ToList();
                        break;
                }

                var CurSign = db.Params.Where(x => x.Name.ToUpper() == "CURRENCYSIGN").SingleOrDefault();

                if (CurSign != null)
                {
                    ViewBag.Sign = CurSign.Value;
                }
                else
                {
                    ViewBag.Sign = string.Empty;
                }

                return View(lstPlans.ToList().ToPagedList(page ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region Create get actionmethod for showing create meal plan form      
        /// <summary>
        /// action method for showing the create view
        /// </summary>
        /// <returns></returns>
        // GET: MealPlans => Create function
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_plans.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                var resultreg = db.Registers.ToList();
                var resultdevice = dbwmn.deviceTypes.ToList();
                var resultdevicegroup = db.RegisterGroups.ToList();

                var result1 = (from t1 in resultreg
                               join t2 in resultdevice
                               on new { commcol = t1.RegisterType } equals new { commcol = t2.id }
                               select new { t1.CODE, t1.DSCR, t2.deviceTypeName });

                List<Register> partsList1 = new List<Register>();
                foreach (var m in result1)
                {
                    Register lstact = new Register();
                    lstact.CODE = m.CODE;
                    lstact.DSCR = m.DSCR;
                    lstact.deviceTypeName = m.deviceTypeName;
                    partsList1.Add(lstact);
                }

                var UniqueDevices = (from t1 in resultreg
                                     join t2 in resultdevice
                                     on new { commcol = t1.RegisterType } equals new { commcol = t2.id }
                                     select new { t1.RegisterType, t2.deviceTypeName, t2.id }).Distinct().OrderBy(x => x.id);
                List<Register> partsList2 = new List<Register>();

                foreach (var m in UniqueDevices)
                {
                    Register lstact = new Register();
                    lstact.RegisterType = m.RegisterType;
                    lstact.deviceTypeName = m.deviceTypeName;
                    lstact.CODE = m.id;
                    partsList2.Add(lstact);
                }

                ViewBag.Devices = partsList1;
                    ViewBag.UniqueDevices = partsList2;

                ViewBag.AllowedProfiles = db.Profiles.Distinct().OrderBy(x => x.CODE).ToList().AsEnumerable();

                ViewBag.AccountID = new MultiSelectList(db.Accounts.ToList(), "CODE", "DSCR");

                var result2 = (from t1 in resultreg
                               join t2 in resultdevicegroup
                               on new { commcol = t1.GROUP_CODE } equals new { commcol = t2.CODE }
                               select new { t1.CODE, t1.DSCR, devicedesc = t2.DSCR });
                List<Register> partsList3 = new List<Register>();

                foreach (var m in result2)
                {
                    Register lstact = new Register();
                    lstact.CODE = m.CODE;
                    lstact.DSCR = m.DSCR;
                    lstact.deviceTypeName = m.devicedesc;
                    partsList3.Add(lstact);
                }

                List<Register> partsList4 = new List<Register>();

                var UniqueDevice_Group = (from t1 in resultreg
                                          join t2 in resultdevicegroup
                                          on new { commcol = t1.GROUP_CODE } equals new { commcol = t2.CODE }
                                          select new { t1.GROUP_CODE, t2.DSCR, t2.CODE }).Distinct().OrderBy(x => x.CODE);

                foreach (var m in UniqueDevice_Group)
                {
                    Register lstact = new Register();
                    lstact.GROUP_CODE = m.GROUP_CODE;
                    lstact.DSCR = m.DSCR;
                    partsList4.Add(lstact);
                }

                ViewBag.Device_Group = partsList3;

                List<RegisterGroup> lstRegisterGroup = Common.Functions.GetPermittedDevice_Group();

                partsList4 = partsList4.Where(x => lstRegisterGroup.Any(y => y.CODE == x.GROUP_CODE)).ToList();

                ViewBag.UniqueDevice_Group = partsList4;

                return View();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region Create Post action method for saving the meal plan details.
        /// <summary>
        /// saving the meal plan details
        /// </summary>
        /// <param name="CODE"></param>
        /// <returns></returns>
        //Get operation fot getting meal plan schedule
        [HttpGet]
        public ActionResult SetPlanSchedule(int CODE)
        {
            try
            {
                TempData["MealPlansmessage"] = string.Empty;
                TempData["class"] = string.Empty;
                // check module permission

                if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.edit_meal_plans.ToString()) == false)
                {
                    return View("NoAccess");
                }

                ViewBag.CodeVal = CODE;
                var objPlan = db.Plans.Where(x => x.CODE == CODE).Single();
                string[] plansperday = objPlan.MEALS.Split(',');
                ViewBag.PlanDesc = objPlan.DSCR;
                List<Meal> lstMeals = new List<Meal>();
                lstMeals = db.Meals.ToList();

                if (lstMeals.Count > 0)
                {
                    List<Meal> lstmeal = new List<Meal>();
                    Meal objMeal = new Meal();

                    for (int i = 0; i < plansperday.Length; i++)
                    {
                        objMeal = lstMeals.Find(x => x.CODE == (i + 1));

                        if (objMeal != null)
                        {
                            objMeal.PlansPerDay = plansperday[i];
                            lstmeal.Add(objMeal);
                        }
                    }

                    for (int i = 0; i < lstmeal.Count; i++)
                    {
                        char[] daychar = lstmeal[i].PlansPerDay.ToCharArray();
                        Meal ml = new Meal();

                        lstmeal[i].sun = daychar[0].ToString();
                        lstmeal[i].mon = daychar[1].ToString();
                        lstmeal[i].tues = daychar[2].ToString();
                        lstmeal[i].wesd = daychar[3].ToString();
                        lstmeal[i].thur = daychar[4].ToString();
                        lstmeal[i].fri = daychar[5].ToString();
                        lstmeal[i].sat = daychar[6].ToString();
                    }

                    ViewBag.AllDay = lstmeal.ToList().AsEnumerable();

                    ViewBag.Plans = plansperday;

                    List<PlanLimit> lstAdvamceRestriction = new List<PlanLimit>();
                    PlanLimit objPlanLimit = new PlanLimit();

                    var PlanLimit = db.PlanLimits.Where(x => x.PlanCode == CODE).ToList();

                    if (PlanLimit.Count > 0)
                    {
                        var Alllines =
                          from tl in db.Meals
                          join j in db.PlanLimits on tl.CODE equals j.MealCode
                          where j.PlanCode == CODE
                          select new
                          {
                              tl.CODE,
                              tl.DSCR,
                              j.IsMealsPerDay,
                              j.MealsPerDay,
                              j.IsMealsPerPlan,
                              j.MealsPerPlan,
                              j.IsAmountPerPlan,
                              j.AmountPerPlan,
                              j.IsAmountPerMeal,
                              j.AmountPerMeal
                          };

                        short divide = 100;
                        foreach (var item in Alllines)
                        {
                            objPlanLimit = new PlanLimit();

                            objPlanLimit.MealCode = item.CODE;
                            objPlanLimit.MealDescription = item.DSCR;
                            objPlanLimit.IsMealsPerDay = item.IsMealsPerDay;
                            objPlanLimit.MealsPerDay = item.MealsPerDay;
                            objPlanLimit.IsMealsPerPlan = item.IsMealsPerPlan;
                            objPlanLimit.MealsPerPlan = item.MealsPerPlan;
                            objPlanLimit.IsAmountPerPlan = item.IsAmountPerPlan;
                            objPlanLimit.AmountPerPlan = item.AmountPerPlan / divide;
                            objPlanLimit.IsAmountPerMeal = item.IsAmountPerMeal;
                            objPlanLimit.AmountPerMeal = item.AmountPerMeal / divide;

                            lstAdvamceRestriction.Add(objPlanLimit);
                        }
                    }
                    else
                    {
                        foreach (var item in lstMeals)
                        {
                            objPlanLimit = new PlanLimit();

                            objPlanLimit.MealCode = item.CODE;
                            objPlanLimit.MealDescription = item.DSCR;
                            objPlanLimit.IsMealsPerDay = 0;
                            objPlanLimit.MealsPerDay = 0;
                            objPlanLimit.IsMealsPerPlan = 0;
                            objPlanLimit.MealsPerPlan = 0;
                            objPlanLimit.IsAmountPerPlan = 0;
                            objPlanLimit.AmountPerPlan = 0;
                            objPlanLimit.IsAmountPerMeal = 0;
                            objPlanLimit.AmountPerMeal = 0;

                            lstAdvamceRestriction.Add(objPlanLimit);
                        }
                    }

                    ViewBag.PlanLimitData = lstAdvamceRestriction;
                    ViewBag.MealType = db.Meals.ToList();
                }
                else
                {
                    ViewBag.plansmessage = "Please add meal types.";
                    ViewBag.plansmessageClass = "error-msg";
                }

                return View();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region SetPlanScheduleInPlan Post actionmethod for set the plan.

        //Post operation for updating meal plan schedule in plan table
        [HttpPost]
        public ActionResult SetPlanScheduleInPlan(FormCollection fc, int Code)
        {
            TempData["MealPlansmessage"] = string.Empty;
            TempData["class"] = string.Empty;
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.edit_meal_plans.ToString()) == false)
            {
                return View("NoAccess");
            }

            List<string> lstsun = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkmealsun_")).ToList();
            List<string> lstmon = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkmealmon_")).ToList();
            List<string> lsttues = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkmealtues_")).ToList();
            List<string> lstwed = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkmealwed_")).ToList();
            List<string> lstthur = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkmealthur_")).ToList();
            List<string> lstfri = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkmealfri_")).ToList();
            List<string> lstsat = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkmealsat_")).ToList();

            StringBuilder sbout = new StringBuilder();

            string strout = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                if (lstsun.Contains("hdnchkmealsun_" + (i + 1).ToString()))
                {
                    strout = fc["hdnchkmealsun_" + (i + 1).ToString()] + fc["hdnchkmealmon_" + (i + 1).ToString()] + fc["hdnchkmealtues_" + (i + 1).ToString()] + fc["hdnchkmealwed_" + (i + 1).ToString()] + fc["hdnchkmealthur_" + (i + 1).ToString()] + fc["hdnchkmealfri_" + (i + 1).ToString()] + fc["hdnchkmealsat_" + (i + 1).ToString()];
                    sbout.Append(strout + ",");
                }
                else
                {
                    sbout.Append("0000000,");
                }
            }

            strout = Convert.ToString(sbout).TrimEnd(',');

            Plan plan = db.Plans.Find(Code);
            plan.MEALS = strout;
            db.Entry(plan).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                TempData["MealPlansmessage"] = "Plan schedule updated successfully.";
                TempData["class"] = "success-msg";

                return RedirectToAction("Edit",new { id = Code });
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region  function to set meal plan schedular in PlanLimits
        //function to set meal plan schedular in PlanLimits
        [HttpPost]
        public ActionResult SetPlanScheduleInPlanLimit(FormCollection fc, PlanLimit pllimit, int Code)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.edit_meal_plans.ToString()) == false)
            {
                return View();
            }

            var planLimit = db.PlanLimits.Where(x => x.PlanCode == Code).ToList();

            if (planLimit != null)
            {
                foreach (var item in planLimit)
                {
                    db.PlanLimits.Remove(item);
                }

                db.SaveChanges();
            }

            List<string> lstchkMealsPerDay = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkMealsperday_")).ToList();
            List<string> lsttxtMealsperday = fc.AllKeys.ToList().Where(x => x.StartsWith("txtMealsperday_")).ToList();

            List<string> lstchkTotalmeals = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkTotalmeals_")).ToList();
            List<string> lsttxtTotalmeals = fc.AllKeys.ToList().Where(x => x.StartsWith("txtTotalmeals_")).ToList();

            List<string> lstchkTotalamount = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkTotalamount_")).ToList();
            List<string> lsttxtTotalamount = fc.AllKeys.ToList().Where(x => x.StartsWith("txtTotalamount")).ToList();

            List<string> lstchkMaxmealprice = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnchkMaxmealprice_")).ToList();
            List<string> lsttxtMaxmealprice = fc.AllKeys.ToList().Where(x => x.StartsWith("txtMaxmealprice_")).ToList();

            for (int i = 0; i < lsttxtMaxmealprice.Count; i++)
            {
                PlanLimit pl = new PlanLimit();
                pl.PlanCode = Code;

                if (fc.AllKeys.Contains(lstchkMealsPerDay[i]))
                {
                    if (fc[lstchkMealsPerDay[i]] == "1")
                    {
                        pl.IsMealsPerDay = 1;
                    }
                }
                else
                {
                    pl.IsMealsPerDay = 0;
                }

                if (fc[lsttxtMealsperday[i]] != "")
                {
                    pl.MealsPerDay = Convert.ToInt32(fc[lsttxtMealsperday[i]]);
                }

                if (fc.AllKeys.Contains(lstchkTotalmeals[i]))
                {
                    if (fc[lstchkTotalmeals[i]] == "1")
                    {
                        pl.IsMealsPerPlan = 1;

                    }
                }
                else
                {
                    pl.IsMealsPerPlan = 0;
                }
                if (fc[lsttxtTotalmeals[i]] != "")
                {
                    pl.MealsPerPlan = Convert.ToInt16(fc[lsttxtTotalmeals[i]]);
                }

                if (fc.AllKeys.Contains(lstchkTotalamount[i]))
                {
                    if (fc[lstchkTotalamount[i]] == "1")
                    {
                        pl.IsAmountPerPlan = 1;
                    }
                }
                else
                {
                    pl.IsAmountPerPlan = 0;
                }

                if (lsttxtTotalamount[i] != "")
                {
                    pl.AmountPerPlan = Convert.ToInt32(fc[lsttxtTotalamount[i]]) * 100;
                }

                if (fc.AllKeys.Contains(lstchkMaxmealprice[i]))
                {
                    if (fc[lstchkMaxmealprice[i]] == "1")
                    {
                        pl.IsAmountPerMeal = 1;

                    }
                }
                else
                {
                    pl.IsAmountPerMeal = 0;
                }

                if (fc[lsttxtMaxmealprice[i]] != string.Empty)
                {
                    pl.AmountPerMeal = Convert.ToInt32(fc[lsttxtMaxmealprice[i]]) * 100;
                }

                pl.MealCode = Convert.ToInt16(lsttxtMaxmealprice[i].Replace("txtMaxmealprice_", ""));
                db.PlanLimits.Add(pl);
            }
            try
            {
                db.SaveChanges();
                TempData["MealPlansmessage"] = "Plan schedule updated successfully.";
                TempData["class"] = "success-msg";

                return RedirectToAction("Edit", new { id = Code });
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region Existing Code Check
        //For Existing Code Check
        public JsonResult IsCodeExist(string CODE)
        {
            try
            {
                int code = Convert.ToInt32(CODE);
                var result = true;
                var code_id = db.Plans.Where(x => x.CODE.ToString().Trim() == code.ToString().Trim()).FirstOrDefault();

                if (code_id != null)
                    result = false;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return null;

            }
        }

        #endregion

        #region Meal plan Create Post actionmethod
        
        // POST: MealPlans => Create function
        [HttpPost]
        public ActionResult Create([Bind(Include = "CODE,DSCR,REGS,MEALS,IS_DAYS,DAYS,START_DATE,IS_EXPIRY_DATE,EXPIRY_DATE,IsTotalMealsPerPlan,TotalMealsPerPlan,IsTotalAmountPerPlan,TotalAmountPerPlan,IsTotalMealsPerWeek,TotalMealsPerWeek,EnableMultipleMeals,PRICE,PrintMode,LastUpdatedBy,LastUpdated,AccountID,Bonus,PassBack,AuthorizationRequired,DaysPerWeek,SecondDSCR,AllowParking,ParkGroup,TaxExempt,BonusPlanCode,Bool_EnableMultipleMeals,Bool_AuthorizationRequired,Bool_IS_DAYS,Bool_IS_EXPIRY_DATE,Bool_IsTotalAmountPerPlan,Bool_IsTotalMealsPerWeek,Bool_IsTotalMealsPerPlan")] Plan plan, FormCollection Fc)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_plans.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    List<string> lstallkeys = Fc.AllKeys.ToList().Where(x => x.StartsWith("type_")).ToList();
                    List<string> lstallgroup = Fc.AllKeys.ToList().Where(x => x.StartsWith("group_")).ToList();
                    List<string> lstalldevkeys = Fc.AllKeys.ToList().Where(x => x.StartsWith("deviceType_")).ToList();
                    List<string> lstalldevgrpkeys = Fc.AllKeys.ToList().Where(x => x.StartsWith("deviceGroup_")).ToList();

                    ArrayList aldevices = new ArrayList();
                    foreach (var item in lstalldevkeys)
                    {
                        if (aldevices.Contains(item.Replace("deviceType_", string.Empty)) == false)
                        {
                            aldevices.Add(item.Replace("deviceType_", string.Empty));
                        }
                    }
                    foreach (var item in lstalldevgrpkeys)
                    {
                        if (aldevices.Contains(item.Replace("deviceGroup_", string.Empty)) == false)
                        {
                            aldevices.Add(item.Replace("deviceGroup_", string.Empty));
                        }

                    }

                    StringBuilder sboutput = new StringBuilder();

                    foreach (var item in aldevices)
                    {
                        sboutput.Append(item + ",");
                    }
                    string output = sboutput.ToString().TrimEnd(',');

                    plan.AllowParking = 0;
                    plan.LastUpdatedBy = Convert.ToString(Session["Username"]);
                    plan.LastUpdated = DateTime.Now;
                    plan.BonusPlanCode = 0;


                    if (plan.Bonus != 0)
                    {
                        plan.Bonus = plan.Bonus * 100;
                    }

                    plan.MEALS = "1111111,1111111,1111111,1111111,1111111";
                    plan.ParkGroup = 0;
                    plan.REGS = output;
                    plan.TaxExempt = 0;

                    if (plan.DSCR == null)
                    {
                        plan.DSCR = "";
                    }
                    db.Plans.Add(plan);

                    List<string> lstprofiles = Fc.AllKeys.ToList().Where(x => x.StartsWith("profile_")).ToList();

                    List<PlanProfileWebDeposit> lstplanProfileWebDeposit = new List<PlanProfileWebDeposit>();

                    foreach (var item in lstprofiles)
                    {
                        if (Convert.ToString(Fc[item]).ToUpper() != "FALSE")
                        {
                            PlanProfileWebDeposit planProfileWebDeposit = new PlanProfileWebDeposit();
                            planProfileWebDeposit.PLAN_CODE = (Int16)plan.CODE;
                            planProfileWebDeposit.PROFILE = Convert.ToInt16(item.Replace("profile_", ""));
                            db.PlanProfileWebDeposits.Add(planProfileWebDeposit);
                        }

                    }

                    db.SaveChanges();

                    TempData["MealPlansmessage"] = "Meal plan created successfully.";
                    TempData["class"] = "success-msg";

                    return RedirectToAction("Index");
                }

                return View("Create");
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region  For Edit the meal plam created
        //Get For Edit the meal plam created
        public ActionResult Edit(int? id)
         {
            string xr = id.ToString();

            TempData["MealPlansmessage"] = string.Empty;
            TempData["class"] = string.Empty;
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_plans.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Plan plan = db.Plans.Find(id);

                if (plan == null)
                {
                    return HttpNotFound();
                }

                ViewBag.strExpirySetDate = plan.EXPIRY_DATE.ToString("yyyy/MM/dd");
                ViewBag.strStartDate = plan.START_DATE.ToString("yyyy/MM/dd");
                List<string> lststr = new List<string>();

                if (plan.REGS != null || plan.REGS != string.Empty)
                {
                    string[] strreg = plan.REGS.ToString().Split(',');
                    for (int i = 0; i < strreg.Length; i++)
                    {
                        lststr.Add(strreg[i]);
                    }
                }


                var tempprofile = db.PlanProfileWebDeposits.Where(x => x.PLAN_CODE == id).ToList();

                if (tempprofile != null)
                {
                    foreach (var item in tempprofile)
                    {
                        lststr.Add(item.PROFILE.ToString());
                    }
                }

                if (lststr != null)
                {
                    ViewBag.IsCheck = lststr.ToList();
                }

                List<string> lstprof = new List<string>();
                var planProfileWebDeposit = db.PlanProfileWebDeposits.Where(x => x.PLAN_CODE == id).ToList().AsEnumerable();
                if (planProfileWebDeposit != null)
                {
                    ViewBag.IsProfileCheck = planProfileWebDeposit.ToList();
                }

                var resultreg = db.Registers.ToList();
                var resultdevice = dbwmn.deviceTypes.ToList();
                var resultdevicegroup = db.RegisterGroups.ToList();

                var result1 = (from t1 in resultreg
                               join t2 in resultdevice
                               on new { commcol = t1.RegisterType } equals new { commcol = t2.id }
                               select new { t1.CODE, t1.DSCR, t2.deviceTypeName });


                List<Register> partsList1 = new List<Register>();

                foreach (var m in result1)
                {
                    Register lstact = new Register();
                    lstact.CODE = m.CODE;
                    lstact.DSCR = m.DSCR;
                    lstact.deviceTypeName = m.deviceTypeName;
                    partsList1.Add(lstact);
                }

                var UniqueDevices = (from t1 in resultreg
                                     join t2 in resultdevice
                                     on new { commcol = t1.RegisterType } equals new { commcol = t2.id }
                                     select new { t1.RegisterType, t2.deviceTypeName, t2.id }).Distinct().OrderBy(x => x.id);
                List<Register> partsList2 = new List<Register>();

                foreach (var m in UniqueDevices)
                {
                    Register lstact = new Register();
                    lstact.RegisterType = m.RegisterType;
                    lstact.deviceTypeName = m.deviceTypeName;
                    lstact.CODE = m.id;
                    partsList2.Add(lstact);
                }

                ViewBag.Devices = partsList1;
                ViewBag.UniqueDevices = partsList2;

                ViewBag.AllowedProfiles = db.Profiles.Distinct().OrderBy(x => x.CODE).ToList().AsEnumerable();

                ViewBag.AccountID = new SelectList(db.Accounts.ToList(), "CODE", "DSCR", plan.AccountID);

                ViewBag.PrintMode = plan.PrintMode;

                var result2 = (from t1 in resultreg
                               join t2 in resultdevicegroup
                               on new { commcol = t1.GROUP_CODE } equals new { commcol = t2.CODE }
                               select new { t1.CODE, t1.DSCR, devicedesc = t2.DSCR });
                List<Register> partsList3 = new List<Register>();

                foreach (var m in result2)
                {
                    Register lstact = new Register();
                    lstact.CODE = m.CODE;
                    lstact.DSCR = m.DSCR;
                    lstact.deviceTypeName = m.devicedesc;
                    partsList3.Add(lstact);
                }

                List<Register> partsList4 = new List<Register>();

                var UniqueDevice_Group = (from t1 in resultreg
                                          join t2 in resultdevicegroup
                                          on new { commcol = t1.GROUP_CODE } equals new { commcol = t2.CODE }
                                          select new { t1.GROUP_CODE, t2.DSCR, t2.CODE }).Distinct().OrderBy(x => x.CODE);
                foreach (var m in UniqueDevice_Group)
                {
                    Register lstact = new Register();
                    lstact.GROUP_CODE = m.GROUP_CODE;
                    lstact.DSCR = m.DSCR;
                    partsList4.Add(lstact);
                }

                ViewBag.Device_Group = partsList3;
                ViewBag.UniqueDevice_Group = partsList4;

                return View(plan);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region Updating the meal plan

        //Post For Edit the meal plam created
        [HttpPost]
        public ActionResult Edit([Bind(Include = "CODE,DSCR,REGS,MEALS,IS_DAYS,DAYS,START_DATE,IS_EXPIRY_DATE,EXPIRY_DATE,IsTotalMealsPerPlan,TotalMealsPerPlan,IsTotalAmountPerPlan,TotalAmountPerPlan,IsTotalMealsPerWeek,TotalMealsPerWeek,EnableMultipleMeals,PRICE,PrintMode,LastUpdatedBy,LastUpdated,AccountID,Bonus,PassBack,AuthorizationRequired,DaysPerWeek,SecondDSCR,AllowParking,ParkGroup,TaxExempt,BonusPlanCode,Bool_EnableMultipleMeals,Bool_AuthorizationRequired,Bool_IS_DAYS,Bool_IS_EXPIRY_DATE,Bool_IsTotalAmountPerPlan,Bool_IsTotalMealsPerWeek,Bool_IsTotalMealsPerPlan")] Plan plan, FormCollection Fc, string Command)
        {
            TempData["MealPlansmessage"] = string.Empty;
            TempData["class"] = string.Empty;
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_plans.ToString()) == false)
            {
                return View("NoAccess");
            }
            if (ModelState.IsValid)
            {
                List<string> lstallkeys = Fc.AllKeys.ToList().Where(x => x.StartsWith("type_")).ToList();
                List<string> lstallgroup = Fc.AllKeys.ToList().Where(x => x.StartsWith("group_")).ToList();
                List<string> lstalldevkeys = Fc.AllKeys.ToList().Where(x => x.StartsWith("deviceType_")).ToList();
                List<string> lstalldevgrpkeys = Fc.AllKeys.ToList().Where(x => x.StartsWith("deviceGroup_")).ToList();

                ArrayList aldevices = new ArrayList();
                foreach (var item in lstalldevkeys)
                {
                    if (aldevices.Contains(item.Replace("deviceType_", string.Empty)) == false)
                    {
                        aldevices.Add(item.Replace("deviceType_", string.Empty));
                    }
                }
                foreach (var item in lstalldevgrpkeys)
                {
                    if (aldevices.Contains(item.Replace("deviceGroup_", string.Empty)) == false)
                    {
                        aldevices.Add(item.Replace("deviceGroup_", string.Empty));
                    }
                }

                StringBuilder sboutput = new StringBuilder();

                foreach (var item in aldevices)
                {
                    sboutput.Append(item + ",");
                }
                string output = sboutput.ToString().TrimEnd(',');


                List<string> lstprofiles = Fc.AllKeys.ToList().Where(x => x.StartsWith("profile_")).ToList();

                List<PlanProfileWebDeposit> lstplanProfileWebDeposit = new List<PlanProfileWebDeposit>();
                short planCODE = (Int16)plan.CODE;

                
                var removeplan = (db.PlanProfileWebDeposits.Where(x => x.PLAN_CODE == planCODE)).ToList();

                foreach (var item in removeplan)
                {
                    db.PlanProfileWebDeposits.Remove(item);
                }

                foreach (var item in lstprofiles)
                {
                    if (Convert.ToString(Fc[item]).ToUpper() != "FALSE")
                    {
                        PlanProfileWebDeposit planProfileWebDeposit = new PlanProfileWebDeposit();
                        short prof = Convert.ToInt16(item.Replace("profile_", string.Empty));
                        planProfileWebDeposit.PLAN_CODE = (Int16)plan.CODE;
                        planProfileWebDeposit.PROFILE = Convert.ToInt16(item.Replace("profile_", string.Empty));
                        db.PlanProfileWebDeposits.Add(planProfileWebDeposit);
                        db.SaveChanges();
                    }
                }
                plan.AllowParking = 0;
                plan.LastUpdatedBy = Convert.ToString(Session["UserName"]);
                plan.LastUpdated = DateTime.Now;
                plan.BonusPlanCode = 0;

                if (plan.Bonus != 0)
                {
                    plan.Bonus = plan.Bonus * 100;
                }

                plan.MEALS = plan.MEALS;
                plan.ParkGroup = 0;
                plan.REGS = output;
                plan.TaxExempt = 0;


                bool IsCopy = false;
                if (Command == "Make Copy of Plan")
                {

                    var result = from c in db.Plans select new { c.CODE };
                    var max = 0;
                    if (result != null)
                    {
                        if (result.Count() > 0)
                        {
                            max = result.Max(x => x.CODE);
                        }
                    }

                    plan.CODE = max + 1;
                    plan.DSCR = "Copy Of " + plan.DSCR;
                    IsCopy = true;
                    db.Plans.Add(plan);

                    //------ checking if alredy values are exist or not?

                    List<PlanProfileWebDeposit> lstplanProfileWebDeposite = new List<PlanProfileWebDeposit>();
                    short planCode = (Int16)plan.CODE;


                    var removePlan = (db.PlanProfileWebDeposits.Where(x => x.PLAN_CODE == planCode)).ToList();

                    foreach (var item in removePlan)
                    {
                        db.PlanProfileWebDeposits.Remove(item);
                    }                    

                    //------Inserting  web deposite profile values if select copy plan----------

                    List<string> lstprofiles1 = Fc.AllKeys.ToList().Where(x => x.StartsWith("profile_")).ToList();

                    List<PlanProfileWebDeposit> lstplanProfileWebDeposit1 = new List<PlanProfileWebDeposit>();
                    short planCODE1 = (Int16)plan.CODE;
                   
                    foreach (var item in lstprofiles1)
                    {
                        if (Convert.ToString(Fc[item]).ToUpper() != "FALSE")
                        {
                            PlanProfileWebDeposit planProfileWebDeposit = new PlanProfileWebDeposit();
                            short prof = Convert.ToInt16(item.Replace("profile_", string.Empty));
                            planProfileWebDeposit.PLAN_CODE = (Int16)plan.CODE;
                            planProfileWebDeposit.PROFILE = Convert.ToInt16(item.Replace("profile_", string.Empty));
                            db.PlanProfileWebDeposits.Add(planProfileWebDeposit);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    db.Entry(plan).State = EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                    if (IsCopy == true)
                    {
                        TempData["MealPlansmessage"] = "Copy of meal plan created successfully.";
                        TempData["class"] = "success-msg";
                    }
                    else
                    {
                        TempData["MealPlansmessage"] = "Meal plan updated successfully.";
                        TempData["class"] = "success-msg";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while processing meal plan:", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
                return RedirectToAction("Index");
            }

            return View(plan);
        }

        #endregion

        #region delete Actionmethod for deleting the meal plan
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // check module permission
                if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_meal_plans.ToString()) == false)
                {
                    return View("NoAccess");
                }

                Plan plan = db.Plans.Find(id);
                bool isDelete = true;

                // Check database version              
                int DB_Structure = Common.Functions.GetDBStructure();

                if (DB_Structure >= 15)
                {
                    List<ClientPlansUnlimited> lstClientPlansUnlimited = db.ClientPlansUnlimiteds.ToList().FindAll(c => c.PlanID == id);

                    // client has purchased plan so do not delete
                    if (lstClientPlansUnlimited.Count > 0)
                    {
                        isDelete = false;
                    }
                }
                else
                {
                    List<Client> lstClient = db.Clients.ToList().FindAll(x => x.PLAN_CODE1 == id || x.PLAN_CODE2 == id || x.PLAN_CODE3 == id);

                    if (lstClient.Count > 0)
                    {
                        isDelete = false;
                    }
                }

                if (isDelete)
                {
                    var removeplan = (db.PlanProfileWebDeposits.Where(x => x.PLAN_CODE == id)).ToList();
                    foreach (var item in removeplan)
                    {
                        db.PlanProfileWebDeposits.Remove(item);
                    }

                    db.Plans.Remove(plan);
                    db.SaveChanges();
                    TempData["MealPlansmessage"] = "Meal plan deleted successfully.";
                    TempData["class"] = "success-msg";
                    return Json(new { Success = true });
                }
                else
                {
                    TempData["MealPlansmessage"] = "Meal Plan can not be deleted as it is purchased by client(s).";
                    TempData["class"] = "error-msg";
                    return Json(new { Success = false });
                }
            }
            catch (Exception ex)
            {
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
