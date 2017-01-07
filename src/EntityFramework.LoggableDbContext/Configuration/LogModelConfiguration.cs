using System.Collections.Generic;

namespace System.Data.Entity.Configuration
{
    public class LogModelConfiguration
    {
        /// <summary>
        /// Is whole entity loggable
        /// </summary>
        public bool IsLoggable { get; set; }

        public bool IgnoreAllProperties { get; set; }

        public bool IsSnapshot { get; set; }

        /// <summary>
        /// Key - Property FullName (with nested properties)
        /// Value - Log this property or not
        /// </summary>
        public Dictionary<string, bool> Properties { get; set; }

        public LogModelConfiguration()
        {
            IsLoggable = true;
            IgnoreAllProperties = false;
        }
    }
}
