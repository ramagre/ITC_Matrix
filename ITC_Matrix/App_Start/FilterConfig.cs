using ITC_Matrix.Common;
using System.Web;
using System.Web.Mvc;

namespace ITC_Matrix
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckSessionOutAttribute());
        }
    }
}
