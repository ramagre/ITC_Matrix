/*
   FileFor : Managing the Defaults module.
   FileName : PrefsController.cs
   Created Date :28-1-2016
   Created By : Sandip Katore
   Modified Date :29-1-2016
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ITC_Matrix.Common;
using ITC_Matrix.Models;

namespace ITC_Matrix.Controllers
{
    public class PrefsController : Controller
    {
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        string moduleName = Common.CommonEnum.SubMenus.sys_defaults.ToString();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Index Get actionmethod for listing the dafaults record.
        public ActionResult Index()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageDefaults.ToString()) == false)
            {
                return View("NoAccess");
            }

            ViewBag.PrefsMessage = TempData["Prefsmessage"];
            ViewBag.Class = TempData["class"];
            BindDropdownlist();

            //getting themename from theme code
            var themeCode = dbwmn.prefs.Where(x => x.name == "skincss").Select(y => y.value).SingleOrDefault();

            if (themeCode != "0")
            {
                int code = Convert.ToInt32(themeCode);

                theme objTheme = dbwmn.themes.Find(code);
                string themeName = objTheme.themename;

                if (themeName != string.Empty)
                {
                    ViewBag.themeName = themeName;
                }
            }
            else
            {
                ViewBag.themeName = "Default Theme";
            }

            return View(dbwmn.prefs.ToList());
        }
        #endregion

        #region Index Post actionmethod for Udating the default records
        [HttpPost]
        public ActionResult Index([Bind(Include = "name,value")] pref prefs, string monthName)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageDefaults.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // if select the language Default
                    if (prefs.name == "langdefault")
                    {
                        var languageCode = dbwmn.AllLanguages.Where(x => x.name.ToLower().Equals(prefs.value)).Select(y => y.lang).SingleOrDefault();
                        if (languageCode != string.Empty)
                        {
                            prefs.value = languageCode;
                        }
                    }

                    // if select the Year to date starts 
                    if (prefs.name == "Year to date starts")
                    {
                        List<pref> lstPrefs = new List<pref>();
                        prefs.name = "ytd_day";
                        prefs.value = prefs.value;
                        dbwmn.Entry(prefs).State = EntityState.Modified;
                        dbwmn.SaveChanges();


                        pref objpref = new pref();

                        objpref.name = "ytd_month";
                        objpref.value = monthName;
                        dbwmn.Entry(objpref).State = EntityState.Modified;
                        dbwmn.SaveChanges();

                        // for adding the operator role name and id
                        TempData["Prefsmessage"] = "Default's updated successfully.";
                        TempData["class"] = "success-msg";
                        return RedirectToAction("Index");
                    }
                    var strName = prefs.name;

                    pref objprefs = dbwmn.prefs.Where(x => x.name == strName).SingleOrDefault();
                    objprefs.value = prefs.value;
                    objprefs.name = prefs.name;

                    dbwmn.Entry(objprefs).State = EntityState.Modified;
                    dbwmn.SaveChanges();

                    //dbwmn.Entry(prefs).State = EntityState.Added;
                    //dbwmn.SaveChanges();
                    // for adding the operator role name and id
                    TempData["Prefsmessage"] = "Default's updated successfully.";
                    TempData["class"] = "success-msg";

                    Common.Functions.SetDefaultTheme();

                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region user define function for binding the dropdown with values.
        private void BindDropdownlist()
        {
            ViewBag.themes = new SelectList(dbwmn.themes.ToList(), "id", "themename");
            ViewBag.languages = new SelectList(db.Languages.ToList(), "Code", "DSCR");

            var selectedDay = dbwmn.prefs.Where(x => x.name.ToLower().Equals("ytd_day")).Select(y => y.value).SingleOrDefault();
            ViewBag.selectedDay = selectedDay;
            var selectedMonth = dbwmn.prefs.Where(x => x.name.ToLower().Equals("ytd_month")).Select(y => y.value).SingleOrDefault();
            //getting month name on values
            var ex = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(selectedMonth));
            ViewBag.monthName = ex;

            //for displaying the month days
            var Month = new List<string>();
            for (int i = 1; i <= 31; i++)
            {
                Month.Add(i.ToString());
            }

            // for debug dropdown
            var selectedDebug = dbwmn.prefs.Where(x => x.name.ToLower().Equals("debug")).Select(y => y.value).SingleOrDefault();
            ViewBag.ddldebug = new SelectList(new[]
              {
                new { Text = "true", Value = "true" },
                new { Text = "false", Value = "false"}
            },
         "Value", "Text", selectedDebug);

            // for debug deepdebug
            var selecteddeepDebug = dbwmn.prefs.Where(x => x.name.ToLower().Equals("deepdebug")).Select(y => y.value).SingleOrDefault();
            ViewBag.ddldeepdebug = new SelectList(new[]
              {
                new { Text = "true", Value = "true" },
                new { Text = "false", Value = "false"}
            },
         "Value", "Text", selecteddeepDebug);


            // for debug ldapUse
            var selectedldapUse = dbwmn.prefs.Where(x => x.name.ToLower().Equals("ldapUse")).Select(y => y.value).SingleOrDefault();
            ViewBag.ddlldapUse = new SelectList(new[]
              {
                new { Text = "true", Value = "true" },
                new { Text = "false", Value = "false"}
            },
         "Value", "Text", selectedldapUse);

            // for debug timer
            var selectedtimer = dbwmn.prefs.Where(x => x.name.ToLower().Equals("timer")).Select(y => y.value).SingleOrDefault();
            ViewBag.ddlltimer = new SelectList(new[]
              {
                new { Text = "true", Value = "true" },
                new { Text = "false", Value = "false"}
            },
         "Value", "Text", selectedtimer);

        

            // for debug reportLogo
            var reportLogo = dbwmn.prefs.Where(x => x.name.ToLower().Equals("reportLogo")).Select(y => y.value).SingleOrDefault();
            ViewBag.ddlReportLogo = new SelectList(new[]
              {
                new { Text = "true", Value = "true" },
                new { Text = "false", Value = "false"}
            },
         "Value", "Text", reportLogo);

            // for debug FirstDay of week
            var firstDayofWeek = dbwmn.prefs.Where(x => x.name.ToLower().Equals("firstDayOfWeek")).Select(y => y.value).SingleOrDefault();
            ViewBag.ddlweek = new SelectList(new[]
              {
                new { Text = "Sunday", Value = "1"  },
                new { Text = "Monday", Value = "2"},
                new {Text = "Tuesday", Value = "3"},
                new {  Text = "Wednesday", Value = "4" },
                new { Text = "Thursday", Value = "5"},
                new { Text = "Friday", Value = "6"},
                new { Text = "Saturday", Value = "7"}
            },
         "Value", "Text", firstDayofWeek);


            ViewBag.selectedMonth = new SelectList(new[]
            {
                new { Text = "January", Value = "1" },
                new { Text = "February", Value = "2"  },
                new { Text = "March", Value = "3" },
                new {Text = "April", Value = "4"  },
                new { Text = "May", Value = "5" },
                new {Text = "June", Value = "6"  },
                new {Text = "July", Value = "7"},
                new {Text = "August", Value = "8"},
                new {Text = "September", Value = "9"},
                new {Text = "October", Value = "10" },
                new { Text = "November", Value = "11"  },
                new {Text = "December", Value = "12" },
            },
            "Value", "Text", selectedMonth);

            if (selectedDay != null)
            {
                ViewBag.Month = new SelectList(Month, selectedDay);
            }
            else
            {
                ViewBag.Month = new SelectList(Month);
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbwmn.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
