using System;
using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Configuration
{
    public class LoggableEntityPropertyIncludeConfiguration<TEntity> : LoggableEntityConfigurationBase<TEntity>
        where TEntity : class
    {
        public LoggableEntityPropertyIncludeConfiguration(Dictionary<string, LogModelConfiguration> config)
            : base(config) { }

        public void Include<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector)
        {
            var propertiesFromLambda = propertySelector.GetComplexPropertyAccessList().ToFullPropertyPath().ToList();

            if (Config[TypeName].Properties == null)
                Config[TypeName].Properties = new Dictionary<string, bool>(propertiesFromLambda.Count);

            propertiesFromLambda.ForEach(x => { Config[TypeName].Properties[x] = true; });
        }
    }
}
