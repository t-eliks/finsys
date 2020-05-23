using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class LoanPlanViewModel
    {
        public IList<Loan> Loans { get; set; }
    }
}
