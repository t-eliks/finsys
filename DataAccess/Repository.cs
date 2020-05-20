﻿using DataAccess.Models;
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
        
        public virtual DbSet<Loan> Loans { get; set; }

        public DbSet<Income> Income { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("User");
            builder.Entity<FinancialGuru>().ToTable("FinancialGuru");
            builder.Entity<Expense>().ToTable("Expense");
            builder.Entity<Loan>().ToTable("Loan");
        }
    }
}
