using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class DataExportViewModel
    {
        public bool ShouldIncludeIncome { get; set; }
        public bool ShouldIncludeExpenses { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? StartingDate { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? EndingDate { get; set; }
    }
}