using System;
using System.Data;
using System.Linq;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModels;

namespace Web.Controllers.Budget
{
    public class LoanController : Controller
    {
        private readonly Repository repository;

        public LoanController(Repository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Route("[controller]/List")]
        public IActionResult Index()
        {
            return View("LoanList", new LoanListViewModel {Loans = repository.Loans.ToList()});
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("LoanForm", new LoanViewModel());
        }

        [HttpPost]
        public IActionResult Create(LoanViewModel viewModel)
        {
            var validation = ValidateData(viewModel);
            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;
                return View("LoanForm", viewModel);
            }

            var loan = new Loan
            {
                Sum = viewModel.Sum,
                Interest = viewModel.Interest,
                Term = viewModel.Term,
                Provider = viewModel.Provider,
                ReturnedSum = viewModel.ReturnedSum,
                Type = viewModel.Type
            };
            repository.Add(loan);
            repository.SaveChanges();
            TempData["Success"] = "Paskola sėkmingai pridėta!";
            return View("LoanList", new LoanListViewModel {Loans = repository.Loans.ToList()});
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        public IActionResult Loan(int id)
        {
            var loan = repository.Loans.FirstOrDefault(x => x.Id == id);
            if (loan == null)
            {
                ViewData["Error"] = "Įvyko klaida. Sistema nerado paskolos";
                return View("LoanList", new LoanListViewModel {Loans = repository.Loans.ToList()});
            }

            var viewModel = new LoanViewModel
            {
                Id = loan.Id,
                Sum = loan.Sum,
                Interest = loan.Interest,
                Term = loan.Term,
                Provider = loan.Provider,
                ReturnedSum = loan.ReturnedSum,
                Type = loan.Type
            };
            return View("LoanDetails", viewModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var loan = repository.Loans.FirstOrDefault(x => x.Id == id);
            if (loan == null)
            {
                ViewData["Error"] = "Įvyko klaida";
                return View("LoanList", new LoanListViewModel {Loans = repository.Loans.ToList()});
            }

            return View("LoanForm", new LoanViewModel
            {
                Id = loan.Id,
                Sum = loan.Sum,
                Interest = loan.Interest,
                Term = loan.Term,
                Provider = loan.Provider,
                ReturnedSum = loan.ReturnedSum,
                Type = loan.Type
            });
        }

        [HttpPost]
        public IActionResult Edit(LoanViewModel viewModel)
        {
            var validation = ValidateData(viewModel);
            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;
                return View("LoanForm", viewModel);
            }

            var loan = repository.Loans.FirstOrDefault(x => x.Id == viewModel.Id);

            if (loan == null)
            {
                TempData["Error"] = "Įvyko klaida";
                return View("LoanForm", viewModel);
            }

            TempData["Success"] = "Paskola sėkmingai atnaujinta";
            loan.Sum = viewModel.Sum;
            loan.Interest = viewModel.Interest;
            loan.Term = viewModel.Term;
            loan.Provider = viewModel.Provider;
            loan.ReturnedSum = viewModel.ReturnedSum;
            loan.Type = viewModel.Type;
            repository.Update(loan);
            repository.SaveChanges();

            return View("LoanDetails", new LoanViewModel
            {
                Id = loan.Id,
                Sum = loan.Sum,
                Interest = loan.Interest,
                Term = loan.Term,
                Provider = loan.Provider,
                ReturnedSum = loan.ReturnedSum,
                Type = loan.Type
            });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("DeleteConfirmForm", new DeletionViewModel
            {
                Id = id,
                LtName = "paskolą",
                Method = "DeleteLoan",
                Controller = "Loan"
            });
        }

        [HttpDelete]
        public IActionResult DeleteLoan(int id)
        {
            var loan = repository.Loans.FirstOrDefault(x => x.Id == id);
            repository.Remove(loan);
            repository.SaveChanges();
            TempData["Success"] = "Paskola sėkmingai panaikinta";
            return Ok(Url.Action("Index", "Loan"));
        }

        [HttpGet]
        public IActionResult OpenLoanPlanner()
        {
            ViewBag.Loans = GenerateLoanSelectList();
            return View("~/Views/LoanPlanner/LoanPlanPage.cshtml", new LoanPlanViewModel());
        }

