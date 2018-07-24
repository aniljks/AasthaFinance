using AasthaFinance.Data;
using AasthaFinance.Models;
using AVS.Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace AasthaFinance.Controllers
{
    [Authorize]
    public class DashBoardController : BaseController
    {
        private AasthaFinanceEntities db = new AasthaFinanceEntities();
        AVS.Common.Logger.ILogManager ILog = new SyncLogger();


        public JsonResult GetDailyCollectionForChart()
        {
            string[] str = new string[13] { "Apr 1", "Mar 2", "Mar 3", "Mar 4", "Mar 5", "Mar 6", "Mar 7", "Mar 8", "Mar 9", "Mar 10", "Mar 12", "Mar 12", "Mar 13" };
            //return Json(str);

            return Json(new JsonResult()
            {
                Data = str
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMonthLabelsForChart()
        {
            string[] str = new string[6] { "January", "February", "March", "April", "May", "June" };
            //return Json(str);

            return Json(new JsonResult()
            {
                Data = str
            }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /DashBoard/

        public ActionResult Index()
        {




            DashBoardDetail dashBoardDetail = new DashBoardDetail();
            try
            {

                //DashBoardDetail dashBoardDetail = new DashBoardDetail();

                DateTime localDateTime = DateTime.Parse(DateTime.Now.Date.ToShortDateString());
                DateTime utcDateTime = localDateTime.ToUniversalTime();

                #region Total Collection of the day
                var repayments = db.LoanRepayments.Where(x => x.LoanDisbursement.LoanStatu.LoanStatus == "Active");
                decimal totalCollection = 0;
                foreach (var item in repayments)
                {
                    DateTime PaymentDatelocalDateTime = DateTime.Parse(item.PaymentDate.Value.Date.ToShortDateString());
                    DateTime paymentUtcDateTime = PaymentDatelocalDateTime.ToUniversalTime();

                    if (paymentUtcDateTime.ToShortDateString() == utcDateTime.ToShortDateString())
                    {
                        totalCollection = totalCollection + item.AmountPaid.Value;
                    }

                }
                dashBoardDetail.TotalCollectionOfDay = totalCollection;

                #endregion

                #region Total Collected of the day
                var repaymentsDay = db.LoanRepayments.Where(x => x.LoanDisbursement.LoanStatu.LoanStatus == "Active" && x.LoanRepaymentStatu.LoanRepaymentStatus == "Paid");
                decimal totalCollected = 0;
                foreach (var item in repaymentsDay)
                {

                    DateTime PaymentDatelocalDateTime = DateTime.Parse(item.PaymentDate.Value.Date.ToShortDateString());
                    DateTime paymentUtcDateTime = PaymentDatelocalDateTime.ToUniversalTime();

                    if (paymentUtcDateTime.ToShortDateString() == utcDateTime.ToShortDateString())
                    {
                        totalCollected = totalCollected + item.AmountPaid.Value;
                    }

                }
                dashBoardDetail.TotalCollectedOfDay = totalCollected;

                #endregion


                #region Total Dues of the day

                var dues = db.LoanEMISchedules.Where(x => x.LoanDisbursement.LoanStatu.LoanStatus == "Active");
                decimal totalDues = 0;
                foreach (var item in dues)
                {
                    DateTime PaymentDatelocalDateTime = DateTime.Parse(item.EMIDate.Value.Date.ToShortDateString());
                    DateTime paymentUtcDateTime = PaymentDatelocalDateTime.ToUniversalTime();

                    if (paymentUtcDateTime.ToShortDateString() == utcDateTime.ToShortDateString())

                        if (paymentUtcDateTime.ToShortDateString() == utcDateTime.ToShortDateString())
                            totalDues = totalDues + item.EMI.Value;
                }
                dashBoardDetail.TotalDueOfDay = totalDues - totalCollected;

                #endregion


                #region Active Loans

                dashBoardDetail.TotalActiveLoan = db.LoanDisbursements.Where(x => x.LoanStatu.LoanStatus == "Active").Count();

                #endregion


                #region Revenue Calculation
                var totalRevenue = db.LoanDisbursements.Sum(x => x.ProposedAmount);
                if (totalRevenue != null)
                    dashBoardDetail.Revenue = totalRevenue.Value;

                #endregion
                string[] str = new string[13] { "Apr 1", "Mar 2", "Mar 3", "Mar 4", "Mar 5", "Mar 6", "Mar 7", "Mar 8", "Mar 9", "Mar 10", "Mar 12", "Mar 12", "Mar 13" };

                string monthNames = @"""Apr 1""," + @"""Mar 2""," + @"""Mar 3""," + @"""Mar 4"""; // "Mar 5", "Mar 6", "Mar 7", "Mar 8", "Mar 9", "Mar 10", "Mar 12", "Mar 12", "Mar 13";

                ViewBag.DateListOfCurrentFortNight = monthNames;


                return View(dashBoardDetail);
            }
            catch (Exception ex)
            {
                ILog.debug(ex.Message);
                return View(dashBoardDetail);
            }
        }

        public ActionResult TestGraph()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerateReport(int firstBranch)
        {
            ReportData reportData = new ReportData();


            int totalCount = 5;

            reportData.labels = new String[] { "January", "February", "March", "April", "May", "June", "July", "August", "Sept", "Oct", "Nov", "Dec" };
            reportData.dataset1 = new Int32[reportData.labels.Count()];
            int k = 9;
            for (int index = 0; index < totalCount; index++)
            {

                reportData.dataset1[index] = k * index + 4;

            }

            reportData.dataset1name = "NBH";
            //return Newtonsoft.Json.JsonConvert.SerializeObject(reportData);
            return Json(reportData);
        }

        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToActionPermanent("Index", "Account");
        }

        //
        // GET: /DashBoard/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DashBoard/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DashBoard/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DashBoard/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DashBoard/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DashBoard/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DashBoard/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
