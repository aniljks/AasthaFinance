using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AasthaFinance.Models.Report
{
    public class DueDetail
    {
        public int BranchId { get; set; }
        public int ClientId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndDate { get; set; }

        public bool isExist { get; set; }

        public PagedList.IPagedList<AasthaFinance.Data.LoanRepayment> LoanRepayments;

        public PagedList.IPagedList<DuesList> LoanDues;


    }


    public class DuesList
    {

        public string BranchName { get; set; }

        public string ClientName { get; set; }

        public string LoanApplicationCode { get; set; }

        public string LoanDisbursementCode { get; set; }

        public string ScheduleDate { get; set; }

        public string EMIDate { get; set; }

        public string Amount { get; set; }

        public string LoanBalanceAmt { get; set; }

    }
}