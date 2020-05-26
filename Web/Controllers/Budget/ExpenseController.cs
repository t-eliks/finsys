using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Web.ViewModels;

namespace Web.Controllers.Budget
{
    public class ExpenseController : Controller
    {
        private readonly Repository repository;

        public ExpenseController(Repository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Route("expenses")]
        public IActionResult OpenExpenseList()
        {
            return View("ExpenseList", new ExpenseListViewModel {Expenses = SelectUsersExpenses()});
        }

        [Route("expense/edit/{id:int}")]
        [HttpGet]
        public IActionResult EditSelected(int id)
        {
            var allCategories = GetAllCategories();
            
            var expense = repository.Expenses
                .Select(x => new ExpenseViewModel
                {
                    Id = x.Id,
                    Comment = x.Comment,
                    Origin = x.Origin,
                    Category = x.Category.Name,
                    CategoryId = x.Category.Id,
                    Amount = x.Amount,
                    CreationDate = x.CreationDate,
                    AvailableCategories = allCategories
                })
                .FirstOrDefault(x => x.Id == id);

            if (expense == null)
            {
                return View("ExpenseList");
            }

            return View("ExpenseForm", expense);
        }

        [HttpPost]
        public IActionResult EditExpense([FromBody] ExpenseViewModel viewModel)
        {
            var validation = ValidateData(viewModel);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                return BadRequest(validation);
            }

            var expense = FetchExpense(viewModel.Id);

            if (expense == null)
            {
                TempData["Error"] = "Something went wrong.";

                return View("ExpenseForm", viewModel);
            }

            TempData["Success"] = "Sėkmingai atnaujinta išlaida!";

            UpdateExpense(expense, viewModel);

            var expenses = SelectUsersExpenses();

            return View("ExpenseList", new ExpenseListViewModel {Expenses = expenses});
        }

        [HttpGet]
        public IActionResult CreateForm()
        {
            return View("ExpenseForm", new ExpenseViewModel() { AvailableCategories = GetAllCategories()});
        }

        [HttpPost]
        public IActionResult Create([FromBody] ExpenseViewModel viewModel)
        {
            var validation = ValidateData(viewModel);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                return BadRequest(validation);
            }

            InsertNewExpense(viewModel);

            TempData["Success"] = "Išlaida sėkmingai pridėta!";

            var expenses = SelectUsersExpenses();

            return View("ExpenseList", new ExpenseListViewModel {Expenses = expenses});
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("DeleteConfirmForm",
                new DeletionViewModel {LtName = "išlaidą", Id = id, Controller = "Expense", Method = "DeleteExpense"});
        }

        [HttpDelete]
        public IActionResult DeleteExpense(int id)
        {
            var expense = repository.Expenses.FirstOrDefault(x => x.Id == id);

            repository.Remove(expense);

            repository.SaveChanges();

            TempData["Success"] = "Išlaida sėkmingai panaikinta!";

            return Ok(Url.Action("OpenExpenseList", "Expense"));
        }

        private void InsertNewExpense(ExpenseViewModel viewModel)
        {
            var expense = new Expense
            {
                Amount = viewModel.Amount.Value,
                Comment = viewModel.Comment,
                Origin = viewModel.Origin,
                CreationDate = DateTime.UtcNow,
                Category = repository.Categories.FirstOrDefault(x => x.Id == viewModel.CategoryId)
            };

            repository.Add(expense);

            repository.SaveChanges();
        }

        private string ValidateData(ExpenseViewModel viewModel)
        {
            if (!viewModel.Amount.HasValue)
            {
                return "Kiekis yra privalomas";
            }

            if (viewModel.Amount < 0)
            {
                return "Kiekis turi būti daugiau už 0.";
            }

            if (string.IsNullOrWhiteSpace(viewModel.Origin))
            {
                return "Paskirties laukas turi būti užpildytas.";
            }

            return string.Empty;
        }

        private void UpdateExpense(Expense expense, ExpenseViewModel viewModel)
        {
            expense.Amount = viewModel.Amount.Value;
            expense.Comment = viewModel.Comment;
            expense.Origin = viewModel.Origin;
            expense.UpdateDate = DateTime.UtcNow;
            expense.Category = repository.Categories.FirstOrDefault(x => x.Id == viewModel.CategoryId);

            repository.Update(expense);

            repository.SaveChanges();
        }

        private IList<ExpenseViewModel> SelectUsersExpenses()
        {
            return repository.Expenses
                .Select(x => new ExpenseViewModel
                {
                    Origin = x.Origin,
                    Comment = x.Comment,
                    Category = x.Category.Name,
                    Amount = x.Amount,
                    Id = x.Id,
                    CreationDate = x.CreationDate
                })
                .ToList();
        }

        private Expense FetchExpense(int id)
        {
            return repository.Expenses
                .Include(x => x.Category)
                .FirstOrDefault(x => x.Id == id);
        }

        private IList<CategoryViewModel> GetAllCategories()
        {
            return repository.Categories
                .Select(x => new CategoryViewModel
                {
                    Name = x.Name,
                    Id = x.Id
                })
                .ToList();
        }
    }
}