using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Goal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public double GoalAmount { get; set; }
        
        public virtual Category Category { get; set; }
    }
}