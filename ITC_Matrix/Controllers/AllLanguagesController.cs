/*
   FileFor : Managing the Client Profile Model
   FileName : AllLanguagesController.cs
   Created Date : 21-12-2015
   Created By : Sandip Katore
   Modified Date :28-01-2016
*/

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
    public class AllLanguagesController : Controller
    {
        #region ---------- Local Variables ----------

        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmm = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.sys_languages.ToString();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region ---------- Action Methods ----------

        /// <summary>
        /// All languages index method
        /// </summary>
        /// <returns></returns>
        // GET: AllLanguages
        public ActionResult Index()
        { 
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageLanguages.ToString()) == false)
            {
                return View("NoAccess");
            }

            ViewBag.AllLanguagesmessage = TempData["AllLanguagesmessage"];
            ViewBag.Class = TempData["class"];

            //getting the default languages from the pref table
            var defaultLanguage = dbwmm.prefs.Where(x => x.name == "langdefault").Select(y => y.value).Single();

            ViewBag.defaultLanguage = GetLanguageCode(defaultLanguage);
            
            ViewBag.Languages = new SelectList(GetLanguages(), "lang", "name");

            return View(db.Languages.ToList());
        }
        
        /// <summary>
        /// Index post
        /// </summary>
        /// <param name="Languages"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string Languages)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageLanguages.ToString()) == false)
            {
                return View("NoAccess");
            }

            ViewBag.AllLanguagesmessage = TempData["AllLanguagesmessage"];
            ViewBag.Class = TempData["class"];

            if (string.IsNullOrEmpty(Languages))
            {
                TempData["AllLanguagesmessage"] = "Please select the language.";
                TempData["class"] = "error-msg";
               
                return RedirectToAction("Index");
            }
           
            Language objLanguage = new Language();

            AllLanguage objAllLanguage = dbwmm.AllLanguages.ToList().FindAll(x => x.lang.Equals(Languages)).FirstOrDefault();
            string languageName = string.Empty;

            if (objAllLanguage != null)
            {
                languageName = objAllLanguage.name;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(languageName))
                    {
                        objLanguage.DSCR = languageName;
                        db.Languages.Add(objLanguage);
                        db.SaveChanges();

                        // Add default labels for selected language
                        AddDefaultLanguage(objLanguage);

                        TempData["AllLanguagesmessage"] = "Language added successfully.";
                        TempData["class"] = "success-msg";
                    }
                    else
                    {
                        TempData["AllLanguagesmessage"] = "Error occurred in adding a language.";
                        TempData["class"] = "error-msg";
                        logger.Error("Error occurred while processing language:");
                        ViewBag.Error = TempData["message"];

                        return View("Error");
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["AllLanguagesmessage"] = "Error occurred in adding a language.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing language:", ex);
                    ViewBag.Error = ex.ToString();

                    return View("Error");
                }
            }

            return RedirectToAction("Index");
        }
               
        // POST: AllLanguages/Delete/5
        /// <summary>
        /// delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(short id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageLanguages.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                Language objLanguage = new Language();

                objLanguage = db.Languages.Find(id);

                db.Entry(objLanguage).State = EntityState.Modified;
                db.Languages.Remove(objLanguage);
                db.SaveChanges();

                DeleteWebLabels(objLanguage);

                TempData["AllLanguagesmessage"] = "Language deleted successfully.";
                TempData["class"] = "success-msg";
               
                return Json(new { Success = true });
            }
            catch (System.Exception ex)
            {
                TempData["AllLanguagesmessage"] = "Error occurred in deleting a language.";
                TempData["class"] = "error-msg";
                logger.Error("Error occurred while processing :", ex);

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
                dbwmm.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region ---------- User Defined Functions ----------

        /// <summary>
        /// Get Languages
        /// </summary>
        /// <returns></returns>
        private List<AllLanguage> GetLanguages()
        {
            List<AllLanguage> lstAllLanguage = new List<AllLanguage>();

            var filtered = dbwmm.AllLanguages.ToList().Where(x => !db.Languages.ToList().Any(y => y.DSCR == x.name));

            foreach (var item in filtered)
            {
                AllLanguage objAllLanguage = new AllLanguage();
                objAllLanguage.lang = item.lang;
                objAllLanguage.name = item.name;

                lstAllLanguage.Add(objAllLanguage);
            }

            return lstAllLanguage;
        }

        /// <summary>
        /// Get Language Code
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private short GetLanguageCode(string lang)
        {
            short code = 0;

            AllLanguage objAllLanguage = dbwmm.AllLanguages.ToList().FindAll(x => x.lang.Equals(lang)).FirstOrDefault();
            Language objLanguage = new Language();

            if (objAllLanguage != null)
            {
                objLanguage = db.Languages.ToList().FindAll(x => x.DSCR.ToLower().Equals(objAllLanguage.name.ToLower())).FirstOrDefault();

                if (objLanguage != null)
                {
                    code = (Int16)objLanguage.Code;
                }
            }

            return code;
        }

        /// <summary>
        /// Add Default Language
        /// </summary>
        /// <param name="objLanguage"></param>
        private void AddDefaultLanguage(Language objLanguage)
        {
            List<CommMessage> lstDefaults = db.CommMessages.ToList().FindAll(x => x.Language == 0 && x.MsgGroup.Trim().ToLower().Equals(Common.CommonEnum.MessageGroup.WebLabel.ToString().ToLower()));

            CommMessage objCommMessage = new CommMessage();

            // update and bulk insert default language weblabels for selected language
            lstDefaults.ForEach(x => x.Language = (Int16)objLanguage.Code);
            lstDefaults.ForEach(x => x.UseCustom = 0);

            db.CommMessages.AddRange(lstDefaults);
            db.SaveChanges();
        }
        
        /// <summary>
        /// Delete weblabels
        /// </summary>
        /// <param name="objLanguage"></param>
        private void DeleteWebLabels(Language objLanguage)
        {
            CommMessage objCommMessage = new CommMessage();

            try
            {
                List<CommMessage> lstDelete = db.CommMessages.ToList().FindAll(x => x.Language == objLanguage.Code);

                ArchiveCommMessage objArchiveCommMessage = new ArchiveCommMessage();
                List<ArchiveCommMessage> lstArchiveCommMessage = new List<ArchiveCommMessage>();

                if (lstDelete.Count > 0)
                {
                    for (int i = 0; i < lstDelete.Count; i++)
                    {
                        // set archive list
                        objArchiveCommMessage = new ArchiveCommMessage();

                        objArchiveCommMessage.MessageID = lstDelete[i].MessageID;
                        objArchiveCommMessage.DefaultMessage = lstDelete[i].DefaultMessage;
                        objArchiveCommMessage.CustomMessage = lstDelete[i].CustomMessage;
                        objArchiveCommMessage.MessageLineLenght = lstDelete[i].MessageLineLenght;
                        objArchiveCommMessage.UseCustom = lstDelete[i].UseCustom;
                        objArchiveCommMessage.Language = lstDelete[i].Language;
                        objArchiveCommMessage.MsgGroup = lstDelete[i].MsgGroup;
                        objArchiveCommMessage.MsgType = lstDelete[i].MsgType;
                        objArchiveCommMessage.DSCR = lstDelete[i].DSCR;

                        lstArchiveCommMessage.Add(objArchiveCommMessage);
                    }
                }

                // Archive all weblabels for deleted language
                if (lstArchiveCommMessage.Count > 0)
                {
                    db.ArchiveCommMessages.AddRange(lstArchiveCommMessage);
                    db.SaveChanges();
                }

                // Remove all weblabels of deleted language
                db.CommMessages.RemoveRange(db.CommMessages.Where(x => x.Language == objLanguage.Code));
                db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                TempData["AllLanguagesmessage"] = "Error occurred in deleting weblabels.";
                TempData["class"] = "error-msg";
                logger.Error("Error occurred while deleting weblabels :", ex);
            }
        }

        #endregion
    }
}
