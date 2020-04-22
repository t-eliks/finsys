using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.ViewModels;

namespace Web.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly Repository repository;

        public ExpenseController(Repository repository)
        {
            this.repository = repository;
        }

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
                Purpose = expense.Purpose,
                Note = expense.Note
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

            var expense = repository.Expenses.FirstOrDefault(x => x.Id == viewModel.Id);

            if (expense == null)
            {
                TempData["Error"] = "Something went wrong.";

                return View("ExpenseForm", viewModel);
            }

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

            var expenses = SelectUsersExpenses();

            return View("ExpenseList", new ExpenseListViewModel { Expenses = expenses });
        }

        //[HttpDelete]
        //[Route("delete/{id:int}")]
        //public IActionResult DeleteSelected(int id)
        //{
        //    
        //}

        private void InsertNewExpense(ExpenseViewModel viewModel)
        {
            var expense = new Expense
            {
                Amount = viewModel.Amount,
                Note = viewModel.Note,
                Purpose = viewModel.Purpose,
                CreatedOn = DateTime.UtcNow
            };

            repository.Add(expense);

            repository.SaveChanges();
        }

        private void Delete(int id)
        {
            var expense = repository.Expenses.FirstOrDefault(x => x.Id == id);

            repository.Remove(expense);

            repository.SaveChanges();
        }

        private string ValidateData(ExpenseViewModel viewModel)
        {
            if (viewModel.Amount < 0)
            {
                return "Kiekis turi būti daugiau už 0.";
            }

            if (string.IsNullOrWhiteSpace(viewModel.Purpose))
            {
                return "Paskirties laukas turi būti užpildytas.";
            }

            return string.Empty;
        }

        private void UpdateExpense(Expense expense, ExpenseViewModel viewModel)
        {
            expense.Amount = viewModel.Amount;
            expense.Note = viewModel.Note;
            expense.Purpose = viewModel.Purpose;
            expense.UpdatedOn = DateTime.UtcNow;

            repository.Update(expense);

            repository.SaveChanges();
        }

        private IList<Expense> SelectUsersExpenses()
        {
            return repository.Expenses.ToList();
        }
    }
}