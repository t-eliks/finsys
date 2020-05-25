namespace Web.ViewModels
{
    public class LimitViewModel
    {
        public int? Id { get; set; }
        
        public double Limit { get; set; }
        
        public int? CategoryId { get; set; }
        
        public double ActualAmount { get; set; }
    }
}