using System.Collections.Generic;

namespace System.Data.Entity.Logging
{
    public class DatabaseLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public int UserId { get; set; }

        public List<EntityLog> EntityLog { get; set; }
    }
}
