﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class IncomeViewModel
    {
        public int Id { get; set; }

        [Required]
        public double? Amount { get; set; }

        [Required]
        public string Source { get; set; }

        public string Comment { get; set; }
        
        public string Category { get; set; }
        
        public int? CategoryId { get; set; }
        
        public IList<CategoryViewModel> AvailableCategories { get; set; }
        
        public DateTime? CreationDate { get; set; }
    }
}