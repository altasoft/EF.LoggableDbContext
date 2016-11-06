namespace System.Data.Entity.Logging
{
    public class EntityLog
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string TypeFullName { get; set; }
        public bool HasMultipleKey { get; set; }
        public string EntityKey { get; set; }
        public string EntityKeyValue { get; set; }

        public int DatabaseLogId { get; set; }
        public DatabaseLog DatabaseLog { get; set; }
    }
}
