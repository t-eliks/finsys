using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class RealEstate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public User Owner { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
        
        public bool IsRented { get; set; }

        public float SquareSpace { get; set; }
        
        public int RoomReNumber { get; set; }

        public float Valuation { get; set; }
    }
}