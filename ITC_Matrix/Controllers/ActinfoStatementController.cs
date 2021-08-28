using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using ITC_Matrix.Models;
using ITC_Matrix.Common;
using System.Data.Entity;

namespace ITC_Matrix.Controllers
{
    public class ActinfoStatementController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.clients_node.ToString();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID_NO"></param>
        /// <param name="ActCode"></param>
        /// <returns></returns>
        public ActionResult ShowStatements(string ID_NO, string ActCode)
        {
            // check module permission         
            string straccCode = string.Concat("acc", ActCode, "_statement");

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, straccCode) == false)
            {
                return View("NoAccess");
                //return new JsonResult() { Data = new { name = "NoAccess" } };
            }

            short intActCode = Convert.ToInt16(ActCode);

            var limitVal = (db.Accounts.Where(x => x.CODE == intActCode)).SingleOrDefault();

            if (limitVal != null)
            {
                ViewBag.ActCode = limitVal.DSCR;
            }
            ViewBag.ID_NO = ID_NO;
            var clientName = (db.Clients.Where(x => x.ID_NO == ID_NO)).SingleOrDefault();

            if (limitVal != null)
            {
                ViewBag.ClientName = clientName.GIVEN;
            }

            List<TrnReg> trnReg = new List<TrnReg>();
            TrnReg temptrnReg = new TrnReg();

            var LstTrnReg = db.TrnRegs.Where(x => x.ID_NO.Equals(ID_NO) && x.ACC_CODE.Equals(intActCode) && x.TRN_DATE.Month.Equals(DateTime.Now.Month) && x.TRN_DATE.Year.Equals(DateTime.Now.Year));
            var lsttrndep = db.TrnDeps.Where(x => x.ID_NO.Equals(ID_NO) && x.ACC_CODE.Equals(intActCode) && x.TRN_DATE.Month.Equals(DateTime.Now.Month) && x.TRN_DATE.Year.Equals(DateTime.Now.Year));

            foreach (var item in LstTrnReg)
            {
                temptrnReg = new TrnReg();
                temptrnReg.ID_NO = item.ID_NO;
                temptrnReg.TRN_DATE = item.TRN_DATE;
                temptrnReg.BALANCE = item.BALANCE;
                temptrnReg.AMOUNT = item.AMOUNT;
                temptrnReg.Comment = Convert.ToString(item.TRN_NO);
                temptrnReg.flgCredit = false;
                temptrnReg.TRN_NO = item.TRN_NO;
                trnReg.Add(temptrnReg);
            }
            foreach (var item in lsttrndep)
            {
                temptrnReg = new TrnReg();
                temptrnReg.ID_NO = item.ID_NO;
                temptrnReg.TRN_DATE = item.TRN_DATE;
                temptrnReg.BALANCE = item.BALANCE;
                temptrnReg.AMOUNT = item.AMOUNT;
                temptrnReg.Comment = item.COMMENT;

                PayMethod paysrc = (db.PayMethods.Where(x => x.CODE == item.SOURCE)).SingleOrDefault();
                temptrnReg.strPaidby = paysrc.DSCR;
                if (paysrc.TRANSTYPE == "1")
                {
                    temptrnReg.flgCredit = true;
                }
                else
                {
                    temptrnReg.flgCredit = false;
                }

                trnReg.Add(temptrnReg);
            }

            return View(trnReg.ToList().OrderByDescending(x => x.TRN_DATE));
        }


        //Statements Generated as per request.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="ID_NO"></param>
        /// <param name="ActCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShowStatements(int searchBy, string ID_NO, short ActCode)
        {
            // check module permission

            string straccCode = string.Concat("acc", ActCode, "_statement");

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, straccCode) == false)
            {
                return View("NoAccess");
            }

            ViewBag.ID_NO = ID_NO;
            var limitVal = (db.Accounts.Where(x => x.CODE == ActCode)).SingleOrDefault();

            if (limitVal != null)
            {
                ViewBag.ActCode = limitVal.DSCR;
            }


            var clientName = (db.Clients.Where(x => x.ID_NO == ID_NO)).SingleOrDefault();

            if (limitVal != null)
            {
                ViewBag.ClientName = clientName.GIVEN;
            }

            List<TrnReg> trnReg = new List<TrnReg>();
            TrnReg temptrnReg = new TrnReg();
            if (searchBy == (int)Common.CommonEnum.SearchByPeriod.This_Month)
            {
                var LstTrnReg = db.TrnRegs.Where(x => x.ID_NO.Equals(ID_NO) && x.ACC_CODE.Equals(ActCode) && x.TRN_DATE.Month.Equals(DateTime.Now.Month) && x.TRN_DATE.Year.Equals(DateTime.Now.Year));
                var lsttrndep = db.TrnDeps.Where(x => x.ID_NO.Equals(ID_NO) && x.ACC_CODE.Equals(ActCode) && x.TRN_DATE.Month.Equals(DateTime.Now.Month) && x.TRN_DATE.Year.Equals(DateTime.Now.Year));

                foreach (var item in LstTrnReg)
                {
                    temptrnReg = new TrnReg();
                    temptrnReg.ID_NO = item.ID_NO;
                    temptrnReg.TRN_DATE = item.TRN_DATE;
                    temptrnReg.BALANCE = item.BALANCE;
                    temptrnReg.AMOUNT = item.AMOUNT;
                    temptrnReg.Comment = Convert.ToString(item.TRN_NO);
                    temptrnReg.flgCredit = false;
                    temptrnReg.TRN_NO = item.TRN_NO;

                    trnReg.Add(temptrnReg);
                }
                foreach (var item in lsttrndep)
                {
                    temptrnReg = new TrnReg();
                    temptrnReg.ID_NO = item.ID_NO;
                    temptrnReg.TRN_DATE = item.TRN_DATE;
                    temptrnReg.BALANCE = item.BALANCE;
                    temptrnReg.AMOUNT = item.AMOUNT;
                    temptrnReg.Comment = item.COMMENT;
                    PayMethod paysrc = (db.PayMethods.Where(x => x.CODE == item.SOURCE)).SingleOrDefault();
                    temptrnReg.strPaidby = paysrc.DSCR;
                    if (paysrc.TRANSTYPE == "1")
                    {
                        temptrnReg.flgCredit = true;
                    }
                    else
                    {
                        temptrnReg.flgCredit = false;
                    }

                    trnReg.Add(temptrnReg);
                }
            }
            else if (searchBy == (int)Common.CommonEnum.SearchByPeriod.Last_Month)
            {
                int lastmonth = DateTime.Now.AddMonths(-1).Month;
                var LstTrnReg = db.TrnRegs
                               .Where(x => x.ID_NO.Equals(ID_NO) && x.ACC_CODE.Equals(ActCode) && x.TRN_DATE.Month == lastmonth && x.TRN_DATE.Year.Equals(DateTime.Now.Year));
                var lsttrndep = db.TrnDeps
                               .Where(x => x.ID_NO.Equals(ID_NO) && x.ACC_CODE.Equals(ActCode) && x.TRN_DATE.Month == lastmonth && x.TRN_DATE.Year.Equals(DateTime.Now.Year));

                foreach (var item in LstTrnReg)
                {
                    temptrnReg = new TrnReg();
                    temptrnReg.ID_NO = item.ID_NO;
                    temptrnReg.TRN_DATE = item.TRN_DATE;
                    temptrnReg.BALANCE = item.BALANCE;
                    temptrnReg.AMOUNT = item.AMOUNT;
                    temptrnReg.Comment = Convert.ToString(item.TRN_NO);
                    temptrnReg.flgCredit = false;
                    temptrnReg.TRN_NO = item.TRN_NO;

                    trnReg.Add(temptrnReg);
                }
                foreach (var item in lsttrndep)
                {
                    temptrnReg = new TrnReg();
                    temptrnReg.ID_NO = item.ID_NO;
                    temptrnReg.TRN_DATE = item.TRN_DATE;
                    temptrnReg.BALANCE = item.BALANCE;
                    temptrnReg.AMOUNT = item.AMOUNT;
                    temptrnReg.Comment = item.COMMENT;
                    PayMethod paysrc = (db.PayMethods.Where(x => x.CODE == item.SOURCE)).SingleOrDefault();
                    temptrnReg.strPaidby = paysrc.DSCR;
                    if (paysrc.TRANSTYPE == "1")
                    {
                        temptrnReg.flgCredit = true;
                    }
                    else
                    {
                        temptrnReg.flgCredit = false;
                    }
                    trnReg.Add(temptrnReg);
                }
            }
            else if (searchBy == (int)Common.CommonEnum.SearchByPeriod.All_Periods)
            {
                var LstTrnReg = db.TrnRegs.Where(x => x.ID_NO.Equals(ID_NO) && x.ACC_CODE.Equals(ActCode));
                var lsttrndep = db.TrnDeps.Where(x => x.ID_NO.Equals(ID_NO) && x.ACC_CODE.Equals(ActCode));

                foreach (var item in LstTrnReg)
                {
                    temptrnReg = new TrnReg();
                    temptrnReg.ID_NO = item.ID_NO;
                    temptrnReg.TRN_DATE = item.TRN_DATE;
                    temptrnReg.BALANCE = item.BALANCE;
                    temptrnReg.AMOUNT = item.AMOUNT;
                    temptrnReg.Comment = Convert.ToString(item.TRN_NO);
                    temptrnReg.flgCredit = false;
                    temptrnReg.TRN_NO = item.TRN_NO;

                    trnReg.Add(temptrnReg);
                }
                foreach (var item in lsttrndep)
                {
                    temptrnReg = new TrnReg();
                    temptrnReg.ID_NO = item.ID_NO;
                    temptrnReg.TRN_DATE = item.TRN_DATE;
                    temptrnReg.BALANCE = item.BALANCE;
                    temptrnReg.AMOUNT = item.AMOUNT;
                    temptrnReg.Comment = item.COMMENT;
                    PayMethod paysrc = (db.PayMethods.Where(x => x.CODE == item.SOURCE)).SingleOrDefault();
                    temptrnReg.strPaidby = paysrc.DSCR;
                    if (paysrc.TRANSTYPE == "1")
                    {
                        temptrnReg.flgCredit = true;
                    }
                    else
                    {
                        temptrnReg.flgCredit = false;
                    }
                    trnReg.Add(temptrnReg);
                }
            }

            return View(trnReg.ToList().OrderByDescending(x => x.TRN_DATE));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strperiod"></param>
        /// <param name="ID_NO"></param>
        /// <param name="ActCode"></param>
        /// <returns></returns>
        public ActionResult GetData(string strperiod, string ID_NO, string ActCode)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName) == false)
            {
                return View();
            }

            var limitVal = (db.Accounts.Where(x => x.DSCR == ActCode)).SingleOrDefault();
            short intActCode = Convert.ToInt16(limitVal.CODE);

            if (strperiod == "This Month")
            {
                return View(db.TrnRegs.ToList().Where(x => x.ID_NO == ID_NO && x.ACC_CODE == intActCode && x.REG_DATE.Month == DateTime.Now.Month && x.REG_DATE.Year == DateTime.Now.Year));
            }
            else if (strperiod == "Last Month")
            {
                return View(db.TrnRegs.ToList().Where(x => x.ID_NO == ID_NO && x.ACC_CODE == intActCode && x.REG_DATE.AddMonths(-1) == DateTime.Now.AddMonths(-1) && x.REG_DATE.Year == DateTime.Now.Year));
            }
            else if (strperiod == "All Periods")
            {
                return View(db.TrnRegs.ToList().Where(x => x.ID_NO == ID_NO && x.ACC_CODE == intActCode));
            }
            return View(db.TrnRegs.ToList().Where(x => x.ID_NO == ID_NO && x.ACC_CODE == intActCode && x.REG_DATE.Month == DateTime.Now.Month));
        }


        #region
        // GET: Account Statement information/Delete
        [HttpPost]
        public ActionResult Delete(string ID_NO, string ActCode)
        {
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client.ToString()) == false)
            {
                return View("NoAccess");
            }

            List<Client> lstClient = new List<Client>();
            lstClient = db.Clients.ToList().FindAll(x => x.ID_NO == ID_NO);

            if (lstClient.Count > 0)
            {
                var DBstructure = Common.Functions.GetDBStructure();
                try
                {
                    if (DBstructure >= (int)CommonEnum.DBStructure.Unlimited)
                    {
                        Int16 Actcode = Convert.ToInt16(ActCode);
                        List<ClientAccount> lstClientAccount = db.ClientAccounts.Where(p => p.ClientID == ID_NO && p.AccountId == Actcode).ToList();

                        if (lstClientAccount.Count > 0)
                        {
                            db.ClientAccounts.RemoveRange(lstClientAccount);
                            db.SaveChanges();
                            TempData["message"] = "Account balance deleted successfully.";
                            TempData["class"] = "success-msg";
                        }
                    }                    
                    return RedirectToAction("ShowStatements");
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Error occurred in deleting a Account Information.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");

                }

            }
            return RedirectToAction("Index");

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
