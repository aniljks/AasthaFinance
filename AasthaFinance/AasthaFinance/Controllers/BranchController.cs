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

namespace AasthaFinance.Controllers
{
    [Authorize]
    public class BranchController : Controller
    {
        private AasthaFinanceEntities db = new AasthaFinanceEntities();

        //
        // GET: /Branch/
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var branches = db.Branches;
            PagedList<Branch> model = new PagedList<Branch>(branches.ToList(), page, pageSize);
            return View(model);
        }

        //
        // GET: /Branch/Details/5

        public ActionResult Details(int id = 0)
        {
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        //
        // GET: /Branch/Create

        public ActionResult Create()
        {
            ViewBag.BranchCode = GetBranchCode();
            return View();
        }

        private string GetBranchCode()
        {
            //AFLA2018000001
            int maxBranchId = 1;
            if (db.Branches.Count() > 0)
            {
                maxBranchId = db.Branches.Max(x => x.BranchId) + 1;
            }

            return "AFBO" + DateTime.Now.Year.ToString() + Common.AddAdditionalZero(maxBranchId.ToString());
        }

        //
        // POST: /Branch/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Branches.Add(branch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(branch);
        }

        //
        // GET: /Branch/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        //
        // POST: /Branch/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(branch);
        }

        //
        // GET: /Branch/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        //
        // POST: /Branch/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Branch branch = db.Branches.Find(id);
            db.Branches.Remove(branch);
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