        [HttpPost]
        public IActionResult SubmitPlanData(LoanPlanViewModel viewModel)
        {
            var validation = ValidateData(viewModel);
            if (!string.IsNullOrEmpty(validation))
            {
                TempData["Error"] = validation;
                ViewBag.Loans = GenerateLoanSelectList();
                return View("~/Views/LoanPlanner/LoanPlanPage.cshtml", viewModel);
            }

            var loan = repository.Loans.FirstOrDefault(x => x.Id == viewModel.SelectedLoanId);
            var daysRemainingWithSurplus = CalculateLoanWithSurplus(loan, viewModel);
            var potentialReturnOnInvestment = CalculatePotentialReturnOnInvestment(loan, viewModel);
            var daysRemainingWithoutSurplus = CalculateLoanWithoutSurplus(loan);
            ViewBag.FinalAnswer = CompareStrategies(daysRemainingWithSurplus, potentialReturnOnInvestment,
                daysRemainingWithoutSurplus, loan);
            ViewBag.Loans = GenerateLoanSelectList();
            return View("~/Views/LoanPlanner/LoanPlanPage.cshtml", viewModel);
        }

        private string CompareStrategies(double daysRemainingWithSurplus, double investmentProfit,
            double daysRemainingWithoutSurplus, Loan loan)
        {
            var finalProfit = investmentProfit - loan.Sum - loan.ReturnedSum;
            if (finalProfit < 0)
            {
                return $"Rekomenduojame neinvestuoti. Paskolą sumokėsite per {Math.Ceiling(daysRemainingWithSurplus)} dienas";
            }

            return
                $"Rekomenduojame investuoti. Paskolą sumokėsite per {Math.Ceiling(daysRemainingWithoutSurplus)} dienas. Taip pat galutinis likutis bus padidėjęs {Math.Floor(finalProfit)} €";
        }

        private double CalculateLoanWithoutSurplus(Loan loan)
        {
            return (loan.Term.Date - DateTime.Now).TotalDays;
        }

        private double CalculatePotentialReturnOnInvestment(Loan loan, LoanPlanViewModel viewModel)
        {
            var dailyProfit = viewModel.InvestmentSum * 12 * (1 + viewModel.InvestmentInterest / 100) / 365;
            var remainingDays = (loan.Term.Date - DateTime.Now).TotalDays;
            return dailyProfit * remainingDays;
        }

        private double CalculateLoanWithSurplus(Loan loan, LoanPlanViewModel viewModel)
        {
            var remainingSum = loan.Sum - loan.ReturnedSum;
            var remainingDays = (loan.Term.Date - DateTime.Now).TotalDays;
            var dailyCharge = remainingSum / remainingDays;
            var dailySurplus = viewModel.InvestmentSum / 31;
            return remainingSum / (dailyCharge + dailySurplus);
        }

        private IQueryable<SelectListItem> GenerateLoanSelectList()
        {
            return repository.Loans.Select(x => new SelectListItem
            {
                Text = $"{x.Id}. {x.Sum} €. Išdavėjas: {x.Provider}. Terminas: {x.Term.ToShortDateString()}",
                Value = x.Id.ToString()
            });
        }

        private string ValidateData(LoanPlanViewModel viewModel)
        {
            if (viewModel.InvestmentSum <= 0)
            {
                return "Būtina įvesti investicijos sumą";
            }

            if (viewModel.InvestmentInterest <= 0)
            {
                return "Būtina įvesti investicijos palūkanas";
            }

            return string.Empty;
        }

        private string ValidateData(LoanViewModel viewModel)
        {
            if (viewModel.Sum <= 0)
            {
                return "Būtina įvesti sumą";
            }

            if (viewModel.Interest <= 0)
            {
                return "Būtina įvesti palūkanas";
            }

            if (viewModel.ReturnedSum <= 0)
            {
                return "Būtina įvesti grąžintą sumą";
            }

            if (string.IsNullOrEmpty(viewModel.Provider))
            {
                return "Būtina įvesti išdavėją";
            }

            return string.Empty;
        }
    }
}