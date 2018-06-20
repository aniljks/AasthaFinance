using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AasthaFinance.Data;
using AasthaFinance.Models;

namespace AasthaFinance.Controllers
{
    [Authorize]
    public class LoanApplicationController : Controller
    {
        private AasthaFinanceEntities db = new AasthaFinanceEntities();

        //
        // GET: /LoanApplication/

        public ActionResult Index()
        {
            var loanapplications = db.LoanApplications.Include(l => l.Branch).Include(l => l.Client).Include(l => l.Freequency).Include(l => l.InterestModel).Include(l => l.LoanApplicationStatu).Include(l => l.User);
            return View(loanapplications.ToList());
        }

        //
        // GET: /LoanApplication/Details/5

        public ActionResult Details(int id = 0)
        {
            LoanApplication loanapplication = db.LoanApplications.Find(id);
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            return View(loanapplication);
        }

        //
        // GET: /LoanApplication/Create

        public ActionResult Create()
        {
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            ViewBag.FreequencyId = new SelectList(db.Freequencies, "FreequencyId", "Freequency1");
            ViewBag.InterestModelId = new SelectList(db.InterestModels, "InterestModelId", "interestModel1");
            ViewBag.LoanApplicationStatusId = new SelectList(db.LoanApplicationStatus, "LoanApplicationStatusId", "LoanApplicationStatus");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
            ViewBag.LaonApplicationCode = GetLoanAppCode();
            return View();
        }

        private string GetLoanAppCode()
        {
            //AFLA2018000001
            int maxLoanId = 1;
            if (db.LoanApplications.Count() > 0)
                maxLoanId = db.LoanApplications.Max(x => x.LoanApplicationId) + 1;

            return "AFLA" + DateTime.Now.Year.ToString() + Common.AddAdditionalZero(maxLoanId.ToString());
        }

        public JsonResult getClient(int branchId)
        {
            //ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            var clients = db.Clients.Where(x => x.BranchId == branchId).ToList();
            List<SelectListItem> listClients = new List<SelectListItem>();

            listClients.Add(new SelectListItem { Text = "--Select Client--", Value = "0" });
            if (clients != null)
            {
                foreach (var x in clients)
                {
                    listClients.Add(new SelectListItem { Text = x.ClientName, Value = x.ClientId.ToString() });
                }
            }

            return Json(new SelectList(listClients, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        //
        // POST: /LoanApplication/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoanApplication loanapplication)
        {
            if (ModelState.IsValid)
            {
                db.LoanApplications.Add(loanapplication);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loanapplication.BranchId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loanapplication.ClientId);
            ViewBag.FreequencyId = new SelectList(db.Freequencies, "FreequencyId", "Freequency1", loanapplication.FreequencyId);
            ViewBag.InterestModelId = new SelectList(db.InterestModels, "InterestModelId", "interestModel1", loanapplication.InterestModelId);
            ViewBag.LoanApplicationStatusId = new SelectList(db.LoanApplicationStatus, "LoanApplicationStatusId", "LoanApplicationStatus", loanapplication.LoanApplicationStatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loanapplication.UserId);
            ViewBag.LaonApplicationCode = GetLoanAppCode();
            return View(loanapplication);
        }

        //
        // GET: /LoanApplication/Edit/5

        public ActionResult Edit(int id = 0)
        {
            LoanApplication loanapplication = db.LoanApplications.Find(id);
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loanapplication.BranchId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loanapplication.ClientId);
            ViewBag.FreequencyId = new SelectList(db.Freequencies, "FreequencyId", "Freequency1", loanapplication.FreequencyId);
            ViewBag.InterestModelId = new SelectList(db.InterestModels, "InterestModelId", "interestModel1", loanapplication.InterestModelId);
            ViewBag.LoanApplicationStatusId = new SelectList(db.LoanApplicationStatus, "LoanApplicationStatusId", "LoanApplicationStatus", loanapplication.LoanApplicationStatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loanapplication.UserId);
            ViewBag.EMIStartDate = loanapplication.EMIStartDate.Value;
            ViewBag.LoanApplicationDate = loanapplication.LoanApplicationDate.Value;

            return View(loanapplication);
        }

        //
        // POST: /LoanApplication/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoanApplication loanapplication)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loanapplication).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loanapplication.BranchId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loanapplication.ClientId);
            ViewBag.FreequencyId = new SelectList(db.Freequencies, "FreequencyId", "Freequency1", loanapplication.FreequencyId);
            ViewBag.InterestModelId = new SelectList(db.InterestModels, "InterestModelId", "interestModel1", loanapplication.InterestModelId);
            ViewBag.LoanApplicationStatusId = new SelectList(db.LoanApplicationStatus, "LoanApplicationStatusId", "LoanApplicationStatus", loanapplication.LoanApplicationStatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loanapplication.UserId);
            return View(loanapplication);
        }

        //
        // GET: /LoanApplication/Delete/5

        public ActionResult Delete(int id = 0)
        {
            LoanApplication loanapplication = db.LoanApplications.Find(id);
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            return View(loanapplication);
        }

        //
        // POST: /LoanApplication/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                LoanApplication loanapplication = db.LoanApplications.Find(id);
                db.LoanApplications.Remove(loanapplication);
                db.SaveChanges();
                ViewBag.Message = "Deleted Successfully!";
                ViewBag.isDeleted = true;

                TempData["isDeleted"] = true;
                TempData["Message"] = "Deleted Successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.isDeleted = false;
                ViewBag.Message = ex.Message;
                TempData["isDeleted"] = false;
                TempData["Message"] = "Record not deleted ! " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}