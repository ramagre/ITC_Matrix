using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;

namespace ITC_Matrix.Controllers
{
    public class RegistersController : Controller
    {
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();
        string moduleName = Common.CommonEnum.SubMenus.device_node.ToString();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Index get actionmethod for Listing page with searching and sorting login 
       /// <summary>
       /// 
       /// </summary>
       /// <param name="searchBy"></param>
       /// <param name="txtSearch"></param>
       /// <param name="sortBy"></param>
       /// <param name="page"></param>
       /// <returns></returns>
        // GET: Registers
        public ActionResult Index(int? searchBy, string txtSearch, string sortBy, int? page)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.view_devices.ToString()) == false)
            {
                return View("NoAccess");
            }

            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_devices.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            // message showing insert/update/delete status
            ViewBag.RegistersMessage = TempData["Registersmessage"];
            ViewBag.Class = TempData["class"];

            var resultreg = db.Registers.ToList();
            var resultdevice = dbwmn.deviceTypes.ToList();
            var resultreggroups = db.RegisterGroups.ToList();

            int pageSize = Common.Functions.GetPageSize();

            if (page < 0)
            {
                page = 1;
            }
            if (!string.IsNullOrEmpty(txtSearch))
            {
                txtSearch = txtSearch.ToLower().Trim();
            }
            ViewBag.CodeSort = String.IsNullOrEmpty(sortBy) ? "Code desc" : "";
            ViewBag.DescriptionSort = sortBy == "DSCR" ? "DSCR desc" : "DSCR";
            ViewBag.IPAddressort = sortBy == "IPAddress" ? "IPAddress desc" : "IPAddress";
            ViewBag.Enabledsort = sortBy == "Enabled" ? "Enabled desc" : "Enabled";
            ViewBag.Typesort = sortBy == "RegisterType" ? "RegisterType desc" : "RegisterType";
            ViewBag.Onlinesort = sortBy == "status" ? "status desc" : "status";
            ViewBag.GroupSort = sortBy == "GroupDesc" ? "GroupDesc desc" : "GroupDesc";

            var result1 = (from t1 in resultreg
                           join t2 in resultdevice
                             on new { commcol = t1.RegisterType } equals new { commcol = t2.id }
                           select new { t1.CODE, t1.DSCR, t1.IPAddress, t1.Enabled, t1.GROUP_CODE, t1.RegisterType, t2.deviceTypeName, t2.status }).ToList().AsEnumerable();

            var result = (from t1 in result1
                          join t2 in resultreggroups on t1.GROUP_CODE equals t2.CODE
                          select new { t1.CODE, t1.DSCR, t1.IPAddress, t1.Enabled, t1.GROUP_CODE, t1.RegisterType, t1.deviceTypeName, t1.status, GroupDesc = t2.DSCR }).ToList().AsEnumerable();


         
            List<Register> lstregister = new List<Register>();

            foreach (var item in result)
            {
                Register register = new Register();
                register.CODE = item.CODE;
                register.DSCR = item.DSCR;
                register.GROUP_CODE = item.GROUP_CODE;
                register.IPAddress = item.IPAddress;
                register.Enabled = item.Enabled;
                register.RegisterType = item.RegisterType;
                register.deviceTypeName = item.deviceTypeName;
                register.status = item.status;
                register.GroupDesc = item.GroupDesc;
                lstregister.Add(register);
            }

            List<RegisterGroup> lstRegisterGroup = Common.Functions.GetPermittedDevice_Group();

            lstregister = lstregister.Where(x => lstRegisterGroup.Any(y => y.CODE == x.GROUP_CODE)).ToList();


            //search conditions
            if (searchBy == (int)CommonEnum.SearchMethod.Code && string.IsNullOrEmpty(txtSearch))
            {
                lstregister = lstregister.OrderBy(x => x.CODE).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Code && (!string.IsNullOrEmpty(txtSearch)))
            {
                lstregister = lstregister.Where(x => x.CODE.ToString().Contains(txtSearch)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Description && string.IsNullOrEmpty(txtSearch))
            {
                lstregister = lstregister.OrderBy(x => x.DSCR).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Description && (!string.IsNullOrEmpty(txtSearch)))
            {
                lstregister = lstregister.Where(x => x.DSCR.ToLower().ToString().Contains(txtSearch)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.GroupDesc && (string.IsNullOrEmpty(txtSearch)))
            {
                lstregister = lstregister.OrderBy(x => x.GroupDesc.ToLower().ToString().Contains(txtSearch)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.GroupDesc && (!string.IsNullOrEmpty(txtSearch)))
            {
                lstregister = lstregister.Where(x => x.GroupDesc.ToLower().ToString().Contains(txtSearch)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.IPAddress && string.IsNullOrEmpty(txtSearch))
            {
                lstregister = lstregister.OrderBy(x => x.IPAddress).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.IPAddress && (!string.IsNullOrEmpty(txtSearch)))
            {
                lstregister = lstregister.Where(x => x.IPAddress.ToString().Contains(txtSearch)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.RegisterType && string.IsNullOrEmpty(txtSearch))
            {
                lstregister = lstregister.OrderBy(x => x.RegisterType).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.RegisterType && (!string.IsNullOrEmpty(txtSearch)))
            {
                lstregister = lstregister.Where(x => x.RegisterType.ToString().Contains(txtSearch)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Enabled && string.IsNullOrEmpty(txtSearch))
            {
                lstregister = lstregister.OrderBy(x => x.Enabled).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Enabled && (!string.IsNullOrEmpty(txtSearch)))
            {
                if (txtSearch.Contains("en"))
                {
                    lstregister = lstregister.FindAll(x => x.Enabled == 1 ).ToList();
                }
                else 
                {
                    lstregister = lstregister.FindAll(x => x.Enabled == 0).ToList();
                }
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.deviceTypeName && string.IsNullOrEmpty(txtSearch))
            {
                lstregister = lstregister.OrderBy(x => x.deviceTypeName).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.deviceTypeName && (!string.IsNullOrEmpty(txtSearch)))
            {
                lstregister = lstregister.Where(x => x.deviceTypeName.ToLower().ToString().Contains(txtSearch)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.status && string.IsNullOrEmpty(txtSearch))
            {
                lstregister = lstregister.OrderBy(x => x.status).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.status && (!string.IsNullOrEmpty(txtSearch)))
            {
                if (txtSearch.StartsWith("online"))
                {
                    lstregister = lstregister.Where(x => x.status == 1).ToList();
                }
                else
                {
                    lstregister = lstregister.Where(x => x.status == 0).ToList();
                }
            }
            else
            {
                lstregister = lstregister.ToList();
            }
            //for sorting
            switch (sortBy)
            {
                case "Code desc":
                    lstregister = lstregister.OrderByDescending(x => x.CODE).ToList();
                    break;

                case "Code":
                    lstregister = lstregister.OrderBy(x => x.CODE).ToList();
                    break;

                case "DSCR desc":
                    lstregister = lstregister.OrderByDescending(x => x.DSCR).ToList();
                    break;

                case "DSCR":
                    lstregister = lstregister.OrderBy(x => x.DSCR).ToList();
                    break;

                case "IPAddress desc":
                    lstregister = lstregister.OrderByDescending(x => x.IPAddress).ToList();
                    break;

                case "IPAddress":
                    lstregister = lstregister.OrderBy(x => x.IPAddress).ToList();
                    break;

                case "RegisterType desc":
                    lstregister = lstregister.OrderByDescending(x => x.RegisterType).ToList();
                    break;

                case "RegisterType":
                    lstregister = lstregister.OrderBy(x => x.RegisterType).ToList();
                    break;

                case "status desc":
                    lstregister = lstregister.OrderByDescending(x => x.status).ToList();
                    break;

                case "status":
                    lstregister = lstregister.OrderBy(x => x.status).ToList();
                    break;

                case "GroupDesc desc":
                    lstregister = lstregister.OrderByDescending(x => x.GroupDesc).ToList();
                    break;

                case "GroupDesc":
                    lstregister = lstregister.OrderBy(x => x.GroupDesc).ToList();
                    break;

                case "Enabled desc":
                    lstregister = lstregister.OrderByDescending(x => x.Enabled).ToList();
                    break;

                case "Enabled":
                    lstregister = lstregister.OrderBy(x => x.Enabled).ToList();
                    break;
                default:
                    lstregister = lstregister.OrderBy(x => x.CODE).ToList();
                    break;
            }

            return View(lstregister.AsEnumerable().ToPagedList(page ?? 1, pageSize));
        }

        #endregion

        #region  create get  actionmethod 

        // GET: Registers/Create
        public ActionResult Create()
         {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_devices.ToString()) == false)
            {
                return View("NoAccess");
            }

            List<RegisterGroup> lstRegister = Common.Functions.GetPermittedDevice_Group();

            if (lstRegister != null)
            {
                ViewBag.Device_Group = new SelectList(lstRegister.ToList(), "CODE", "DSCR");
            }

            ViewBag.DeviceType = new SelectList(dbwmn.deviceTypes.OrderBy(x => x.deviceTypeName).ToList(), "id", "deviceTypeName");
            ViewBag.ActCodelist = new MultiSelectList(db.Accounts.ToList(), "Code", "DSCR");
            var TimeList = new List<SelectListItem>();

            for (int i = 0; i < 24; i++)
            {
                TimeList.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString("D2") + ":00 Hrs",
                });
            }

            ViewBag.PrintQueue = null;

            ViewBag.Timelist = new MultiSelectList(TimeList.ToList().AsEnumerable(), "Value", "Text");
            return View();
        }

        #endregion

        #region acitonmethod for adding registers
        /// <summary>
        /// for adding new regiters
        /// </summary>
        /// <param name="register"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        
        // POST: Registers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODE,DSCR,Enabled,IPAddress,ACCOUNTS,LoadOldTimeMap,OffFileTimeMap,OffFileNegativeEnabled,OffFileMaxNumber,ModemEnabled,ModemPhoneNumber,RegisterType,GROUP_CODE,Desktop,ERA570OfflineMax,Budget,AccountDiscounts,UseExtendedPassBack,ExtendedPassBack,LastConnectionTime,MACAddress,Initializetype,ExtendedPassBackReset,NewCardsLastSyncTime,DeletedCardsLastSyncTime,PrimaryCredentialOnly,CreateVirtualPlan,Enabled_Bool,UseExtendedPassBack_Bool,ModemEnabled_Bool,ExtendedPassBackReset_Bool")] Register register, FormCollection fc)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_devices.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    StringBuilder straccountdisc = new StringBuilder();
                    if (fc["selectedProductString"] != "")
                    {
                        List<string> lstallkeys = fc.AllKeys.ToList();

                        string[] strsplit = fc["selectedProductString"].Replace("lbl_", "").Split(',');
                        ArrayList al = new ArrayList();
                        for (int i = 0; i < strsplit.Length; i++)
                        {
                            al.Add(strsplit[i]);
                        }

                        foreach (var item in al)
                        {
                            if (fc["devicetype"] == "15")
                            {
                                if (fc["txt_" + item.ToString()] == string.Empty)
                                {
                                    straccountdisc.Append("0.00,");
                                }
                                else
                                {
                                    straccountdisc.Append(fc["txt_" + item.ToString()] + ",");
                                }
                            }
                            else
                            {
                                straccountdisc.Append("0.00,");
                            }
                        }
                    }
                    string strout = straccountdisc.ToString();
                    if (strout.Length > 1)
                    {
                        strout = strout.Remove(strout.Length - 1, 1);
                    }

                    register.CODE = register.CODE;
                    if (register.DSCR == null)
                    {
                        register.DSCR = "";
                    }
                    else
                    {
                        register.DSCR = register.DSCR;
                    }

                    register.Enabled = register.Enabled;
                    if (register.IPAddress == null)
                    {
                        register.IPAddress = "";
                    }
                    else
                    {
                        register.IPAddress = register.IPAddress;
                    }

                    if (fc["selectedProductString"] != "")
                    {
                        register.ACCOUNTS = fc["selectedProductString"].Replace("lbl_", "");
                    }
                    else
                    {
                        register.ACCOUNTS = "";
                    }

                    if (fc["selectedTime"] != "")
                    {
                        string[] strsplittime = fc["selectedTime"].Split(',');

                        StringBuilder sb = new StringBuilder();
                        ArrayList al = new ArrayList();
                        for (int i = 0; i < strsplittime.Length; i++)
                        {
                            al.Add(strsplittime[i]);
                        }
                        for (int i = 0; i < 24; i++)
                        {
                            if (al.Contains(i.ToString()))
                                sb.Append("1");
                            else
                                sb.Append("0");
                        }
                        register.LoadOldTimeMap = sb.ToString();
                    }
                    else
                    {
                        register.LoadOldTimeMap = "000000000000000000000000";
                    }

                    register.OffFileTimeMap = "";
                    register.OffFileNegativeEnabled = register.OffFileNegativeEnabled;
                    register.OffFileMaxNumber = register.OffFileMaxNumber;
                    register.ModemEnabled = register.ModemEnabled;
                    if (register.ModemPhoneNumber == null)
                    {
                        register.ModemPhoneNumber = "";
                    }
                    else
                    {
                        register.ModemPhoneNumber = register.ModemPhoneNumber;
                    }

                    if (fc["devicetype"] == string.Empty)
                    {
                        register.RegisterType = 0;
                    }
                    else
                    {
                        register.RegisterType = Convert.ToInt16(fc["devicetype"]);
                    }
                    if (fc["Device_Group"] == string.Empty)
                    {
                        register.GROUP_CODE = 0;
                    }
                    else
                    {
                        register.GROUP_CODE = Convert.ToInt16(fc["Device_Group"]);
                    }

                    register.Desktop = register.Desktop;
                    register.ERA570OfflineMax = register.ERA570OfflineMax;


                    if (register.Budget == null)
                    {
                        register.Budget = string.Empty;
                    }
                    else
                    {
                        register.Budget = register.Budget;
                    }
                    register.AccountDiscounts = strout.ToString().Replace("0.00", "0");
                    register.UseExtendedPassBack = register.UseExtendedPassBack;
                    register.ExtendedPassBack = register.ExtendedPassBack;
                    register.LastConnectionTime = Convert.ToDateTime("1753-01-01 00:00:00.000");
                    register.ExtendedPassBackReset = register.ExtendedPassBackReset;
                    if (register.MACAddress == null)
                    {
                        register.MACAddress = string.Empty;
                    }
                    else
                    {
                        register.MACAddress = register.MACAddress;
                    }

                    register.Initializetype = register.Initializetype;

                    if (register.NewCardsLastSyncTime == null)
                    {
                        register.NewCardsLastSyncTime = DateTime.Now;
                    }
                    else
                    {
                        register.NewCardsLastSyncTime = register.NewCardsLastSyncTime;
                    }
                    if (register.DeletedCardsLastSyncTime == null)
                    {
                        register.DeletedCardsLastSyncTime = DateTime.Now;
                    }
                    else
                    {
                        register.DeletedCardsLastSyncTime = register.DeletedCardsLastSyncTime;
                    }

                    if (register.PrimaryCredentialOnly == null)
                    {
                        register.PrimaryCredentialOnly = 0;
                    }
                    else
                    {
                        register.PrimaryCredentialOnly = register.PrimaryCredentialOnly;
                    }
                    if (register.CreateVirtualPlan == null)
                    {
                        register.CreateVirtualPlan = 0;
                    }
                    else
                    {
                        register.CreateVirtualPlan = register.CreateVirtualPlan;
                    }

                    db.Registers.Add(register);

                    if (fc["ddlPriceLine1"] != string.Empty || fc["ddlPriceLine2"] != string.Empty || fc["ddlPriceLine3"] != "" || fc["ddlPriceLine4"] != string.Empty)
                    {
                        cpyDevicePaper objcpyDevicePaper = new cpyDevicePaper();
                        objcpyDevicePaper.DeviceID = register.CODE;

                        if (fc["ddlPriceLine1"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine1PaperID = 0;
                        }
                        else if (fc["ddlPriceLine1"] == "1")
                        {
                            objcpyDevicePaper.PrLine1PaperID = 1;
                        }
                        else if (fc["ddlPriceLine1"] == "2")
                        {
                            objcpyDevicePaper.PrLine1PaperID = 2;
                        }

                        if (fc["ddlPriceLine2"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine2PaperID = 0;
                        }
                        else if (fc["ddlPriceLine2"] == "1")
                        {
                            objcpyDevicePaper.PrLine2PaperID = 1;
                        }
                        else if (fc["ddlPriceLine2"] == "2")
                        {
                            objcpyDevicePaper.PrLine2PaperID = 2;
                        }

                        if (fc["ddlPriceLine3"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine3PaperID = 0;
                        }
                        else if (fc["ddlPriceLine3"] == "1")
                        {
                            objcpyDevicePaper.PrLine3PaperID = 1;
                        }
                        else if (fc["ddlPriceLine3"] == "2")
                        {
                            objcpyDevicePaper.PrLine3PaperID = 2;
                        }

                        if (fc["ddlPriceLine4"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine4PaperID = 0;
                        }
                        else if (fc["ddlPriceLine4"] == "1")
                        {
                            objcpyDevicePaper.PrLine4PaperID = 1;
                        }
                        else if (fc["ddlPriceLine4"] == "2")
                        {
                            objcpyDevicePaper.PrLine4PaperID = 2;
                        }
                        db.cpyDevicePapers.Add(objcpyDevicePaper);
                    }
                    try
                    {
                        db.SaveChanges();
                        TempData["Registersmessage"] = "Device added successfully.";
                        TempData["class"] = "success-msg";
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        TempData["Registersmessage"] = "Error occurred while adding the device.";
                        TempData["class"] = "error-msg";
                        return RedirectToAction("Index");
                    }
                }
                return View(register);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :",ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
        #endregion

        #region Get actionmethod for udpating regiter view
        /// <summary>
        /// for displaying edit mode 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Registers/Edit/5
        public ActionResult Edit(int? id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_devices.ToString()) == false)
            {
                return View("NoAccess");
            }

            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Register register = db.Registers.Find(id);

                //getting permitted device group only
                List<RegisterGroup> lstRegister = Common.Functions.GetPermittedDevice_Group();

                if (lstRegister != null)
                {
                    ViewBag.Device_Group = new SelectList(lstRegister.ToList(), "CODE", "DSCR", register.GROUP_CODE);
                }
                
                ViewBag.DeviceType = new SelectList(dbwmn.deviceTypes.OrderBy(x => x.deviceTypeName).ToList(), "id", "deviceTypeName", register.RegisterType);
                ViewBag.ActCodelist = new MultiSelectList(db.Accounts.ToList(), "Code", "DSCR");

                if (register.ACCOUNTS != null)
                {
                    string strselectedcheck = register.ACCOUNTS;
                    List<Account> lstAccounts = db.Accounts.OrderBy(x => x.CODE).ToList();
                    if (strselectedcheck != null)
                    {
                        string[] listAccount = strselectedcheck.Split(',');
                        string[] listDiscount = register.AccountDiscounts.Split(',');
                        for (int i = 0; i < listAccount.Count(); i++)
                        {
                            for (int j = 0; j < lstAccounts.Count(); j++)
                            {
                                if (listAccount[i].Equals(lstAccounts[j].CODE.ToString()))
                                {
                                    lstAccounts[j].Isselected = true;
                                    lstAccounts[j].AccountDiscount = Convert.ToString(listDiscount[i]);
                                }
                            }
                        }
                    }
                    ViewBag.AccountType = lstAccounts;
                }

                var SelectedTimeList = new List<SelectListItem>();
                var NotSelectedTimeList = new List<SelectListItem>();

                cpyDevicePaper objcpyDevicePaper = new cpyDevicePaper();
                objcpyDevicePaper = db.cpyDevicePapers.Find(register.CODE);
                var ddlPriceLine = new List<SelectListItem>();
                ddlPriceLine.Add(new SelectListItem()
                {
                    Text = "Regular",
                    Value = "1"
                });
                ddlPriceLine.Add(new SelectListItem()
                {
                    Text = "Premium",
                    Value = "2"
                });
                if (objcpyDevicePaper != null)
                {
                    ViewBag.ddlPriceLine1 = objcpyDevicePaper.PrLine1PaperID;
                    ViewBag.ddlPriceLine2 = objcpyDevicePaper.PrLine2PaperID;
                    ViewBag.ddlPriceLine3 = objcpyDevicePaper.PrLine3PaperID;
                    ViewBag.ddlPriceLine4 = objcpyDevicePaper.PrLine4PaperID;
                }
                else
                {
                    ViewBag.ddlPriceLine1 = new SelectList(ddlPriceLine.ToList(), "Value", "Text");
                    ViewBag.ddlPriceLine2 = new SelectList(ddlPriceLine.ToList(), "Value", "Text");
                    ViewBag.ddlPriceLine3 = new SelectList(ddlPriceLine.ToList(), "Value", "Text");
                    ViewBag.ddlPriceLine4 = new SelectList(ddlPriceLine.ToList(), "Value", "Text");
                }

                char[] array = register.LoadOldTimeMap.ToCharArray();

                for (int i = 0; i < array.Length; i++)
                {
                    if (Convert.ToString(array[i]) == "1")
                    {
                        SelectedTimeList.Add(new SelectListItem()
                        {
                            Value = i.ToString(),
                            Text = i.ToString("D2") + ":00 Hrs",
                        });
                    }
                    else
                    {
                        NotSelectedTimeList.Add(new SelectListItem()
                        {
                            Value = i.ToString(),
                            Text = i.ToString("D2") + ":00 Hrs",
                        });
                    }
                }
                ViewBag.SelTimelist = new MultiSelectList(SelectedTimeList.ToList().AsEnumerable(), "Value", "Text");
                ViewBag.NotSelTimelist = new MultiSelectList(NotSelectedTimeList.ToList().AsEnumerable(), "Value", "Text");
                if (register == null)
                {
                    return HttpNotFound();
                }

                var PrintQ = (from t1 in db.prtPrinters
                              join t2 in db.prtPrinterGroups on t1.PrinterGroupID equals t2.PrinterGroupID
                              where t1.DeviceID == id
                              select new { t1.PrinterName, t2.Name });

                List<prtPrinter> partsList = new List<prtPrinter>();

                foreach (var m in PrintQ)
                {
                    prtPrinter lstprt = new prtPrinter();
                    lstprt.PrinterName = m.PrinterName;
                    lstprt.PrinterGroupName = m.Name;
                    partsList.Add(lstprt);
                }

                ViewBag.PrintQueue = partsList;

                return View(register);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :",ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region User define function for check register id is exist or not in database?
        /// <summary>
        /// For remote validations
        /// </summary>
        /// <param name="CODE"></param>
        /// <returns></returns>
       
        public JsonResult IsCodeExist(string CODE)  //For Existing Code Check
        {
            try
            {
                int code = Convert.ToInt32(CODE);
                var result = true;
                var code_id = db.Registers.Where(x => x.CODE.ToString().Trim() == code.ToString().Trim()).FirstOrDefault();

                if (code_id != null)
                    result = false;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region actionmethod for updating the register
        /// <summary>
        /// for updating the registers
        /// </summary>
        /// <param name="register"></param>
        /// <param name="fc"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODE,DSCR,Enabled,IPAddress,ACCOUNTS,LoadOldTimeMap,OffFileTimeMap,OffFileNegativeEnabled,OffFileMaxNumber,ModemEnabled,ModemPhoneNumber,RegisterType,GROUP_CODE,Desktop,ERA570OfflineMax,Budget,AccountDiscounts,UseExtendedPassBack,ExtendedPassBack,LastConnectionTime,MACAddress,Initializetype,ExtendedPassBackReset,NewCardsLastSyncTime,DeletedCardsLastSyncTime,PrimaryCredentialOnly,CreateVirtualPlan,Enabled_Bool,UseExtendedPassBack_Bool,ModemEnabled_Bool,ExtendedPassBackReset_Bool")] Register register, FormCollection fc, string Command)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_devices.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (ModelState.IsValid)
            {
                StringBuilder straccountdisc = new StringBuilder();
                if (fc["selectedProductString"] != string.Empty)
                {
                    string[] strsplit = fc["selectedProductString"].Replace("lbl_", string.Empty).Split(',');
                    ArrayList al = new ArrayList();
                    for (int i = 0; i < strsplit.Length; i++)
                    {
                        al.Add(strsplit[i]);
                    }

                    foreach (var item in al)
                    {
                        if (fc["devicetype"] == "15")
                        {
                            if(fc["txt_" + item.ToString()] == string.Empty)
                            {
                                straccountdisc.Append("0.00,");
                            }
                            else
                            {
                                straccountdisc.Append(fc["txt_" + item.ToString()] + ",");
                            }
                        }
                        else
                        {
                            straccountdisc.Append("0.00,");
                        }
                    }
                }
                string strout = straccountdisc.ToString();
                if(strout.Length > 1)
                {
                    strout = strout.Remove(strout.Length - 1, 1);
                }

                register.CODE = register.CODE;
                if (register.DSCR == null)
                {
                    register.DSCR = string.Empty;
                }
                else
                {
                    register.DSCR = register.DSCR;
                }

                register.Enabled = register.Enabled;
                if (register.IPAddress == null)
                {
                    register.IPAddress = string.Empty;
                }
                else
                {
                    register.IPAddress = register.IPAddress;
                }

                if (fc["selectedProductString"] != string.Empty)
                {
                    register.ACCOUNTS = fc["selectedProductString"].Replace("lbl_", string.Empty);
                }
                else
                {
                    register.ACCOUNTS = string.Empty;
                }

                if (fc["selectedTime"] != string.Empty)
                {
                    string[] strsplittime = fc["selectedTime"].Split(',');

                    StringBuilder sb = new StringBuilder();
                    ArrayList al = new ArrayList();
                    for (int i = 0; i < strsplittime.Length; i++)
                    {
                        al.Add(strsplittime[i]);
                    }
                    for (int i = 0; i < 24; i++)
                    {
                        if (al.Contains(i.ToString()))
                            sb.Append("1");
                        else
                            sb.Append("0");
                    }
                    register.LoadOldTimeMap = sb.ToString();
                }
                else
                {
                    register.LoadOldTimeMap = "000000000000000000000000";
                }

                register.OffFileTimeMap = "";
                register.OffFileNegativeEnabled = register.OffFileNegativeEnabled;
                register.OffFileMaxNumber = register.OffFileMaxNumber;
                register.ModemEnabled = register.ModemEnabled;
                if (register.ModemPhoneNumber == null)
                {
                    register.ModemPhoneNumber = string.Empty;
                }
                else
                {
                    register.ModemPhoneNumber = register.ModemPhoneNumber;
                }

                if (fc["devicetype"] == string.Empty)
                {
                    register.RegisterType = 0;
                }
                else
                {
                    register.RegisterType = Convert.ToInt16(fc["devicetype"]);
                }
                if (fc["Device_Group"] == string.Empty)
                {
                    register.GROUP_CODE = 0;
                }
                else
                {
                    register.GROUP_CODE = Convert.ToInt16(fc["Device_Group"]);
                }

                register.Desktop = register.Desktop;
                register.ERA570OfflineMax = register.ERA570OfflineMax;


                if (register.Budget == null)
                {
                    register.Budget = string.Empty;
                }
                else
                {
                    register.Budget = register.Budget;
                }
                register.AccountDiscounts = strout.ToString().Replace("0.00", "0");
                register.UseExtendedPassBack = register.UseExtendedPassBack;
                register.ExtendedPassBack = register.ExtendedPassBack;
                register.LastConnectionTime = Convert.ToDateTime("1753-01-01 00:00:00.000");
                register.ExtendedPassBackReset = register.ExtendedPassBackReset;
                if (register.MACAddress == null)
                {
                    register.MACAddress = string.Empty;
                }
                else
                {
                    register.MACAddress = register.MACAddress;
                }

                register.Initializetype = register.Initializetype;

                if (register.NewCardsLastSyncTime == null)
                {
                    //register.NewCardsLastSyncTime = DateTime.Now;
                }
                else
                {
                    register.NewCardsLastSyncTime = register.NewCardsLastSyncTime;
                }
                if (register.DeletedCardsLastSyncTime == null)
                {
                    //register.DeletedCardsLastSyncTime = DateTime.Now;
                }
                else
                {
                    register.DeletedCardsLastSyncTime = register.DeletedCardsLastSyncTime;
                }

                if (register.PrimaryCredentialOnly == null)
                {
                    register.PrimaryCredentialOnly = 0;
                }
                else
                {
                    register.PrimaryCredentialOnly = register.PrimaryCredentialOnly;
                }
                if (register.CreateVirtualPlan == null)
                {
                    register.CreateVirtualPlan = 0;
                }
                else
                {
                    register.CreateVirtualPlan = register.CreateVirtualPlan;
                }

                bool IsCopy = false;

                //Check for creating the copy of devices

                if (Command == "Make Copy of Device")
                {
                    var result = from c in db.Registers select new { c.CODE };
                    var max = 0;
                    if (result != null)
                    {
                        if (result.Count() > 0)
                        {
                            max = result.Max(x => x.CODE);
                        }
                    }

                    register.CODE = max + 1;
                    register.DSCR = "Copy Of " + register.DSCR;
                    IsCopy = true;
                }
                
                if (fc["ddlPriceLine1"] != string.Empty || fc["ddlPriceLine2"] != string.Empty || fc["ddlPriceLine3"] != string.Empty || fc["ddlPriceLine4"] != string.Empty)
                {

                    cpyDevicePaper objcpyDevicePaper = new cpyDevicePaper();
                    objcpyDevicePaper = db.cpyDevicePapers.Find(register.CODE);
                    if (objcpyDevicePaper != null)
                    {
                        objcpyDevicePaper.DeviceID = register.CODE;
                        if (fc["ddlPriceLine1"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine1PaperID = 0;
                        }
                        else if (fc["ddlPriceLine1"] == "1")
                        {
                            objcpyDevicePaper.PrLine1PaperID = 1;
                        }
                        else if (fc["ddlPriceLine1"] == "2")
                        {
                            objcpyDevicePaper.PrLine1PaperID = 2;
                        }

                        if (fc["ddlPriceLine2"] == "")
                        {
                            objcpyDevicePaper.PrLine2PaperID = 0;
                        }
                        else if (fc["ddlPriceLine2"] == "1")
                        {
                            objcpyDevicePaper.PrLine2PaperID = 1;
                        }
                        else if (fc["ddlPriceLine2"] == "2")
                        {
                            objcpyDevicePaper.PrLine2PaperID = 2;
                        }

                        if (fc["ddlPriceLine3"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine3PaperID = 0;
                        }
                        else if (fc["ddlPriceLine3"] == "1")
                        {
                            objcpyDevicePaper.PrLine3PaperID = 1;
                        }
                        else if (fc["ddlPriceLine3"] == "2")
                        {
                            objcpyDevicePaper.PrLine3PaperID = 2;
                        }

                        if (fc["ddlPriceLine4"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine4PaperID = 0;
                        }
                        else if (fc["ddlPriceLine4"] == "1")
                        {
                            objcpyDevicePaper.PrLine4PaperID = 1;
                        }
                        else if (fc["ddlPriceLine4"] == "2")
                        {
                            objcpyDevicePaper.PrLine4PaperID = 2;
                        }

                        db.Entry(objcpyDevicePaper).State = EntityState.Modified;
                    }
                    else
                    {
                        objcpyDevicePaper = new cpyDevicePaper();
                        objcpyDevicePaper.DeviceID = register.CODE;
                        if (fc["ddlPriceLine1"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine1PaperID = 0;
                        }
                        else if (fc["ddlPriceLine1"] == "1")
                        {
                            objcpyDevicePaper.PrLine1PaperID = 1;
                        }
                        else if (fc["ddlPriceLine1"] == "2")
                        {
                            objcpyDevicePaper.PrLine1PaperID = 2;
                        }

                        if (fc["ddlPriceLine2"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine2PaperID = 0;
                        }
                        else if (fc["ddlPriceLine2"] == "1")
                        {
                            objcpyDevicePaper.PrLine2PaperID = 1;
                        }
                        else if (fc["ddlPriceLine2"] == "2")
                        {
                            objcpyDevicePaper.PrLine2PaperID = 2;
                        }

                        if (fc["ddlPriceLine3"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine3PaperID = 0;
                        }
                        else if (fc["ddlPriceLine3"] == "1")
                        {
                            objcpyDevicePaper.PrLine3PaperID = 1;
                        }
                        else if (fc["ddlPriceLine3"] == "2")
                        {
                            objcpyDevicePaper.PrLine3PaperID = 2;
                        }

                        if (fc["ddlPriceLine4"] == string.Empty)
                        {
                            objcpyDevicePaper.PrLine4PaperID = 0;
                        }
                        else if (fc["ddlPriceLine4"] == "1")
                        {
                            objcpyDevicePaper.PrLine4PaperID = 1;
                        }
                        else if (fc["ddlPriceLine4"] == "2")
                        {
                            objcpyDevicePaper.PrLine4PaperID = 2;
                        }
                        db.cpyDevicePapers.Add(objcpyDevicePaper);
                    }
                }

                if (IsCopy == true)
                {
                    db.Registers.Add(register);
                }
                else
                {
                    db.Entry(register).State = EntityState.Modified;
                }
                try
                {
                    
                    db.SaveChanges();
                    if (IsCopy == true)
                    {
                        TempData["Registersmessage"] = "Copy of Device created successfully.";
                        TempData["class"] = "success-msg";
                    }
                    else
                    {
                        TempData["Registersmessage"] = "Device updated successfully.";
                        TempData["class"] = "success-msg";
                    }
                    
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while processing :",ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }
            }
            return View(register);
        }


        #endregion

        #region actionmethod for deleting the device/register
        /// <summary>
        /// actionmethod for deleting the device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.manage_devices.ToString()) == false)
            {
                return View("NoAccess");
            }
            try
            {
                Register register = db.Registers.Find(id);
                db.Registers.Remove(register);
                db.SaveChanges();
                TempData["Registersmessage"] = "Device deleted successfully.";
                TempData["class"] = "success-msg";
                return Json(new { Success = false });
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while processing :",ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        #endregion

        #region  Get Class of device to show and hide the sections
        /// <summary>
        ///Class of device to show and hide the sections
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //Get Class of device to show and hide the sections
        public JsonResult GetDeviceClass(int id)
        {
            var result = 0;
            var objDeviceTypes = dbwmn.deviceTypes.Where(x => x.id == id).FirstOrDefault();

            if (objDeviceTypes == null)
            {
                result = 0;
            }
            else
            {
                result = objDeviceTypes.deviceClass;
            }

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
