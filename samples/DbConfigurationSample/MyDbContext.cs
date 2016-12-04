using DbConfigurationSample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
