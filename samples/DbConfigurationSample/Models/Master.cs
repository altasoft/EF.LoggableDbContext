using System.Collections.Generic;

namespace DbConfigurationSample.Models
{
    public class Master
    {
        public int Id { get; set; }
        public byte[] RowVersion { get; set; }
        public string Name { get; set; }

        public List<Detail> Details { get; set; }
    }
}
