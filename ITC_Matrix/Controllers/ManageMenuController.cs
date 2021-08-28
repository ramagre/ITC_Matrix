/*
   FileFor : Managing menus
   FileName : ManageMenuController.cs
   Created Date : 
   Created By : Nahid Shaikh
   Modified Date :12-01-2016
*/

using ITC_Matrix.Common;
using ITC_Matrix.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static ITC_Matrix.Common.CommonEnum;

namespace ITC_Matrix.Controllers
{
    public class ManageMenuController : Controller
    {
        #region ---------- Class Varaibles ----------

        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();       
        string moduleName = Common.CommonEnum.SubMenus.sys_menus.ToString();

        #endregion

        #region ---------- Action Methods ----------
        // GET: ManageMenu
        public ActionResult Index()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageMenus.ToString()) == false)
            {
                return View("NoAccess");
            }

            FillMenus();

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageMenus.ToString()) == false)
            {
                return View("NoAccess");
            }

            string name = "globalMenus";

            pref objpref = dbwmn.prefs.Find(name);

            string globalMenus = string.Join<string>(",", collection.AllKeys.ToList());

            if (objpref != null)            // Edit
            {
                objpref.value = globalMenus;

                if (ModelState.IsValid)
                {
                    dbwmn.Entry(objpref).State = EntityState.Modified;
                    dbwmn.SaveChanges();

                    ViewBag.ManageMenuMessage = "Menus updated successfully.";
                    ViewBag.MessageClass = "success-msg";
                }
            }
            else                            // Add if not exists
            {
                objpref = new pref();

                objpref.name = "globalMenus";
                objpref.value = globalMenus;

                if (ModelState.IsValid)
                {
                    try
                    {
                        dbwmn.prefs.Add(objpref);
                        dbwmn.SaveChanges();
                        ViewBag.ManageMenuMessage = "Menus added successfully.";
                        ViewBag.MessageClass = "success-msg";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ManageMenuMessage = "Error in adding menus.";
                        ViewBag.MessageClass = "error-msg";
                    }
                }
            }

            FillMenus();

            return View();
        }

        #endregion

        #region ---------- User defined functions ----------
        /// <summary>
        /// Fills all menus
        /// </summary>
        private void FillMenus()
        {
            List<string> lstPref = new List<string>();

            string name = "globalMenus";

            pref objpref = dbwmn.prefs.Find(name);

            if (objpref != null)
            {
                lstPref = objpref.value.Split(',').ToList();
            }

            StringBuilder strMenuHTML = new StringBuilder();

            List<Menus> lstMainMenu = new List<Menus>();
            Menus objMenu = new Menus();

            foreach (MainMenu mainMenuType in Enum.GetValues(typeof(MainMenu)))
            {
                bool isChecked = false;

                if (lstPref.FindAll(x => x.ToLower().Equals(mainMenuType.ToString().ToLower())).Count == 1)
                {
                    strMenuHTML = strMenuHTML.Append("<tr><td><section class='accordian-title'><section class='accordian-icon open' onclick='SlidUp(this);'></section>");
                    strMenuHTML = strMenuHTML.Append(string.Concat("<input type='checkbox' name=", mainMenuType.ToString(), " id=", mainMenuType.ToString(), " checked=checked", " onclick='CheckMainMenu(this);'>"));
                    isChecked = true;
                }
                else
                {
                    strMenuHTML = strMenuHTML.Append("<tr><td><section class='accordian-title'><section class='accordian-icon close' onclick='SlidDown(this);'></section>");
                    strMenuHTML = strMenuHTML.Append(string.Concat("<input type='checkbox' name=", mainMenuType.ToString(), " id=", mainMenuType.ToString(), " onclick='CheckMainMenu(this);'>"));
                    isChecked = false;
                }

                strMenuHTML = strMenuHTML.Append(string.Concat("<label for=", mainMenuType.ToString(), " class='checkbox-inline'> <span></span>"));
                strMenuHTML = strMenuHTML.Append(string.Concat("<div class='label-txt'>", CommonEnum.StringEnum.GetStringValue(mainMenuType), "</div>"));
                strMenuHTML = strMenuHTML.Append("</label>");

                // fill sub menues
                strMenuHTML = strMenuHTML.Append(FillSubMenus(mainMenuType.ToString(), lstPref, isChecked));
                strMenuHTML = strMenuHTML.Append(string.Concat("</label></section></td></tr>"));
            }

            ViewBag.HtmlStr = strMenuHTML;
        }

        /// <summary>
        /// Fills sub menus
        /// </summary>
        /// <param name="lstMainMenu"></param>
        private string FillSubMenus(string mainMenuType, List<string> lstPref, bool isParentChecked)
        {
            StringBuilder strSubMenuHTML = new StringBuilder();

            List<Menus> lstSubMenu = new List<Menus>();
            Menus objMenu = new Menus();

            if (isParentChecked == true)
            {
                strSubMenuHTML = strSubMenuHTML.Append("<ul class='groupOptions' style='padding-left:25px;'>");
            }
            else
            {
                strSubMenuHTML = strSubMenuHTML.Append("<ul class='groupOptions' style='padding-left:25px; display:none;'>");
            }

            foreach (SubMenus subMenuType in Enum.GetValues(typeof(SubMenus)))
            {
                if (subMenuType.ToString().ToLower().Contains(mainMenuType.ToLower()))
                {
                    if (lstPref.FindAll(x => x.ToLower().Equals(subMenuType.ToString().ToLower())).Count == 1)
                    {
                        strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li>", "<input type = 'checkbox' name=", subMenuType.ToString(), " id=", subMenuType.ToString(), " checked=checked onclick='CheckSubMenu(this, ", mainMenuType, ");' >"));
                    }
                    else
                    {
                        strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li>", "<input type = 'checkbox' name=", subMenuType.ToString(), " id=", subMenuType.ToString(), " onclick='CheckSubMenu(this, ", mainMenuType, ");' >"));
                    }

                    strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<label class='checkbox-inline' for=", subMenuType.ToString(), "><span></span>"));
                    strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<div class='label-txt'>", CommonEnum.StringEnum.GetStringValue(subMenuType), "</div>"));
                    strSubMenuHTML = strSubMenuHTML.Append("</label></li>");
                }
            }

            strSubMenuHTML = strSubMenuHTML.Append("</ul>");

            return strSubMenuHTML.ToString();
        }
        
        /// <summary>
        /// Left Menu
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult LeftMenu()
        {
            New_ITC_WMMEntities dbwmm = new New_ITC_WMMEntities();
            List<string> lstPref = new List<string>();
            StringBuilder strMenuHTML = new StringBuilder();

            string name = "globalMenus";

            pref objpref = dbwmm.prefs.Find(name);

            if (objpref != null)
            {
                lstPref = objpref.value.Split(',').ToList();
            }

            foreach (MainMenu mainMenuType in Enum.GetValues(typeof(MainMenu)))
            {
                if (lstPref.FindAll(x => x.ToLower().Equals(mainMenuType.ToString().ToLower())).Count == 1)
                {
                    //string menu = CommonEnum.StringEnum.GetStringValue(MainMenu.Clients);
                    switch (mainMenuType.ToString().ToLower())
                    {                        
                        case "clients":
                            strMenuHTML = strMenuHTML.Append(string.Concat("<li class='has-sub'><a href='#' class='clients'>", Functions.GetTranslation((int)CommonEnum.Translation.Clients), "</a>"));
                            break;                                                                     
                        case "accounts":                                                               
                            strMenuHTML = strMenuHTML.Append(string.Concat("<li class='has-sub'><a href='#' class='accounts'>", Functions.GetTranslation((int)CommonEnum.Translation.Accounts), "</a>"));
                            break;                                                                     
                        case "device":                                                                 
                            strMenuHTML = strMenuHTML.Append(string.Concat("<li class='has-sub'><a href='#' class='devices'>", Functions.GetTranslation((int)CommonEnum.Translation.Devices), "</a>"));
                            break;                                                                     
                        case "plans":                                                                  
                            strMenuHTML = strMenuHTML.Append(string.Concat("<li class='has-sub'><a href='#' class='plans'>", Functions.GetTranslation((int)CommonEnum.Translation.Privileges), "</a>"));
                            break;
                        case "sys":
                            strMenuHTML = strMenuHTML.Append(string.Concat("<li class='has-sub'><a href='#' class='admin'>", Functions.GetTranslation((int)CommonEnum.Translation.System_Administration), "</a>"));
                            break;
                        //case "tools":                                                                  
                        //    strMenuHTML = strMenuHTML.Append(string.Concat("<li class='has-sub'><a href='#' class='tools'>", CommonEnum.StringEnum.GetStringValue(mainMenuType), "</a>"));
                        //    break;
                        case "reports":
                            strMenuHTML = strMenuHTML.Append(string.Concat("<li class='has-sub'><a href='#' class='report'>", Functions.GetTranslation((int)CommonEnum.Translation.Reports), "</a>"));
                            break;
                    }
                    
                    strMenuHTML = strMenuHTML.Append("<ul class='children'>");
                    strMenuHTML = strMenuHTML.Append(FillLeftSubMenus(mainMenuType.ToString(), lstPref));
                    strMenuHTML = strMenuHTML.Append("</ul></li> ");
                }
            }            

            ViewBag.LeftMenu = strMenuHTML;

            return PartialView();
        }

        /// <summary>
        /// fills left sub menus
        /// </summary>
        /// <param name="mainMenuType"></param>
        /// <param name="lstPref"></param>
        /// <returns></returns>
        private string FillLeftSubMenus(string mainMenuType, List<string> lstPref)
        {
            StringBuilder strSubMenuHTML = new StringBuilder();
            List<Menus> lstSubMenu = new List<Menus>();
            Menus objMenu = new Menus();

            foreach (SubMenus subMenuType in Enum.GetValues(typeof(SubMenus)))
            {
                if (subMenuType.ToString().ToLower().Contains(mainMenuType.ToLower()))
                {
                    if (lstPref.FindAll(x => x.ToLower().Equals(subMenuType.ToString().ToLower())).Count == 1)
                    {
                        switch (subMenuType.ToString().ToLower())
                        {
                            // Clients
                            case "clients_node": 
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Clients' id='clientsNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Clients), "</a></li>"));
                                break;
                            case "clients_profiles":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Profile' id='profileNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Client_Profiles), "</a></li>"));
                                break;
                            case "clients_deleted":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/DeletedClients' id='deletedClientsNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Deleted_Client), "</a></li>"));
                                break;
                            case "clients_departments":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/AccountCodes' id='accountCodesNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Departments), "</a></li>"));
                                break;
                            case "clients_costcenters":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/CostCenter' id='costCenterNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Cost_Center), "</a></li>"));
                                break;

                            // Accounts
                            case "accounts_types":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/AccountType' id='accountTypeNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Account_Types), "</a></li>"));
                                break;
                            case "accounts_paymenttypes":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/PayMethod' id='payMethodNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Payment_Methods), "</a></li>"));
                                break;

                            // Devices
                            case "device_node":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Registers' id='registersNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Devices), "</a></li>"));
                                break;
                            case "device_groups":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/RegisterGroup' id='registerGroupsNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Device_Groups), "</a></li>"));
                                break;

                            // Meal Plans
                            case "plans_mealplans":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/MealPlans' id='mealPlansNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Meal_Plans), "</a></li>"));
                                break;
                            case "plans_types":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Meal' id='mealNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Meal_Types), "</a></li>"));
                                break;

                            // System Administrator
                            // TODO - remove style when menu implemented
                            case "sys_operators":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Operators' id='operatorsNode'>", CommonEnum.StringEnum.GetStringValue(subMenuType), "</a></li>"));
                                break;

                            case "sys_roles":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/OperatorRoles' id='operatorRolesNode'>", CommonEnum.StringEnum.GetStringValue(subMenuType), "</a></li>"));
                                break;
                            case "sys_defaults":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Prefs' id='prefsNode'>", CommonEnum.StringEnum.GetStringValue(subMenuType), "</a></li>"));
                                break;

                            case "sys_themes":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Themes' id='themesNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Themes), "</a></li>"));
                                break;

                            case "sys_languages":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/AllLanguages' id='allLanguagesNode'>", Functions.GetTranslation((int)CommonEnum.Translation.Languages), "</a></li>"));
                                break;

                            case "sys_translations":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Translation' id='translationNode'>", CommonEnum.StringEnum.GetStringValue(subMenuType), "</a></li>"));
                                break;

                            case "sys_menus":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/ManageMenu' id='managemenuNode'>", CommonEnum.StringEnum.GetStringValue(subMenuType), "</a></li>"));
                                break;

                            // Tools
                            case "tools_node":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li style='display:none;'><a href ='#'>", CommonEnum.StringEnum.GetStringValue(subMenuType), "</a></li>"));
                                break;

                            // Tools
                            case "reports_node":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href =" + GlobalVariables.reportsPath + " target=_blank>", CommonEnum.StringEnum.GetStringValue(subMenuType), "</a></li>"));
                                break;
                        }
                    }
                }
            }

            return strSubMenuHTML.ToString();
        }

        #endregion
    }
}