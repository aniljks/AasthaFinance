using AasthaFinance.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace AasthaFinance.Controllers.Reports
{
    public class PaymentDueController : Controller
    {

        AasthaFinance.Data.AasthaFinanceEntities db = new Data.AasthaFinanceEntities();

        //
        // GET: /PaymentDue/

        public ActionResult Create(DueDetail dueDetail, int? page = 1)
        {
            //    DueDetail dueDetail = new DueDetail();
            List<DuesList> dueLists = new List<DuesList>();
            decimal totalDue = 0;

            if (dueDetail.ClientId == 0)
            {
                var loanSchedules = db.LoanEMISchedules.Where(x => x.LoanDisbursement.BranchId == dueDetail.BranchId &&
                    x.EMIDate >= dueDetail.StartDate && x.EMIDate <= dueDetail.EndDate);
                //&& x.LoanDisbursement.ClientId == dueDetail.ClientId


                var loanRepayments = db.LoanRepayments.Where(x => x.BranchId == dueDetail.BranchId
                    && x.PaymentDate >= dueDetail.StartDate && x.PaymentDate <= dueDetail.EndDate
                     && x.RepaymentStatusId.Value == 1);
                //&& x.ClientId == dueDetail.ClientId


                foreach (var schedule in loanSchedules)
                {
                    var repayment = loanRepayments.Where(x => x.PaymentDate == schedule.EMIDate && x.LoanEMIScheduletId == schedule.LoanEMIScheduleId).FirstOrDefault();

                    if (repayment == null)
                    {
                        dueLists.Add(new DuesList
                            {
                                Amount = schedule.EMI.Value.ToString(),
                                BranchName = schedule.LoanDisbursement.Branch.BranchName.ToString(),
                                ClientName = schedule.LoanDisbursement.Client.ClientName.ToString(),
                                LoanApplicationCode = schedule.LoanDisbursement.LoanApplication.LoanApplicationNo.ToString(),
                                LoanDisbursementCode = schedule.LoanDisbursement.DisbursementCode.ToString(),
                                ScheduleDate = schedule.ScheduleDate.Value.ToShortDateString(),
                                LoanBalanceAmt = schedule.LoanDisbursement.LoanRepayBalance.Value.ToString(),
                                EMIDate = schedule.EMIDate.Value.ToShortDateString()
                            });

                        totalDue += schedule.EMI.Value;
                    }

                }

                //dueDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == dueDetail.BranchId
                //    && x.PaymentDate >= dueDetail.StartDate && x.PaymentDate <= dueDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(page.Value, 5);


                dueDetail.LoanDues = dueLists.OrderBy(x => x.EMIDate).ToPagedList(page.Value, 5);

                if (dueDetail.LoanDues.Count > 0)
                    dueDetail.isExist = true;
            }
            else
            {
                //For a specific Client
                var loanSchedules = db.LoanEMISchedules.Where(x => x.LoanDisbursement.BranchId == dueDetail.BranchId &&
                    x.EMIDate >= dueDetail.StartDate && x.EMIDate <= dueDetail.EndDate && x.LoanDisbursement.ClientId == dueDetail.ClientId);



                var loanRepayments = db.LoanRepayments.Where(x => x.BranchId == dueDetail.BranchId
                    && x.PaymentDate >= dueDetail.StartDate && x.PaymentDate <= dueDetail.EndDate
                     && x.RepaymentStatusId.Value == 1 && x.ClientId == dueDetail.ClientId);



                foreach (var schedule in loanSchedules)
                {
                    var repayment = loanRepayments.Where(x => x.PaymentDate == schedule.EMIDate && x.LoanEMIScheduletId == schedule.LoanEMIScheduleId).FirstOrDefault();

                    if (repayment == null)
                    {
                        dueLists.Add(new DuesList
                        {
                            Amount = schedule.EMI.Value.ToString(),
                            BranchName = schedule.LoanDisbursement.Branch.BranchName.ToString(),
                            ClientName = schedule.LoanDisbursement.Client.ClientName.ToString(),
                            LoanApplicationCode = schedule.LoanDisbursement.LoanApplication.LoanApplicationNo.ToString(),
                            LoanDisbursementCode = schedule.LoanDisbursement.DisbursementCode.ToString(),
                            ScheduleDate = schedule.ScheduleDate.Value.ToShortDateString(),
                            LoanBalanceAmt = schedule.LoanDisbursement.LoanRepayBalance.Value.ToString(),
                            EMIDate = schedule.EMIDate.Value.ToShortDateString()
                        });

                        totalDue += schedule.EMI.Value;
                    }

                }

                //dueDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == dueDetail.BranchId
                //    && x.PaymentDate >= dueDetail.StartDate && x.PaymentDate <= dueDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(page.Value, 5);


                dueDetail.LoanDues = dueLists.OrderBy(x => x.EMIDate).ToPagedList(page.Value, 5);

                if (dueDetail.LoanDues.Count > 0)
                    dueDetail.isExist = true;
            }

            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            ViewBag.TotalDue = totalDue;
            return View(dueDetail);
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


        public void pdf(DueDetail dueDetail)
        {
            List<DuesList> dueLists = new List<DuesList>();
            decimal totalDue = 0;
            if (dueDetail.ClientId == 0)
            {
                var loanSchedules = db.LoanEMISchedules.Where(x => x.LoanDisbursement.BranchId == dueDetail.BranchId &&
                    x.EMIDate >= dueDetail.StartDate && x.EMIDate <= dueDetail.EndDate);
                //&& x.LoanDisbursement.ClientId == dueDetail.ClientId


                var loanRepayments = db.LoanRepayments.Where(x => x.BranchId == dueDetail.BranchId
                    && x.PaymentDate >= dueDetail.StartDate && x.PaymentDate <= dueDetail.EndDate
                     && x.RepaymentStatusId.Value == 1);
                //&& x.ClientId == dueDetail.ClientId


                foreach (var schedule in loanSchedules)
                {
                    var repayment = loanRepayments.Where(x => x.LoanEMIScheduletId == schedule.LoanEMIScheduleId).FirstOrDefault();
                    //var repayment = loanRepayments.Where(x => x.PaymentDate == schedule.EMIDate && x.LoanEMIScheduletId == schedule.LoanEMIScheduleId).FirstOrDefault();

                    if (repayment == null)
                    {
                        dueLists.Add(new DuesList
                        {
                            Amount = schedule.EMI.Value.ToString(),
                            BranchName = schedule.LoanDisbursement.Branch.BranchName.ToString(),
                            ClientName = schedule.LoanDisbursement.Client.ClientName.ToString(),
                            LoanApplicationCode = schedule.LoanDisbursement.LoanApplication.LoanApplicationNo.ToString(),
                            LoanDisbursementCode = schedule.LoanDisbursement.DisbursementCode.ToString(),
                            ScheduleDate = schedule.ScheduleDate.Value.ToShortDateString(),
                            LoanBalanceAmt = schedule.LoanDisbursement.LoanRepayBalance.Value.ToString(),
                            EMIDate = schedule.EMIDate.Value.ToShortDateString()
                        });

                        totalDue += schedule.EMI.Value;
                    }

                }

                dueDetail.LoanDues = dueLists.OrderBy(x => x.EMIDate).ToPagedList(1, 1000);

                if (dueDetail.LoanDues.Count > 0)
                    CreateCollectedReportForSingleClient(dueDetail);
            }
            else
            {
                //For a specific Client
                var loanSchedules = db.LoanEMISchedules.Where(x => x.LoanDisbursement.BranchId == dueDetail.BranchId &&
                    x.EMIDate >= dueDetail.StartDate && x.EMIDate <= dueDetail.EndDate && x.LoanDisbursement.ClientId == dueDetail.ClientId);



                var loanRepayments = db.LoanRepayments.Where(x => x.BranchId == dueDetail.BranchId
                    && x.PaymentDate >= dueDetail.StartDate && x.PaymentDate <= dueDetail.EndDate
                     && x.RepaymentStatusId.Value == 1 && x.ClientId == dueDetail.ClientId);



                foreach (var schedule in loanSchedules)
                {
                    var repayment = loanRepayments.Where(x => x.PaymentDate == schedule.EMIDate && x.LoanEMIScheduletId == schedule.LoanEMIScheduleId).FirstOrDefault();

                    if (repayment == null)
                    {
                        dueLists.Add(new DuesList
                        {
                            Amount = schedule.EMI.Value.ToString(),
                            BranchName = schedule.LoanDisbursement.Branch.BranchName.ToString(),
                            ClientName = schedule.LoanDisbursement.Client.ClientName.ToString(),
                            LoanApplicationCode = schedule.LoanDisbursement.LoanApplication.LoanApplicationNo.ToString(),
                            LoanDisbursementCode = schedule.LoanDisbursement.DisbursementCode.ToString(),
                            ScheduleDate = schedule.ScheduleDate.Value.ToShortDateString(),
                            LoanBalanceAmt = schedule.LoanDisbursement.LoanRepayBalance.Value.ToString(),
                            EMIDate = schedule.EMIDate.Value.ToShortDateString()
                        });

                        totalDue += schedule.EMI.Value;
                    }

                }

                dueDetail.LoanDues = dueLists.OrderBy(x => x.EMIDate).ToPagedList(1, 1000);

                if (dueDetail.LoanDues.Count > 0)
                    CreateCollectedReportForSingleClient(dueDetail);
            }



            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            RedirectToAction("Create");
        }

        private void CreateCollectedReportForSingleClient(DueDetail dueDetail)
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;
            PdfPTable table = new PdfPTable(8);
            table.TotalWidth = 450f;
            //fix the absolute width of the table
            table.LockedWidth = true;
            //relative col widths in proportions - 1/3 and 2/3
            float[] widths = new float[] { 5f, 5f, 4f, 4f, 5f, 5f, 4f, 4f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;
            //leave a gap before and after the table
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;
            Font bold = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
            Phrase p = new Phrase("Payment Due Report", bold);
            PdfPCell cell = new PdfPCell(p);
            cell.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
            cell.Colspan = 8;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = 1;
            table.AddCell(cell);

            Font b = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD);
            Phrase reportSummary = new Phrase(string.Format("Due between {0} and {1}", dueDetail.StartDate.Value.ToShortDateString(), dueDetail.EndDate.Value.ToShortDateString()), b);
            PdfPCell cellHeading = new PdfPCell(reportSummary);
            cellHeading.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            cellHeading.Colspan = 8;
            cellHeading.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellHeading.VerticalAlignment = 1;
            table.AddCell(cellHeading);


            PdfPCell cellBranchName = new PdfPCell(new Phrase("Branch Name"));
            cellBranchName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellBranchName);

            PdfPCell cellClientName = new PdfPCell(new Phrase("Client Name"));
            cellClientName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellClientName);

            PdfPCell cellLoanAppliCode = new PdfPCell(new Phrase("LoanAppliCode"));
            cellLoanAppliCode.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellLoanAppliCode);

            PdfPCell cellLoanDisburseCode = new PdfPCell(new Phrase("LoanDisburseCode"));
            cellLoanDisburseCode.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellLoanDisburseCode);

            PdfPCell cellEMIDate = new PdfPCell(new Phrase("EMI Date"));
            cellEMIDate.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellEMIDate);

            PdfPCell cellAmountPaid = new PdfPCell(new Phrase("Amount"));
            cellAmountPaid.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellAmountPaid);

            PdfPCell cellPaymentDate = new PdfPCell(new Phrase("Schedule Date"));
            cellPaymentDate.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellPaymentDate);

            PdfPCell cellLoanBalance = new PdfPCell(new Phrase("LoanBalanceAmt"));
            cellLoanBalance.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellLoanBalance);

            decimal totalDue = 0;

            foreach (var item in dueDetail.LoanDues)
            {

                table.AddCell(item.BranchName);

                table.AddCell(item.ClientName);

                table.AddCell(item.LoanApplicationCode);

                table.AddCell(item.LoanDisbursementCode);

                table.AddCell(item.EMIDate);

                table.AddCell(item.Amount);

                table.AddCell(item.ScheduleDate);

                table.AddCell(item.LoanBalanceAmt);

                totalDue += Convert.ToDecimal(item.Amount);
            }

            Font bt = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
            Phrase phraseTotal = new Phrase("Total Due : " + totalDue.ToString(), bt);
            PdfPCell cellTotal = new PdfPCell(phraseTotal);
            cellTotal.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
            cellTotal.Colspan = 8;
            cellTotal.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            cellTotal.VerticalAlignment = 1;
            table.AddCell(cellTotal);


            document.Open();
            document.Add(table);
            document.Add(new Paragraph("(Signature)"));
            document.Add(new Paragraph("Date :" + DateTime.Now.ToString()));
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition", "attachment; filename= " + Server.HtmlEncode("Due.pdf"));
            Response.ContentType = "APPLICATION/pdf";
            Response.BinaryWrite(byteInfo);
        }
    }
}
