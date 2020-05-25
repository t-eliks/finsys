using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }
        
        public double? Amount { get; set; }
        
        public string Origin { get; set; }

        public string Comment { get; set; }
        
        public string Category { get; set; }
        
        public int? CategoryId { get; set; }
        
        public IList<CategoryViewModel> AvailableCategories { get; set; }
        
        public DateTime? CreationDate { get; set; }
    }
}
