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
    public class PreClosingController : Controller
    {
        private AasthaFinanceEntities db = new AasthaFinanceEntities();

        //
        // GET: /PreClosing/

        public ActionResult Index()
        {
            var loanpreclosings = db.LoanPreClosings.Include(l => l.LoanDisbursement).Include(l => l.User);
            return View(loanpreclosings.ToList());
        }

        //
        // GET: /PreClosing/Details/5

        public ActionResult Details(int id = 0)
        {
            LoanPreClosing loanpreclosing = db.LoanPreClosings.Find(id);
            if (loanpreclosing == null)
            {
                return HttpNotFound();
            }
            return View(loanpreclosing);
        }

        //
        // GET: /PreClosing/Create

        public ActionResult Create()
        {
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        //
        // POST: /PreClosing/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoanPreClosing loanpreclosing)
        {
            if (ModelState.IsValid)
            {
                //TODO: Enter in repayment of final entry to close loan 
                LoanRepayment loanrepayment = new LoanRepayment();
                
                var disbursedLoan = db.LoanDisbursements.Where(x => x.LoanDisbursementId == loanpreclosing.LoanDisbursementId).FirstOrDefault();
                if (disbursedLoan != null)
                {

                    //Repayment Entry
                    loanrepayment.BranchId = disbursedLoan.BranchId;
                    loanrepayment.ClientId = disbursedLoan.ClientId;
                    loanrepayment.AmountPaid = loanpreclosing.AmountPaid;
                    loanrepayment.BalanceAmount = loanpreclosing.ActualLoanBalance;
                    loanrepayment.LoanApplicationId = disbursedLoan.LoanApplicationId;
                    loanrepayment.LoanCycleId = disbursedLoan.LoanCycleId;
                    loanrepayment.LoanDisbursementId = disbursedLoan.LoanDisbursementId;

                    var lastEMI = db.LoanEMISchedules.Where(x => x.LoanDisbursementId == disbursedLoan.LoanDisbursementId).OrderByDescending(x => x.EMIDate).FirstOrDefault();
                    if (lastEMI != null)
                        loanrepayment.LoanEMIScheduletId = lastEMI.LoanEMIScheduleId;
                    else
                        loanrepayment.LoanEMIScheduletId = 0;


                    loanrepayment.PaymentDate = DateTime.Now;
                    loanrepayment.RepaymentStatusId = 1; //1 : Paid and 2 : Due
                    loanrepayment.UserId = loanpreclosing.UserId;
                    db.LoanRepayments.Add(loanrepayment);

                    if (disbursedLoan.LoanStatu.LoanStatus.ToString() == LoanStatus.Active.ToString())
                    {
                        disbursedLoan.LoanStatusId = 2;
                        db.SaveChanges();
                    }
                    db.LoanPreClosings.Add(loanpreclosing);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanpreclosing.LoanDisbursementId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loanpreclosing.UserId);
            return View(loanpreclosing);
        }

        //
        // GET: /PreClosing/Edit/5

        public ActionResult Edit(int id = 0)
        {
            LoanPreClosing loanpreclosing = db.LoanPreClosings.Find(id);
            if (loanpreclosing == null)
            {
                return HttpNotFound();
            }
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanpreclosing.LoanDisbursementId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loanpreclosing.UserId);
            return View(loanpreclosing);
        }

        //
        // POST: /PreClosing/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoanPreClosing loanpreclosing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loanpreclosing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanpreclosing.LoanDisbursementId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loanpreclosing.UserId);
            return View(loanpreclosing);
        }

        //
        // GET: /PreClosing/Delete/5

        public ActionResult Delete(int id = 0)
        {
            LoanPreClosing loanpreclosing = db.LoanPreClosings.Find(id);
            if (loanpreclosing == null)
            {
                return HttpNotFound();
            }
            return View(loanpreclosing);
        }

        //
        // POST: /PreClosing/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoanPreClosing loanpreclosing = db.LoanPreClosings.Find(id);
            db.LoanPreClosings.Remove(loanpreclosing);
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