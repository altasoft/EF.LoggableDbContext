using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Configuration
{
    public static class LogModelExtensions
    {
        public static IEnumerable<string> ToFullPropertyPath(this IEnumerable<PropertyPath> collection)
        {
            foreach (var item in collection)
            {
                yield return string.Join(".", item.Select(x => x.Name));
            }
        }
    }
}
