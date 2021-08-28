using ITC_Matrix.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ITC_Matrix.Common;

namespace ITC_Matrix.Controllers
{
    public class ClientMealPlansController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.clients_node.ToString();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_plans.ToString()) == false)
            {
                return View("NoAccess");
            }

            ViewBag.Message = TempData["Message"];
            ViewBag.MessageClass = TempData["MessageClass"];
            var DB_Structure = db.Params.Where(x => x.Name.ToUpper() == "DB_Structure").SingleOrDefault();
            int DBStructure = 0;
            if (DB_Structure.Value != "")
            {
                DBStructure = Convert.ToInt32(DB_Structure.Value);
            }

            ViewBag.Id = id;
            if (DBStructure >= 15)
            {
                var clientPlansUnlimiteds = db.ClientPlansUnlimiteds.Where(c => c.ClientID.Equals(id));
                return View(clientPlansUnlimiteds.ToList());
            }
            else
            {
                List<ClientPlansUnlimited> clientPlansUnlimiteds = new List<ClientPlansUnlimited>();
                ClientPlansUnlimited clientPlansUnlimited = new ClientPlansUnlimited();
                Client temclientmeal = db.Clients.Where(c => c.ID_NO.Equals(id)).SingleOrDefault();

                clientPlansUnlimited.Client = new Client();
                clientPlansUnlimited.Plan = new Plan();
                string plandscription1 = string.Empty;
                string plandscription2 = string.Empty;
                string plandscription3 = string.Empty;
                if (temclientmeal != null)
                {
                    if (temclientmeal.PLAN_CODE1 != 0)
                    {
                        var PlanDscr1 = db.Plans.Where(x => x.CODE.Equals(temclientmeal.PLAN_CODE1)).SingleOrDefault();
                        if (PlanDscr1 != null)
                        {
                            plandscription1 = PlanDscr1.DSCR;
                        }
                        clientPlansUnlimited.ClientID = temclientmeal.ID_NO;
                        clientPlansUnlimited.PlanID = temclientmeal.PLAN_CODE1;
                        clientPlansUnlimited.LastMealTime = temclientmeal.LastMealTime;
                        clientPlansUnlimited.WeekMealCount = temclientmeal.WeekMealCount;
                        clientPlansUnlimited.Client.FAMILY = temclientmeal.FAMILY;
                        clientPlansUnlimited.PlanStartDate = temclientmeal.PLAN_START1;
                        clientPlansUnlimited.Plan.DSCR = plandscription1;
                        clientPlansUnlimiteds.Add(clientPlansUnlimited);
                    }
                }
                if (temclientmeal != null)
                {
                    if (temclientmeal.PLAN_CODE2 != 0)
                    {
                        clientPlansUnlimited = new ClientPlansUnlimited();
                        clientPlansUnlimited.Client = new Client();
                        clientPlansUnlimited.Plan = new Plan();

                        var PlanDscr2 = db.Plans.Where(x => x.CODE.Equals(temclientmeal.PLAN_CODE2)).SingleOrDefault();
                        if (PlanDscr2 != null)
                        {
                            plandscription2 = PlanDscr2.DSCR;

                        }

                        clientPlansUnlimited.Client.FAMILY = temclientmeal.FAMILY;
                        clientPlansUnlimited.ClientID = temclientmeal.ID_NO;
                        clientPlansUnlimited.PlanID = temclientmeal.PLAN_CODE2;
                        clientPlansUnlimited.LastMealTime = temclientmeal.LastMealTime2;
                        clientPlansUnlimited.PlanStartDate = temclientmeal.PLAN_START2;
                        clientPlansUnlimited.WeekMealCount = temclientmeal.WeekMealCount2;
                        clientPlansUnlimited.Plan.DSCR = plandscription2;
                        clientPlansUnlimiteds.Add(clientPlansUnlimited);

                    }
                }
                if (temclientmeal != null)
                {
                    if (temclientmeal.PLAN_CODE3 != 0)
                    {
                        clientPlansUnlimited = new ClientPlansUnlimited();
                        clientPlansUnlimited.Client = new Client();
                        clientPlansUnlimited.Plan = new Plan();

                        var PlanDscr3 = db.Plans.Where(x => x.CODE.Equals(temclientmeal.PLAN_CODE3)).SingleOrDefault();
                        if (PlanDscr3 != null)
                        {
                            plandscription3 = PlanDscr3.DSCR;
                        }

                        clientPlansUnlimited.Client.FAMILY = temclientmeal.FAMILY;
                        clientPlansUnlimited.WeekMealCount = temclientmeal.WeekMealCount3;
                        clientPlansUnlimited.ClientID = temclientmeal.ID_NO;
                        clientPlansUnlimited.PlanID = temclientmeal.PLAN_CODE3;
                        clientPlansUnlimited.PlanStartDate = temclientmeal.PLAN_START3;
                        clientPlansUnlimited.LastMealTime = temclientmeal.LastMealTime3;
                        clientPlansUnlimited.Plan.DSCR = plandscription3;
                        clientPlansUnlimiteds.Add(clientPlansUnlimited);


                    }
                }
                return View(clientPlansUnlimiteds.ToList());
            }
        }
        #endregion

        #region
        /// <summary> use defined funtioin for GetPlanPrice
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetPlanPrice(int id)
        {
            var planprice = db.Plans.Where(p => p.CODE == id).SingleOrDefault();
            string strplanprice = "0.00";
            if (planprice != null)
            {
                if (planprice.PRICE != 0)
                {
                    strplanprice = Convert.ToString((double)planprice.PRICE / 100);
                }
                if (!strplanprice.Contains("."))
                {
                    strplanprice = strplanprice + ".00";
                }
            }
            //return new JsonResult() { Data = new { name = strplanprice } };
            return new JsonResult() { Data = new { name = strplanprice, TextStartDate = planprice.START_DATE.ToString("yyyy/MM/dd"), TextEndDate = planprice.EXPIRY_DATE.ToString("yyyy/MM/dd"), IsEndDate = planprice.IS_EXPIRY_DATE } };
        }
        #endregion

        #region
        /// <summary>search Monthly statement
        /// 
        /// </summary>
        /// <param name="ClientID"></param>
        /// <param name="PlanID"></param>
        /// <param name="searchBy"></param>
        /// <returns></returns>
        public List<TrnReg> LoadMonthlyStatement(string ClientID, int PlanID, int searchBy)
        {
            int comparemonth = DateTime.Now.Month;
            int compareyear = DateTime.Now.Year;
            if (searchBy == (int)Common.CommonEnum.SearchByPeriod.Last_Month)
            {
                comparemonth = DateTime.Now.AddMonths(-1).Month;
                if (comparemonth == 12)
                {
                    compareyear = DateTime.Now.AddYears(-1).Year;
                }
            }

            List<TrnReg> LstTrnReg = new List<TrnReg>();
            var TrnRegData = (from t1 in db.TrnRegs
                              join t2 in db.Meals on t1.MEAL_CODE equals t2.CODE
                              join t3 in db.Registers on t1.REG_CODE equals t3.CODE
                              where t1.ID_NO == ClientID && t1.MEAL_CODE != 0 && t1.PLAN_CODE == PlanID
                              && t1.TRN_DATE.Month.Equals(comparemonth) && t1.TRN_DATE.Year.Equals(compareyear)
                              select new { t1.PLAN_CODE, t1.ID_NO, DeviceDesc = t3.DSCR, t1.TRN_DATE, t1.AMOUNT, MealDesc = t2.DSCR, t1.Comment });
            TrnReg trnReg = new TrnReg();
            if (TrnRegData != null)
            {
                foreach (var item in TrnRegData)
                {
                    trnReg = new TrnReg();
                    trnReg.IsTaken = false;
                    trnReg.ID_NO = item.ID_NO;
                    trnReg.DeviceDesc = item.DeviceDesc;
                    trnReg.strTrnDate = item.TRN_DATE.ToString("yyyy/MM/dd");
                    trnReg.AMOUNT = item.AMOUNT;
                    trnReg.MealDesc = item.MealDesc;
                    var IsTakenflg = db.Plans.Where(p => p.CODE == item.PLAN_CODE).SingleOrDefault();
                    if (IsTakenflg != null)
                    {
                        if (IsTakenflg.IsTotalAmountPerPlan == 1)
                        {
                            trnReg.IsTaken = true;
                        }
                    }
                    trnReg.Comment = item.Comment;
                    LstTrnReg.Add(trnReg);
                }
            }

            List<TrnPlan> lstplan = new List<TrnPlan>();
            var lstTrnPlanData = (from t1 in db.TrnPlans
                                  join t2 in db.Plans on t1.PLAN_CODE equals t2.CODE
                                  where t1.ID_NO == ClientID && t1.TRN_DATE.Month.Equals(comparemonth)
                                  && t1.TRN_DATE.Year.Equals(compareyear) && t1.PLAN_CODE == PlanID
                                  select new { t1.ID_NO, t1.TRN_DATE, t1.PLAN_PRICE, t2.DSCR }).ToList().AsEnumerable();
            foreach (var item in lstTrnPlanData)
            {
                TrnPlan tp = new TrnPlan();
                tp.ID_NO = item.ID_NO;
                tp.strTrnDate = item.TRN_DATE.ToString("yyyy/MM/dd");
                tp.PLAN_PRICE = item.PLAN_PRICE;
                tp.PlanDesc = item.DSCR;
                lstplan.Add(tp);
            }
            ViewBag.lstTrnPlan = lstplan;
            return LstTrnReg;
        }
        #endregion

        #region search monthly all statement
        public List<TrnReg> LoadMonthlyAllStatement(string ClientID, int searchBy)
        {
            int comparemonth = DateTime.Now.Month;
            int compareyear = DateTime.Now.Year;
            if (searchBy == (int)Common.CommonEnum.SearchByPeriod.Last_Month)
            {
                comparemonth = DateTime.Now.AddMonths(-1).Month;
                if (comparemonth == 12)
                {
                    compareyear = DateTime.Now.AddYears(-1).Year;
                }
            }


            List<TrnReg> LstTrnReg = new List<TrnReg>();
            var TrnRegData = (from t1 in db.TrnRegs
                              join t2 in db.Meals on t1.MEAL_CODE equals t2.CODE
                              join t3 in db.Registers on t1.REG_CODE equals t3.CODE
                              where t1.ID_NO == ClientID && t1.MEAL_CODE != 0
                              && t1.TRN_DATE.Month.Equals(comparemonth) && t1.TRN_DATE.Year.Equals(compareyear)
                              select new { t1.PLAN_CODE, t1.ID_NO, DeviceDesc = t3.DSCR, t1.TRN_DATE, t1.AMOUNT, MealDesc = t2.DSCR, t1.Comment });
            TrnReg trnReg = new TrnReg();
            if (TrnRegData != null)
            {
                foreach (var item in TrnRegData)
                {
                    trnReg = new TrnReg();
                    trnReg.IsTaken = false;
                    trnReg.ID_NO = item.ID_NO;
                    trnReg.DeviceDesc = item.DeviceDesc;
                    trnReg.strTrnDate = item.TRN_DATE.ToString("yyyy/MM/dd");
                    trnReg.AMOUNT = item.AMOUNT;
                    trnReg.MealDesc = item.MealDesc;
                    var IsTakenflg = db.Plans.Where(p => p.CODE == item.PLAN_CODE).SingleOrDefault();
                    if (IsTakenflg != null)
                    {
                        if (IsTakenflg.IsTotalAmountPerPlan == 1)
                        {
                            trnReg.IsTaken = true;
                        }
                    }
                    trnReg.Comment = item.Comment;
                    LstTrnReg.Add(trnReg);
                }
            }

            List<TrnPlan> lstplan = new List<TrnPlan>();
            var lstTrnPlanData = (from t1 in db.TrnPlans
                                  join t2 in db.Plans on t1.PLAN_CODE equals t2.CODE
                                  where t1.ID_NO == ClientID && t1.TRN_DATE.Month.Equals(comparemonth)
                                  && t1.TRN_DATE.Year.Equals(compareyear)
                                  select new { t1.ID_NO, t1.TRN_DATE, t1.PLAN_PRICE, t2.DSCR }).ToList().AsEnumerable();
            foreach (var item in lstTrnPlanData)
            {
                TrnPlan tp = new TrnPlan();
                tp.ID_NO = item.ID_NO;
                tp.strTrnDate = item.TRN_DATE.ToString("yyyy/MM/dd");
                tp.PLAN_PRICE = item.PLAN_PRICE;
                tp.PlanDesc = item.DSCR;
                lstplan.Add(tp);
            }
            ViewBag.lstTrnPlan = lstplan;

            return LstTrnReg;
        }
        #endregion

        #region
        /// <summary> Load all satement
        /// 
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public List<TrnReg> LoadAllPerAllStatement(string ClientID)
        {
            List<TrnReg> LstTrnReg = new List<TrnReg>();
            var TrnRegData = (from t1 in db.TrnRegs
                              join t2 in db.Meals on t1.MEAL_CODE equals t2.CODE
                              join t3 in db.Registers on t1.REG_CODE equals t3.CODE
                              where t1.ID_NO == ClientID && t1.MEAL_CODE != 0
                              select new { t1.PLAN_CODE, t1.ID_NO, DeviceDesc = t3.DSCR, t1.TRN_DATE, t1.AMOUNT, MealDesc = t2.DSCR, t1.Comment });
            TrnReg trnReg = new TrnReg();
            if (TrnRegData != null)
            {
                foreach (var item in TrnRegData)
                {
                    trnReg = new TrnReg();
                    trnReg.IsTaken = false;
                    trnReg.ID_NO = item.ID_NO;
                    trnReg.DeviceDesc = item.DeviceDesc;
                    trnReg.strTrnDate = item.TRN_DATE.ToString("yyyy/MM/dd");
                    trnReg.AMOUNT = item.AMOUNT;
                    trnReg.MealDesc = item.MealDesc;
                    var IsTakenflg = db.Plans.Where(p => p.CODE == item.PLAN_CODE).SingleOrDefault();
                    if (IsTakenflg != null)
                    {
                        if (IsTakenflg.IsTotalAmountPerPlan == 1)
                        {
                            trnReg.IsTaken = true;
                        }
                    }

                    trnReg.Comment = item.Comment;
                    LstTrnReg.Add(trnReg);
                }
            }

            List<TrnPlan> lstplan = new List<TrnPlan>();
            var lstTrnPlanData = (from t1 in db.TrnPlans
                                  join t2 in db.Plans on t1.PLAN_CODE equals t2.CODE
                                  where t1.ID_NO == ClientID
                                  select new { t1.ID_NO, t1.TRN_DATE, t1.PLAN_PRICE, t2.DSCR }).ToList().AsEnumerable();
            foreach (var item in lstTrnPlanData)
            {
                TrnPlan tp = new TrnPlan();
                tp.ID_NO = item.ID_NO;
                tp.strTrnDate = item.TRN_DATE.ToString("yyyy/MM/dd");
                tp.PLAN_PRICE = item.PLAN_PRICE;
                tp.PlanDesc = item.DSCR;
                lstplan.Add(tp);
            }
            ViewBag.lstTrnPlan = lstplan;

            return LstTrnReg;
        }
        #endregion

        #region
        public List<TrnReg> LoadAllPerStatement(string ClientID, int PlanID)
        {
            List<TrnReg> LstTrnReg = new List<TrnReg>();
            var TrnRegData = (from t1 in db.TrnRegs
                              join t2 in db.Meals on t1.MEAL_CODE equals t2.CODE
                              join t3 in db.Registers on t1.REG_CODE equals t3.CODE
                              where t1.ID_NO == ClientID && t1.MEAL_CODE != 0 && t1.PLAN_CODE == PlanID
                              select new { t1.PLAN_CODE, t1.ID_NO, DeviceDesc = t3.DSCR, t1.TRN_DATE, t1.AMOUNT, MealDesc = t2.DSCR, t1.Comment });
            TrnReg trnReg = new TrnReg();
            if (TrnRegData != null)
            {
                foreach (var item in TrnRegData)
                {
                    trnReg = new TrnReg();
                    trnReg.IsTaken = false;
                    trnReg.ID_NO = item.ID_NO;
                    trnReg.DeviceDesc = item.DeviceDesc;
                    trnReg.strTrnDate = item.TRN_DATE.ToString("yyyy/MM/dd");
                    trnReg.AMOUNT = item.AMOUNT;
                    trnReg.MealDesc = item.MealDesc;
                    trnReg.Comment = item.Comment;
                    var IsTakenflg = db.Plans.Where(p => p.CODE == item.PLAN_CODE).SingleOrDefault();
                    if (IsTakenflg != null)
                    {
                        if (IsTakenflg.IsTotalAmountPerPlan == 1)
                        {
                            trnReg.IsTaken = true;
                        }
                    }
                    LstTrnReg.Add(trnReg);
                }
            }
            List<TrnPlan> lstplan = new List<TrnPlan>();
            var lstTrnPlanData = (from t1 in db.TrnPlans
                                  join t2 in db.Plans on t1.PLAN_CODE equals t2.CODE
                                  where t1.ID_NO == ClientID && t1.PLAN_CODE == PlanID
                                  select new { t1.ID_NO, t1.TRN_DATE, t1.PLAN_PRICE, t2.DSCR }).ToList().AsEnumerable();

            foreach (var item in lstTrnPlanData)
            {
                TrnPlan tp = new TrnPlan();
                tp.ID_NO = item.ID_NO;
                tp.strTrnDate = item.TRN_DATE.ToString("yyyy/MM/dd");
                tp.PLAN_PRICE = item.PLAN_PRICE;
                tp.PlanDesc = item.DSCR;
                lstplan.Add(tp);
            }
            ViewBag.lstTrnPlan = lstplan;
            return LstTrnReg;
        }
        public ActionResult GenerateStatement(string ClientID, int PlanID)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }
            ViewBag.Message = TempData["Message"];
            ViewBag.MessageClass = TempData["MessageClass"];
            if (ClientID == null || PlanID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<TrnReg> LstTrnReg = LoadMonthlyStatement(ClientID, PlanID, (int)Common.CommonEnum.SearchByPeriod.This_Month);
            ViewBag.TrnID = ClientID;
            var MealPlan = db.Plans.Where(t1 => t1.CODE == PlanID).SingleOrDefault();
            if (MealPlan != null)
            {
                ViewBag.ClientMealPlan = MealPlan.DSCR;
            }
            else
            {
                ViewBag.ClientMealPlan = "";
            }

            ViewBag.SearchAll = false;
            return View(LstTrnReg);
        }
        #endregion

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="ClientID"></param>
        /// <param name="PlanID"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateStatement(int searchBy, string ClientID, int PlanID, FormCollection fc)
        {
            bool SearchAll = false;
            if (fc["SearchAll"] != "")
            {
                string[] str = fc["SearchAll"].Split(',');
                SearchAll = Convert.ToBoolean(str[0].ToString());
            }

            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }

            if (ClientID == null || PlanID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<TrnReg> LstTrnReg = new List<TrnReg>();
            TrnReg trnReg = new TrnReg();

            ViewBag.SearchAll = SearchAll;
            if (SearchAll == true)
            {
                if (searchBy == (int)Common.CommonEnum.SearchByPeriod.This_Month)
                {
                    LstTrnReg = LoadMonthlyAllStatement(ClientID, (int)Common.CommonEnum.SearchByPeriod.This_Month);
                }
                else if (searchBy == (int)Common.CommonEnum.SearchByPeriod.Last_Month)
                {
                    LstTrnReg = LoadMonthlyAllStatement(ClientID, (int)Common.CommonEnum.SearchByPeriod.Last_Month);
                }
                else if (searchBy == (int)Common.CommonEnum.SearchByPeriod.All_Periods)
                {
                    LstTrnReg = LoadAllPerAllStatement(ClientID);
                }
                else
                {
                    TempData["Message"] = "Please select the period.";
                    TempData["MessageClass"] = "error-msg";
                    return RedirectToAction("GenerateStatement", new { ClientID = ClientID, PlanID = PlanID });
                }
            }
            else if (searchBy == (int)Common.CommonEnum.SearchByPeriod.This_Month)
            {
                LstTrnReg = LoadMonthlyStatement(ClientID, PlanID, (int)Common.CommonEnum.SearchByPeriod.This_Month);
            }
            else if (searchBy == (int)Common.CommonEnum.SearchByPeriod.Last_Month)
            {
                LstTrnReg = LoadMonthlyStatement(ClientID, PlanID, (int)Common.CommonEnum.SearchByPeriod.Last_Month);
            }
            else if (searchBy == (int)Common.CommonEnum.SearchByPeriod.All_Periods)
            {
                LstTrnReg = LoadAllPerStatement(ClientID, PlanID);
            }
            else
            {
                TempData["Message"] = "Please select the period.";
                TempData["MessageClass"] = "error-msg";
                return RedirectToAction("GenerateStatement", new { ClientID = ClientID, PlanID = PlanID });
            }
            ViewBag.TrnID = ClientID;
            var MealPlan = db.Plans.Where(t1 => t1.CODE == PlanID).SingleOrDefault();
            if (MealPlan != null)
            {
                ViewBag.ClientMealPlan = MealPlan.DSCR;
            }
            else
            {
                ViewBag.ClientMealPlan = "";
            }

            TempData["Message"] = "";
            TempData["MessageClass"] = "";

            return View(LstTrnReg);
        }

        #endregion

        // GET: ClientMealPlans/Details/5
        public ActionResult Details(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientPlansUnlimited clientPlansUnlimited = db.ClientPlansUnlimiteds.Find(id);
            if (clientPlansUnlimited == null)
            {
                return HttpNotFound();
            }
            return View(clientPlansUnlimited);
        }

        public ActionResult LookUp(string ClientID, int PlanID)
        {
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }
            if (ClientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Message = TempData["Message"];
            ViewBag.MessageClass = "MessageClass";
            ViewBag.Id = ClientID;
            var clientPlan = db.ClientPlans.Where(x => x.ID_NO == ClientID && x.PlanCode == PlanID).ToList();
            var meals = (from s in db.Meals select s).ToList();
            var result = (from t1 in clientPlan
                          join t2 in meals
                          on new { commcol = (short)t1.MealCode } equals new { commcol = t2.CODE }
                          select new { t1.ID_NO, t1.PlanCode, t1.MealCode, t1.Meals, t1.Amount, t2.DSCR }).AsEnumerable();

            if (clientPlan == null)
            {
                return HttpNotFound();
            }
            List<ClientPlan> clplan = new List<ClientPlan>();
            foreach (var item in result)
            {
                ClientPlan clientplan = new ClientPlan();
                clientplan.ID_NO = item.ID_NO;

                var MaxMealPerPlanData = db.PlanLimits.Where(x => x.PlanCode == item.PlanCode && x.MealCode == item.MealCode).SingleOrDefault();
                if (MaxMealPerPlanData != null)
                {
                    clientplan.Max = Convert.ToString(MaxMealPerPlanData.MealsPerPlan);
                    clientplan.MaxAmount = MaxMealPerPlanData.AmountPerPlan;
                }
                var PlanData = db.Plans.Where(x => x.CODE == item.PlanCode).SingleOrDefault();
                if (PlanData != null)
                {
                    clientplan.TotalAmountPerPlan = PlanData.TotalAmountPerPlan;
                }
                clientplan.Description = item.DSCR;
                clientplan.PlanCode = item.PlanCode;
                clientplan.Meals = item.Meals;
                clientplan.Amount = item.Amount;
                clientplan.MealCode = item.MealCode;
                clplan.Add(clientplan);
            }

            return View(clplan.ToList().AsEnumerable());


        }

        public ActionResult Manage(string ClientID, int PlanID)
        {
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }
            if (ClientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Id = ClientID;
            ViewBag.Message = TempData["Message"];
            ViewBag.MessageClass = TempData["MessageClass"];

            var clientPlan = db.ClientPlans.Where(x => x.ID_NO == ClientID && x.PlanCode == PlanID).ToList();
            var meals = (from s in db.Meals select s).ToList();
            var result = (from t1 in clientPlan
                          join t2 in meals
                          on new { commcol = (short)t1.MealCode } equals new { commcol = t2.CODE }
                          select new { t1.ID_NO, t1.PlanCode, t1.MealCode, t1.Meals, t1.Amount, t2.DSCR }).AsEnumerable();

            if (clientPlan == null)
            {
                return HttpNotFound();
            }
            List<ClientPlan> clplan = new List<ClientPlan>();
            foreach (var item in result)
            {
                ClientPlan clientplan = new ClientPlan();
                clientplan.ID_NO = item.ID_NO;

                var MaxMealPerPlanData = db.PlanLimits.Where(x => x.PlanCode == item.PlanCode && x.MealCode == item.MealCode).SingleOrDefault();
                if (MaxMealPerPlanData != null)
                {
                    clientplan.Max = Convert.ToString(MaxMealPerPlanData.MealsPerPlan);
                    clientplan.MaxAmount = MaxMealPerPlanData.AmountPerPlan;
                }
                var PlanData = db.Plans.Where(x => x.CODE == item.PlanCode).SingleOrDefault();
                if (PlanData != null)
                {
                    clientplan.TotalAmountPerPlan = PlanData.TotalAmountPerPlan;
                }
                clientplan.Description = item.DSCR;
                clientplan.PlanCode = item.PlanCode;
                clientplan.Meals = item.Meals;
                clientplan.Amount = item.Amount;
                clientplan.MealCode = item.MealCode;
                clplan.Add(clientplan);
            }

            return View(clplan.ToList().AsEnumerable());

        }

        [HttpPost]
        public ActionResult Manage(FormCollection fc)
        {
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }

            List<string> lstIDNO = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnID_NO_")).ToList();
            List<string> lstPlanCode = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnPlanCode_")).ToList();
            List<string> lstmealcode = fc.AllKeys.ToList().Where(x => x.StartsWith("hdnmealcode_")).ToList();
            List<string> lstmeals = fc.AllKeys.ToList().Where(x => x.StartsWith("hdntxtmeals_")).ToList();
            List<string> lstamount = fc.AllKeys.ToList().Where(x => x.StartsWith("hdntxtamount_")).ToList();

            bool isValid = true;
            for (int i = 0; i < lstPlanCode.Count; i++)
            {
                string Id = fc[lstIDNO[i]];
                int PlanId = Convert.ToInt32(fc[lstPlanCode[i]]);
                short Mealcode = Convert.ToInt16(fc[lstmealcode[i]]);

                ClientPlan client = (db.ClientPlans.Where(x => x.ID_NO == Id)
                                  .Where(x => x.PlanCode == PlanId)
                                  .Where(x => x.MealCode == Mealcode)).FirstOrDefault();
                Regex regex = new Regex(@"^[0-9]+$");
                if (fc[lstmeals[i]] != "")
                {
                    string strtemp = fc[lstmeals[i]];
                    if (!regex.IsMatch(strtemp))
                    {
                        TempData["Message"] = "Meal should be numeric.";
                        TempData["MessageClass"] = "error-msg";
                        List<ClientPlan> lstClientPlan = ManageClientPlan(client.ID_NO, client.PlanCode);
                        isValid = false;
                        return RedirectToAction("Manage", new { ClientID = client.ID_NO, PlanID = client.PlanCode });

                    }
                    client.Meals = Convert.ToInt16(Convert.ToInt16(client.Meals) - Convert.ToInt16(fc[lstmeals[i]]));

                    if (client.Meals < 0)
                    {
                        TempData["Message"] = "Meal should not be grater than Meals Taken.";
                        TempData["MessageClass"] = "error-msg";
                        List<ClientPlan> lstClientPlan = ManageClientPlan(client.ID_NO, client.PlanCode);
                        isValid = false;
                        return RedirectToAction("Manage", new { ClientID = client.ID_NO, PlanID = client.PlanCode });
                    }
                }
                else
                {
                    TempData["Message"] = "Please enter value in meal";
                    TempData["MessageClass"] = "error-msg";
                    List<ClientPlan> lstClientPlan = ManageClientPlan(client.ID_NO, client.PlanCode);
                    isValid = false;
                    return RedirectToAction("Manage", new { ClientID = client.ID_NO, PlanID = client.PlanCode });
                }

                if (fc[lstamount[i]] != "")
                {

                    string strtemp = fc[lstamount[i]];
                    if (!regex.IsMatch(strtemp))
                    {
                        TempData["Message"] = "Amount should be numeric.";
                        TempData["MessageClass"] = "error-msg";
                        List<ClientPlan> lstClientPlan = ManageClientPlan(client.ID_NO, client.PlanCode);
                        isValid = false;
                        return RedirectToAction("Manage", new { ClientID = client.ID_NO, PlanID = client.PlanCode });
                    }
                    client.Amount = Convert.ToInt16(Convert.ToInt16(client.Amount / 100) - Convert.ToInt16(fc[lstamount[i]])) * 100;

                    if (client.Amount < 0)
                    {
                        TempData["Message"] = "Amount should not be grater than Spent.";
                        TempData["MessageClass"] = "error-msg";
                        List<ClientPlan> lstClientPlan = ManageClientPlan(client.ID_NO, client.PlanCode);
                        isValid = false;
                        return RedirectToAction("Manage", new { ClientID = client.ID_NO, PlanID = client.PlanCode });
                    }
                }
                else
                {
                    TempData["Message"] = "Please enter value in amount";
                    TempData["MessageClass"] = "error-msg";
                    List<ClientPlan> lstClientPlan = ManageClientPlan(client.ID_NO, client.PlanCode);
                    isValid = false;
                    return RedirectToAction("Manage", new { ClientID = client.ID_NO, PlanID = client.PlanCode });
                }

                //client.Amount = amount;
                //client.Meals = meals;
                db.Entry(client).State = EntityState.Modified;
            }

            if (isValid)
            {
                db.SaveChanges();
                TempData["Message"] = "Manage meal plan edited successfully.";
                TempData["MessageClass"] = "success-msg";
            }
            return RedirectToAction("Index", new { id = fc[lstIDNO[0]] });
        }

        private List<ClientPlan> ManageClientPlan(string clientID, int planID)
        {
            ViewBag.message = TempData["message"];

            var clientPlan = db.ClientPlans.Where(x => x.ID_NO == clientID).ToList();
            var meals = (from s in db.Meals select s).ToList();
            var result = (from t1 in clientPlan
                          join t2 in meals
                          on new { commcol = (short)t1.MealCode } equals new { commcol = t2.CODE }
                          select new { t1.ID_NO, t1.PlanCode, t1.MealCode, t1.Meals, t1.Amount, t2.DSCR }).AsEnumerable();

            List<ClientPlan> lstClientPlan = new List<ClientPlan>();
            foreach (var item in result)
            {
                ClientPlan clientplan = new ClientPlan();
                clientplan.ID_NO = item.ID_NO;

                var MaxMealPerPlanData = db.PlanLimits.Where(x => x.PlanCode == item.PlanCode && x.MealCode == item.MealCode).SingleOrDefault();
                if (MaxMealPerPlanData != null)
                {
                    clientplan.Max = Convert.ToString(MaxMealPerPlanData.MealsPerPlan);
                    clientplan.MaxAmount = MaxMealPerPlanData.AmountPerPlan;
                }
                var PlanData = db.Plans.Where(x => x.CODE == item.PlanCode).SingleOrDefault();
                if (PlanData != null)
                {
                    clientplan.TotalAmountPerPlan = PlanData.TotalAmountPerPlan;
                }
                clientplan.Description = item.DSCR;
                clientplan.PlanCode = item.PlanCode;
                clientplan.Meals = item.Meals;
                clientplan.Amount = item.Amount;
                clientplan.MealCode = item.MealCode;

                lstClientPlan.Add(clientplan);
            }

            return lstClientPlan;
            //return View(clplan.ToList().AsEnumerable());
        }

        // GET: ClientMealPlans/Create
        public ActionResult Purchase(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                //var PayMethodsQuery = (from s in db.PayMethods select s).ToList();


                // Get permitted profile only.
                List<PayMethod> lstpayMethod = Common.Functions.GetPermittedPayMethods();

                if (lstpayMethod != null)
                {
                    ViewBag.PaymentMethods = new SelectList(lstpayMethod.ToList(), "CODE", "DSCR");
                }

                //ViewBag.PaymentMethods = new MultiSelectList(PayMethodsQuery, "CODE", "DSCR");


                ViewBag.ClientID = new SelectList(db.Clients, "ID_NO", "FAMILY");



                ViewBag.TempClientID = id;
                var DB_Structure = db.Params.Where(x => x.Name.ToUpper() == "DB_Structure").SingleOrDefault();
                int DBStructure = 0;
                if (DB_Structure.Value != "")
                {
                    DBStructure = Convert.ToInt32(DB_Structure.Value);
                }
                if (DBStructure >= 15)
                {
                    var query = from c in db.Plans
                                where !(from o in db.ClientPlansUnlimiteds
                                        where o.ClientID == id
                                        select o.PlanID)
                                       .Contains(c.CODE)
                                select new { c.CODE, c.DSCR };

                    ViewBag.PlanID = new SelectList(query.ToList(), "CODE", "DSCR");
                    return View(db.ClientPlansUnlimiteds.Where(p => p.ClientID == id).ToList());
                }
                else
                {
                    var planquery = db.Plans;
                    List<Plan> planlist = new List<Plan>();
                    Plan plan = new Plan();
                    var temclientmeal1 = db.Clients.Where(c => c.ID_NO.Equals(id)).FirstOrDefault();
                    //var temclientmeal1 = db.Clients.Where(c => c.ID_NO == id).SingleOrDefault();

                    //var temclientmeal1 = (from c in db.Clients
                    //                     where c.ID_NO == id
                    //                     select c).FirstOrDefault();


                    if (temclientmeal1 != null)
                    {
                        if (temclientmeal1.PLAN_CODE1 != 0 && temclientmeal1.PLAN_CODE2 != 0 && temclientmeal1.PLAN_CODE3 != 0)
                        {
                            return RedirectToAction("Index", "ClientMealPlans", new { id = id });
                        }
                    }

                    for (int i = 0; i < planquery.ToList().Count; i++)
                    {
                        if (planquery.ToList()[i].CODE != temclientmeal1.PLAN_CODE1 && planquery.ToList()[i].CODE != temclientmeal1.PLAN_CODE2 && planquery.ToList()[i].CODE != temclientmeal1.PLAN_CODE3)
                        {
                            plan = new Plan();
                            plan.CODE = planquery.ToList()[i].CODE;
                            plan.DSCR = planquery.ToList()[i].DSCR;
                            planlist.Add(plan);
                        }
                    }

                    ViewBag.PlanID = new SelectList(planlist.ToList(), "CODE", "DSCR");
                    List<ClientPlansUnlimited> clientPlansUnlimiteds = new List<ClientPlansUnlimited>();
                    ClientPlansUnlimited clientPlansUnlimited = new ClientPlansUnlimited();


                    clientPlansUnlimited.Client = new Client();
                    clientPlansUnlimited.Plan = new Plan();
                    string plandscription1 = string.Empty;
                    string plandscription2 = string.Empty;
                    string plandscription3 = string.Empty;

                    var PlanDscr1 = db.Plans.Where(x => x.CODE.Equals(temclientmeal1.PLAN_CODE1)).SingleOrDefault();
                    if (PlanDscr1 != null)
                    {
                        plandscription1 = PlanDscr1.DSCR;
                    }
                    clientPlansUnlimited.ClientID = temclientmeal1.ID_NO;
                    clientPlansUnlimited.PlanID = temclientmeal1.PLAN_CODE1;
                    clientPlansUnlimited.LastMealTime = temclientmeal1.LastMealTime;
                    clientPlansUnlimited.WeekMealCount = temclientmeal1.WeekMealCount;
                    clientPlansUnlimited.Client.FAMILY = temclientmeal1.FAMILY;
                    clientPlansUnlimited.Plan.DSCR = plandscription1;

                    clientPlansUnlimiteds.Add(clientPlansUnlimited);

                    clientPlansUnlimited = new ClientPlansUnlimited();
                    clientPlansUnlimited.Client = new Client();
                    clientPlansUnlimited.Plan = new Plan();

                    var PlanDscr2 = db.Plans.Where(x => x.CODE.Equals(temclientmeal1.PLAN_CODE2)).SingleOrDefault();
                    if (PlanDscr2 != null)
                    {
                        plandscription2 = PlanDscr2.DSCR;

                    }

                    clientPlansUnlimited.Client.FAMILY = temclientmeal1.FAMILY;
                    clientPlansUnlimited.ClientID = temclientmeal1.ID_NO;
                    clientPlansUnlimited.PlanID = temclientmeal1.PLAN_CODE2;
                    clientPlansUnlimited.LastMealTime = temclientmeal1.LastMealTime2;
                    clientPlansUnlimited.WeekMealCount = temclientmeal1.WeekMealCount2;
                    clientPlansUnlimited.Plan.DSCR = plandscription2;
                    clientPlansUnlimiteds.Add(clientPlansUnlimited);


                    clientPlansUnlimited = new ClientPlansUnlimited();
                    clientPlansUnlimited.Client = new Client();
                    clientPlansUnlimited.Plan = new Plan();

                    var PlanDscr3 = db.Plans.Where(x => x.CODE.Equals(temclientmeal1.PLAN_CODE3)).SingleOrDefault();
                    if (PlanDscr3 != null)
                    {
                        plandscription3 = PlanDscr3.DSCR;
                    }

                    clientPlansUnlimited.Client.FAMILY = temclientmeal1.FAMILY;
                    clientPlansUnlimited.WeekMealCount = temclientmeal1.WeekMealCount3;
                    clientPlansUnlimited.ClientID = temclientmeal1.ID_NO;
                    clientPlansUnlimited.PlanID = temclientmeal1.PLAN_CODE3;
                    clientPlansUnlimited.LastMealTime = temclientmeal1.LastMealTime3;
                    clientPlansUnlimited.Plan.DSCR = plandscription3;
                    clientPlansUnlimiteds.Add(clientPlansUnlimited);
                    return View(clientPlansUnlimiteds.ToList());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                TempData["message"] = "Error occurred in getting plan.";
                ViewBag.MessageClass = "error-msg";
                return View();
            }
        }

        // POST: ClientMealPlans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientPlansUnlimited"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase([Bind(Include = "ClientID,PlanID,PlanStartDate,LastMealTime,WeekMealCount,WeekDayCount,OrderIndex")] ClientPlansUnlimited clientPlansUnlimited, FormCollection fc)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }

            try
            {
                var DB_Structure = db.Params.Where(x => x.Name.ToUpper() == "DB_Structure").SingleOrDefault();
                int DBStructure = 0;
                if (DB_Structure.Value != "")
                {
                    DBStructure = Convert.ToInt32(DB_Structure.Value);
                }

                if (DBStructure >= 15)
                {
                    if (ModelState.IsValid)
                    {

                        var result = from c in db.ClientPlansUnlimiteds.Where(c => c.ClientID == clientPlansUnlimited.ClientID) select new { c.OrderIndex };
                        var max = result.Max(x => x.OrderIndex);

                        if (max == null)
                        {
                            max = 0;
                        }
                        clientPlansUnlimited.OrderIndex = max + 1;
                        db.ClientPlansUnlimiteds.Add(clientPlansUnlimited);

                        db.SaveChanges();
                        double price = Convert.ToDouble(fc["txtPrice"]);
                        TrnPlan trnPlan = new TrnPlan();
                        trnPlan.ID_NO = clientPlansUnlimited.ClientID;
                        trnPlan.PLAN_CODE = clientPlansUnlimited.PlanID;
                        trnPlan.PLAN_START = clientPlansUnlimited.PlanStartDate;
                        trnPlan.PLAN_PRICE = Convert.ToInt32(price) * 100;
                        trnPlan.TRN_DATE = DateTime.Now;
                        trnPlan.LOGIN = Convert.ToString(Session["UserName"]);
                        trnPlan.COMMENT = fc["txtComment"];
                        trnPlan.CreditCardAuthNumber = "0";
                        trnPlan.IsPlanNew = 1;
                        trnPlan.ACC_CODE = 0;
                        trnPlan.SOURCE = Convert.ToInt16(fc["PaymentMethods"]);
                        trnPlan.CreditCardType = -1;
                        trnPlan.CardPurchased = 0;
                        //trnPlan.PLAN_END = Convert.ToDateTime(fc["txtEndDate"]);
                        if (fc["txtEndDate"] != "")
                        {
                            trnPlan.PLAN_END = Convert.ToDateTime(fc["txtEndDate"]); ;
                        }
                        db.TrnPlans.Add(trnPlan);

                        db.SaveChanges();
                        return RedirectToAction("Index", new { id = clientPlansUnlimited.ClientID });
                    }
                    return RedirectToAction("Index", new { id = clientPlansUnlimited.ClientID });
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var plancode1 = db.Clients.Where(c => c.ID_NO == clientPlansUnlimited.ClientID).SingleOrDefault();
                        Client client = new Client();
                        if (plancode1 != null)
                        {
                            if (plancode1.PLAN_CODE1 == 0)
                            {
                                client = db.Clients.Find(clientPlansUnlimited.ClientID);
                                client.ID_NO = clientPlansUnlimited.ClientID;
                                client.PLAN_CODE1 = clientPlansUnlimited.PlanID;
                                client.PLAN_START1 = clientPlansUnlimited.PlanStartDate;

                                db.Entry(client).State = EntityState.Modified;
                            }
                            else if (plancode1.PLAN_CODE2 == 0)
                            {
                                client = db.Clients.Find(clientPlansUnlimited.ClientID);
                                client.ID_NO = clientPlansUnlimited.ClientID;
                                client.PLAN_CODE2 = clientPlansUnlimited.PlanID;
                                client.PLAN_START2 = clientPlansUnlimited.PlanStartDate;

                                db.Entry(client).State = EntityState.Modified;

                            }
                            else if (plancode1.PLAN_CODE3 == 0)
                            {
                                client = db.Clients.Find(clientPlansUnlimited.ClientID);
                                client.ID_NO = clientPlansUnlimited.ClientID;
                                client.PLAN_CODE3 = clientPlansUnlimited.PlanID;
                                client.PLAN_START3 = clientPlansUnlimited.PlanStartDate;
                                db.Entry(client).State = EntityState.Modified;
                            }
                        }

                        double price = Convert.ToDouble(fc["txtPrice"]);
                        TrnPlan trnPlan = new TrnPlan();
                        trnPlan.ID_NO = clientPlansUnlimited.ClientID;
                        trnPlan.PLAN_CODE = clientPlansUnlimited.PlanID;
                        trnPlan.PLAN_START = clientPlansUnlimited.PlanStartDate;
                        trnPlan.PLAN_PRICE = Convert.ToInt32(price) * 100;
                        trnPlan.TRN_DATE = DateTime.Now;
                        trnPlan.LOGIN = Convert.ToString(Session["UserName"]);
                        trnPlan.COMMENT = fc["txtComment"];
                        trnPlan.CreditCardAuthNumber = "0";
                        trnPlan.IsPlanNew = 1;
                        trnPlan.ACC_CODE = 0;
                        trnPlan.SOURCE = Convert.ToInt16(fc["PaymentMethods"]);
                        trnPlan.CreditCardType = -1;
                        trnPlan.CardPurchased = 0;
                        if (fc["txtEndDate"] != "")
                        {
                            trnPlan.PLAN_END = Convert.ToDateTime(fc["txtEndDate"]); ;
                        }

                        db.TrnPlans.Add(trnPlan);

                        db.SaveChanges();
                    }
                    TempData["message"] = "plan purchase successfully.";
                    ViewBag.MessageClass = "success-msg";
                    return RedirectToAction("Index", new { id = clientPlansUnlimited.ClientID });
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error occurred in plan purchase.";
                ViewBag.MessageClass = "error-msg";
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return RedirectToAction("Index", new { id = clientPlansUnlimited.ClientID });
            }
        }

        #region
        /// <summary> for edit client Meal Plan
        /// 
        /// </summary>
        /// <param name="ClientID"></param>
        /// <param name="PlanID"></param>
        /// <returns></returns>
        public ActionResult Edit(string ClientID, int PlanID)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }

            try
            {
                if (ClientID == null || PlanID == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var DB_Structure = db.Params.Where(x => x.Name.ToUpper() == "DB_Structure").SingleOrDefault();
                int DBStructure = 0;
                if (DB_Structure.Value != "")
                {
                    DBStructure = Convert.ToInt32(DB_Structure.Value);
                }
                if (DBStructure >= 15)
                {
                    var query = from c in db.Plans
                                where !(from o in db.ClientPlansUnlimiteds
                                        where o.ClientID == ClientID
                                        select o.PlanID)
                                       .Contains(c.CODE)
                                select new { c.CODE, c.DSCR };

                    var PlanDesc = db.Plans.Where(x => x.CODE == PlanID).SingleOrDefault();

                    List<Plan> planlst = new List<Plan>();

                    Plan pl = new Plan();
                    pl.CODE = PlanID;
                    pl.DSCR = PlanDesc.DSCR;
                    planlst.Add(pl);
                    foreach (var item in query)
                    {
                        pl = new Plan();
                        pl.CODE = item.CODE;
                        pl.DSCR = item.DSCR;
                        planlst.Add(pl);
                    }

                    ViewBag.PlanID = new SelectList(planlst.ToList(), "CODE", "DSCR", PlanID);

                    var clientPlansUnlimited = db.ClientPlansUnlimiteds.Where(c => c.ClientID == ClientID && c.PlanID == PlanID).SingleOrDefault();
                    if (clientPlansUnlimited == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.ClientID = new SelectList(db.Clients, "ID_NO", "FAMILY", clientPlansUnlimited.ClientID);





                    var temsel = db.TrnPlans.Where(p => p.ID_NO == ClientID && p.PLAN_CODE == PlanID).ToList().OrderByDescending(p => p.ID).Take(1).SingleOrDefault();

                    //var PayMethodsQuery = (from s in db.PayMethods select s).ToList();
                    //ViewBag.PaymentMethods = new SelectList(PayMethodsQuery, "CODE", "DSCR", temsel.SOURCE);


                    //---------------

                    // Get permitted profile only.

                    List<PayMethod> lstpayMethod = Common.Functions.GetPermittedPayMethods();

                    if (lstpayMethod != null)
                    {
                        ViewBag.PaymentMethods = new SelectList(lstpayMethod.ToList(), "CODE", "DSCR", temsel.SOURCE);
                    }

                    //----------------

                    ViewBag.EndDate = Convert.ToDateTime(temsel.PLAN_END).ToString("yyyy/MM/dd");

                    decimal planprice = 0;
                    if (temsel.PLAN_PRICE != 0)
                    {
                        planprice = temsel.PLAN_PRICE / 100;
                    }

                    ViewBag.Price = planprice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                    ViewBag.Comment = temsel.COMMENT;
                    ViewBag.TempClientID = ClientID;
                    return View(clientPlansUnlimited);
                }
                else
                {
                    var planquery = db.Plans;
                    List<Plan> planlist = new List<Plan>();
                    Plan plan = new Plan();

                    var temclientmeal1 = (from c in db.Clients
                                          where c.ID_NO == ClientID
                                          select c).FirstOrDefault();
                    Client temclientmeal = db.Clients.Where(c => c.ID_NO.Equals(ClientID)).SingleOrDefault();

                    for (int i = 0; i < planquery.ToList().Count; i++)
                    {
                        if (planquery.ToList()[i].CODE != temclientmeal1.PLAN_CODE1 && planquery.ToList()[i].CODE != temclientmeal1.PLAN_CODE2 && planquery.ToList()[i].CODE != temclientmeal1.PLAN_CODE3)
                        {
                            plan = new Plan();
                            plan.CODE = planquery.ToList()[i].CODE;
                            plan.DSCR = planquery.ToList()[i].DSCR;
                            planlist.Add(plan);
                        }
                        else if (planquery.ToList()[i].CODE == PlanID)
                        {
                            plan = new Plan();
                            plan.CODE = planquery.ToList()[i].CODE;
                            plan.DSCR = planquery.ToList()[i].DSCR;
                            planlist.Add(plan);
                        }
                    }


                    List<ClientPlansUnlimited> clientPlansUnlimiteds = new List<ClientPlansUnlimited>();
                    ClientPlansUnlimited clientPlansUnlimited = new ClientPlansUnlimited();


                    clientPlansUnlimited.Client = new Client();
                    clientPlansUnlimited.Plan = new Plan();
                    string plandscription1 = string.Empty;

                    clientPlansUnlimited.ClientID = ClientID;
                    clientPlansUnlimited.PlanID = PlanID;

                    if (PlanID == temclientmeal.PLAN_CODE1)
                    {
                        clientPlansUnlimited.PlanStartDate = temclientmeal.PLAN_START1;
                    }
                    else if (PlanID == temclientmeal.PLAN_CODE2)
                    {
                        clientPlansUnlimited.PlanStartDate = temclientmeal.PLAN_START2;
                    }
                    else if (PlanID == temclientmeal.PLAN_CODE3)
                    {
                        clientPlansUnlimited.PlanStartDate = temclientmeal.PLAN_START3;
                    }


                    clientPlansUnlimiteds.Add(clientPlansUnlimited);


                    //var clientPlansUnlimited = db.ClientPlansUnlimiteds.Where(c => c.ClientID == ClientID && c.PlanID == PlanID).SingleOrDefault();
                    if (clientPlansUnlimited == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.ClientID = new SelectList(db.Clients, "ID_NO", "FAMILY", clientPlansUnlimited.ClientID);
                    ViewBag.PlanID = new SelectList(planlist.ToList(), "CODE", "DSCR", clientPlansUnlimited.PlanID);
                    //ViewBag.PlanID = new SelectList(db.Plans, "CODE", "DSCR", clientPlansUnlimited.PlanID);


                    //var PayMethodsQuery = (from s in db.PayMethods select s).ToList();
                    //var PermissionsGrantsQuery = (from s in dbwmn.permissionsGrants select s).ToList().Where(x => x.ptype == "payMethod");
                    //var result = (from t1 in PayMethodsQuery
                    //              join t2 in PermissionsGrantsQuery
                    //              on new { commcol = (int)t1.CODE } equals new { commcol = t2.pid }
                    //              select new { t1.DSCR, t1.TYPE, t1.CODE, t1.TRANSTYPE, t2.pid, t2.ptype }).AsEnumerable();

                    var temsel = db.TrnPlans.Where(p => p.ID_NO == ClientID && p.PLAN_CODE == PlanID).ToList().OrderByDescending(p => p.ID).Take(1).SingleOrDefault();
                    if (temsel != null)
                    {


                        //ViewBag.PaymentMethods = new SelectList(PayMethodsQuery, "CODE", "DSCR", temsel.SOURCE);

                        List<PayMethod> lstpayMethod = Common.Functions.GetPermittedPayMethods();

                        if (lstpayMethod != null)
                        {
                            ViewBag.PaymentMethods = new SelectList(lstpayMethod.ToList(), "CODE", "DSCR", temsel.SOURCE);
                        }


                        ViewBag.EndDate = temsel.PLAN_END;

                        decimal planprice = 0;
                        if (temsel.PLAN_PRICE != 0)
                        {
                            planprice = temsel.PLAN_PRICE / 100;
                        }

                        ViewBag.Price = planprice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                        ViewBag.Comment = temsel.COMMENT;
                    }
                    ViewBag.TempClientID = ClientID;

                    return View(clientPlansUnlimited);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error occurred in getting plan.";
                ViewBag.MessageClass = "error-msg";
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientID,PlanID,PlanStartDate,LastMealTime,WeekMealCount,WeekDayCount,OrderIndex")] ClientPlansUnlimited clientPlansUnlimited, FormCollection fc)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }
            try
            {
                if (clientPlansUnlimited.ClientID == null || clientPlansUnlimited.PlanID == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var DB_Structure = db.Params.Where(x => x.Name.ToUpper() == "DB_Structure").SingleOrDefault();
                int DBStructure = 0;
                if (DB_Structure.Value != "")
                {
                    DBStructure = Convert.ToInt32(DB_Structure.Value);
                }
                if (DBStructure >= 15)
                {
                    if (ModelState.IsValid)
                    {
                        double price = Convert.ToDouble(fc["txtPrice"]);
                        TrnPlan trnPlan = new TrnPlan();
                        trnPlan.ID_NO = clientPlansUnlimited.ClientID;
                        trnPlan.PLAN_CODE = clientPlansUnlimited.PlanID;
                        trnPlan.PLAN_START = clientPlansUnlimited.PlanStartDate;
                        trnPlan.PLAN_PRICE = Convert.ToInt32(price) * 100;
                        trnPlan.TRN_DATE = DateTime.Now;
                        trnPlan.LOGIN = Convert.ToString(Session["UserName"]);
                        trnPlan.COMMENT = fc["txtComment"];
                        trnPlan.CreditCardAuthNumber = "0";
                        trnPlan.IsPlanNew = 1;
                        trnPlan.ACC_CODE = 0;
                        trnPlan.SOURCE = Convert.ToInt16(fc["PaymentMethods"]);
                        trnPlan.CreditCardType = -1;
                        trnPlan.CardPurchased = 0;
                        trnPlan.PLAN_END = Convert.ToDateTime(fc["txtEndDate"]);
                        db.TrnPlans.Add(trnPlan);

                        int LastPlan = Convert.ToInt32(fc["LastPlanID"]);
                        var entity = db.ClientPlansUnlimiteds.Where(c => c.ClientID == clientPlansUnlimited.ClientID && c.PlanID == LastPlan).SingleOrDefault();


                        db.ClientPlansUnlimiteds.Remove(entity);
                        db.SaveChanges();
                        clientPlansUnlimited.PlanStartDate = Convert.ToDateTime(fc["PlanStartDate"]);
                        clientPlansUnlimited.PlanID = Convert.ToInt32(fc["PlanID"]);
                        db.ClientPlansUnlimiteds.Add(clientPlansUnlimited);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var plancode1 = db.Clients.Where(c => c.ID_NO == clientPlansUnlimited.ClientID).SingleOrDefault();
                        Client client = new Client();
                        if (plancode1 != null)
                        {
                            int LastPlan = Convert.ToInt32(fc["LastPlanID"]);
                            if (plancode1.PLAN_CODE1 == LastPlan)
                            {
                                client = db.Clients.Find(clientPlansUnlimited.ClientID);
                                client.ID_NO = clientPlansUnlimited.ClientID;
                                client.PLAN_CODE1 = Convert.ToInt32(fc["PlanID"]);
                                client.PLAN_START1 = Convert.ToDateTime(fc["PlanStartDate"]);

                                db.Entry(client).State = EntityState.Modified;
                            }
                            else if (plancode1.PLAN_CODE2 == LastPlan)
                            {
                                client = db.Clients.Find(clientPlansUnlimited.ClientID);
                                client.ID_NO = clientPlansUnlimited.ClientID;
                                client.PLAN_CODE2 = Convert.ToInt32(fc["PlanID"]);
                                client.PLAN_START2 = Convert.ToDateTime(fc["PlanStartDate"]);

                                db.Entry(client).State = EntityState.Modified;

                            }
                            else if (plancode1.PLAN_CODE3 == LastPlan)
                            {
                                client = db.Clients.Find(clientPlansUnlimited.ClientID);
                                client.ID_NO = clientPlansUnlimited.ClientID;
                                client.PLAN_CODE3 = Convert.ToInt32(fc["PlanID"]);
                                client.PLAN_START3 = Convert.ToDateTime(fc["PlanStartDate"]);
                                db.Entry(client).State = EntityState.Modified;
                            }
                        }
                        double price = 0;
                        if (fc["txtPrice"] != "")
                        {
                            price = Convert.ToDouble(fc["txtPrice"]);
                        }
                        TrnPlan trnPlan = new TrnPlan();
                        trnPlan.ID_NO = clientPlansUnlimited.ClientID;
                        trnPlan.PLAN_CODE = clientPlansUnlimited.PlanID;
                        trnPlan.PLAN_START = clientPlansUnlimited.PlanStartDate;
                        trnPlan.PLAN_PRICE = Convert.ToInt32(price) * 100;
                        trnPlan.TRN_DATE = DateTime.Now;
                        trnPlan.LOGIN = Convert.ToString(Session["UserName"]);
                        trnPlan.COMMENT = fc["txtComment"];
                        trnPlan.CreditCardAuthNumber = "0";
                        trnPlan.IsPlanNew = 1;
                        trnPlan.ACC_CODE = 0;
                        trnPlan.SOURCE = Convert.ToInt16(fc["PaymentMethods"]);
                        trnPlan.CreditCardType = -1;
                        trnPlan.CardPurchased = 0;
                        if (fc["txtEndDate"] != "")
                        {
                            trnPlan.PLAN_END = Convert.ToDateTime(fc["txtEndDate"]); ;
                        }
                        db.TrnPlans.Add(trnPlan);

                        db.SaveChanges();
                    }
                }
                TempData["message"] = "Plan edited successfully.";
                ViewBag.MessageClass = "success-msg";
                return RedirectToAction("Index", new { id = clientPlansUnlimited.ClientID });
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error occurred in editing plan.";
                ViewBag.MessageClass = "error-msg";
                logger.Error("Error occurred while processing :", ex);
                return RedirectToAction("Index", new { id = clientPlansUnlimited.ClientID });
            }
        }

        #endregion

        #region
        /// <summary> for delete client mealplans
        /// 
        /// </summary>
        /// <param name="ClientID"></param>
        /// <param name="PlanID"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string ClientID, int PlanID)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_plans.ToString()) == false)
            {
                return View();
            }

            try
            {
                var DB_Structure = db.Params.Where(x => x.Name.ToUpper() == "DB_Structure").SingleOrDefault();
                int DBStructure = 0;
                if (DB_Structure.Value != "")
                {
                    DBStructure = Convert.ToInt32(DB_Structure.Value);
                }
                if (DBStructure >= 15)
                {
                    var clientPlansUnlimited = db.ClientPlansUnlimiteds.Where(c => c.ClientID == ClientID && c.PlanID == PlanID).SingleOrDefault();
                    if (clientPlansUnlimited == null)
                    {
                        return HttpNotFound();
                    }
                    db.ClientPlansUnlimiteds.Remove(clientPlansUnlimited);
                    db.SaveChanges();
                }
                else
                {
                    Client client = new Client();
                    client = db.Clients.Find(ClientID);
                    if (client.PLAN_CODE1 == PlanID)
                    {
                        client.PLAN_CODE1 = 0;
                    }
                    else if (client.PLAN_CODE2 == PlanID)
                    {
                        client.PLAN_CODE2 = 0;
                    }
                    else if (client.PLAN_CODE3 == PlanID)
                    {
                        client.PLAN_CODE3 = 0;
                    }
                    db.Entry(client).State = EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["message"] = "Plan deleted successfully.";
                ViewBag.MessageClass = "success-msg";
                return RedirectToAction("Index", new { id = ClientID });
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error occurred in deleting plan.";
                ViewBag.MessageClass = "error-msg";
                logger.Error("Error occurred while processing :", ex);

                return RedirectToAction("Index", new { id = ClientID });

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
