using System.Web;
using System.Web.Mvc;

namespace AasthaFinance
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // filters.Add(new AuthorizeAttribute());
            //filters.Add(new AasthaFinance.Filters.AuthorizeFilter(new string[] { "Admin", "Manager" }));
        }
    }
}