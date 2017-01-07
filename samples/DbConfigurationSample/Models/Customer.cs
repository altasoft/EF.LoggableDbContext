using System.Collections.Generic;

namespace DbConfigurationSample.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public byte[] RowVersion { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public List<Account> Accounts { get; set; }
    }
}
