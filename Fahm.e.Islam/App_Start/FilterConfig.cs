using System.Web;
using System.Web.Mvc;

namespace ZeeSoft.Web.MVC.FI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
