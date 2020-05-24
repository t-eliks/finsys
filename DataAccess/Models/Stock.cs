using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Stock
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public int Amount { get; set; }
        
    }
}