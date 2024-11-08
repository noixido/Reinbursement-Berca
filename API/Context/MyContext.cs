using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Context
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountDetail> AccountDetails { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Reimbursement> Reimbursements { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ReimbursementProfiling> ReimbursementProfilings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definisikan composite key untuk ReimbursementProfiling
            modelBuilder.Entity<ReimbursementProfiling>()
                .HasKey(rp => new { rp.Id_Account, rp.Id_Reimbursement });
        }

    }
}
