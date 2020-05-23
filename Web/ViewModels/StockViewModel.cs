using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class StockViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Company { get; set; }
        
        [Required]
        public int? Amount { get; set; }
    }
}