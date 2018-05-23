//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using System.Web;
//using System.Web.Mvc;

//namespace AasthaFinance.Filters
//{
//    public class AuthorizeFilter : FilterAttribute, IAuthorizationFilter
//    {
//        string[] _allowRoles;
//        public AuthorizeFilter(params string[] roles)
//        {
//            _allowRoles = roles;
//        }
//        public void OnAuthorization(AuthorizationContext filterContext)
//        {
//            IPrincipal user = filterContext.HttpContext.User;
//            string authType = user.Identity.AuthenticationType;
//            string uName = user.Identity.Name;
            
//        }
//    }
//}