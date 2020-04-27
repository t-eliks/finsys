namespace DataAccess.Models
{
    public class Expense : Transaction
    {
        public string Purpose { get; set; }

        public string Origin { get; set; }
    }
}
