namespace System.Data.Entity.Logging
{
    public class EntityPropertiesLog
    {
        public int Id { get; set; }
        public string PropertyName { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public int EntityLogId { get; set; }
        public EntityLog EntityLog { get; set; }
    }
}
