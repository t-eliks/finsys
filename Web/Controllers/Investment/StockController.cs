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

namespace Web.Controllers.Investment
{
    public class StockController : Controller
    {
        private readonly Repository _repository;

        private readonly string _APIServerKey = "&apikey=b5244e77cd9c4eb39b448d73c687bee4";
        
        private static HttpClient _cliet = new HttpClient();
        
        # region Constructor
        public StockController(Repository repository)
        {
            this._repository = repository;
        }
        # endregion
        
        # region Get methods

        [HttpGet]
        public IActionResult OpenStocksReport()
        {
            var userStocks = GetUserStocks();
            var stocks = GetStockValuesAndPrognosis(userStocks).Result;
            return View("StocksReport", stocks);
        }
        [HttpGet]
        public IActionResult OpenStockList()
        {
            return View("StockList", new StockListViewModel{Stocks = GetUserStocks()});
        }
        [HttpGet]
        public IActionResult CreateStock()
        {
            return View("StockForm", new StockViewModel());
        }
        [HttpGet]
        public IActionResult DeleteSelected(int id)
        {
            return PartialView("DeleteConfirmForm", new DeleteViewModel
            {
                Id = id,
                Controller = "Stock",
                Message = "akciją",
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
                return View("StockList", new StockListViewModel{Stocks = GetUserStocks()});
            }
            return View("StockForm", new StockViewModel
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
        public IActionResult CreateNewStock(StockViewModel stockViewModel)
        {
            if (ValidateData())
            {
                TempData["Error"] = "Neteisingai įvesti duomenys";
                return View("StockForm", stockViewModel);
            }
            InsertStock(stockViewModel);
            TempData["Success"] = "Akcija sėkmingai pridėta! Atsipalaiduokite.";
            
            return View("StockList", new StockListViewModel{Stocks = GetUserStocks()});
        }

        [HttpPost]
        public IActionResult UpdateStock(StockViewModel stock)
        {
            if (ValidateData())
            {
                TempData["Error"] = "Neteisingai įvesti duomenys";
                return View("StockForm", stock);
            }
            TempData["Success"] = "Akcija sėkmingai atnaujinta";
            UpdateStock(stock, stock.Id);
            
            return View("StockList", new StockListViewModel{Stocks = GetUserStocks()});
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
            var stock = _repository.Stocks.FirstOrDefault(x => x.Id == id);
            if (stock != null)
            {
                _repository.Stocks.Remove(stock);
                _repository.SaveChanges();
            }
        }
        private Stock GetById(int id)
        {
            return _repository.Stocks.FirstOrDefault(x => x.Id == id);
        }
        private void UpdateStock(StockViewModel stock, int id)
        {
            _repository.Stocks.Update(new Stock
            {
                Id = id,
                Name = stock.Name,
                Company = stock.Company,
                Amount = stock.Amount.Value
            });
            _repository.SaveChanges();
        }
        private void InsertStock(StockViewModel stock)
        {
            _repository.Stocks.Add(new Stock
            {
                Name = stock.Name,
                Company = stock.Company,
                Amount = stock.Amount.Value
            });
            _repository.SaveChanges();
        }
        private bool ValidateData()
        {
            return !ModelState.IsValid;
        }
        private IList<Stock> GetUserStocks()
        {
            return _repository.Stocks.ToList();
        }
        /*
         * Very bad code
         * I surrender in living
         */
        private async Task<StockReportViewModel> GetStockValuesAndPrognosis(IList<Stock> stocks)
        {
            var stocksList = new List<StockData>();
            DateTime startDate = DateTime.Now.AddMonths(-6);
            foreach (var stock in stocks)
            {
                string requestURL = $"https://api.twelvedata.com/time_series?symbol={stock.Name}&interval=1week{_APIServerKey}&start_date={startDate.Date.ToShortDateString()}";
                var  response = await _cliet.GetStreamAsync(requestURL);

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