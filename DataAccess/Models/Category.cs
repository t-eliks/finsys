using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public IList<Limit> Limits { get; set; }
        
        public IList<Goal> Goals { get; set; }
        
        public IList<Income> Incomes { get; set; }
        
        public IList<Expense> Expenses { get; set; }
    }
}