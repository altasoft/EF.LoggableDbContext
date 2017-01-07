using DbConfigurationSample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DbConfigurationSample
{
    class Program
    {
        private const int UserId = 1;

        static void Main(string[] args)
        {
            using (var db = new MyDbContext(UserId))
            {
                #region Create customer with accounts and ops
                var customer = new Customer()
                {
                    FirstName = "Indiana",
                    LastName = "Jones",
                    Age = 58,
                    Accounts = new List<Account>()
                    {
                        new Account()
                        {
                             BankName = "Bank of America", AccountNumber = "1230000123", Comment = "Comment for Bank of America"
                        },
                        new Account()
                        {
                            BankName = "PNC Bank", AccountNumber = "4560000456", Comment = "Comment for PNC Bank"
                        }
                    }
                };

                var customerOpCreate = new CustomerOp()
                {
                    Customer = customer,
                    Date = DateTime.Now,
                    Type = 1,
                    UserId = UserId,
                    Comment = "Create customer Indiana Jones"
                };

                db.Customers.Add(customer);
                db.CustomerOps.Add(customerOpCreate);
                db.SaveChanges();

                #endregion

                #region Update Customer, add ops

                customer.FirstName = "Harrison";
                customer.LastName = "Ford";

                var customerOpUpdate = new CustomerOp()
                {
                    Customer = customer,
                    Date = DateTime.Now,
                    Type = 2,
                    UserId = UserId,
                    Comment = "Update customer Indiana Jones => Harrison Ford"
                };
                db.CustomerOps.Add(customerOpUpdate);

                db.SaveChanges();

                #endregion

                #region Update customer account

                var account = customer.Accounts.First();
                account.BankName = "SunTrust Bank";
                account.AccountNumber = "7890000789";
                account.Comment = "Comment for SunTrust Bank";
                db.SaveChanges();

                #endregion

                var log = db.EntityPropertiesLog.Include(x => x.EntityLog.DatabaseLog).ToList();
                foreach (var item in log)
                {
                    Console.WriteLine($@"Log: Id={item.EntityLog.DatabaseLog.Id}, {item.EntityLog.DatabaseLog.Timestamp}, 
                    TypeName={item.EntityLog.TypeName} [PropertyName={item.PropertyName}, OldValue={item.OldValue}, NewValue={item.NewValue}]");
                }

                Console.ReadLine();
            }
        }
    }
}
