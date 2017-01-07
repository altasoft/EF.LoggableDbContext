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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Master>().HasMany(x => x.Details).WithRequired().HasForeignKey(x => x.MasterId);
        }

        protected override void OnLogModelCreating(LogModelBuilder modelBuilder)
        {
            base.OnLogModelCreating(modelBuilder);
        }
    }
}
