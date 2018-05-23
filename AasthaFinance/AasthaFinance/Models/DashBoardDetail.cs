using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AasthaFinance.Models
{
    public class DashBoardDetail
    {
        public decimal TotalCollectionOfDay { get; set; }
        public decimal TotalCollectedOfDay { get; set; }
        public decimal TotalDueOfDay { get; set; }
        public decimal TotalActiveLoan { get; set; }

        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal Margin { get; set; }

    }
}