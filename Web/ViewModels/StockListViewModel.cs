using System.Collections.Generic;
using DataAccess.Models;

namespace Web.ViewModels
{
    public class StockListViewModel
    {
        public IList<Stock> Stocks { get; set; }
    }
}