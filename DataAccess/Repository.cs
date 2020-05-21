using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class Repository : DbContext
    {
        public Repository(DbContextOptions<Repository> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<FinancialGuru> FinancialGurus { get; set; }

        public virtual DbSet<Expense> Expenses { get; set; }

        public virtual DbSet<Income> Income { get; set; }
        
        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Goal> Goals { get; set; }
        
        public virtual DbSet<Limit> Limits { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("User");
            builder.Entity<FinancialGuru>().ToTable("FinancialGuru");
            builder.Entity<Expense>().ToTable("Expense");
            builder.Entity<Category>().ToTable("Category");
        }
    }
}