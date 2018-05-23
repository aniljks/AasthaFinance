using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AasthaFinance.Models
{
    public class BranchProgressReport
    {
        public int TotalClient { get; set; }

        public int TotalActiveClient { get; set; }

        public int TotalDisbursedLoan { get; set; }

        public decimal TotalLoanAmountDisbursed { get; set; }

        public decimal TotalCollection { get; set; }

        public decimal TotalDue { get; set; }

        public decimal TotalDueRemaining { get; set; }

        public decimal TotalInterest { get; set; }

        public decimal TotalInterestEarn { get; set; }

        public decimal TotalInterestDue { get; set; }

        public int TotalDisputedLoan { get; set; }

        #region Report Parameters

        public string BranchName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        #endregion

    }
}