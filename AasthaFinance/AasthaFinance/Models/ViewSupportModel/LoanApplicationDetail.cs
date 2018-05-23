using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AasthaFinance.Models.ViewSupportModel
{
    public class LoanApplicationDetail
    {
        public string ClientName { get; set; }
        public string BranchName { get; set; }
        public string BankName { get; set; }
        public string BankAccountNo { get; set; }
        public string ChequeNo { get; set; }
        public decimal? LoanAmount { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> EMIStartDate { get; set; }

        public int? FreequencyId { get; set; }
        public decimal? InterestRate { get; set; }

        public int? InterestModelId { get; set; }


    }
}