using ITC_Matrix.Common;
using ITC_Matrix.Models;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace ITC_Matrix.Controllers
{
    public class OperatorsController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
        private New_ITC_WMMEntities dbwmn = new New_ITC_WMMEntities();

        string moduleName = Common.CommonEnum.SubMenus.sys_operators.ToString();

        // GET: Operators



        //#region  GET method for Sorting the Operators 
        ///// <summary>
        ///// POST method for Sorting the Operators 
        ///// </summary>
        ///// <param name="LOGIN"></param>
        ///// <param name="search"></param>
        ///// <returns></returns>
        //// POST : Operators

        [HttpGet]
        public ActionResult Index(int? searchBy, string search, string sortBy, int? page) //for Search Functionality
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.operator_list.ToString()) == false)
            {
                return View("NoAccess");
            }

            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.operator_edit.ToString()) == false)
            {
                ViewBag.isPermission = false;
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().ToString().Trim();
            }

            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();

            int pageSize = Functions.GetPageSize();

            // message showing insert/update/delete status
            ViewBag.Message = TempData["message"];
            ViewBag.Class = TempData["class"];

            //for sorting
            ViewBag.LoginSort = sortBy == "Login" ? "Login desc" : "Login";
            ViewBag.FamilySort = sortBy == "Family" ? "Family desc" : "Family";
            ViewBag.GivenSort = sortBy == "Given" ? "Given desc" : "Given";
            ViewBag.CommentsSort = sortBy == "Comment" ? "Comment desc" : "Comment";
            ViewBag.EmailSort = sortBy == "Email" ? "Email desc" : "Email";

            AddRole();

            DataTable dt = new DataTable();
            dt = (DataTable)Session["Operatordata"];

            List<Operator> lstOperator = new List<Operator>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Operator Operator = new Operator();

                Operator.LOGIN = dt.Rows[i]["LOGIN"].ToString();
                Operator.FAMILY = dt.Rows[i]["FAMILY"].ToString();
                Operator.GIVEN = dt.Rows[i]["GIVEN"].ToString();
                Operator.PASSWORD = dt.Rows[i]["PASSWORD"].ToString();
                Operator.COMMENT = dt.Rows[i]["COMMENT"].ToString();
                Operator.DevGroup = dt.Rows[i]["DevGroup"].ToString();
                Operator.Profile = dt.Rows[i]["Profile"].ToString();
                Operator.Email = dt.Rows[i]["Email"].ToString();
                lstOperator.Add(Operator);
            }


            //search conditions

            if (searchBy == (int)CommonEnum.SearchMethod.Login && search != string.Empty)
            {
                lstOperator = lstOperator.Where(x => x.LOGIN.ToString().StartsWith(search)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Family && search != string.Empty)
            {
                lstOperator = lstOperator.Where(x => x.FAMILY.ToString().ToLower().StartsWith(search)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Given && search != string.Empty)
            {
                lstOperator = lstOperator.Where(x => x.GIVEN.ToLower().StartsWith(search)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Comments && search != string.Empty)
            {
                lstOperator = lstOperator.Where(x => x.COMMENT.ToLower().StartsWith(search)).ToList();
            }
            else if (searchBy == (int)CommonEnum.SearchMethod.Email && search != string.Empty)
            {
                lstOperator = lstOperator.Where(x => x.LOGIN.ToLower().StartsWith(search)).ToList();
            }
            else
            {
                lstOperator = lstOperator.ToList();
            }

            //sorting
            switch (sortBy)
            {
                case "Login desc":
                    lstOperator = lstOperator.OrderByDescending(x => x.LOGIN).ToList();
                    break;

                case "Login":
                    lstOperator = lstOperator.OrderBy(x => x.LOGIN).ToList();
                    break;

                case "Family desc":
                    lstOperator = lstOperator.OrderByDescending(x => x.FAMILY).ToList();
                    break;

                case "Family":
                    lstOperator = lstOperator.OrderBy(x => x.FAMILY).ToList();
                    break;

                case "Given desc":
                    lstOperator = lstOperator.OrderByDescending(x => x.GIVEN).ToList();
                    break;

                case "Given":
                    lstOperator = lstOperator.OrderBy(x => x.GIVEN).ToList();
                    break;

                case "Comment desc":
                    lstOperator = lstOperator.OrderByDescending(x => x.COMMENT).ToList();
                    break;

                case "Comment":
                    lstOperator = lstOperator.OrderBy(x => x.COMMENT).ToList();
                    break;

                case "Email desc":
                    lstOperator = lstOperator.OrderByDescending(x => x.Email).ToList();
                    break;

                case "Email":
                    lstOperator = lstOperator.OrderBy(x => x.Email).ToList();
                    break;

                default:
                    lstOperator = lstOperator.OrderBy(x => x.LOGIN).ToList();
                    break;
            }
            return View(lstOperator.ToPagedList(page ?? 1, pageSize));
        }

        #region   All about Adding roles

        private void AddRole()
        {
            List<Operator> lstOperator = db.Operators.ToList();

            List<RightGroup> lstRightGroup = db.RightGroups.ToList();


            DataTable dtRoleId = new DataTable();
            DataRow dr;
            string RoleName = string.Empty;

            for (int i = 0; i < lstOperator.Count; i++)
            {
                // Fetching all the roles from existing operator

                RoleName = CheckIsExist(lstOperator[i].LOGIN.ToString());

                // Removing last "," comma from string

                RoleName = RoleName.Remove(RoleName.Length - 1);

                if (dtRoleId != null && dtRoleId.Rows.Count > 0)
                {
                    dr = dtRoleId.NewRow();

                    dr["LOGIN"] = lstOperator[i].LOGIN.ToString();
                    dr["FAMILY"] = lstOperator[i].FAMILY.ToString();
                    dr["GIVEN"] = lstOperator[i].GIVEN.ToString();
                    dr["PASSWORD"] = lstOperator[i].PASSWORD.ToString();
                    dr["RightGroup"] = lstOperator[i].RightGroup.ToString();
                    dr["COMMENT"] = lstOperator[i].COMMENT.ToString();
                    dr["DevGroup"] = lstOperator[i].DevGroup.ToString();
                    dr["Profile"] = RoleName;
                    dr["Account"] = string.Empty;
                    dr["DP"] = string.Empty;
                    dr["Email"] = lstOperator[i].Email.ToString();

                    dtRoleId.Rows.Add(dr);

                }
                else
                {
                    dr = dtRoleId.NewRow();

                    dtRoleId.Columns.Add("LOGIN", typeof(string));
                    dtRoleId.Columns.Add("FAMILY", typeof(string));
                    dtRoleId.Columns.Add("GIVEN", typeof(string));
                    dtRoleId.Columns.Add("PASSWORD", typeof(string));
                    dtRoleId.Columns.Add("RightGroup", typeof(string));
                    dtRoleId.Columns.Add("COMMENT", typeof(string));
                    dtRoleId.Columns.Add("DevGroup", typeof(string));
                    dtRoleId.Columns.Add("Profile", typeof(string));
                    dtRoleId.Columns.Add("Account", typeof(string));
                    dtRoleId.Columns.Add("DP", typeof(string));
                    dtRoleId.Columns.Add("Email", typeof(string));


                    dr["LOGIN"] = lstOperator[i].LOGIN.ToString();
                    dr["FAMILY"] = lstOperator[i].FAMILY.ToString();
                    dr["GIVEN"] = lstOperator[i].GIVEN.ToString();
                    dr["PASSWORD"] = lstOperator[i].PASSWORD.ToString();
                    dr["RightGroup"] = lstOperator[i].RightGroup.ToString();
                    dr["COMMENT"] = lstOperator[i].COMMENT.ToString();
                    dr["DevGroup"] = lstOperator[i].DevGroup.ToString();
                    dr["Profile"] = RoleName;
                    dr["Account"] = string.Empty;
                    dr["DP"] = string.Empty;
                    dr["Email"] = lstOperator[i].Email.ToString();

                    dtRoleId.Rows.Add(dr);

                }


            }

            Session["Operatordata"] = dtRoleId;

        }
        private string CheckIsExist(string LOGIN)
        {
            string RoleName = string.Empty;
            List<permissionsAssigned> lstpermissionsAssigned = dbwmn.permissionsAssigneds.ToList();

            for (int j = 0; j < lstpermissionsAssigned.Count; j++)
            {

                if (LOGIN == lstpermissionsAssigned[j].userid)
                {
                    string UserId = LOGIN;
                    RoleName = GetRoleId(UserId);
                    break;
                }
                else
                {
                    RoleName = "-";
                }

            }
            return RoleName;

        }
        private string GetRoleId(string userid)
        {
            string Rolename = string.Empty;
            Session["Previousrole"] = null;
            List<permissionsAssigned> lstpermissionsAssigned = dbwmn.permissionsAssigneds.Where(r => r.userid == userid).ToList();

            if (lstpermissionsAssigned.Count > 0)
            {

                for (int i = 0; i < lstpermissionsAssigned.Count; i++)
                {

                    Rolename = GetRoleName(Convert.ToInt16(lstpermissionsAssigned[i].roleid));


                    if (Rolename != null && Rolename != string.Empty)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(string.Join(",", Rolename));

                        if (Session["Previousrole"] != null)
                        {
                            Rolename = sb.ToString() + "," + " " + Session["Previousrole"].ToString();
                            Session["Previousrole"] = Rolename;
                        }
                        else
                        {
                            Rolename = sb.ToString() + ",";
                            Session["Previousrole"] = Rolename;
                        }
                    }
                    else
                    {
                        Rolename = "-";
                    }
                }

            }

            return Rolename;
        }
        private string GetRoleName(int Code)
        {
            string Rolename = string.Empty;
            List<RightGroup> lstRightGroup = db.RightGroups.Where(r => r.Code == Code).ToList();

            if (lstRightGroup.Count > 0)
            {
                Rolename = lstRightGroup[0].Dscr;
            }

            return Rolename;
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

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string password, string ConfirmPassword, string OldPassword)
        {
            ViewBag.Message = null;
            bool IsOldPasswordCorrect = false;

            if (Common.Functions.CheckSession("UserName"))
            {
                string login = Session["UserName"].ToString();

                Operator operators = db.Operators.Find(login);

                if (operators != null)
                {
                    if (operators.PASSWORD.Equals(Common.Functions.SvcEncrypt(OldPassword)))
                    {
                        IsOldPasswordCorrect = true;
                    }
                    else
                    {
                        IsOldPasswordCorrect = false;
                    }
                }

                if (IsOldPasswordCorrect)
                {
                    if (Common.Functions.SvcEncrypt(password).Equals(Common.Functions.SvcEncrypt(OldPassword)))
                    {
                        TempData["message"] = "New password should not be same as old password.";
                        ViewBag.MessageClass = "error-msg";
                    }
                    else
                    {
                        // Check password policy and display message accordingly ----------
                        if (CheckPaswordPolicy(password) == true)
                        {
                            operators.PASSWORD = Common.Functions.SvcEncrypt(password);
                            operators.ConfirmPassword = Common.Functions.SvcEncrypt(ConfirmPassword);
                            operators.OldPassword = Common.Functions.SvcEncrypt(OldPassword);

                            if (ModelState.IsValid)
                            {
                                db.Entry(operators).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                TempData["message"] = "Your password updated successfully.";
                                ViewBag.MessageClass = "success-msg";
                            }
                        }
                    }
                }
                else
                {
                    TempData["message"] = "Old password is wrong.";
                    ViewBag.MessageClass = "error-msg";
                }
            }

            ViewBag.Message = TempData["message"];
            return View();
        }

        [HttpGet]
        public ActionResult UpdateProfile()
        {
            if (Common.Functions.CheckSession("UserName"))
            {
                string login = Session["UserName"].ToString();

                if (login == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Operator @operator = db.Operators.Find(login);

                if (@operator == null)
                {
                    return HttpNotFound();
                }

                return View(@operator);
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdateProfile(string given, string family, string password, string confirmPassword, string comment, string email, string login)
        {
            ViewBag.Message = null;

            if (Common.Functions.CheckSession("UserName"))
            {
                login = Session["UserName"].ToString();

                Operator operators = db.Operators.Find(login);

                operators.FAMILY = family;
                operators.GIVEN = given;
                operators.ConfirmPassword = operators.PASSWORD;
                operators.OldPassword = operators.PASSWORD;
                operators.COMMENT = comment;
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Entry(operators).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        TempData["message"] = "Operator updated successfully.";
                        ViewBag.MessageClass = "success-msg";
                    }
                }
                catch (System.Exception ex)
                {
                    logger.Error("Error occurred while processing :", ex);
                    ViewBag.Error = ex.ToString();
                    return View("Error");
                }

            }

            ViewBag.Message = TempData["message"];
            return View();
        }

        /// <summary>
        /// Check Pasword Policy
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool CheckPaswordPolicy(string password)
        {
            string errorMessage = Common.Functions.CheckPasswordPolicy(password);

            if (!string.IsNullOrEmpty(errorMessage.Trim()))
            {
                TempData["message"] = errorMessage;
                ViewBag.Class = "error-msg";

                return false;
            }

            return true;
        }

        #region   GET create action method
        // GET: Operator/Create
        public ActionResult Create()
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName) == false)
            {
                return View("NoAccess");
            }
            ViewBag.RightGroupslist = new MultiSelectList(db.RightGroups.ToList(), "Code", "Dscr");

            return View();

        }
        #endregion


        #region Create Post Method for creating the new Operator
        /// <summary>
        /// GET method for creating the new Operator
        /// </summary>
        /// <returns></returns>
        // POST: Operators/Create

        [HttpPost]
        public ActionResult Create([Bind(Include = "LOGIN,FAMILY,GIVEN,PASSWORD,RightGroup,COMMENT,DevGroup,Profile,Account,DP,Email,ConfirmPassword,OldPassword,Username,OperatorEmail,CreatePASSWORD,CreateConfirmPassword")]Operator Operator, IEnumerable<string> RightGrouplist, permissionsAssigned permissionsAssigned)
        {
            // check module permission

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName) == false)
            {
                return View("NoAccess");
            }

            try
            {
                if (CheckPaswordPolicy(Operator.CreatePASSWORD) == true)
                {
                    if (Operator.COMMENT == null)
                    {
                        Operator.COMMENT = string.Empty;
                    }

                    Operator.Email = Operator.OperatorEmail;
                    Operator.LOGIN = Operator.Username;
                    Operator.PASSWORD = Operator.CreatePASSWORD;
                    Operator.ConfirmPassword = Operator.CreateConfirmPassword;
                    Operator.OldPassword = string.Empty;
                    Operator.RightGroup = 0;
                    Operator.DevGroup = string.Empty;
                    Operator.Profile = string.Empty;
                    Operator.PASSWORD = Common.Functions.SvcEncrypt(Operator.PASSWORD);
                    Operator.ConfirmPassword = Common.Functions.SvcEncrypt(Operator.ConfirmPassword);
                    db.Operators.Add(Operator);
                    db.SaveChanges();

                    /* Save Permission detail unnder permissionsAssigned table */

                    string RightsId = string.Empty;
                    StringBuilder sb = new StringBuilder();

                    if (RightGrouplist != null)
                    {
                        sb.Append(string.Join(",", RightGrouplist));
                        RightsId = sb.ToString() + ",";
                        if (RightsId != string.Empty && RightsId != null)
                        {
                            var words = RightsId.Split(',');
                            if (words != null)
                            {
                                for (int j = 0; j < words.Length; j++)
                                {
                                    string Id = words[j].ToString();

                                    if (Id != string.Empty && Id != null)
                                    {
                                        permissionsAssigned objpermissionsAssigned = new permissionsAssigned();
                                        objpermissionsAssigned.roleid = Convert.ToInt32(Id);
                                        objpermissionsAssigned.userid = Operator.Username;
                                        dbwmn.permissionsAssigneds.Add(objpermissionsAssigned);
                                        dbwmn.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        RightsId = string.Empty;
                    }

                    TempData["message"] = "operator added successfully.";
                    TempData["class"] = "success-msg";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = TempData["message"];
                    ViewBag.Class = "error-msg";
                    ViewBag.RightGroupslist = new MultiSelectList(db.RightGroups.ToList(), "Code", "Dscr");
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error occurred in adding operator.";
                TempData["class"] = "error-msg";
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }

        }

        #endregion

        public ActionResult Edit(string id)
        {

            // check module permission

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.operator_edit.ToString()) == false)
            {
                return View("NoAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = id.Trim();

            Operator Operator = db.Operators.Find(id);
            Operator.OperatorEmail = Operator.Email;
            Operator.Username = Operator.LOGIN;
            Operator.PASSWORD = null;

            List<permissionsAssigned> lstpermissionsAssigned = dbwmn.permissionsAssigneds.ToList();
            lstpermissionsAssigned = dbwmn.permissionsAssigneds.Where(x => x.userid.Equals(id)).ToList();

            List<RightGroup> lstRightGroup = db.RightGroups.ToList();

            List<SelectListItem> SelectedList = new List<SelectListItem>();
            List<SelectListItem> NotSelectedList = new List<SelectListItem>();

            for (int i = 0; i < lstRightGroup.Count; i++)
            {
                for (int j = 0; j < lstpermissionsAssigned.Count; j++)
                {
                    if (lstRightGroup[i].Code == lstpermissionsAssigned[j].roleid)
                    {
                        SelectedList.Add(new SelectListItem()
                        {
                            Value = Convert.ToString(lstRightGroup[i].Code),
                            Text = lstRightGroup[i].Dscr,
                        });

                    }
                }
            }


            for (int k = 0; k < lstRightGroup.Count; k++)
            {
                if (lstpermissionsAssigned.Count > 0)
                {
                    for (int m = 0; m < lstpermissionsAssigned.Count; m++)
                    {
                        if (lstRightGroup[k].Code != lstpermissionsAssigned[m].roleid)
                        {
                            NotSelectedList.Add(new SelectListItem()
                            {
                                Value = Convert.ToString(lstRightGroup[k].Code),
                                Text = lstRightGroup[k].Dscr,
                            });
                            break;
                        }
                    }
                }
            }

            for (int n = 0; n < NotSelectedList.Count; n++)
            {
                for (int p = 0; p < SelectedList.Count; p++)
                {
                    if (NotSelectedList[n].Value == SelectedList[p].Value)
                    {
                        if (NotSelectedList[n].Text == SelectedList[p].Text)
                        {
                            NotSelectedList.RemoveAt(n);
                        }
                    }
                }
            }

            if (SelectedList.Count == 0)
            {
                for (int l = 0; l < lstRightGroup.Count; l++)
                {
                    NotSelectedList.Add(new SelectListItem()
                    {
                        Value = Convert.ToString(lstRightGroup[l].Code),
                        Text = lstRightGroup[l].Dscr,
                    });

                }
            }

            TempData["Selgouplist"] = new MultiSelectList(SelectedList.ToList().AsEnumerable(), "Value", "Text");
            ViewBag.NotSelgoupsRightslist = new MultiSelectList(NotSelectedList.ToList().AsEnumerable(), "Value", "Text");
            ViewBag.RightGroupslist = new SelectList(db.RightGroups.ToList(), "Code", "Dscr", id);

            if (Operator == null)
            {
                return HttpNotFound();
            }

            return View(Operator);
        }


        #region


        private void FillRoles(string id)
        {
            Operator Operator = db.Operators.Find(id);
            Operator.OperatorEmail = Operator.Email;
            Operator.Username = Operator.LOGIN;
            Operator.PASSWORD = null;

            List<permissionsAssigned> lstpermissionsAssigned = dbwmn.permissionsAssigneds.ToList();
            lstpermissionsAssigned = dbwmn.permissionsAssigneds.Where(x => x.userid.Equals(id)).ToList();

            List<RightGroup> lstRightGroup = db.RightGroups.ToList();

            List<SelectListItem> SelectedList = new List<SelectListItem>();
            List<SelectListItem> NotSelectedList = new List<SelectListItem>();

            for (int i = 0; i < lstRightGroup.Count; i++)
            {
                for (int j = 0; j < lstpermissionsAssigned.Count; j++)
                {
                    if (lstRightGroup[i].Code == lstpermissionsAssigned[j].roleid)
                    {
                        SelectedList.Add(new SelectListItem()
                        {
                            Value = Convert.ToString(lstRightGroup[i].Code),
                            Text = lstRightGroup[i].Dscr,
                        });

                    }
                }
            }


            for (int k = 0; k < lstRightGroup.Count; k++)
            {
                if (lstpermissionsAssigned.Count > 0)
                {
                    for (int m = 0; m < lstpermissionsAssigned.Count; m++)
                    {
                        if (lstRightGroup[k].Code != lstpermissionsAssigned[m].roleid)
                        {
                            NotSelectedList.Add(new SelectListItem()
                            {
                                Value = Convert.ToString(lstRightGroup[k].Code),
                                Text = lstRightGroup[k].Dscr,
                            });
                            break;
                        }
                    }
                }
            }

            for (int n = 0; n < NotSelectedList.Count; n++)
            {
                for (int p = 0; p < SelectedList.Count; p++)
                {
                    if (NotSelectedList[n].Value == SelectedList[p].Value)
                    {
                        if (NotSelectedList[n].Text == SelectedList[p].Text)
                        {
                            NotSelectedList.RemoveAt(n);
                        }
                    }
                }
            }

            if (SelectedList.Count == 0)
            {
                for (int l = 0; l < lstRightGroup.Count; l++)
                {
                    NotSelectedList.Add(new SelectListItem()
                    {
                        Value = Convert.ToString(lstRightGroup[l].Code),
                        Text = lstRightGroup[l].Dscr,
                    });

                }
            }

            TempData["Selgouplist"] = new MultiSelectList(SelectedList.ToList().AsEnumerable(), "Value", "Text");
            ViewBag.NotSelgoupsRightslist = new MultiSelectList(NotSelectedList.ToList().AsEnumerable(), "Value", "Text");
            ViewBag.RightGroupslist = new SelectList(db.RightGroups.ToList(), "Code", "Dscr", id);

        }

        #endregion


        #region Edit POST method for Updating operator
        /// <summary>
        /// Post method for Updating operator
        /// </summary>
        /// <returns></returns>
        // POST: operator/Edit 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOGIN,FAMILY,GIVEN,PASSWORD,RightGroup,COMMENT,DevGroup,Profile,Account,DP,Email,ConfirmPassword,OldPassword,Username,OperatorEmail,CreatePASSWORD,CreateConfirmPassword")]Operator Operator, IEnumerable<string> RightGrouplist, string selectedProductString, permissionsAssigned permissionsAssigned)
        {
            // check module permission

            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.operator_edit.ToString()) == false)
            {
                return View();
            }
            try
            {

                if (Operator.COMMENT == null)
                {
                    Operator.COMMENT = string.Empty;
                }
                
                // if Password & confirm password field is null then save old password
                if (Operator.PASSWORD == null && Operator.ConfirmPassword == null)
                {
                    Operator objoperator = db.Operators.Find(Operator.Username);
                    string username = Operator.Username;
                    string OperatorEmail = Operator.OperatorEmail;

                    objoperator.Email = Operator.OperatorEmail;
                    objoperator.LOGIN = Operator.Username;
                    objoperator.OldPassword = string.Empty;
                    objoperator.RightGroup = 0;
                    objoperator.DevGroup = string.Empty;
                    objoperator.Profile = string.Empty;
                    objoperator.ConfirmPassword = GetOldPass(Operator.Username);
                    objoperator.CreatePASSWORD = GetOldPass(Operator.Username);
                    objoperator.CreateConfirmPassword = GetOldPass(Operator.Username);

                    objoperator.PASSWORD = GetOldPass(Operator.Username);
                    objoperator.ConfirmPassword = GetOldPass(Operator.Username);
                    objoperator.FAMILY = Operator.FAMILY;
                    objoperator.GIVEN = Operator.GIVEN;
                    objoperator.Username = username;
                    objoperator.OperatorEmail = OperatorEmail;

                    db.SaveChanges();
                }
                else
                {
                    // if Password & confirm password is filled then save current password

                    if (CheckPaswordPolicy(Operator.PASSWORD.Trim()) == true)
                    {
                        Operator.Email = Operator.OperatorEmail;
                        Operator.LOGIN = Operator.Username;
                        Operator.OldPassword = string.Empty;
                        Operator.RightGroup = 0;
                        Operator.DevGroup = string.Empty;
                        Operator.Profile = string.Empty;
                        Operator.ConfirmPassword = Operator.PASSWORD.Trim();
                        Operator.CreatePASSWORD = Operator.PASSWORD.Trim();
                        Operator.CreateConfirmPassword = Operator.PASSWORD.Trim();

                        Operator.PASSWORD = Common.Functions.SvcEncrypt(Operator.PASSWORD.Trim());
                        Operator.ConfirmPassword = Operator.PASSWORD.Trim();
                        Operator.FAMILY = Operator.FAMILY;
                        Operator.GIVEN = Operator.GIVEN;
                        db.Entry(Operator).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.Message = TempData["message"];
                        ViewBag.Class = "error-msg";
                        FillRoles(Operator.Username);
                        return View();
                    }
                }

                string userid = Operator.Username.ToString();

                /* delete existing permission */

                List<permissionsAssigned> permissionAssignedlst = dbwmn.permissionsAssigneds.Where(p => p.userid == userid).ToList();
                dbwmn.permissionsAssigneds.RemoveRange(permissionAssignedlst);
                dbwmn.SaveChanges();

                /* Save new Permission detail unnder permissionsAssigned table */

                string RightsId = string.Empty;

                if (selectedProductString != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Join(",", selectedProductString));
                    RightsId = sb.ToString() + ",";

                    if (RightsId != string.Empty && RightsId != null)
                    {
                        var words = RightsId.Split(',');

                        if (words != null)
                        {
                            for (int j = 0; j < words.Length; j++)
                            {
                                string Id = words[j].ToString();

                                if (Id != string.Empty && Id != null)
                                {
                                    /* save new permission */

                                    List<permissionsAssigned> lstpermissionsAssigned = dbwmn.permissionsAssigneds.ToList();
                                    lstpermissionsAssigned = dbwmn.permissionsAssigneds.Where(x => x.userid.Equals(Id)).ToList();

                                    if (lstpermissionsAssigned.Count == 0)
                                    {
                                        permissionsAssigned objpermissionsAssigned = new permissionsAssigned();
                                        objpermissionsAssigned.roleid = Convert.ToInt32(Id);
                                        objpermissionsAssigned.userid = Operator.Username;
                                        dbwmn.permissionsAssigneds.Add(objpermissionsAssigned);
                                        dbwmn.SaveChanges();
                                    }
                                }
                            }


                        }
                    }
                }

                TempData["message"] = "Operator updated successfully.";
                TempData["class"] = "success-msg";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error occurred in updating a operator.";
                ViewBag.MessageClass = "error-msg";
                logger.Error("Error occurred while processing :", ex);
                ViewBag.Error = ex.ToString();
                return View("Error");
            }


        }
        #endregion


        private string GetOldPass(string LOGIN)
        {
            List<Operator> lstOperator = db.Operators.Where(p => p.LOGIN == LOGIN).ToList();
            string oldpassword = lstOperator[0].PASSWORD.ToString();
            return oldpassword;
        }

        // GET: Operators/Delete
        [HttpPost]
        public ActionResult Delete(string id)
        {
            // check module permission
            if (Common.Functions.CheckMenuPermission(dbwmn.prefs.ToList(), moduleName, CommonEnum.ModulePermission.operator_edit.ToString()) == false)
            {
                return View("NoAccess");
            }


            Operator Operator = db.Operators.Find(id);
            db.Operators.Remove(Operator);
            db.SaveChanges();

            TempData["message"] = "Operator deleted successfully.";
            TempData["class"] = "success-msg";
            return Json(new { Success = true });

        }

        [HttpGet]
        public ActionResult CheckUserNameExists(string username)
        {

            var result = true;
            var nameexits = db.Operators.Where(x => x.LOGIN.Equals(username)).SingleOrDefault();

            if (nameexits != null || nameexits != null)
                result = false;
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult CheckEmailExists(string OperatorEmail)
        {

            var result = true;
            var emailexits = db.Operators.Where(x => x.Email.Equals(OperatorEmail) && !x.Email.Equals(OperatorEmail)).SingleOrDefault();

            if (emailexits != null || emailexits != null)
                result = false;
            return Json(result, JsonRequestBehavior.AllowGet);

        }

    }
}
