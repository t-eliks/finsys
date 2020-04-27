using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        [Required]
        public double? Amount { get; set; }

        [Required]
        public string Origin { get; set; }

        public string Comment { get; set; }
    }
}
