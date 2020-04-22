using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double Amount { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string Note { get; set; }
    }
}
