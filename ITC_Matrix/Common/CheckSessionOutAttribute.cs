// @filename                : CheckSessionOutAttribute.cs
// @description             : Contains functionality related to tarack Session timeout.
// @functionality includes  : 1) While making request check for session timeout.
// @create by               : Aress Software.

using System;
using System.Web;
using System.Web.Mvc;

namespace ITC_Matrix.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            if (!controllerName.Contains("login"))
            {
                HttpSessionStateBase session = filterContext.HttpContext.Session;
                var user = session["UserName"];
                if (((user == null) && (!session.IsNewSession)) || (session.IsNewSession))
                {
                    //send them off to the login page
                    var url = new UrlHelper(filterContext.RequestContext);
                    var loginUrl = url.Content("~/Login");
                    
                    filterContext.Result = new RedirectResult(loginUrl);
                }
            }
        }
    }
}