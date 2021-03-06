﻿using System.Collections.Generic;

namespace System.Data.Entity.Configuration
{
    public class LoggableEntityConfiguration<TEntity> : LoggableEntityConfigurationBase<TEntity>
        where TEntity : class
    {
        public LoggableEntityConfiguration(Dictionary<string, LogModelConfiguration> config)
            : base(config) { }

        public LoggableEntityPropertyIgnoreConfiguration<TEntity> Include()
        {
            return CreateModelConfiguration(false);
        }

        public LoggableEntityPropertyIgnoreConfiguration<TEntity> Snapshot()
        {
            return CreateModelConfiguration(true);
        }

        protected LoggableEntityPropertyIgnoreConfiguration<TEntity> CreateModelConfiguration(bool isSnapshot)
        {
            if (!Config.ContainsKey(TypeName))
            {
                Config.Add(TypeName, new LogModelConfiguration()
                {
                    IsLoggable = true,
                    IsSnapshot = isSnapshot
                });
            }
            else
            {
                Config[TypeName].IsLoggable = true;
                Config[TypeName].IsSnapshot = isSnapshot;
            }

            return new LoggableEntityPropertyIgnoreConfiguration<TEntity>(Config);
        }

        public void Ignore()
        {
            Config[TypeName] = new LogModelConfiguration() { IsLoggable = false };
        }
    }
}
