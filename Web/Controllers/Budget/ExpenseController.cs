using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return View("ExpenseList", new ExpenseListViewModel { Expenses = SelectUsersExpenses() });
        }

        [Route("edit/{id:int}")]
        [HttpGet]
        public IActionResult EditSelected(int id)
        {
            var expense = repository.Expenses.FirstOrDefault(x => x.Id == id);

            if (expense == null)
            {
                return View("ExpenseList");
            }

            return View("ExpenseForm", new ExpenseViewModel
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Origin = expense.Origin,
                Comment = expense.Comment
            });
        }

        [HttpPost]
        public IActionResult EditExpense(ExpenseViewModel viewModel)
        {
            var validation = ValidateData(viewModel);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;

                return View("ExpenseForm", viewModel);
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

            return View("ExpenseList", new ExpenseListViewModel { Expenses = expenses });
        }

        [HttpGet]
        public IActionResult CreateForm()
        {
            return View("ExpenseForm", new ExpenseViewModel());
        }

        [HttpPost]
        public IActionResult Create(ExpenseViewModel viewModel)
        {
            var validation = ValidateData(viewModel);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;

                return View("ExpenseForm", viewModel);
            }

            InsertNewExpense(viewModel);

            TempData["Success"] = "Išlaida sėkmingai pridėta!";

            var expenses = SelectUsersExpenses();

            return View("ExpenseList", new ExpenseListViewModel { Expenses = expenses });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("DeleteConfirmForm", new DeleteExpenseViewModel { Id = id });
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
                CreationDate = DateTime.UtcNow
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

            repository.Update(expense);

            repository.SaveChanges();
        }

        private IList<Expense> SelectUsersExpenses()
        {
            return repository.Expenses.ToList();
        }

        private Expense FetchExpense(int id)
        {
            return repository.Expenses.FirstOrDefault(x => x.Id == id);
        }
    }
}