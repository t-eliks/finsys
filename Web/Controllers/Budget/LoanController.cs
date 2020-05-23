using System.Linq;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
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
            return View("LoanPlan", new LoanPlanViewModel
            {
                Loans = repository.Loans.ToList()
            });
        }

        [HttpPost]
        public IActionResult SubmitLoanQuery()
        {
            return NotFound();
        }

        private string ValidateData(LoanViewModel viewModel)
        {
            if (viewModel.Sum < 0)
            {
                return "Būtina įvesti sumą";
            }

            if (viewModel.Interest < 0)
            {
                return "Būtina įvesti palūkanas";
            }

            if (viewModel.ReturnedSum < 0)
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