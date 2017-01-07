namespace DbConfigurationSample.Models
{
    public class Account
    {
        public int Id { get; set; }
        public byte[] RowVersion { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string BankName { get; set; }
        public string AccountNumber { get; set; }

        public string Comment { get; set; }
    }
}
