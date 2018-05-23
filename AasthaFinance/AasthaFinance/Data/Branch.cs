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
    
    public partial class Branch
    {
        public Branch()
        {
            this.Clients = new HashSet<Client>();
            this.LoanApplications = new HashSet<LoanApplication>();
            this.LoanDisbursements = new HashSet<LoanDisbursement>();
            this.LoanRepayments = new HashSet<LoanRepayment>();
        }
    
        public int BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string BranchContact { get; set; }
    
        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<LoanApplication> LoanApplications { get; set; }
        public virtual ICollection<LoanDisbursement> LoanDisbursements { get; set; }
        public virtual ICollection<LoanRepayment> LoanRepayments { get; set; }
    }
}
