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
    }
}