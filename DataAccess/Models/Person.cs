using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public abstract class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }
    }
}
