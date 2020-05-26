using System;
using System.Collections.Generic;
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
            return View("LoanList", new LoanListViewModel {Loans = All()});
        }

        [HttpGet]
        public IActionResult GetCreate()
        {
            return View("LoanForm", new LoanViewModel());
        }

        [HttpPost]
        public IActionResult PostCreate(LoanViewModel viewModel)
        {
            var validation = Validate(viewModel);
            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;
                return View("LoanForm", viewModel);
            }
            
            Create(viewModel);
            TempData["Success"] = "Paskola sėkmingai pridėta!";
            return View("LoanList", new LoanListViewModel {Loans = All()});
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        public IActionResult GetLoan(int id)
        {
            var loan = Find(id);
            if (loan == null)
            {
                ViewData["Error"] = "Įvyko klaida. Sistema nerado paskolos";
                return View("LoanList", new LoanListViewModel {Loans = All()});
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
        public IActionResult GetEdit(int id)
        {
            var loan = Find(id);
            if (loan == null)
            {
                ViewData["Error"] = "Įvyko klaida";
                return View("LoanList", new LoanListViewModel {Loans = All()});
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
        public IActionResult PostEdit(LoanViewModel viewModel)
        {
            var validation = Validate(viewModel);
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
            loan.Sum = viewModel.Sum.Value;
            loan.Interest = viewModel.Interest.Value;
            loan.Term = viewModel.Term;
            loan.Provider = viewModel.Provider;
            loan.ReturnedSum = viewModel.ReturnedSum.Value;
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
        public IActionResult GetDelete(int id)
        {
            return PartialView("DeleteConfirmForm", new DeletionViewModel
            {
                Id = id,
                LtName = "paskolą",
                Method = "PostDelete",
                Controller = "Loan"
            });
        }

        [HttpDelete]
        public IActionResult PostDelete(int id)
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
            var validation = Validate(viewModel);
            if (!string.IsNullOrEmpty(validation))
            {
                TempData["Error"] = validation;
                ViewBag.Loans = GenerateLoanSelectList();
                return View("~/Views/LoanPlanner/LoanPlanPage.cshtml", viewModel);
            }

            var loan = Find(viewModel.SelectedLoanId);
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
                return
                    $"Rekomenduojame neinvestuoti. Paskolą sumokėsite per {Math.Ceiling(daysRemainingWithSurplus)} dienas";
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
            return (double) (dailyProfit * remainingDays);
        }

        private double CalculateLoanWithSurplus(Loan loan, LoanPlanViewModel viewModel)
        {
            var remainingSum = loan.Sum - loan.ReturnedSum;
            var remainingDays = (loan.Term.Date - DateTime.Now).TotalDays;
            var dailyCharge = remainingSum / remainingDays;
            var dailySurplus = viewModel.InvestmentSum / 31;
            return (double) (remainingSum / (dailyCharge + dailySurplus));
        }

        private IQueryable<SelectListItem> GenerateLoanSelectList()
        {
            return repository.Loans.Select(x => new SelectListItem
            {
                Text = $"Id: {x.Id}. {x.Sum} €. Išdavėjas: {x.Provider}. Terminas: {x.Term.ToShortDateString()}",
                Value = x.Id.ToString()
            });
        }

        private string Validate(LoanPlanViewModel viewModel)
        {
            if (!viewModel.InvestmentSum.HasValue || viewModel.InvestmentSum <= 0)
            {
                return "Būtina įvesti investicijos sumą";
            }

            if (!viewModel.InvestmentInterest.HasValue || viewModel.InvestmentInterest <= 0)
            {
                return "Būtina įvesti investicijos palūkanas";
            }

            return string.Empty;
        }

        private string Validate(LoanViewModel viewModel)
        {
            if (!viewModel.Sum.HasValue || viewModel.Sum.Value <= 0)
            {
                return "Būtina įvesti sumą";
            }

            if (!viewModel.Interest.HasValue || viewModel.Interest.Value <= 0)
            {
                return "Būtina įvesti palūkanas";
            }

            if (!viewModel.ReturnedSum.HasValue || viewModel.ReturnedSum < 0)
            {
                return "Būtina įvesti grąžintą sumą";
            }

            if (string.IsNullOrEmpty(viewModel.Provider))
            {
                return "Būtina įvesti išdavėją";
            }

            return string.Empty;
        }

        private void Create(LoanViewModel viewModel)
        {
            var loan = new Loan
            {
                Sum = viewModel.Sum.Value,
                Interest = viewModel.Interest.Value,
                Term = viewModel.Term,
                Provider = viewModel.Provider,
                ReturnedSum = viewModel.ReturnedSum.Value,
                Type = viewModel.Type
            };
            repository.Add(loan);
            repository.SaveChanges();
        }

        private List<Loan> All()
        {
            return repository.Loans.ToList();
        }

        private Loan Find(int id)
        {
            return repository.Loans.FirstOrDefault(x => x.Id == id);
        }
    }
}