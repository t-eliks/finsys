using System;
using System.Collections.Generic;
using DataAccess.Models;

namespace Web.ViewModels
{
    /*
     * That's a lot of bad code
     */
    public class StockValues
    {
        /*
         * Won't map to decimal, because Json values is strings
         * and don't use setter to set value, using Convert,
         * from API floating point values comes with '.' not ','
         * fails to convert
         */
        public DateTime datetime { get; set; }
        
        public string open { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string close { get; set; }
        public string volume { get; set; }
    }
    public class StockData
    {
        public IList<StockValues> values { get; set; }
        
        public string Name { get; set; }
        
        public string Company { get; set; }

        public IList<Tuple<DateTime, double>> GetCharData()
        {
            var list = new List<Tuple<DateTime, double>>();
            foreach (var stockData in values)
            {
                list.Insert(0, 
                    new Tuple<DateTime, double>(stockData.datetime, 
                        Convert.ToDouble(stockData.close.Replace('.',','))));
            }

            return list;
        }
    }
    public class StockReportViewModel
    {
        public IList<StockData> stocksData { get; set; }
    }
}