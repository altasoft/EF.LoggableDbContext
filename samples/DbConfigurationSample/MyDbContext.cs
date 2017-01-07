using DbConfigurationSample.Models;
using System.Data.Entity;

namespace DbConfigurationSample
{
    public class MyDbContext : LoggableDbContext
    {
        public MyDbContext(int userId)
            : base(userId)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerOp> CustomerOps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().Property(x => x.RowVersion).IsRowVersion();
            modelBuilder.Entity<Account>().Property(x => x.RowVersion).IsRowVersion();

            modelBuilder.Entity<Customer>().HasMany(x => x.Accounts).WithRequired().HasForeignKey(x => x.CustomerId);
            modelBuilder.Entity<CustomerOp>().HasRequired(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
        }

        protected override void OnLogModelCreating(LogModelBuilder modelBuilder)
        {
            base.OnLogModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerOp>().Ignore();

            modelBuilder.Entity<Customer>().Include().Ignore(x => x.Age);

            modelBuilder.Entity<Account>().Include().IgnoreAll().Include(x => x.BankName);
            modelBuilder.Entity<Account>().Include().IgnoreAll().Include(x => x.AccountNumber);
        }
    }
}
