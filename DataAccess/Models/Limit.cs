using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Limit
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public double LimitAmount { get; set; }
        
        public virtual Category Category { get; set; }
    }
}