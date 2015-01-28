using System.Web;
using System.Web.Mvc;

namespace aspnet_mvc_azuresearch
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
