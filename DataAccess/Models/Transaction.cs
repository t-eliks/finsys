using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double Amount { get; set; }
        
        public DateTime CreationDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string Comment { get; set; }
    }
}
