using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers.Investment
{
    public class InvestmentController : Controller
    {
        private readonly Repository _repository;
        
        # region Constructor
        public InvestmentController(Repository repository)
        {
            this._repository = repository;
        }
        # endregion
        
        # region Get methods
        
        [HttpGet]
        public IActionResult OpenStockList()
        {
            return View("StockList", new StockListViewModel{Stocks = GetUserStocks()});
        }
        [HttpGet]
        public IActionResult CreateStock()
        {
            return View("StockFrom", new StockViewModel());
        }
        [HttpGet]
        public IActionResult DeleteSelected(int id)
        {
            return PartialView("DeleteConfirmForm", new DeleteViewModel
            {
                Id = id,
                Controller = "Investment",
                Message = "akciją",
                Method = "DeleteConfirmed"
            });
        }
        
        [Route("stock/edit/{id:int}")]
        [HttpGet]
        public IActionResult UpdateSelected(int id)
        {
            var stock = _repository.Stocks.FirstOrDefault(x => x.Id == id);
            if (stock == null)
            {
                return View("StockList", new StockListViewModel{Stocks = GetUserStocks()});
            }
            return View("StockFrom", new StockViewModel
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
            if (!ModelState.IsValid)
            {
                return View("StockFrom", stockViewModel);
            }
            string errorMessage = ValidateData(stockViewModel);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                TempData["Error"] = errorMessage;
                return View("StockFrom", stockViewModel);
            }
            InsertStock(stockViewModel);
            TempData["Success"] = "Akcija sėkmingai pridėta! Atsipalaiduokite.";
            return View("StockList", new StockListViewModel
            {
                Stocks = GetUserStocks()
                
            });
        }

        [HttpPost]
        public IActionResult UpdateStock(StockViewModel stock)
        {
            if (!ModelState.IsValid)
            {
                return View("StockFrom", stock);
            }
            var error = ValidateData(stock);
            if (!string.IsNullOrEmpty(error))
            {
                TempData["Error"] = error;
                return View("StockFrom", stock);
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
            var stock = _repository.Stocks.FirstOrDefault(x => x.Id == id);
            if (stock != null)
            {
                _repository.Stocks.Remove(stock);
                _repository.SaveChanges();
            }
            TempData["Success"] = "Akcija pašalinta";

            return Ok(Url.Action("OpenStockList", "Investment"));
        }
        # endregion
        
        # region Private helpers

        private void UpdateStock(StockViewModel stock, int id)
        {
            _repository.Stocks.Update(new Stock
            {
                Id = id,
                Name = stock.Name,
                Company = stock.Company,
                Amount = stock.Amount
            });
            _repository.SaveChanges();
        }
        private void InsertStock(StockViewModel stock)
        {
            _repository.Stocks.Add(new Stock
            {
                Name = stock.Name,
                Company = stock.Company,
                Amount = stock.Amount
            });
            _repository.SaveChanges();
        }
        private string ValidateData(StockViewModel stockViewModel)
        {
            if (_repository.Stocks.FirstOrDefault(x=>
                x.Company == stockViewModel.Company &&
                x.Name == stockViewModel.Name &&
                x.Id != stockViewModel.Id) != null)
            {
                return "Akcija tokiu įmonės pavadinimu ir vardu pas jus jau registruota";
            }
            return String.Empty;
        }
        private IList<Stock> GetUserStocks()
        {
            return _repository.Stocks.ToList();
        }
        # endregion
    }
}