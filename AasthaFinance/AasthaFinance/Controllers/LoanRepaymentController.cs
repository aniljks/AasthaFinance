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
    public class LoanRepaymentController : Controller
    {
        private AasthaFinanceEntities db = new AasthaFinanceEntities();

        //
        // GET: /LoanRepayment/

        public ActionResult Index()
        {
            var loanrepayments = db.LoanRepayments.Include(l => l.Branch).Include(l => l.Client).Include(l => l.LoanApplication).Include(l => l.LoanCycle).Include(l => l.LoanDisbursement).Include(l => l.LoanEMISchedule).Include(l => l.LoanRepaymentStatu).Include(l => l.User);
            return View(loanrepayments.ToList());
        }

        //
        // GET: /LoanRepayment/Details/5

        public ActionResult Details(int id = 0)
        {
            LoanRepayment loanrepayment = db.LoanRepayments.Find(id);
            if (loanrepayment == null)
            {
                return HttpNotFound();
            }
            return View(loanrepayment);
        }

        //
        // GET: /LoanRepayment/Create

        public ActionResult Create()
        {
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo");
            ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1");
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode");
            ViewBag.LoanEMIScheduletId = new SelectList(db.LoanEMISchedules, "LoanEMIScheduleId", "EMIDate");
            ViewBag.RepaymentStatusId = new SelectList(db.LoanRepaymentStatus, "LoanRepaymentStatusId", "LoanRepaymentStatus");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        //
        // POST: /LoanRepayment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoanRepayment loanrepayment, string Advance)
        {
            if (ModelState.IsValid)
            {
                db.LoanRepayments.Add(loanrepayment);
                db.SaveChanges();

                // Update repay balance in Disbursement table

                decimal amountPaid = db.LoanRepayments.Where(x => x.LoanDisbursementId == loanrepayment.LoanDisbursementId && x.LoanCycleId == loanrepayment.LoanCycleId).Sum(x => x.AmountPaid).Value;

                LoanDisbursement loanDisburseRecord = db.LoanDisbursements.Where(x => x.LoanDisbursementId == loanrepayment.LoanDisbursementId && x.LoanCycleId == loanrepayment.LoanCycleId).FirstOrDefault();
                decimal actualPaidLoanAmt = 0;
                if (loanDisburseRecord != null)
                {
                    actualPaidLoanAmt = loanDisburseRecord.TotalRepayAmountWithInterest.Value;
                    loanDisburseRecord.LoanRepayBalance = loanDisburseRecord.LoanRepayBalance - loanrepayment.AmountPaid.Value;
                }

                decimal remainLoanBalance = actualPaidLoanAmt - amountPaid;

                if (remainLoanBalance <= 0)
                {
                    //Update Status in disbursement when all amount paid 
                    // Change loan status 
                    loanDisburseRecord.LoanStatusId = 2;
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loanrepayment.BranchId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loanrepayment.ClientId);
            ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo", loanrepayment.LoanApplicationId);
            ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1", loanrepayment.LoanCycleId);
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanrepayment.LoanDisbursementId);
            ViewBag.LoanEMIScheduletId = new SelectList(db.LoanEMISchedules, "LoanEMIScheduleId", "EMIDate", loanrepayment.LoanEMIScheduletId);
            ViewBag.RepaymentStatusId = new SelectList(db.LoanRepaymentStatus, "LoanRepaymentStatusId", "LoanRepaymentStatus", loanrepayment.RepaymentStatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loanrepayment.UserId);
            return View(loanrepayment);
        }


        public JsonResult getClient(int branchId)
        {
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

        /// <summary>
        /// Get All Loan Application of a client 
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public JsonResult getClientLoanApplication(int branchId, int clientId)
        {
            var applications = db.LoanApplications.Where(x => x.BranchId == branchId && x.ClientId == clientId).ToList();

            List<SelectListItem> listApplications = new List<SelectListItem>();

            listApplications.Add(new SelectListItem { Text = "--Select Loan Application--", Value = "0" });
            if (applications != null)
            {
                foreach (var item in applications)
                {
                    var loanDisbursed = item.LoanDisbursements.Where(x => x.LoanApplicationId == item.LoanApplicationId).FirstOrDefault();

                    if (loanDisbursed != null)
                    {
                        if (item.LoanApplicationStatu.LoanApplicationStatus == LoanApplicationStatus.Approved.ToString())
                            listApplications.Add(new SelectListItem { Text = item.LoanApplicationNo, Value = item.LoanApplicationId.ToString() });
                    }

                }
            }

            return Json(new SelectList(listApplications, "Value", "Text", JsonRequestBehavior.AllowGet));
        }


        public JsonResult getClientLoanDisbursementCode(int branchId, int clientId, int applicationId)
        {
            var disbursements = db.LoanDisbursements.Where(x => x.BranchId == branchId && x.ClientId == clientId && x.LoanApplicationId == applicationId).ToList();

            List<SelectListItem> listApplications = new List<SelectListItem>();

            listApplications.Add(new SelectListItem { Text = "--Select Disbursement Code--", Value = "0" });
            if (disbursements != null)
            {
                foreach (var item in disbursements)
                {
                    if (item.LoanStatu.LoanStatus == LoanStatus.Active.ToString())
                        listApplications.Add(new SelectListItem { Text = item.DisbursementCode, Value = item.LoanDisbursementId.ToString() });
                }
            }

            return Json(new SelectList(listApplications, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        public JsonResult getDisbursementLoanSchedule(int disbursementId)
        {
            var EMISchedules = db.LoanEMISchedules.Where(x => x.LoanDisbursementId == disbursementId).ToList();

            List<SelectListItem> listOfEMI = new List<SelectListItem>();

            listOfEMI.Add(new SelectListItem { Text = "--Select EMI to repay--", Value = "0" });
            if (EMISchedules != null)
            {
                foreach (var item in EMISchedules)
                {
                    var paidEMI = db.LoanRepayments.Where(x => x.LoanEMIScheduletId == item.LoanEMIScheduleId).FirstOrDefault();
                    if (paidEMI != null)
                    {
                        if (paidEMI.LoanRepaymentStatu.LoanRepaymentStatus.ToString() != LoanRepaymentStatus.Paid.ToString())
                            listOfEMI.Add(new SelectListItem { Text = item.EMIDate.Value.ToShortDateString(), Value = item.LoanEMIScheduleId.ToString() });
                    }
                    else
                    {
                        listOfEMI.Add(new SelectListItem { Text = item.EMIDate.Value.ToShortDateString(), Value = item.LoanEMIScheduleId.ToString() });
                    }
                }
            }

            return Json(new SelectList(listOfEMI, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        //
        // GET: /LoanRepayment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            LoanRepayment loanrepayment = db.LoanRepayments.Find(id);
            if (loanrepayment == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loanrepayment.BranchId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loanrepayment.ClientId);
            ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo", loanrepayment.LoanApplicationId);
            ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1", loanrepayment.LoanCycleId);
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanrepayment.LoanDisbursementId);
            ViewBag.LoanEMIScheduletId = new SelectList(db.LoanEMISchedules, "LoanEMIScheduleId", "EMIDate", loanrepayment.LoanEMIScheduletId);
            ViewBag.RepaymentStatusId = new SelectList(db.LoanRepaymentStatus, "LoanRepaymentStatusId", "LoanRepaymentStatus", loanrepayment.RepaymentStatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loanrepayment.UserId);
            return View(loanrepayment);
        }

        //
        // POST: /LoanRepayment/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoanRepayment loanrepayment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loanrepayment).State = EntityState.Modified;
                db.SaveChanges();

                // Update repay balance in Disbursement table

                decimal amountPaid = db.LoanRepayments.Where(x => x.LoanDisbursementId == loanrepayment.LoanDisbursementId && x.LoanCycleId == loanrepayment.LoanCycleId).Sum(x => x.AmountPaid).Value;

                LoanDisbursement loanDisburseRecord = db.LoanDisbursements.Where(x => x.LoanDisbursementId == loanrepayment.LoanDisbursementId && x.LoanCycleId == loanrepayment.LoanCycleId).FirstOrDefault();
                decimal actualPaidLoanAmt = 0;
                if (loanDisburseRecord != null)
                    actualPaidLoanAmt = loanDisburseRecord.ActualPaidAmount.Value;

                decimal remainLoanBalance = actualPaidLoanAmt - amountPaid;

                if (remainLoanBalance <= 0)
                {
                    //Update Status in disbursement when all amount paid 
                    // Change loan status 
                    loanDisburseRecord.LoanStatusId = 2;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loanrepayment.BranchId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loanrepayment.ClientId);
            ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo", loanrepayment.LoanApplicationId);
            ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1", loanrepayment.LoanCycleId);
            ViewBag.LoanDisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode", loanrepayment.LoanDisbursementId);
            ViewBag.LoanEMIScheduletId = new SelectList(db.LoanEMISchedules, "LoanEMIScheduleId", "EMIDate", loanrepayment.LoanEMIScheduletId);
            ViewBag.RepaymentStatusId = new SelectList(db.LoanRepaymentStatus, "LoanRepaymentStatusId", "LoanRepaymentStatus", loanrepayment.RepaymentStatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loanrepayment.UserId);
            return View(loanrepayment);
        }

        //
        // GET: /LoanRepayment/Delete/5

        public ActionResult Delete(int id = 0)
        {
            LoanRepayment loanrepayment = db.LoanRepayments.Find(id);
            if (loanrepayment == null)
            {
                return HttpNotFound();
            }
            return View(loanrepayment);
        }

        //
        // POST: /LoanRepayment/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoanRepayment loanrepayment = db.LoanRepayments.Find(id);
            db.LoanRepayments.Remove(loanrepayment);
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