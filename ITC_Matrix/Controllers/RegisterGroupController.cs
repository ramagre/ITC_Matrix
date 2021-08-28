/*
   FileFor : Managing the RegisterGroup Model
   FileName : RegisterGroupController.cs
   Created Date : 22-11-2015
   Created By : Sandip Katore
   Modified Date :12-8-2015
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;

namespace ITC_Matrix.Controllers
{
    public class RegisterGroupController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.device_groups.ToString();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region GET Index method for filtering records based on given input
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        // GET
        [HttpGet]
        public ActionResult Index(int? searchBy, string txtSearch, int? page, string sortBy)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_device_group.ToString()) == false)
            {
                return View("NoAccess");
            }

            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.edit_device_group.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            //Converting the search text into lower case
            if (!string.IsNullOrEmpty(txtSearch))
            {
                txtSearch = txtSearch.ToLower().Trim();
            }

            int pageSize = Common.Functions.GetPageSize();

            ViewBag.CodeSort = String.IsNullOrEmpty(sortBy) ? "Code desc" : "";
            ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";

            // filter client list based on profile right assigned to operator role
            var registerGroups = db.RegisterGroups.AsQueryable();
            //List<RegisterGroup> lstregisterGroup = Common.Functions.GetPermittedDevice_Group();
            //registerGroups = registerGroups.Where(x => registerGroups.Any(y => x.CODE == x.CODE)).ToList();


           

            // message showing insert/update/delete status
            ViewBag.RegisterGroupMessage = TempData["RegisterGroupmessage"];
            ViewBag.Class = TempData["class"];

            if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(txtSearch))
            {
                //int Acc_Code = Convert.ToInt32(searchBy);
                registerGroups = registerGroups.OrderBy(x => x.CODE);
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(txtSearch))
            {
                registerGroups = registerGroups.OrderBy(x => x.DSCR);
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Code && txtSearch != string.Empty)
            {
                registerGroups = registerGroups.Where(x => x.CODE.ToString().Contains(txtSearch) || x.DSCR.ToLower().Contains(txtSearch));
            }

            else if (searchBy == (int)CommonEnum.SearchMethod.Description && txtSearch != string.Empty)
            {
                registerGroups = registerGroups.Where(x => x.DSCR.ToLower().Contains(txtSearch) || x.CODE.ToString().Contains(txtSearch));
            }
            else
            {
                registerGroups = db.RegisterGroups;
            }

            switch (sortBy)
            {
                case "Code desc":
                    registerGroups = registerGroups.OrderByDescending(x => x.CODE);
                    break;

                case "Code":
                    registerGroups = registerGroups.OrderBy(x => x.CODE);
                    break;

                case "Description desc":
                    registerGroups = registerGroups.OrderByDescending(x => x.DSCR);
                    break;

                case "Description":
                    registerGroups = registerGroups.OrderBy(x => x.DSCR);
                    break;

                default:
                    registerGroups = registerGroups.OrderBy(x => x.CODE);
                    break;
            }

            return View(registerGroups.ToPagedList(page ?? 1, pageSize));

        }

        #endregion

        #region Create Get MEthod
        // GET: RegisterGroup/Create
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.edit_device_group.ToString()) == false)
            {
                return View("NoAccess");
            }

            ViewBag.Profiles = db.Profiles.ToList();
            return View();
        }

        #endregion

        #region Create Post Method 
        // POST: RegisterGroup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODE,DSCR")] RegisterGroup registerGroup, string selecteddepartmentCR, string selectedDeviceCR)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.edit_device_group.ToString()) == false)
            {
                return View("NoAccess");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.RegisterGroups.Add(registerGroup);
                    db.SaveChanges();
                    TempData["RegisterGroupmessage"] = "Device group added successfully.";
                    TempData["class"] = "success-msg";
                    AddDeviceGroupProfile(registerGroup, selecteddepartmentCR, selectedDeviceCR);
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["RegisterGroupmessage"] = "Error occurred in adding a Register.";
                    TempData["class"] = "error-msg";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();

                return View("Error");
            }
            //return View(registerGroup);
        }

        /// <summary>
        /// Add device group profile
        /// </summary>
        /// <param name="registerGroup"></param>
        /// <param name="selecteddepartmentCR"></param>
        /// <param name="selectedDeviceCR"></param>
        private void AddDeviceGroupProfile(RegisterGroup registerGroup, string selecteddepartmentCR, string selectedDeviceCR)
        {
            selecteddepartmentCR = selecteddepartmentCR.Replace("SelectAllDepartment", string.Empty);
            string[] selecteddepartment = selecteddepartmentCR.Split(',');

            selectedDeviceCR = selectedDeviceCR.Replace("SelectAllDevice", string.Empty);
            string[] selectedDevice = selectedDeviceCR.Split(',');

            if (selecteddepartment != null)
            {
                foreach (var item in selecteddepartment)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        DeviceGroupProfile objDeviceGroupProfile = new DeviceGroupProfile();
                        objDeviceGroupProfile.DeviceGroupID = registerGroup.CODE;
                        objDeviceGroupProfile.ProfileID = Convert.ToInt16(item.Replace("department_", string.Empty));
                        objDeviceGroupProfile.ChargeType = 0;

                        db.DeviceGroupProfiles.Add(objDeviceGroupProfile);
                        db.SaveChanges();
                    }
                }
            }

            if (selectedDevice != null)
            {
                foreach (var item in selectedDevice)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        DeviceGroupProfile objDeviceGroupProfile = new DeviceGroupProfile();
                        objDeviceGroupProfile.DeviceGroupID = registerGroup.CODE;
                        objDeviceGroupProfile.ProfileID = Convert.ToInt16(item.Replace("device_", string.Empty));
                        objDeviceGroupProfile.ChargeType = 1;

                        db.DeviceGroupProfiles.Add(objDeviceGroupProfile);
                        db.SaveChanges();
                    }
                }
            }
        }

        #endregion

        #region Edit Get Method
        // GET: RegisterGroup/Edit/5
        public ActionResult Edit(short? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.edit_device_group.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RegisterGroup registerGroup = db.RegisterGroups.Find(id);

            List<Profile> lstProfileDeviceCharge = new List<Profile>();
            lstProfileDeviceCharge = db.Profiles.ToList();

            // make a copy for device and departmental restrictions as generic list updates original list also
            IEnumerable<Profile> copy = lstProfileDeviceCharge.Select(item => new Profile
            {
                AccCode = item.AccCode,
                AllowEmail = item.AllowEmail,
                CODE = item.CODE,
                DiscountPercent = item.DiscountPercent,
                DSCR = item.DSCR,
                FromHH = item.FromHH,
                FromMM = item.FromMM,
                IsCashOnly = item.IsCashOnly,
                isSelected = item.isSelected,
                ProCompleteNonTaxableAccount = item.ProCompleteNonTaxableAccount,
                ProCompleteNonTaxableAccountBool = item.ProCompleteNonTaxableAccountBool,
                RemoveTax1 = item.RemoveTax1,
                RemoveTax1Bool = item.RemoveTax1Bool,
                RemoveTax2 = item.RemoveTax2,
                RemoveTax2Bool = item.RemoveTax2Bool,
                RemoveTax3 = item.RemoveTax3,
                RemoveTax3Bool = item.RemoveTax3Bool,
                RemoveTax4 = item.RemoveTax4,
                RemoveTax4Bool = item.RemoveTax4Bool,
                RemoveTax5 = item.RemoveTax5,
                RemoveTax5Bool = item.RemoveTax5Bool,
                ResetBalanceEnabled = item.ResetBalanceEnabled,
                ResetBalanceEnabledBool = item.ResetBalanceEnabledBool,
                ResetBalanceFrom = item.ResetBalanceFrom,
                ResetBalanceLast = item.ResetBalanceLast,
                ResetBalanceTo = item.ResetBalanceTo,
                ResetBalanceValue = item.ResetBalanceValue,
                ResetDayOfMonth = item.ResetDayOfMonth,
                ResetDayOfWeek = item.ResetDayOfWeek,
                ResetFrequency = item.ResetFrequency,
                ResetFrequencyBool = item.ResetFrequencyBool,
                SecondDSCR = item.SecondDSCR,
                ToHH = item.ToHH,
                ToMM = item.ToMM,
                WeekDays = item.WeekDays
            });

            //showing the selected chechbox checked in edit mode
            List<Profile> lstProfileDepartmentalCharge = new List<Profile>(copy);

            ViewBag.ProfilesDeviceCharge = GetDeviceCR(id, lstProfileDeviceCharge);

            ViewBag.ProfilesDepartmentalCharge = GetDepartmentalCR(id, lstProfileDepartmentalCharge);

            if (registerGroup == null)
            {
                return HttpNotFound();
            }
            return View(registerGroup);
        }

        /// <summary>
        /// get departmental charge restrictions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lstProfileDepartmentalCharge"></param>
        /// <returns></returns>
        private List<Profile> GetDepartmentalCR(short? id, List<Profile> lstProfileDepartmentalCharge)
        {
            List<DeviceGroupProfile> lstDepartmentalCR = new List<DeviceGroupProfile>();
            List<DeviceGroupProfile> lstDeviceGroupProfile = db.DeviceGroupProfiles.ToList();
            lstDepartmentalCR = lstDeviceGroupProfile.FindAll(x => x.DeviceGroupID == id && x.ChargeType == 0);

            if (lstDepartmentalCR != null)
            {
                for (int i = 0; i < lstProfileDepartmentalCharge.Count; i++)
                {
                    if (lstDepartmentalCR.FindAll(x => x.ProfileID == lstProfileDepartmentalCharge[i].CODE).Count() == 1)
                    {
                        lstProfileDepartmentalCharge[i].isSelected = true;
                    }
                }
            }

            return lstProfileDepartmentalCharge;
        }

        /// <summary>
        /// get device charge restrictions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lstProfileDeviceCharge"></param>
        /// <returns></returns>
        private List<Profile> GetDeviceCR(short? id, List<Profile> lstProfileDeviceCharge)
        {
            List<DeviceGroupProfile> lstDeviceGroupProfile = db.DeviceGroupProfiles.ToList();

            List<DeviceGroupProfile> lstDeviceCR = new List<DeviceGroupProfile>();
            lstDeviceCR = lstDeviceGroupProfile.FindAll(x => x.DeviceGroupID == id && x.ChargeType == 1);

            if (lstDeviceCR != null)
            {
                for (int i = 0; i < lstProfileDeviceCharge.Count; i++)
                {
                    if (lstDeviceCR.FindAll(x => x.ProfileID == lstProfileDeviceCharge[i].CODE).Count() == 1)
                    {
                        lstProfileDeviceCharge[i].isSelected = true;
                    }
                }
            }

            return lstProfileDeviceCharge;
        }

        #endregion

        #region Edit Post Method 
        // POST: RegisterGroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODE,DSCR")] RegisterGroup registerGroup, string selecteddepartmentCR, string selectedDeviceCR)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.edit_device_group.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (ModelState.IsValid)
            {
                db.Entry(registerGroup).State = EntityState.Modified;
                db.SaveChanges();

                // first delete previous records and then insert new records.
                List<DeviceGroupProfile> lstDeviceGroupProfile = db.DeviceGroupProfiles.Where(x => x.DeviceGroupID == registerGroup.CODE).ToList();

                for (int i = 0; i < lstDeviceGroupProfile.Count; i++)
                {
                    db.DeviceGroupProfiles.Remove(lstDeviceGroupProfile[i]);
                    db.SaveChanges();
                }

                TempData["RegisterGroupmessage"] = "Device group updated successfully.";
                TempData["class"] = "success-msg";

                // insert new records
                if (ModelState.IsValid)
                {
                    AddDeviceGroupProfile(registerGroup, selecteddepartmentCR, selectedDeviceCR);

                    return RedirectToAction("Index");
                }
            }

            return View(registerGroup);
        }

        #endregion

        #region Delete actionmethod for deleting register group
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(short id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.edit_device_group.ToString()) == false)
            {
                return View("NoAccess");
            }

            var isExistInRegister = db.Registers.Where(x => x.GROUP_CODE == id).SingleOrDefault();

            if (isExistInRegister != null)
            {
                TempData["RegisterGroupmessage"] = "Device group can not be deleted as it has reference in device.";
                TempData["class"] = "error-msg";
                return Json(new { Success = false });
            }

            RegisterGroup registerGroup = db.RegisterGroups.Find(id);
            db.RegisterGroups.Remove(registerGroup);
            db.SaveChanges();
            TempData["RegisterGroupmessage"] = "Device group deleted successfully.";
            TempData["class"] = "success-msg";
            return Json(new { Success = true });
        }
        #endregion

        #region User Define function for Checking the ClientId exist or not?
        /// <summary>
        /// User Define function for Checking the ClientId exist or not?
        /// </summary>
        /// <param name="ID_NO"></param>
        /// <returns></returns>

        public JsonResult IsCODEAvailble(short? CODE)
        {
            int code = Convert.ToInt32(CODE);
            var result = true;
            var register_Code = db.RegisterGroups.Where(x => x.CODE.ToString().ToString().Trim() == code.ToString().Trim()).FirstOrDefault();

            if (register_Code != null)
                result = false;
            return Json(result, JsonRequestBehavior.AllowGet);
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
