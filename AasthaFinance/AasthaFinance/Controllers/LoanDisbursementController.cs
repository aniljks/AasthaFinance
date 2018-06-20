using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AasthaFinance.Data;
using AasthaFinance.Models;
using AasthaFinance.Models.ViewSupportModel;
using AVS.Common.Logger;

namespace AasthaFinance.Controllers
{
    [Authorize]
    public class LoanDisbursementController : Controller
    {
        private AasthaFinanceEntities db = new AasthaFinanceEntities();
        ILogManager ILog = new SyncLogger();
        //
        // GET: /LoanDisbursement/

        public ActionResult Index()
        {
            var loandisbursements = db.LoanDisbursements.Include(l => l.Branch).Include(l => l.Client).Include(l => l.Freequency).Include(l => l.LoanApplication).Include(l => l.LoanCycle).Include(l => l.LoanStatu).Include(l => l.PaymentMode).Include(l => l.User);
            return View(loandisbursements.ToList());
        }

        //
        // GET: /LoanDisbursement/Details/5

        public ActionResult Details(int id = 0)
        {
            LoanDisbursement loandisbursement = db.LoanDisbursements.Find(id);
            if (loandisbursement == null)
            {
                return HttpNotFound();
            }
            return View(loandisbursement);
        }

        //
        // GET: /LoanDisbursement/Create

