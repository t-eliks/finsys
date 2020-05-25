using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class StockViewModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Company { get; set; }
        
        public int? Amount { get; set; }
    }
}