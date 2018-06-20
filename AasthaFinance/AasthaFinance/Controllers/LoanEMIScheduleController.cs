using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AasthaFinance.Data;
using PagedList;
using ReportManagement;

namespace AasthaFinance.Controllers
{
    [Authorize]
    public class LoanEMIScheduleController : Controller
    {
        private AasthaFinanceEntities db = new AasthaFinanceEntities();

        //
        // GET: /LoanEMISchedule/

        //public ActionResult Index()
        //{
        //    var loanemischedules = db.LoanEMISchedules.Include(l => l.LoanDisbursement);
        //    return View(loanemischedules.ToList());
        //}
                

        [HttpGet]
        public ActionResult Index(int page = 1, int pageSize = 50)
        {

            var loanemischedules = db.LoanEMISchedules.Include(l => l.LoanDisbursement).Include(l => l.LoanRepayments);
            List<LoanEMISchedule> lstLoanEMIScheduleDue = new List<LoanEMISchedule>();
            foreach (var item in loanemischedules)
            {
                var loanRepaid = db.LoanRepayments.Where(x => x.LoanEMIScheduletId == item.LoanEMIScheduleId && x.LoanRepaymentStatu.LoanRepaymentStatus == "Paid").FirstOrDefault();
                if (loanRepaid == null)
                {
                    lstLoanEMIScheduleDue.Add(item);
                }
            }

            PagedList<LoanEMISchedule> model = new PagedList<LoanEMISchedule>(lstLoanEMIScheduleDue, page, pageSize);
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode");

            return View(model);

        }


        [HttpPost]
        public ActionResult Index(int page = 1, int pageSize = 50, int LoanDisbursementId = 0)
        {
            if (LoanDisbursementId == 0)
            {
                var loanemischedules = db.LoanEMISchedules.Include(l => l.LoanDisbursement);
                PagedList<LoanEMISchedule> model = new PagedList<LoanEMISchedule>(loanemischedules.ToList(), page, pageSize);
                ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode");

                return View(model);
            }
            else
            {
                var loanemischedules = db.LoanEMISchedules.Include(l => l.LoanDisbursement).Where(x => x.LoanDisbursementId == LoanDisbursementId);
                PagedList<LoanEMISchedule> model = new PagedList<LoanEMISchedule>(loanemischedules.ToList(), page, pageSize);
                ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode");

                return View(model);
            }

        }

            

        //
        // GET: /LoanEMISchedule/Details/5

        public ActionResult Details(int id = 0)
        {
            LoanEMISchedule loanemischedule = db.LoanEMISchedules.Find(id);
            if (loanemischedule == null)
            {
                return HttpNotFound();
            }
            return View(loanemischedule);
        }

        //
        // GET: /LoanEMISchedule/Create

        public ActionResult Create()
        {
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode");
            return View();
        }

        //
        // POST: /LoanEMISchedule/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoanEMISchedule loanemischedule)
        {
            ////if (ModelState.IsValid)
            ////{
            ////    db.LoanEMISchedules.Add(loanemischedule);
            ////    db.SaveChanges();
            ////    return RedirectToAction("Index");
            ////}

            //Generate Reschedule Here 
            #region Regenerate Schedule

            try
            {

                if (ModelState.IsValid)
                {
                    int id = loanemischedule.LoanDisbursementId.HasValue ? loanemischedule.LoanDisbursementId.Value : 0;

                    var existingSchedules = db.LoanEMISchedules.Where(x => x.LoanDisbursementId.Value == id).ToList();
                    foreach (var item in existingSchedules)
                    {
                        db.LoanEMISchedules.Remove(item);
                    }
                    db.SaveChanges();


                    var loandisbursement = db.LoanDisbursements.Where(x => x.LoanDisbursementId == id).FirstOrDefault();
                    //Create Schedule
                    if (loandisbursement != null)
                    {
                        for (int i = 0; i < loandisbursement.TimePeriod; i++)
                        {
                            db.LoanEMISchedules.Add(new LoanEMISchedule
                            {
                                LoanDisbursementId = id,
                                EMIDate = loandisbursement.EMIStartDate.Value.AddDays(i),
                                EMI = loandisbursement.LoanEMI,
                                ScheduleDate = DateTime.Now,
                                Balance = loandisbursement.TotalRepayAmountWithInterest - (loandisbursement.LoanEMI * (i + 1)),
                                PrincipleAmount = loandisbursement.LoanEMI,
                                InterestAmount = 0
                            });
                        }

                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.Message = "EMI re-Schedule failed.";
                    }
                    return RedirectToAction("Index");
                }
            #endregion


                ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanemischedule.LoanDisbursementId);
                return View(loanemischedule);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().Contains("LoanRepayment"))
                {
                    ViewBag.Message = "EMI re-Schedule failed as already repayment started for this Loan.";
                }
                else
                {
                    ViewBag.Message = "EMI re-Schedule failed." + ex.Message + " InnerException: " + ex.InnerException;

                }

                ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanemischedule.LoanDisbursementId);
                return View(loanemischedule);
            }

        }

        //
        // GET: /LoanEMISchedule/Edit/5

        public ActionResult Edit(int id = 0)
        {
            LoanEMISchedule loanemischedule = db.LoanEMISchedules.Find(id);
            if (loanemischedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanemischedule.LoanDisbursementId);
            return View(loanemischedule);
        }

        //
        // POST: /LoanEMISchedule/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoanEMISchedule loanemischedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loanemischedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanemischedule.LoanDisbursementId);
            return View(loanemischedule);
        }

        //
        // GET: /LoanEMISchedule/Delete/5

        public ActionResult Delete(int id = 0)
        {
            LoanEMISchedule loanemischedule = db.LoanEMISchedules.Find(id);
            if (loanemischedule == null)
            {
                return HttpNotFound();
            }
            return View(loanemischedule);
        }

        //
        // POST: /LoanEMISchedule/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoanEMISchedule loanemischedule = db.LoanEMISchedules.Find(id);
            db.LoanEMISchedules.Remove(loanemischedule);
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