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
            var categories = FetchCategories();
            var limits = FetchLimits();
            var goals = FetchGoals();
            
            return View("PlanPage", CalculateBudget(categories, goals, limits));
        }
        
        [HttpPost]
        [Route("goal")]
        public ActionResult SubmitGoal([FromBody] GoalViewModel viewModel)
        {
            var response = StoreGoal(viewModel);

            return Ok(new { Id = response.Item1, ActualAmount = response.Item2 });
        }

        public (int, double) StoreGoal(GoalViewModel viewModel)
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

            goal.GoalAmount = viewModel.Goal;
            goal.Category = repository.Categories.FirstOrDefault(x => x.Id == viewModel.CategoryId);

            repository.SaveChanges();

            var sum = repository.Income.Where(x => x.Category == goal.Category).Sum(x => x.Amount);
            
            return (goal.Id, sum);
        }
            
        [HttpPost]
        [Route("limit")]
        public ActionResult SubmitLimit([FromBody] LimitViewModel viewModel)
        {
            var response = StoreLimit(viewModel);

            return Ok(new { Id = response.Item1, ActualAmount = response.Item2 });
        }
        
        public (int, double) StoreLimit(LimitViewModel viewModel)
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
            
            var sum = repository.Expenses.Where(x => x.Category == limit.Category).Sum(x => x.Amount);

            return (limit.Id, sum);
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
                Goal = x.GoalAmount,
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

        public PlanBudgetViewModel CalculateBudget(List<DataAccess.Models.Category> categories, List<GoalViewModel> goals, List<LimitViewModel> limits)
        {
            var processedGoals = new List<GoalViewModel>();
            var processedLimits = new List<LimitViewModel>();

            categories.Union(new List<DataAccess.Models.Category> { null }).ToList().ForEach(c =>
            {
                processedGoals.AddRange(goals.Where(x => x.CategoryId == c?.Id).Select(g =>
                {
                    g.ActualAmount = repository.Income.Where(x => x.Category == c).Sum(x => x.Amount);

                    return g;
                }).ToList());

                processedLimits.AddRange(limits.Where(x => x.CategoryId == c?.Id).Select(l =>
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