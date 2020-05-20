namespace DataAccess.Models
{
    public class Expense : Transaction
    {
        public string Origin { get; set; }
        
        public virtual Category Category { get; set; }
    }
}
