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
    
    public partial class LoanStatu
    {
        public LoanStatu()
        {
            this.LoanDisbursements = new HashSet<LoanDisbursement>();
        }
    
        public int LoanStatusId { get; set; }
        public string LoanStatus { get; set; }
    
        public virtual ICollection<LoanDisbursement> LoanDisbursements { get; set; }
    }
}