using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AasthaFinance.Models.Report
{
    public class ReportBuilderModel
    {
        public string ReportHeading { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string ScheduleDate { get; set; }

        public string BranchName { get; set; }

        public string ActualPaid { get; set; }
                

    }
        
}