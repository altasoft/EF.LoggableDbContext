using System.Collections.Generic;

namespace System.Data.Entity.Configuration
{
    public class LogModelConfiguration
    {
        /// <summary>
        /// Is whole entity logable
        /// </summary>
        public bool IsLogable { get; set; }

        public bool IgnoreAllProperties { get; set; }

        public bool IsSnapshot { get; set; }

        /// <summary>
        /// Key - Property FullName (with nested properties)
        /// Value - Log this property or not
        /// </summary>
        public Dictionary<string, bool> Properties { get; set; }

        public LogModelConfiguration()
        {
            IsLogable = true;
            IgnoreAllProperties = false;
        }
    }
}
