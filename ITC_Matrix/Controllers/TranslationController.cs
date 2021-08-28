/*
   FileFor : Managing Translations and common messages
   FileName : TranslationController.cs
   Created Date : 25-01-2016
   Created By : Nahid Shaikh
   Modified Date :25-01-2016
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ITC_Matrix.Models;
using PagedList;
using ITC_Matrix.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using static ITC_Matrix.Common.CommonEnum;

namespace ITC_Matrix.Controllers
{
    public class TranslationController : Controller
    {
        #region ---------- Class Variables ----------

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        New_ITC_WMMEntities dbwmm = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.sys_translations.ToString();

        #endregion

        #region ---------- Action Methods ----------

        // GET: CommMessages
        public ActionResult Index(string searchBy, Int32? Language, string sortBy, int? page)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageTranslations.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (!string.IsNullOrEmpty(searchBy) && Language != null)
            {
                Session["searchBy"] = searchBy;
                Session["Language"] = Convert.ToString(Language);
            }


            // message showing insert/update/delete status
            ViewBag.TranslationMessage = TempData["Translationmessage"];
            ViewBag.Class = TempData["class"];

            int pageSize = Functions.GetPageSize();

            if (Session["Language"] != null)
            {
                ViewBag.Language = new SelectList(db.Languages, "CODE", "DSCR", Session["Language"].ToString());
            }
            else
            {
                ViewBag.Language = new SelectList(db.Languages.ToList(), "CODE", "DSCR");
            }
            if (Session["searchBy"] != null)
            {
                ViewBag.searchBy = new SelectList(db.CommMessages.Select(m => m.MsgGroup).Distinct().ToList(), Session["searchBy"].ToString());
            }
            else
            {
                List<string> lstmsgroup = new List<string>();
                lstmsgroup = db.CommMessages.Select(m => m.MsgGroup).Distinct().ToList();
                ViewBag.searchBy = new SelectList(lstmsgroup.ToList());
             
            }

            //for sorting
            ViewBag.MessageIDSort = string.IsNullOrEmpty(sortBy) ? "MessageID desc" : string.Empty;
            ViewBag.DSCRSort = sortBy == "DSCR" ? "DSCR desc" : "DSCR";
            ViewBag.LanguageSort = sortBy == "Language" ? "Language desc" : "Language";

            List<CommMessage> lstCommMessage = new List<CommMessage>();

            if (!string.IsNullOrEmpty(searchBy))
            {
                searchBy = searchBy.ToLower();
            }

            var CommMessage = (from t1 in db.CommMessages
                               join t2 in db.Languages on t1.Language equals t2.Code
                               select new { t1.MessageID, t1.DefaultMessage, t1.CustomMessage, t1.MessageLineLenght, t1.UseCustom, t1.Language, LanguageName = t2.DSCR, t1.MsgGroup, t1.MsgType, t1.DSCR, t1.CODE });

            foreach (var item in CommMessage)
            {
                CommMessage objCommMessage = new CommMessage();
                objCommMessage.MessageID = item.MessageID;
                objCommMessage.DefaultMessage = item.DefaultMessage;
                objCommMessage.CustomMessage = item.CustomMessage;
                objCommMessage.MessageLineLenght = item.MessageLineLenght;
                objCommMessage.UseCustom = item.UseCustom;
                objCommMessage.Language = item.Language;
                objCommMessage.LanguageName = item.LanguageName;
                objCommMessage.MsgGroup = item.MsgGroup;
                objCommMessage.MsgType = item.MsgType;
                objCommMessage.DSCR = item.DSCR;
                objCommMessage.CODE = item.CODE;

                lstCommMessage.Add(objCommMessage);
            }

            if (Session["searchBy"] != null && Session["Language"] != null)
            {
                searchBy = Session["searchBy"].ToString();
                Language = Convert.ToInt32(Session["Language"]);
                searchBy = searchBy.ToLower();
            }


            //search conditions
            if (!string.IsNullOrEmpty(searchBy))
            {
                lstCommMessage = lstCommMessage.Where(x => x.MsgGroup.ToLower().StartsWith(searchBy)).ToList();
            }
            else
            {
                lstCommMessage = lstCommMessage.Where(x => x.MsgGroup.ToLower().StartsWith("weblabel")).ToList();
            }

            if (Language != null)
            {
                lstCommMessage = lstCommMessage.Where(x => x.Language == Language).ToList();
            }
            else
            {
                lstCommMessage = lstCommMessage.Where(x => x.Language == 0).ToList();
            }

            //sorting
            switch (sortBy)
            {
                case "MessageID desc":
                    lstCommMessage = lstCommMessage.OrderByDescending(x => x.MessageID).ToList();
                    break;

                case "MessageID":
                    lstCommMessage = lstCommMessage.OrderBy(x => x.MessageID).ToList();
                    break;

                case "DSCR desc":
                    lstCommMessage = lstCommMessage.OrderByDescending(x => x.DSCR).ToList();
                    break;

                case "DSCR":
                    lstCommMessage = lstCommMessage.OrderBy(x => x.DSCR).ToList();
                    break;

                default:
                    lstCommMessage = lstCommMessage.OrderBy(x => x.MessageID).ToList();
                    break;
            }

            return View(lstCommMessage.ToPagedList(page ?? 1, pageSize));
        }

        // GET: CommMessages/Edit/5
        public ActionResult Edit(int? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageTranslations.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CommMessage commMessage = db.CommMessages.Find(id);

            if (commMessage == null)
            {
                return HttpNotFound();
            }

            Language objLanguage = db.Languages.Find(commMessage.Language);

            if (objLanguage != null)
            {
                commMessage.LanguageName = objLanguage.DSCR;
            }

            AllLanguage objAllLanguage = dbwmm.AllLanguages.ToList().FindAll(x => x.name.ToLower().Equals(commMessage.LanguageName.ToLower())).FirstOrDefault();

            if (objAllLanguage != null)
            {
                ViewBag.target = objAllLanguage.lang;
            }


            return View(commMessage);
        }

        // POST: CommMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MessageID,DefaultMessage,CustomMessage,MessageLineLenght,UseCustom_bool,Language,MsgGroup,MsgType,DSCR,CODE")] CommMessage commMessage, string Lang)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    commMessage.MsgType = string.Empty;

                    if (commMessage.DefaultMessage == null)
                    {
                        commMessage.DefaultMessage = string.Empty;
                    }

                    if (commMessage.CustomMessage == null)
                    {
                        commMessage.CustomMessage = string.Empty;
                    }

                    db.Entry(commMessage).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Translationmessage"] = "Translation updated successfully.";
                    TempData["class"] = "success-msg";
                }
                catch (Exception ex)
                {

                    TempData["Translationmessage"] = "Error occurred in updating a translation.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred while processing translation:", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }


                return RedirectToAction("Index");
            }

            return View(commMessage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Clear(int? id)
        {
            Session["searchBy"] = null;
            Session["Language"] = null;
            return RedirectToAction("Index");
        }
        #endregion
    }
}
