using AasthaFinance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AasthaFinance.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);


          

            //var model = filterContext.Controller.ViewData.Model as AdminLayoutModel;
            //model.UserName = getUserName();
            //Session["UserName"] = getUserName();
        }

        //private string getUserName()
        //{
        //    //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    //if (authCookie != null)
        //    //{
        //    //    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
        //    //    return ticket.Name;
        //    //}
        //    //else
        //    //{
        //    //    return "Anonymous!!";
        //    //}
        //}

    }
}
