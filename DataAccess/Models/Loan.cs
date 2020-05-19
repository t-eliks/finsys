using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Loan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public float Sum { get; set; }

        public float Interest { get; set; }

        public DateTime Term { get; set; }

        public string Provider { get; set; }

        public float ReturnedSum { get; set; }

        public string Type { get; set; }
    }
}