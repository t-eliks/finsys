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
            return View("PlanPage", CalculateBudget());
        }
        
        [HttpPost]
        [Route("goal")]
        public ActionResult SubmitGoal([FromBody] GoalViewModel viewModel)
        {
            var sum = StoreGoal(viewModel);

            return Ok(sum);
        }

        public int StoreGoal(GoalViewModel viewModel)
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
            
            return (int) repository.Income.Where(x => x.Category == goal.Category).Sum(x => x.Amount);
        }
            
        [HttpPost]
        [Route("limit")]
        public ActionResult SubmitLimit([FromBody] LimitViewModel viewModel)
        {
            var sum = StoreLimit(viewModel);

            return Ok(sum);
        }
        
        public int StoreLimit(LimitViewModel viewModel)
        {
            Limit limit;
            
            if (viewModel.Id.HasValue && viewModel.Id != 0)
            {
                limit = repository.Limits.FirstOrDefault(x => x.Id == viewModel.Id);
            }
            else
            {
                limit = new Limit();
                repository.Add(limit);
            }

            limit.LimitAmount = viewModel.Limit;
            limit.Category = repository.Categories.FirstOrDefault(x => x.Id == viewModel.CategoryId);

            repository.SaveChanges();
            
            return (int) repository.Expenses.Where(x => x.Category == limit.Category).Sum(x => x.Amount);
        }

        [HttpDelete]
        public ActionResult RemoveGoal([FromRoute] int id)
        {
            var goal = repository.Goals.FirstOrDefault(x => x.Id == id);

            if (goal != null)
            {
                repository.Remove(goal);

                repository.SaveChanges();
            }

            return Ok();
        }
        
        [HttpDelete]
        public ActionResult RemoveLimit([FromRoute] int id)
        {
            var limit = repository.Limits.FirstOrDefault(x => x.Id == id);

            if (limit != null)
            {
                repository.Remove(limit);

                repository.SaveChanges();
            }

            return Ok();
        }
        
        public List<DataAccess.Models.Category> FetchCategories()
        {
            return repository.Categories.ToList();
        }

        public List<GoalViewModel> FetchGoals()
        {
            return repository.Goals.Select(x => new GoalViewModel
            {
                Id = x.Id,
                Limit = x.Limit,
                CategoryId = x.Category.Id
            }).ToList();
        }
        
        public List<LimitViewModel> FetchLimits()
        {
            return repository.Limits.Select(x => new LimitViewModel()
            {
                Id = x.Id,
                Limit = x.LimitAmount,
                CategoryId = x.Category.Id
            }).ToList();
        }

        public PlanBudgetViewModel CalculateBudget()
        {
            var categories = FetchCategories();
            var goals = FetchGoals();
            var limits = FetchLimits();

            var processedGoals = new List<GoalViewModel>();
            var processedLimits = new List<LimitViewModel>();
            
            categories.ForEach(c =>
            {
                processedGoals.AddRange(goals.Where(x => x.CategoryId == c.Id).Select(g =>
                {
                    g.ActualAmount = repository.Income.Where(x => x.Category == c).Sum(x => x.Amount);

                    return g;
                }).ToList());

                processedLimits.AddRange(limits.Where(x => x.CategoryId == c.Id).Select(l =>
                {
                    l.ActualAmount = repository.Expenses.Where(x => x.Category == c).Sum(x => x.Amount);

                    return l;
                }).ToList());
            });

            return new PlanBudgetViewModel
            {
                Categories = categories,
                Goals = processedGoals,
                Limits = processedLimits
            };
        }
    }
}