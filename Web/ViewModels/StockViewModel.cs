using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class StockViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Pavadinimas", Prompt = "Akcijos pavadinimas pvz. AMZN")]
        [Required(ErrorMessage = "Laukas yra privalomas")]
        [MinLength(1, ErrorMessage = "Įveskite pavadinimą")]
        [MaxLength(10, ErrorMessage = "Pavadinimas per ilgas, reikia iki 10 simbolių")]
        public string Name { get; set; }
        
        [Display(Name = "Kompanija", Prompt = "Kompanijos pavadinimas pvz. UAB \"Autista\"")]
        [Required(ErrorMessage = "Laukas yra privalomas")]
        [MinLength(1, ErrorMessage = "Įveskite pavadinimą")]
        [MaxLength(40, ErrorMessage = "Pavadinimas per ilgas, reikia iki 40 simbolių")]
        public string Company { get; set; }
        
        [Display(Name = "Kiekis", Prompt = "Kiekis")]
        [Required(ErrorMessage = "Laukas yra privalomas")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Skaičius nuo 0")]
        public int? Amount { get; set; }
    }
}