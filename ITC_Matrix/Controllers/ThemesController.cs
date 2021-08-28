/*
   FileFor : Managing themes
   FileName : ThemesController.cs
   Created Date : 12-01-2016
   Created By : Nahid Shaikh
   Modified Date :12-01-2016
*/

using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ITC_Matrix.Controllers
{
    public class ThemesController : Controller
    {
        #region ---------- Class Variables ----------

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        New_ITC_WMMEntities dbwmm = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.sys_themes.ToString();
        string separator = "#";
        string strsize = "px";
        
        #endregion

        #region ---------- Action Methods ----------

        // GET: Themes
        public ActionResult Index(int? searchBy, string search, string sortBy, int? page)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageThemes.ToString()) == false)
            {
                return View("NoAccess");
            }

            // message showing insert/update/delete status
            ViewBag.Message = TempData["message"];
            ViewBag.Class = TempData["class"];

            int pageSize = Functions.GetPageSize();

            //for sorting
            ViewBag.CodeSort = string.IsNullOrEmpty(sortBy) ? "Code desc" : string.Empty;
            ViewBag.NameSort = sortBy == "Name" ? "Name desc" : "Name";

            List<theme> lsttheme = dbwmm.themes.ToList();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
            }

            //search conditions
            if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(search))
            {
                lsttheme = lsttheme.Where(x => x.id.ToString().Contains(search)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Code && search != string.Empty)
            {
                lsttheme = lsttheme.Where(x => x.id.ToString().Contains(search)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Name && string.IsNullOrEmpty(search))
            {
                lsttheme = lsttheme.Where(x => x.themename.ToLower().StartsWith(search)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Name && search != string.Empty)
            {
                lsttheme = lsttheme.Where(x => x.themename.ToLower().StartsWith(search)).ToList();
            }
            else
            {
                lsttheme = lsttheme.ToList();
            }

            //sorting
            switch (sortBy)
            {
                case "Code desc":
                    lsttheme = lsttheme.OrderByDescending(x => x.id).ToList();
                    break;

                case "Code":
                    lsttheme = lsttheme.OrderBy(x => x.id).ToList();
                    break;

                case "Name desc":
                    lsttheme = lsttheme.OrderByDescending(x => x.themename).ToList();
                    break;

                case "Name":
                    lsttheme = lsttheme.OrderBy(x => x.themename).ToList();
                    break;

                default:
                    lsttheme = lsttheme.OrderBy(x => x.id).ToList();
                    break;
            }

            return View(lsttheme.ToPagedList(page ?? 1, pageSize));
        }

        // GET: Themes/Create
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageThemes.ToString()) == false)
            {
                return View("NoAccess");
            }

            #region ---------- Fill font styles ----------

            // Fill font families
            List<TblFont> lstFonts = dbwmm.TblFonts.ToList();

            // fill font size
            List<string> lstFontSize = new List<string>();

            for (int i = 10; i <= 30; i++)
            {
                lstFontSize.Add(i.ToString());
            }

            // fill font weight            
            List<string> lstFontWeight = new List<string>();

            foreach (CommonEnum.FontWeight fontWeight in Enum.GetValues(typeof(CommonEnum.FontWeight)))
            {
                string item = CommonEnum.StringEnum.GetStringValue(fontWeight).ToString();
                lstFontWeight.Add(item);
            }

            GetFontSettings(lstFonts, lstFontSize, lstFontWeight);

            #endregion

            return View();
        }

        // POST: Themes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "themename")] theme objtheme, FormCollection collection, HttpPostedFileBase SiteBackgroundImageUpload, HttpPostedFileBase SiteLogoImageUpload,
            HttpPostedFileBase ClientTileBackgroundImageUpload, HttpPostedFileBase AccountsTileBackgroundImageUpload, HttpPostedFileBase PrivilegesTileBackgroundImageUpload,
            HttpPostedFileBase DevicesTileBackgroundImageUpload, HttpPostedFileBase ReportsTileBackgroundImageUpload, HttpPostedFileBase RolesTileBackgroundImageUpload,
            HttpPostedFileBase AdministrationTileBackgroundImageUpload, HttpPostedFileBase ToolsTileBackgroundImageUpload, HttpPostedFileBase AppearanceTileBackgroundImageUpload)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageThemes.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    objtheme.status = 1;
                    objtheme.themename = objtheme.themename.Trim();

                    dbwmm.themes.Add(objtheme);
                    dbwmm.SaveChanges();

                    // Add folder and default file
                    ManageTheme(objtheme, collection, SiteBackgroundImageUpload, SiteLogoImageUpload, ClientTileBackgroundImageUpload, AccountsTileBackgroundImageUpload, PrivilegesTileBackgroundImageUpload, DevicesTileBackgroundImageUpload, ReportsTileBackgroundImageUpload, RolesTileBackgroundImageUpload, AdministrationTileBackgroundImageUpload, ToolsTileBackgroundImageUpload, AppearanceTileBackgroundImageUpload);

                    TempData["Themesmessage"] = "Theme added successfully.";
                    TempData["class"] = "success-msg";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Themesmessage"] = "Error occurred in adding theme.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred in adding theme:", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
            }

            return View(objtheme);
        }

        // GET: Themes/Edit
        public ActionResult Edit(int? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageThemes.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ThemeMessage = TempData["ThemeMessage"];
            ViewBag.ThemeClass = TempData["ThemeClass"];

            theme objtheme = dbwmm.themes.Find(id);

            TempData["themeName"] = objtheme.themename;

            if (objtheme == null)
            {
                return HttpNotFound();
            }

            PopulateTheme(id);
            
            return View(objtheme);
        }

        // POST: Themes/Edit/id 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,themename")] theme objtheme, FormCollection collection, HttpPostedFileBase SiteBackgroundImageUpload, HttpPostedFileBase SiteLogoImageUpload,
            HttpPostedFileBase ClientTileBackgroundImageUpload, HttpPostedFileBase AccountsTileBackgroundImageUpload, HttpPostedFileBase PrivilegesTileBackgroundImageUpload,
            HttpPostedFileBase DevicesTileBackgroundImageUpload, HttpPostedFileBase ReportsTileBackgroundImageUpload, HttpPostedFileBase RolesTileBackgroundImageUpload,
            HttpPostedFileBase AdministrationTileBackgroundImageUpload, HttpPostedFileBase ToolsTileBackgroundImageUpload, HttpPostedFileBase AppearanceTileBackgroundImageUpload)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageThemes.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    objtheme.themename = objtheme.themename.Trim();
                    dbwmm.Entry(objtheme).State = EntityState.Modified;
                    dbwmm.SaveChanges();

                    if (TempData["themeName"] != null)
                    {
                        string themeName = TempData["themeName"].ToString();

                        if (!themeName.ToLower().Equals(objtheme.themename.ToLower()))
                        {
                            string oldTheme = string.Concat(Server.MapPath(GlobalVariables.Themes), themeName);
                            string currentTheme = string.Concat(Server.MapPath(GlobalVariables.Themes), objtheme.themename);

                            System.IO.Directory.Move(oldTheme, currentTheme);
                        }
                    }

                    ManageTheme(objtheme, collection, SiteBackgroundImageUpload, SiteLogoImageUpload, ClientTileBackgroundImageUpload, AccountsTileBackgroundImageUpload, PrivilegesTileBackgroundImageUpload, DevicesTileBackgroundImageUpload, ReportsTileBackgroundImageUpload, RolesTileBackgroundImageUpload, AdministrationTileBackgroundImageUpload, ToolsTileBackgroundImageUpload, AppearanceTileBackgroundImageUpload);

                    TempData["ThemeMessage"] = "Theme updated successfully.";
                    TempData["ThemeClass"] = "success-msg";

                    // Check if the current theme is updated
                    if (dbwmm.prefs.ToList().FindAll(x => x.name.ToLower().Equals("skincss") && Convert.ToInt32(x.value) == objtheme.id).Count > 0)
                    {
                        Common.Functions.SetDefaultTheme();
                    }

                    return RedirectToAction("Edit", new { id = objtheme.id });
                }
                catch (Exception ex)
                {
                    TempData["Themesmessage"] = "Error occurred in updating a theme.";
                    ViewBag.MessageClass = "error-msg";
                    logger.Error("Error occurred in updating a theme:", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
            }

            return View(objtheme);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmm.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageThemes.ToString()) == false)
            {
                return View("NoAccess");
            }

            bool isValid = true;
            pref objpref;
            objpref = dbwmm.prefs.Where(x => x.name.Trim().ToLower().Equals("skincss")).FirstOrDefault();

            if (Convert.ToInt32(objpref.value) == id)
            {
                isValid = false;
            }

            if (isValid == false)
            {
                TempData["Themesmessage"] = "This theme can not be deleted as it is assigned as default theme.";
                TempData["class"] = "error-msg";

                return Json(new { Success = false });
            }
            else
            {
                try
                {
                    theme objtheme = dbwmm.themes.Find(id);

                    // first delete theme settings
                    DeleteTheme(objtheme);

                    dbwmm.themes.Remove(objtheme);
                    dbwmm.SaveChanges();

                    // delete css folder while deleting theme
                    string themeFilePath = string.Concat(Server.MapPath(GlobalVariables.Themes), objtheme.themename);

                    if (Directory.Exists(themeFilePath))
                    {
                        Directory.Delete(themeFilePath, true);
                    }

                    TempData["Themesmessage"] = "Theme deleted successfully.";
                    TempData["class"] = "success-msg";

                    return Json(new { Success = true });
                }
                catch (Exception ex)
                {
                    TempData["Themesmessage"] = "Error occurred in deleting theme.";
                    TempData["class"] = "error-msg";
                    logger.Error("Error occurred in deleting theme:", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
            }
        }
        
        public ActionResult Preview(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                int themeid = Common.Functions.ParseInteger(id);

                theme objtheme = dbwmm.themes.ToList().FindAll(x => x.id == themeid).FirstOrDefault();
                
                if(objtheme!=null)
                {
                   ViewBag.Preview = string.Concat(GlobalVariables.ThemeFolder, objtheme.themename, "/", GlobalVariables.defaultThemeFile);
                }
            }                      

            return View();
        }

        #endregion

        #region ---------- User Defined Functions ----------

        /// <summary>
        /// Check if the theme already exists
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        public JsonResult IsExistTheme(string themeName, int id = 0)
        {
            var result = true;
            theme objtheme;

            if (id == 0)        // check while adding
            {
                objtheme = dbwmm.themes.Where(x => x.themename.Trim().ToLower().Equals(themeName.Trim().ToLower())).FirstOrDefault();
            }
            else               // check while editing
            {
                objtheme = dbwmm.themes.Where(x => x.themename.Trim().ToLower().Equals(themeName.Trim().ToLower()) && x.id != id).FirstOrDefault();
            }

            if (objtheme != null)
            {
                result = false;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Manage Theme
        /// </summary>
        /// <param name="objtheme"></param>
        /// <param name="collection"></param>
        /// <param name="SiteBackgroundImageUpload"></param>
        /// <param name="SiteLogoImageUpload"></param>
        /// <param name="ClientTileBackgroundImageUpload"></param>
        /// <param name="AccountsTileBackgroundImageUpload"></param>
        /// <param name="PrivilegesTileBackgroundImageUpload"></param>
        /// <param name="DevicesTileBackgroundImageUpload"></param>
        /// <param name="ReportsTileBackgroundImageUpload"></param>
        /// <param name="RolesTileBackgroundImageUpload"></param>
        /// <param name="AdministrationTileBackgroundImageUpload"></param>
        /// <param name="ToolsTileBackgroundImageUpload"></param>
        /// <param name="AppearanceTileBackgroundImageUpload"></param>
        private void ManageTheme(theme objtheme, FormCollection collection, HttpPostedFileBase SiteBackgroundImageUpload, HttpPostedFileBase SiteLogoImageUpload, 
            HttpPostedFileBase ClientTileBackgroundImageUpload, HttpPostedFileBase AccountsTileBackgroundImageUpload, HttpPostedFileBase PrivilegesTileBackgroundImageUpload,
            HttpPostedFileBase DevicesTileBackgroundImageUpload, HttpPostedFileBase ReportsTileBackgroundImageUpload, HttpPostedFileBase RolesTileBackgroundImageUpload,
            HttpPostedFileBase AdministrationTileBackgroundImageUpload, HttpPostedFileBase ToolsTileBackgroundImageUpload, HttpPostedFileBase AppearanceTileBackgroundImageUpload)
        {
            string themeFilePath = string.Concat(Server.MapPath(GlobalVariables.Themes), objtheme.themename);
            string fileName = GlobalVariables.defaultThemeFile;
            string sourcePath = Server.MapPath(GlobalVariables.defaultCSSPath);
            string targetPath = themeFilePath;

            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // check and create folder
            if (!Directory.Exists(themeFilePath))
            {
                Directory.CreateDirectory(themeFilePath);
            }

            // check and copy file
            if (!System.IO.File.Exists(destFile))
            {
                System.IO.File.Copy(sourceFile, destFile, true);
            }
            else
            {
                System.IO.File.Delete(destFile);
                System.IO.File.Copy(sourceFile, destFile, true);
            }

            SetThemeSetting(objtheme, sourcePath, destFile, themeFilePath, collection, SiteBackgroundImageUpload, SiteLogoImageUpload, ClientTileBackgroundImageUpload, AccountsTileBackgroundImageUpload, PrivilegesTileBackgroundImageUpload, DevicesTileBackgroundImageUpload, ReportsTileBackgroundImageUpload, RolesTileBackgroundImageUpload, AdministrationTileBackgroundImageUpload, ToolsTileBackgroundImageUpload, AppearanceTileBackgroundImageUpload);
        }

        /// <summary>
        /// Set Theme Setting
        /// </summary>
        /// <param name="objtheme"></param>
        /// <param name="sourcePath"></param>
        /// <param name="destFile"></param>
        /// <param name="themeFilePath"></param>
        /// <param name="collection"></param>
        /// <param name="SiteBackgroundImageUpload"></param>
        /// <param name="SiteLogoImageUpload"></param>
        /// <param name="ClientTileBackgroundImageUpload"></param>
        /// <param name="AccountsTileBackgroundImageUpload"></param>
        /// <param name="PrivilegesTileBackgroundImageUpload"></param>
        /// <param name="DevicesTileBackgroundImageUpload"></param>
        /// <param name="ReportsTileBackgroundImageUpload"></param>
        /// <param name="RolesTileBackgroundImageUpload"></param>
        /// <param name="AdministrationTileBackgroundImageUpload"></param>
        /// <param name="ToolsTileBackgroundImageUpload"></param>
        /// <param name="AppearanceTileBackgroundImageUpload"></param>
        private void SetThemeSetting(theme objtheme, string sourcePath, string destFile, string themeFilePath, FormCollection collection, HttpPostedFileBase SiteBackgroundImageUpload, HttpPostedFileBase SiteLogoImageUpload,
            HttpPostedFileBase ClientTileBackgroundImageUpload, HttpPostedFileBase AccountsTileBackgroundImageUpload, HttpPostedFileBase PrivilegesTileBackgroundImageUpload,
            HttpPostedFileBase DevicesTileBackgroundImageUpload, HttpPostedFileBase ReportsTileBackgroundImageUpload, HttpPostedFileBase RolesTileBackgroundImageUpload,
            HttpPostedFileBase AdministrationTileBackgroundImageUpload, HttpPostedFileBase ToolsTileBackgroundImageUpload, HttpPostedFileBase AppearanceTileBackgroundImageUpload)
        {
            List<ThemeSetting> lstThemeSetting = new List<ThemeSetting>();
            string themeSettingName = string.Empty;
            string themeSettingValue = string.Empty;
            string themeSettingDBValue = string.Empty;
            Enum enumValue = null;

            string templateFile = GlobalVariables.templateFile;
            string imagePath = string.Concat(GlobalVariables.ThemeFolder, objtheme.themename, "/");
            string defaultImagePath = GlobalVariables.defaultImagePath;

            // Get template file and replace placeholders
            string templateFilePath = System.IO.Path.Combine(sourcePath, templateFile);
            string themeCss = Common.Functions.ReadFile(templateFilePath);

            if (collection != null)
            {
                foreach (var key in collection.AllKeys)
                {
                    HttpPostedFileBase upload = null;
                    imagePath = string.Concat(GlobalVariables.ThemeFolder, objtheme.themename, "/");
                    themeSettingName = string.Empty;
                    themeSettingValue = string.Empty;
                    themeSettingDBValue = string.Empty;
                    enumValue = null;

                    #region ---------- Background Color ----------

                    if (key.ToLower().Equals(CommonEnum.ThemeSetting.SiteBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.MenuBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.GridHeaderBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.GridFooterBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.GridRowBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.GridRowAlternateBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.ClientTileBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.AccountsTileBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.PrivilegesTileBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.DevicesTileBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.ReportsTileBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.RolesTileBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.AdministrationTileBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.ToolsTileBackgroundColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.AppearanceTileBackgroundColor.ToString().ToLower()))
                    {
                        if (key.ToLower().Equals(CommonEnum.ThemeSetting.SiteBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.SiteBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.SiteBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.HeaderBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.HeaderBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.MenuBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.MenuBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.MenuBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.SubmenuBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.SubmenuBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.GridHeaderBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.GridHeaderBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.GridHeaderBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.GridFooterBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.GridFooterBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.GridFooterBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.GridRowBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.GridRowBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.GridRowBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.GridRowAlternateBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.GridRowAlternateBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.GridRowAlternateBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.ClientTileBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.ClientTileBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.ClientTileBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.AccountsTileBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.AccountsTileBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.AccountsTileBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.PrivilegesTileBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.PrivilegesTileBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.PrivilegesTileBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.DevicesTileBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.DevicesTileBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.DevicesTileBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.ReportsTileBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.ReportsTileBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.ReportsTileBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.RolesTileBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.RolesTileBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.RolesTileBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.AdministrationTileBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.AdministrationTileBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.AdministrationTileBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.ToolsTileBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.ToolsTileBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.ToolsTileBackgroundColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.AppearanceTileBackgroundColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.AppearanceTileBackgroundColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.AppearanceTileBackgroundColor;
                        }

                        if (!string.IsNullOrEmpty(collection[themeSettingName]))
                        {
                            themeSettingValue = string.Concat("background:", collection[themeSettingName], ";");
                            themeSettingDBValue = collection[themeSettingName];
                        }
                        else
                        {
                            themeSettingValue = string.Concat("background:", CommonEnum.StringEnum.GetStringValue(enumValue), ";");
                            themeSettingDBValue = CommonEnum.StringEnum.GetStringValue(enumValue);
                        }
                    }

                    #endregion

                    #region ---------- Site Background Image ----------

                    else if (key.ToLower().Equals(CommonEnum.ThemeSetting.SiteBackgroundImage.ToString().ToLower()))
                    {
                        themeSettingName = CommonEnum.ThemeSetting.SiteBackgroundImage.ToString();
                        enumValue = CommonEnum.ThemeSetting.SiteBackgroundImage;

                        if (!string.IsNullOrEmpty(collection[themeSettingName]))
                        {
                            string imageName = string.Empty;

                            if (SiteBackgroundImageUpload != null)     // if the file is selected to upload
                            {
                                imageName = Path.GetFileName(SiteBackgroundImageUpload.FileName);

                                if (CheckValidImage(imageName))
                                {
                                    SiteBackgroundImageUpload.SaveAs(Path.Combine(themeFilePath, imageName));
                                }
                                else
                                {
                                    imageName = CommonEnum.StringEnum.GetStringValue(enumValue);
                                    imagePath = string.Empty;
                                }
                            }
                            else
                            {
                                imageName = collection[themeSettingName];

                                if (!System.IO.File.Exists(Path.Combine(themeFilePath, imageName)))
                                {
                                    imagePath = defaultImagePath;
                                }
                            }

                            themeSettingDBValue = string.Concat(imagePath, imageName);
                            themeSettingValue = string.Concat("background:url(", themeSettingDBValue, ") no-repeat top center;");
                        }
                        else
                        {
                            themeSettingDBValue = CommonEnum.StringEnum.GetStringValue(enumValue);
                            themeSettingValue = string.Concat("background:url(", themeSettingDBValue, ") no-repeat top center;");
                        }
                    }

                    #endregion

                    #region ---------- Site Logo Image ----------

                    else if (key.ToLower().Equals(CommonEnum.ThemeSetting.SiteLogo.ToString().ToLower()))
                    {
                        themeSettingName = CommonEnum.ThemeSetting.SiteLogo.ToString();
                        enumValue = CommonEnum.ThemeSetting.SiteLogo;

                        if (!string.IsNullOrEmpty(collection[themeSettingName]))
                        {
                            string imageName = string.Empty;

                            if (SiteLogoImageUpload != null)     // if the file is selected to upload
                            {
                                imageName = Path.GetFileName(SiteLogoImageUpload.FileName);

                                if (CheckValidImage(imageName))
                                {
                                    SiteLogoImageUpload.SaveAs(Path.Combine(themeFilePath, imageName));
                                }
                                else
                                {
                                    imageName = CommonEnum.StringEnum.GetStringValue(enumValue);
                                    imagePath = string.Empty;
                                }
                            }
                            else
                            {
                                imageName = collection[themeSettingName];

                                if (!System.IO.File.Exists(Path.Combine(themeFilePath, imageName)))
                                {
                                    imagePath = defaultImagePath;
                                }
                            }

                            themeSettingDBValue = string.Concat(imagePath, imageName);
                            themeSettingValue = string.Concat("background:url(", themeSettingDBValue, ") no-repeat 0px 0;");
                        }
                        else
                        {
                            themeSettingDBValue = CommonEnum.StringEnum.GetStringValue(enumValue);
                            themeSettingValue = string.Concat("background:url(", themeSettingDBValue, ") no-repeat 0px 0;");
                        }
                    }

                    #endregion

                    #region ---------- Font Color ----------

                    else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormLableFontStyleFontColor.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontColor.ToString().ToLower()))
                    {
                        if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormControlFontColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormControlFontColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.HeaderFontStyleFontColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.HeaderFontStyleFontColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.MenuFontStyleFontColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.MenuFontStyleFontColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.SubmenuFontStyleFontColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.SubmenuFontStyleFontColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormHeaderFontStyleFontColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormHeaderFontStyleFontColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormLableFontStyleFontColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormLableFontStyleFontColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormLableFontStyleFontColor;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormControlFontStyleFontColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormControlFontStyleFontColor;
                        }

                        if (!string.IsNullOrEmpty(collection[themeSettingName]))
                        {
                            themeSettingValue = string.Concat("color:", collection[themeSettingName], ";");
                            themeSettingDBValue = collection[themeSettingName];
                        }
                        else
                        {
                            themeSettingValue = string.Concat("color:", CommonEnum.StringEnum.GetStringValue(enumValue), ";");
                            themeSettingDBValue = CommonEnum.StringEnum.GetStringValue(enumValue);
                        }
                    }

                    #endregion

                    #region ---------- Border Color ----------

                    else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlBorderColor.ToString().ToLower()))
                    {
                        if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlBorderColor.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormControlBorderColor.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormControlBorderColor;
                        }

                        if (!string.IsNullOrEmpty(collection[themeSettingName]))
                        {
                            themeSettingValue = string.Concat("border-color:", collection[themeSettingName], ";");
                            themeSettingDBValue = collection[themeSettingName];
                        }
                        else
                        {
                            themeSettingValue = string.Concat("border-color:", CommonEnum.StringEnum.GetStringValue(enumValue), ";");
                            themeSettingDBValue = CommonEnum.StringEnum.GetStringValue(enumValue);
                        }
                    }

                    #endregion

                    #region ---------- Font Size ----------

                    else if (key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontSize.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontSize.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontSize.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontSize.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontSize.ToString().ToLower()))
                    {
                        if (key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontSize.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.HeaderFontStyleFontSize.ToString();
                            enumValue = CommonEnum.ThemeSetting.HeaderFontStyleFontSize;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontSize.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.MenuFontStyleFontSize.ToString();
                            enumValue = CommonEnum.ThemeSetting.MenuFontStyleFontSize;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontSize.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.SubmenuFontStyleFontSize.ToString();
                            enumValue = CommonEnum.ThemeSetting.SubmenuFontStyleFontSize;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontSize.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormHeaderFontStyleFontSize.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormHeaderFontStyleFontSize;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontSize.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormControlFontStyleFontSize.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormControlFontStyleFontSize;
                        }

                        if (!string.IsNullOrEmpty(collection[themeSettingName]))
                        {
                            themeSettingValue = string.Concat("font-size:", collection[themeSettingName], "px;");
                            themeSettingDBValue = collection[themeSettingName];
                        }
                        else
                        {
                            themeSettingValue = string.Concat("font-size:", CommonEnum.StringEnum.GetStringValue(enumValue), "px;");
                            themeSettingDBValue = CommonEnum.StringEnum.GetStringValue(enumValue);
                        }
                    }

                    #endregion

                    #region ---------- Font Weight ----------

                    else if (key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontWight.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontWight.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontWight.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontWight.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormLableFontStyleFontWight.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontWight.ToString().ToLower()))
                    {
                        if (key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontWight.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.HeaderFontStyleFontWight.ToString();
                            enumValue = CommonEnum.ThemeSetting.HeaderFontStyleFontWight;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontWight.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.MenuFontStyleFontWight.ToString();
                            enumValue = CommonEnum.ThemeSetting.MenuFontStyleFontWight;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontWight.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.SubmenuFontStyleFontWight.ToString();
                            enumValue = CommonEnum.ThemeSetting.SubmenuFontStyleFontWight;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontWight.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormHeaderFontStyleFontWight.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormHeaderFontStyleFontWight;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormLableFontStyleFontWight.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormLableFontStyleFontWight.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormLableFontStyleFontWight;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontWight.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormControlFontStyleFontWight.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormControlFontStyleFontWight;
                        }

                        if (!string.IsNullOrEmpty(collection[themeSettingName]))
                        {
                            themeSettingValue = string.Concat("font-weight:", collection[themeSettingName], ";");
                            themeSettingDBValue = collection[themeSettingName];
                        }
                        else
                        {
                            themeSettingValue = string.Concat("font-weight:", CommonEnum.StringEnum.GetStringValue(enumValue), ";");
                            themeSettingDBValue = CommonEnum.StringEnum.GetStringValue(enumValue);
                        }
                    }

                    #endregion

                    #region ---------- Font Family ----------

                    else if (key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontFamily.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontFamily.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontFamily.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontFamily.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormLableFontStyleFontFamily.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontFamily.ToString().ToLower()))
                    {
                        if (key.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontFamily.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.HeaderFontStyleFontFamily.ToString();
                            enumValue = CommonEnum.ThemeSetting.HeaderFontStyleFontFamily;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontFamily.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.MenuFontStyleFontFamily.ToString();
                            enumValue = CommonEnum.ThemeSetting.MenuFontStyleFontFamily;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontFamily.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.SubmenuFontStyleFontFamily.ToString();
                            enumValue = CommonEnum.ThemeSetting.SubmenuFontStyleFontFamily;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontFamily.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormHeaderFontStyleFontFamily.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormHeaderFontStyleFontFamily;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormLableFontStyleFontFamily.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormLableFontStyleFontFamily.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormLableFontStyleFontFamily;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontFamily.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.FormControlFontStyleFontFamily.ToString();
                            enumValue = CommonEnum.ThemeSetting.FormControlFontStyleFontFamily;
                        }

                        if (!string.IsNullOrEmpty(collection[themeSettingName]))
                        {
                            themeSettingValue = string.Concat("font-family:", collection[themeSettingName], ";");
                            themeSettingDBValue = collection[themeSettingName];
                        }
                        else
                        {
                            themeSettingValue = string.Concat("font-family:", CommonEnum.StringEnum.GetStringValue(enumValue), ";");
                            themeSettingDBValue = CommonEnum.StringEnum.GetStringValue(enumValue);
                        }
                    }

                    #endregion                    

                    #region ---------- Tile Background Image ----------

                    else if (key.ToLower().Equals(CommonEnum.ThemeSetting.ClientTileBackgroundImage.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.AccountsTileBackgroundImage.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.PrivilegesTileBackgroundImage.ToString().ToLower())||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.DevicesTileBackgroundImage.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.ReportsTileBackgroundImage.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.RolesTileBackgroundImage.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.AdministrationTileBackgroundImage.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.ToolsTileBackgroundImage.ToString().ToLower()) ||
                        key.ToLower().Equals(CommonEnum.ThemeSetting.AppearanceTileBackgroundImage.ToString().ToLower()))
                    {
                        if (key.ToLower().Equals(CommonEnum.ThemeSetting.ClientTileBackgroundImage.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.ClientTileBackgroundImage.ToString();
                            enumValue = CommonEnum.ThemeSetting.ClientTileBackgroundImage;
                            upload = ClientTileBackgroundImageUpload;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.AccountsTileBackgroundImage.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.AccountsTileBackgroundImage.ToString();
                            enumValue = CommonEnum.ThemeSetting.AccountsTileBackgroundImage;
                            upload = AccountsTileBackgroundImageUpload;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.PrivilegesTileBackgroundImage.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.PrivilegesTileBackgroundImage.ToString();
                            enumValue = CommonEnum.ThemeSetting.PrivilegesTileBackgroundImage;
                            upload = PrivilegesTileBackgroundImageUpload;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.DevicesTileBackgroundImage.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.DevicesTileBackgroundImage.ToString();
                            enumValue = CommonEnum.ThemeSetting.DevicesTileBackgroundImage;
                            upload = DevicesTileBackgroundImageUpload;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.ReportsTileBackgroundImage.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.ReportsTileBackgroundImage.ToString();
                            enumValue = CommonEnum.ThemeSetting.ReportsTileBackgroundImage;
                            upload = ReportsTileBackgroundImageUpload;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.RolesTileBackgroundImage.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.RolesTileBackgroundImage.ToString();
                            enumValue = CommonEnum.ThemeSetting.RolesTileBackgroundImage;
                            upload = RolesTileBackgroundImageUpload;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.AdministrationTileBackgroundImage.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.AdministrationTileBackgroundImage.ToString();
                            enumValue = CommonEnum.ThemeSetting.AdministrationTileBackgroundImage;
                            upload = AdministrationTileBackgroundImageUpload;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.ToolsTileBackgroundImage.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.ToolsTileBackgroundImage.ToString();
                            enumValue = CommonEnum.ThemeSetting.ToolsTileBackgroundImage;
                            upload = ToolsTileBackgroundImageUpload;
                        }
                        else if (key.ToLower().Equals(CommonEnum.ThemeSetting.AppearanceTileBackgroundImage.ToString().ToLower()))
                        {
                            themeSettingName = CommonEnum.ThemeSetting.AppearanceTileBackgroundImage.ToString();
                            enumValue = CommonEnum.ThemeSetting.AppearanceTileBackgroundImage;
                            upload = AppearanceTileBackgroundImageUpload;
                        }

                        if (!string.IsNullOrEmpty(collection[themeSettingName]))
                        {
                            string imageName = string.Empty;

                            if (upload != null)     // if the file is selected to upload
                            {
                                imageName = Path.GetFileName(upload.FileName);

                                if (CheckValidImage(imageName))
                                {
                                    upload.SaveAs(Path.Combine(themeFilePath, imageName));
                                }
                                else
                                {
                                    imageName = CommonEnum.StringEnum.GetStringValue(enumValue);
                                    imagePath = string.Empty;
                                }
                            }
                            else
                            {
                                imageName = collection[themeSettingName];

                                if (!System.IO.File.Exists(Path.Combine(themeFilePath, imageName)))
                                {
                                    imagePath = defaultImagePath;
                                }
                            }

                            themeSettingDBValue = string.Concat(imagePath, imageName);
                            themeSettingValue = string.Concat("background:url(", themeSettingDBValue, ") repeat-x 0 0;");
                        }
                        else
                        {
                            themeSettingDBValue = CommonEnum.StringEnum.GetStringValue(enumValue);
                            themeSettingValue = string.Concat("background:url(", themeSettingDBValue, ") repeat-x 0 0;");
                        }
                    }

                    #endregion                 

                    if (!string.IsNullOrEmpty(themeSettingName) && !string.IsNullOrEmpty(themeSettingValue))
                    {
                        themeCss = themeCss.Replace(string.Concat(separator, themeSettingName, separator), themeSettingValue);

                        // add in list to update database
                        lstThemeSetting = AddSetting(lstThemeSetting, themeSettingName, themeSettingDBValue);
                    }
                }

                AddTheme(objtheme, lstThemeSetting);
            }

            System.IO.File.WriteAllText(destFile, themeCss);
        }

        /// <summary>
        /// Check Valid Image
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        private bool CheckValidImage(string imageName)
        {
            bool isValid = false;

            string extension = Path.GetExtension(imageName).ToLower();

            if (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png") || extension.Equals(".gif"))
            {
                isValid = true;
            }

            return isValid;
        }

        /// <summary>
        /// Add Setting
        /// </summary>
        /// <param name="lstThemeSetting"></param>
        /// <param name="themeSettingName"></param>
        /// <param name="themeSettingDBValue"></param>
        /// <returns></returns>
        private List<ThemeSetting> AddSetting(List<ThemeSetting> lstThemeSetting, string themeSettingName, string themeSettingDBValue)
        {
            ThemeSetting objThemeSetting = new ThemeSetting();
            objThemeSetting.ThemeSettingName = themeSettingName;
            objThemeSetting.ThemeSettingValue = themeSettingDBValue;
            objThemeSetting.Placeholder = themeSettingName;

            lstThemeSetting.Add(objThemeSetting);

            return lstThemeSetting;
        }

        /// <summary>
        /// Add Theme
        /// </summary>
        /// <param name="objtheme"></param>
        /// <param name="lstThemeSettings"></param>
        private void AddTheme(theme objtheme, List<ThemeSetting> lstThemeSettings)
        {
            if (lstThemeSettings.Count > 0)
            {
                //delete settings
                DeleteTheme(objtheme);
                
                // bulk insert theme settings
                lstThemeSettings.ForEach(x => x.ThemeId = objtheme.id);
                dbwmm.ThemeSettings.AddRange(lstThemeSettings);
                dbwmm.SaveChanges();
            }
        }

        /// <summary>
        /// Delete Theme
        /// </summary>
        /// <param name="objtheme"></param>
        private void DeleteTheme(theme objtheme)
        {
            List<ThemeSetting> lstDelete = dbwmm.ThemeSettings.ToList().FindAll(i => i.ThemeId == objtheme.id);
            
            dbwmm.ThemeSettings.RemoveRange(dbwmm.ThemeSettings.Where(x => x.ThemeId == objtheme.id));
            dbwmm.SaveChanges();
        }

        /// <summary>
        /// Set Theme
        /// </summary>
        /// <returns></returns>
        public ActionResult SetTheme()
        {
            return View();
        }

        /// <summary>
        /// Populate Theme
        /// </summary>
        /// <param name="id"></param>
        private void PopulateTheme(int? id)
        {
            List<ThemeSetting> lstThemeSetting = new List<ThemeSetting>();
            int startIndex = 0;
            int length = 0;
            
            lstThemeSetting = dbwmm.ThemeSettings.ToList().FindAll(x => x.ThemeId == id);

            #region ---------- Fill font styles ----------

            // Fill font families
            List<TblFont> lstFonts = dbwmm.TblFonts.ToList();
            
            // fill font size
            List<string> lstFontSize = new List<string>();

            for (int i = 10; i <= 30; i ++)
            {
                lstFontSize.Add(i.ToString());
            }
            
            // fill font weight            
            List<string> lstFontWeight = new List<string>();

            foreach (CommonEnum.FontWeight fontWeight in Enum.GetValues(typeof(CommonEnum.FontWeight)))
            {
                string item = CommonEnum.StringEnum.GetStringValue(fontWeight).ToString();
                lstFontWeight.Add(item);
            }

            GetFontSettings(lstFonts, lstFontSize, lstFontWeight);
           
            #endregion

            #region ---------- Populate data ----------

            for (int i = 0; i < lstThemeSetting.Count; i++)
            {
                startIndex = lstThemeSetting[i].ThemeSettingValue.LastIndexOf("/") + 1;
                length = lstThemeSetting[i].ThemeSettingValue.Length;

                if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.SiteBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.SiteBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.SiteBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.SiteBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.SiteLogo.ToString().ToLower()))
                {
                    ViewBag.SiteLogo = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.HeaderBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.HeaderBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.MenuBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.MenuBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.SubmenuBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontColor.ToString().ToLower()))
                {
                    ViewBag.FormControlFontColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormControlBorderColor.ToString().ToLower()))
                {
                    ViewBag.FormControlBorderColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.GridHeaderBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.GridHeaderBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.GridFooterBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.GridFooterBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.GridRowBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.GridRowBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.GridRowAlternateBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.GridRowAlternateBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontColor.ToString().ToLower()))
                {
                    ViewBag.HeaderFontStyleFontColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontFamily.ToString().ToLower()))
                {
                    ViewBag.HeaderFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontSize.ToString().ToLower()))
                {
                    ViewBag.HeaderFontStyleFontSize = new SelectList(lstFontSize, lstThemeSetting[i].ThemeSettingValue.Replace(strsize, string.Empty));                    
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.HeaderFontStyleFontWight.ToString().ToLower()))
                {
                    ViewBag.HeaderFontStyleFontWight = new SelectList(lstFontWeight, lstThemeSetting[i].ThemeSettingValue);                   
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontColor.ToString().ToLower()))
                {
                    ViewBag.MenuFontStyleFontColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontFamily.ToString().ToLower()))
                {
                    ViewBag.MenuFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontSize.ToString().ToLower()))
                {
                    ViewBag.MenuFontStyleFontSize = new SelectList(lstFontSize, lstThemeSetting[i].ThemeSettingValue.Replace(strsize, string.Empty));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.MenuFontStyleFontWight.ToString().ToLower()))
                {
                    ViewBag.MenuFontStyleFontWight = new SelectList(lstFontWeight, lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontColor.ToString().ToLower()))
                {
                    ViewBag.SubmenuFontStyleFontColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontFamily.ToString().ToLower()))
                {
                    ViewBag.SubmenuFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontSize.ToString().ToLower()))
                {
                    ViewBag.SubmenuFontStyleFontSize = new SelectList(lstFontSize, lstThemeSetting[i].ThemeSettingValue.Replace(strsize, string.Empty));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.SubmenuFontStyleFontWight.ToString().ToLower()))
                {
                    ViewBag.SubmenuFontStyleFontWight = new SelectList(lstFontWeight, lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontColor.ToString().ToLower()))
                {
                    ViewBag.FormHeaderFontStyleFontColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontFamily.ToString().ToLower()))
                {
                    ViewBag.FormHeaderFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontSize.ToString().ToLower()))
                {
                    ViewBag.SubmenuFontStyleFontSize = new SelectList(lstFontSize, lstThemeSetting[i].ThemeSettingValue.Replace(strsize, string.Empty));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormHeaderFontStyleFontWight.ToString().ToLower()))
                {
                    ViewBag.FormHeaderFontStyleFontWight = new SelectList(lstFontWeight, lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormLableFontStyleFontColor.ToString().ToLower()))
                {
                    ViewBag.FormLableFontStyleFontColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormLableFontStyleFontFamily.ToString().ToLower()))
                {
                    ViewBag.FormLableFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", lstThemeSetting[i].ThemeSettingValue);
                }               
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormLableFontStyleFontWight.ToString().ToLower()))
                {
                    ViewBag.FormLableFontStyleFontWight = new SelectList(lstFontWeight, lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontColor.ToString().ToLower()))
                {
                    ViewBag.FormControlFontStyleFontColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontFamily.ToString().ToLower()))
                {
                    ViewBag.FormControlFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontSize.ToString().ToLower()))
                {
                    ViewBag.FormControlFontStyleFontSize = new SelectList(lstFontSize, lstThemeSetting[i].ThemeSettingValue.Replace(strsize, string.Empty));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.FormControlFontStyleFontWight.ToString().ToLower()))
                {
                    ViewBag.FormControlFontStyleFontWight = new SelectList(lstFontWeight, lstThemeSetting[i].ThemeSettingValue);
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.ClientTileBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.ClientTileBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.AccountsTileBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.AccountsTileBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.PrivilegesTileBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.PrivilegesTileBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.DevicesTileBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.DevicesTileBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.ReportsTileBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.ReportsTileBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.RolesTileBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.RolesTileBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.AdministrationTileBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.AdministrationTileBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.ToolsTileBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.ToolsTileBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.AppearanceTileBackgroundColor.ToString().ToLower()))
                {
                    ViewBag.AppearanceTileBackgroundColor = lstThemeSetting[i].ThemeSettingValue;
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.ClientTileBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.ClientTileBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.AccountsTileBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.AccountsTileBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.PrivilegesTileBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.PrivilegesTileBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.DevicesTileBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.DevicesTileBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.ReportsTileBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.ReportsTileBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.RolesTileBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.RolesTileBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.AdministrationTileBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.AdministrationTileBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.ToolsTileBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.ToolsTileBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
                else if (lstThemeSetting[i].ThemeSettingName.ToLower().Equals(CommonEnum.ThemeSetting.AppearanceTileBackgroundImage.ToString().ToLower()))
                {
                    ViewBag.AppearanceTileBackgroundImage = lstThemeSetting[i].ThemeSettingValue.Substring(startIndex, (length - startIndex));
                }
            }           

            #endregion
        }

        /// <summary>
        /// Get Font Settings
        /// </summary>
        /// <param name="lstFonts"></param>
        /// <param name="lstFontSize"></param>
        /// <param name="lstFontWeight"></param>
        private void GetFontSettings(List<TblFont> lstFonts, List<string> lstFontSize, List<string> lstFontWeight)
        {
            ViewBag.HeaderFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.HeaderFontStyleFontFamily));
            ViewBag.HeaderFontStyleFontSize = new SelectList(lstFontSize, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.HeaderFontStyleFontSize).Replace(strsize, string.Empty));
            ViewBag.HeaderFontStyleFontWight = new SelectList(lstFontWeight, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.HeaderFontStyleFontWight));

            ViewBag.MenuFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.MenuFontStyleFontFamily));
            ViewBag.MenuFontStyleFontSize = new SelectList(lstFontSize, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.MenuFontStyleFontSize).Replace(strsize, string.Empty));
            ViewBag.MenuFontStyleFontWight = new SelectList(lstFontWeight, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.MenuFontStyleFontWight));

            ViewBag.SubmenuFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.SubmenuFontStyleFontFamily));
            ViewBag.SubmenuFontStyleFontSize = new SelectList(lstFontSize, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.SubmenuFontStyleFontSize).Replace(strsize, string.Empty));
            ViewBag.SubmenuFontStyleFontWight = new SelectList(lstFontWeight, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.SubmenuFontStyleFontWight));

            ViewBag.FormHeaderFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.FormHeaderFontStyleFontFamily));
            ViewBag.FormHeaderFontStyleFontSize = new SelectList(lstFontSize, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.FormHeaderFontStyleFontSize).Replace(strsize, string.Empty));
            ViewBag.FormHeaderFontStyleFontWight = new SelectList(lstFontWeight, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.FormHeaderFontStyleFontWight));

            ViewBag.FormLableFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.FormLableFontStyleFontFamily));
            ViewBag.FormLableFontStyleFontWight = new SelectList(lstFontWeight, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.FormLableFontStyleFontWight));

            ViewBag.FormControlFontStyleFontFamily = new SelectList(lstFonts, "FontName", "FontName", CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.FormControlFontStyleFontFamily));
            ViewBag.FormControlFontStyleFontSize = new SelectList(lstFontSize, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.FormControlFontStyleFontSize).Replace(strsize, string.Empty));
            ViewBag.FormControlFontStyleFontWight = new SelectList(lstFontWeight, CommonEnum.StringEnum.GetStringValue(CommonEnum.ThemeSetting.FormControlFontStyleFontWight));
        }

        #endregion
    }
}