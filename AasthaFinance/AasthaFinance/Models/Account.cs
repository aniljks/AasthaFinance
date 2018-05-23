using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AasthaFinance.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public List<String> Roles { get; set; }

        public string Message { get; set; }
    }
}