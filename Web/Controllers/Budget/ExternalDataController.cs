using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers.Budget
{
    public class ExternalDataController : Controller
    {
        private readonly Repository repository;

        public ExternalDataController(Repository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Route("export")]
        public IActionResult OpenExportPage()
        {
            return View("DataExportPage", new DataExportViewModel {StartingDate = DateTime.Today, EndingDate = DateTime.Today.Add(TimeSpan.FromDays(7))});
        }

        public IActionResult ExportData(DataExportViewModel viewModel)
        {
            string validation = ValidateData(viewModel);
            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;
                return View("DataExportPage", viewModel);
            }

            IList<Income> incomeList = null;
            IList<Expense> expenses = null;
            if (viewModel.ShouldIncludeIncome)
            {
                incomeList = FetchUserIncomeList(viewModel.StartingDate.Value, viewModel.EndingDate.Value);
            }
            if (viewModel.ShouldIncludeExpenses)
            {
                expenses = SelectUsersExpenses(viewModel.StartingDate.Value, viewModel.EndingDate.Value);
            }
            
            return GenerateDocument(incomeList, expenses);
        }

        private string ValidateData(DataExportViewModel viewModel)
        {
            if (!viewModel.ShouldIncludeIncome && !viewModel.ShouldIncludeExpenses)
            {
                return "Reikia pasirinkti kažką eksportuoti!";
            }
            if (viewModel.StartingDate == null || viewModel.EndingDate == null)
            {
                return "Pasirinkite laiko intervalą duomenų eksportui";
            }

            return string.Empty;
        }

        private IActionResult GenerateDocument(IList<Income> incomeList, IList<Expense> expenses)
        {
            string filePath = Path.GetTempFileName();
            string fileName = Regex.Replace(DateTime.Now.ToShortDateString(), @"\s+", "") + "-export-data.csv";
            
            using (var w = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                WriteDataToFile(incomeList, expenses, w);
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            
            return File(fileBytes, "application/octet-stream", fileName);
        }

        private static void WriteDataToFile(IList<Income> incomeList, IList<Expense> expenses, StreamWriter writer)
        {
            if (incomeList != null && incomeList.Count > 0)
            {
                WriteIncomeData(incomeList, writer);
            }

            if (expenses != null && expenses.Count > 0)
            {
                WriteExpenseData(expenses, writer);
            }
        }
        private static void WriteIncomeData(IList<Income> incomeList, StreamWriter writer)
        {
            var header = "Kategorija,Šaltinis,Kiekis,Komentaras,Sukūrimo data";
            writer.WriteLine("Pajamos");
            writer.WriteLine(header);
            foreach (var income in incomeList)
            {
                string category = income.Category != null ? income.Category.Name : "-";
                var line = $"{category},{income.Source},{income.Amount},{income.Comment},{income.CreationDate}";
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        private static void WriteExpenseData(IList<Expense> expenses, StreamWriter writer)
        {
            var header = "Kategorija,Paskirtis,Kiekis,Pastaba,Sukūrimo data";
            writer.WriteLine("Išlaidos");
            writer.WriteLine(header);
            foreach (var expense in expenses)
            {
                string category = expense.Category != null ? expense.Category.Name : "-";
                var line = $"{category},{expense.Origin},{expense.Amount},{expense.Comment},{expense.CreationDate}";
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        private IList<Income> FetchUserIncomeList(DateTime startingDate, DateTime endingDate)
        {
            return repository.Income.Where(x => x.CreationDate <= endingDate && x.CreationDate >= startingDate)
                .ToList();
        }

        private IList<Expense> SelectUsersExpenses(DateTime startingDate, DateTime endingDate)
        {
            return repository.Expenses.Where(x => x.CreationDate <= endingDate && x.CreationDate >= startingDate)
                .ToList();
        }
    }
}