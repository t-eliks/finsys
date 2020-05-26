namespace DataAccess.Models
{
    public class Income : Transaction
    {
        public string Source { get; set; }
        
        public virtual Category Category { get; set; }
    }
}