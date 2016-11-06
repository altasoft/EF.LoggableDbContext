using System.Collections.Generic;

namespace System.Data.Entity.Configuration
{
    public abstract class LoggableEntityConfigurationBase<TEntity>
        where TEntity : class
    {
        protected Dictionary<string, LogModelConfiguration> Config { get; set; }
        protected string TypeName { get; set; }

        public LoggableEntityConfigurationBase(Dictionary<string, LogModelConfiguration> config)
        {
            this.Config = config;
            this.TypeName = typeof(TEntity).FullName;
        }
    }
}
