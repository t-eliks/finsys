using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        [Required]
        public double? Amount { get; set; }

        [Required]
        public string Purpose { get; set; }

        public string Note { get; set; }
    }
}
