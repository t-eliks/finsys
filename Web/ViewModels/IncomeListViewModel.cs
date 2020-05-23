using System.Collections.Generic;
using DataAccess.Models;

namespace Web.ViewModels
{
    public class IncomeListViewModel
    {
        public IList<IncomeViewModel> Income { get; set; }
    }
}