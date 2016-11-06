namespace System.Data.Entity.Logging
{
    public class EntitySnapshotLog
    {
        public int Id { get; set; }
        public string XmlValue { get; set; }

        public int EntityLogId { get; set; }
        public EntityLog EntityLog { get; set; }
    }
}
