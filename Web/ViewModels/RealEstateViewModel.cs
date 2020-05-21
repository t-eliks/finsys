namespace Web.ViewModels
{
    public class RealEstateViewModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Address { get; set; }
        
        public bool IsRented { get; set; }

        public float? SquareSpace { get; set; }
        
        public int? RoomNumber { get; set; }

        public float? Valuation { get; set; }
    }
}