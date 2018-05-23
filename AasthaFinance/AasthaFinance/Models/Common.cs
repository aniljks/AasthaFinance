using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AasthaFinance.Models
{
    public static class Common
    {
        public static string AddAdditionalZero(string number)
        {
            return number.PadLeft(6, '0');
        }
    }


    public enum LoanApplicationStatus
    {
        Initiate,
        Verification,
        Approved,
        Disputed,
        Closed
    }

    public enum LoanStatus
    {
        Active,
        Closed,
        Disputed

    }


    public enum LoanRepaymentStatus
    {
        Paid,
        PaymentDue

    }

    public enum LoanRepaymentFreequency
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }


    public enum LoanInterestModel
    {
        Flat,
        Reducing
    }



}