using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class LoanViewModel
    {
        public int Id { get; set; }
        
        public float? Sum { get; set; }

        public float? Interest { get; set; }

        [DataType(DataType.Date)]
        public DateTime Term { get; set; }

        public string Provider { get; set; }

        public float? ReturnedSum { get; set; }

        public string Type { get; set; }
    }
}