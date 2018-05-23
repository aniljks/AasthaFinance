//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AasthaFinance.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class LoanApplication
    {
        public LoanApplication()
        {
            this.LoanDisbursements = new HashSet<LoanDisbursement>();
            this.LoanRepayments = new HashSet<LoanRepayment>();
        }
    
        public int LoanApplicationId { get; set; }
        public string LoanApplicationNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LoanApplicationDate { get; set; }
        public Nullable<decimal> LastMonthIncome { get; set; }
        public Nullable<bool> IsKYCVerified { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EMIStartDate { get; set; }
        public Nullable<decimal> LoanAmount { get; set; }
        public string LoanPurpose { get; set; }
        public string BankName { get; set; }
        public string BankAccountNo { get; set; }
        public string ChequeNo { get; set; }
        public string BranchName { get; set; }
        public string GuarantorName { get; set; }
        public string GuarantorMobile { get; set; }
        public Nullable<decimal> InterestRate { get; set; }
        public Nullable<int> InterestModelId { get; set; }
        public string ReasonForLoan { get; set; }
        public Nullable<int> FreequencyId { get; set; }
        public Nullable<int> LoanApplicationStatusId { get; set; }
        public Nullable<int> ClientId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Notes { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Client Client { get; set; }
        public virtual Freequency Freequency { get; set; }
        public virtual InterestModel InterestModel { get; set; }
        public virtual LoanApplicationStatu LoanApplicationStatu { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<LoanDisbursement> LoanDisbursements { get; set; }
        public virtual ICollection<LoanRepayment> LoanRepayments { get; set; }
    }
}
