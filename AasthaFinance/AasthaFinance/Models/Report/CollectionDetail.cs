using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AasthaFinance.Models.Report
{
    public class CollectionDetail
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
    }
}