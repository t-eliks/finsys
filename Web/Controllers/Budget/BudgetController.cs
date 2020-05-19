using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers.Budget
{
    public class BudgetController : Controller
    {
        private readonly Repository repository;

        public BudgetController(Repository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Route("budget")]
        public ActionResult OpenPlanningPage()
        {
            return View("PlanPage", new PlanBudgetViewModel { Goals =  FetchGoals(), Categories = FetchCategories()});
        }
        
        [HttpPost]
        [Route("goal")]
        public ActionResult SubmitGoal([FromBody] GoalViewModel viewModel)
        {
            StoreGoal(viewModel);

            return Ok();
        }

        public void StoreGoal(GoalViewModel viewModel)
        {
            Goal goal;
            
            if (viewModel.Id.HasValue && viewModel.Id != 0)
            {
                goal = repository.Goals.FirstOrDefault(x => x.Id == viewModel.Id);
            }
            else
            {
                goal = new Goal();
                repository.Add(goal);
            }

            goal.Limit = viewModel.Limit;
            goal.Category = repository.Categories.FirstOrDefault(x => x.Id == viewModel.CategoryId);

            repository.SaveChanges();
        }
            
            
        public IList<DataAccess.Models.Category> FetchCategories()
        {
            return repository.Categories.ToList();
        }

        public IList<GoalViewModel> FetchGoals()
        {
            return repository.Goals.Select(x => new GoalViewModel
            {
                Id = x.Id,
                Limit = x.Limit,
                CategoryId = x.Category.Id
            }).ToList();
        }
    }
}