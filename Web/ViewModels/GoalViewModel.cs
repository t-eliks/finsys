namespace Web.ViewModels
{
    public class GoalViewModel
    {
        public int? Id { get; set; }
        
        public double Goal { get; set; }
        
        public int? CategoryId { get; set; }
        
        public double ActualAmount { get; set; }
    }
}