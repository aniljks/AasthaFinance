using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AasthaFinance.Data;
using PagedList;
using AasthaFinance.Models;
using System.IO;
//using AasthaFinance.Filters;

namespace AasthaFinance.Controllers
{
    //[AuthorizeFilter("Admin")]
    [Authorize]
    public class ClientController : BaseController
    {
        private AasthaFinanceEntities db = new AasthaFinanceEntities();

        //
        // GET: /Client/

        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            System.Security.Principal.IPrincipal user = this.ControllerContext.HttpContext.User;
            
            var clients = db.Clients.Include(c => c.Branch).Include(c => c.Gender).Include(c => c.User);
            PagedList<Client> model = new PagedList<Client>(clients.ToList(), page, pageSize);
            return View(model);
        }

        //
        // GET: /Client/Details/5

        public ActionResult Details(int id = 0)
        {
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        //
        // GET: /Client/Create

        public ActionResult Create()
        {
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.GenderId = new SelectList(db.Genders, "GenderId", "Gender1");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");

            #region Create Client Code
            //AF20180001001
            int maxClientId = 1;
            if (db.Clients.Count() > 0)
                maxClientId = db.Clients.Max(x => x.ClientId) + 1;

            ViewBag.ClientCode = "AFC" + DateTime.Now.Year + Common.AddAdditionalZero(maxClientId.ToString());

            #endregion


            return View();
        }

        //
        // POST: /Client/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Client client, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {


                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/ClientImage"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                    client.ClientImage = "/ClientImage/" + pic;
                    // save the image path path to the database or you can send image
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    file.InputStream.CopyTo(ms);
                    //    byte[] array = ms.GetBuffer();

                    //    //client.ClientImage = array.ToString();
                    //}
                }

                db.Clients.Add(client);
                db.SaveChanges();

                int id = client.ClientId;

                return RedirectToAction("Index");
            }

            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", client.BranchId);
            ViewBag.GenderId = new SelectList(db.Genders, "GenderId", "Gender1", client.GenderId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", client.UserId);
            return View(client);
        }

        //
        // GET: /Client/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", client.BranchId);
            ViewBag.GenderId = new SelectList(db.Genders, "GenderId", "Gender1", client.GenderId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", client.UserId);
            return View(client);
        }

        //
        // POST: /Client/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", client.BranchId);
            ViewBag.GenderId = new SelectList(db.Genders, "GenderId", "Gender1", client.GenderId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", client.UserId);
            return View(client);
        }

        //
        // GET: /Client/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        //
        // POST: /Client/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);

            string fullPath = Request.MapPath(client.ClientImage);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            db.Clients.Remove(client);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}