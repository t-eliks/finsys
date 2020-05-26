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
        
        public virtual DbSet<Stock> Stocks { get; set; }

        public virtual DbSet<Income> Income { get; set; }
        
        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Goal> Goals { get; set; }
        
        public virtual DbSet<Limit> Limits { get; set; }
        
        public virtual DbSet<RealEstate> RealEstate { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("User");
            builder.Entity<FinancialGuru>().ToTable("FinancialGuru");
            builder.Entity<Expense>().ToTable("Expense");
            builder.Entity<Stock>().ToTable("Stock");
            builder.Entity<Category>().ToTable("Category");

            builder.Entity<Category>()
                .HasMany(x => x.Limits)
                .WithOne(x => x.Category)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Category>()
                .HasMany(x => x.Goals)
                .WithOne(x => x.Category)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Category>()
                .HasMany(x => x.Expenses)
                .WithOne(x => x.Category);
            
            builder.Entity<Category>()
                .HasMany(x => x.Incomes)
                .WithOne(x => x.Category);
            
            builder.Entity<Income>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Incomes)
                .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<Expense>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Expenses)
                .OnDelete(DeleteBehavior.ClientNoAction);
        }
    }
}