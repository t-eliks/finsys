using System.Collections.Generic;
using DataAccess.Models;

namespace Web.ViewModels
{
    public class PlanBudgetViewModel
    {
        public IList<GoalViewModel> Goals { get; set; }
        
        public IList<LimitViewModel> Limits { get; set; }

        public IList<Category> Categories { get; set; }
    }
}