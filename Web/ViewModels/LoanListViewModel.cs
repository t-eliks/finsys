using System.Collections.Generic;
using DataAccess.Models;

namespace Web.ViewModels
{
    public class LoanListViewModel
    {
        public IList<Loan> Loans { get; set; }
    }
}