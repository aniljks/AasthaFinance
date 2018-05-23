using AasthaFinance.Data;
using AasthaFinance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AasthaFinance.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private AasthaFinanceEntities db = new AasthaFinanceEntities();
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Account account)
        {
            var user = db.Users.Where(u => u.UserName == account.UserName
                && u.Password == account.Password).FirstOrDefault();

            if (user != null)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, account.UserName, DateTime.Now.Date, DateTime.Now.Date.AddMinutes(5), false, user.Role.RoleName.ToString());
                FormsAuthentication.SetAuthCookie(account.UserName, false);
                string[] roles = new string[] { "Admin", "Manager" };
                System.Web.HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(ticket), roles);
                this.HttpContext.User = new GenericPrincipal(new FormsIdentity(ticket), roles);
                ControllerContext.HttpContext.User = new GenericPrincipal(new FormsIdentity(ticket), roles);
                HttpContext.User = new GenericPrincipal(new FormsIdentity(ticket), roles);


                Session["UserId"] = user.UserName;

                return RedirectToAction("Index", "DashBoard");
            }
            else
            {
                account.Message = "Please enter correct username and password!";
            }
            return View(account);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(Account model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newUser = db.Users.Create();
                    var encrypPass = SHA256.Create(model.Password);
                    var user = model.UserName;
                    newUser.UserName = user;
                    newUser.Password = encrypPass.ToString();
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}
