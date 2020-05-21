using System;
using System.Collections.Generic;
using DataAccess.Models;

namespace Web.ViewModels
{
    public class StockData
    {
        /*
         * Won't map to decimal, because Json values is strings
         * and don't use setter to set value
         */
        public DateTime datetime { get; set; }
        
        public string open { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string close { get; set; }
        public string volume { get; set; }
    } 
    public class StockReportViewModel
    {
        public IList<StockData> values { get; set; }
        
        public Stock stockData { get; set; }

        public IList<Tuple<DateTime, double>> GetCharData()
        {
            var list = new List<Tuple<DateTime, double>>();
            foreach (var stockData in values)
            {
                list.Insert(0, new Tuple<DateTime, double>(stockData.datetime, Convert.ToDouble(stockData.close.Replace('.',','))));
            }

            return list;
        }
    }
}