using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers.Investments
{
    public class StockController : Controller
    {
        private readonly Repository repository;

        private readonly string apiServerKey = "&apikey=b5244e77cd9c4eb39b448d73c687bee4";
        
        private static HttpClient cliet = new HttpClient();
        
        # region Constructor
        public StockController(Repository repository)
        {
            this.repository = repository;
        }
        # endregion
        
        # region Get methods

        [HttpGet]
        public IActionResult OpenStocksReport()
        {
            var userStocks = GetUserStocks();
            var stocks = GetStockValuesAndPrognosis(userStocks).Result;
            
            return View("~/Views/Investment/StocksReport.cshtml", stocks);
        }
        [HttpGet]
        public IActionResult OpenStockList()
        {
            return View("~/Views/Investment/StockList.cshtml", new StockListViewModel{Stocks = GetUserStocks()});
        }
        [HttpGet]
        public IActionResult CreateStock()
        {
            return View("~/Views/Investment/StockForm.cshtml", new StockViewModel());
        }
        [HttpGet]
        public IActionResult DeleteSelected(int id)
        {
            return PartialView("DeleteConfirmForm", new DeletionViewModel()
            {
                Id = id,
                Controller = "Stock",
                LtName = "akciją",
                Method = "DeleteConfirmed"
            });
        }
        
        [Route("stock/edit/{id:int}")]
        [HttpGet]
        public IActionResult UpdateSelected(int id)
        {
            var stock = GetById(id);
            if (stock == null)
            {
                return View("~/Views/Investment/StockList.cshtml", new StockListViewModel{Stocks = GetUserStocks()});
            }
            
            return View("~/Views/Investment/StockForm.cshtml", new StockViewModel
            {
                Id = stock.Id,
                Name = stock.Name,
                Amount = stock.Amount,
                Company = stock.Company
            });
        }
        # endregion
        
        # region Post methods
        [HttpPost]
        public IActionResult CreateNewStock(StockViewModel stock)
        {
            
            var error = ValidateData(stock); 
            
            if (!string.IsNullOrWhiteSpace(error))
            {
                TempData["Error"] = error;
                
                return View("~/Views/Investment/StockForm.cshtml", stock);
            }
            InsertStock(stock);
            
            TempData["Success"] = "Akcija sėkmingai pridėta!";
            
            return View("~/Views/Investment/StockList.cshtml", new StockListViewModel{Stocks = GetUserStocks()});
        }

        [HttpPost]
        public IActionResult UpdateStock(StockViewModel stock)
        {
            var error = ValidateData(stock); 
            
            if (!string.IsNullOrWhiteSpace(error))
            {
                TempData["Error"] = error;
                
                return View("~/Views/Investment/StockForm.cshtml", stock);
            }
            
            TempData["Success"] = "Akcija sėkmingai atnaujinta";
            
            UpdateSelectedStock(stock);
            
            return View("~/Views/Investment/StockList.cshtml", new StockListViewModel{Stocks = GetUserStocks()});
        }
        # endregion 
        
        # region Delete methods

        [HttpDelete]
        public IActionResult DeleteConfirmed(int id)
        {
            Delete(id);
            
            TempData["Success"] = "Akcija pašalinta";

            return Ok(Url.Action("OpenStockList", "Stock"));
        }
        # endregion
        
        # region Private helpers

        private void Delete(int id)
        {
            var stock = repository.Stocks.FirstOrDefault(x => x.Id == id);
            
            if (stock != null)
            {
                repository.Stocks.Remove(stock);
                repository.SaveChanges();
            }
        }
        private Stock GetById(int id)
        {
            return repository.Stocks.FirstOrDefault(x => x.Id == id);
        }
        private void UpdateSelectedStock(StockViewModel stock)
        {
            repository.Stocks.Update(new Stock
            {
                Id = stock.Id,
                Name = stock.Name,
                Company = stock.Company,
                Amount = stock.Amount.Value
            });
            
            repository.SaveChanges();
        }
        private void InsertStock(StockViewModel stock)
        {
            repository.Stocks.Add(new Stock
            {
                Name = stock.Name,
                Company = stock.Company,
                Amount = stock.Amount.Value
            });
            
            repository.SaveChanges();
        }
        private string ValidateData(StockViewModel stock)
        {
            if (string.IsNullOrWhiteSpace(stock.Name))
            {
                return "Akcijos pavadinimas yra privalomas";
            }
            
            if (string.IsNullOrWhiteSpace(stock.Company))
            {
                return "Įmonės pavadinimas yra privalomas";
            }

            if (!stock.Amount.HasValue)
            {
                return "Akcijų kiekis yra privalomas";
            }

            if (stock.Amount <= 0)
            {
                return "Akcijų kiekis turi būti daugiau už nulį";
            }

            return string.Empty;
        }
        private IList<Stock> GetUserStocks()
        {
            return repository.Stocks.ToList();
        }

        private async Task<StockReportViewModel> GetStockValuesAndPrognosis(IList<Stock> stocks)
        {
            var stocksList = new List<StockData>();
            DateTime startDate = DateTime.Now.AddMonths(-6);
            
            foreach (var stock in stocks)
            {
                string requestURL = $"https://api.twelvedata.com/time_series?symbol={stock.Name}&interval=1week{apiServerKey}&start_date={startDate.Date.ToShortDateString()}";
                
                var  response = await cliet.GetStreamAsync(requestURL);

                var stockReportData = await JsonSerializer.DeserializeAsync<StockData>(response);
                
                if (stockReportData.values != null)
                {
                    stockReportData.Name = stock.Name;
                    stockReportData.Company = stock.Company;
                    
                    stocksList.Add(stockReportData);
                }
                else
                {
                    if ( TempData["Error"] == null)
                    {
                        TempData["Error"] = $"Nepavyko gauti {stock.Company} {stock.Name}";
                    }
                    else
                    {
                        TempData["Error"] += $", {stock.Company} {stock.Name}";
                    }
                }
            }
            
            if (TempData["Error"] != null)
            {
                TempData["Error"] += " duomenų";
            }
            
            return new StockReportViewModel{stocksData = stocksList};
        }
        # endregion
    }
}