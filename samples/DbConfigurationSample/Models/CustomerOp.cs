using System;

namespace DbConfigurationSample.Models
{
    public class CustomerOp
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public byte Type { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }

        public int UserId { get; set; }
    }
}
