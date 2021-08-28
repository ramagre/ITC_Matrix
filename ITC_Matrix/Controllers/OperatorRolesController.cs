/*
    FileName : OperatorRolesController.cs
    FileFor : Managing the OperatorRole    
    Created Date : 11-1-2016 
    Created By : Sandip Katore
    Last Moddified Date :     
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;

namespace ITC_Matrix.Controllers
{
    public class OperatorRolesController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.sys_operators.ToString();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Index Post ActionMethod for search result,pagination. 
        // index actionmethod for listing the operator roles,searching and pagination.
        // GET: OperatorRoles
        [HttpGet]
        public ActionResult Index(int? searchBy, string txtSearch, int? page, string sortBy)
        {
            try
            {
                ViewBag.OperatorRolesMessage = TempData["OperatorRolesmessage"];
                ViewBag.MessageClass = TempData["class"];

                if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName) == false)
                {
                    return View("NoAccess");
                }

                // check module permission to hiding the buttons.
                if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageRoles.ToString()) == false)
                {
                    ViewBag.isPermission = false;
                }

                int pageSize = Common.Functions.GetPageSize();

                if (!string.IsNullOrEmpty(txtSearch))
                {
                    txtSearch = txtSearch.ToLower().Trim();
                }

                ViewBag.CodeSort = String.IsNullOrEmpty(sortBy) ? "Code desc" : "";
                ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";

                List<RightGroup> lstRightGroup = db.RightGroups.ToList();

                //for searching conditions
                if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(txtSearch))
                {
                    lstRightGroup = lstRightGroup.OrderBy(x => x.Code).ToList();
                }
                else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(txtSearch))
                {
                    lstRightGroup = lstRightGroup.OrderBy(x => x.Dscr).ToList();
                }
                else if (searchBy == (int)CommonEnum.SearchMethod.Code && !string.IsNullOrEmpty(txtSearch))
                {
                    lstRightGroup = lstRightGroup.Where(x => x.Code.ToString().Trim().Contains(txtSearch)).ToList();
                }
                else if (searchBy == (int)CommonEnum.SearchMethod.Description && !string.IsNullOrEmpty(txtSearch))
                {
                    lstRightGroup = lstRightGroup.Where(x => x.Dscr.Contains(txtSearch)).ToList();
                }
                else
                {
                    lstRightGroup = db.RightGroups.ToList();
                }

                switch (sortBy)
                {
                    case "Code desc":
                        lstRightGroup = lstRightGroup.OrderByDescending(x => x.Code).ToList();
                        break;

                    case "Code":
                        lstRightGroup = lstRightGroup.OrderBy(x => x.Code).ToList();
                        break;

                    case "Description desc":
                        lstRightGroup = lstRightGroup.OrderByDescending(x => x.Dscr).ToList();
                        break;

                    case "Description":
                        lstRightGroup = lstRightGroup.OrderBy(x => x.Dscr).ToList();
                        break;

                    default:
                        lstRightGroup = lstRightGroup.OrderBy(x => x.Code).ToList();
                        break;
                }
                return View(lstRightGroup.ToList().ToPagedList(page ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region create get actionmethod

        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageRoles.ToString()) == false)
            {
                return View("NoAccess");
            }

            //data for multiselectlist 
            ViewBag.RegisterGroup = new MultiSelectList(db.RegisterGroups.ToList(), "CODE", "DSCR");
            ViewBag.ProfileGroup = new MultiSelectList(db.Profiles.ToList(), "CODE", "DSCR");
            ViewBag.PaymentMethod = new MultiSelectList(db.PayMethods.ToList(), "CODE", "DSCR");

            var DBstructure = Common.Functions.GetDBStructure();

            if (DBstructure >= 15) {

                List<string> lstpermissionsTask = dbwmn.permissionsTasks.ToList().Select(x =>x.taskgroup).Distinct().ToList();
                var listAccounts = new List<string> { "account1", "account2", "account3", "account4", "account5","reports" };
                List<string> lstString = new List<string>();

                lstString = lstpermissionsTask.Except(listAccounts).ToList();

                List<permissionsTask> lstPermissionsTask = new List<permissionsTask>();
               
                foreach (var item in lstString)
                {
                    lstPermissionsTask.AddRange(dbwmn.permissionsTasks.Where(x => x.taskgroup == item).ToList());
                }

                ViewBag.distinctTaskGroups = lstPermissionsTask.OrderBy(x => x.taskgroup).Select(x => x.taskgroup).Distinct().ToList();
                ViewBag.permissionsTasks = dbwmn.permissionsTasks.ToList();
            }

            else
            {
                var perlst = dbwmn.permissionsTasks.ToList();
                ViewBag.distinctTaskGroups = perlst.OrderBy(x => x.taskgroup).Select(x => x.taskgroup).Distinct().ToList();
                ViewBag.permissionsTasks = dbwmn.permissionsTasks.ToList();
            }
            return View();
        }
        #endregion

        #region create post actionmethod
        /// <summary>
        /// Create post method for creating a new operator role
        /// </summary>
        /// <param name="rightGroup"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create([Bind(Include = "Code,RightMap,Dscr,RegisterList,ProfileList,PayMethodList")] RightGroup rightGroup, IEnumerable<string> RegisterList, IEnumerable<string> ProfileList, IEnumerable<string> PayMethodList, string selectedChechboxid)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageRoles.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                var result = from c in db.RightGroups select new { c.Code };
                short max = 0;
                if (result != null)
                {
                    if (result.Count() > 0)
                    {
                        max = result.Max(x => x.Code);
                    }
                }
                rightGroup.Code = Convert.ToInt16(max + 1);


                if (string.IsNullOrEmpty(rightGroup.RightMap))
                {
                    rightGroup.RightMap = string.Empty;
                }

                if (string.IsNullOrEmpty(rightGroup.Dscr))
                {
                    rightGroup.Dscr = string.Empty;
                }

                db.RightGroups.Add(rightGroup);
                db.SaveChanges();

                //for storing the permissionsRolesTasks 
                permissionsRolesTask objpermissionsRolesTasks = new permissionsRolesTask();
                if (selectedChechboxid != null)
                {
                    string[] selectedid = selectedChechboxid.Split(',');
                    //separting string and interger from the string list.
                    List<int> lstint = new List<int>();
                    List<string> lststring = new List<string>();
                    foreach (var item in selectedid)
                    {
                        if (item.All(char.IsDigit) == true)
                        {
                            lstint.Add(Convert.ToInt32(item));
                        }
                        else
                        {
                            lststring.Add(item);
                        }
                    }

                    if (lstint != null && lstint.Count() > 0)
                    {
                        foreach (var item in lstint)
                        {
                            objpermissionsRolesTasks.taskid = Convert.ToInt32(item);
                            objpermissionsRolesTasks.roleid = rightGroup.Code;
                            dbwmn.permissionsRolesTasks.Add(objpermissionsRolesTasks);
                            dbwmn.SaveChanges();
                        }
                    }
                }

                // for storing data into Permission Grant Table
                permissionsGrant objpermissionsGrant = new permissionsGrant();
                // inserting selected devices
                if (RegisterList != null)
                {
                    foreach (var item in RegisterList)
                    {
                        objpermissionsGrant.pid = Convert.ToInt32(item);
                        objpermissionsGrant.ptype = "devgroup";
                        objpermissionsGrant.roleid = rightGroup.Code;
                        dbwmn.permissionsGrants.Add(objpermissionsGrant);
                        dbwmn.SaveChanges();
                    }
                }
                // inserting selected Profiles
                if (ProfileList != null)
                {
                    foreach (var item in ProfileList)
                    {
                        objpermissionsGrant.pid = Convert.ToInt32(item);
                        objpermissionsGrant.ptype = "profile";
                        objpermissionsGrant.roleid = rightGroup.Code;
                        dbwmn.permissionsGrants.Add(objpermissionsGrant);
                        dbwmn.SaveChanges();
                    }
                }

                // inserting selected paymethods
                if (PayMethodList != null)
                {
                    foreach (var item in PayMethodList)
                    {
                        objpermissionsGrant.pid = Convert.ToInt32(item);
                        objpermissionsGrant.ptype = "payMethod";
                        objpermissionsGrant.roleid = rightGroup.Code;
                        dbwmn.permissionsGrants.Add(objpermissionsGrant);
                        dbwmn.SaveChanges();
                    }
                }

                // for adding the operator role name and id
                TempData["OperatorRolesmessage"] = "Operator Role added successfully.";
                TempData["class"] = "success-msg";
                return RedirectToAction("Index");
                //return Json(new { Success = false });
            }
            //}
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return RedirectToAction("Index");
                //return Json(new { Success = true });
            }
        }

        #endregion

        #region Edit Get method for editing the Operator Role details  
        /// <summary>
        /// Edit Get method for editing the account type details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        // GET: AccountType/Edit/5
        public ActionResult Edit(short? id)
        {
            //// check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageRoles.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //for displaying selected and not selected list of the devices For multiselectd list.

           var SelectedDeviceList = new List<SelectListItem>();
            var NotSelectedDevicelist = new List<SelectListItem>();

            var QueryRegister = dbwmn.permissionsGrants.Where(x => x.roleid == id && x.ptype == "devgroup").ToList();

            var Register = db.RegisterGroups.ToList();
            //adding selected device group into arraylist.
            ArrayList al = new ArrayList();
            foreach (var item in QueryRegister)
            {
                al.Add(item.pid.ToString());
            }

            var SelectedList = new List<SelectListItem>();
            var NotSelectedList = new List<SelectListItem>();
            foreach (var item in Register)
            {
                if (al.Contains(item.CODE.ToString().Trim()))
                {
                    SelectedList.Add(new SelectListItem() //add selected device
                    {
                        Value = Convert.ToString(item.CODE),
                        Text = item.DSCR,
                    });
                }
                else
                {
                    NotSelectedList.Add(new SelectListItem() // add not selected device
                    {
                        Value = Convert.ToString(item.CODE),
                        Text = item.DSCR,
                    });
                }
            }

            ViewBag.SelectedDeviceList = new MultiSelectList(SelectedList.ToList().AsEnumerable(), "Value", "Text");
            ViewBag.NotSelectedDevicelist = new MultiSelectList(NotSelectedList.ToList().AsEnumerable(), "Value", "Text");

            // for displaying selected and not selected list of the profile
            var lstProfileInPermissionGrant = dbwmn.permissionsGrants.Where(x => x.roleid == id && x.ptype == "profile").ToList();

            ArrayList lstSelectedProfile = new ArrayList();
            foreach (var item in lstProfileInPermissionGrant)
            {
                lstSelectedProfile.Add(item.pid.ToString());
            }

            var lstProfile = db.Profiles.ToList();

            var selectedProfileList = new List<SelectListItem>();
            var notSelectedProfileList = new List<SelectListItem>();

            foreach (var item in lstProfile)
            {
                if (lstSelectedProfile.Contains(item.CODE.ToString().Trim()))
                {
                    selectedProfileList.Add(new SelectListItem() //add selected profile
                    {
                        Value = Convert.ToString(item.CODE),
                        Text = item.DSCR,
                    });
                }
                else
                {
                    notSelectedProfileList.Add(new SelectListItem() // add not selected profile
                    {
                        Value = Convert.ToString(item.CODE),
                        Text = item.DSCR,
                    });
                }
            }

            ViewBag.selectedProfileList = new MultiSelectList(selectedProfileList.ToList().AsEnumerable(), "Value", "Text");
            ViewBag.notSelectedProfileList = new MultiSelectList(notSelectedProfileList.ToList().AsEnumerable(), "Value", "Text");


            // for displaying selected and not selected list of the profile
            var lstpayMethodInPermissionGrant = dbwmn.permissionsGrants.Where(x => x.roleid == id && x.ptype == "payMethod").ToList();
            var lstPayMethod = db.PayMethods.ToList();

            ArrayList lstSelectedPayMethod = new ArrayList();
            foreach (var item in lstpayMethodInPermissionGrant)
            {
                lstSelectedPayMethod.Add(item.pid.ToString());
            }

            var selectedPayMethodList = new List<SelectListItem>();
            var notSelectedPayMethodList = new List<SelectListItem>();

            foreach (var item in lstPayMethod)
            {
                if (lstSelectedPayMethod.Contains(item.CODE.ToString().Trim()))
                {
                    selectedPayMethodList.Add(new SelectListItem() // add selected pay method
                    {
                        Value = Convert.ToString(item.CODE),
                        Text = item.DSCR,
                    });
                }
                else
                {
                    notSelectedPayMethodList.Add(new SelectListItem() //add not selected pay method
                    {
                        Value = Convert.ToString(item.CODE),
                        Text = item.DSCR,
                    });
                }

            }

            ViewBag.selectedPayMethodList = new MultiSelectList(selectedPayMethodList.ToList().AsEnumerable(), "Value", "Text");
            ViewBag.notSelectedPayMethodList = new MultiSelectList(notSelectedPayMethodList.ToList().AsEnumerable(), "Value", "Text");


            // if DBstructure greter then 15
            var DBstructure = Common.Functions.GetDBStructure();
            List<string> listAccounts = new List<string>();

            List<string> lstpermissionsTask1 = new List<string>();
            lstpermissionsTask1 = dbwmn.permissionsTasks.ToList().Select(x => x.taskgroup).Distinct().ToList();

            if (DBstructure >= (int) CommonEnum.DBStructure.Unlimited)
            {                
                listAccounts = new List<string> { "account1", "account2", "account3", "account4", "account5", "reports", "menuAccess" };                                           
            }
            else
            {               
                listAccounts = new List<string> { "reports", "menuAccess" };               
            }

            List<string> lstString = new List<string>();

            lstString = lstpermissionsTask1.Except(listAccounts).ToList();

            // exclude unlimited accounts
            if (DBstructure < (int)CommonEnum.DBStructure.Unlimited)
            {
                lstString = lstString.FindAll(x => !x.ToLower().Contains("unlimited"));
            }

            List<permissionsTask> lstPermissionsTask = new List<permissionsTask>();

            foreach (var item in lstString)
            {
                lstPermissionsTask.AddRange(dbwmn.permissionsTasks.Where(x => x.taskgroup.ToLower() == item.ToLower()).ToList());
            }

            ViewBag.distinctTaskGroups = lstPermissionsTask.OrderBy(x=>x.taskgroup).Select(x => x.taskgroup).Distinct().ToList();
            ViewBag.permissionsTasks = dbwmn.permissionsTasks.ToList();

            RightGroup objRightGroup = db.RightGroups.Find(id);

            //List<RightGroup> lstRightGroup = db.RightGroups.ToList();
            List<permissionsRolesTask> lstpermissionsRolesTask = dbwmn.permissionsRolesTasks.Where(x => x.roleid == id).ToList();
            List<permissionsTask> lstpermissionsTask = dbwmn.permissionsTasks.ToList();

            foreach (var item in lstpermissionsRolesTask)
            {
                foreach (var item1 in lstpermissionsTask)
                {
                    if (item.taskid.Equals(item1.taskid))
                    {
                        item1.isSelected = true;
                    }
                }
            }

            ViewBag.lstpermissionsRolesTask = lstpermissionsRolesTask;

            if (objRightGroup == null)
            {
                return HttpNotFound();
            }
            return View(objRightGroup);
        }

        #endregion

        #region Edit Post Actionmthod for updating Operator Role details.

        // GET: AccountType/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Code,RightMap,Dscr")] RightGroup rightGroup, string selectedChechboxid,string ID,string selectedPayMethodString, string selectedDeviceString,string selectedProfileString)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageRoles.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                short id = Convert.ToInt16(ID);
                rightGroup.RightMap = string.Empty;
                rightGroup.Code = id;
                db.Entry(rightGroup).State = EntityState.Modified;

                List<permissionsRolesTask> lstpermissionsRolesTask = dbwmn.permissionsRolesTasks.Where(x => x.roleid == id).ToList();
                if (lstpermissionsRolesTask != null && lstpermissionsRolesTask.Count() > 0)
                {
                    foreach (var item in lstpermissionsRolesTask)
                    {
                        dbwmn.permissionsRolesTasks.Remove(item);
                        dbwmn.SaveChanges();
                    }
                }
                
                List<permissionsGrant> lstpermissionsGrant = dbwmn.permissionsGrants.Where(x => x.roleid == id).ToList();
                if (lstpermissionsGrant != null && lstpermissionsGrant.Count() > 0) {
                    foreach (var item in lstpermissionsGrant)
                    {
                        dbwmn.permissionsGrants.Remove(item);
                        dbwmn.SaveChanges();
                    }
                }

                //----------------------------------------

                //for storing the permissionsRolesTasks 
                permissionsRolesTask objpermissionsRolesTasks = new permissionsRolesTask();
                if (selectedChechboxid != string.Empty)
                {
                    string[] selectedid = selectedChechboxid.Split(',');
                    //separting string and interger from the string list.
                    List<int> lstint = new List<int>();
                    List<string> lststring = new List<string>();
                    foreach (var item in selectedid)
                    {
                        if (item.All(char.IsDigit) == true)
                        {
                            lstint.Add(Convert.ToInt32(item));
                        }
                        else
                        {
                            lststring.Add(item);
                        }
                    }

                    if (lstint != null && lstint.Count() > 0)
                    {
                        foreach (var item in lstint)
                        {   
                                objpermissionsRolesTasks.taskid = Convert.ToInt32(item);
                                objpermissionsRolesTasks.roleid = rightGroup.Code;
                                dbwmn.permissionsRolesTasks.Add(objpermissionsRolesTasks);
                                dbwmn.SaveChanges();
                        }
                    }
                }

                // for storing data into Permission Grant Table
                permissionsGrant objpermissionsGrant = new permissionsGrant();
                // inserting selected devices
                if (selectedDeviceString != string.Empty)
                {
                    string[] selecteDeviceid = selectedDeviceString.Split(',');
                    foreach (var item in selecteDeviceid)
                    {
                        if (item != string.Empty)
                        {
                            objpermissionsGrant.pid = Convert.ToInt32(item);
                            objpermissionsGrant.ptype = "devgroup";
                            objpermissionsGrant.roleid = rightGroup.Code;
                            dbwmn.permissionsGrants.Add(objpermissionsGrant);
                            dbwmn.SaveChanges();
                        }
                    }
                }
                // inserting selected Profiles
                if (selectedProfileString != string.Empty)
                {
                    string[] selecteProfileid = selectedProfileString.Split(',');
                    foreach (var item in selecteProfileid)
                    {
                        if (item != string.Empty)
                        {
                            objpermissionsGrant.pid = Convert.ToInt32(item);
                            objpermissionsGrant.ptype = "profile";
                            objpermissionsGrant.roleid = rightGroup.Code;
                            dbwmn.permissionsGrants.Add(objpermissionsGrant);
                            dbwmn.SaveChanges();
                        }
                    }
                }

                // inserting selected paymethods
                if (selectedPayMethodString != string.Empty)
                {
                    string[] selectepayMethodid = selectedPayMethodString.Split(',');
                    foreach (var item in selectepayMethodid)
                    {
                        if (item != string.Empty)
                        {
                            objpermissionsGrant.pid = Convert.ToInt32(item);
                            objpermissionsGrant.ptype = "payMethod";
                            objpermissionsGrant.roleid = rightGroup.Code;
                            dbwmn.permissionsGrants.Add(objpermissionsGrant);
                            dbwmn.SaveChanges();
                        }
                    }
                }

                // for adding the operator role name and id
                db.SaveChanges();
                TempData["OperatorRolesmessage"] = "Operator role updated successfully.";
                TempData["class"] = "success-msg";
                return RedirectToAction("Index");
                //return Json(new { Success = false });
            }
            catch (Exception ex)
            {
                TempData["OperatorRolesmessage"] = "Error occurred in updating a operator role.";
                TempData["class"] = "error-msg";
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion
        

        #region User Define function for Checking the ClientId exist or not?
        /// <summary>
        /// User Define function for Checking the ClientId exist or not?
        /// </summary>
        /// <param name="ID_NO"></param>
        /// <returns></returns>


        public JsonResult IsDescAvailable(string Dscr,string exitingName)
           {
            var result = true;
            if (Dscr != string.Empty && exitingName == "undefined") {
                var OperatorDesc = db.RightGroups.Where(x => x.Dscr.ToString().ToLower() == Dscr.ToString().ToLower().Trim()).FirstOrDefault();
                if (OperatorDesc != null) {
                    result = false;
                }                
            }
            else {
                var OperatorDesc = db.RightGroups.Where(x => x.Dscr.ToString().ToLower() == Dscr.ToString().ToLower().Trim() && x.Dscr != exitingName).FirstOrDefault();

                if (OperatorDesc != null)
                {
                    result = false;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete actionmethod for deleting the operator Role.
        [HttpPost]
        public ActionResult Delete(string id)
        {
            short ID = Convert.ToInt16(id);

            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manageRoles.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                //removing from right group model.
                RightGroup objRightGroup = db.RightGroups.Find(ID);
                db.RightGroups.Remove(objRightGroup);
                db.SaveChanges();

                //removing from PermissionRoleTask Model.

                List<permissionsRolesTask> lstpermissionsRolesTask = dbwmn.permissionsRolesTasks.Where(x => x.roleid.Equals(ID)).ToList();
                if (lstpermissionsRolesTask.Count() > 0)
                {


                    foreach (var item in lstpermissionsRolesTask)
                    {
                        dbwmn.permissionsRolesTasks.Remove(item);
                        dbwmn.SaveChanges();
                    }
                }

                List<permissionsGrant> lstpermissionsGrant = dbwmn.permissionsGrants.Where(x => x.roleid.Equals(ID)).ToList();
                if (lstpermissionsGrant != null && lstpermissionsGrant.Count() > 0)
                {
                    foreach (var item in lstpermissionsGrant)
                    {
                        dbwmn.permissionsGrants.Remove(item);
                        dbwmn.SaveChanges();
                    }
                }

                TempData["OperatorRolesmessage"] = "Operator role deleted successfully.";
                TempData["class"] = "success-msg";
                return Json(new { Success = false });
            }
            catch (Exception ex)
            {
                TempData["OperatorRolesmessage"] = "Error occurred in deleting a Department.";
                TempData["class"] = "error-msg";
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion
    }
}