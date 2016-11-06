//using System.Data.Common;
using System.Collections.Generic;
using System.Data.Entity.Configuration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Logging;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity
{
    public abstract class LoggableDbContext : DbContext
    {
        private LogModelBuilder _logModelBuilder = new LogModelBuilder();
        public LogModelBuilder LogModelBuilder
        {
            get
            {
                return _logModelBuilder;
            }
        }

        public int UserId { get; set; }

        public LoggableDbContext(int userId)
            : base()
        {
            InitInternal(userId);
        }

        public LoggableDbContext(int userId, string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            InitInternal(userId);
        }

        //public LoggableDbContext(int userId, DbConnection existingConnection, bool contextOwnsConnection)
        //    : base(existingConnection, contextOwnsConnection)
        //{
        //    InitInternal(userId);
        //}

        public DbSet<DatabaseLog> DatabaseLog { get; set; }
        public DbSet<EntityLog> EntityLog { get; set; }
        public DbSet<EntityPropertiesLog> EntityPropertiesLog { get; set; }
        public DbSet<EntitySnapshotLog> EntitySnapshotLog { get; set; }


        private void InitInternal(int userId)
        {
            UserId = userId;

            Init();
            OnLogModelCreating(_logModelBuilder);
        }

        protected virtual void Init() { }

        protected virtual void OnLogModelCreating(LogModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatabaseLog>().Ignore();
            modelBuilder.Entity<EntityLog>().Ignore();
            modelBuilder.Entity<EntityPropertiesLog>().Ignore();
            modelBuilder.Entity<EntitySnapshotLog>().Ignore();
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DatabaseLog>().ToTable("DatabaseLog", "log");
            modelBuilder.Entity<DatabaseLog>()
                .Property(x => x.Timestamp)
                .HasColumnType("datetime");



            modelBuilder.Entity<EntityLog>().ToTable("EntityLog", "log");
            modelBuilder.Entity<EntityLog>()
                .Property(x => x.TypeName)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(200);

            modelBuilder.Entity<EntityLog>()
                .Property(x => x.TypeFullName)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(500);

            modelBuilder.Entity<EntityLog>()
                .Property(x => x.EntityKey)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<EntityLog>()
                .Property(x => x.EntityKeyValue)
                .HasMaxLength(200);

            modelBuilder.Entity<EntityLog>()
                .HasRequired(x => x.DatabaseLog)
                .WithMany(x => x.EntityLog)
                .HasForeignKey(x => x.DatabaseLogId);



            modelBuilder.Entity<EntityPropertiesLog>().ToTable("EntityPropertiesLog", "log");
            modelBuilder.Entity<EntityPropertiesLog>()
                .Property(x => x.PropertyName)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(200);

            modelBuilder.Entity<EntityPropertiesLog>()
                .Property(x => x.NewValue)
                .IsMaxLength();

            modelBuilder.Entity<EntityPropertiesLog>()
                .Property(x => x.OldValue)
                .IsMaxLength();

            modelBuilder.Entity<EntityPropertiesLog>()
                .HasRequired(x => x.EntityLog)
                .WithMany()
                .HasForeignKey(x => x.EntityLogId);




            //ToTable("EntitySnapshotLog", "log");

            //Property(x => x.XmlValue)
            //    .HasColumnType("xml");

            //HasRequired(x => x.EntityLog).WithMany().HasForeignKey(x => x.EntityLogId);
        }


        //public DatabaseLog LogEntityProperties(string entityTypeName, string entityKey, string entityKeyValue, string propertyName, string oldValue, string newValue, DatabaseLog dbLog = null)
        //{
        //    return LogEntityProperties(entityTypeName,
        //        entityTypeName,
        //        entityKey,
        //        entityKeyValue,
        //        true,
        //        new List<EntityPropertiesLog>
        //        {
        //            new EntityPropertiesLog
        //            {
        //                PropertyName = propertyName,
        //                OldValue = oldValue,
        //                NewValue = newValue
        //            }
        //        },
        //        dbLog);
        //}

        //public DatabaseLog LogEntityProperties(string entityTypeName, string entityTypeFullName, string entityKey, string entityKeyValue, bool hasMultipleKey, List<EntityPropertiesLog> entityProperties, DatabaseLog dbLog = null)
        //{
        //    if (entityProperties == null || !entityProperties.Any())
        //        throw new ArgumentException("Entity properties is required.");

        //    if (dbLog == null)
        //    {
        //        dbLog = new DatabaseLog()
        //        {
        //            Timestamp = DateTime.Now,
        //            UserId = UserId
        //        };
        //    }

        //    var dbEntityLog = new EntityLog()
        //    {
        //        DatabaseLog = dbLog,
        //        TypeName = entityTypeName,
        //        TypeFullName = entityTypeFullName,
        //        EntityKey = entityKey,
        //        EntityKeyValue = entityKeyValue,
        //        HasMultipleKey = hasMultipleKey
        //    };

        //    entityProperties.ForEach(x => x.EntityLog = dbEntityLog);
        //    this.EntityPropertiesLog.AddRange(entityProperties);

        //    return dbLog;
        //}

        public override int SaveChanges()
        {
            using (DbContextTransaction transaction = this.Database.BeginTransaction())
            {
                var objContext = ((IObjectContextAdapter)this).ObjectContext;
                objContext.SaveChanges(SaveOptions.DetectChangesBeforeSave);

                var propertiesLog = GenerateEntityPropertiesLog(UserId);

                objContext.AcceptAllChanges();

                if (propertiesLog != null)
                    this.EntityPropertiesLog.AddRange(propertiesLog);

                int result = base.SaveChanges();

                transaction.Commit();

                return result;
            }
        }

        private List<EntityPropertiesLog> GenerateEntityPropertiesLog(int userId)
        {
            var entries = this.ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted).ToList();
            if (entries.Any())
            {
                DatabaseLog dbLog = new DatabaseLog()
                {
                    Timestamp = DateTime.Now,
                    UserId = userId
                };

                var propertiesLog = new List<EntityPropertiesLog>();

                foreach (var entry in entries)
                {
                    if (!IsEntityLogable(entry.Entity))
                        continue;

                    EntityLog entityLog = new EntityLog() { DatabaseLog = dbLog };

                    Type entiryType = entry.Entity.GetType();
                    entityLog.TypeName = entiryType.Name;
                    entityLog.TypeFullName = entiryType.FullName;

                    var entityKeys = GetEntityKeys(this, entry.Entity);
                    if (entityKeys == null || !entityKeys.Any())
                        throw new ArgumentException("entityKeys is empty");

                    entityLog.HasMultipleKey = entityKeys.Count > 1;
                    entityLog.EntityKey = string.Join(",", entityKeys.Select(x => x.Name));

                    var entityKeyValues = GetEntityKeyValues(entityKeys, entry);
                    entityLog.EntityKeyValue = string.Join(",", entityKeyValues);


                    string entryTypeName = entry.Entity.GetType().FullName;
                    LogModelConfiguration config = null;
                    if (_logModelBuilder.Configurations != null)
                        _logModelBuilder.Configurations.TryGetValue(entryTypeName, out config);

                    DbPropertyValues oldValues = null, newValues = null;

                    if (entry.State == EntityState.Added)
                        newValues = entry.CurrentValues;
                    else if (entry.State == EntityState.Modified)
                    {
                        oldValues = entry.OriginalValues;
                        newValues = entry.CurrentValues;
                    }
                    else
                        oldValues = entry.OriginalValues;

                    var tempPropsLog = GenerateEntityPropertiesLog(oldValues, newValues, config, null);
                    tempPropsLog.ForEach(x => x.EntityLog = entityLog);
                    propertiesLog.AddRange(tempPropsLog);
                }

                return propertiesLog;
            }

            return null;
        }

        private List<EntityPropertiesLog> GenerateEntityPropertiesLog(DbPropertyValues oldValues, DbPropertyValues newValues, LogModelConfiguration config, string complexPropPrefix)
        {
            if (oldValues == null && newValues == null)
                throw new ArgumentNullException("Entity not changed or not tracked, properties[oldValues, newValues]");

            var propertiesLog = new List<EntityPropertiesLog>();

            if (newValues != null && oldValues == null) // Create
            {
                foreach (var propName in newValues.PropertyNames)
                {
                    object propValue;
                    if ((propValue = newValues[propName]) != null)
                    {
                        string entityPropName = string.IsNullOrWhiteSpace(complexPropPrefix) ? propName : string.Join(".", complexPropPrefix, propName);

                        if (propValue is DbPropertyValues)
                        {
                            propertiesLog.AddRange(GenerateEntityPropertiesLog(null, (DbPropertyValues)propValue, config, entityPropName));
                        }
                        else if (IsEntityPropertyLogable(config, entityPropName))
                        {
                            propertiesLog.Add(new EntityPropertiesLog()
                            {
                                PropertyName = entityPropName,
                                NewValue = FormatPropertyValue(propValue)
                            });
                        }
                    }
                }
            }
            else if (newValues != null && oldValues != null) // Update
            {
                foreach (var propName in newValues.PropertyNames)
                {
                    string entityPropName = string.IsNullOrWhiteSpace(complexPropPrefix) ? propName : string.Join(".", complexPropPrefix, propName);

                    object propValueOld = oldValues[propName];
                    object propValueNew = newValues[propName];

                    if (!object.Equals(propValueOld, propValueNew))
                    {
                        if (propValueOld is DbPropertyValues || propValueNew is DbPropertyValues)
                        {
                            propertiesLog.AddRange(GenerateEntityPropertiesLog(propValueOld as DbPropertyValues, propValueNew as DbPropertyValues, config, entityPropName));
                        }
                        else if (IsEntityPropertyLogable(config, entityPropName))
                        {
                            propertiesLog.Add(new EntityPropertiesLog()
                            {
                                PropertyName = entityPropName,
                                OldValue = FormatPropertyValue(propValueOld),
                                NewValue = FormatPropertyValue(propValueNew),
                            });
                        }
                    }
                }
            }
            else // Delete
            {
                foreach (var propName in oldValues.PropertyNames)
                {
                    object propValue;
                    if ((propValue = oldValues[propName]) != null)
                    {
                        string entityPropName = string.IsNullOrWhiteSpace(complexPropPrefix) ? propName : string.Join(".", complexPropPrefix, propName);

                        if (propValue is DbPropertyValues)
                        {
                            propertiesLog.AddRange(GenerateEntityPropertiesLog((DbPropertyValues)propValue, null, config, entityPropName));
                        }
                        else if (IsEntityPropertyLogable(config, entityPropName))
                        {
                            propertiesLog.Add(new EntityPropertiesLog()
                            {
                                PropertyName = entityPropName,
                                OldValue = FormatPropertyValue(propValue)
                            });
                        }
                    }
                }
            }

            return propertiesLog;
        }

        #region Utility Methods

        public static List<EdmProperty> GetEntityKeys(DbContext context, object entity)
        {
            return GetEntityKeys(((IObjectContextAdapter)context).ObjectContext, entity);
        }

        public static List<EdmProperty> GetEntityKeys(ObjectContext context, object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity must be specified.");

            Type entityType = entity.GetType();
            MethodInfo method = context.GetType().GetMethod("CreateObjectSet", new Type[0]).MakeGenericMethod(entityType);
            dynamic methodRetVal = method.Invoke(context, null);
            var entityKeys = ((ReadOnlyMetadataCollection<EdmProperty>)methodRetVal.EntitySet.ElementType.KeyProperties).ToList();

            return entityKeys;
        }

        public static List<EdmProperty> GetEntityKeys<T>(DbContext context, T entity) where T : class
        {
            return GetEntityKeys<T>(((IObjectContextAdapter)context).ObjectContext, entity);
        }

        public static List<EdmProperty> GetEntityKeys<T>(ObjectContext context, T entity) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException("entity must be specified.");

            return context.CreateObjectSet<T>().EntitySet.ElementType.KeyProperties.ToList();
        }

        public static List<object> GetEntityKeyValues(List<EdmProperty> keyProperties, DbEntityEntry entry)
        {
            if (keyProperties == null)
                throw new ArgumentNullException("keyProperties");

            if (!keyProperties.Any())
                throw new ArgumentException("keyProperties is empty");

            var result = new List<object>(keyProperties.Count);

            foreach (var keyProp in keyProperties)
            {
                var prop = entry.Property(keyProp.Name);
                result.Add(entry.State == EntityState.Deleted ? prop.OriginalValue : prop.CurrentValue);
            }

            return result;
        }

        private bool IsEntityLogable(object entity)
        {
            if (_logModelBuilder.Configurations == null)
                return _logModelBuilder.LogAllEntities;

            LogModelConfiguration config = null;

            if (_logModelBuilder.Configurations.TryGetValue(entity.GetType().FullName, out config))
            {
                if (!config.IsLogable)
                    return false;

                if (config.IgnoreAllProperties && (config.Properties == null || !config.Properties.Any(x => x.Value == true)))
                    return false;
            }
            else if (!_logModelBuilder.LogAllEntities)
                return false;

            return true;
        }

        // todo: unda daematos property-is tipze shemowmebac (type ignore)
        private bool IsEntityPropertyLogable(LogModelConfiguration config, string propertyName)
        {
            if (config == null)
                return true;

            if (config.Properties != null && config.Properties.Any(x => x.Key.Equals(propertyName)))
                return config.Properties[propertyName];
            else
                return !config.IgnoreAllProperties;
        }

        private string FormatPropertyValue(object value)
        {
            if (value == null)
                return null;

            if (value is DateTime)
                return ((DateTime)value).ToString("s");

            if (value is byte[])
                return Convert.ToBase64String((byte[])value);

            return value.ToString();
        }

        #endregion

        // todo: snapshot log
        //private List<EntitySnapshotLog> GenerateEntitySnapshotLog()
        //{
        //    return null;
        //}
    }
}