        public ActionResult Create()
        {
            try
            {
                ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
                ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
                ViewBag.LoanRepayFreequencyId = new SelectList(db.Freequencies, "FreequencyId", "Freequency1");
                ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo");
                ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1");
                ViewBag.LoanStatusId = new SelectList(db.LoanStatus, "LoanStatusId", "LoanStatus");
                ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "PaymentModeId", "PaymentMode1");
                ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
                ViewBag.DisbursementCode = GetLoanDisbursementCode();
                ViewBag.DisbursementDate = DateTime.Now.ToShortDateString();
                return View();
            }
            catch (Exception ex)
            {
                ILog.debug("Message: " + ex.Message + "Inner Exception: " + ex.InnerException + "Stack trace:" + ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        private string GetLoanDisbursementCode()
        {
            //AFLA2018000001
            int maxLoanId = 1;
            if (db.LoanDisbursements.Count() > 0)
                maxLoanId = db.LoanDisbursements.Max(x => x.LoanDisbursementId) + 1;

            return "AFLD" + DateTime.Now.Year.ToString() + Common.AddAdditionalZero(maxLoanId.ToString());
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

                    if (loanDisbursed == null)
                    {
                        if (item.LoanApplicationStatu.LoanApplicationStatus == LoanApplicationStatus.Approved.ToString())
                            listApplications.Add(new SelectListItem { Text = item.LoanApplicationNo, Value = item.LoanApplicationId.ToString() });
                    }

                }
            }

            return Json(new SelectList(listApplications, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        public JsonResult getClientLoanApplicationDetails(int branchId, int clientId, int applicationId)
        {
            LoanApplicationDetail _loanApplicationDetail = new LoanApplicationDetail();
            LoanApplication loanApplication = db.LoanApplications.FirstOrDefault(x => x.BranchId == branchId && x.ClientId == clientId && x.LoanApplicationId == applicationId);
            if (loanApplication != null)
            {

                _loanApplicationDetail.BankAccountNo = loanApplication.BankAccountNo;
                _loanApplicationDetail.BankName = loanApplication.BankName;
                _loanApplicationDetail.BranchName = loanApplication.Branch.BranchName;
                _loanApplicationDetail.ChequeNo = loanApplication.ChequeNo;
                _loanApplicationDetail.ClientName = loanApplication.Client.ClientName;
                _loanApplicationDetail.EMIStartDate = loanApplication.EMIStartDate;
                _loanApplicationDetail.FreequencyId = loanApplication.FreequencyId;
                _loanApplicationDetail.InterestModelId = loanApplication.InterestModelId;
                _loanApplicationDetail.InterestRate = loanApplication.InterestRate;
                _loanApplicationDetail.LoanAmount = loanApplication.LoanAmount;

                return Json(_loanApplicationDetail);
            }
            else
            {
                return Json(_loanApplicationDetail);
            }
        }

        //
        // POST: /LoanDisbursement/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoanDisbursement loandisbursement)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var IsLoanAlreadyDisbursed = db.LoanDisbursements.Where(x => x.LoanApplicationId == loandisbursement.LoanApplicationId &&
                        x.LoanCycleId == loandisbursement.LoanCycleId).FirstOrDefault();

                    if (IsLoanAlreadyDisbursed == null)
                    {
                        if (loandisbursement.EMIStartDate == null)
                            loandisbursement.EMIStartDate = DateTime.Now;

                        if (loandisbursement.DisbursmentDate == null)
                            loandisbursement.DisbursmentDate = DateTime.Now;
                        loandisbursement.LoanRepayBalance = loandisbursement.TotalRepayAmountWithInterest;
                        db.LoanDisbursements.Add(loandisbursement);
                        db.SaveChanges();

                        int id = loandisbursement.LoanDisbursementId;

                        //Create Schedule

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

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loandisbursement.BranchId);
                        ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loandisbursement.ClientId);
                        ViewBag.LoanRepayFreequencyId = new SelectList(db.Freequencies, "FreequencyId", "Freequency1", loandisbursement.LoanRepayFreequencyId);
                        ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo", loandisbursement.LoanApplicationId);
                        ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1", loandisbursement.LoanCycleId);
                        ViewBag.LoanStatusId = new SelectList(db.LoanStatus, "LoanStatusId", "LoanStatus", loandisbursement.LoanStatusId);
                        ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "PaymentModeId", "PaymentMode1", loandisbursement.PaymentModeId);
                        ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loandisbursement.UserId);
                        ViewBag.DisbursementCode = GetLoanDisbursementCode();
                        ViewBag.DisbursementDate = DateTime.Now.ToShortDateString();
                        ViewBag.Message = "This Loan application already disbursed you can change the cycle and procced.";
                        return View(loandisbursement);
                    }
                }

                ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loandisbursement.BranchId);
                ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loandisbursement.ClientId);
                ViewBag.LoanRepayFreequencyId = new SelectList(db.Freequencies, "FreequencyId", "Freequency1", loandisbursement.LoanRepayFreequencyId);
                ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo", loandisbursement.LoanApplicationId);
                ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1", loandisbursement.LoanCycleId);
                ViewBag.LoanStatusId = new SelectList(db.LoanStatus, "LoanStatusId", "LoanStatus", loandisbursement.LoanStatusId);
                ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "PaymentModeId", "PaymentMode1", loandisbursement.PaymentModeId);
                ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loandisbursement.UserId);
                ViewBag.DisbursementCode = GetLoanDisbursementCode();
                ViewBag.DisbursementDate = DateTime.Now.ToShortDateString();
                return View(loandisbursement);
            }
            catch (Exception ex)
            {
                ILog.debug("Message: " + ex.Message + "Inner Exception: " + ex.InnerException + "Stack trace:" + ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /LoanDisbursement/Edit/5

        public ActionResult Edit(int id = 0)
        {
            LoanDisbursement loandisbursement = db.LoanDisbursements.Find(id);
            if (loandisbursement == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loandisbursement.BranchId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loandisbursement.ClientId);
            ViewBag.LoanRepayFreequencyId = new SelectList(db.Freequencies, "FreequencyId", "Freequency1", loandisbursement.LoanRepayFreequencyId);
            ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo", loandisbursement.LoanApplicationId);
            ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1", loandisbursement.LoanCycleId);
            ViewBag.LoanStatusId = new SelectList(db.LoanStatus, "LoanStatusId", "LoanStatus", loandisbursement.LoanStatusId);
            ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "PaymentModeId", "PaymentMode1", loandisbursement.PaymentModeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loandisbursement.UserId);
            return View(loandisbursement);
        }

        //
        // POST: /LoanDisbursement/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoanDisbursement loandisbursement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loandisbursement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode", loandisbursement.BranchId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName", loandisbursement.ClientId);
            ViewBag.LoanRepayFreequencyId = new SelectList(db.Freequencies, "FreequencyId", "Freequency1", loandisbursement.LoanRepayFreequencyId);
            ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo", loandisbursement.LoanApplicationId);
            ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1", loandisbursement.LoanCycleId);
            ViewBag.LoanStatusId = new SelectList(db.LoanStatus, "LoanStatusId", "LoanStatus", loandisbursement.LoanStatusId);
            ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "PaymentModeId", "PaymentMode1", loandisbursement.PaymentModeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", loandisbursement.UserId);
            return View(loandisbursement);
        }

        //
        // GET: /LoanDisbursement/Delete/5

        public ActionResult Delete(int id = 0)
        {
            LoanDisbursement loandisbursement = db.LoanDisbursements.Find(id);
            if (loandisbursement == null)
            {
                return HttpNotFound();
            }
            return View(loandisbursement);
        }

        //
        // POST: /LoanDisbursement/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoanDisbursement loandisbursement = db.LoanDisbursements.Find(id);
            db.LoanDisbursements.Remove(loandisbursement);
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