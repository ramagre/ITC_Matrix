using ITC_Matrix.Common;
using ITC_Matrix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using static ITC_Matrix.Common.CommonEnum;

namespace ITC_Matrix.Controllers
{
    public class DashboardController : Controller
    {
        New_ITC_WMMEntities dbwmm = new New_ITC_WMMEntities();
        int TilesCount = 0;

        // GET: Dashboard
        public ActionResult Index()
        {
            List<string> lstPref = new List<string>();

            string name = "globalMenus";

            pref objpref = dbwmm.prefs.Find(name);

            if (objpref != null)
            {
                lstPref = objpref.value.Split(',').ToList();
            }

            if (lstPref.Count > 0)
            {
                CustomizeDashboard(lstPref);
            }

            return View();
        }

        /// <summary>
        /// Customize Dashboard
        /// </summary>
        /// <param name="lstPref"></param>
        private void CustomizeDashboard(List<string> lstPref)
        {
            StringBuilder strMenuHTML = new StringBuilder();

            //strMenuHTML = strMenuHTML.Append("<section class='#TilesStyle#'><section class='dashBox'><section class='dashInner-box dashReports'>");
            //strMenuHTML = strMenuHTML.Append("<b class='dashProfile-icon'></b><p>Profile</p><span class='dashReadmore'></span>");
            //strMenuHTML = strMenuHTML.Append("<section class='dropMenus'><section class='children'><ul>");
            //strMenuHTML = strMenuHTML.Append("<li><a href = '/Operators/UpdateProfile'> Profile </a></li>");
            //strMenuHTML = strMenuHTML.Append("</ul></section></section></section></section></section>");

            foreach (MainMenu mainMenuType in Enum.GetValues(typeof(MainMenu)))
            {
                if (lstPref.FindAll(x => x.ToLower().Equals(mainMenuType.ToString().ToLower())).Count == 1)
                {
                    // as System administrator is divided in three menus: Roles, Appearance, System Administrator 
                    // so separate code
                    if (mainMenuType.ToString().ToLower().Equals("sys"))
                    {
                        strMenuHTML = SplitSystemAdministratorMenus(strMenuHTML, lstPref);
                    }
                    else
                    {
                        TilesCount = TilesCount + 1;

                        strMenuHTML = strMenuHTML.Append("<section class='#TilesStyle#'><section class='dashBox'>");

                        switch (mainMenuType.ToString().ToLower())
                        {
                            case "clients":
                                strMenuHTML = strMenuHTML.Append(string.Concat("<section class='dashInner-box dashClient' onclick='ShowMenu(this);'><b class='dashClient-icon'></b><p>", Functions.GetTranslation((int)CommonEnum.Translation.Clients), "</p>"));
                                break;
                            case "accounts":
                                strMenuHTML = strMenuHTML.Append(string.Concat("<section class='dashInner-box dashAccount' onclick='ShowMenu(this);'><b class='dashAccount-icon'></b><p>", Functions.GetTranslation((int)CommonEnum.Translation.Accounts), "</p>"));
                                break;
                            case "device":
                                strMenuHTML = strMenuHTML.Append(string.Concat("<section class='dashInner-box dashDevices' onclick='ShowMenu(this);'><b class='dashDevices-icon'></b><p>", Functions.GetTranslation((int)CommonEnum.Translation.Devices), "</p>"));
                                break;
                            case "plans":
                                strMenuHTML = strMenuHTML.Append(string.Concat("<section class='dashInner-box dashMeal-plans' onclick='ShowMenu(this);'><b class='dashMeal-icon'></b><p>", Functions.GetTranslation((int)CommonEnum.Translation.Privileges), "</p>"));
                                break;
                            case "reports":
                                strMenuHTML = strMenuHTML.Append(string.Concat("<section class='dashInner-box dashReports' onclick='ShowMenu(this);'><b class='dashReports-icon'></b><p>", Functions.GetTranslation((int)CommonEnum.Translation.Reports), "</p>"));
                                break;
                            case "tools":
                                strMenuHTML = strMenuHTML.Append(string.Concat("<section class='dashInner-box dashSchedule' onclick='ShowMenu(this);'><b class='dashTools-icon'></b><p>", Functions.GetTranslation((int)CommonEnum.Translation.Tools), "</p>"));
                                break;
                        }

                        strMenuHTML = strMenuHTML.Append("<span class='dashReadmore'></span><section class='dropMenus'><section class='children'><ul>");
                        strMenuHTML = strMenuHTML.Append(FillSubMenus(mainMenuType.ToString(), lstPref));
                        strMenuHTML = strMenuHTML.Append("</ul></section></section></section></section></section>");

                    }
                }
            }

            if (TilesCount <= 3)
            {
                ViewBag.HtmlStr = strMenuHTML.ToString().Replace("#TilesStyle#", "dashCol-large");
            }
            else if (TilesCount <= 6)
            {
                ViewBag.HtmlStr = strMenuHTML.ToString().Replace("#TilesStyle#", "dashCol-medium");
            }
            else if (TilesCount <= 9)
            {
                ViewBag.HtmlStr = strMenuHTML.ToString().Replace("#TilesStyle#", "dashCol-small");
            }
        }

