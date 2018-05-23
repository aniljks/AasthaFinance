using AasthaFinance.Data;
using AasthaFinance.Models;
using AasthaFinance.Models.Report;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AasthaFinance.Controllers.Reports
{
    public class ScheduleReportController : Controller
    {
        AasthaFinance.Data.AasthaFinanceEntities db = new Data.AasthaFinanceEntities();

        //
        // GET: /ScheduleReport/


        [HttpGet]
        public ActionResult PrintSchedule()
        {
            ScheduleReportModel ScheduleReportModel = new ScheduleReportModel();
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo");
            ViewBag.DisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode");
            ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1");

            return View(ScheduleReportModel);
        }

        [HttpPost]
        public ActionResult PrintSchedule(ScheduleReportModel ScheduleReportModel)
        {
            PrepareSchedule(ScheduleReportModel);
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            ViewBag.LoanApplicationId = new SelectList(db.LoanApplications, "LoanApplicationId", "LoanApplicationNo");
            ViewBag.DisbursementId = new SelectList(db.LoanDisbursements, "LoanDisbursementId", "DisbursementCode");
            ViewBag.LoanCycleId = new SelectList(db.LoanCycles, "LoanCycleId", "LoanCycle1");

            return View(ScheduleReportModel);
        }

        private void PrepareSchedule(ScheduleReportModel ScheduleReportModel)
        {
            IEnumerable<GetPrintScheduleByDID_Result> results = db.GetPrintScheduleByDID(
                 Convert.ToInt16(ScheduleReportModel.BranchId),
                 Convert.ToInt16(ScheduleReportModel.ClientId),
                 Convert.ToInt16(ScheduleReportModel.LoanApplicationId),
                 Convert.ToInt16(ScheduleReportModel.DisbursementId),
                 Convert.ToInt16(ScheduleReportModel.LoanCycleId));


            List<GetPrintScheduleByDID_Result> resultsFinalList = results.ToList<GetPrintScheduleByDID_Result>();

            GetPrintScheduleByDID_Result row = new GetPrintScheduleByDID_Result();

            foreach (var item in resultsFinalList)
            {
                row = item;

                break;
            }

            PrintCollectionSchedule(resultsFinalList, new ReportBuilderModel { ReportHeading = "Collection report", ActualPaid = row.ActualPaidAmount.Value.ToString(), ScheduleDate = row.ScheduleDate.Value.ToShortDateString(), BranchName = row.BranchCode + " / " + row.BranchName });

        }


        public void PrintCollectionSchedule(List<GetPrintScheduleByDID_Result> results, ReportBuilderModel model)
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;
            PdfPTable table = new PdfPTable(8);
            table.TotalWidth = 550f;
            //fix the absolute width of the table
            table.LockedWidth = true;
            //relative col widths in proportions - 1/3 and 2/3
            float[] widths = new float[] { 7f, 4f, 2f, 3f, 3f, 3f, 3f, 4f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;
            //leave a gap before and after the table
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;
            Font bold = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
            Phrase p = new Phrase(model.ReportHeading, bold);
            PdfPCell cell = new PdfPCell(p);
            cell.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
            cell.Colspan = 8;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = 1;
            table.AddCell(cell);

            Font b = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD);
            Phrase reportSummary = new Phrase(string.Format("Collection Schedule on : {0}", DateTime.Now.Date.ToShortDateString()), b);
            PdfPCell cellHeading = new PdfPCell(reportSummary);
            cellHeading.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            cellHeading.Colspan = 8;
            cellHeading.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellHeading.VerticalAlignment = 1;
            table.AddCell(cellHeading);

            //Add Branch and Total Amount            
            Phrase branchSummary = new Phrase(string.Format("Branch: {0} Schedule Date : {1} Actual Paid : {2}", model.BranchName, model.ScheduleDate, model.ActualPaid), b);
            PdfPCell cellHeading1 = new PdfPCell(branchSummary);
            cellHeading1.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            cellHeading1.Colspan = 8;
            cellHeading1.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellHeading1.VerticalAlignment = 1;
            table.AddCell(cellHeading1);

            PdfPCell cellBranchName = new PdfPCell(new Phrase("CL Code/Name"));
            cellBranchName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellBranchName);

            PdfPCell cellClientName = new PdfPCell(new Phrase("EMI Date"));
            cellClientName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellClientName);

            PdfPCell cellAmountPaid = new PdfPCell(new Phrase("EMI"));
            cellAmountPaid.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellAmountPaid);

            PdfPCell cellPaymentDate = new PdfPCell(new Phrase("InstallNo"));
            cellPaymentDate.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellPaymentDate);

            PdfPCell cellPrincipleAmount = new PdfPCell(new Phrase("Amt Paid"));
            cellPrincipleAmount.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellPrincipleAmount);

            PdfPCell cellInterestAmount = new PdfPCell(new Phrase("Balance"));
            cellInterestAmount.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellInterestAmount);


            PdfPCell cellSignature = new PdfPCell(new Phrase("Sign"));
            cellSignature.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellSignature);

            PdfPCell cellAdditonalRemark = new PdfPCell(new Phrase("Remark"));
            cellAdditonalRemark.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellAdditonalRemark);


            long? totalInstallment = results.Max(x => x.EMINumber);
            GetPrintScheduleByDID_Result row = results.FirstOrDefault(x => x.ActualPaidAmount != 0);
            decimal? balance = row.ActualPaidAmount;
            decimal? amountPaidTillNow = 0;
            decimal remainBalanceAll = 0;
            foreach (Data.GetPrintScheduleByDID_Result item in results.ToList())
            {

                table.AddCell(item.ClientCode + " / " + item.ClientName);
                                
                table.AddCell(item.EMIDate.Value.ToShortDateString());

                table.AddCell(item.EMI.ToString());

                table.AddCell(item.EMINumber.ToString() + " / " + totalInstallment);

                table.AddCell(item.AmountPaid.ToString());

                amountPaidTillNow += item.AmountPaid;
                decimal remainBalance = balance.Value - amountPaidTillNow.Value;
                remainBalanceAll = remainBalance;
                table.AddCell(remainBalance.ToString());

                table.AddCell(".............");

                table.AddCell(".............");
            }


            //Final Row summary
            table.AddCell(" ");

            table.AddCell(" ");

            table.AddCell("Total");

            table.AddCell(" - ");

            table.AddCell(amountPaidTillNow.ToString());

            table.AddCell(remainBalanceAll.ToString());

            table.AddCell(" ");

            table.AddCell(" ");


            document.Open();
            document.Add(table);
            document.Add(new Paragraph(DateTime.Now.ToString()));
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition", "attachment; filename= " + Server.HtmlEncode(string.Format("CollectionSchedule_{0}.pdf", DateTime.Now.Date.ToShortDateString())));
            Response.ContentType = "APPLICATION/pdf";
            Response.BinaryWrite(byteInfo);
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

                    if (loanDisbursed != null)
                    {
                        if (item.LoanApplicationStatu.LoanApplicationStatus == LoanApplicationStatus.Approved.ToString())
                            listApplications.Add(new SelectListItem { Text = item.LoanApplicationNo, Value = item.LoanApplicationId.ToString() });
                    }

                }
            }

            return Json(new SelectList(listApplications, "Value", "Text", JsonRequestBehavior.AllowGet));
        }


        public JsonResult getClientLoanDisbursedCode(int branchId, int clientId, int loanAppId)
        {
            var applications = db.LoanDisbursements.Where(x => x.BranchId == branchId && x.ClientId == clientId && x.LoanApplicationId == loanAppId).ToList();

            List<SelectListItem> listApplications = new List<SelectListItem>();

            listApplications.Add(new SelectListItem { Text = "--Select Loan Disbursement code--", Value = "0" });
            if (applications != null)
            {
                foreach (var item in applications)
                {

                    listApplications.Add(new SelectListItem { Text = item.DisbursementCode, Value = item.LoanDisbursementId.ToString() });

                }
            }

            return Json(new SelectList(listApplications, "Value", "Text", JsonRequestBehavior.AllowGet));
        }


        public JsonResult getClientLoanCycles(int branchId, int clientId, int loanAppId, int loanDisburseId)
        {
            var applications = db.LoanDisbursements.Where(x => x.BranchId == branchId && x.ClientId == clientId && x.LoanApplicationId == loanAppId && x.LoanDisbursementId == loanDisburseId).ToList();

            List<SelectListItem> listApplications = new List<SelectListItem>();

            listApplications.Add(new SelectListItem { Text = "--Select Loan Cycle--", Value = "0" });
            if (applications != null)
            {
                var uniqueApps = applications.Distinct();
                foreach (var item in uniqueApps)
                {
                    listApplications.Add(new SelectListItem { Text = item.LoanCycle.LoanCycle1, Value = item.LoanCycleId.ToString() });

                }
            }

            return Json(new SelectList(listApplications, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        #region Commented
        //public void ReportBuilder(ReportBuilderModel model)
        //{
        //    MemoryStream workStream = new MemoryStream();
        //    Document document = new Document();
        //    PdfWriter.GetInstance(document, workStream).CloseStream = false;
        //    PdfPTable table = new PdfPTable(10);
        //    table.TotalWidth = 550f;
        //    //fix the absolute width of the table
        //    table.LockedWidth = true;
        //    //relative col widths in proportions - 1/3 and 2/3
        //    float[] widths = new float[] { 5f, 4f, 4f, 4f, 2f, 3f, 3f, 4f, 3f, 4f };
        //    table.SetWidths(widths);
        //    table.HorizontalAlignment = 0;
        //    //leave a gap before and after the table
        //    table.SpacingBefore = 10f;
        //    table.SpacingAfter = 10f;
        //    Font bold = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
        //    Phrase p = new Phrase(model.ReportHeading, bold);
        //    PdfPCell cell = new PdfPCell(p);
        //    cell.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
        //    cell.Colspan = 10;
        //    cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
        //    cell.VerticalAlignment = 1;
        //    table.AddCell(cell);

        //    Font b = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD);
        //    Phrase reportSummary = new Phrase(string.Format("Date : {0}", DateTime.Now.Date.ToShortDateString()), b);
        //    PdfPCell cellHeading = new PdfPCell(reportSummary);
        //    cellHeading.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    cellHeading.Colspan = 10;
        //    cellHeading.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
        //    cellHeading.VerticalAlignment = 1;
        //    table.AddCell(cellHeading);



        //    PdfPCell cellBranchName = new PdfPCell(new Phrase("CL Code"));
        //    cellBranchName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellBranchName);

        //    PdfPCell cellClientName = new PdfPCell(new Phrase("CL Name"));
        //    cellClientName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellClientName);

        //    PdfPCell cellAmountPaid = new PdfPCell(new Phrase("Contact"));
        //    cellAmountPaid.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellAmountPaid);

        //    PdfPCell cellPaymentDate = new PdfPCell(new Phrase("EMI Date"));
        //    cellPaymentDate.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellPaymentDate);

        //    PdfPCell cellEMI = new PdfPCell(new Phrase("EMI"));
        //    cellEMI.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellEMI);

        //    PdfPCell cellPrincipleAmount = new PdfPCell(new Phrase("Amt"));
        //    cellPrincipleAmount.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellPrincipleAmount);

        //    PdfPCell cellInterestAmount = new PdfPCell(new Phrase("Interst Amt"));
        //    cellInterestAmount.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellInterestAmount);

        //    PdfPCell cellBalance = new PdfPCell(new Phrase("Balance"));
        //    cellBalance.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellBalance);

        //    //PdfPCell cellNotes = new PdfPCell(new Phrase("Notes"));
        //    //cellNotes.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    //table.AddCell(cellNotes);

        //    PdfPCell cellSignature = new PdfPCell(new Phrase("Signature"));
        //    cellSignature.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellSignature);

        //    PdfPCell cellAdditonalRemark = new PdfPCell(new Phrase("Remark"));
        //    cellAdditonalRemark.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
        //    table.AddCell(cellAdditonalRemark);


        //    //table.AddCell("Branch Name");               

        //    var collectionDetails = db.GetCollectionRecipt(collectionDetail.BranchId).ToList<Data.GetCollectionRecipt_Result>();

        //    foreach (Data.GetCollectionRecipt_Result item in collectionDetails)
        //    {

        //        table.AddCell(item.ClientCode);

        //        table.AddCell(item.ClientName);

        //        table.AddCell(item.PersonalContact);

        //        table.AddCell(item.EMIDate.Value.ToShortDateString());

        //        table.AddCell(item.EMI.ToString());

        //        table.AddCell(item.PrincipleAmount.ToString());

        //        table.AddCell(item.InterestAmount.ToString());

        //        table.AddCell(item.Balance.ToString());

        //        //table.AddCell(item.Notes.ToString());

        //        table.AddCell(item.Signature.ToString());

        //        table.AddCell(item.Additonal_Remark);

        //    }

        //    document.Open();
        //    document.Add(table);
        //    document.Add(new Paragraph(DateTime.Now.ToString()));
        //    document.Close();

        //    byte[] byteInfo = workStream.ToArray();
        //    workStream.Write(byteInfo, 0, byteInfo.Length);
        //    workStream.Position = 0;
        //    Response.Buffer = true;
        //    Response.AddHeader("Content-Disposition", "attachment; filename= " + Server.HtmlEncode(string.Format("CollectionSchedule_{0}.pdf", DateTime.Now.Date.ToShortDateString())));
        //    Response.ContentType = "APPLICATION/pdf";
        //    Response.BinaryWrite(byteInfo);
        //}
        #endregion
    }
}
