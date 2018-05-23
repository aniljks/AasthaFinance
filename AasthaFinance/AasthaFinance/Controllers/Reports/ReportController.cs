using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReportManagement;
using AasthaFinance.Models;
using AasthaFinance.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace AasthaFinance.Controllers.Reports
{
    public class ReportController : PdfViewController //Controller
    {

        AasthaFinance.Data.AasthaFinanceEntities db = new Data.AasthaFinanceEntities();
        //
        // GET: /Report/

        public ActionResult Index()
        {
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            return View();
        }

        [HttpPost]
        public ActionResult Index(BranchProgressReportModel model)
        {
            if (ModelState.IsValid)
            {
                GetBranchProgress(model);
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            return View();
        }

        public void GetBranchProgress(BranchProgressReportModel branchProgressReportModel)
        {

            //return ViewPdf("Branch Progress Report", "Index", null);
            int branchId = branchProgressReportModel.BranchId;



            #region Total Client & Active Clients (No Date Filter)
            int totalClient = db.Clients.Count(x => x.BranchId == branchId);
            int totalActiveClient = 0;
            var activeClients = db.LoanDisbursements.Where(x => x.LoanStatu.LoanStatus == "Active" && x.BranchId == branchId);

            var uniuqeClient = from m in activeClients
                               group m by m.ClientId into g
                               select new { g.Key };

            totalActiveClient = uniuqeClient.Count();
            #endregion

            BranchProgressReport model = new BranchProgressReport();
            model.TotalClient = totalClient;
            model.TotalActiveClient = totalActiveClient;

            model.TotalDisbursedLoan = db.LoanDisbursements.Where(x => x.BranchId == branchId
                && x.LoanStatu.LoanStatus == "Active"
                && x.DisbursmentDate >= branchProgressReportModel.StartDate
                && x.DisbursmentDate <= branchProgressReportModel.EndDate
                ).Count();


            model.TotalDisputedLoan = db.LoanDisbursements.Where(x => x.BranchId == branchId
                && x.LoanStatu.LoanStatus == "Disputed"
                && x.DisbursmentDate >= branchProgressReportModel.StartDate
                && x.DisbursmentDate <= branchProgressReportModel.EndDate
                ).Count();

            var loanAmountDisbursed = db.LoanDisbursements.Where(x => x.BranchId == branchId
                && x.LoanStatu.LoanStatus == "Active"
                && x.DisbursmentDate >= branchProgressReportModel.StartDate
                && x.DisbursmentDate <= branchProgressReportModel.EndDate
                ).Sum(x => x.ActualPaidAmount);

            if (loanAmountDisbursed.HasValue)
                model.TotalLoanAmountDisbursed = loanAmountDisbursed.Value;


            var loanDisbursed = db.LoanDisbursements.Where(x => x.BranchId == branchId
                && x.LoanStatu.LoanStatus == "Active"
                && x.DisbursmentDate >= branchProgressReportModel.StartDate
                && x.DisbursmentDate <= branchProgressReportModel.EndDate
                ).ToList();

            decimal totalCollection = 0;
            decimal totalCollectionDues = 0;
            foreach (var item in loanDisbursed)
            {
                totalCollectionDues += (db.LoanEMISchedules.Where(x => x.LoanDisbursementId == item.LoanDisbursementId
                    && x.EMIDate >= branchProgressReportModel.StartDate
                    && x.EMIDate <= branchProgressReportModel.EndDate).Sum(x => x.EMI)).HasValue ? (db.LoanEMISchedules.Where(x => x.LoanDisbursementId == item.LoanDisbursementId
                    && x.EMIDate >= branchProgressReportModel.StartDate
                    && x.EMIDate <= branchProgressReportModel.EndDate).Sum(x => x.EMI)).Value : 0;


                totalCollection += (db.LoanRepayments.Where(x => x.LoanDisbursementId == item.LoanDisbursementId
                   && x.LoanRepaymentStatu.LoanRepaymentStatus == "Paid"
                && x.PaymentDate >= branchProgressReportModel.StartDate
                && x.PaymentDate <= branchProgressReportModel.EndDate
                ).Sum(x => x.AmountPaid)).HasValue ? (db.LoanRepayments.Where(x => x.LoanDisbursementId == item.LoanDisbursementId
                   && x.LoanRepaymentStatu.LoanRepaymentStatus == "Paid"
                && x.PaymentDate >= branchProgressReportModel.StartDate
                && x.PaymentDate <= branchProgressReportModel.EndDate
                ).Sum(x => x.AmountPaid)).Value : 0;
            }

            model.TotalCollection = totalCollection;
            model.TotalDue = totalCollectionDues;
            model.TotalDueRemaining = totalCollectionDues - model.TotalCollection;

            model.StartDate = branchProgressReportModel.StartDate.ToShortDateString();
            model.EndDate = branchProgressReportModel.EndDate.ToShortDateString();
            model.BranchName = db.Branches.FirstOrDefault(x => x.BranchId == branchProgressReportModel.BranchId).BranchName.ToString();

            DownloadPdf(model);
            //return this.ViewPdf("", "GetBranchProgress", model);
        }

        //ReportManagement.HtmlViewRenderer htmlViewRenderer = new HtmlViewRenderer();
        //StandardPdfRenderer standardPdfRenderer = new StandardPdfRenderer();

        //protected FileContentResult ViewPdf(string pageTitle, string viewName, object model)
        //{
        //    string htmlText = htmlViewRenderer.RenderViewToString(this, viewName, model);
        //    byte[] buffer = standardPdfRenderer.Render(htmlText, pageTitle);
        //    return File(buffer, "application/pdf", "file.pdf");
        //}


        public void DownloadPdf(BranchProgressReport model)
        {
            #region commented code
            //TODO: Increase the number of records in pdf we need to work
            ////int branchId = branchProgressReportModel.BranchId;

            ////#region Total Client & Active Clients (No Date Filter)
            ////int totalClient = db.Clients.Count(x => x.BranchId == branchId);
            ////int totalActiveClient = 0;
            ////var activeClients = db.LoanDisbursements.Where(x => x.LoanStatu.LoanStatus == "Active" && x.BranchId == branchId);

            ////var uniuqeClient = from m in activeClients
            ////                   group m by m.ClientId into g
            ////                   select new { g.Key };

            ////totalActiveClient = uniuqeClient.Count();
            ////#endregion

            ////BranchProgressReport model = new BranchProgressReport();
            ////model.TotalClient = totalClient;
            ////model.TotalActiveClient = totalActiveClient;

            ////model.TotalDisbursedLoan = db.LoanDisbursements.Where(x => x.BranchId == branchId
            ////    && x.LoanStatu.LoanStatus == "Active"
            ////    && x.DisbursmentDate >= branchProgressReportModel.StartDate
            ////    && x.DisbursmentDate <= branchProgressReportModel.EndDate
            ////    ).Count();


            ////model.TotalDisputedLoan = db.LoanDisbursements.Where(x => x.BranchId == branchId
            ////    && x.LoanStatu.LoanStatus == "Disputed"
            ////    && x.DisbursmentDate >= branchProgressReportModel.StartDate
            ////    && x.DisbursmentDate <= branchProgressReportModel.EndDate
            ////    ).Count();

            ////var loanAmountDisbursed = db.LoanDisbursements.Where(x => x.BranchId == branchId
            ////    && x.LoanStatu.LoanStatus == "Active"
            ////    && x.DisbursmentDate >= branchProgressReportModel.StartDate
            ////    && x.DisbursmentDate <= branchProgressReportModel.EndDate
            ////    ).Sum(x => x.TotalRepayAmountWithInterest);

            ////if (loanAmountDisbursed.HasValue)
            ////    model.TotalLoanAmountDisbursed = loanAmountDisbursed.Value;


            ////var totalCollection = db.LoanRepayments.Where(x => x.BranchId == branchId
            ////    && x.LoanRepaymentStatu.LoanRepaymentStatus == "Paid"
            ////    && x.PaymentDate >= branchProgressReportModel.StartDate
            ////    && x.PaymentDate <= branchProgressReportModel.EndDate
            ////    ).Sum(x => x.AmountPaid);

            ////if (totalCollection.HasValue)
            ////    model.TotalCollection = totalCollection.Value;

            ////model.TotalDue = model.TotalLoanAmountDisbursed - model.TotalCollection;
            #endregion


            MemoryStream workStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 400f;
            //fix the absolute width of the table
            table.LockedWidth = true;
            //relative col widths in proportions - 1/3 and 2/3
            float[] widths = new float[] { 8f, 8f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;
            //leave a gap before and after the table
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;
            //Font regular = new Font(FontFamily.HELVETICA, 12);
            Font bold = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
            Phrase p = new Phrase("Branch Progress Report", bold);
            //p.add(new Chunk(CC_CUST_NAME, regular));
            //PdfPCell cell = new PdfPCell(p);

            //PdfPCell cell = new PdfPCell(new Phrase("Collection report details"));
            PdfPCell cell = new PdfPCell(p);
            cell.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
            cell.Colspan = 2;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = 1;
            table.AddCell(cell);

            Phrase heading = new Phrase(string.Format("Branch : {0}  Report From :{1} To : {2}", model.BranchName, model.StartDate, model.EndDate));
            PdfPCell reportParameterCell = new PdfPCell(heading);
            reportParameterCell.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
            reportParameterCell.Colspan = 2;
            reportParameterCell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            reportParameterCell.VerticalAlignment = 1;
            table.AddCell(reportParameterCell);

            PdfPCell cellBranchName = new PdfPCell(new Phrase("Item"));
            cellBranchName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellBranchName);

            PdfPCell cellClientName = new PdfPCell(new Phrase("No #"));
            cellClientName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellClientName);

            table.AddCell("Total Active Client");
            table.AddCell(model.TotalActiveClient.ToString());

            table.AddCell("Total Client");
            table.AddCell(model.TotalClient.ToString());

            table.AddCell("Total Disbursed Loan");
            table.AddCell(model.TotalDisbursedLoan.ToString());

            table.AddCell("Total Disputed Loan");
            table.AddCell(model.TotalDisputedLoan.ToString());

            table.AddCell("Total Loan Amount Disbursed");
            table.AddCell(model.TotalLoanAmountDisbursed.ToString());

            table.AddCell("Total Collection");
            table.AddCell(model.TotalCollection.ToString());

            table.AddCell("Total Due");
            table.AddCell(model.TotalDue.ToString());

            table.AddCell("Total Due Remaining");
            table.AddCell(model.TotalDueRemaining.ToString());


            //table.AddCell("Col 1 Row 2");            

            document.Open();
            document.Add(table);
            //document.Add(new Paragraph("Hello World"));
            document.Add(new Paragraph(DateTime.Now.ToString()));
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition", "attachment; filename= " + Server.HtmlEncode("BranchProgressReport.pdf"));
            Response.ContentType = "APPLICATION/pdf";
            Response.BinaryWrite(byteInfo);
            //return new FileStreamResult(workStream, "application/pdf");

        }
    }
}
