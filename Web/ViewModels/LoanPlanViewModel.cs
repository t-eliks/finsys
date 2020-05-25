using System.Collections.Generic;
using DataAccess.Models;

namespace Web.ViewModels
{
    public class LoanPlanViewModel
    {
        public float InvestmentSum { get; set; }
        public float InvestmentInterest { get; set; }
        public int SelectedLoanId { get; set; }
    }
}