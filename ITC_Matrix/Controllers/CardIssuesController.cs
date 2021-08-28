using ITC_Matrix.Common;
using ITC_Matrix.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ITC_Matrix.Controllers
{
    public class CardIssuesController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.clients_node.ToString();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Updated Pin
        /// </summary>
        /// <param name="cardIssue"></param>                  
        public void UpdatePin(CardIssue cardIssue)
        {
            // Check password policy and display message accordingly ----------
            if (CheckPaswordPolicy(cardIssue.Tag_Code) == true)
            {
                Client client = db.Clients.Find(cardIssue.ID_NO);
                client.ID_NO = cardIssue.ID_NO;
                client.TAG_CODE = Common.Functions.SvcEncrypt(cardIssue.Tag_Code);
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();

                TempData["message"] = "Pin updated successfully.";
                TempData["MessageClass"] = "success-msg";
            }
            DisplayIssuedCard(cardIssue.ID_NO);
        }

        /// <summary>
        /// Check Pasword Policy
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool CheckPaswordPolicy(string pin)
        {
            string errorMessage = Common.Functions.CheckPasswordPolicy(pin, "Pin");

            if (!string.IsNullOrEmpty(errorMessage.Trim()))
            {
                TempData["message"] = errorMessage;
                TempData["MessageClass"] = "error-msg";

                return false;
            }

            return true;
        }

        /// <summary>
        /// check if the card is not issued to anybody else
        /// </summary>
        /// <param name="TAG_NO"></param>
        /// <returns></returns>
        public JsonResult IsTagNoAvailble(string TAG_NO)
        {
            var result = true;
            //int code = Convert.ToInt32(TAG_NO);

            var ClientID = db.CardIssues.Where(x => x.TAG_NO.Trim().Equals(TAG_NO.ToString().Trim())).FirstOrDefault();

            if (ClientID != null)
            {
                result = false;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Check card length
        /// </summary>
        /// <param name="TAG_NO"></param>
        /// <returns></returns>
        public JsonResult CheckCardLen(string TAG_NO)
        {
            var result = string.Empty;
            var len = db.Params.Where(x => x.Name == "CardLength").FirstOrDefault();
            if (len != null)
            {
                if (TAG_NO.Length != Convert.ToInt32(len.Value))
                {
                    result = len.Value;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Displays all card listing
        /// </summary>
        /// <param name="id"></param>
        private void DisplayIssuedCard(string id)
        {
            // check if the current card is outstanding
            CheckCardOutstanding(id);

            ViewBag.Message = TempData["message"];
            ViewBag.MessageClass = TempData["MessageClass"];
            ViewBag.strId = id;
            var lstcardissue = (from t1 in db.CardIssues
                                join t2 in db.Clients on t1.ID_NO equals t2.ID_NO
                                where t2.ID_NO == id
                                orderby t1.ISSUED_AT descending
                                select new { t1.TAG_NO, t1.ISSUED_AT, t2.TAG_EXPIRY, t1.RETURNED_AT });

            List<CardIssue> partsList = new List<CardIssue>();

            foreach (var m in lstcardissue)
            {
                CardIssue lstact = new CardIssue();
                lstact.TAG_NO = m.TAG_NO;
                lstact.StrIssue_Date = m.ISSUED_AT.ToString(GlobalVariables.DateFormat);
                lstact.StrReturn_Date = m.RETURNED_AT.ToString(GlobalVariables.DateFormat);
                partsList.Add(lstact);
            }

            ViewBag.cardissuelist = partsList;

            var checkExpirySetDate = db.Params.Where(x => x.Name.ToLower().Equals("expirysetdate")).SingleOrDefault();
            if (Convert.ToBoolean(checkExpirySetDate.Value) == true)
            {
                var checkExpiryDefaultDate = db.Params.Where(x => x.Name.ToLower().Equals("expirydefaultdate")).SingleOrDefault();
                if (checkExpiryDefaultDate != null)
                {
                    DateTime expiryDate = DateTime.Now.Date;

                    DateTime.TryParse(checkExpiryDefaultDate.Value, out expiryDate);

                    ViewBag.strExpirySetDate = expiryDate.ToString(GlobalVariables.DateFormat);
                }
            }
            else
            {
                var checkExpiryAddMonths = db.Params.Where(x => x.Name.ToLower().Equals("expiryaddmonths")).SingleOrDefault();
                DateTime dttemp = DateTime.Now.AddMonths(Convert.ToInt32(checkExpiryAddMonths.Value));
                ViewBag.strExpirySetDate = dttemp.ToString(GlobalVariables.DateFormat);
            }
        }

        /// <summary>
        /// Check Card Outstanding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private void CheckCardOutstanding(string id)
        {
            List<CardIssue> lstCardIssue = new List<CardIssue>();
            lstCardIssue = db.CardIssues.ToList().FindAll(x => x.ID_NO.Equals(id) && x.RETURNED_AT > DateTime.Now);
            if (lstCardIssue.Count == 1)
            {
                ViewBag.Outstanding = "true";
                ViewBag.Tag_No = lstCardIssue[0].TAG_NO;

                // set expiry date and status
                Client client = db.Clients.ToList().FindAll(x => x.ID_NO.Equals(id)).FirstOrDefault();

                if (client != null)
                {
                    ViewBag.Expiry_Date = client.TAG_EXPIRY.ToString(GlobalVariables.DateFormat);
                    ViewBag.ddlstatus = client.TAG_STATUS;
                }
            }
            else
            {
                ViewBag.Outstanding = "false";
            }
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public ActionResult Create(string id, string Command)
        {
            // check module permission

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client_card.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.return_client_card.ToString()) == false)
            {
                ViewBag.ReturnPermission = false;
            }

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client_card_pin.ToString()) == false)
            {
                ViewBag.PinPermission = false;
            }

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.new_client_card.ToString()) == false)
            {
                ViewBag.IssueCardPermissions = false;
            }
            
            ViewBag.Outstanding = "false";

            DisplayIssuedCard(id);

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create([Bind(Include = "Confirmtag_Code,Tag_Code,Expiry_Date,ID_NO,TAG_NO,ISSUED_AT,RETURNED_AT,COMMENT,ExpiryDate")] CardIssue cardIssue, string ddlstatus, string Command)
        {
            //check model permissions
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.new_client_card.ToString()) == false)
            {
                return View("NoAccess");
            }

            // quick fix if the validation fails, need modifications
            if (Command == null)
            {
                List<CardIssue> lstCardIssue = new List<CardIssue>();
                lstCardIssue = db.CardIssues.ToList().FindAll(x => x.ID_NO.Equals(cardIssue.ID_NO) && x.RETURNED_AT > DateTime.Now);

                if (lstCardIssue.Count == 0)
                {
                    Command = "Issue Card";
                }
            }

            try
            {
                if (Command != null)
                {
                    if (Command.Equals("Issue Card"))
                    {
                        ViewBag.Message = null;

                        //Check if the client has outstanding card
                        List<CardIssue> lstCardIssue = new List<CardIssue>();

                        lstCardIssue = db.CardIssues.ToList().FindAll(x => x.ID_NO.Equals(cardIssue.ID_NO));
                        if (lstCardIssue.FindAll(x => x.RETURNED_AT > DateTime.Now).Count > 0)
                        {
                            TempData["message"] = "There is an outstanding card for this client. Please return that card prior to issuing a new one.";
                            TempData["MessageClass"] = "error-msg";

                            DisplayIssuedCard(cardIssue.ID_NO);

                            return View();
                        }

                        var len = db.Params.Where(x => x.Name.ToLower().Equals("cardlength")).FirstOrDefault();

                        if (len != null)
                        {
                            if (cardIssue.TAG_NO.Length != Convert.ToInt32(len.Value))
                            {
                                TempData["message"] = string.Concat("Card Length must be ", len.Value, " characters.");
                                TempData["MessageClass"] = "error-msg";

                                DisplayIssuedCard(cardIssue.ID_NO);
                            }
                            else
                            {
                                // Check password policy and display message accordingly ----------
                                if (CheckPaswordPolicy(cardIssue.Tag_Code) == true)
                                {
                                    if (Convert.ToDateTime(cardIssue.Expiry_Date) < DateTime.Now.Date)
                                    {
                                        TempData["message"] = string.Concat("Expiry date must be after today.");
                                        TempData["MessageClass"] = "error-msg";
                                    }
                                    else
                                    {
                                        if (ModelState.IsValid)
                                        {
                                            Client client = db.Clients.Find(cardIssue.ID_NO);
                                            client.ID_NO = cardIssue.ID_NO;
                                            client.TAG_CODE = Common.Functions.SvcEncrypt(cardIssue.Tag_Code);
                                            client.TAG_NO = cardIssue.TAG_NO;
                                            client.TAG_ISSUE = DateTime.Now;
                                            client.TAG_EXPIRY = Convert.ToDateTime(cardIssue.Expiry_Date);
                                            client.TAG_STATUS = Convert.ToInt16(ddlstatus);

                                            db.Entry(client).State = EntityState.Modified;

                                            cardIssue.ISSUED_AT = DateTime.Now;
                                            cardIssue.COMMENT = string.Empty;
                                            cardIssue.RETURNED_AT = Convert.ToDateTime("2173-10-14");
                                            db.CardIssues.Add(cardIssue);
                                            db.SaveChanges();

                                            TempData["message"] = string.Concat("Card number ", cardIssue.TAG_NO, " issued.");
                                            TempData["MessageClass"] = "success-msg";
                                        }
                                    }
                                }

                                DisplayIssuedCard(cardIssue.ID_NO);
                            }
                        }
                    }
                    else if (Command.Equals("Update Card"))
                    {
                        bool result = true;
                        Client client = db.Clients.Find(cardIssue.ID_NO);
                        cardIssue.TAG_NO = client.TAG_NO;

                        // if Pin is not updated then get from client table and set
                        if (cardIssue.Tag_Code != null)
                        {
                            // Check password policy and display message accordingly ----------
                            if (CheckPaswordPolicy(cardIssue.Tag_Code) == true)
                            {
                                cardIssue.Tag_Code = Common.Functions.SvcEncrypt(cardIssue.Tag_Code);
                                result = true;
                            }
                        }
                        else
                        {
                            cardIssue.Tag_Code = client.TAG_CODE;
                            cardIssue.Confirmtag_Code = client.TAG_CODE;
                        }

                        if (result == true)
                        {
                            if (Convert.ToDateTime(cardIssue.Expiry_Date) < DateTime.Now.Date)
                            {
                                TempData["message"] = string.Concat("Expiry date must be after today.");
                                TempData["MessageClass"] = "error-msg";
                            }
                            else
                            {
                                client.TAG_CODE = cardIssue.Tag_Code;
                                client.TAG_EXPIRY = Convert.ToDateTime(cardIssue.Expiry_Date);
                                client.TAG_STATUS = Convert.ToInt16(ddlstatus);

                                db.Entry(client).State = EntityState.Modified;
                                db.SaveChanges();

                                TempData["message"] = string.Concat("Card number ", cardIssue.Tag_Code, " updated.");
                                TempData["MessageClass"] = "success-msg";
                            }
                        }

                        DisplayIssuedCard(cardIssue.ID_NO);
                    }
                    else if (Command.Equals("Set Pin"))
                    {
                        //check model permissions
                        if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client_card_pin.ToString()) == false)
                        {
                            return View("NoAccess");
                        }

                        UpdatePin(cardIssue);
                    }
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

        public ActionResult ReturnCard(string Tag_No, string ClientID, string Command)
        {
            //check model permissions
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.return_client_card.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                Client client = db.Clients.Find(ClientID);
                string Tag_Code = client.TAG_CODE;

                client.TAG_CODE = string.Empty;
                client.TAG_NO = string.Empty;
                client.TAG_STATUS = 0;
                db.Entry(client).State = EntityState.Modified;
                CardIssue cardissue = (db.CardIssues.Where(x => x.ID_NO == ClientID)
                                   .Where(x => x.TAG_NO == Tag_No)).FirstOrDefault();

                DateTime dtissue = client.TAG_ISSUE;
                db.CardIssues.Remove(cardissue);
                db.SaveChanges();

                CardIssue cardissue2 = new CardIssue();
                cardissue2.Tag_Code = Tag_Code;
                cardissue2.Confirmtag_Code = Tag_Code;
                cardissue2.Expiry_Date = DateTime.Now.ToString(GlobalVariables.DateFormat);

                cardissue2.ID_NO = ClientID;
                cardissue2.TAG_NO = Tag_No;
                cardissue2.RETURNED_AT = DateTime.Now;
                cardissue2.ISSUED_AT = dtissue;
                cardissue2.COMMENT = string.Empty;
                db.CardIssues.Add(cardissue2);

                db.SaveChanges();

                TempData["message"] = string.Concat("Card Number ", Tag_No, " returned.");
                TempData["MessageClass"] = "success-msg";

                DisplayIssuedCard(ClientID);

                return RedirectToAction("Create", new { id = ClientID });
            }
            catch (Exception ex)
            {
                TempData["message"] = string.Concat("Error occurred while processing return card.");
                TempData["MessageClass"] = "error-msg";

                logger.Error("Error occurred while processing return card:", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
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
