/*
   FileFor : Managing Credentials for unlimited database structure
   FileName : CredentialsController.cs
   Created Date : 02-02-2016
   Created By : Nahid Shaikh
   Modified Date :02-02-2016
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
    public class CredentialsController : Controller
    {
        #region ---------- Class Variables ----------

        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.clients_node.ToString();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        public ActionResult Index(bool? searchBy, int? page, string id)
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

            ViewBag.CredentialMessage = TempData["CredentialMessage"];
            ViewBag.MessageClass = TempData["MessageClass"];

            int pageSize = Common.Functions.GetPageSize();

            List<Credential> lstCredential = new List<Credential>();
            lstCredential = db.Credentials.ToList();

            if (!string.IsNullOrEmpty(id))
            {                
                lstCredential = db.Credentials.ToList().FindAll(x => x.ClientID.ToLower().Equals(id.Trim().ToLower()));
            }

            if (searchBy == null)
            {
                searchBy = true;
            }

            if (searchBy == true)
            {
                lstCredential = lstCredential.Where(x => x.Valid == 1).ToList();
            }

            ViewBag.ClientID = id;

            return View(lstCredential.ToPagedList(page ?? 1, pageSize));
        }

        // GET: Credentials/Create
        public ActionResult Create(string id)
        {
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.new_client_card.ToString()) == false)
            {
                return View("NoAccess");
            }

            ViewBag.CredentialTypeID = new SelectList(db.CredentialTypes, "CredentialTypeID", "CredentialName");

            var checkExpirySetDate = db.Params.Where(x => x.Name.ToLower().Equals("expirysetdate")).SingleOrDefault();
            if (Convert.ToBoolean(checkExpirySetDate.Value) == true)
            {
                var checkExpiryDefaultDate = db.Params.Where(x => x.Name.ToLower().Equals("expirydefaultdate")).SingleOrDefault();
                if (checkExpiryDefaultDate != null)
                {
                    DateTime expiryDate = DateTime.Now.Date;

                    DateTime.TryParse(checkExpiryDefaultDate.Value, out expiryDate);

                    ViewBag.ExpiryDate = expiryDate.ToString(GlobalVariables.DateFormat);
                }
            }
            else
            {
                var checkExpiryAddMonths = db.Params.Where(x => x.Name.ToLower().Equals("expiryaddmonths")).SingleOrDefault();
                DateTime dttemp = DateTime.Now.AddMonths(Convert.ToInt32(checkExpiryAddMonths.Value));
                ViewBag.ExpiryDate = dttemp.ToString(GlobalVariables.DateFormat);
            }

            ViewBag.ClientID = id;

            return View();
        }

        // POST: Credentials/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientID,CredentialTypeID,Auth1,Auth2,Confirm_Auth2,ManuelEntryAllowed_bool,Auth2Required_bool,ExpiryDate,ChangedDate,Reset,Valid_bool,Locked,LockoutAttempts,ReturnedDate,PrimaryCredential_bool")] Credential credential)
        {
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.new_client_card.ToString()) == false)
            {
                return View("NoAccess");
            }

            credential.Auth2 = string.Empty;
            credential.Confirm_Auth2 = credential.Auth2;
            credential.Valid = 1;

            if (ModelState.IsValid)
            {
                try
                {
                    SetDefault(credential);
                    
                    db.Credentials.Add(credential);
                    db.SaveChanges();

                    // if current credential is primary then set other credentials for the selected client as non-primary
                    UpdatePrimary(credential);

                    TempData["CredentialMessage"] = "Credentials added successfully.";
                    TempData["MessageClass"] = "success-msg";

                    return RedirectToAction("Index", new { id = credential.ClientID });
                }
                catch   (Exception ex)
                {
                    TempData["CredentialMessage"] = "Error occurred in adding a credentials.";
                    TempData["MessageClass"] = "error-msg";
                    logger.Error("Error occurred while adding credentials:", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
            }

            ViewBag.CredentialTypeID = new SelectList(db.CredentialTypes, "CredentialTypeID", "CredentialName", credential.CredentialTypeID);
            return View(credential);
        }

        // GET: Credentials/Edit/5
        public ActionResult Edit(int? cid, string aid)
        {
            // check module permission

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client_card.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client_card_pin.ToString()) == false)
            {
                ViewBag.PinPermission = false;
            }

            if (cid == null || string.IsNullOrEmpty(aid))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Credential credential = db.Credentials.Find(cid, aid);

            if (credential == null)
            {
                return HttpNotFound();
            }

            ViewBag.ExpiryDate = credential.ExpiryDate.Value.ToString(GlobalVariables.DateFormat);

            ViewBag.CredentialTypeID = new SelectList(db.CredentialTypes, "CredentialTypeID", "CredentialName", 0);

            ViewBag.Pin = credential.Auth2;

            ViewBag.CredentialMessage = TempData["CredentialMessage"];
            ViewBag.MessageClass = TempData["MessageClass"];

            return View(credential);
        }

        // POST: Credentials/Edit/5        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientID,CredentialTypeID,Auth1,Auth2,Confirm_Auth2,ManuelEntryAllowed_bool,Auth2Required_bool,ExpiryDate,ChangedDate,Reset,Valid_bool,Locked,LockoutAttempts,ReturnedDate,PrimaryCredential_bool")] Credential credential)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client_card.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_client_card_pin.ToString()) == false)
            {
                ViewBag.PinPermission = false;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string pin = Convert.ToString(Request.Params["Pin"]);
                    string plainPin = credential.Auth2;     // store in pin to check password policy as, below we will be encrypting pin

                    if (string.IsNullOrEmpty(credential.Auth2))
                    {   
                        if (!string.IsNullOrEmpty(pin))
                        {
                            credential.Auth2 = pin;
                            credential.Confirm_Auth2 = pin;
                        }
                        else
                        {
                            credential.Auth2 = string.Empty;
                            credential.Confirm_Auth2 = string.Empty;
                        }
                    }
                    else
                    {                        
                        credential.Auth2 = Common.Functions.SvcEncrypt(credential.Auth2);
                        credential.Confirm_Auth2 = credential.Auth2;
                    }

                    if (!string.IsNullOrEmpty(plainPin))
                    {
                        // Check password policy and display message accordingly ----------
                        if (CheckPaswordPolicy(plainPin) == true)
                        {
                            EditCredentials(credential);

                            return RedirectToAction("Index", new { id = credential.ClientID });
                        }
                        else
                        {
                            return RedirectToAction("Edit", new { cid = credential.CredentialTypeID, aid = credential.Auth1 });
                        }
                    }
                    else
                    {
                        EditCredentials(credential);

                        return RedirectToAction("Index", new { id = credential.ClientID });
                    }
                }
                catch (Exception ex)
                {
                    TempData["CredentialMessage"] = "Error occurred in updating a credentials.";
                    TempData["MessageClass"] = "error-msg";
                    logger.Error("Error occurred while processing credentials:", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
            }

            ViewBag.CredentialTypeID = new SelectList(db.CredentialTypes, "CredentialTypeID", "CredentialName", credential.CredentialTypeID);

            return View(credential);
        }

        private void EditCredentials(Credential credential)
        {
            SetDefault(credential);

            db.Entry(credential).State = EntityState.Modified;
            db.SaveChanges();

            // if current credential is primary then set other credentials for the selected client as non-primary
            UpdatePrimary(credential);

            TempData["CredentialMessage"] = "Credentials updated successfully.";
            TempData["MessageClass"] = "success-msg";
        }

        private void SetDefault(Credential credential)
        {
            credential.ChangedDate = DateTime.Now;
            credential.Locked = 0;
            credential.LockoutAttempts = 0;
            credential.Reset = 0;

            if (Request.Params["ReturnedDate"] != null)
            {
                credential.ReturnedDate = Convert.ToDateTime(Request.Params["ReturnedDate"]);
            }
            else
            {
                credential.ReturnedDate = Convert.ToDateTime("2173-10-14");
            }

            //// Check if the card is returned
            //var objcredential = db.Credentials.Find(credential.CredentialTypeID, credential.Auth1);

            //if (objcredential != null)
            //{
            //    credential.ReturnedDate = objcredential.ReturnedDate;
            //}
            //else
            //{
            //    credential.ReturnedDate = Convert.ToDateTime("2173-10-14");
            //}
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
                TempData["CredentialMessage"] = errorMessage;
                TempData["MessageClass"] = "error-msg";

                return false;
            }

            return true;
        }
        
        public ActionResult Cancel(int? cid, string aid)
        {
            Credential credential = db.Credentials.Find(cid, aid);

            if (credential == null)
            {
                return HttpNotFound();
            }

            try
            {
                credential.Valid = 0;
                credential.ReturnedDate = DateTime.Now;
                credential.Confirm_Auth2 = credential.Auth2;

                db.Entry(credential).State = EntityState.Modified;
                db.SaveChanges();

                TempData["CredentialMessage"] = "Credential cancelled successfully.";
                TempData["MessageClass"] = "success-msg";

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                TempData["CredentialMessage"] = "Error occurred in canceling a credentials.";
                TempData["MessageClass"] = "error-msg";
                logger.Error("Error occurred while canceling credentials:", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
        
        private void UpdatePrimary(Credential credential)
        {
            if (credential.PrimaryCredential == 1)
            {
                List<Credential> lstCredential = new List<Credential>();

                lstCredential = db.Credentials.ToList().FindAll(x => x.ClientID.ToLower().Equals(credential.ClientID.ToLower())
                                                             && !x.Auth1.ToLower().Equals(credential.Auth1.ToLower()) && x.PrimaryCredential == 1);

                for (int i = 0; i < lstCredential.Count; i++)
                {
                    lstCredential[i].PrimaryCredential = 0;
                    lstCredential[i].Confirm_Auth2 = lstCredential[i].Auth2;
                    db.Entry(lstCredential[i]).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public JsonResult IsCredentialAvailble(string Auth1, int CredentialTypeID, string Flag)
        {
            var result = true;
            Credential objCredential = new Credential();
            
            if (Flag.Equals("0"))        // check while adding
            {
                objCredential = db.Credentials.Where(x => x.Auth1.Trim().Equals(Auth1.Trim())).FirstOrDefault();
            }
            else if(Flag.Equals("1"))              // check while editing
            {
                objCredential = db.Credentials.Where(x => x.Auth1.Trim().ToLower().Equals(Auth1.Trim().ToLower()) && x.CredentialTypeID != CredentialTypeID).FirstOrDefault();
            }

            if (objCredential != null)
            {
                result = false;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

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