        /// <summary>
        /// Fills sub menus
        /// </summary>
        /// <param name="mainMenuType"></param>
        /// <param name="lstPref"></param>       
        /// <returns></returns>
        private string FillSubMenus(string mainMenuType, List<string> lstPref)
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
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Clients'>", Functions.GetTranslation((int)CommonEnum.Translation.Clients), "</a></li>"));
                                break;
                            case "clients_profiles":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Profile'>", Functions.GetTranslation((int)CommonEnum.Translation.Client_Profiles), "</a></li>"));
                                break;
                            case "clients_deleted":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/DeletedClients'>", Functions.GetTranslation((int)CommonEnum.Translation.Deleted_Clients), "</a></li>"));
                                break;
                            case "clients_departments":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/AccountCodes'>", Functions.GetTranslation((int)CommonEnum.Translation.Departments), "</a></li>"));
                                break;
                            case "clients_costcenters":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/CostCenter'>", Functions.GetTranslation((int)CommonEnum.Translation.Cost_Center), "</a></li>"));
                                break;

                            // Accounts
                            case "accounts_types":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/AccountType'>", Functions.GetTranslation((int)CommonEnum.Translation.Account_Types), "</a></li>"));
                                break;
                            case "accounts_paymenttypes":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/PayMethod'>", Functions.GetTranslation((int)CommonEnum.Translation.Payment_Methods), "</a></li>"));
                                break;

                            // Devices
                            case "device_node":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Registers'>", Functions.GetTranslation((int)CommonEnum.Translation.Devices), "</a></li>"));
                                break;
                            case "device_groups":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/RegisterGroup'>", Functions.GetTranslation((int)CommonEnum.Translation.Device_Groups), "</a></li>"));
                                break;

                            // Meal Plans
                            case "plans_mealplans":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/MealPlans'>", Functions.GetTranslation((int)CommonEnum.Translation.Meal_Plans), "</a></li>"));
                                break;
                            case "plans_types":
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href ='/Meal'>", Functions.GetTranslation((int)CommonEnum.Translation.Meal_Types), "</a></li>"));
                                break;

                            // Tools
                            case "tools_node":
                                // TODO - add href when menu implemented                                
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a>", Functions.GetTranslation((int)CommonEnum.Translation.Tools), "</a></li>"));
                                break;

