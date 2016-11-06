using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Utilities
{
    public static class PropertyInfoExtensions
    {
        public static bool IsSameAs(this PropertyInfo propertyInfo, PropertyInfo otherPropertyInfo)
        {
            return (propertyInfo == otherPropertyInfo) ||
                   (propertyInfo.Name == otherPropertyInfo.Name
                    && (propertyInfo.DeclaringType == otherPropertyInfo.DeclaringType
                        || propertyInfo.DeclaringType.GetTypeInfo().IsSubclassOf(otherPropertyInfo.DeclaringType)
                        || otherPropertyInfo.DeclaringType.GetTypeInfo().IsSubclassOf(propertyInfo.DeclaringType)
                        || propertyInfo.DeclaringType.GetInterfaces().Contains(otherPropertyInfo.DeclaringType)
                        || otherPropertyInfo.DeclaringType.GetInterfaces().Contains(propertyInfo.DeclaringType)));
        }

        public static bool ContainsSame(this IEnumerable<PropertyInfo> enumerable, PropertyInfo propertyInfo)
        {
            return enumerable.Any(propertyInfo.IsSameAs);
        }
    }
}
