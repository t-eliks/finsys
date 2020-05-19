using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers.Budget
{
    public class IncomeController : Controller
    {
        private readonly Repository repository;

        public IncomeController(Repository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Route("income")]
        public IActionResult OpenIncomeList()
        {
            return View("IncomeList", new IncomeListViewModel {Income = FetchUserIncomeList()});
        }

        [Route("income/edit/{id:int}")]
        [HttpGet]
        public IActionResult OpenEditForm(int id)
        {
            var income = repository.Income.FirstOrDefault(x => x.Id == id);

            if (income == null)
            {
                return View("IncomeList");
            }

            return View("IncomeForm", new IncomeViewModel
            {
                Id = income.Id,
                Amount = income.Amount,
                Source = income.Source,
                Comment = income.Comment
            });
        }
        
        [HttpPost]
        public IActionResult EditIncome(IncomeViewModel viewModel)
        {
            var validation = ValidateData(viewModel);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;

                return View("IncomeForm", viewModel);
            }

            var income = FetchIncome(viewModel.Id);

            if (income == null)
            {
                TempData["Error"] = "Something went wrong.";

                return View("IncomeForm", viewModel);
            }

            TempData["Success"] = "Sėkmingai atnaujintos pajamos!";

            UpdateIncome(income, viewModel);

            var incomeList = FetchUserIncomeList();

            return View("IncomeList", new IncomeListViewModel() {Income = incomeList});
        }

        [HttpPost]
        public IActionResult Submit(IncomeViewModel viewModel)
        {
            var validation = ValidateData(viewModel);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;

                return View("IncomeForm", viewModel);
            }

            var income = FetchIncome(viewModel.Id);

            if (income == null)
            {
                TempData["Error"] = "Something went wrong.";

                return View("IncomeForm", viewModel);
            }

            TempData["Success"] = "Sėkmingai atnaujintos pajamos!";

            UpdateIncome(income, viewModel);

            var incomeList = FetchUserIncomeList();

            return View("IncomeList", new IncomeListViewModel {Income = incomeList});
        }

        [HttpGet]
        public IActionResult OpenCreationForm()
        {
            return View("IncomeForm", new IncomeViewModel());
        }

        [HttpPost]
        public IActionResult CreateIncome(IncomeViewModel viewModel)
        {
            var validation = ValidateData(viewModel);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;

                return View("IncomeForm", viewModel);
            }

            InsertNewIncome(viewModel);

            TempData["Success"] = "Pajamos sėkmingai pridėtos!";

            var incomeList = FetchUserIncomeList();

            return View("IncomeList", new IncomeListViewModel {Income = incomeList});
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("DeleteConfirmForm",
                new DeletionViewModel() {Id = id, LtName = "pajamas", Method = "DeleteIncome", Controller = "Income"});
        }

        [HttpDelete]
        public IActionResult DeleteIncome(int id)
        {
            var income = repository.Income.FirstOrDefault(x => x.Id == id);

            repository.Remove(income);

            repository.SaveChanges();

            TempData["Success"] = "Pajamos sėkmingai panaikintos!";

            return Ok(Url.Action("OpenIncomeList", "Income"));
        }

        private void InsertNewIncome(IncomeViewModel viewModel)
        {
            var income = new Income
            {
                Amount = viewModel.Amount.Value,
                Comment = viewModel.Comment,
                Source = viewModel.Source,
                CreationDate = DateTime.UtcNow
            };

            repository.Add(income);

            repository.SaveChanges();
        }

        private string ValidateData(IncomeViewModel viewModel)
        {
            if (!viewModel.Amount.HasValue)
            {
                return "Kiekis yra privalomas";
            }

            if (viewModel.Amount < 0)
            {
                return "Kiekis turi būti daugiau už 0.";
            }

            if (string.IsNullOrWhiteSpace(viewModel.Source))
            {
                return "Šaltinio laukas turi būti užpildytas.";
            }

            return string.Empty;
        }

        private void UpdateIncome(Income income, IncomeViewModel viewModel)
        {
            income.Amount = viewModel.Amount.Value;
            income.Comment = viewModel.Comment;
            income.Source = viewModel.Source;
            income.UpdateDate = DateTime.UtcNow;

            repository.Update(income);

            repository.SaveChanges();
        }

        private IList<Income> FetchUserIncomeList()
        {
            return repository.Income.ToList();
        }

        private Income FetchIncome(int id)
        {
            return repository.Income.FirstOrDefault(x => x.Id == id);
        }
    }
}