                            // Tools
                            case "reports_node":                               
                                strSubMenuHTML = strSubMenuHTML.Append(string.Concat("<li><a href =" + GlobalVariables.reportsPath + " target=_blank >", Functions.GetTranslation((int)CommonEnum.Translation.Reports), "</a></li>"));
                                break;
                        }
                    }
                }
            }

            return strSubMenuHTML.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMenuHTML"></param>
        /// <param name="lstPref"></param>
        /// <returns></returns>
        private StringBuilder SplitSystemAdministratorMenus(StringBuilder strMenuHTML, List<string> lstPref)
        {
            List<string> lstSys = lstPref.FindAll(x => x.ToLower().Contains("sys"));

            #region ----------  Operators and opeartor roles ----------

            if (lstSys.FindAll(x => x.ToLower().Equals("sys_operators") || (x.ToLower().Equals("sys_roles"))).Count() > 0)
            {
                TilesCount = TilesCount + 1;

                strMenuHTML = strMenuHTML.Append("<section class='#TilesStyle#'><section class='dashBox'>");
                strMenuHTML = strMenuHTML.Append("<section class='dashInner-box dashRoles' onclick='ShowMenu(this);'><b class='dashRoles-icon'></b><p>Roles</p>");
                strMenuHTML = strMenuHTML.Append("<span class='dashReadmore'></span><section class='dropMenus'><section class='children'><ul>");

                // TODO - add href when menu implemented      
                if (lstSys.FindAll(x => x.ToLower().Equals("sys_operators")).Count == 1)
                {
                    strMenuHTML = strMenuHTML.Append("<li><a href ='/Operators'>Operators</a></li>");
                }

                if (lstSys.FindAll(x => x.ToLower().Equals("sys_roles")).Count == 1)
                {
                    strMenuHTML = strMenuHTML.Append("<li><a href ='/OperatorRoles'>Operator Roles</a></li>");
                }

                strMenuHTML = strMenuHTML.Append("</ul></section></section></section></section></section>");
            }

            #endregion

            #region ----------  Defaults, Translations ----------

            if (lstSys.FindAll(x => x.ToLower().Equals("sys_defaults") || (x.ToLower().Equals("sys_translations"))).Count() > 0)
            {
                TilesCount = TilesCount + 1;

                strMenuHTML = strMenuHTML.Append("<section class='#TilesStyle#'><section class='dashBox'>");
                strMenuHTML = strMenuHTML.Append(string.Concat("<section class='dashInner-box dashSystem-admin' onclick='ShowMenu(this);'><b class='dashAdmin-icon'></b><p>System Administration</p>"));
                strMenuHTML = strMenuHTML.Append("<span class='dashReadmore'></span><section class='dropMenus'><section class='children'><ul>");

                // TODO - add href when menu implemented      
                if (lstSys.FindAll(x => x.ToLower().Equals("sys_defaults")).Count == 1)
                {
                    strMenuHTML = strMenuHTML.Append("<li><a href ='/Prefs'>Defaults</a></li>");
                }

                if (lstSys.FindAll(x => x.ToLower().Equals("sys_translations")).Count == 1)
                {
                    strMenuHTML = strMenuHTML.Append("<li><a href ='/Translation'>Translations</a></li>");
                }

                strMenuHTML = strMenuHTML.Append("</ul></section></section></section></section></section>");
            }

            #endregion

            #region ----------  Themes, Languages and Manage Menus ----------

            if (lstSys.FindAll(x => x.ToLower().Equals("sys_themes") || (x.ToLower().Equals("sys_languages")) || (x.ToLower().Equals("sys_menus"))).Count() > 0)
            {
                TilesCount = TilesCount + 1;

                strMenuHTML = strMenuHTML.Append("<section class='#TilesStyle#'><section class='dashBox'>");
                strMenuHTML = strMenuHTML.Append(string.Concat("<section class='dashInner-box dashAppearences' onclick='ShowMenu(this);'><b class='dashAppearences-icon'></b><p>Appearance</p>"));
                strMenuHTML = strMenuHTML.Append("<span class='dashReadmore'></span><section class='dropMenus'><section class='children'><ul>");

                // TODO - add href when menu implemented      
                if (lstSys.FindAll(x => x.ToLower().Equals("sys_themes")).Count == 1)
                {
                    strMenuHTML = strMenuHTML.Append("<li><a href ='/Themes'>Themes</a></li>");
                }

                if (lstSys.FindAll(x => x.ToLower().Equals("sys_languages")).Count == 1)
                {
                    strMenuHTML = strMenuHTML.Append("<li><a href ='/AllLanguages'>Languages</a></li>");
                }

                if (lstSys.FindAll(x => x.ToLower().Equals("sys_menus")).Count == 1)
                {
                    strMenuHTML = strMenuHTML.Append("<li><a href ='/ManageMenu'>Manage Menus</a></li>");
                }

                strMenuHTML = strMenuHTML.Append("</ul></section></section></section></section></section>");
            }

            #endregion

            return strMenuHTML;
        }
    }
}