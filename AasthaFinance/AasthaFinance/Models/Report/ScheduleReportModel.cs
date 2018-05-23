using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AasthaFinance.Models.Report
{
    public class ScheduleReportModel
    {
        
        public string BranchId { get; set; }
        public string ClientId { get; set; }

        public string LoanApplicationId { get; set; }

        public string DisbursementId { get; set; }

        public string LoanCycleId { get; set; }

        public List<AasthaFinance.Data.LoanEMISchedule> LoanEMISchedule { get; set; }        

    }
}