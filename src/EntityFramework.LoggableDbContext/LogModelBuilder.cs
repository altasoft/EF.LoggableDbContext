using System.Data.Entity.Configuration;
using System.Collections.Generic;

namespace System.Data.Entity
{
    public sealed class LogModelBuilder
    {
        public bool LogAllEntities { get; set; }

        /// <summary>
        /// Key - Entity FullName (include namespace)
        /// </summary>
        public Dictionary<string, LogModelConfiguration> Configurations { get; private set; }

        public LogModelBuilder()
        {
            LogAllEntities = true;
        }

        public LoggableEntityConfiguration<TEntity> Entity<TEntity>() where TEntity : class
        {
            if (Configurations == null)
                Configurations = new Dictionary<string, LogModelConfiguration>();

            return new LoggableEntityConfiguration<TEntity>(Configurations);
        }
    }
}
