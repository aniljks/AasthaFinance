using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using AasthaFinance.Models.Report;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
//using System.Drawing;

namespace AasthaFinance.Controllers.Reports
{
    public class CollectionController : Controller
    {

        AasthaFinance.Data.AasthaFinanceEntities db = new Data.AasthaFinanceEntities();

        //
        // GET: /Collection/        
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in db.LoanRepayments
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Client.ClientName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Notes);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.PaymentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.PaymentDate);
                    break;
                default:  // Name ascending 
                    students = students.OrderBy(s => s.Notes);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }


        //public ViewResult GetCollectionDetail()
        //{
        //    ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
        //    ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
        //    return View();
        //}

        /// <summary>
        /// This Action is used to display collection details report
        /// </summary>
        /// <param name="collectionDetail"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ViewResult GetCollectionDetail(CollectionDetail collectionDetail, int? page = 1)
        {
            if (collectionDetail.ClientId != 0)
            {
                collectionDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == collectionDetail.BranchId
                    && x.ClientId == collectionDetail.ClientId && x.PaymentDate >= collectionDetail.StartDate && x.PaymentDate <= collectionDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(page.Value, 15);
            }
            else
            {
                collectionDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == collectionDetail.BranchId
                    && x.PaymentDate >= collectionDetail.StartDate && x.PaymentDate <= collectionDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(page.Value, 15);
            }
            if (collectionDetail.LoanRepayments.Count > 0)
                collectionDetail.isExist = true;


            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            return View(collectionDetail);
        }

        [HttpGet]
        public ViewResult GetCollectioninputDetail()
        {
            //if (collectionDetail.ClientId != 0)
            //{
            //    collectionDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == collectionDetail.BranchId
            //        && x.ClientId == collectionDetail.ClientId && x.PaymentDate >= collectionDetail.StartDate && x.PaymentDate <= collectionDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(page.Value, 15);
            //}
            //else
            //{
            //    collectionDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == collectionDetail.BranchId
            //        && x.PaymentDate >= collectionDetail.StartDate && x.PaymentDate <= collectionDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(page.Value, 15);
            //}
            //if (collectionDetail.LoanRepayments.Count > 0)
            //{
            //    collectionDetail.isExist = true;             
            //}
                        
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            //return View(collectionDetail);
            return View();
        }

        [HttpPost]
        public ViewResult GetCollectioninputDetail(CollectionDetail collectionDetail, int? page = 1)
        {
            if (collectionDetail.ClientId != 0)
            {
                collectionDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == collectionDetail.BranchId
                    && x.ClientId == collectionDetail.ClientId && x.PaymentDate >= collectionDetail.StartDate && x.PaymentDate <= collectionDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(page.Value, 15);
            }
            else
            {
                collectionDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == collectionDetail.BranchId
                    && x.PaymentDate >= collectionDetail.StartDate && x.PaymentDate <= collectionDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(page.Value, 15);
            }
            if (collectionDetail.LoanRepayments.Count > 0)
            {
                //collectionDetail.isExist = true;
                pdf(collectionDetail);
            }



            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            return View(collectionDetail);
        }

        /// <summary>
        /// This Action is used for Due Details report
        /// </summary>
        /// <param name="collectionDetail"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ViewResult GetDuesDetail(CollectionDetail collectionDetail, int? page = 1)
        {

            collectionDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == collectionDetail.BranchId
                && x.ClientId == collectionDetail.ClientId && x.PaymentDate >= collectionDetail.StartDate && x.PaymentDate <= collectionDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(page.Value, 5);

            if (collectionDetail.LoanRepayments.Count > 0)
                collectionDetail.isExist = true;

            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            return View(collectionDetail);

            //           SELECT TOP 1000
            //C.PersonalContact       
            //     ,[EMIDate]
            //     ,[EMI]
            //     ,[PrincipleAmount]
            //     ,[InterestAmount]
            //     ,[Balance]  
            //     ,LA.Notes
            // FROM [AasthaFinance].[dbo].[LoanEMISchedule] LES
            // inner join LoanDisbursement LD on LES.LoanDisbursementId=LD.LoanDisbursementId  
            // inner join LoanApplication LA on LD.LoanApplicationId = LA.LoanApplicationId
            // inner join Branch B on LA.BranchId = B.BranchId
            // inner join Client C on LA.ClientId = C.ClientId  
            // left join LoanRepayment LR on LES.LoanEMIScheduleId = LR.LoanEMIScheduletId  
            // Where LD.BranchId=1 and LD.ClientId=1 
            //  and LD.LoanDisbursementId=1
            //  and LES.EMIDate <= GETDATE() 
            //  and LR.AmountPaid is null


            /****** Script for SelectTopNRows command from SSMS  ******/
            //SELECT TOP 1000
            // C.PersonalContact       
            //      ,[EMIDate]
            //      ,[EMI]
            //      ,[PrincipleAmount]
            //      ,[InterestAmount]
            //      ,[Balance]  
            //      ,LA.Notes
            //  FROM [AasthaFinance].[dbo].[LoanEMISchedule] LES
            //  inner join LoanDisbursement LD on LES.LoanDisbursementId=LD.LoanDisbursementId  
            //  inner join LoanApplication LA on LD.LoanApplicationId = LA.LoanApplicationId
            //  inner join Branch B on LA.BranchId = B.BranchId
            //  inner join Client C on LA.ClientId = C.ClientId  

            //  Where LD.BranchId=1 and LD.ClientId=1 
            //   and LD.LoanDisbursementId=1
            //   and LES.EMIDate <= GETDATE() and LES.LoanEMIScheduleId not in(
            //   Select LoanEMIScheduletId from LoanRepayment LR
            //   where LR.BranchId=1 and LR.ClientId=1 
            //   and LR.LoanDisbursementId=1)



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

        //To display Collection detail report this method is used.
        public void pdf(CollectionDetail collectionDetail)
        {

            if (collectionDetail.ClientId != 0)
            {
                //TODO: Increase the number of records in pdf we need to work
                collectionDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == collectionDetail.BranchId
                    && x.ClientId == collectionDetail.ClientId && x.PaymentDate >= collectionDetail.StartDate && x.PaymentDate <= collectionDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(1, 100);

                if (collectionDetail.LoanRepayments.Count > 0)
                {
                    CreateCollectedReportForSingleClient(collectionDetail);
                    //return new FileStreamResult(workStream, "application/pdf");
                }
            }
            else
            {
                collectionDetail.LoanRepayments = db.LoanRepayments.Where(x => x.BranchId == collectionDetail.BranchId
                     && x.PaymentDate >= collectionDetail.StartDate && x.PaymentDate <= collectionDetail.EndDate).OrderBy(x => x.PaymentDate).ToPagedList(1, 1000);

                if (collectionDetail.LoanRepayments.Count > 0)
                {
                    CreateCollectedReportForSingleClient(collectionDetail);
                }
            }

            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "BranchCode");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            RedirectToAction("GetCollectionDetail");
        }

        private void CreateCollectedReportForSingleClient(CollectionDetail collectionDetail)
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;
            PdfPTable table = new PdfPTable(6);
            table.TotalWidth = 400f;
            //fix the absolute width of the table
            table.LockedWidth = true;
            //relative col widths in proportions - 1/3 and 2/3
            float[] widths = new float[] { 5f, 5f, 4f, 4f, 4f, 4f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;
            //leave a gap before and after the table
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;
            Font bold = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
            Phrase p = new Phrase("Payment Collected Report", bold);
            PdfPCell cell = new PdfPCell(p);
            cell.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
            cell.Colspan = 6;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = 1;
            table.AddCell(cell);

            Font b = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD);
            Phrase reportSummary = new Phrase(string.Format("Collection between {0} and {1}", collectionDetail.StartDate.Value.ToShortDateString(), collectionDetail.EndDate.Value.ToShortDateString()), b);
            PdfPCell cellHeading = new PdfPCell(reportSummary);
            cellHeading.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            cellHeading.Colspan = 6;
            cellHeading.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellHeading.VerticalAlignment = 1;
            table.AddCell(cellHeading);


            PdfPCell cellBranchName = new PdfPCell(new Phrase("Branch Name"));
            cellBranchName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellBranchName);

            PdfPCell cellClientName = new PdfPCell(new Phrase("Client Name"));
            cellClientName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellClientName);

            PdfPCell cellAmountPaid = new PdfPCell(new Phrase("Amount Paid"));
            cellAmountPaid.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellAmountPaid);

            PdfPCell cellPaymentDate = new PdfPCell(new Phrase("Payment Date"));
            cellPaymentDate.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellPaymentDate);

            PdfPCell cellEMIDate = new PdfPCell(new Phrase("EMI Date"));
            cellEMIDate.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellEMIDate);

            PdfPCell cellEMI = new PdfPCell(new Phrase("EMI"));
            cellEMI.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellEMI);

            //table.AddCell("Branch Name");               

            //table.AddCell("Client Name");

            //table.AddCell("Amount Paid");

            //table.AddCell("Payment Date");

            decimal totalCollection = 0;

            foreach (var item in collectionDetail.LoanRepayments)
            {

                table.AddCell(item.Branch.BranchName);

                table.AddCell(item.Client.ClientName);

                table.AddCell(item.AmountPaid.ToString());

                table.AddCell(item.PaymentDate.Value.ToShortDateString());

                table.AddCell(item.LoanEMISchedule.EMIDate.Value.ToShortDateString());
                
                table.AddCell(item.LoanEMISchedule.EMI.Value.ToString());

                totalCollection += item.AmountPaid.Value;

            }

            Font bt = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
            Phrase phraseTotal = new Phrase("Total Collected : " + totalCollection.ToString(), bt);
            PdfPCell cellTotal = new PdfPCell(phraseTotal);
            cellTotal.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
            cellTotal.Colspan = 6;
            cellTotal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cellTotal.VerticalAlignment = 1;
            table.AddCell(cellTotal);

            //table.AddCell("");
            //table.AddCell("Total :");
            //table.AddCell(totalCollection.ToString());
            //table.AddCell("");

            //table.AddCell("Col 1 Row 2");

            //table.AddCell("Col 2 Row 2");

            //table.AddCell("Col 3 Row 2");

            document.Open();
            document.Add(table);
            //document.Add(new Paragraph("Hello World"));
            document.Add(new Paragraph("(Signature)"));
            document.Add(new Paragraph("Date :" + DateTime.Now.ToString()));
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition", "attachment; filename= " + Server.HtmlEncode("Collection.pdf"));
            Response.ContentType = "APPLICATION/pdf";
            Response.BinaryWrite(byteInfo);
        }

        public void PrintTodayCollectionReciptsForBranch(CollectionDetail collectionDetail)
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;
            PdfPTable table = new PdfPTable(10);
            table.TotalWidth = 550f;
            //fix the absolute width of the table
            table.LockedWidth = true;
            //relative col widths in proportions - 1/3 and 2/3
            float[] widths = new float[] { 5f, 4f, 4f, 4f, 2f, 3f, 3f, 4f, 3f, 4f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;
            //leave a gap before and after the table
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;
            Font bold = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
            Phrase p = new Phrase("Today's Collection Receipt", bold);
            PdfPCell cell = new PdfPCell(p);
            cell.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
            cell.Colspan = 10;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = 1;
            table.AddCell(cell);

            Font b = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD);
            Phrase reportSummary = new Phrase(string.Format("Collection recipt on {0}", DateTime.Now.Date.ToShortDateString()), b);
            PdfPCell cellHeading = new PdfPCell(reportSummary);
            cellHeading.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            cellHeading.Colspan = 10;
            cellHeading.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellHeading.VerticalAlignment = 1;
            table.AddCell(cellHeading);



            PdfPCell cellBranchName = new PdfPCell(new Phrase("CL Code"));
            cellBranchName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellBranchName);

            PdfPCell cellClientName = new PdfPCell(new Phrase("CL Name"));
            cellClientName.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellClientName);

            PdfPCell cellAmountPaid = new PdfPCell(new Phrase("Contact"));
            cellAmountPaid.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellAmountPaid);

            PdfPCell cellPaymentDate = new PdfPCell(new Phrase("EMI Date"));
            cellPaymentDate.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellPaymentDate);

            PdfPCell cellEMI = new PdfPCell(new Phrase("EMI"));
            cellEMI.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellEMI);

            PdfPCell cellPrincipleAmount = new PdfPCell(new Phrase("Amt"));
            cellPrincipleAmount.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellPrincipleAmount);

            PdfPCell cellInterestAmount = new PdfPCell(new Phrase("Interst Amt"));
            cellInterestAmount.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellInterestAmount);

            PdfPCell cellBalance = new PdfPCell(new Phrase("Balance"));
            cellBalance.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellBalance);

            //PdfPCell cellNotes = new PdfPCell(new Phrase("Notes"));
            //cellNotes.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            //table.AddCell(cellNotes);

            PdfPCell cellSignature = new PdfPCell(new Phrase("Signature"));
            cellSignature.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellSignature);

            PdfPCell cellAdditonalRemark = new PdfPCell(new Phrase("Remark"));
            cellAdditonalRemark.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
            table.AddCell(cellAdditonalRemark);


            //table.AddCell("Branch Name");               

            var collectionDetails = db.GetCollectionRecipt(collectionDetail.BranchId).ToList<Data.GetCollectionRecipt_Result>();

            foreach (Data.GetCollectionRecipt_Result item in collectionDetails)
            {

                table.AddCell(item.ClientCode);

                table.AddCell(item.ClientName);

                table.AddCell(item.PersonalContact);

                table.AddCell(item.EMIDate.Value.ToShortDateString());

                table.AddCell(item.EMI.ToString());

                table.AddCell(item.PrincipleAmount.ToString());

                table.AddCell(item.InterestAmount.ToString());

                table.AddCell(item.Balance.ToString());

                //table.AddCell(item.Notes.ToString());

                table.AddCell(item.Signature.ToString());

                table.AddCell(item.Additonal_Remark);

            }

            //Font bt = new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
            //Phrase phraseTotal = new Phrase("Total Collected : " + totalCollection.ToString(), bt);
            //PdfPCell cellTotal = new PdfPCell(phraseTotal);
            //cellTotal.BackgroundColor = new iTextSharp.text.BaseColor(51, 192, 192);
            //cellTotal.Colspan = 4;
            //cellTotal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            //cellTotal.VerticalAlignment = 1;
            //table.AddCell(cellTotal);

            //table.AddCell("Col 1 Row 2");

            //table.AddCell("Col 2 Row 2");

            //table.AddCell("Col 3 Row 2");

            document.Open();
            document.Add(table);
            //document.Add(new Paragraph("Hello World"));
            document.Add(new Paragraph(DateTime.Now.ToString()));
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition", "attachment; filename= " + Server.HtmlEncode(string.Format("TodaysCollectionRecipt_{0}.pdf", DateTime.Now.Date.ToShortDateString())));
            Response.ContentType = "APPLICATION/pdf";
            Response.BinaryWrite(byteInfo);
        }

    }
}
